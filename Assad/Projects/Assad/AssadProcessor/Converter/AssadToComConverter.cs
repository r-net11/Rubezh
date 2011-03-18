using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssadDevices;
using Common;
using ServiceApi;

namespace AssadProcessor
{
    public static class AssadToComConverter
    {
        public static StateConfiguration Convert()
        {
            StateConfiguration configuration = new StateConfiguration();

            ClientApi.Device rootDevice = AddChild(AssadConfiguration.Devices[0]);
            configuration.ShortZones = FindAllZones(AssadConfiguration.Devices[0]);
            //configuration.shortDevice = rootDevice;
            return configuration;
        }

        static ClientApi.Device AddChild(AssadBase assadParent)
        {
            if (assadParent is AssadZone)
            {
                return null;
            }

            if ((assadParent is AssadDevice) || (assadParent is AssadMC) || (assadParent is AssadChannel) || (assadParent is AssadPanel) || (assadParent is AssadMC34) || (assadParent is AssadPage) || (assadParent is AssadIndicator) || (assadParent is AssadComputer) || (assadParent is AssadASPT) || (assadParent is AssadPumpStation) || (assadParent is AssadPump))
            {
                ClientApi.Device device = new ClientApi.Device();
                device.Address = assadParent.Address;
                device.DriverId = assadParent.DriverId;
                device.Description = assadParent.Description;

                if (assadParent is AssadDevice)
                    device.Zone = (assadParent as AssadDevice).Zones[0];
                if (assadParent is AssadPumpStation)
                    device.Zone = (assadParent as AssadPumpStation).Zones[0];


                // скопировать все свойства
                device.DeviceProperties = new List<DeviceProperty>();
                if (assadParent.Properties != null)
                {
                    if (assadParent.Properties.Count > 0)
                    {
                        foreach (AssadProperty assadProperty in assadParent.Properties)
                        {
                            DeviceProperty deviceProperty = new DeviceProperty();
                            deviceProperty.Name = assadProperty.Name;
                            deviceProperty.Value = assadProperty.Value;
                            device.DeviceProperties.Add(deviceProperty);
                        }
                    }
                }

                foreach (AssadTreeBase childTreeBase in assadParent.Children)
                {
                    AssadBase childAssad = (AssadBase)childTreeBase;
                    ClientApi.Device childDevice = AddChild(childAssad);
                    if (childDevice != null)
                    {
                        if (device.Children == null)
                            device.Children = new List<ClientApi.Device>();
                        device.Children.Add(childDevice);
                    }
                }
                return device;
            }

            throw new Exception("Неизвестное устройство");
        }

        static List<ShortZone> FindAllZones(AssadBase assadParent)
        {
            List<ShortZone> Zones = new List<ShortZone>();

            foreach (AssadTreeBase childTreeBase in assadParent.Children)
            {
                if (childTreeBase is AssadZone)
                {
                    AssadZone assadZone = childTreeBase as AssadZone;
                    ShortZone zone = new ShortZone();
                    zone.Name = assadZone.ZoneName;
                    zone.Id = assadZone.ZoneId;
                    zone.DetectorCount = assadZone.DetectorCount;
                    zone.EvacuationTime = assadZone.EvecuationTime;
                    zone.Description = assadZone.Description;
                    Zones.Add(zone);
                }
            }

            return Zones;
        }
    }
}
