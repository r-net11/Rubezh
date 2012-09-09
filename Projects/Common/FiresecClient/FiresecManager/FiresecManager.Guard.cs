using System;
using System.Linq;
using FiresecAPI.Models;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		public static void SetZoneGuard(Zone zone)
		{
			if ((zone.ZoneType == ZoneType.Guard) && (zone.SecPanelUID != Guid.Empty))
			{
				var localNo = FiresecConfiguration.GetZoneLocalSecNo(zone);
				if (localNo > 0)
				{
                    FiresecDriver.SetZoneGuard(zone.SecPanelUID, localNo);
				}
			}
		}

		public static void UnSetZoneGuard(Zone zone)
		{
			if ((zone.ZoneType == ZoneType.Guard) && (zone.SecPanelUID != Guid.Empty))
			{
				var localNo = FiresecConfiguration.GetZoneLocalSecNo(zone);
				if (localNo > 0)
				{
                    FiresecDriver.UnSetZoneGuard(zone.SecPanelUID, localNo);
				}
			}
		}

		public static void SetDeviceGuard(Device device)
		{
            FiresecDriver.SetZoneGuard(device.UID, 0);
		}

		public static void UnSetDeviceGuard(Device device)
		{
            FiresecDriver.UnSetZoneGuard(device.UID, 0);
		}

		public static bool IsZoneOnGuard(ZoneState zoneState)
		{
			if (zoneState.Zone.ZoneType == ZoneType.Fire)
			{
				return false;
			}
			foreach (var device in zoneState.Zone.DevicesInZone)
			{
				if (device.Driver.DriverType != DriverType.AM1_O)
					continue;

				var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == device.UID);
				if (deviceState.States.Count != 1)
					return false;
				if (deviceState.States.First().Code != "OnGuard")
					return false;
			}
			return true;
		}

		public static bool IsZoneOnGuardAlarm(ZoneState zoneState)
		{
			if (zoneState.Zone.ZoneType == ZoneType.Fire)
			{
				return false;
			}
			foreach (var device in zoneState.Zone.DevicesInZone)
			{
				if (device.Driver.DriverType != DriverType.AM1_O)
					continue;

				var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == device.UID);
				foreach (var state in deviceState.States)
				{
					if (state.Code.Contains("Alarm"))
						return true;
					if (state.Code == "InitFailed")
						return true;
				}
			}
			return false;
		}
	}
}