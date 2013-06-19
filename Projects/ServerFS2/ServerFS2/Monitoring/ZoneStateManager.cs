using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Models;
using FS2Api;
using ServerFS2.Service;

namespace ServerFS2.Monitoring
{
	public static class ZoneStateManager
	{
		public static void ChangeOnDeviceState(bool isSilent = false)
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

			if(!isSilent)
				CallbackManager.Add(new FS2Callbac() { ChangedZoneStates = changedZoneStates });
		}

	}
}