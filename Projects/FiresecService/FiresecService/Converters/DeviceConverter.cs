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
            ConfigurationConverter.DeviceConfiguration.Devices = new List<Device>();

            var rootInnerDevice = ConfigurationConverter.FiresecConfiguration.dev[0];
            var rootDevice = new Device()
            {
                Parent = null
            };
            SetInnerDevice(rootDevice, rootInnerDevice);
            ConfigurationConverter.DeviceConfiguration.Devices.Add(rootDevice);
            AddDevice(rootInnerDevice, rootDevice);

            ConfigurationConverter.DeviceConfiguration.RootDevice = rootDevice;
        }

        static void AddDevice(devType parentInnerDevice, Device parentDevice)
        {
            if (parentInnerDevice.dev == null)
                return;

            parentDevice.Children = new List<Device>();
            foreach (var innerDevice in parentInnerDevice.dev)
            {
                var device = new Device() { Parent = parentDevice };

                parentDevice.Children.Add(device);
                SetInnerDevice(device, innerDevice);
                ConfigurationConverter.DeviceConfiguration.Devices.Add(device);
                AddDevice(innerDevice, device);
            }
        }

        static void SetInnerDevice(Device device, devType innerDevice)
        {
            var driverId = ConfigurationConverter.FiresecConfiguration.drv.FirstOrDefault(x => x.idx == innerDevice.drv).id;
            var driverUID = new Guid(driverId);
            device.DriverUID = driverUID;
            device.Driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == driverUID);

            device.IntAddress = int.Parse(innerDevice.addr);

            if ((innerDevice.disabled != null) && (innerDevice.disabled == "1"))
                device.IsMonitoringDisabled = true;

            if (innerDevice.param != null)
            {
                var DatabaseIdParam = innerDevice.param.FirstOrDefault(x => x.name == "DB$IDDevices");
                if (DatabaseIdParam != null)
                    device.DatabaseId = DatabaseIdParam.value;

                var UIDParam = innerDevice.param.FirstOrDefault(x => x.name == "INT$DEV_GUID");
                if (UIDParam != null)
                    device.UID = GuidHelper.ToGuid(UIDParam.value);
                else
                    device.UID = Guid.NewGuid();
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

            device.ShapeIds = new List<string>();
            if (innerDevice.shape != null)
            {
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
                device.ZoneNo = ulong.Parse(zoneNo);
            }
            if (innerDevice.prop != null)
            {
                var zoneLogicProperty = innerDevice.prop.FirstOrDefault(x => x.name == "ExtendedZoneLogic");
                if (zoneLogicProperty != null)
                {
                    string zoneLogicstring = zoneLogicProperty.value;
                    if (string.IsNullOrEmpty(zoneLogicstring) == false)
                        device.ZoneLogic = ZoneLogicConverter.Convert(SerializerHelper.GetZoneLogic(zoneLogicstring));
                }

                var indicatorLogicProperty = innerDevice.prop.FirstOrDefault(x => x.name == "C4D7C1BE-02A3-4849-9717-7A3C01C23A24");
                if (indicatorLogicProperty != null)
                {
                    string indicatorLogicString = indicatorLogicProperty.value;
                    if (string.IsNullOrEmpty(indicatorLogicString) == false)
                    {
                        var indicatorLogic = SerializerHelper.GetIndicatorLogic(indicatorLogicString);
                        if (indicatorLogic != null)
                            device.IndicatorLogic = IndicatorLogicConverter.Convert(indicatorLogic);
                    }
                }

                var pDUGroupLogicProperty = innerDevice.prop.FirstOrDefault(x => x.name == "E98669E4-F602-4E15-8A64-DF9B6203AFC5");
                if (pDUGroupLogicProperty != null)
                {
                    string pDUGroupLogicPropertyString = pDUGroupLogicProperty.value;
                    if (string.IsNullOrEmpty(pDUGroupLogicPropertyString) == false)
                        device.PDUGroupLogic = PDUGroupLogicConverter.Convert(SerializerHelper.GetGroupProperties(pDUGroupLogicPropertyString));
                }
            }
        }

        public static void ConvertBack(DeviceConfiguration deviceConfiguration)
        {
            ConvertDriversBack();
            var rootDevice = deviceConfiguration.RootDevice;
            var rootInnerDevice = DeviceToInnerDevice(rootDevice);
            AddInnerDevice(rootDevice, rootInnerDevice);

            ConfigurationConverter.FiresecConfiguration.dev = new devType[1];
            ConfigurationConverter.FiresecConfiguration.dev[0] = rootInnerDevice;
        }

        static void ConvertDriversBack()
        {
            var drivers = new List<drvType>();
            foreach (var driver in FiresecManager.Drivers)
            {
                drivers.Add(new drvType()
                {
                    idx = FiresecManager.Drivers.IndexOf(driver).ToString(),
                    id = driver.UID.ToString().ToUpper(),
                    name = driver.Name
                });
            }
            ConfigurationConverter.FiresecConfiguration.drv = drivers.ToArray();
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

            innerDevice.drv = ConfigurationConverter.FiresecConfiguration.drv.FirstOrDefault(x => x.id.ToUpper() == device.DriverUID.ToString().ToUpper()).idx;

            if (device.Driver.HasAddress)
                innerDevice.addr = device.IntAddress.ToString();
            else
                innerDevice.addr = "0";

            if (device.IsMonitoringDisabled == true)
                innerDevice.disabled = "1";
            else
                innerDevice.disabled = null;

            if (device.ZoneNo != null)
            {
                var zones = new List<inZType>();
                zones.Add(new inZType() { idz = device.ZoneNo.ToString() });
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
                parameters.Add(new paramType()
                {
                    name = "INT$DEV_GUID",
                    type = "String",
                    value = GuidHelper.ToString(device.UID)
                });
            }

            if (device.DatabaseId != null)
            {
                parameters.Add(new paramType()
                {
                    name = "DB$IDDevices",
                    type = "Int",
                    value = device.DatabaseId
                });
            }

            return parameters;
        }

        static List<propType> AddProperties(Device device)
        {
            var propertyList = new List<propType>();
            if (device.Driver.DriverType != DriverType.Computer && device.Properties.IsNotNullOrEmpty())
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

                    zoneLogicProperty.name = "ExtendedZoneLogic";
                    var zoneLogic = ZoneLogicConverter.ConvertBack(device.ZoneLogic);
                    zoneLogicProperty.value = SerializerHelper.SetZoneLogic(zoneLogic);
                }
            }

            if ((device.Driver.IsIndicatorDevice) && (device.IndicatorLogic != null))
            {
                var indicatorLogicProperty = propertyList.FirstOrDefault(x => x.name == "C4D7C1BE-02A3-4849-9717-7A3C01C23A24");
                if (indicatorLogicProperty == null)
                {
                    indicatorLogicProperty = new propType();
                    propertyList.Add(indicatorLogicProperty);
                }

                indicatorLogicProperty.name = "C4D7C1BE-02A3-4849-9717-7A3C01C23A24";
                var indicatorLogic = IndicatorLogicConverter.ConvertBack(device.IndicatorLogic);
                indicatorLogicProperty.value = SerializerHelper.SetIndicatorLogic(indicatorLogic);
            }

            if ((device.Driver.DriverType == DriverType.PDU) && (device.PDUGroupLogic != null))
            {
                var pDUGroupLogicProperty = propertyList.FirstOrDefault(x => x.name == "E98669E4-F602-4E15-8A64-DF9B6203AFC5");
                if (pDUGroupLogicProperty == null)
                {
                    pDUGroupLogicProperty = new propType();
                    propertyList.Add(pDUGroupLogicProperty);
                }

                pDUGroupLogicProperty.name = "E98669E4-F602-4E15-8A64-DF9B6203AFC5";
                var pDUGroupLogic = PDUGroupLogicConverter.ConvertBack(device.PDUGroupLogic);
                pDUGroupLogicProperty.value = SerializerHelper.SeGroupProperty(pDUGroupLogic);
            }

            return propertyList;
        }
    }
}