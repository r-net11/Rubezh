using System.Collections.Generic;
using System.Threading;
using Common;
using Common.GK;
using Infrastructure.Common.Windows;
using System;
using System.Linq;
using Infrastructure;
using GKModule.Events;
using FiresecClient;
using XFiresecAPI;

namespace GKModule
{
    public static class WatcherManager
    {
        static List<Watcher> Watchers;

        public static void Start()
        {
			//var journalItems = GKDBHelper.GetTopLast(100);
			//ApplicationService.Invoke(() => { ServiceFactory.Events.GetEvent<NewXJournalEvent>().Publish(journalItems); });

            Watchers = new List<Watcher>();
            foreach (var gkDatabase in DatabaseManager.GkDatabases)
            {
				if (!XManager.IsValidIpAddress(gkDatabase.RootDevice))
                {
                    continue;
                }
                var watcher = new Watcher(gkDatabase);
				watcher.ReadMissingJournalItems();
				watcher.StartThread();
                Watchers.Add(watcher);
            }
            ApplicationService.Closing += new System.ComponentModel.CancelEventHandler(ApplicationService_Closing);
            ApplicationService.Restarting += new Action(ApplicationService_Restarting);
        }

        static void Stop()
        {
            try
            {
                foreach (var watcher in Watchers)
                {
					watcher.StopThread();
                }
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

        public static void Send(SendPriority sendPriority, XDevice gkParentDevice, ushort length, byte command, ushort inputLenght, List<byte> data = null, bool hasAnswer = true, bool sleepInsteadOfRecieve = false)
        {
			var watcher = Watchers.FirstOrDefault(x => x.GkDatabase.RootDevice.UID == gkParentDevice.UID);
			if (watcher != null)
			{
				watcher.AddTask(() =>
					{
						SendManager.Send(gkParentDevice, length, command, inputLenght, data, hasAnswer, sleepInsteadOfRecieve);
					});
			}
			else
			{
				Logger.Error("JournalWatcherManager.Send journalWatcher = null " + gkParentDevice.UID.ToString());
			}
        }
    }
}