using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.GK;
using FiresecClient;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using Infrastructure;
using GKModule.Events;
using Infrastructure.Events;

namespace GKModule
{
	public static class WatcherManager
	{
		public static List<Watcher> Watchers { get; private set; }

		public static void Start()
		{
			Watchers = new List<Watcher>();
			foreach (var gkDatabase in DatabaseManager.GkDatabases)
			{
				if (!XManager.IsValidIpAddress(gkDatabase.RootDevice))
				{
					continue;
				}
				var watcher = new Watcher(gkDatabase);
				watcher.StartThread();
				Watchers.Add(watcher);
			}
			ApplicationService.Closing += new System.ComponentModel.CancelEventHandler(ApplicationService_Closing);
			ApplicationService.Restarting += new Action(ApplicationService_Restarting);
			TimeSynchronisationHelper.Start();
			LifeTimeWatcher.Start();
		}

		static void Stop()
		{
			try
			{
				foreach (var watcher in Watchers)
				{
					watcher.StopThread();
					TimeSynchronisationHelper.Stop();
					LifeTimeWatcher.Stop();
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

		public static void Send(Action<SendResult> onCompleted, SendPriority sendPriority, XDevice gkParentDevice, ushort length, byte command, ushort inputLenght, List<byte> data = null, bool hasAnswer = true, bool sleepInsteadOfRecieve = false)
		{
			var watcher = Watchers.FirstOrDefault(x => x.GkDatabase.RootDevice.UID == gkParentDevice.UID);
			if (watcher != null)
			{
				watcher.AddTask(() =>
				{
					var sendResult = SendManager.Send(gkParentDevice, length, command, inputLenght, data, hasAnswer, sleepInsteadOfRecieve);
					onCompleted(sendResult);
				});
			}
			else
			{
				Logger.Error("JournalWatcherManager.Send watcher = null " + gkParentDevice.UID.ToString());
			}
		}
	}
}