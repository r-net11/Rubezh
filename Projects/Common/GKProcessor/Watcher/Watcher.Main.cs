using Common;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.License;
using System;
using System.Linq;
using System.Threading;

namespace GKProcessor
{
	public partial class Watcher
	{
		public bool IsSuspending { get; private set; }
		AutoResetEvent SuspendingEvent = new AutoResetEvent(false);

		public bool IsStopping { get; private set; }
		AutoResetEvent StopEvent;
		Thread RunThread;
		public GkDatabase GkDatabase { get; private set; }
		public DateTime LastUpdateTime { get; private set; }
		public DateTime LastKAUMeasureTime { get; private set; }
		DateTime LastMissmatchCheckTime;
		GKCallbackResult GKCallbackResult { get; set; }
		bool IsHashFailure { get; set; }
        public bool UseReservedIp { get; set; }

		bool MustCheckTechnologicalRegime = false;
		DateTime LastTechnologicalRegimeCheckTime = DateTime.Now;
		int TechnologicalRegimeCheckCount = 0;
		bool HasLicense = true;
        public Watcher ReservedWatcher;

        public Watcher(GkDatabase gkDatabase, bool useReservedIp = false)
		{
			GkDatabase = gkDatabase;
			GKCallbackResult = new GKCallbackResult();
			IsStopping = false;
			IsSuspending = false;
            UseReservedIp = useReservedIp;
		}

		public void StartThread()
		{
			using (var gkLifecycleManager = new GKLifecycleManager(GkDatabase.RootDevice, "Запуск потока мониторинга"))
			{
				IsStopping = false;
				SetDescriptorsSuspending(false);
				if (RunThread == null)
				{
					StopEvent = new AutoResetEvent(false);
					RunThread = new Thread(OnRunThread);
					RunThread.Name = "GK Watcher " + GkDatabase.RootDevice.PresentationName;
					RunThread.Start();
				}
			}
		}

		public void StopThread()
		{
			using (var gkLifecycleManager = new GKLifecycleManager(GkDatabase.RootDevice, "Остановка потока мониторинга"))
			{
				IsStopping = true;
				if (StopEvent != null)
				{
					StopEvent.Set();
				}
				if (RunThread != null)
				{
					RunThread.Join(TimeSpan.FromSeconds(5));
				}
				RunThread = null;
				SetDescriptorsSuspending(true);
			}
		}

		public void Suspend()
		{
			using (var gkLifecycleManager = new GKLifecycleManager(GkDatabase.RootDevice, "Приостановка потока мониторинга"))
			{
				IsSuspending = true;
				if (StopEvent != null)
				{
					StopEvent.Set();
				}
				SetDescriptorsSuspending(true);
			}
		}

		public void Resume()
		{
			using (var gkLifecycleManager = new GKLifecycleManager(GkDatabase.RootDevice, "Возобновление потока мониторинга"))
			{
				IsSuspending = false;
				SuspendingEvent.Set();
				SetDescriptorsSuspending(false);
			}
		}

		bool ReturnAfterWait(int milliSeconds)
		{
			if (StopEvent != null)
			{
				StopEvent.WaitOne(TimeSpan.FromMilliseconds(milliSeconds));
			}
			WaitIfSuspending();
			return IsStopping;
		}

		void WaitIfSuspending()
		{
			if (IsSuspending)
			{
				SuspendingEvent.WaitOne(TimeSpan.FromMinutes(10));
			}
		}

		object CallbackResultLocker = new object();

		void SetDescriptorsSuspending(bool isSuspending)
		{
			Monitor.TryEnter(CallbackResultLocker, TimeSpan.FromSeconds(30));
			{
				GKCallbackResult = new GKCallbackResult();
				foreach (var descriptor in GkDatabase.Descriptors)
				{
					if (descriptor.GKBase.GetInternalState(UseReservedIp) != null)
					{
						descriptor.GKBase.GetInternalState(UseReservedIp).IsSuspending = isSuspending;
					}
				}
				NotifyAllObjectsStateChanged();
				OnGKCallbackResult(GKCallbackResult);
			}
			Monitor.Exit(CallbackResultLocker);
		}

