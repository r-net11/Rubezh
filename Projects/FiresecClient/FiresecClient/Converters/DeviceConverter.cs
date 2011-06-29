using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient.Models;

namespace FiresecClient.Converters
{
    public static class DeviceConverter
    {
        static Firesec.CoreConfig.config _firesecConfig;

        public static void Convert(Firesec.CoreConfig.config firesecConfig)
        {
            _firesecConfig = firesecConfig;

            FiresecManager.Configuration.Devices = new List<Device>();
            FiresecManager.States.DeviceStates = new List<DeviceState>();

            var rootInnerDevice = FiresecManager.CoreConfig.dev[0];
            Device rootDevice = new Device();
            rootDevice.Parent = null;
            SetInnerDevice(rootDevice, rootInnerDevice);
            FiresecManager.Configuration.Devices.Add(rootDevice);
            FiresecManager.States.DeviceStates.Add(CreateDeviceState(rootDevice));
            AddDevice(rootInnerDevice, rootDevice);

            FiresecManager.Configuration.RootDevice = rootDevice;
        }

        static void AddDevice(Firesec.CoreConfig.devType parentInnerDevice, Device parentDevice)
        {
            parentDevice.Children = new List<Device>();
            if (parentInnerDevice.dev != null)
            {
                foreach (var innerDevice in parentInnerDevice.dev)
                {
                    Device device = new Device();
                    device.Parent = parentDevice;
                    parentDevice.Children.Add(device);
                    SetInnerDevice(device, innerDevice);
                    FiresecManager.Configuration.Devices.Add(device);
                    FiresecManager.States.DeviceStates.Add(CreateDeviceState(device));
                    AddDevice(innerDevice, device);
                }
            }
        }

        static DeviceState CreateDeviceState(Device device)
        {
            DeviceState deviceState = new DeviceState();
            deviceState.ChangeEntities = new ChangeEntities();
            deviceState.Id = device.Id;
            deviceState.PlaceInTree = device.PlaceInTree;

            deviceState.InnerStates = new List<InnerState>();
            foreach (var innerState in device.Driver.States)
            {
                InnerState state = new InnerState(innerState);
                deviceState.InnerStates.Add(state);
            }

            deviceState.Parameters = new List<Parameter>();
            foreach (var innerParameter in device.Driver.Parameters)
            {
                Parameter parameter = new Parameter(innerParameter);
                deviceState.Parameters.Add(parameter);
            }

            return deviceState;
        }

        static void SetInnerDevice(Device device, Firesec.CoreConfig.devType innerDevice)
        {
            var driverId = _firesecConfig.drv.FirstOrDefault(x => x.idx == innerDevice.drv).id;
            device.Driver = FiresecManager.Configuration.Drivers.FirstOrDefault(x => x.Id == driverId);


            device.IntAddress = System.Convert.ToInt32(innerDevice.addr);

            //device.Address = innerDevice.addr;
            //if (device.Driver.HasAddressMask)
            //{
            //    int intAddress = System.Convert.ToInt32(device.Address);
            //    if (intAddress > 255)
            //    {
            //        int intShleifAddress = intAddress / 255;
            //        int intSelfAddress = intAddress % 256;
            //        device.Address = intShleifAddress.ToString() + "." + intSelfAddress.ToString();
            //    }
            //}

            if (innerDevice.param != null)
            {
                var param = innerDevice.param.FirstOrDefault(x => x.name == "DB$IDDevices");
                if (param != null)
                {
                    device.DatabaseId = param.value;
                }
            }

            device.Properties = new List<Property>();
            if (innerDevice.prop != null)
            {
                foreach (var innerProperty in innerDevice.prop)
                {
                    Property deviceProperty = new Property();
                    deviceProperty.Name = innerProperty.name;
                    deviceProperty.Value = innerProperty.value;
                    device.Properties.Add(deviceProperty);
                }
            }

            device.Description = innerDevice.name;

            //SetAddress(device, innerDevice);
            SetPlaceInTree(device);
            SetZone(device, innerDevice);
        }

