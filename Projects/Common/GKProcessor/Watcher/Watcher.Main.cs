using System;
using System.Linq;
using System.Threading;
using Common;
using Infrastructure.Common;
using XFiresecAPI;
using System.Collections.Generic;
using System.Diagnostics;

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
            Trace.WriteLine("###### Before OnObjectStateChanged " + xBase.PresentationName + " ");
            foreach (var additionalState in xBase.BaseState.AdditionalStates)
            {
                Trace.WriteLine("\t" + additionalState.Name);
            }

            xBase.State.StateClasses = xBase.BaseState.StateClasses.ToList();
			xBase.State.StateClass = xBase.BaseState.StateClass;
            xBase.State.AdditionalStates = xBase.BaseState.AdditionalStates.ToList();
            xBase.State.HoldDelay = xBase.BaseState.HoldDelay;
            xBase.State.OnDelay = xBase.BaseState.OnDelay;
            xBase.State.OffDelay = xBase.BaseState.OffDelay;
            xBase.State.MeasureParameter = xBase.BaseState.MeasureParameter;
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

            Trace.WriteLine("###### Ater OnObjectStateChanged " + xBase.PresentationName + " ");
            foreach (var additionalState in xBase.State.AdditionalStates)
            {
                Trace.WriteLine("\t" + additionalState.Name);
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