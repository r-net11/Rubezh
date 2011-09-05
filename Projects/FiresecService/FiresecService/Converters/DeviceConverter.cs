using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Firesec.CoreConfig;
using FiresecAPI.Models;

namespace FiresecService.Converters
{
    public static class DeviceConverter
    {
        static Firesec.CoreConfig.config _firesecConfig;

        public static void Convert(Firesec.CoreConfig.config firesecConfig)
        {
            _firesecConfig = firesecConfig;

            FiresecManager.DeviceConfiguration.Devices = new List<Device>();

            var rootInnerDevice = FiresecManager.CoreConfig.dev[0];
            var rootDevice = new Device();
            rootDevice.Parent = null;
            SetInnerDevice(rootDevice, rootInnerDevice);
            FiresecManager.DeviceConfiguration.Devices.Add(rootDevice);
            AddDevice(rootInnerDevice, rootDevice);

            FiresecManager.DeviceConfiguration.RootDevice = rootDevice;
        }

        static void AddDevice(Firesec.CoreConfig.devType parentInnerDevice, Device parentDevice)
        {
            parentDevice.Children = new List<Device>();
            if (parentInnerDevice.dev != null)
            {
                foreach (var innerDevice in parentInnerDevice.dev)
                {
                    var device = new Device()
                    {
                        Parent = parentDevice
                    };

                    parentDevice.Children.Add(device);
                    SetInnerDevice(device, innerDevice);
                    FiresecManager.DeviceConfiguration.Devices.Add(device);
                    AddDevice(innerDevice, device);
                }
            }
        }

        static void SetInnerDevice(Device device, Firesec.CoreConfig.devType innerDevice)
        {
            var driverId = _firesecConfig.drv.FirstOrDefault(x => x.idx == innerDevice.drv).id;
            device.DriverId = driverId;
            device.Driver = FiresecManager.Drivers.FirstOrDefault(x => x.Id == driverId);

            device.IntAddress = int.Parse(innerDevice.addr);

            if (innerDevice.param != null)
            {
                var DatabaseIdParam = innerDevice.param.FirstOrDefault(x => x.name == "DB$IDDevices");
                if (DatabaseIdParam != null)
                {
                    device.DatabaseId = DatabaseIdParam.value;
                }
                var UIDParam = innerDevice.param.FirstOrDefault(x => x.name == "INT$DEV_GUID");
                if (UIDParam != null)
                {
                    device.UID = GuidHelper.ToGuid(UIDParam.value);
                }
                else
                {
                    device.UID = Guid.NewGuid();
                }
            }

            device.Properties = new List<Property>();
            if (innerDevice.prop != null)
            {
                foreach (var innerProperty in innerDevice.prop)
                {
                    device.Properties.Add(new Property()
                    {
                        Name = innerProperty.name,
                        Value = innerProperty.value
                    });
                }
            }

            device.IsRmAlarmDevice = device.Properties.Any(x => x.Name == "IsAlarmDevice");

            device.Description = innerDevice.name;

            SetZone(device, innerDevice);

            if (innerDevice.shape != null)
            {
                device.ShapeIds = new List<string>();
                foreach (var shape in innerDevice.shape)
                {
                    device.ShapeIds.Add(shape.id);
                }
            }
        }

        static void SetZone(Device device, Firesec.CoreConfig.devType innerDevice)
        {
            if (innerDevice.inZ != null)
            {
                string zoneIdx = innerDevice.inZ[0].idz;
                string zoneNo = _firesecConfig.zone.FirstOrDefault(x => x.idx == zoneIdx).no;
                device.ZoneNo = zoneNo;
            }
            if (innerDevice.prop != null)
            {
                var zoneLogicProperty = innerDevice.prop.FirstOrDefault(x => x.name == "ExtendedZoneLogic");
                if (zoneLogicProperty != null)
                {
                    string zoneLogicstring = zoneLogicProperty.value;
                    if (string.IsNullOrEmpty(zoneLogicstring) == false)
                    {
                        var zoneLogic = SerializerHelper.GetZoneLogic(zoneLogicstring);
                        device.ZoneLogic = ZoneLogicConverter.Convert(zoneLogic);
                    }
                }

                var indicatorLogicProperty = innerDevice.prop.FirstOrDefault(x => x.name == "C4D7C1BE-02A3-4849-9717-7A3C01C23A24");
                if (indicatorLogicProperty != null)
                {
                    string indicatorLogicString = indicatorLogicProperty.value;
                    if (string.IsNullOrEmpty(indicatorLogicString) == false)
                    {
                        var indicatorLogic = SerializerHelper.GetIndicatorLogic(indicatorLogicString);
                        if (indicatorLogic != null)
                        {
                            device.IndicatorLogic = IndicatorLogicConverter.Convert(indicatorLogic);
                        }
                    }
                }

                var pDUGroupLogicProperty = innerDevice.prop.FirstOrDefault(x => x.name == "E98669E4-F602-4E15-8A64-DF9B6203AFC5");
                if (pDUGroupLogicProperty != null)
                {
                    string pDUGroupLogicPropertyString = pDUGroupLogicProperty.value;
                    if (string.IsNullOrEmpty(pDUGroupLogicPropertyString) == false)
                    {
                        var pDUGroupLogic = SerializerHelper.GetGroupProperties(pDUGroupLogicPropertyString);
                        device.PDUGroupLogic = PDUGroupLogicConverter.Convert(pDUGroupLogic);
                    }
                }
            }
        }

        public static void ConvertBack(DeviceConfiguration deviceConfiguration)
        {
            Device rootDevice = deviceConfiguration.RootDevice;
            Firesec.CoreConfig.devType rootInnerDevice = DeviceToInnerDevice(rootDevice);
            AddInnerDevice(rootDevice, rootInnerDevice);

            FiresecManager.CoreConfig.dev = new Firesec.CoreConfig.devType[1];
            FiresecManager.CoreConfig.dev[0] = rootInnerDevice;
        }

