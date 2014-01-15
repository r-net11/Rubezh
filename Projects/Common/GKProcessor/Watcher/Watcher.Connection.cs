using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecClient;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using Infrastructure.Common;

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
					if (ConnectionLostCount < 3)
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
						JournalItemType = JournalItemType.System,
						StateClass = XStateClass.Unknown,
						ObjectStateClass = XStateClass.Norm,
						Name = isConnected ? "Восстановление связи с прибором" : "Потеря связи с прибором"
					};
					AddJournalItem(journalItem);

					IsConnected = isConnected;
					if (isConnected)
					{
						var hashBytes = GKFileInfo.CreateHash1(XManager.DeviceConfiguration, GkDatabase.RootDevice);
						var gkFileReaderWriter = new GKFileReaderWriter();
						var gkFileInfo = gkFileReaderWriter.ReadInfoBlock(GkDatabase.RootDevice);
						IsHashFailure = gkFileInfo == null || !GKFileInfo.CompareHashes(hashBytes, gkFileInfo.Hash1);
					}

					var gkDevice = XManager.Devices.FirstOrDefault(x => x.UID == GkDatabase.RootDevice.UID);
					foreach (var device in XManager.GetAllDeviceChildren(gkDevice))
					{
						device.InternalState.IsConnectionLost = !isConnected;
						OnObjectStateChanged(device);
					}
					foreach (var device in XManager.GetAllDeviceChildren(gkDevice))
					{
						if (device.Driver.IsGroupDevice || device.DriverType == XDriverType.KAU_Shleif || device.DriverType == XDriverType.RSR2_KAU_Shleif)
						{
							OnObjectStateChanged(device);
						}
					}
					foreach (var zone in XManager.Zones)
					{
						if (zone.GkDatabaseParent == gkDevice)
						{
							zone.InternalState.IsConnectionLost = !isConnected;
							OnObjectStateChanged(zone);
						}
					}
					foreach (var direction in XManager.Directions)
					{
						if (direction.GkDatabaseParent == gkDevice)
						{
							direction.InternalState.IsConnectionLost = !isConnected;
							OnObjectStateChanged(direction);
						}
					}
					foreach (var pumpStation in XManager.PumpStations)
					{
						if (pumpStation.GkDatabaseParent == gkDevice)
						{
							pumpStation.InternalState.IsConnectionLost = !isConnected;
							OnObjectStateChanged(pumpStation);
						}
					}
					foreach (var delay in XManager.Delays)
					{
						if (delay.GkDatabaseParent == gkDevice)
						{
							delay.InternalState.IsConnectionLost = !isConnected;
							OnObjectStateChanged(delay);
						}
					}
					foreach (var pim in XManager.Pims)
					{
						if (pim.GkDatabaseParent == gkDevice)
						{
							pim.InternalState.IsConnectionLost = !isConnected;
							OnObjectStateChanged(pim);
						}
					}
				}
			}
		}
	}
}