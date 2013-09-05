using System;
using System.Collections.Generic;
using System.Linq;
using Common.GK;
using FiresecAPI.XModels;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using XFiresecAPI;
using FiresecClient;

namespace GKModule
{
	public partial class Watcher
	{
		int LastId = -1;

		void PingJournal()
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
			if (IsStopping)
				return -1;
			if (sendResult.HasError)
			{
				ConnectionChanged(false);
				return -1;
			}
			ConnectionChanged(true);
			var internalJournalItem = new InternalJournalItem(GkDatabase.RootDevice, sendResult.Bytes);
			return internalJournalItem.GKNo;
		}

		JournalItem ReadJournal(int index)
		{
			var data = BitConverter.GetBytes(index).ToList();
			var sendResult = SendManager.Send(GkDatabase.RootDevice, 4, 7, 64, data);
			if (IsStopping)
				return null;
			if (sendResult.HasError)
			{
				ConnectionChanged(false);
				return null;
			}
			if (sendResult.Bytes.Count == 64)
			{
				var internalJournalItem = new InternalJournalItem(GkDatabase.RootDevice, sendResult.Bytes);
				var journalItem = internalJournalItem.ToJournalItem();
				if (internalJournalItem != null && internalJournalItem.Device != null)
				{
					GetState(internalJournalItem.Device);
				}
				return journalItem;
			}
			return null;
		}

		void ReadAndPublish(int startIndex, int endIndex)
		{
			var journalItems = new List<JournalItem>();
			for (int index = startIndex + 1; index <= endIndex; index++)
			{
				var journalItem = ReadJournal(index);
				if (journalItem != null)
				{
					ApplicationService.Invoke(() =>
					{
						LoadingService.DoStep(journalItem.GKJournalRecordNo.ToString());
					});

					journalItems.Add(journalItem);
					var binaryObject = GkDatabase.BinaryObjects.FirstOrDefault(x => x.GetNo() == journalItem.GKObjectNo);
					if (binaryObject != null)
					{
						ChangeAM1TMessage(binaryObject, journalItem);
						CheckAdditionalStates(binaryObject);
						ApplicationService.Invoke(() =>
						{
							CheckServiceRequired(binaryObject.BinaryBase, journalItem);
							binaryObject.BinaryBase.GetXBaseState().StateBits = XStatesHelper.StatesFromInt(journalItem.ObjectState);
							ServiceFactory.Events.GetEvent<GKObjectsStateChangedEvent>().Publish(null);
						});
					}
				}
			}
			if (journalItems.Count > 0)
			{
				GKDBHelper.AddMany(journalItems);
				ApplicationService.Invoke(() => { ServiceFactory.Events.GetEvent<NewXJournalEvent>().Publish(journalItems); });
			}
		}

		void ChangeAM1TMessage(BinaryObjectBase binaryObjectBase, JournalItem journalItem)
		{
			if (binaryObjectBase.Device != null)
			{
				var device = binaryObjectBase.Device;
				if (device.Driver.DriverType == XDriverType.AM1_T)
				{
					if (journalItem.Name == "Пожар-1")
					{
						if (journalItem.YesNo == JournalYesNoType.Yes)
						{
							var property = device.Properties.FirstOrDefault(x => x.Name == "Сообщение для нормы");
							if (property != null)
							{
								journalItem.Name = property.StringValue;
							}
						}
						else if (journalItem.YesNo == JournalYesNoType.No)
						{
							var property = device.Properties.FirstOrDefault(x => x.Name == "Сообщение для сработки");
							if (property != null)
							{
								journalItem.Name = property.StringValue;
							}
						}
					}
				}
			}
		}

		void CheckDelays()
		{
			foreach (var direction in XManager.DeviceConfiguration.Directions)
			{
				bool mustGetState = false;
				switch(direction.DirectionState.StateClass)
				{
					case XStateClass.TurningOn:
						mustGetState = direction.DirectionState.OnDelay > 0 || (DateTime.Now - direction.DirectionState.LastDateTime).Seconds > 1;
						break;
					case XStateClass.On:
						mustGetState = direction.DirectionState.HoldDelay > 0 || (DateTime.Now - direction.DirectionState.LastDateTime).Seconds > 1;
						break;
					case XStateClass.TurningOff:
						mustGetState = direction.DirectionState.OffDelay > 0 || (DateTime.Now - direction.DirectionState.LastDateTime).Seconds > 1;
						break;
				}
				if (mustGetState)
				{
					GetState(direction);
				}
			}

			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				bool mustGetState = false;
				switch (device.DeviceState.StateClass)
				{
					case XStateClass.TurningOn:
						mustGetState = device.DeviceState.OnDelay > 0 || (DateTime.Now - device.DeviceState.LastDateTime).Seconds > 1;
						break;
					case XStateClass.On:
						mustGetState = device.DeviceState.HoldDelay > 0 || (DateTime.Now - device.DeviceState.LastDateTime).Seconds > 1;
						break;
					case XStateClass.TurningOff:
						mustGetState = device.DeviceState.OffDelay > 0 || (DateTime.Now - device.DeviceState.LastDateTime).Seconds > 1;
						break;
				}
				if (mustGetState)
				{
					GetState(device);
				}
			}
		}
	}
}