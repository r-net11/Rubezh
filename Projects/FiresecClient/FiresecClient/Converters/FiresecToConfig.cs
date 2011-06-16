using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firesec;
using System.Diagnostics;
using FiresecClient.Models;

namespace FiresecClient
{
    public class FiresecToConfig
    {
        Firesec.CoreConfig.config firesecConfig;

        public void Convert(Firesec.CoreConfig.config firesecConfig)
        {
            this.firesecConfig = firesecConfig;

            FiresecManager.Configuration.Devices = new List<Device>();
            FiresecManager.States = new CurrentStates();
            FiresecManager.States.DeviceStates = new List<DeviceState>();
            FiresecManager.States.ZoneStates = new List<ZoneState>();

            AddZones();

            Firesec.CoreConfig.devType rootInnerDevice = FiresecManager.CoreConfig.dev[0];
            Device rootDevice = new Device();
            rootDevice.Parent = null;
            SetInnerDevice(rootDevice, rootInnerDevice);
            FiresecManager.Configuration.Devices.Add(rootDevice);
            FiresecManager.States.DeviceStates.Add(CreateDeviceState(rootDevice));
            AddDevice(rootInnerDevice, rootDevice);

            FiresecManager.Configuration.RootDevice = rootDevice;

            AddDirections();
            AddSecurity();
        }

        void AddDevice(Firesec.CoreConfig.devType parentInnerDevice, Device parentDevice)
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

        DeviceState CreateDeviceState(Device device)
        {
            var driver = FiresecManager.Configuration.Metadata.drv.First(x => x.id == device.DriverId);

            DeviceState deviceState = new DeviceState();
            deviceState.ChangeEntities = new ChangeEntities();
            deviceState.Id = device.Id;
            deviceState.PlaceInTree = device.PlaceInTree;

            deviceState.InnerStates = new List<InnerState>();
            foreach (var innerState in driver.state)
            {
                InnerState state = new InnerState(innerState);
                deviceState.InnerStates.Add(state);
            }

            deviceState.Parameters = new List<Parameter>();
            if (driver.paramInfo != null)
                foreach (var parameterInfo in driver.paramInfo)
                {
                    Parameter parameter = new Parameter();
                    parameter.Name = parameterInfo.name;
                    parameter.Caption = parameterInfo.caption;
                    parameter.Visible = ((parameterInfo.hidden == "0") && (parameterInfo.showOnlyInState == "0"));
                    deviceState.Parameters.Add(parameter);
                }

            return deviceState;
        }

        void SetInnerDevice(Device device, Firesec.CoreConfig.devType innerDevice)
        {
            device.DriverId = firesecConfig.drv.FirstOrDefault(x => x.idx == innerDevice.drv).id;
            var metadataDriver = FiresecManager.Configuration.Metadata.drv.First(x => x.id == device.DriverId);
            device.Driver = metadataDriver;

            // addrMask="[8(1)-15(2)];[0(1)-7(255)]"
            device.Address = innerDevice.addr;
            if (metadataDriver.addrMask != null)
            {
                int intAddress = System.Convert.ToInt32(device.Address);
                if (intAddress > 255)
                {
                    int intShleifAddress = intAddress / 255;
                    int intSelfAddress = intAddress % 256;
                    device.Address = intShleifAddress.ToString() + "." + intSelfAddress.ToString();
                }
            }

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

            SetAddress(device, innerDevice);
            SetId(device);
            SetPlaceInTree(device);
            SetZone(device, innerDevice);
        }

        void SetAddress(Device device, Firesec.CoreConfig.devType innerDevice)
        {
            string DriverName = DriversHelper.GetDriverNameById(device.DriverId);

            switch (DriverName)
            {
                case "Компьютер":
                case "Насосная Станция":
                case "Жокей-насос":
                case "Компрессор":
                case "Дренажный насос":
                case "Насос компенсации утечек":
                    device.Address = "0";
                    break;

                case "USB преобразователь МС-1":
                case "USB преобразователь МС-2":
                    if (innerDevice.prop != null)
                    {
                        var serialNoProperty = innerDevice.prop.FirstOrDefault(x => x.name == "SerialNo");
                        if (serialNoProperty != null)
                            device.Address = serialNoProperty.value;
                    }
                    else
                        device.Address = "0";
                    break;
            }
        }

        void SetId(Device device)
        {
            string currentId = device.DriverId + ":" + device.Address;
            if (device.Parent != null)
            {
                device.Id = device.Parent.Id + @"/" + currentId;
            }
            else
            {
                device.Id = currentId;
            }
        }

        void SetPlaceInTree(Device device)
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

