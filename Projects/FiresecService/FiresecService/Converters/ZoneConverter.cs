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
                    var zone = new Zone();
                    zone.Name = innerZone.name;
                    zone.No = innerZone.no;
                    zone.Description = innerZone.desc;
                    if (innerZone.param != null)
                    {
                        var zoneTypeParam = innerZone.param.FirstOrDefault(x => x.name == "ZoneType");
                        if (zoneTypeParam != null)
                            zone.ZoneType = zoneTypeParam.value;

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
                            zone.GuardZoneType = guardZoneTypeParam.value;
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
                var innerZone = new Firesec.CoreConfig.zoneType();
                innerZone.name = zone.Name;
                innerZone.idx = zone.No;
                innerZone.no = zone.No;
                if (!string.IsNullOrEmpty(zone.Description))
                    innerZone.desc = zone.Description;

                var zoneParams = new List<Firesec.CoreConfig.paramType>();

                if (!string.IsNullOrEmpty(zone.ZoneType))
                {
                    var ZoneTypeParam = new Firesec.CoreConfig.paramType();
                    ZoneTypeParam.name = "ZoneType";
                    ZoneTypeParam.type = "Int";
                    ZoneTypeParam.value = zone.ZoneType;
                    zoneParams.Add(ZoneTypeParam);
                }

                if (!string.IsNullOrEmpty(zone.DetectorCount))
                {
                    var DetectorCountParam = new Firesec.CoreConfig.paramType();
                    DetectorCountParam.name = "FireDeviceCount";
                    DetectorCountParam.type = "Int";
                    DetectorCountParam.value = zone.DetectorCount;
                    zoneParams.Add(DetectorCountParam);
                }

                if (!string.IsNullOrEmpty(zone.EvacuationTime))
                {
                    var EvacuationTimeZoneParam = new Firesec.CoreConfig.paramType();
                    EvacuationTimeZoneParam.name = "ExitTime";
                    EvacuationTimeZoneParam.type = "SmallInt";
                    EvacuationTimeZoneParam.value = zone.EvacuationTime;
                    zoneParams.Add(EvacuationTimeZoneParam);
                }

                if (!string.IsNullOrEmpty(zone.AutoSet))
                {
                    var AutoSetParam = new Firesec.CoreConfig.paramType();
                    AutoSetParam.name = "AutoSet";
                    AutoSetParam.type = "Int";
                    AutoSetParam.value = zone.AutoSet;
                    zoneParams.Add(AutoSetParam);
                }

                if (!string.IsNullOrEmpty(zone.Delay))
                {
                    var DelaytParam = new Firesec.CoreConfig.paramType();
                    DelaytParam.name = "Delay";
                    DelaytParam.type = "Int";
                    DelaytParam.value = zone.Delay;
                    zoneParams.Add(DelaytParam);
                }

                if (true)
                {
                    var GuardZoneTypeParam = new Firesec.CoreConfig.paramType();
                    GuardZoneTypeParam.name = "Skipped";
                    GuardZoneTypeParam.type = "Int";
                    GuardZoneTypeParam.value = zone.Skipped ? "1" : "0";
                    zoneParams.Add(GuardZoneTypeParam);
                }

                if (!string.IsNullOrEmpty(zone.GuardZoneType))
                {
                    var GuardZoneTypeParam = new Firesec.CoreConfig.paramType();
                    GuardZoneTypeParam.name = "GuardZoneType";
                    GuardZoneTypeParam.type = "Int";
                    GuardZoneTypeParam.value = zone.GuardZoneType;
                    zoneParams.Add(GuardZoneTypeParam);
                }

                if (zoneParams.Count > 0)
                    innerZone.param = zoneParams.ToArray();

                innerZones.Add(innerZone);
            }

            if (innerZones.Count > 0)
                FiresecManager.CoreConfig.zone = innerZones.ToArray();
        }
    }
}