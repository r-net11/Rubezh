using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssadDevices;
using ServiceApi;

namespace AssadProcessor
{
    public static class AssadToServiceConverter
    {
        public static CurrentConfiguration Convert()
        {
            CurrentConfiguration currentConfiguration = new CurrentConfiguration();

            ServiceApi.Device rootDevice = AddChild(AssadConfiguration.Devices[0]);
            currentConfiguration.Zones = FindAllZones(AssadConfiguration.Devices[0]);
            //configuration.shortDevice = rootDevice;
            return currentConfiguration;
        }

        static ServiceApi.Device AddChild(AssadBase parentAssadBase)
        {
            if (parentAssadBase is AssadZone)
            {
                return null;
            }

            if ((parentAssadBase is AssadDevice) || (parentAssadBase is AssadMC) || (parentAssadBase is AssadChannel) || (parentAssadBase is AssadPanel) || (parentAssadBase is AssadMC34) || (parentAssadBase is AssadPage) || (parentAssadBase is AssadIndicator) || (parentAssadBase is AssadComputer) || (parentAssadBase is AssadASPT) || (parentAssadBase is AssadPumpStation) || (parentAssadBase is AssadPump))
            {
                ServiceApi.Device device = new ServiceApi.Device();
                device.Address = parentAssadBase.Address;
                device.DriverId = parentAssadBase.DriverId;
                device.Description = parentAssadBase.Description;

                if (parentAssadBase is AssadDevice)
                    device.ZoneNo = (parentAssadBase as AssadDevice).Zone;
                if (parentAssadBase is AssadPumpStation)
                    device.ZoneNo = (parentAssadBase as AssadPumpStation).Zone;


                // скопировать все свойства
                device.Properties = new List<Property>();
                if (parentAssadBase.Properties != null)
                {
                    if (parentAssadBase.Properties.Count > 0)
                    {
                        foreach (AssadProperty assadProperty in parentAssadBase.Properties)
                        {
                            Property deviceProperty = new Property();
                            deviceProperty.Name = assadProperty.Name;
                            deviceProperty.Value = assadProperty.Value;
                            device.Properties.Add(deviceProperty);
                        }
                    }
                }

                foreach (AssadBase assadBase in parentAssadBase.Children)
                {
                    ServiceApi.Device childDevice = AddChild(assadBase);
                    if (childDevice != null)
                    {
                        if (device.Children == null)
                            device.Children = new List<ServiceApi.Device>();
                        device.Children.Add(childDevice);
                    }
                }
                return device;
            }

            throw new Exception("Неизвестное устройство");
        }

        static List<Zone> FindAllZones(AssadBase assadParent)
        {
            List<Zone> zones = new List<Zone>();

            foreach (AssadBase assadBase in assadParent.Children)
            {
                if (assadBase is AssadZone)
                {
                    AssadZone assadZone = assadBase as AssadZone;
                    Zone zone = new Zone();
                    zone.No = assadZone.ZoneNo;
                    zone.Name = assadZone.ZoneName;
                    zone.DetectorCount = assadZone.DetectorCount;
                    zone.EvacuationTime = assadZone.EvecuationTime;
                    zone.Description = assadZone.Description;
                    zones.Add(zone);
                }
            }

            return zones;
        }
    }
}
