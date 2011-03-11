using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComDevices;
using Common;
using System.Runtime.Serialization;
using ServiceApi;

namespace ComDevices
{
    public class Converter
    {
        public ComServer.CoreConfig.config Convert(Configuration configuration)
        {
            Device rootDevice = configuration.Devices[0];

            Device device = AddChild(rootDevice);

            Services.Configuration.CoreConfig.dev = new ComServer.CoreConfig.devType[1];
            Services.Configuration.CoreConfig.dev[0] = device.InnerDevice;

            Services.Configuration.CoreConfig.zone = new ComServer.CoreConfig.zoneType[Services.Configuration.Zones.Count];
            for (int i = 0; i < Services.Configuration.Zones.Count; i++)
            {
                Services.Configuration.CoreConfig.zone[i] = new ComServer.CoreConfig.zoneType();
                Services.Configuration.CoreConfig.zone[i].name = Services.Configuration.Zones[i].Name;
                Services.Configuration.CoreConfig.zone[i].idx = Services.Configuration.Zones[i].Id;
                Services.Configuration.CoreConfig.zone[i].no = Services.Configuration.Zones[i].Id;
                if (!string.IsNullOrEmpty(Services.Configuration.Zones[i].Description))
                    Services.Configuration.CoreConfig.zone[i].desc = Services.Configuration.Zones[i].Description;

                List<ComServer.CoreConfig.paramType> zoneParams = new List<ComServer.CoreConfig.paramType>();
                if (!string.IsNullOrEmpty(Services.Configuration.Zones[i].DetectorCount))
                {
                    ComServer.CoreConfig.paramType DetectorCountZoneParam = new ComServer.CoreConfig.paramType();
                    DetectorCountZoneParam.name = "FireDeviceCount";
                    DetectorCountZoneParam.type = "Int";
                    DetectorCountZoneParam.value = Services.Configuration.Zones[i].DetectorCount;
                    zoneParams.Add(DetectorCountZoneParam);
                }
                if (!string.IsNullOrEmpty(Services.Configuration.Zones[i].EvacuationTime))
                {
                    ComServer.CoreConfig.paramType EvacuationTimeZoneParam = new ComServer.CoreConfig.paramType();
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
            ComServer.CoreConfig.devType innerDevice = new ComServer.CoreConfig.devType();

            SetAddress(innerDevice, parentDevice);

            innerDevice.drv = Services.Configuration.CoreConfig.drv.FirstOrDefault(x => x.id == parentDevice.DriverId).idx;

            AddZone(parentDevice, innerDevice);
            DeviceHelper.SetDeviceInnerDevice(parentDevice, innerDevice);

            // добавление прочих параметров конфигурации
            List<ComServer.CoreConfig.propType> propertyList = AddCustomProperties(parentDevice);
            if (propertyList.Count > 0)
                innerDevice.prop = propertyList.ToArray();

            List<ComServer.CoreConfig.devType> innerDeviceChildren = new List<ComServer.CoreConfig.devType>();

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
        public void SetAddress(ComServer.CoreConfig.devType innerComDevice, Device device)
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

        List<ComServer.CoreConfig.propType> AddCustomProperties(Device parentComDevice)
        {
            List<ComServer.CoreConfig.propType> propertyList = new List<ComServer.CoreConfig.propType>();

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
                                ComServer.Metadata.drvType metadataDriver = Services.Configuration.Metadata.drv.First(x => x.id == parentComDevice.DriverId);
                                if (metadataDriver.propInfo != null)
                                {
                                    if (metadataDriver.propInfo.Any(x => x.caption == deviceProperty.Name))
                                    {
                                        ComServer.Metadata.propInfoType propertyInfo = metadataDriver.propInfo.FirstOrDefault(x => x.caption == deviceProperty.Name);
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
                                        ComServer.CoreConfig.propType property = new ComServer.CoreConfig.propType();
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

        void AddZone(Device device, ComServer.CoreConfig.devType innerComDevice)
        {
            if (device.Zones != null)
            {
                if (device.Zones.Count > 0)
                {
                    List<ComServer.CoreConfig.inZType> zones = new List<ComServer.CoreConfig.inZType>();
                    foreach (string zoneId in device.Zones)
                    {
                        if (zoneId != "")
                        {
                            zones.Add(new ComServer.CoreConfig.inZType() { idz = zoneId });
                            
                        }
                    }
                    innerComDevice.inZ = zones.ToArray();
                }
            }

            //string zoneName = device.Zones[0];
            //if (zoneName != null)
            //    if (zoneName != "")
            //    {
            //        {
            //            innerComDevice.inZ = new ComServer.CoreConfig.inZType[1];
            //            innerComDevice.inZ[0] = new ComServer.CoreConfig.inZType();
            //            innerComDevice.inZ[0].idz = Services.Configuration.Zones.FirstOrDefault(x => x.Name == zoneName).Id;
            //        }
            //    }
        }
    }
}
