using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecClient;
using FiresecAPI.Events;
using System.Reflection;

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
		DateTime LastMissmatchCheckTime;
		GKCallbackResult GKCallbackResult { get; set; }
		bool IsHashFailure { get; set; }

		bool MustCheckTechnologicalRegime = false;
		DateTime LastTechnologicalRegimeCheckTime = DateTime.Now;
		int TechnologicalRegimeCheckCount = 0;

		bool HasLicense = true;

		public Watcher(GkDatabase gkDatabase)
		{
			GkDatabase = gkDatabase;
			GKCallbackResult = new GKCallbackResult();
			IsStopping = false;
			IsSuspending = false;
		}

		public void StartThread()
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

		public void StopThread()
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

		public void Suspend()
		{
			IsSuspending = true;
			if (StopEvent != null)
			{
				StopEvent.Set();
			}
			SetDescriptorsSuspending(true);
		}

		public void Resume()
		{
			IsSuspending = false;
			SuspendingEvent.Set();
			SetDescriptorsSuspending(false);
		}

		bool ReturnArterWait(int milliSeconds)
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
					if (descriptor.XBase.InternalState != null)
					{
						descriptor.XBase.InternalState.IsSuspending = isSuspending;
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
					AddMessage(GlobalEventNameEnum.Ошибка_инициализации_мониторинга, "");
					Logger.Error(e, "JournalWatcher.InitializeMonitoring");
				}

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
						if (ReturnArterWait(pollInterval))
							break;
					}

					WaitIfSuspending();
					LastUpdateTime = DateTime.Now;
				}
			}
		}

		bool InitializeMonitoring()
		{
			bool IsPingFailure = false;
			bool IsInTechnologicalRegime = false;
			bool IsGetStatesFailure = false;
			IsHashFailure = false;

			foreach (var descriptor in GkDatabase.Descriptors)
			{
				descriptor.XBase.InternalState.Clear();
			}

			while (true)
			{
				LastUpdateTime = DateTime.Now;
				GKCallbackResult = new GKCallbackResult();
				foreach (var descriptor in GkDatabase.Descriptors)
				{
					descriptor.XBase.InternalState.IsInitialState = true;
				}

				var deviceInfo = DeviceBytesHelper.GetDeviceInfo(GkDatabase.RootDevice);
				var result = string.IsNullOrEmpty(deviceInfo);
				if (IsPingFailure != result)
				{
					GKCallbackResult = new GKCallbackResult();
					IsPingFailure = result;
					if (IsPingFailure)
						AddFailureJournalItem(GlobalEventNameEnum.Нет_связи_с_ГК, EventDescription.Старт_мониторинга);
					else
						AddFailureJournalItem(GlobalEventNameEnum.Связь_с_ГК_восстановлена, EventDescription.Старт_мониторинга);

					foreach (var descriptor in GkDatabase.Descriptors)
					{
						descriptor.XBase.InternalState.IsConnectionLost = IsPingFailure;
						descriptor.XBase.InternalState.IsInitialState = !IsPingFailure;
					}
					NotifyAllObjectsStateChanged();
					OnGKCallbackResult(GKCallbackResult);
				}

				if (IsPingFailure)
				{
					if (ReturnArterWait(5000))
						return false;
					continue;
				}

				result = CheckTechnologicalRegime();
				if (IsInTechnologicalRegime != result)
				{
					GKCallbackResult = new GKCallbackResult();
					IsInTechnologicalRegime = result;
					if (IsInTechnologicalRegime)
						AddFailureJournalItem(GlobalEventNameEnum.ГК_в_технологическом_режиме, "Старт мониторинга");
					else
						AddFailureJournalItem(GlobalEventNameEnum.ГК_в_рабочем_режиме, "Старт мониторинга");

					NotifyAllObjectsStateChanged();
					OnGKCallbackResult(GKCallbackResult);
				}

				if (IsInTechnologicalRegime)
				{
					if (ReturnArterWait(5000))
						return false;
					continue;
				}

				var hashBytes = GKFileInfo.CreateHash1(XManager.DeviceConfiguration, GkDatabase.RootDevice);
				var gkFileReaderWriter = new GKFileReaderWriter();
				var gkFileInfo = gkFileReaderWriter.ReadInfoBlock(GkDatabase.RootDevice);
				result = gkFileInfo == null || !GKFileInfo.CompareHashes(hashBytes, gkFileInfo.Hash1);
				if (IsHashFailure != result)
				{
					GKCallbackResult = new GKCallbackResult();
					IsHashFailure = result;
					if (IsHashFailure)
						AddFailureJournalItem(GlobalEventNameEnum.Конфигурация_прибора_не_соответствует_конфигурации_ПК, EventDescription.Не_совпадает_хэш);
					else
						AddFailureJournalItem(GlobalEventNameEnum.Конфигурация_прибора_соответствует_конфигурации_ПК, EventDescription.Совпадает_хэш);

					foreach (var descriptor in GkDatabase.Descriptors)
					{
						descriptor.XBase.InternalState.IsDBMissmatch = IsHashFailure;
						descriptor.XBase.InternalState.IsInitialState = false;
					}
					NotifyAllObjectsStateChanged();
					OnGKCallbackResult(GKCallbackResult);
				}

				if (IsHashFailure)
				{
					if (ReturnArterWait(5000))
						return false;
					continue;
				}

				GKCallbackResult = new GKCallbackResult();
				if (!ReadMissingJournalItems())
					AddFailureJournalItem(GlobalEventNameEnum.Ошибка_при_синхронизации_журнала);
				OnGKCallbackResult(GKCallbackResult);

				GKCallbackResult = new GKCallbackResult();
				GetAllStates();
				result = IsDBMissmatchDuringMonitoring || !IsConnected;
				if (IsGetStatesFailure != result)
				{
					IsGetStatesFailure = result;
					if (IsGetStatesFailure)
						AddFailureJournalItem(GlobalEventNameEnum.Ошибка_при_опросе_состояний_компонентов_ГК, DBMissmatchDuringMonitoringReason);
					else
						AddFailureJournalItem(GlobalEventNameEnum.Устранена_ошибка_при_опросе_состояний_компонентов_ГК);
				}
				OnGKCallbackResult(GKCallbackResult);

				if (IsGetStatesFailure)
				{
					if (ReturnArterWait(5000))
						return false;
					continue;
				}

				GKCallbackResult = new GKCallbackResult();
				foreach (var descriptor in GkDatabase.Descriptors)
				{
					descriptor.XBase.InternalState.IsInitialState = false;
				}
				NotifyAllObjectsStateChanged();
				OnGKCallbackResult(GKCallbackResult);

				return true;
			}
		}

		void RunMonitoring()
		{
			var hasLicense = GKLicenseProcessor.HasLicense;
			if (HasLicense != hasLicense)
			{
				HasLicense = hasLicense;
				foreach (var descriptor in GkDatabase.Descriptors)
				{
					descriptor.XBase.InternalState.IsNoLicense = !HasLicense;
				}
				NotifyAllObjectsStateChanged();
			}
			if (!GKLicenseProcessor.HasLicense)
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
				CheckTasks();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Watcher.OnRunThread CheckTasks");
			}

			try
			{
				CheckDelays();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Watcher.OnRunThread CheckNPT");
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
				CheckMeasure();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Watcher.OnRunThread CheckMeasure");
			}
		}

		void AddFailureJournalItem(GlobalEventNameEnum globalEventNameEnum, string description = "")
		{
			var journalItem = new JournalItem()
			{
				GlobalEventNameType = globalEventNameEnum,
				Name = EventDescriptionAttributeHelper.ToName(globalEventNameEnum),
				Description = description,
				StateClass = XStateClass.Unknown,
				ObjectStateClass = XStateClass.Norm,
				GKIpAddress = GkDatabase.RootDevice.GetGKIpAddress()
			};
			GKDBHelper.Add(journalItem);
			GKCallbackResult.JournalItems.Add(journalItem);
		}

		void AddFailureJournalItem(GlobalEventNameEnum globalEventNameEnum, EventDescription description)
		{
			var journalItem = new JournalItem()
			{
				GlobalEventNameType = globalEventNameEnum,
				Name = EventDescriptionAttributeHelper.ToName(globalEventNameEnum),
				Description = description.ToDescription(),
				StateClass = XStateClass.Unknown,
				ObjectStateClass = XStateClass.Norm,
				GKIpAddress = GkDatabase.RootDevice.GetGKIpAddress()
			};
			GKDBHelper.Add(journalItem);
			GKCallbackResult.JournalItems.Add(journalItem);
		}

		void OnObjectStateChanged(XBase xBase)
		{
			AddObjectStateToGKStates(GKCallbackResult.GKStates, xBase);
		}

		public static void AddObjectStateToGKStates(GKStates gkStates, XBase xBase)
		{
			if (xBase.State != null)
			{
				xBase.InternalState.CopyToXState(xBase.State);
				if (xBase is XDevice)
				{
					gkStates.DeviceStates.RemoveAll(x => x.UID == xBase.BaseUID);
					gkStates.DeviceStates.Add(xBase.State);
				}
				if (xBase is XZone)
				{
					gkStates.ZoneStates.Add(xBase.State);
				}
				if (xBase is XDirection)
				{
					gkStates.DirectionStates.Add(xBase.State);
				}
				if (xBase is XPumpStation)
				{
					gkStates.PumpStationStates.Add(xBase.State);
				}
				if (xBase is XMPT)
				{
					gkStates.MPTStates.Add(xBase.State);
				}
				if (xBase is XDelay)
				{
					xBase.State.PresentationName = xBase.PresentationName;
					gkStates.DelayStates.Add(xBase.State);
				}
				if (xBase is XPim)
				{
					xBase.State.PresentationName = xBase.PresentationName;
					gkStates.PimStates.Add(xBase.State);
				}
			}
		}

		void OnMeasureParametersChanged(XDeviceMeasureParameters deviceMeasureParameters)
		{
			GKCallbackResult.GKStates.DeviceMeasureParameters.Add(deviceMeasureParameters);
		}

		internal void AddMessage(GlobalEventNameEnum globalEventNameEnum, string userName)
		{
			var journalItem = GKDBHelper.AddMessage(globalEventNameEnum, userName);
			GKCallbackResult.JournalItems.Add(journalItem);
		}

		void AddJournalItem(JournalItem journalItem)
		{
			GKDBHelper.Add(journalItem);
			GKCallbackResult.JournalItems.Add(journalItem);
		}

		void AddJournalItems(List<JournalItem> journalItems)
		{
			GKDBHelper.AddMany(journalItems);
			GKCallbackResult.JournalItems.AddRange(journalItems);
		}

		void OnGKCallbackResult(GKCallbackResult gkCallbackResult)
		{
			GKProcessorManager.OnGKCallbackResult(GKCallbackResult);
		}
	}
}