		void OnRunThread()
		{
			while (true)
			{
				try
				{
					if (IsStopping)
						return;
					if (!InitializeMonitoring())
						return;
					if (IsStopping)
						return;
				}
				catch (Exception e)
				{
					AddMessage(JournalEventNameType.Ошибка_инициализации_мониторинга);
					Logger.Error(e, "JournalWatcher.InitializeMonitoring");
				}

				GKCallbackResult = new GKCallbackResult();
				var journalItem = new JournalItem()
					{
						SystemDateTime = DateTime.Now,
						DeviceDateTime = DateTime.Now,
						JournalObjectType = JournalObjectType.GKDevice,
						ObjectUID = GkDatabase.RootDevice.UID,
                        IsReserved = UseReservedIp,
						JournalEventNameType = JournalEventNameType.Начало_мониторинга,
					};
				AddJournalItem(journalItem);
				OnGKCallbackResult(GKCallbackResult);

				while (true)
				{
					if (IsStopping)
						return;

					Monitor.TryEnter(CallbackResultLocker, TimeSpan.FromSeconds(30));
					{
						GKCallbackResult = new GKCallbackResult();
					}
					Monitor.Exit(CallbackResultLocker);

					RunMonitoring();

					Monitor.TryEnter(CallbackResultLocker, TimeSpan.FromSeconds(30));
					{
						OnGKCallbackResult(GKCallbackResult);
					}
					Monitor.Exit(CallbackResultLocker);

					if (IsStopping)
						return;

					if (IsHashFailure)
						break;

					if (StopEvent != null)
					{
						var pollInterval = 10;
						var property = GkDatabase.RootDevice.Properties.FirstOrDefault(x => x.Name == "PollInterval");
						if (property != null)
						{
							pollInterval = property.Value;
						}
						if (ReturnAfterWait(pollInterval))
							break;
					}

					WaitIfSuspending();
					LastUpdateTime = DateTime.Now;
				}
			}
		}

        public bool IsPingFailure;
        public bool IsOtherPingFailure;

