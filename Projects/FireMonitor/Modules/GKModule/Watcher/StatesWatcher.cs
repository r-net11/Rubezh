using System.Collections.Generic;
using System.Linq;
using Common.GK;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Events;
using XFiresecAPI;
using Infrastructure.Common.Windows;

namespace GKModule
{
	public static class StatesWatcher
	{
		public static void RequestAllStates()
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
			var binaryObjectState = new BinaryObjectState(sendResult.Bytes);
			SetObjectStates(binaryBase, binaryObjectState.States);
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
					deviceState.StateType = XStatesToState(states);
					deviceState.OnStateChanged();
				}
			}
			if (binaryBase is XZone)
			{
				var zone = binaryBase as XZone;
				var zoneState = XManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == zone.No);
				if (zoneState != null)
				{
					zoneState.States = states;
					zoneState.StateType = XStatesToState(states);
					zoneState.OnStateChanged();
				}
			}
		}

		static StateType XStatesToState(List<XStateType> states)
		{
			var minPriority = 7;
			foreach (var state in states)
			{
				var priority = StatesHelper.XStateTypeToPriority(state);
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