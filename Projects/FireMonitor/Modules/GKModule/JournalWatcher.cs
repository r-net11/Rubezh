using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using Common.GK;
using FiresecClient;
using XFiresecAPI;
using Infrastructure.Common.Windows;
using GKModule.ViewModels;
using Infrastructure;
using GKModule.Events;
using System.Diagnostics;

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
			if (LastId == -1)
				return;
			if (newLastId > LastId)
			{
				StatesWatcher.Run();
				for (int index = LastId + 1; index <= newLastId; index++)
				{
					ServiceFactory.Events.GetEvent<NewJournalEvent>().Publish(index);
				}
				LastId = newLastId;
			}
		}

		static int LastId;
		static DispatcherTimer Timer;

		public static int GetLastId()
		{
			var firstGkDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Driver.DriverType == XDriverType.GK);
			var sendResult = SendManager.Send(firstGkDevice, 0, 6, 64);
			if (sendResult.HasError)
			{
				MessageBoxService.Show("Ошибка связи с устройством");
				return - 1;
			}
			var journalItem = new JournalItem(sendResult.Bytes);
			var lastId = journalItem.GKNo;
			return lastId;
		}
	}
}