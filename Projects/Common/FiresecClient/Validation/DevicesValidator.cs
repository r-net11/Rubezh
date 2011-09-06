using System;
using System.Collections.Generic;
using System.Linq;

namespace FiresecClient.Validation
{
    public static class DevicesValidator
    {
        public static List<ZoneError> ZoneErrors { get; set; }
        public static List<DeviceError> DeviceErrors { get; set; }

        public static void Validate()
        {
            ZoneErrors = new List<ZoneError>();
            DeviceErrors = new List<DeviceError>();

            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                if (device.Driver.IsZoneDevice)
                {
                    if (string.IsNullOrEmpty(device.ZoneNo))
                    {
                        var deviceError = new DeviceError(device, "Устройство должно относиться к зоне", ErrorLevel.Normal);
                        DeviceErrors.Add(deviceError);
                    }
                }
            }

            foreach (var zone in FiresecManager.DeviceConfiguration.Zones)
            {
                if (FiresecManager.DeviceConfiguration.Zones.Count(x => x.No == zone.No) > 1)
                {
                    var zoneError = new ZoneError(zone, "Зона с таким номером уже существует", ErrorLevel.Normal);
                    ZoneErrors.Add(zoneError);
                }

                if (int.Parse(zone.DetectorCount) < FiresecManager.DeviceConfiguration.Devices.Count(x => x.ZoneNo == zone.No))
                {
                    var zoneError = new ZoneError(zone, "Количество подключенных к зоне датчиков меньше количества датчиков для сработки", ErrorLevel.Normal);
                    ZoneErrors.Add(zoneError);
                }
            }
        }
    }
}