		bool InitializeMonitoring()
		{
			IsPingFailure = false;
			bool IsInTechnologicalRegime = false;
			bool IsGetStatesFailure = false;
			IsHashFailure = false;

			foreach (var descriptor in GkDatabase.Descriptors)
			{
				descriptor.GKBase.GetInternalState(UseReservedIp).Clear();
			}

			while (true)
			{
				LastUpdateTime = DateTime.Now;
				GKCallbackResult = new GKCallbackResult();
				foreach (var descriptor in GkDatabase.Descriptors)
				{
					descriptor.GKBase.GetInternalState(UseReservedIp).IsInitialState = true;
				}

				using (var gkLifecycleManager = new GKLifecycleManager(GkDatabase.RootDevice, "Проверка связи"))
				{
				    string deviceInfo = "";
                    if (ReservedWatcher != null)
                    {
                        deviceInfo = DeviceBytesHelper.GetDeviceInfo(GkDatabase.RootDevice, !UseReservedIp);
                        IsOtherPingFailure = string.IsNullOrEmpty(deviceInfo);
                        foreach (var descriptor in GkDatabase.Descriptors.FindAll(x => x.GetDescriptorNo() >= 0x19))
                        {
                            descriptor.GKBase.GetInternalState(UseReservedIp).IsConnectionLost = IsPingFailure && IsOtherPingFailure;
                            descriptor.GKBase.GetInternalState(UseReservedIp).IsInitialState = !(IsPingFailure && IsOtherPingFailure);
                        }
                    }

					deviceInfo = DeviceBytesHelper.GetDeviceInfo(GkDatabase.RootDevice, UseReservedIp);
					var pingResult = string.IsNullOrEmpty(deviceInfo);
					if (IsPingFailure != pingResult)
					{
						GKCallbackResult = new GKCallbackResult();
						IsPingFailure = pingResult;
						if (IsPingFailure)
							AddFailureJournalItem(JournalEventNameType.Нет_связи_с_ГК, JournalEventDescriptionType.Старт_мониторинга);
						else
							AddFailureJournalItem(JournalEventNameType.Связь_с_ГК_восстановлена, JournalEventDescriptionType.Старт_мониторинга);

                        foreach (var descriptor in GkDatabase.Descriptors.FindAll(x => x.GetDescriptorNo() < 0x19))
						{
							descriptor.GKBase.GetInternalState(UseReservedIp).IsConnectionLost = IsPingFailure;
							descriptor.GKBase.GetInternalState(UseReservedIp).IsInitialState = !IsPingFailure;
						}

					    NotifyAllObjectsStateChanged();
						OnGKCallbackResult(GKCallbackResult);
					}

					if (IsPingFailure)
					{
						gkLifecycleManager.SetError("Ошибка");
						if (ReturnAfterWait(5000))
							return false;
						continue;
					}
				}

				var result = CheckTechnologicalRegime();
				if (IsInTechnologicalRegime != result)
				{
					GKCallbackResult = new GKCallbackResult();
					IsInTechnologicalRegime = result;
					if (IsInTechnologicalRegime)
						AddFailureJournalItem(JournalEventNameType.ГК_в_технологическом_режиме, "Старт мониторинга");
					else
						AddFailureJournalItem(JournalEventNameType.ГК_в_рабочем_режиме, "Старт мониторинга");

					NotifyAllObjectsStateChanged();
					OnGKCallbackResult(GKCallbackResult);
				}

				if (IsInTechnologicalRegime)
				{
					GKLifecycleManager.Add(GkDatabase.RootDevice, "Устройство в технологическом режиме");
					if (ReturnAfterWait(5000))
						return false;
					continue;
				}

				using (var gkLifecycleManager = new GKLifecycleManager(GkDatabase.RootDevice, "Запрос хэша"))
				{
					var hashBytes = GKFileInfo.CreateHash1(GkDatabase.RootDevice);
					var gkFileReaderWriter = new GKFileReaderWriter();
					var gkFileInfo = gkFileReaderWriter.ReadInfoBlock(GkDatabase.RootDevice, UseReservedIp);
					result = false;
					if (IsHashFailure != result)
					{
						GKCallbackResult = new GKCallbackResult();
						IsHashFailure = false;
						if (IsHashFailure)
							AddFailureJournalItem(JournalEventNameType.Конфигурация_прибора_не_соответствует_конфигурации_ПК, JournalEventDescriptionType.Не_совпадает_хэш);
						else
							AddFailureJournalItem(JournalEventNameType.Конфигурация_прибора_соответствует_конфигурации_ПК, JournalEventDescriptionType.Совпадает_хэш);

						foreach (var descriptor in GkDatabase.Descriptors)
						{
							descriptor.GKBase.GetInternalState(UseReservedIp).IsDBMissmatch = IsHashFailure;
							descriptor.GKBase.GetInternalState(UseReservedIp).IsInitialState = false;
						}
						NotifyAllObjectsStateChanged();
						OnGKCallbackResult(GKCallbackResult);
					}

					if (IsHashFailure)
					{
						gkLifecycleManager.SetError(gkFileInfo == null ? "Ошибка" : "Не совпадает хэш");
						if (ReturnAfterWait(5000))
							return false;
						continue;
					}
				}

				GKCallbackResult = new GKCallbackResult();
				if (!ReadMissingJournalItems())
					AddFailureJournalItem(JournalEventNameType.Ошибка_при_синхронизации_журнала);
				OnGKCallbackResult(GKCallbackResult);

				GKCallbackResult = new GKCallbackResult();
				GetAllStates();
				result = IsDBMissmatchDuringMonitoring || !IsConnected;
				if (IsGetStatesFailure != result)
				{
					IsGetStatesFailure = result;
					if (IsGetStatesFailure)
						AddFailureJournalItem(JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК, DBMissmatchDuringMonitoringReason);
					else
						AddFailureJournalItem(JournalEventNameType.Устранена_ошибка_при_опросе_состояний_компонентов_ГК);
				}
				using (var gkLifecycleManager = new GKLifecycleManager(GkDatabase.RootDevice, "Передача состояний объектов"))
				{
					OnGKCallbackResult(GKCallbackResult);
				}

				if (IsGetStatesFailure)
				{
					GKLifecycleManager.Add(GkDatabase.RootDevice, "Ошибки при опросе состояний объектов");
					if (ReturnAfterWait(5000))
						return false;
					continue;
				}

				using (var gkLifecycleManager = new GKLifecycleManager(GkDatabase.RootDevice, "Передача состояний объектов"))
				{
					GKCallbackResult = new GKCallbackResult();
					foreach (var descriptor in GkDatabase.Descriptors)
					{
						descriptor.GKBase.GetInternalState(UseReservedIp).IsInitialState = false;
					}
					NotifyAllObjectsStateChanged();
					OnGKCallbackResult(GKCallbackResult);
				}

				return true;
			}
		}

