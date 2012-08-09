using System;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using Common.GK;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Events;
using XFiresecAPI;
using Infrastructure.Common.Windows;
using System.Collections.Generic;

namespace GKModule
{
	public static class JournalWatcher
	{
		static Thread thread;
		static int LastId;
		static AutoResetEvent StopEvent;

		public static void Start()
		{
			StopEvent = new AutoResetEvent(false);
			thread = new Thread(new ThreadStart(OnRun));
			thread.Start();
			ApplicationService.Closing += new System.ComponentModel.CancelEventHandler(ApplicationService_Closing);
		}

		static void ApplicationService_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			StopEvent.Set();
		}

		static void OnRun()
		{
			LastId = GetLastId();
			while (true)
			{
				try
				{
					PingJournal();
				}
				catch { }
				if (StopEvent.WaitOne(100))
					break;
			}
		}

		static void PingJournal()
		{
			var newLastId = GetLastId();
			if (newLastId == -1)
				return;
			if (LastId == -1)
				LastId = newLastId;
			if (newLastId > LastId)
			{
				StatesWatcher.GetAllStates();
				ReadAndPublish(LastId, newLastId);
				LastId = newLastId;
			}
		}

		static int GetLastId()
		{
			var firstGkDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Driver.DriverType == XDriverType.GK);
			var sendResult = SendManager.Send(firstGkDevice, 0, 6, 64);
			if (sendResult.HasError)
			{
				ConnectionChanged(false);
				return - 1;
			}
			ConnectionChanged(true);
			var journalItem = new JournalItem(sendResult.Bytes);
			return journalItem.GKNo;
		}

		static JournalItem ReadJournal(int index)
		{
			var firstGkDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Driver.DriverType == XDriverType.GK);
			var data = new List<byte>();
			data = BitConverter.GetBytes(index).ToList();
			var sendResult = SendManager.Send(firstGkDevice, 4, 7, 64, data);
			if (sendResult.HasError)
			{
				ConnectionChanged(false);
				return null;
			}
			if (sendResult.Bytes.Count == 64)
			{
				var journalItem = new JournalItem(sendResult.Bytes);
				return journalItem;
			}
			return null;
		}

		static void ReadAndPublish(int startIndex, int endIndex)
		{
			var journalItems = new List<JournalItem>();
			for (int index = startIndex; index <= endIndex; index++)
			{
				var journalItem = ReadJournal(index);
				if (journalItem != null)
				{
					journalItems.Add(journalItem);
				}
			}
			if (journalItems.Count > 0)
			{
				ApplicationService.Invoke(() => { ServiceFactory.Events.GetEvent<NewJournalEvent>().Publish(journalItems); });
			}
		}

		public static void GetLastJournalItems(int count)
		{
			var lastId = JournalWatcher.GetLastId();
			ReadAndPublish(Math.Max(0, lastId - count), lastId);
		}

		static void ConnectionChanged(bool value)
		{
			ApplicationService.Invoke(() => { ServiceFactory.Events.GetEvent<GKConnectionChanged>().Publish(value); });
		}
	}
}