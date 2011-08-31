using System.Linq;
using FiresecAPI.Models;

namespace FiresecClient
{
    public static class DevicePresentationZoneExtention
    {
        public static string GetPersentationZone(this Device device)
        {
            if (device.Driver.IsZoneDevice)
            {
                Zone zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == device.ZoneNo);
                if (zone != null)
                {
                    return zone.PresentationName;
                }
                return "";
            }
            if (device.Driver.IsZoneLogicDevice)
            {
                return device.ZoneLogic.ToString();
            }
            return "";
        }
    }
}