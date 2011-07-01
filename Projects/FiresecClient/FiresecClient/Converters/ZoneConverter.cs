using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient.Models;

namespace FiresecClient.Converters
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
                        var exitTimeParam = innerZone.param.FirstOrDefault(x => x.name == "ExitTime");
                        if (exitTimeParam != null)
                            zone.EvacuationTime = exitTimeParam.value;

                        var fireDeviceCountParam = innerZone.param.FirstOrDefault(x => x.name == "FireDeviceCount");
                        if (fireDeviceCountParam != null)
                            zone.DetectorCount = fireDeviceCountParam.value;
                    }
                    FiresecManager.Configuration.Zones.Add(zone);
                    FiresecManager.States.ZoneStates.Add(new ZoneState(zone.No));
                }
            }
        }

        public static void ConvertBack(CurrentConfiguration currentConfiguration)
        {
            List<Firesec.CoreConfig.zoneType> zones = new List<Firesec.CoreConfig.zoneType>();
            //FiresecManager.CoreConfig.zone = new Firesec.CoreConfig.zoneType[currentConfiguration.Zones.Count];
            foreach (var zone in currentConfiguration.Zones)
            //for (int i = 0; i < currentConfiguration.Zones.Count; i++)
            {
                Firesec.CoreConfig.zoneType firesecZone = new Firesec.CoreConfig.zoneType();
                firesecZone.name = zone.Name;
                firesecZone.idx = zone.No;
                firesecZone.no = zone.No;
                if (!string.IsNullOrEmpty(zone.Description))
                    firesecZone.desc = zone.Description;

                List<Firesec.CoreConfig.paramType> zoneParams = new List<Firesec.CoreConfig.paramType>();
                if (!string.IsNullOrEmpty(zone.DetectorCount))
                {
                    Firesec.CoreConfig.paramType DetectorCountZoneParam = new Firesec.CoreConfig.paramType();
                    DetectorCountZoneParam.name = "FireDeviceCount";
                    DetectorCountZoneParam.type = "Int";
                    DetectorCountZoneParam.value = zone.DetectorCount;
                    zoneParams.Add(DetectorCountZoneParam);
                }
                if (!string.IsNullOrEmpty(zone.EvacuationTime))
                {
                    Firesec.CoreConfig.paramType EvacuationTimeZoneParam = new Firesec.CoreConfig.paramType();
                    EvacuationTimeZoneParam.name = "ExitTime";
                    EvacuationTimeZoneParam.type = "SmallInt";
                    EvacuationTimeZoneParam.value = zone.EvacuationTime;
                    zoneParams.Add(EvacuationTimeZoneParam);
                }
                if (zoneParams.Count > 0)
                    firesecZone.param = zoneParams.ToArray();
            }

            if (zones.Count > 0)
                FiresecManager.CoreConfig.zone = zones.ToArray();
        }
    }
}
