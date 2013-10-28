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
			var binaryObject = GkDatabase.BinaryObjects.FirstOrDefault(x => x.GetNo() == journalItem.GKObjectNo);
			if (binaryObject != null && binaryObject.Device != null)
			{
				if (journalItem.Name == "Неисправность")
				{
					switch (journalItem.YesNo)
					{
						case JournalYesNoType.Yes:
							AddAdditionalState(binaryObject.Device, XStateClass.Failure, journalItem.Description);
							break;

						case JournalYesNoType.No:
							if (string.IsNullOrEmpty(journalItem.Description))
							{
								binaryObject.Device.DeviceState.AdditionalStates.Clear();
							}
							else
							{
								binaryObject.Device.DeviceState.AdditionalStates.RemoveAll(x => x.Name == journalItem.Description);
							}
							break;

						case JournalYesNoType.Unknown:
							break;
					}

					if (journalItem.YesNo != JournalYesNoType.No)
					{
						
					}
					else
					{
					}
				}
			}
		}

		void AddAdditionalState(XDevice devce, XStateClass stateClass, string name)
		{
			if (!devce.DeviceState.AdditionalStates.Any(x => x.Name == name))
			{
				var additionalState = new XAdditionalState()
				{
					StateClass = stateClass,
					Name = name
				};
				devce.DeviceState.AdditionalStates.Add(additionalState);
			}
		}

        void GetDeviceStateFromKAU(BinaryObjectBase binaryObjectBase)
        {
            return;
            var no = binaryObjectBase.BinaryBase.GetDatabaseNo(DatabaseType.Kau);
            if (no == 0)
                return;
            var sendResult = SendManager.Send(binaryObjectBase.BinaryBase.KauDatabaseParent, 2, 12, 32, BytesHelper.ShortToBytes(no));
            if (sendResult.HasError)
            {
                ConnectionChanged(false);
                return;
            }
            if (sendResult.Bytes.Count != 32)
            {
                IsAnyDBMissmatch = true;
                ApplicationService.Invoke(() => { binaryObjectBase.BinaryBase.GetXBaseState().IsGKMissmatch = true; });
                return;
            }
            ConnectionChanged(true);
            var binaryObjectStateHelper = new BinaryObjectStateHelper();
            binaryObjectStateHelper.ParseAdditionalParameters(sendResult.Bytes);
            ApplicationService.Invoke(() =>
            {
                var binaryState = binaryObjectBase.BinaryBase.GetXBaseState();
                binaryState.AdditionalStates = binaryObjectStateHelper.AdditionalStates;
                binaryState.LastDateTime = DateTime.Now;
            });
            Trace.WriteLine("binaryObjectStateHelper.AdditionalStates.Count = " + binaryObjectStateHelper.AdditionalStates.Count);
        }
	}
}