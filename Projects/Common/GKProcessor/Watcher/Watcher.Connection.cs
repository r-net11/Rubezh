using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.GK;
using FiresecClient;
using GKProcessor.Events;
using Infrastructure;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using Infrastructure.Common.Services;

namespace GKProcessor
{
	public partial class Watcher
	{
		bool IsConnected = true;
		int ConnectionLostCount = 0;
		object connectionChangedLocker = new object();

		public void ConnectionChanged(bool isConnected)
		{
			lock (connectionChangedLocker)
			{
				if (!isConnected)
				{
					ConnectionLostCount++;
					if (ConnectionLostCount < 5)
						return;
				}
				else
					ConnectionLostCount = 0;

				if (IsConnected != isConnected)
				{
					var journalItem = new JournalItem()
					{
						SystemDateTime = DateTime.Now,
						DeviceDateTime = DateTime.Now,
						GKIpAddress = XManager.GetIpAddress(GkDatabase.RootDevice),
						ObjectUID = GkDatabase.RootDevice.UID,
						GKObjectNo = GkDatabase.RootDevice.GKDescriptorNo,
						JournalItemType = JournalItemType.GK,
						StateClass = XStateClass.Unknown,
						Name = isConnected ? "Восстановление связи с прибором" : "Потеря связи с прибором"
					};

					var journalItems = new List<JournalItem>() { journalItem };
					GKDBHelper.AddMany(journalItems);
					ApplicationService.Invoke(() => { ServiceFactoryBase.Events.GetEvent<NewXJournalEvent>().Publish(journalItems); });

					IsConnected = isConnected;
					if (isConnected)
					{
						GetAllStates(false);
					}
				}
                var gkDevice = XManager.Devices.FirstOrDefault(x => x.UID == GkDatabase.RootDevice.UID);
				if (gkDevice == null)
				{
					var uidEquality = XManager.Devices.Any(x => x.UID == GkDatabase.RootDevice.UID);
					Logger.Error("JournalWatcher ConnectionChanged gkDevice = null " + uidEquality.ToString() + " " + GkDatabase.RootDevice.UID.ToString());
					return;
				}

				foreach (var childDevice in XManager.GetAllDeviceChildren(gkDevice))
				{
					if (childDevice != null)
					{
						childDevice.DeviceState.IsConnectionLost = !isConnected;
					}
				}
				foreach (var zoneState in XManager.GetAllGKZoneStates(gkDevice.DeviceState))
				{
					zoneState.IsConnectionLost = !isConnected;
				}
				foreach (var directionState in XManager.GetAllGKDirectionStates(gkDevice.DeviceState))
				{
					directionState.IsConnectionLost = !isConnected;
				}

				if (ServiceFactoryBase.Events != null)
					ApplicationService.Invoke(() => { ServiceFactoryBase.Events.GetEvent<GKConnectionChangedEvent>().Publish(isConnected); });
			}
		}
	}
}