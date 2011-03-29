using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceApi;
using FiresecMetadata;

namespace ServiseProcessor
{
    public class ConfigToFiresec
    {
        public Firesec.CoreConfig.config Convert(StateConfiguration configuration)
        {
            SetZones(configuration);

            ShortDevice rootShortDevice = configuration.RootShortDevice;
            Firesec.CoreConfig.devType rootInnerDevice = ShortDeviceToInnerDevice(rootShortDevice);
            AddInnerDevice(rootShortDevice, rootInnerDevice);

            Services.Configuration.CoreConfig.dev = new Firesec.CoreConfig.devType[1];
            Services.Configuration.CoreConfig.dev[0] = rootInnerDevice;
            return Services.Configuration.CoreConfig;
        }

        void AddInnerDevice(ShortDevice parentShortDevice, Firesec.CoreConfig.devType parentInnerDevice)
        {
            List<Firesec.CoreConfig.devType> childInnerDevices = new List<Firesec.CoreConfig.devType>();

            foreach (ShortDevice shortDevice in parentShortDevice.Children)
            {
                Firesec.CoreConfig.devType childInnerDevice = ShortDeviceToInnerDevice(shortDevice);
                childInnerDevices.Add(childInnerDevice);
                AddInnerDevice(shortDevice, childInnerDevice);
            }
            parentInnerDevice.dev = childInnerDevices.ToArray();
        }

        Firesec.CoreConfig.devType ShortDeviceToInnerDevice(ShortDevice shortDevice)
        {
            Firesec.CoreConfig.devType innerDevice = new Firesec.CoreConfig.devType();
            innerDevice.drv = Services.Configuration.CoreConfig.drv.FirstOrDefault(x => x.id == shortDevice.DriverId).idx;
            innerDevice.addr = ConvertAddress(shortDevice);

            if (shortDevice.ZoneNo != null)
            {
                List<Firesec.CoreConfig.inZType> zones = new List<Firesec.CoreConfig.inZType>();
                zones.Add(new Firesec.CoreConfig.inZType() { idz = shortDevice.ZoneNo });
                innerDevice.inZ = zones.ToArray();
            }

            innerDevice.prop = AddProperties(shortDevice).ToArray();

            return innerDevice;
        }

        string ConvertAddress(ShortDevice shortDevice)
        {
            if (string.IsNullOrEmpty(shortDevice.Address))
                return "0";

            if (shortDevice.Address.Contains("."))
            {
                List<string> addresses = shortDevice.Address.Split(new char[] { '.' }, StringSplitOptions.None).ToList();

                int intShleifAddress = System.Convert.ToInt32(addresses[0]);
                int intAddress = System.Convert.ToInt32(addresses[1]);
                return (intShleifAddress * 256 + intAddress).ToString();
            }

            return shortDevice.Address;
        }

        void SetZones(StateConfiguration configuration)
        {
            Services.Configuration.CoreConfig.zone = new Firesec.CoreConfig.zoneType[configuration.ShortZones.Count];
            for (int i = 0; i < configuration.ShortZones.Count; i++)
            {
                Services.Configuration.CoreConfig.zone[i] = new Firesec.CoreConfig.zoneType();
                Services.Configuration.CoreConfig.zone[i].name = configuration.ShortZones[i].Name;
                Services.Configuration.CoreConfig.zone[i].idx = configuration.ShortZones[i].No;
                Services.Configuration.CoreConfig.zone[i].no = configuration.ShortZones[i].No;
                if (!string.IsNullOrEmpty(configuration.ShortZones[i].Description))
                    Services.Configuration.CoreConfig.zone[i].desc = configuration.ShortZones[i].Description;

                List<Firesec.CoreConfig.paramType> zoneParams = new List<Firesec.CoreConfig.paramType>();
                if (!string.IsNullOrEmpty(configuration.ShortZones[i].DetectorCount))
                {
                    Firesec.CoreConfig.paramType DetectorCountZoneParam = new Firesec.CoreConfig.paramType();
                    DetectorCountZoneParam.name = "FireDeviceCount";
                    DetectorCountZoneParam.type = "Int";
                    DetectorCountZoneParam.value = configuration.ShortZones[i].DetectorCount;
                    zoneParams.Add(DetectorCountZoneParam);
                }
                if (!string.IsNullOrEmpty(configuration.ShortZones[i].EvacuationTime))
                {
                    Firesec.CoreConfig.paramType EvacuationTimeZoneParam = new Firesec.CoreConfig.paramType();
                    EvacuationTimeZoneParam.name = "ExitTime";
                    EvacuationTimeZoneParam.type = "SmallInt";
                    EvacuationTimeZoneParam.value = configuration.ShortZones[i].EvacuationTime;
                    zoneParams.Add(EvacuationTimeZoneParam);
                }
                if (zoneParams.Count > 0)
                    Services.Configuration.CoreConfig.zone[i].param = zoneParams.ToArray();
            }
        }

        List<Firesec.CoreConfig.propType> AddProperties(ShortDevice device)
        {
            List<Firesec.CoreConfig.propType> propertyList = new List<Firesec.CoreConfig.propType>();

            string driverName = DriversHelper.GetDriverNameById(device.DriverId);
            if (driverName != "Компьютер")
            {
                if (device.DeviceProperties != null)
                {
                    if (device.DeviceProperties.Count > 0)
                    {
                        foreach (DeviceProperty deviceProperty in device.DeviceProperties)
                        {
                            if ((!string.IsNullOrEmpty(deviceProperty.Name)) && (!string.IsNullOrEmpty(deviceProperty.Value)))
                            {
                                Firesec.Metadata.drvType metadataDriver = Services.Configuration.Metadata.drv.First(x => x.id == device.DriverId);
                                if (metadataDriver.propInfo != null)
                                {
                                    if (metadataDriver.propInfo.Any(x => x.name == deviceProperty.Name))
                                    {
                                        Firesec.CoreConfig.propType property = new Firesec.CoreConfig.propType();
                                        property.name = deviceProperty.Name;
                                        property.value = deviceProperty.Value;
                                        propertyList.Add(property);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return propertyList;
        }
    }
}
