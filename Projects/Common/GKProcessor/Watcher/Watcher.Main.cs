using System;
using System.Linq;
using System.Threading;
using Common;
using Infrastructure.Common;
using XFiresecAPI;
using System.Collections.Generic;
using System.Diagnostics;
using FiresecClient;

namespace GKProcessor
{
	public partial class Watcher
	{
		bool IsStopping = false;
		AutoResetEvent StopEvent;
		Thread RunThread;
		public GkDatabase GkDatabase { get; private set; }
		public DateTime LastUpdateTime { get; private set; }
		DateTime LastMissmatchCheckTime;
		GKCallbackResult GKCallbackResult { get; set; }
		bool IsHashFailure { get; set; }

		public Watcher(GkDatabase gkDatabase)
		{
			GkDatabase = gkDatabase;
			GKCallbackResult = new GKCallbackResult();
		}

		public void StartThread()
		{
			IsStopping = false;
			if (RunThread == null)
			{
				StopEvent = new AutoResetEvent(false);
				RunThread = new Thread(OnRunThread);
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
		}

		void OnRunThread()
		{
			while (true)
			{
				try
				{
					if (!InitializeMonitoring() || IsStopping)
						return;
				}
				catch (Exception e)
				{
					AddMessage("Ошибка инициализации мониторинга", "");
					Logger.Error(e, "JournalWatcher.InitializeMonitoring");
				}

				while (true)
				{
					RunMonitoring();
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
						if (StopEvent.WaitOne(pollInterval))
							break;
					}

					LastUpdateTime = DateTime.Now;
				}
			}
		}

		bool InitializeMonitoring()
		{
			bool IsPingFailure = false;
			bool IsGetStatesFailure = false;
			IsHashFailure = false;

			while (true)
			{
				LastUpdateTime = DateTime.Now;
				foreach (var descriptor in GkDatabase.Descriptors)
				{
					descriptor.XBase.BaseState.Clear();
				}

				var deviceInfo = DeviceBytesHelper.GetDeviceInfo(GkDatabase.RootDevice);
				var result = string.IsNullOrEmpty(deviceInfo);
				if (IsPingFailure != result)
				{
					GKCallbackResult = new GKCallbackResult();
					IsPingFailure = result;
					if(IsPingFailure)
						AddFailureJournalItem("Нет связи с ГК", "Старт мониторинга");
					else
						AddFailureJournalItem("Связь с ГК восстановлена", "Старт мониторинга");

					foreach (var descriptor in GkDatabase.Descriptors)
					{
						descriptor.XBase.BaseState.IsConnectionLost = IsPingFailure;
					}
					NotifyAllObjectsStateChanged();
					GKProcessorManager.OnGKCallbackResult(GKCallbackResult);
				}

				if (IsPingFailure)
				{
					if (ReturnArterWait(5))
						return false;
					continue;
				}

				if (GlobalSettingsHelper.GlobalSettings.UseGKHash)
				{
					var hashBytes = GKFileInfo.CreateHash1(XManager.DeviceConfiguration, GkDatabase.RootDevice);
					var gkFileReaderWriter = new GKFileReaderWriter();
					var gkFileInfo = gkFileReaderWriter.ReadInfoBlock(GkDatabase.RootDevice);
					result = gkFileInfo == null || !GKFileInfo.CompareHashes(hashBytes, gkFileInfo.Hash1);
					if (IsHashFailure != result)
					{
						GKCallbackResult = new GKCallbackResult();
						IsHashFailure = result;
						if (IsHashFailure)
							AddFailureJournalItem("Конфигурация прибора не соответствует конфигурации ПК", "Не совпадает хэш");
						else
							AddFailureJournalItem("Конфигурация прибора соответствует конфигурации ПК", "Совпадает хэш");

						foreach (var descriptor in GkDatabase.Descriptors)
						{
							descriptor.XBase.BaseState.IsGKMissmatch = IsHashFailure;
						}
						NotifyAllObjectsStateChanged();
						GKProcessorManager.OnGKCallbackResult(GKCallbackResult);
					}
				}

				if (IsHashFailure)
				{
					if (ReturnArterWait(5))
						return false;
					continue;
				}

				GKCallbackResult = new GKCallbackResult();
				if (!ReadMissingJournalItems())
					AddFailureJournalItem("Ошибка при синхронизации журнала", "");
				GKProcessorManager.OnGKCallbackResult(GKCallbackResult);

				GKCallbackResult = new GKCallbackResult();
				result = !GetAllStates(true);
				if (IsGetStatesFailure != result)
				{
					IsGetStatesFailure = result;
					if (IsGetStatesFailure)
						AddFailureJournalItem("Ошибка при опросе состояний компонентов ГК", "");
					else
						AddFailureJournalItem("Устранена ошибка при опросе состояний компонентов ГК", "");
				}
				GKProcessorManager.OnGKCallbackResult(GKCallbackResult);

				if (IsGetStatesFailure)
				{
					if (ReturnArterWait(5))
						return false;
					continue;
				}

				return true;
			}
		}

