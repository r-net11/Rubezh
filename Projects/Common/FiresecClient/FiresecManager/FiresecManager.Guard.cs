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

				if (device.DeviceState.States.Count != 1)
					return false;
				if (device.DeviceState.States.First().DriverState.Code != "OnGuard")
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

				foreach (var state in device.DeviceState.States)
				{
					if (state.DriverState.Code.Contains("Alarm"))
						return true;
					if (state.DriverState.Code == "InitFailed")
						return true;
				}
			}
			return false;
		}
	}
}