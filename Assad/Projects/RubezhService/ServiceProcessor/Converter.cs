using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiseProcessor;
using Common;
using System.Runtime.Serialization;
using ServiceApi;

namespace ServiseProcessor
{
    public class Converter
    {
        public Firesec.CoreConfig.config Convert(Configuration configuration)
        {
            Device rootDevice = configuration.Devices[0];

            Device device = AddChild(rootDevice);

            Services.Configuration.CoreConfig.dev = new Firesec.CoreConfig.devType[1];
            Services.Configuration.CoreConfig.dev[0] = device.InnerDevice;

            Services.Configuration.CoreConfig.zone = new Firesec.CoreConfig.zoneType[Services.Configuration.Zones.Count];
            for (int i = 0; i < Services.Configuration.Zones.Count; i++)
            {
                Services.Configuration.CoreConfig.zone[i] = new Firesec.CoreConfig.zoneType();
                Services.Configuration.CoreConfig.zone[i].name = Services.Configuration.Zones[i].Name;
                Services.Configuration.CoreConfig.zone[i].idx = Services.Configuration.Zones[i].Id;
                Services.Configuration.CoreConfig.zone[i].no = Services.Configuration.Zones[i].Id;
                if (!string.IsNullOrEmpty(Services.Configuration.Zones[i].Description))
                    Services.Configuration.CoreConfig.zone[i].desc = Services.Configuration.Zones[i].Description;

                List<Firesec.CoreConfig.paramType> zoneParams = new List<Firesec.CoreConfig.paramType>();
                if (!string.IsNullOrEmpty(Services.Configuration.Zones[i].DetectorCount))
                {
                    Firesec.CoreConfig.paramType DetectorCountZoneParam = new Firesec.CoreConfig.paramType();
                    DetectorCountZoneParam.name = "FireDeviceCount";
                    DetectorCountZoneParam.type = "Int";
                    DetectorCountZoneParam.value = Services.Configuration.Zones[i].DetectorCount;
                    zoneParams.Add(DetectorCountZoneParam);
                }
                if (!string.IsNullOrEmpty(Services.Configuration.Zones[i].EvacuationTime))
                {
                    Firesec.CoreConfig.paramType EvacuationTimeZoneParam = new Firesec.CoreConfig.paramType();
                    EvacuationTimeZoneParam.name = "ExitTime";
                    EvacuationTimeZoneParam.type = "SmallInt";
                    EvacuationTimeZoneParam.value = Services.Configuration.Zones[i].EvacuationTime;
                    zoneParams.Add(EvacuationTimeZoneParam);
                }
                if (zoneParams.Count > 0)
                    Services.Configuration.CoreConfig.zone[i].param = zoneParams.ToArray();
            }

            return Services.Configuration.CoreConfig;
        }

        Device AddChild(Device parentDevice)
        {
            Firesec.CoreConfig.devType innerDevice = new Firesec.CoreConfig.devType();

            SetAddress(innerDevice, parentDevice);

            innerDevice.drv = Services.Configuration.CoreConfig.drv.FirstOrDefault(x => x.id == parentDevice.DriverId).idx;

            AddZone(parentDevice, innerDevice);
            parentDevice.SetInnerDevice(innerDevice);

            // добавление прочих параметров конфигурации
            List<Firesec.CoreConfig.propType> propertyList = AddCustomProperties(parentDevice);
            if (propertyList.Count > 0)
                innerDevice.prop = propertyList.ToArray();

            List<Firesec.CoreConfig.devType> innerDeviceChildren = new List<Firesec.CoreConfig.devType>();

            foreach (Device childTreeBase in parentDevice.Children)
            {
                Device childDevice = AddChild((Device)childTreeBase);
                innerDeviceChildren.Add(childDevice.InnerDevice);
            }

            parentDevice.InnerDevice = innerDevice;
            parentDevice.InnerDevice.dev = innerDeviceChildren.ToArray();
            if (innerDeviceChildren.Count == 0)
                parentDevice.InnerDevice.dev = null;

            return parentDevice;
        }

        // установить адрес
        public void SetAddress(Firesec.CoreConfig.devType innerComDevice, Device device)
        {
            switch (device.DriverName)
            {
                case "Компьютер":
                case "USB преобразователь МС-1":
                case "USB преобразователь МС-2":
                case "Насосная Станция":
                case "Жокей-насос":
                case "Компрессор":
                case "Дренажный насос":
                case "Насос компенсации утечек":
                    innerComDevice.addr = "0";
                    return;
                default:
                    break;
            }

            if (device.Address == null)
            {
                throw new Exception("Адрес не может отсутствовать");
            }

            innerComDevice.addr = device.Address;
        }

        List<Firesec.CoreConfig.propType> AddCustomProperties(Device parentComDevice)
        {
            List<Firesec.CoreConfig.propType> propertyList = new List<Firesec.CoreConfig.propType>();

            if (parentComDevice.DriverName != "Компьютер")
            {
                if (parentComDevice.DeviceProperties != null)
                {
                    if (parentComDevice.DeviceProperties.Count > 0)
                    {
                        foreach (DeviceProperty deviceProperty in parentComDevice.DeviceProperties)
                        {
                            if ((!string.IsNullOrEmpty(deviceProperty.Name)) && (!string.IsNullOrEmpty(deviceProperty.Value)))
                            {
                                Firesec.Metadata.drvType metadataDriver = Services.Configuration.Metadata.drv.First(x => x.id == parentComDevice.DriverId);
                                if (metadataDriver.propInfo != null)
                                {
                                    if (metadataDriver.propInfo.Any(x => x.caption == deviceProperty.Name))
                                    {
                                        Firesec.Metadata.propInfoType propertyInfo = metadataDriver.propInfo.FirstOrDefault(x => x.caption == deviceProperty.Name);
                                        string propertyType = propertyInfo.type;
                                        if (propertyType == "Int")
                                        {
                                            try
                                            {
                                                int intValue = System.Convert.ToInt32(deviceProperty.Value);
                                            }
                                            catch
                                            {
                                                throw new Exception("Дополнительное авойство должно быть целым");
                                            }
                                        }
                                        string propertyName = propertyInfo.name;
                                        Firesec.CoreConfig.propType property = new Firesec.CoreConfig.propType();
                                        property.name = propertyName;

                                        // тип свойства
                                        if (propertyType == "Bool")
                                        {
                                            switch (deviceProperty.Value)
                                            {
                                                case "Да":
                                                    property.value = "1";
                                                    break;
                                                case "Нет":
                                                    property.value = "0";
                                                    break;
                                                default:
                                                    throw new Exception("Неправильный формат выражения типа bool");
                                            }
                                        }
                                        else
                                        {
                                            property.value = deviceProperty.Value;
                                        }
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

        void AddZone(Device device, Firesec.CoreConfig.devType innerComDevice)
        {
            if (device.Zone != null)
            {
                List<Firesec.CoreConfig.inZType> zones = new List<Firesec.CoreConfig.inZType>();
                zones.Add(new Firesec.CoreConfig.inZType() { idz = device.Zone });
                innerComDevice.inZ = zones.ToArray();
            }
        }
    }
}
