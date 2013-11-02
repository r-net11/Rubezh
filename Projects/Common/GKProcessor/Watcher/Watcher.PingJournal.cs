using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.XModels;
using GKProcessor.Events;
using Infrastructure;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using FiresecClient;
using Infrastructure.Common.Services;
using System.Diagnostics;
using System.Threading;

namespace GKProcessor
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
			var journalParser = new JournalParser(GkDatabase.RootDevice, sendResult.Bytes);
			return journalParser.JournalItem.GKJournalRecordNo.Value;
		}

		JournalItem ReadJournal(int index)
		{
			if (IsStopping)
				return null;
			var data = BitConverter.GetBytes(index).ToList();
			var sendResult = SendManager.Send(GkDatabase.RootDevice, 4, 7, 64, data);
			if (sendResult.HasError)
			{
				ConnectionChanged(false);
				return null;
			}
			if (sendResult.Bytes.Count == 64)
			{
				var journalParser = new JournalParser(GkDatabase.RootDevice, sendResult.Bytes);
				return journalParser.JournalItem;
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
                    var descriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GetDescriptorNo() == journalItem.GKObjectNo);
                    if (descriptor != null)
                    {
                        ChangeAM1TMessage(descriptor, journalItem);
                        CheckAdditionalStates(descriptor);
                        ApplicationService.Invoke(() =>
                        {
                            CheckServiceRequired(descriptor.XBase, journalItem);
                            descriptor.XBase.GetXBaseState().StateBits = XStatesHelper.StatesFromInt(journalItem.ObjectState);
                            ServiceFactoryBase.Events.GetEvent<GKObjectsStateChangedEvent>().Publish(null);
                        });
                    }
                }
            }
            if (journalItems.Count > 0)
            {
                GKDBHelper.AddMany(journalItems);
                ApplicationService.Invoke(() =>
                {
                    ServiceFactoryBase.Events.GetEvent<NewXJournalEvent>().Publish(journalItems);
                    foreach (var journalItem in journalItems)
                    {
                        ParseAdditionalStates(journalItem);
                    }
                });
            }
        }

		void ChangeAM1TMessage(BaseDescriptor descriptor, JournalItem journalItem)
		{
			if (descriptor.Device != null)
			{
				var device = descriptor.Device;
				if (device.DriverType == XDriverType.AM1_T)
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
			foreach (var direction in XManager.Directions)
			{
				bool mustGetState = false;
				switch (direction.DirectionState.StateClass)
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

			foreach (var device in XManager.Devices)
			{
				if (!device.Driver.IsGroupDevice && device.AllParents.Any(x=>x.DriverType == XDriverType.RSR2_KAU))
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
}