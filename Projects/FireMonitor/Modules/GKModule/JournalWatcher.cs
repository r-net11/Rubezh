using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using Commom.GK;
using FiresecClient;
using XFiresecAPI;
using Common.GK;
using Infrastructure.Common.Windows;
using GKModule.ViewModels;
using Infrastructure;
using GKModule.Events;

namespace GKModule
{
	public static class JournalWatcher
	{
		public static void Start()
		{
			LastId = GetLastId();
			Timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(100) };
			Timer.Tick += new EventHandler(Timer_Tick);
			Timer.Start();
		}

		static void Timer_Tick(object sender, EventArgs e)
		{
			var newLastId = GetLastId();
			if (newLastId > LastId)
			{
				StatesWatcher.Run();
				for (int i = LastId+1; i <= newLastId; i++)
				{
					ServiceFactory.Events.GetEvent<NewJournalEvent>().Publish(i);
				}
				LastId = newLastId;
			}
		}

		static int LastId;
		static DispatcherTimer Timer;

		public static int GetLastId()
		{
			var firstGkDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Driver.DriverType == XDriverType.GK);
			var bytes = SendManager.Send(firstGkDevice, 0, 6, 64);
			var journalItem = new JournalItem(bytes);
			var lastId = journalItem.GKNo;
			return lastId;
		}
	}
}