        //static void SetAddress(Device device, Firesec.CoreConfig.devType innerDevice)
        //{
        //    switch (device.Driver.DriverName)
        //    {
        //        case "Компьютер":
        //        case "Насосная Станция":
        //        case "Жокей-насос":
        //        case "Компрессор":
        //        case "Дренажный насос":
        //        case "Насос компенсации утечек":
        //            device.Address = "0";
        //            break;

        //        case "USB преобразователь МС-1":
        //        case "USB преобразователь МС-2":
        //            if (innerDevice.prop != null)
        //            {
        //                var serialNoProperty = innerDevice.prop.FirstOrDefault(x => x.name == "SerialNo");
        //                if (serialNoProperty != null)
        //                    device.Address = serialNoProperty.value;
        //            }
        //            else
        //                device.Address = "0";
        //            break;
        //    }
        //}

        static void SetPlaceInTree(Device device)
        {
            if (device.Parent == null)
            {
                device.PlaceInTree = "";
            }
            else
            {
                string localPlaceInTree = (device.Parent.Children.Count - 1).ToString();
                if (device.Parent.PlaceInTree == "")
                {
                    device.PlaceInTree = localPlaceInTree;
                }
                else
                {
                    device.PlaceInTree = device.Parent.PlaceInTree + @"\" + localPlaceInTree;
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
                var property = innerDevice.prop.FirstOrDefault(x => x.name == "ExtendedZoneLogic");
                if (property != null)
                {
                    string zoneLogicstring = property.value;
                    if (string.IsNullOrEmpty(zoneLogicstring) == false)
                    {
                        device.ZoneLogic = FiresecInternalClient.GetZoneLogic(zoneLogicstring);
                    }
                }
            }
        }

        public static void ConvertBack(CurrentConfiguration currentConfiguration)
        {
            Device rootDevice = currentConfiguration.RootDevice;
            Firesec.CoreConfig.devType rootInnerDevice = DeviceToInnerDevice(rootDevice);
            AddInnerDevice(rootDevice, rootInnerDevice);

            FiresecManager.CoreConfig.dev = new Firesec.CoreConfig.devType[1];
            FiresecManager.CoreConfig.dev[0] = rootInnerDevice;
        }

        static void AddInnerDevice(Device parentDevice, Firesec.CoreConfig.devType parentInnerDevice)
        {
            List<Firesec.CoreConfig.devType> childInnerDevices = new List<Firesec.CoreConfig.devType>();

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
            Firesec.CoreConfig.devType innerDevice = new Firesec.CoreConfig.devType();
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
                List<Firesec.CoreConfig.inZType> zones = new List<Firesec.CoreConfig.inZType>();
                zones.Add(new Firesec.CoreConfig.inZType() { idz = device.ZoneNo });
                innerDevice.inZ = zones.ToArray();
            }

            innerDevice.prop = AddProperties(device).ToArray();

            return innerDevice;
        }

        static List<Firesec.CoreConfig.propType> AddProperties(Device device)
        {
            List<Firesec.CoreConfig.propType> propertyList = new List<Firesec.CoreConfig.propType>();

            if (device.Driver.DriverName != "Компьютер")
            {
                if ((device.Properties != null) && (device.Properties.Count > 0))
                {
                    foreach (var deviceProperty in device.Properties)
                    {
                        if ((string.IsNullOrEmpty(deviceProperty.Name) == false) && (string.IsNullOrEmpty(deviceProperty.Value)) == false)
                        {
                            //if ((device.Driver.propInfo != null) && (device.Driver.propInfo.Any(x => x.name == deviceProperty.Name)))
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

            if (device.ZoneLogic != null)
            {
                string zoneLogic = FiresecInternalClient.SetZoneLogic(device.ZoneLogic);
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
