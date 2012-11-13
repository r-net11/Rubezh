using System.Collections.Generic;
using System.Threading;
using Common;
using Common.GK;
using Infrastructure.Common.Windows;
using System;
using Infrastructure;
using GKModule.Events;

namespace GKModule
{
    public static class JournalWatcherManager
    {
        public static SafeSendManager SafeSendManager { get; private set; }
        static Thread WorkThread;
        static AutoResetEvent StopThreadEvent;
        static List<JournalWatcher> JournalWatchers;

        public static void Start()
        {
			SafeSendManager = new Common.GK.SafeSendManager();

			var journalItems = GKDBHelper.GetTopLast(100);
			ApplicationService.Invoke(() => { ServiceFactory.Events.GetEvent<NewXJournalEvent>().Publish(journalItems); });

            JournalWatchers = new List<JournalWatcher>();
            foreach (var gkDatabase in DatabaseManager.GkDatabases)
            {
                var ipAddress = gkDatabase.RootDevice.GetGKIpAddress();
                if (string.IsNullOrEmpty(ipAddress))
                {
                    Logger.Error("JournalWatcherManager.Start ipAddress = null");
                    continue;
                }
                var journalWatcher = new JournalWatcher(gkDatabase);
                JournalWatchers.Add(journalWatcher);
            }

            StopThreadEvent = new AutoResetEvent(false);
            WorkThread = new Thread(new ThreadStart(OnRun));
            WorkThread.Start();
            ApplicationService.Closing += new System.ComponentModel.CancelEventHandler(ApplicationService_Closing);
            ApplicationService.Restarting += new Action(ApplicationService_Restarting);
        }

        public static void Stop()
        {
            try
            {
                foreach (var journalWatcher in JournalWatchers)
                {
                    journalWatcher.IsStopped = true;
                }
                SafeSendManager.StopThread();
                StopThreadEvent.Set();
                WorkThread.Join(TimeSpan.FromSeconds(1));
            }
            catch (Exception e)
            {
                Logger.Error(e, "JournalWatcherManager.Stop");
            }
        }

        static void ApplicationService_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Stop();
        }
        static void ApplicationService_Restarting()
        {
            Stop();
        }

        static void OnRun()
        {
            try
            {
                foreach (var journalWatcher in JournalWatchers)
                {
					journalWatcher.ReadMissingJournalItems();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "JournalWatcherManager.OnRun 1");
            }

            while (true)
            {
                try
                {
                    foreach (var journalWatcher in JournalWatchers)
                    {
                        journalWatcher.PingJournal();
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e, "JournalWatcherManager.OnRun 2");
                }
                if (StopThreadEvent.WaitOne(100))
                    break;
            }
        }
    }
}