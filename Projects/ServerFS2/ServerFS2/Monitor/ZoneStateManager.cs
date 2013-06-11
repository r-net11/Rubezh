using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecAPI;
using ServerFS2.Service;
using FS2Api;

namespace ServerFS2.Monitor
{
	public static class ZoneStateManager
	{
		public static void ChangeOnDeviceState(Device device)
		{
			var changedZoneStates = new List<ZoneState>();
			foreach (var zone in ConfigurationManager.Zones)
			{
				var minStateType = StateType.Norm;
				foreach (var deviceInZone in zone.DevicesInZone)
				{
					if (deviceInZone.DeviceState.StateType < minStateType)
						minStateType = deviceInZone.DeviceState.StateType;
				}
				if (zone.ZoneState.StateType != minStateType)
				{
					zone.ZoneState.StateType = minStateType;
					changedZoneStates.Add(zone.ZoneState);
				}
			}

			CallbackManager.Add(new FS2Callbac() { ChangedZoneStates = changedZoneStates });
		}
	}
}