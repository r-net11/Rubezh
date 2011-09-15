using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Firesec.CoreConfiguration;
using FiresecAPI.Models;

namespace FiresecService.Converters
{
    public static class DeviceConverter
    {
        public static void Convert()
        {
            FiresecManager.DeviceConfiguration.Devices = new List<Device>();

            var rootInnerDevice = ConfigurationConverter.FiresecConfiguration.dev[0];
            var rootDevice = new Device();
            rootDevice.Parent = null;
            SetInnerDevice(rootDevice, rootInnerDevice);
            FiresecManager.DeviceConfiguration.Devices.Add(rootDevice);
            AddDevice(rootInnerDevice, rootDevice);

            FiresecManager.DeviceConfiguration.RootDevice = rootDevice;
        }

        static void AddDevice(devType parentInnerDevice, Device parentDevice)
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

        static void SetInnerDevice(Device device, devType innerDevice)
        {
            var driverId = ConfigurationConverter.FiresecConfiguration.drv.FirstOrDefault(x => x.idx == innerDevice.drv).id;
            var driverUID = new Guid(driverId);
            device.DriverUID = driverUID;
            device.Driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == driverUID);

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

        static void SetZone(Device device, devType innerDevice)
        {
            if (innerDevice.inZ != null)
            {
                string zoneIdx = innerDevice.inZ[0].idz;
                string zoneNo = ConfigurationConverter.FiresecConfiguration.zone.FirstOrDefault(x => x.idx == zoneIdx).no;
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
            var rootDevice = deviceConfiguration.RootDevice;
            var rootInnerDevice = DeviceToInnerDevice(rootDevice);
            AddInnerDevice(rootDevice, rootInnerDevice);

            ConfigurationConverter.FiresecConfiguration.dev = new devType[1];
            ConfigurationConverter.FiresecConfiguration.dev[0] = rootInnerDevice;
        }

        static void AddInnerDevice(Device parentDevice, devType parentInnerDevice)
        {
            var childInnerDevices = new List<devType>();

            foreach (var device in parentDevice.Children)
            {
                var childInnerDevice = DeviceToInnerDevice(device);
                childInnerDevices.Add(childInnerDevice);
                AddInnerDevice(device, childInnerDevice);
            }
            parentInnerDevice.dev = childInnerDevices.ToArray();
        }

        static devType DeviceToInnerDevice(Device device)
        {
            var innerDevice = new devType();
            innerDevice.drv = ConfigurationConverter.FiresecConfiguration.drv.FirstOrDefault(x => x.id == device.Driver.UID.ToString()).idx;

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
                var zones = new List<inZType>();
                zones.Add(new inZType() { idz = device.ZoneNo });
                innerDevice.inZ = zones.ToArray();
            }

            innerDevice.prop = AddProperties(device).ToArray();

            innerDevice.param = AddParameters(device).ToArray();

            return innerDevice;
        }

        static List<paramType> AddParameters(Device device)
        {
            var parameters = new List<paramType>();

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

        static List<propType> AddProperties(Device device)
        {
            var propertyList = new List<propType>();

            if (device.Driver.DriverType != DriverType.Computer)
            {
                if (device.Properties != null && device.Properties.Count > 0)
                {
                    foreach (var deviceProperty in device.Properties)
                    {
                        if (string.IsNullOrEmpty(deviceProperty.Name) == false &&
                            string.IsNullOrEmpty(deviceProperty.Value) == false)
                        {
                            propertyList.Add(new propType()
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
                    propertyList.Add(new propType() { name = "IsAlarmDevice", value = "1" });
                }
            }

            if (device.ZoneLogic != null)
            {
                if (device.ZoneLogic.Clauses.Count > 0)
                {
                    var zoneLogicProperty = propertyList.FirstOrDefault(x => x.name == "ExtendedZoneLogic");
                    if (zoneLogicProperty == null)
                    {
                        propertyList.Add(new propType());
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
                    indicatorLogicProperty = new propType();
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
                    pDUGroupLogicProperty = new propType();
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