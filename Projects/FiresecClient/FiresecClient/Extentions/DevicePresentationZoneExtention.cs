using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace FiresecClient
{
    public static class DevicePresentationZoneExtention
    {
        public static string GetPersentationZone(this Device device)
        {
            if (device.Driver.IsZoneDevice)
            {
                Zone zone = FiresecManager.Configuration.Zones.FirstOrDefault(x => x.No == device.ZoneNo);
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