		void RunMonitoring()
		{
			using (var gkLifecycleManager = new GKLifecycleManager(GkDatabase.RootDevice, "Цикл мониторинга"))
			{
				gkLifecycleManager.AddItem("Проверка лицензии");
				var hasLicense = LicenseManager.CurrentLicenseInfo.LicenseMode != LicenseMode.NoLicense;
				if (HasLicense != hasLicense)
				{
					HasLicense = hasLicense;
					foreach (var descriptor in GkDatabase.Descriptors)
					{
						descriptor.GKBase.GetInternalState(UseReservedIp).IsNoLicense = !HasLicense;
					}
					NotifyAllObjectsStateChanged();
				}
				if (!hasLicense)
					return;

				if (WatcherManager.IsConfigurationReloading)
				{
					if ((DateTime.Now - WatcherManager.LastConfigurationReloadingTime).TotalSeconds > 100)
						WatcherManager.IsConfigurationReloading = false;
				}
				if (WatcherManager.IsConfigurationReloading)
					return;

				if (IsDBMissmatchDuringMonitoring)
				{
					if ((DateTime.Now - LastMissmatchCheckTime).TotalSeconds > 60)
					{
						gkLifecycleManager.AddItem("Ошибка сопоставления конфигурации. Опрос объектов");
						GetAllStates();
						LastMissmatchCheckTime = DateTime.Now;
					}
					return;
				}

				try
				{
					if (MustCheckTechnologicalRegime)
					{
						if ((DateTime.Now - LastTechnologicalRegimeCheckTime).TotalSeconds > 10)
						{
							LastTechnologicalRegimeCheckTime = DateTime.Now;
							CheckTechnologicalRegime();
							NotifyAllObjectsStateChanged();

							TechnologicalRegimeCheckCount++;
							if (TechnologicalRegimeCheckCount >= 10)
								MustCheckTechnologicalRegime = false;
						}
					}
				}
				catch (Exception e)
				{
					Logger.Error(e, "Watcher.OnRunThread CheckTechnologicalRegime");
				}

				try
				{
					gkLifecycleManager.AddItem("Проверка задач");
					CheckTasks();
				}
				catch (Exception e)
				{
					Logger.Error(e, "Watcher.OnRunThread CheckTasks");
				}

				try
				{
					gkLifecycleManager.AddItem("Проверка задержек");
					CheckDelays();
				}
				catch (Exception e)
				{
					Logger.Error(e, "Watcher.OnRunThread CheckDelays");
				}

				try
				{
					PingJournal();
				}
				catch (Exception e)
				{
					Logger.Error(e, "Watcher.OnRunThread PingJournal");
				}

				try
				{
					PingNextState();
				}
				catch (Exception e)
				{
					Logger.Error(e, "Watcher.OnRunThread PingNextState");
				}

				try
				{
					gkLifecycleManager.AddItem("Проверка измерений");
					CheckMeasure();
				}
				catch (Exception e)
				{
					Logger.Error(e, "Watcher.OnRunThread CheckMeasure");
				}

				try
				{
					if ((DateTime.Now - LastKAUMeasureTime) > TimeSpan.FromHours(1))
					{
						LastKAUMeasureTime = DateTime.Now;
						gkLifecycleManager.AddItem("Измерение токопотребления");
						CheckKAUMeasure();
					}
				}
				catch (Exception e)
				{
					Logger.Error(e, "Watcher.OnRunThread CheckKAUMeasure");
				}
			}
		}

		void AddFailureJournalItem(JournalEventNameType journalEventNameType, string description = "")
		{
			var journalItem = new JournalItem()
			{
				JournalEventNameType = journalEventNameType,
				DescriptionText = description,
			};
			//var gkIpAddress = GkDatabase.RootDevice.GetGKIpAddress();
			//if (!string.IsNullOrEmpty(gkIpAddress))
			//    journalItem.JournalDetalisationItems.Add(new JournalDetalisationItem("IP-адрес ГК", gkIpAddress.ToString()));
			GKCallbackResult.JournalItems.Add(journalItem);
		}

		void AddFailureJournalItem(JournalEventNameType journalEventNameType, JournalEventDescriptionType description)
		{
			var journalItem = new JournalItem()
			{
				JournalEventNameType = journalEventNameType,
				JournalEventDescriptionType = description,
			};
			var gkIpAddress = GkDatabase.RootDevice.GetGKIpAddress();
			if (!string.IsNullOrEmpty(gkIpAddress))
				journalItem.JournalDetalisationItems.Add(new JournalDetalisationItem("IP-адрес ГК", gkIpAddress.ToString()));
		}

