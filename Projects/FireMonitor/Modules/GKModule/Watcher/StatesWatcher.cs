using System.Collections.Generic;
using System.Linq;
using Common.GK;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using XFiresecAPI;
using System.Threading;
using FiresecAPI;
using FiresecAPI.XModels;
using Common;

namespace GKModule
{
	public static class StatesWatcher
	{
		public static void RequestAllStates()
		{
            var thread = new Thread(new ThreadStart(OnRun));
            thread.Start();
		}

        static void OnRun()
        {
            foreach (var gkDatabase in DatabaseManager.GkDatabases)
            {
                foreach (var binaryObject in gkDatabase.BinaryObjects)
                {
                    GetState(binaryObject.BinaryBase, gkDatabase.RootDevice);
                }
            }
        }

		public static void RequestObjectState(XBinaryBase binaryBase)
		{
			GetState(binaryBase, binaryBase.GkDatabaseParent);
		}

		static void GetState(XBinaryBase binaryBase, XDevice gkParent)
		{
			var no = binaryBase.GetDatabaseNo(DatabaseType.Gk);
			var sendResult = SendManager.Send(gkParent, 2, 12, 68, BytesHelper.ShortToBytes(no));
			if (sendResult.HasError)
			{
				ConnectionChanged(false);
				return;
			}
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

		public static void SetObjectStates(XBinaryBase binaryBase, List<XStateType> states)
		{
			if (binaryBase is XDevice)
			{
				var device = binaryBase as XDevice;
				var deviceState = XManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == device.UID);
				if (deviceState != null)
				{
					deviceState.States = states;
				}
			}
			if (binaryBase is XZone)
			{
				var zone = binaryBase as XZone;
				var zoneState = XManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.UID == zone.UID);
				if (zoneState != null)
				{
					zoneState.States = states;
				}
			}
		}

		static StateType XStatesToState(List<XStateType> states)
		{
			var minPriority = 7;
			foreach (var state in states)
			{
				var priority = XStatesHelper.XStateTypeToPriority(state);
				if (priority < minPriority)
				{
					minPriority = priority;
				}
			}
			StateType stateType = (StateType)minPriority;
			return stateType;
		}

		static void ConnectionChanged(bool value)
		{
			ApplicationService.Invoke(() => { ServiceFactory.Events.GetEvent<GKConnectionChanged>().Publish(value); });
		}
	}
}