        static void AddInnerDevice(Device parentDevice, Firesec.CoreConfig.devType parentInnerDevice)
        {
            var childInnerDevices = new List<Firesec.CoreConfig.devType>();

            foreach (var device in parentDevice.Children)
            {
                Firesec.CoreConfig.devType childInnerDevice = DeviceToInnerDevice(device);
                childInnerDevices.Add(childInnerDevice);
                AddInnerDevice(device, childInnerDevice);
            }
            parentInnerDevice.dev = childInnerDevices.ToArray();
        }

        static Firesec.CoreConfig.devType DeviceToInnerDevice(Device device)
        {
            var innerDevice = new Firesec.CoreConfig.devType();
            innerDevice.drv = FiresecManager.CoreConfig.drv.FirstOrDefault(x => x.id == device.Driver.Id).idx;

            if (device.Driver.HasAddress)
            {
                innerDevice.addr = device.IntAddress.ToString();
            }
            else
            {
                innerDevice.addr = "0";
            }

            if (device.ZoneNo != null)
            {
                var zones = new List<Firesec.CoreConfig.inZType>();
                zones.Add(new Firesec.CoreConfig.inZType() { idz = device.ZoneNo });
                innerDevice.inZ = zones.ToArray();
            }

            innerDevice.prop = AddProperties(device).ToArray();

            innerDevice.param = AddParameters(device).ToArray();

            return innerDevice;
        }

        static List<Firesec.CoreConfig.paramType> AddParameters(Device device)
        {
            var parameters = new List<Firesec.CoreConfig.paramType>();

            if (device.UID != Guid.Empty)
            {
                var UIDParam = new paramType()
                {
                    name = "INT$DEV_GUID",
                    type = "String",
                    value = GuidHelper.ToString(device.UID)
                };
                parameters.Add(UIDParam);
            }

            if (device.DatabaseId != null)
            {
                var DatabaseIdParam = new paramType()
                {
                    name = "DB$IDDevices",
                    type = "Int",
                    value = device.DatabaseId
                };
                parameters.Add(DatabaseIdParam);
            }

            return parameters;
        }

        static List<Firesec.CoreConfig.propType> AddProperties(Device device)
        {
            var propertyList = new List<Firesec.CoreConfig.propType>();

            if (device.Driver.DriverName != "Компьютер")
            {
                if (device.Properties != null && device.Properties.Count > 0)
                {
                    foreach (var deviceProperty in device.Properties)
                    {
                        if (string.IsNullOrEmpty(deviceProperty.Name) == false &&
                            string.IsNullOrEmpty(deviceProperty.Value) == false)
                        {
                            propertyList.Add(new Firesec.CoreConfig.propType()
                            {
                                name = deviceProperty.Name,
                                value = deviceProperty.Value
                            });
                        }
                    }
                }
            }

            if (device.IsRmAlarmDevice)
            {
                var zoneLogicProperty = propertyList.FirstOrDefault(x => x.name == "IsAlarmDevice");
                if (zoneLogicProperty == null)
                {
                    propertyList.Add(new Firesec.CoreConfig.propType() { name = "IsAlarmDevice", value = "1" });
                }
            }

            if (device.ZoneLogic != null)
            {
                if (device.ZoneLogic.Clauses.Count > 0)
                {
                    var zoneLogicProperty = propertyList.FirstOrDefault(x => x.name == "ExtendedZoneLogic");
                    if (zoneLogicProperty == null)
                    {
                        propertyList.Add(new Firesec.CoreConfig.propType());
                    }

                    var zoneLogic = ZoneLogicConverter.ConvertBack(device.ZoneLogic);
                    var zoneLogicString = SerializerHelper.SetZoneLogic(zoneLogic);

                    zoneLogicProperty.name = "ExtendedZoneLogic";
                    zoneLogicProperty.value = zoneLogicString;
                }
            }

            if (device.IndicatorLogic != null)
            {
                var indicatorLogicProperty = propertyList.FirstOrDefault(x => x.name == "C4D7C1BE-02A3-4849-9717-7A3C01C23A24");
                if (indicatorLogicProperty == null)
                {
                    indicatorLogicProperty = new Firesec.CoreConfig.propType();
                    propertyList.Add(indicatorLogicProperty);
                }

                var indicatorLogic = IndicatorLogicConverter.ConvertBack(device.IndicatorLogic);
                var indicatorLogicString = SerializerHelper.SetIndicatorLogic(indicatorLogic);

                indicatorLogicProperty.name = "C4D7C1BE-02A3-4849-9717-7A3C01C23A24";
                indicatorLogicProperty.value = indicatorLogicString;
            }

            if (device.PDUGroupLogic != null)
            {
                var pDUGroupLogicProperty = propertyList.FirstOrDefault(x => x.name == "E98669E4-F602-4E15-8A64-DF9B6203AFC5");
                if (pDUGroupLogicProperty == null)
                {
                    pDUGroupLogicProperty = new Firesec.CoreConfig.propType();
                    propertyList.Add(pDUGroupLogicProperty);
                }

                var pDUGroupLogic = PDUGroupLogicConverter.ConvertBack(device.PDUGroupLogic);
                var pDUGroupLogicString = SerializerHelper.SeGroupProperty(pDUGroupLogic);

                pDUGroupLogicProperty.name = "E98669E4-F602-4E15-8A64-DF9B6203AFC5";
                pDUGroupLogicProperty.value = pDUGroupLogicString;
            }

            return propertyList;
        }
    }
}