		void OnObjectStateChanged(GKBase gkBase, bool overrideExistingDeviceStates = true)
		{
            if (!IsPingFailure || gkBase.GKDescriptorNo < 0x19)
                AddObjectStateToGKStates(GKCallbackResult.GKStates, gkBase, overrideExistingDeviceStates, UseReservedIp);
		}

		public static void AddObjectStateToGKStates(GKStates gkStates, GKBase gkBase, bool overrideExistingDeviceStates = true, bool useReservedIp = false)
		{
			if (gkBase.State != null)
			{
                if (useReservedIp)
			    {
                    gkBase.ReservedState.IsReservedState = true;
                    gkBase.GetInternalState(true).UseReservedState = true;
                    gkBase.GetInternalState(true).CopyToGKState(gkBase.ReservedState);
			    }
			    else
                    gkBase.GetInternalState(false).CopyToGKState(gkBase.State);
			    if (gkBase is GKDevice)
			    {
			        if (overrideExistingDeviceStates)
					{
                        if (!useReservedIp)
                            gkStates.DeviceStates.RemoveAll(x => x.UID == gkBase.UID && x.IsReservedState == gkBase.State.IsReservedState);
                        else
                            gkStates.DeviceStates.RemoveAll(x => x.UID == gkBase.UID && x.IsReservedState == gkBase.ReservedState.IsReservedState);
                    }

			        gkStates.DeviceStates.Add(!useReservedIp ? gkBase.State : gkBase.ReservedState);
			    }
			    if (gkBase is GKZone)
				{
                    gkStates.ZoneStates.Add(!useReservedIp ? gkBase.State : gkBase.ReservedState);
				}
				if (gkBase is GKDirection)
				{
                    gkStates.DirectionStates.Add(!useReservedIp ? gkBase.State : gkBase.ReservedState);
				}
				if (gkBase is GKPumpStation)
				{
                    gkStates.PumpStationStates.Add(!useReservedIp ? gkBase.State : gkBase.ReservedState);
				}
				if (gkBase is GKMPT)
				{
                    gkStates.MPTStates.Add(!useReservedIp ? gkBase.State : gkBase.ReservedState);
				}
				if (gkBase is GKDelay)
				{
				    if (!useReservedIp)
				    {
				        gkBase.State.PresentationName = gkBase.PresentationName;
				        gkStates.DelayStates.Add(gkBase.State);
				    }
				    else
				    {
                        gkBase.ReservedState.PresentationName = gkBase.PresentationName;
                        gkStates.DelayStates.Add(gkBase.ReservedState);
				    }
				}
				if (gkBase is GKPim)
				{
				    if (!useReservedIp)
				    {
				        gkBase.State.PresentationName = gkBase.PresentationName;
				        gkStates.PimStates.Add(gkBase.State);
				    }
				    else
				    {
                        gkBase.ReservedState.PresentationName = gkBase.PresentationName;
                        gkStates.PimStates.Add(gkBase.ReservedState);
				    }
				}
				if (gkBase is GKGuardZone)
				{
                    gkStates.GuardZoneStates.Add(!useReservedIp ? gkBase.State : gkBase.ReservedState);
				}
				if (gkBase is GKDoor)
				{
                    gkStates.DoorStates.Add(!useReservedIp ? gkBase.State : gkBase.ReservedState);
				}
				if (gkBase is GKSKDZone)
				{
                    gkStates.SKDZoneStates.Add(!useReservedIp ? gkBase.State : gkBase.ReservedState);
				}
			}
		}

		void OnMeasureParametersChanged(GKDeviceMeasureParameters deviceMeasureParameters)
		{
			GKCallbackResult.GKStates.DeviceMeasureParameters.Add(deviceMeasureParameters);
		}

		internal void AddMessage(JournalEventNameType journalEventNameType)
		{
			var journalItem = new JournalItem()
			{
				JournalEventNameType = journalEventNameType,
				SystemDateTime = DateTime.Now,
			};
			AddJournalItem(journalItem);
		}

		void AddJournalItem(JournalItem journalItem)
		{
			GKCallbackResult.JournalItems.Add(journalItem);
		}

		void OnGKCallbackResult(GKCallbackResult gkCallbackResult)
		{
			GKProcessorManager.OnGKCallbackResult(GKCallbackResult);
		}
	}
}