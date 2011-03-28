using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiseProcessor;
using System.Runtime.Serialization;
using ServiceApi;
using FiresecMetadata;

namespace ServiseProcessor
{
    public class Converter
    {
        public Firesec.CoreConfig.config Convert2(StateConfiguration configuration)
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

            if (shortDevice.Zone != null)
            {
                List<Firesec.CoreConfig.inZType> zones = new List<Firesec.CoreConfig.inZType>();
                zones.Add(new Firesec.CoreConfig.inZType() { idz = shortDevice.Zone });
                innerDevice.inZ = zones.ToArray();
            }

            innerDevice.prop = AddCustomProperties(shortDevice).ToArray();

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



        public Firesec.CoreConfig.config Convert(StateConfiguration configuration)
        {
            return Convert2(configuration);
            ;
            ;
            ;
            ;

            ShortDevice rootDevice = configuration.RootShortDevice;

            ShortDevice device = AddChild(rootDevice);

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

        ShortDevice AddChild(ShortDevice parentDevice)
        {
            Firesec.CoreConfig.devType innerDevice = new Firesec.CoreConfig.devType();

            SetAddress(innerDevice, parentDevice);

            innerDevice.drv = Services.Configuration.CoreConfig.drv.FirstOrDefault(x => x.id == parentDevice.DriverId).idx;

            AddZone(parentDevice, innerDevice);

            parentDevice.InnerDevice = innerDevice;
            //parentDevice.SetInnerDevice(innerDevice);

            // добавление прочих параметров конфигурации
            //List<Firesec.CoreConfig.propType> propertyList = AddCustomProperties(parentDevice);
            //if (propertyList.Count > 0)
            //    innerDevice.prop = propertyList.ToArray();

            List<Firesec.CoreConfig.devType> innerDeviceChildren = new List<Firesec.CoreConfig.devType>();

            foreach (ShortDevice childTreeBase in parentDevice.Children)
            {
                ShortDevice childDevice = AddChild((ShortDevice)childTreeBase);
                innerDeviceChildren.Add(childDevice.InnerDevice);
            }

            parentDevice.InnerDevice = innerDevice;
            parentDevice.InnerDevice.dev = innerDeviceChildren.ToArray();
            if (innerDeviceChildren.Count == 0)
                parentDevice.InnerDevice.dev = null;

            return parentDevice;
        }

        // установить адрес
        public void SetAddress(Firesec.CoreConfig.devType innerDevice, ShortDevice device)
        {
            Firesec.Metadata.drvType metadataDriver = Services.Configuration.Metadata.drv.First(x => x.id == device.DriverId);
            if (metadataDriver.addrMask != null)
            {
                List<string> addresses = device.Address.Split(new char[] { '.' }, StringSplitOptions.None).ToList();
                if (addresses.Count != 2)
                {
                    throw new Exception("Адрес должен быть разделен точкой");
                }
                int intShleifAddress = 0;
                try
                {
                    intShleifAddress = System.Convert.ToInt32(addresses[0]);
                }
                catch
                {
                    throw new Exception("Адрес шлейфа не является целочичленным значением");
                }
                int intAddress = 0;
                try
                {
                    intAddress = System.Convert.ToInt32(addresses[1]);
                }
                catch
                {
                    throw new Exception("Адрес является целочичленным значением");
                }
                if ((intAddress < 1) && (intAddress > 255))
                {
                    throw new Exception("Адрес должен быть в диапазоне 1 - 255");
                }
                string Address = (intShleifAddress * 256 + intAddress).ToString();
                innerDevice.addr = Address;
            }

            string driverName = DriversHelper.GetDriverNameById(device.DriverId);
            switch (driverName)
            {
                case "Компьютер":
                case "USB преобразователь МС-1":
                case "USB преобразователь МС-2":
                case "Насосная Станция":
                case "Жокей-насос":
                case "Компрессор":
                case "Дренажный насос":
                case "Насос компенсации утечек":
                    innerDevice.addr = "0";
                    return;
                default:
                    break;
            }

            if (device.Address == null)
            {
                throw new Exception("Адрес не может отсутствовать");
            }

            innerDevice.addr = device.Address;
        }

        List<Firesec.CoreConfig.propType> AddCustomProperties(ShortDevice parentDevice)
        {
            List<Firesec.CoreConfig.propType> propertyList = new List<Firesec.CoreConfig.propType>();

            string driverName = DriversHelper.GetDriverNameById(parentDevice.DriverId);
            if (driverName != "Компьютер")
            {
                if (parentDevice.Parameters != null)
                {
                    if (parentDevice.Parameters.Count > 0)
                    {
                        foreach (Parameter parameter in parentDevice.Parameters)
                        {
                            if ((!string.IsNullOrEmpty(parameter.Name)) && (!string.IsNullOrEmpty(parameter.Value)))
                            {
                                Firesec.Metadata.drvType metadataDriver = Services.Configuration.Metadata.drv.First(x => x.id == parentDevice.DriverId);
                                if (metadataDriver.propInfo != null)
                                {
                                    if (metadataDriver.propInfo.Any(x => x.caption == parameter.Name))
                                    {
                                        Firesec.Metadata.propInfoType propertyInfo = metadataDriver.propInfo.FirstOrDefault(x => x.caption == parameter.Name);
                                        string propertyType = propertyInfo.type;
                                        if (propertyType == "Int")
                                        {
                                            try
                                            {
                                                int intValue = System.Convert.ToInt32(parameter.Value);
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
                                            switch (parameter.Value)
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
                                            property.value = parameter.Value;
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

        void AddZone(ShortDevice device, Firesec.CoreConfig.devType innerComDevice)
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
