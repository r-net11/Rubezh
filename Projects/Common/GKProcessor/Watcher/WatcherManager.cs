using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecClient;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using Infrastructure;

namespace GKProcessor
{
	internal class WatcherManager
	{
		public static List<Watcher> Watchers { get; private set; }
		public static bool IsConfigurationReloading { get; set; }
		public static DateTime LastConfigurationReloadingTime { get; set; }

		public static void Start()
		{
			Watchers = new List<Watcher>();
			foreach (var gkDatabase in DescriptorsManager.GkDatabases)
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

		public static void Stop()
		{
			try
			{
				if (Watchers != null)
				{
					foreach (var watcher in Watchers)
					{
						watcher.StopThread();
						TimeSynchronisationHelper.Stop();
						LifeTimeWatcher.Stop();
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "JournalWatcherManager.Stop");
			}
		}

		static void ApplicationService_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!e.Cancel)
			{
				Stop();
			}
		}
		static void ApplicationService_Restarting()
		{
			Stop();
		}

		public static void Send(Action<SendResult> onCompleted, XDevice gkParentDevice, ushort length, byte command, ushort inputLenght, List<byte> data = null, bool hasAnswer = true, bool sleepInsteadOfRecieve = false)
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