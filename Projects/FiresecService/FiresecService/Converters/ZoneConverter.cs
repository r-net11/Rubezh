using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace FiresecService.Converters
{
    public static class ZoneConverter
    {
        public static void Convert(Firesec.CoreConfig.config firesecConfig)
        {
            FiresecManager.DeviceConfiguration.Zones = new List<Zone>();
            FiresecManager.DeviceConfigurationStates.ZoneStates = new List<ZoneState>();

            if (firesecConfig.zone != null)
            {
                foreach (var innerZone in firesecConfig.zone)
                {
                    var zone = new Zone()
                    {
                        Name = innerZone.name,
                        No = innerZone.no,
                        Description = innerZone.desc
                    };

                    if (innerZone.param != null)
                    {
                        var zoneTypeParam = innerZone.param.FirstOrDefault(x => x.name == "ZoneType");
                        if (zoneTypeParam != null)
                        {
                            zone.ZoneType = (zoneTypeParam.value == "0") ? ZoneType.Fire : ZoneType.Guard;
                        }

                        var exitTimeParam = innerZone.param.FirstOrDefault(x => x.name == "ExitTime");
                        if (exitTimeParam != null)
                            zone.EvacuationTime = exitTimeParam.value;

                        var fireDeviceCountParam = innerZone.param.FirstOrDefault(x => x.name == "FireDeviceCount");
                        if (fireDeviceCountParam != null)
                            zone.DetectorCount = fireDeviceCountParam.value;

                        var autoSetParam = innerZone.param.FirstOrDefault(x => x.name == "AutoSet");
                        if (autoSetParam != null)
                            zone.AutoSet = autoSetParam.value;

                        var delayParam = innerZone.param.FirstOrDefault(x => x.name == "Delay");
                        if (delayParam != null)
                            zone.Delay = delayParam.value;

                        var skippedParam = innerZone.param.FirstOrDefault(x => x.name == "Skipped");
                        if (skippedParam != null)
                            zone.Skipped = skippedParam.value == "1" ? true : false;

                        var guardZoneTypeParam = innerZone.param.FirstOrDefault(x => x.name == "GuardZoneType");
                        if (guardZoneTypeParam != null)
                        {
                            switch (guardZoneTypeParam.value)
                            {
                                case "0":
                                    zone.GuardZoneType = GuardZoneType.Ordinary;
                                    break;

                                case "1":
                                    zone.GuardZoneType = GuardZoneType.Passby;
                                    break;

                                case "2":
                                    zone.GuardZoneType = GuardZoneType.Delay;
                                    break;
                            }
                        }
                    }
                    FiresecManager.DeviceConfiguration.Zones.Add(zone);
                    FiresecManager.DeviceConfigurationStates.ZoneStates.Add(new ZoneState() { No = zone.No });
                }
            }
        }

        public static void ConvertBack(DeviceConfiguration deviceConfiguration)
        {
            var innerZones = new List<Firesec.CoreConfig.zoneType>();
            foreach (var zone in deviceConfiguration.Zones)
            {
                var innerZone = new Firesec.CoreConfig.zoneType()
                {
                    name = zone.Name,
                    idx = zone.No,
                    no = zone.No
                };

                if (string.IsNullOrEmpty(zone.Description) == false)
                    innerZone.desc = zone.Description;

                var zoneParams = new List<Firesec.CoreConfig.paramType>();

                zoneParams.Add(new Firesec.CoreConfig.paramType()
                {
                    name = "ZoneType",
                    type = "Int",
                    value = (zone.ZoneType == ZoneType.Fire) ? "0" : "1"
                });

                if (string.IsNullOrEmpty(zone.DetectorCount) == false)
                {
                    zoneParams.Add(new Firesec.CoreConfig.paramType()
                    {
                        name = "FireDeviceCount",
                        type = "Int",
                        value = zone.DetectorCount
                    });
                }

                if (string.IsNullOrEmpty(zone.EvacuationTime) == false)
                {
                    zoneParams.Add(new Firesec.CoreConfig.paramType()
                    {
                        name = "ExitTime",
                        type = "SmallInt",
                        value = zone.EvacuationTime
                    });
                }

                if (string.IsNullOrEmpty(zone.AutoSet) == false)
                {
                    zoneParams.Add(new Firesec.CoreConfig.paramType()
                    {
                        name = "AutoSet",
                        type = "Int",
                        value = zone.AutoSet
                    });
                }

                if (string.IsNullOrEmpty(zone.Delay) == false)
                {
                    zoneParams.Add(new Firesec.CoreConfig.paramType()
                    {
                        name = "Delay",
                        type = "Int",
                        value = zone.Delay
                    });
                }

                if (true)
                {
                    zoneParams.Add(new Firesec.CoreConfig.paramType()
                    {
                        name = "Skipped",
                        type = "Int",
                        value = zone.Skipped ? "1" : "0"
                    });
                }

                string GuardZoneTypeParamString = null;
                switch (zone.GuardZoneType)
                {
                    case GuardZoneType.Ordinary:
                        GuardZoneTypeParamString = "0";
                        break;

                    case GuardZoneType.Passby:
                        GuardZoneTypeParamString = "1";
                        break;

                    case GuardZoneType.Delay:
                        GuardZoneTypeParamString = "2";
                        break;
                }

                zoneParams.Add(new Firesec.CoreConfig.paramType()
                {
                    name = "GuardZoneType",
                    value = GuardZoneTypeParamString
                });

                if (zoneParams.Count > 0)
                    innerZone.param = zoneParams.ToArray();

                innerZones.Add(innerZone);
            }

            if (innerZones.Count > 0)
                FiresecManager.CoreConfig.zone = innerZones.ToArray();
        }
    }
}