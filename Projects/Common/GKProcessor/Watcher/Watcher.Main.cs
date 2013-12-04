using System;
using System.Linq;
using System.Threading;
using Common;
using Infrastructure.Common;
using XFiresecAPI;
using System.Collections.Generic;

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
			try
			{
				GKCallbackResult = new GKCallbackResult();
				GetAllStates(true);
				if (!IsAnyDBMissmatch)
				{
					ReadMissingJournalItems();
				}
				GKProcessorManager.OnGKCallbackResult(GKCallbackResult);
			}
			catch (Exception e)
			{
				AddMessage("Ошибка мониторинга", "");
				Logger.Error(e, "JournalWatcher.OnRunThread GetAllStates");
			}

			while (true)
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
								Logger.Error(e, "JournalWatcher.OnRunThread CheckTasks");
							}

							try
							{
								CheckDelays();
							}
							catch (Exception e)
							{
								Logger.Error(e, "JournalWatcher.OnRunThread CheckNPT");
							}

							try
							{
								PingJournal();
							}
							catch (Exception e)
							{
								Logger.Error(e, "JournalWatcher.OnRunThread PingJournal");
							}

							try
							{
								PingNextState();
							}
							catch (Exception e)
							{
								Logger.Error(e, "JournalWatcher.OnRunThread PingNextState");
							}
							GKProcessorManager.OnGKCallbackResult(GKCallbackResult);
						}
					}
				}

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

		void OnObjectStateChanged(XBase xBase)
		{
			xBase.State.StateClasses = xBase.BaseState.StateClasses;
			xBase.State.StateClass = xBase.BaseState.StateClass;
			if (xBase is XDevice)
			{
				GKCallbackResult.DeviceStates.RemoveAll(x => x.UID == xBase.BaseUID);
				GKCallbackResult.DeviceStates.Add(xBase.State);
			}
			if (xBase is XZone)
			{
				GKCallbackResult.ZoneStates.Add(xBase.State);
			}
			if (xBase is XDirection)
			{
				GKCallbackResult.DirectionStates.Add(xBase.State);
			}
			if (xBase is XPumpStation)
			{
				GKCallbackResult.PumpStationStates.Add(xBase.State);
			}
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