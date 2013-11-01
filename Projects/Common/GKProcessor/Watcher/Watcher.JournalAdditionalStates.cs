using System;
using System.Collections.Generic;
using System.Linq;
using Common.GK;
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
        void ParseAdditionalStates(JournalItem journalItem)
        {
            var descriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GetDescriptorNo() == journalItem.GKObjectNo);
            if (descriptor != null && descriptor.Device != null)
            {
                var deviceState = descriptor.Device.DeviceState;
                if (journalItem.Name == "Неисправность")
                {
                    switch (journalItem.YesNo)
                    {
                        case JournalYesNoType.Yes:
                            if (!string.IsNullOrEmpty(journalItem.Description))
                            {
                                if (!deviceState.AdditionalStates.Any(x => x.Name == journalItem.Description))
                                {
                                    var additionalState = new XAdditionalState()
                                    {
                                        StateClass = XStateClass.Failure,
                                        Name = journalItem.Description
                                    };
                                    deviceState.AdditionalStates.Add(additionalState);
                                }
                            }
                            break;

                        case JournalYesNoType.No:
                            if (string.IsNullOrEmpty(journalItem.Description))
                            {
                                deviceState.AdditionalStates.Clear();
                            }
                            else
                            {
                                deviceState.AdditionalStates.RemoveAll(x => x.Name == journalItem.Description);
                            }
                            break;

                        case JournalYesNoType.Unknown:
                            break;
                    }

                    deviceState.OnStateChanged();
                }
            }
        }
    }
}