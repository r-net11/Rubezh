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

                if (device.DeviceState.ThreadSafeStates.Count == 1 && device.DeviceState.ThreadSafeStates.First().DriverState.Code == "OnGuard")
					return true;
			}
			return false;
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

                foreach (var state in device.DeviceState.ThreadSafeStates)
				{
					if (state.DriverState.Code.Contains("Alarm") || state.DriverState.Code.Contains("InitFailed"))
						return true;
				}
			}
			return false;
		}
	}
}