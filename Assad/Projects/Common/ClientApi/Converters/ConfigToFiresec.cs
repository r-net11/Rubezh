using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceApi;
using FiresecMetadata;

namespace ClientApi
{
    public class ConfigToFiresec
    {
        public Firesec.CoreConfig.config Convert(CurrentConfiguration configuration)
        {
            SetZones(configuration);

            Device rootDevice = configuration.RootDevice;
            Firesec.CoreConfig.devType rootInnerDevice = DeviceToInnerDevice(rootDevice);
            AddInnerDevice(rootDevice, rootInnerDevice);

            ServiceClient.CoreConfig.dev = new Firesec.CoreConfig.devType[1];
            ServiceClient.CoreConfig.dev[0] = rootInnerDevice;
            return ServiceClient.CoreConfig;
        }

        void AddInnerDevice(Device parentDevice, Firesec.CoreConfig.devType parentInnerDevice)
        {
            List<Firesec.CoreConfig.devType> childInnerDevices = new List<Firesec.CoreConfig.devType>();

            foreach (Device device in parentDevice.Children)
            {
                Firesec.CoreConfig.devType childInnerDevice = DeviceToInnerDevice(device);
                childInnerDevices.Add(childInnerDevice);
                AddInnerDevice(device, childInnerDevice);
            }
            parentInnerDevice.dev = childInnerDevices.ToArray();
        }

        Firesec.CoreConfig.devType DeviceToInnerDevice(Device device)
        {
            Firesec.CoreConfig.devType innerDevice = new Firesec.CoreConfig.devType();
            innerDevice.drv = ServiceClient.CoreConfig.drv.FirstOrDefault(x => x.id == device.DriverId).idx;
            innerDevice.addr = ConvertAddress(device);

            if (device.ZoneNo != null)
            {
                List<Firesec.CoreConfig.inZType> zones = new List<Firesec.CoreConfig.inZType>();
                zones.Add(new Firesec.CoreConfig.inZType() { idz = device.ZoneNo });
                innerDevice.inZ = zones.ToArray();
            }

            innerDevice.prop = AddProperties(device).ToArray();

            return innerDevice;
        }

        string ConvertAddress(Device device)
        {
            if (string.IsNullOrEmpty(device.Address))
                return "0";

            if (device.Address.Contains("."))
            {
                List<string> addresses = device.Address.Split(new char[] { '.' }, StringSplitOptions.None).ToList();

                int intShleifAddress = System.Convert.ToInt32(addresses[0]);
                int intAddress = System.Convert.ToInt32(addresses[1]);
                return (intShleifAddress * 256 + intAddress).ToString();
            }

            return device.Address;
        }

        void SetZones(CurrentConfiguration currentConfiguration)
        {
            ServiceClient.CoreConfig.zone = new Firesec.CoreConfig.zoneType[currentConfiguration.Zones.Count];
            for (int i = 0; i < currentConfiguration.Zones.Count; i++)
            {
                ServiceClient.CoreConfig.zone[i] = new Firesec.CoreConfig.zoneType();
                ServiceClient.CoreConfig.zone[i].name = currentConfiguration.Zones[i].Name;
                ServiceClient.CoreConfig.zone[i].idx = currentConfiguration.Zones[i].No;
                ServiceClient.CoreConfig.zone[i].no = currentConfiguration.Zones[i].No;
                if (!string.IsNullOrEmpty(currentConfiguration.Zones[i].Description))
                    ServiceClient.CoreConfig.zone[i].desc = currentConfiguration.Zones[i].Description;

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
                    ServiceClient.CoreConfig.zone[i].param = zoneParams.ToArray();
            }
        }

        List<Firesec.CoreConfig.propType> AddProperties(Device device)
        {
            List<Firesec.CoreConfig.propType> propertyList = new List<Firesec.CoreConfig.propType>();

            string driverName = DriversHelper.GetDriverNameById(device.DriverId);
            if (driverName != "Компьютер")
            {
                if (device.Properties != null)
                {
                    if (device.Properties.Count > 0)
                    {
                        foreach (Property deviceProperty in device.Properties)
                        {
                            if ((!string.IsNullOrEmpty(deviceProperty.Name)) && (!string.IsNullOrEmpty(deviceProperty.Value)))
                            {
                                Firesec.Metadata.drvType metadataDriver = ServiceClient.CurrentConfiguration.Metadata.drv.First(x => x.id == device.DriverId);
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

            if (device.ZoneLogic != null)
            {
                string zoneLogic = FiresecClient.SetZoneLogic(device.ZoneLogic);
                Firesec.CoreConfig.propType property = new Firesec.CoreConfig.propType();
                property.name = "ExtendedZoneLogic";
                property.value = zoneLogic;
                property.value = "&lt;?xml version&#061;&quot;1.0&quot; encoding&#061;&quot;windows-1251&quot;?&gt;&#010;&lt;expr&gt;&lt;clause operation&#061;&quot;or&quot; state&#061;&quot;2&quot;&gt;&lt;zone&gt;0&lt;&#047;zone&gt;&lt;zone&gt;2&lt;&#047;zone&gt;&lt;zone&gt;3&lt;&#047;zone&gt;&lt;zone&gt;4&lt;&#047;zone&gt;&lt;zone&gt;5&lt;&#047;zone&gt;&lt;&#047;clause&gt;&lt;&#047;expr&gt;&#010;";
                propertyList.Add(property);
            }

            return propertyList;
        }
    }
}