		bool ReturnArterWait(int seconds)
		{
			if (StopEvent != null)
			{
				return StopEvent.WaitOne(TimeSpan.FromSeconds(seconds));
			}
			return false;
		}

		void RunMonitoring()
		{
			if (CheckLicense())
			{
				if (WatcherManager.IsConfigurationReloading)
				{
					if ((DateTime.Now - WatcherManager.LastConfigurationReloadingTime).TotalSeconds > 100)
						WatcherManager.IsConfigurationReloading = false;
				}
				if (!WatcherManager.IsConfigurationReloading)
				{
					if (IsAnyDBMissmatch)
					{
						if ((DateTime.Now - LastMissmatchCheckTime).TotalSeconds > 60)
						{
							GKCallbackResult = new GKCallbackResult();
							GetAllStates(false);
							LastMissmatchCheckTime = DateTime.Now;
							GKProcessorManager.OnGKCallbackResult(GKCallbackResult);
						}
					}
					else
					{
						GKCallbackResult = new GKCallbackResult();
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

						GKProcessorManager.OnGKCallbackResult(GKCallbackResult);
					}
				}
			}
		}

		void AddFailureJournalItem(string name, string description)
		{
			var journalItem = new JournalItem()
			{
				Name = name,
				Description = description,
				StateClass = XStateClass.Unknown,
				ObjectStateClass = XStateClass.Norm,
				GKIpAddress = GkDatabase.RootDevice.GetGKIpAddress()
			};
			GKDBHelper.Add(journalItem);
			GKCallbackResult.JournalItems.Add(journalItem);
		}

		void OnObjectStateChanged(XBase xBase)
		{
            xBase.BaseState.IsInitialState = false;
            xBase.State.StateClasses = xBase.BaseState.StateClasses.ToList();
			xBase.State.StateClass = xBase.BaseState.StateClass;
            xBase.State.AdditionalStates = xBase.BaseState.AdditionalStates.ToList();
            xBase.State.HoldDelay = xBase.BaseState.HoldDelay;
            xBase.State.OnDelay = xBase.BaseState.OnDelay;
            xBase.State.OffDelay = xBase.BaseState.OffDelay;
			if (xBase is XDevice)
			{
				GKCallbackResult.GKStates.DeviceStates.RemoveAll(x => x.UID == xBase.BaseUID);
				GKCallbackResult.GKStates.DeviceStates.Add(xBase.State);
			}
			if (xBase is XZone)
			{
				GKCallbackResult.GKStates.ZoneStates.Add(xBase.State);
			}
			if (xBase is XDirection)
			{
				GKCallbackResult.GKStates.DirectionStates.Add(xBase.State);
			}
			if (xBase is XPumpStation)
			{
				GKCallbackResult.GKStates.PumpStationStates.Add(xBase.State);
			}
			if (xBase is XDelay)
			{
				GKCallbackResult.GKStates.DelayStates.Add(xBase.State);
			}
			if (xBase is XPim)
			{
				GKCallbackResult.GKStates.PimStates.Add(xBase.State);
			}
		}

		void OnMeasureParametersChanged(XDeviceMeasureParameters deviceMeasureParameters)
		{
			GKCallbackResult.GKStates.DeviceMeasureParameters.Add(deviceMeasureParameters);
		}

		internal void AddMessage(string name, string userName)
		{
			var journalItem = GKDBHelper.AddMessage(name, userName);
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
	}
}