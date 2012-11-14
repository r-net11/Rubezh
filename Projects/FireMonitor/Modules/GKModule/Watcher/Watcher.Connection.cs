using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.GK;
using FiresecAPI.XModels;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using XFiresecAPI;

namespace GKModule
{
    public partial class Watcher
    {
        bool IsConnected = true;

        void ConnectionChanged(bool isConnected)
        {
            if (IsConnected != isConnected)
            {
                var journalItem = new JournalItem()
                {
                    DateTime = DateTime.Now,
                    GKIpAddress = XManager.GetIpAddress(GkDatabase.RootDevice),
                    ObjectUID = GkDatabase.RootDevice.UID,
                    GKObjectNo = GkDatabase.RootDevice.GetDatabaseNo(DatabaseType.Gk),
                    JournalItemType = JournalItemType.GK,
                    StateClass = XStateClass.Unknown
                };
                if (isConnected)
                {
                    journalItem.Description = "Восстановление связи с прибором";
                }
                else
                {
                    journalItem.Description = "Потеря связи с прибором";
                }
                ApplicationService.Invoke(() => { ServiceFactory.Events.GetEvent<NewXJournalEvent>().Publish(new List<JournalItem>() { journalItem }); });
                GKDBHelper.Add(journalItem);
                IsConnected = isConnected;
            }
            var gkDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x == GkDatabase.RootDevice);
            if (gkDevice == null)
            {
                var uidEquality = XManager.DeviceConfiguration.Devices.Any(x => x.UID == GkDatabase.RootDevice.UID);
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

            ApplicationService.Invoke(() => { ServiceFactory.Events.GetEvent<GKConnectionChanged>().Publish(isConnected); });
        }
    }
}