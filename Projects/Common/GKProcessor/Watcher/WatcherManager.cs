using Common;
using Infrastructure.Common.Windows;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using System;
using System.Collections.Generic;
using System.Linq;

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
				if (GKManager.IsValidIpAddress(gkDatabase.RootDevice))
				{
					var watcher = new Watcher(gkDatabase);
					Watchers.Add(watcher);
					watcher.StartThread();
				}
			}
			ApplicationService.Closing += new System.ComponentModel.CancelEventHandler(ApplicationService_Closing);
			TimeSynchronisationHelper.Start();
			LifeTimeWatcher.Start();
		}

		public static void Stop()
		{
			try
			{
				if (Watchers != null)
					foreach (var watcher in Watchers)
						watcher.StopThread();
				TimeSynchronisationHelper.Stop();
				LifeTimeWatcher.Stop();
				foreach (var progressCallback in GKProcessorManager.GKProgressCallbacks)
				{
					GKProcessorManager.StopProgress(progressCallback);
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
				Stop();
		}

		public static void Send(Action<SendResult> onCompleted, GKDevice gkParentDevice, ushort length, byte command, ushort inputLenght, List<byte> data = null, bool hasAnswer = true, bool sleepInsteadOfRecieve = false)
		{
			if (gkParentDevice != null)
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
					Logger.Error("WatcherManager.Send watcher = null " + gkParentDevice.UID.ToString());
				}
			}
			else
			{
				GKProcessorManager.AddGKMessage(JournalEventNameType.Ошибка_при_выполнении_команды_над_устройством, JournalEventDescriptionType.Не_найдено_родительское_устройство_ГК, "", null, null);
			}
		}
	}
}