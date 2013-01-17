using System;
using System.Collections.Generic;
using System.Linq;
using Common.GK;
using FiresecAPI.XModels;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Events;

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
                        ApplicationService.Invoke(() =>
                        {
							CheckServiceRequired(binaryObject.BinaryBase, journalItem);
                            SetObjectStates(binaryObject.BinaryBase, XStatesHelper.StatesFromInt(journalItem.ObjectState));
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
    }
}