using System;
using System.Linq;
using Common;
using FiresecAPI.Models;

namespace FiresecClient
{
    public partial class FiresecManager
    {
        public static void SetZoneGuard(Zone zone)
        {
            try
            {
                if ((zone.ZoneType == ZoneType.Guard) && (zone.SecPanelUID != Guid.Empty))
                {
                    var localNo = FiresecConfiguration.GetZoneLocalSecNo(zone);
                    if (localNo > 0)
                    {
                        SetZoneGuard(zone.SecPanelUID, localNo);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "FiresecManager.SetZoneGuard");
            }
        }

        public static void UnSetZoneGuard(Zone zone)
        {
            try
            {
                if ((zone.ZoneType == ZoneType.Guard) && (zone.SecPanelUID != Guid.Empty))
                {
                    var localNo = FiresecConfiguration.GetZoneLocalSecNo(zone);
                    if (localNo > 0)
                    {
                        UnSetZoneGuard(zone.SecPanelUID, localNo);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "FiresecManager.UnSetZoneGuard");
            }
        }

        public static void SetDeviceGuard(Device device)
        {
            try
            {
                SetZoneGuard(device.UID, 0);
            }
            catch (Exception e)
            {
                Logger.Error(e, "FiresecManager.SetDeviceGuard");
            }
        }

        public static void UnSetDeviceGuard(Device device)
        {
            try
            {
                UnSetZoneGuard(device.UID, 0);
            }
            catch (Exception e)
            {
                Logger.Error(e, "FiresecManager.UnSetDeviceGuard");
            }
        }

        public static bool IsZoneOnGuard(ZoneState zoneState)
        {
            try
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
            catch (Exception e)
            {
                Logger.Error(e, "FiresecManager.IsZoneOnGuard");
                return false;
            }
        }

        public static bool IsZoneOnGuardAlarm(ZoneState zoneState)
        {
            try
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
            catch (Exception e)
            {
                Logger.Error(e, "FiresecManager.IsZoneOnGuardAlarm");
                return false;
            }
        }
    }
}