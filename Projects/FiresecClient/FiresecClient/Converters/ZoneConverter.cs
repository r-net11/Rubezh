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
            FiresecManager.CoreConfig.zone = new Firesec.CoreConfig.zoneType[currentConfiguration.Zones.Count];
            for (int i = 0; i < currentConfiguration.Zones.Count; i++)
            {
                FiresecManager.CoreConfig.zone[i] = new Firesec.CoreConfig.zoneType();
                FiresecManager.CoreConfig.zone[i].name = currentConfiguration.Zones[i].Name;
                FiresecManager.CoreConfig.zone[i].idx = currentConfiguration.Zones[i].No;
                FiresecManager.CoreConfig.zone[i].no = currentConfiguration.Zones[i].No;
                if (!string.IsNullOrEmpty(currentConfiguration.Zones[i].Description))
                    FiresecManager.CoreConfig.zone[i].desc = currentConfiguration.Zones[i].Description;

                List<Firesec.CoreConfig.paramType> zoneParams = new List<Firesec.CoreConfig.paramType>();
                if (!string.IsNullOrEmpty(currentConfiguration.Zones[i].DetectorCount))
                {
                    Firesec.CoreConfig.paramType DetectorCountZoneParam = new Firesec.CoreConfig.paramType();
                    DetectorCountZoneParam.name = "FireDeviceCount";
                    DetectorCountZoneParam.type = "Int";
                    DetectorCountZoneParam.value = currentConfiguration.Zones[i].DetectorCount;
                    zoneParams.Add(DetectorCountZoneParam);
                }
                if (!string.IsNullOrEmpty(currentConfiguration.Zones[i].EvacuationTime))
                {
                    Firesec.CoreConfig.paramType EvacuationTimeZoneParam = new Firesec.CoreConfig.paramType();
                    EvacuationTimeZoneParam.name = "ExitTime";
                    EvacuationTimeZoneParam.type = "SmallInt";
                    EvacuationTimeZoneParam.value = currentConfiguration.Zones[i].EvacuationTime;
                    zoneParams.Add(EvacuationTimeZoneParam);
                }
                if (zoneParams.Count > 0)
                    FiresecManager.CoreConfig.zone[i].param = zoneParams.ToArray();
            }
        }
    }
}
