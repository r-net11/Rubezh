using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace FiresecService.Converters
{
    public static class ZoneConverter
    {
        public static void Convert(Firesec.CoreConfig.config firesecConfig)
        {
            FiresecManager.Configuration.Zones = new List<Zone>();
            FiresecManager.States.ZoneStates = new List<ZoneState>();

            if (firesecConfig.zone != null)
            {
                foreach (var innerZone in firesecConfig.zone)
                {
                    Zone zone = new Zone();
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
                    FiresecManager.Configuration.Zones.Add(zone);
                    FiresecManager.States.ZoneStates.Add(new ZoneState() { No = zone.No });
                }
            }
        }

        public static void ConvertBack(CurrentConfiguration currentConfiguration)
        {
            List<Firesec.CoreConfig.zoneType> zones = new List<Firesec.CoreConfig.zoneType>();
            foreach (var zone in currentConfiguration.Zones)
            {
                Firesec.CoreConfig.zoneType firesecZone = new Firesec.CoreConfig.zoneType();
                firesecZone.name = zone.Name;
                firesecZone.idx = zone.No;
                firesecZone.no = zone.No;
                if (!string.IsNullOrEmpty(zone.Description))
                    firesecZone.desc = zone.Description;

                List<Firesec.CoreConfig.paramType> zoneParams = new List<Firesec.CoreConfig.paramType>();

                if (!string.IsNullOrEmpty(zone.ZoneType))
                {
                    Firesec.CoreConfig.paramType ZoneTypeParam = new Firesec.CoreConfig.paramType();
                    ZoneTypeParam.name = "ZoneType";
                    ZoneTypeParam.type = "Int";
                    ZoneTypeParam.value = zone.ZoneType;
                    zoneParams.Add(ZoneTypeParam);
                }

                if (!string.IsNullOrEmpty(zone.DetectorCount))
                {
                    Firesec.CoreConfig.paramType DetectorCountParam = new Firesec.CoreConfig.paramType();
                    DetectorCountParam.name = "FireDeviceCount";
                    DetectorCountParam.type = "Int";
                    DetectorCountParam.value = zone.DetectorCount;
                    zoneParams.Add(DetectorCountParam);
                }

                if (!string.IsNullOrEmpty(zone.EvacuationTime))
                {
                    Firesec.CoreConfig.paramType EvacuationTimeZoneParam = new Firesec.CoreConfig.paramType();
                    EvacuationTimeZoneParam.name = "ExitTime";
                    EvacuationTimeZoneParam.type = "SmallInt";
                    EvacuationTimeZoneParam.value = zone.EvacuationTime;
                    zoneParams.Add(EvacuationTimeZoneParam);
                }

                if (!string.IsNullOrEmpty(zone.AutoSet))
                {
                    Firesec.CoreConfig.paramType AutoSetParam = new Firesec.CoreConfig.paramType();
                    AutoSetParam.name = "AutoSet";
                    AutoSetParam.type = "Int";
                    AutoSetParam.value = zone.AutoSet;
                    zoneParams.Add(AutoSetParam);
                }

                if (!string.IsNullOrEmpty(zone.Delay))
                {
                    Firesec.CoreConfig.paramType DelaytParam = new Firesec.CoreConfig.paramType();
                    DelaytParam.name = "Delay";
                    DelaytParam.type = "Int";
                    DelaytParam.value = zone.Delay;
                    zoneParams.Add(DelaytParam);
                }

                if (true)
                {
                    Firesec.CoreConfig.paramType GuardZoneTypeParam = new Firesec.CoreConfig.paramType();
                    GuardZoneTypeParam.name = "Skipped";
                    GuardZoneTypeParam.type = "Int";
                    GuardZoneTypeParam.value = zone.Skipped ? "1" : "0";
                    zoneParams.Add(GuardZoneTypeParam);
                }

                if (!string.IsNullOrEmpty(zone.GuardZoneType))
                {
                    Firesec.CoreConfig.paramType GuardZoneTypeParam = new Firesec.CoreConfig.paramType();
                    GuardZoneTypeParam.name = "GuardZoneType";
                    GuardZoneTypeParam.type = "Int";
                    GuardZoneTypeParam.value = zone.GuardZoneType;
                    zoneParams.Add(GuardZoneTypeParam);
                }

                if (zoneParams.Count > 0)
                    firesecZone.param = zoneParams.ToArray();
            }

            if (zones.Count > 0)
                FiresecManager.CoreConfig.zone = zones.ToArray();
        }
    }
}