        void SetZone(Device device, Firesec.CoreConfig.devType innerDevice)
        {
            if (innerDevice.inZ != null)
            {
                string zoneIdx = innerDevice.inZ[0].idz;
                string zoneNo = firesecConfig.zone.FirstOrDefault(x => x.idx == zoneIdx).no;
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
                        Trace.WriteLine(device.Address);
                        Trace.WriteLine(zoneLogicstring);
                        device.ZoneLogic = FiresecInternalClient.GetZoneLogic(zoneLogicstring);
                    }
                }
            }
        }

        void AddZones()
        {
            FiresecManager.Configuration.Zones = new List<Zone>();

            if (firesecConfig.zone != null)
            {
                foreach (var innerZone in firesecConfig.zone)
                {
                    Zone zone = new Zone();
                    zone.Name = innerZone.name;
                    zone.No = innerZone.no;
                    zone.Description = innerZone.desc;
                    if (innerZone.param != null)
                    {
                        var exitTimeParam = innerZone.param.FirstOrDefault(x => x.name == "ExitTime");
                        if (exitTimeParam != null)
                            zone.EvacuationTime = exitTimeParam.value;

                        var fireDeviceCountParam = innerZone.param.FirstOrDefault(x => x.name == "FireDeviceCount");
                        if (fireDeviceCountParam != null)
                            zone.DetectorCount = fireDeviceCountParam.value;
                    }
                    FiresecManager.Configuration.Zones.Add(zone);
                    FiresecManager.States.ZoneStates.Add(CreateZoneState(zone));
                }
            }
        }

        ZoneState CreateZoneState(Zone zone)
        {
            ZoneState zoneState = new ZoneState();
            zoneState.No = zone.No;
            return zoneState;
        }

        void AddDirections()
        {
            FiresecManager.Configuration.Directions = new List<Direction>();

            if (FiresecManager.CoreConfig.part != null)
            {
                foreach (var part in FiresecManager.CoreConfig.part)
                {
                    if (part.type == "direction")
                    {
                        Direction direction = new Direction();
                        direction.Id = System.Convert.ToInt32(part.id);
                        direction.Name = part.name;
                        direction.Description = part.desc;

                        direction.Zones = new List<int>();
                        if (part.PinZ != null)
                        {
                            foreach (var partZone in part.PinZ)
                            {
                                direction.Zones.Add(System.Convert.ToInt32(partZone.pidz));
                            }
                        }

                        FiresecManager.Configuration.Directions.Add(direction);
                    }
                }
            }
        }

        void AddSecurity()
        {
            FiresecManager.Configuration.Users = new List<User>();
            FiresecManager.Configuration.UserGroups = new List<UserGroup>();
            FiresecManager.Configuration.Perimissions = new List<Perimission>();

            foreach (var firesecUser in FiresecManager.CoreConfig.user)
            {
                User user = new User();
                user.Id = firesecUser.param.value;
                user.Name = firesecUser.name;
                user.FullName = firesecUser.fullName;
                user.Password = firesecUser.password;
                user.IsBuiltIn = (firesecUser.builtin != "0");

                if (firesecUser.grp != null)
                {
                    //foreach (var groupIdx in firesecUser.grp)
                    var groupIdx = firesecUser.grp;
                    {
                        var firesecGroup = FiresecManager.CoreConfig.userGroup.FirstOrDefault(x => x.idx == groupIdx.idx);
                        user.Groups.Add(firesecGroup.param.value);
                    }
                }

                FiresecManager.Configuration.Users.Add(user);
            }

            foreach (var firesecUserGroup in FiresecManager.CoreConfig.userGroup)
            {
                UserGroup userGroup = new UserGroup();
                userGroup.Id = firesecUserGroup.param.value;
                userGroup.Name = firesecUserGroup.name;
                FiresecManager.Configuration.UserGroups.Add(userGroup);
            }

            var secObj = FiresecManager.CoreConfig.secObjType.FirstOrDefault(x => x.name == "Функции программы");
            foreach (var firesecPerimission in secObj.secAction)
            {
                Perimission perimission = new Perimission();
                perimission.Id = firesecPerimission.num;
                perimission.Name = firesecPerimission.name;
                FiresecManager.Configuration.Perimissions.Add(perimission);
            }

            foreach (var secRight in FiresecManager.CoreConfig.secGUI.FirstOrDefault(x => x.name == "Функции программы").secRight)
            {
                var permissionIdx = secRight.act;
                var permission = secObj.secAction.FirstOrDefault(x => x.idx == permissionIdx);
                var permissionId = permission.param.value;

                var idx = secRight.subj;
                var firesecUser = FiresecManager.CoreConfig.user.FirstOrDefault(x=>x.idx == idx);
                var firesecGroup = FiresecManager.CoreConfig.userGroup.FirstOrDefault(x => x.idx == idx);

                if (firesecUser != null)
                {
                    string userId = firesecUser.param.value;
                    var user = FiresecManager.Configuration.Users.FirstOrDefault(x => x.Id == userId);
                    if (user != null)
                    user.Permissions.Add(permissionId);
                }

                if (firesecGroup != null)
                {
                    string groupId = firesecGroup.param.value;
                    var group = FiresecManager.Configuration.UserGroups.FirstOrDefault(x => x.Id == groupId);
                    if (group != null)
                    group.Permissions.Add(permissionId);
                }
            }
        }
    }
}
