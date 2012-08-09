using System.Linq;
using Common.GK;
using FiresecAPI.Models;
using FiresecClient;
using System.Diagnostics;
using System.Collections.Generic;
using Infrastructure.Common.Windows;
using Infrastructure;
using Infrastructure.Events;

namespace GKModule
{
	public static class StatesWatcher
	{
		public static void GetAllStates()
		{
			foreach (var gkDatabase in DatabaseManager.GkDatabases)
			{
				GetStatesFromDB(gkDatabase);
			}
		}

		static void GetStatesFromDB(CommonDatabase commonDatabase)
		{
			foreach (var binaryObject in commonDatabase.BinaryObjects)
			{
				var rootDevice = commonDatabase.RootDevice;
				var no = binaryObject.GetNo();
				var sendResult = SendManager.Send(rootDevice, 2, 12, 68, BytesHelper.ShortToBytes(no));
				if (sendResult.HasError)
				{
					ServiceFactory.Events.GetEvent<GKConnectionChanged>().Publish(false);
					//MessageBoxService.Show("Ошибка связи с устройством");
					return;
				}
				if (binaryObject.Device != null)
				{
					var deviceState = XManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == binaryObject.Device.UID);
					if (deviceState != null)
					{
						var binaryDeviceState = new BinaryDeviceState(sendResult.Bytes, binaryObject.DatabaseType);
						deviceState.States = binaryDeviceState.States;
						var minPriority = 7;
						foreach (var state in deviceState.States)
						{
							var priority = StatesHelper.XStateTypeToPriority(state);
							if (priority < minPriority)
							{
								minPriority = priority;
							}
						}
						deviceState.StateType = (StateType)minPriority;
						deviceState.OnStateChanged();
					}
				}
				if (binaryObject.Zone != null)
				{
					var zoneState = XManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == binaryObject.Zone.No);
					if (zoneState != null)
					{
						var binaryZoneState = new BinaryDeviceState(sendResult.Bytes, binaryObject.DatabaseType);
						zoneState.States = binaryZoneState.States;
						var minPriority = 7;
						foreach (var state in zoneState.States)
						{
							var priority = StatesHelper.XStateTypeToPriority(state);
							if (priority < minPriority)
							{
								minPriority = priority;
							}
						}
						zoneState.StateType = (StateType)minPriority;
						zoneState.OnStateChanged();
					}
				}
			}
		}
	}
}