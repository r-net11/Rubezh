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
    public class JournalWatcher
    {
        GkDatabase GkDatabase;
        int LastId;
        int LastDBNo;

        public JournalWatcher(GkDatabase gkDatabase)
        {
            GkDatabase = gkDatabase;
            var gkIpAddress = XManager.GetIpAddress(gkDatabase.RootDevice);
            LastDBNo = GKDBHelper.GetLastGKID(gkIpAddress);
        }

        public void Start()
        {
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
            var internalJournalItem = new InternalJournalItem(GkDatabase.RootDevice, sendResult.Bytes);
            return internalJournalItem.GKNo;
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
                var internalJournalItem = new InternalJournalItem(GkDatabase.RootDevice, sendResult.Bytes);
                var journalItem = internalJournalItem.ToJournalItem();
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
                        ApplicationService.Invoke(() =>
                        {
                            StatesWatcher.SetObjectStates(binaryObject.BinaryBase, XStatesHelper.StatesFromInt(journalItem.ObjectState));
                            ServiceFactory.Events.GetEvent<GKObjectsStateChangedEvent>().Publish(null);
                        });
                    }
                }
            }
            if (journalItems.Count > 0)
            {
                foreach (var journalItem in journalItems)
                {
                    if (journalItem.GKJournalRecordNo > LastDBNo)
                    {
                        GKDBHelper.Add(journalItem);
                        LastDBNo = journalItem.GKJournalRecordNo.Value;
                    }
                }
                ApplicationService.Invoke(() => { ServiceFactory.Events.GetEvent<NewXJournalEvent>().Publish(journalItems); });
            }
        }

        public void GetLastJournalItems(int count)
        {
            var lastId = GetLastId();
            ReadAndPublish(Math.Max(0, lastId - count), lastId);
        }

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
                Logger.Error("JournalWatcher ConnectionChanged gkDevice = null");
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