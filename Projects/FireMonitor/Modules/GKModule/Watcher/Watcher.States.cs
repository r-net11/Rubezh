﻿using System.Collections.Generic;
using Common;
using Common.GK;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using XFiresecAPI;

namespace GKModule
{
    public partial class Watcher
	{
        void GetAllStates()
        {
			StartProgress("Опрос объектов ГК", GkDatabase.BinaryObjects.Count);
            foreach (var binaryObject in GkDatabase.BinaryObjects)
            {
                GetState(binaryObject.BinaryBase, GkDatabase.RootDevice);
				DoProgress("Опрос объекта ГК " + binaryObject.BinaryBase.BinaryInfo.ToString());
            }
            StopProgress();

            ApplicationService.Invoke(() => { ServiceFactory.Events.GetEvent<GKObjectsStateChangedEvent>().Publish(null); });
        }

		void GetState(XBinaryBase binaryBase, XDevice gkParent)
		{
			var no = binaryBase.GetDatabaseNo(DatabaseType.Gk);
			var sendResult = SendManager.Send(gkParent, 2, 12, 68, BytesHelper.ShortToBytes(no));
			if (sendResult.HasError)
			{
				ConnectionChanged(false);
				return;
			}
            ConnectionChanged(true);
			if (sendResult.Bytes.Count == 68)
			{
				var binaryObjectState = new BinaryObjectState(sendResult.Bytes);
				ApplicationService.Invoke(() => { SetObjectStates(binaryBase, binaryObjectState.States); });
			}
			else
			{
				Logger.Error("StatesWatcher.GetState sendResult.Bytes.Count != 68");
			}
		}

		void SetObjectStates(XBinaryBase binaryBase, List<XStateType> states)
		{
			if (binaryBase is XDevice)
			{
				var device = binaryBase as XDevice;
				device.DeviceState.States = states;
			}
			if (binaryBase is XZone)
			{
				var zone = binaryBase as XZone;
				zone.ZoneState.States = states;
			}
			if (binaryBase is XDirection)
			{
				var direction = binaryBase as XDirection;
				direction.DirectionState.States = states;
			}
		}

		void CheckServiceRequired(XBinaryBase binaryBase, JournalItem journalItem)
		{
			if(journalItem.Name != "Запыленность")
				return;

			if (binaryBase is XDevice)
			{
				var device = binaryBase as XDevice;
				bool isDusted = journalItem.YesNo;
				ApplicationService.Invoke(() => { device.DeviceState.IsService = isDusted; });
			}
		}
	}
}