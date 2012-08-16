using System;
using System.Collections.Generic;
using System.Linq;
using Common.GK;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Events;

namespace GKModule
{
	public class JournalWatcher
	{
		GkDatabase GkDatabase;
		int LastId;

		public JournalWatcher(GkDatabase gkDatabase)
		{
			GkDatabase = gkDatabase;
			LastId = GetLastId();
		}

		public void PingJournal()
		{
			var newLastId = GetLastId();
			if (newLastId == -1)
				return;
			if (LastId == -1)
				LastId = newLastId;
			if (newLastId > LastId)
			{
				ReadAndPublish(LastId, newLastId);
				LastId = newLastId;
			}
		}

		int GetLastId()
		{
			var sendResult = SendManager.Send(GkDatabase.RootDevice, 0, 6, 64);
			if (sendResult.HasError)
			{
				ConnectionChanged(false);
				return -1;
			}
			ConnectionChanged(true);
			var journalItem = new JournalItem(sendResult.Bytes);
			return journalItem.GKNo;
		}

		JournalItem ReadJournal(int index)
		{
			var data = BitConverter.GetBytes(index).ToList();
			var sendResult = SendManager.Send(GkDatabase.RootDevice, 4, 7, 64, data);
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

		void ReadAndPublish(int startIndex, int endIndex)
		{
			var journalItems = new List<JournalItem>();
			for (int index = startIndex; index <= endIndex; index++)
			{
				var journalItem = ReadJournal(index);
				if (journalItem != null)
				{
					journalItems.Add(journalItem);
					var binaryObject = GkDatabase.BinaryObjects.FirstOrDefault(x => x.GetNo() == journalItem.GKObjectNo);
					if (binaryObject != null)
					{
						StatesWatcher.SetObjectStates(binaryObject.BinaryBase, StatesHelper.StatesFromInt(journalItem.ObjectState));
						//StatesWatcher.RequestObjectState(binaryObject.BinaryBase);
					}
				}
			}
			if (journalItems.Count > 0)
			{
				ApplicationService.Invoke(() => { ServiceFactory.Events.GetEvent<NewJournalEvent>().Publish(journalItems); });
			}
		}

		public void GetLastJournalItems(int count)
		{
			var lastId = GetLastId();
			ReadAndPublish(Math.Max(0, lastId - count), lastId);
		}

		void ConnectionChanged(bool value)
		{
            if (!value)
            {
                var deviceState = XManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Device == GkDatabase.RootDevice);
                foreach (var childDeviceState in XManager.GetAllChildren(deviceState))
                {
                    childDeviceState.SetIsConnectionLost(true);
                }
            }
			ApplicationService.Invoke(() => { ServiceFactory.Events.GetEvent<GKConnectionChanged>().Publish(value); });
		}
	}
}