using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firesec;
using System.Diagnostics;

namespace FiresecClient
{
    public class FiresecToConfig
    {
        Firesec.CoreConfig.config firesecConfig;

        public void Convert(Firesec.CoreConfig.config firesecConfig)
        {
            this.firesecConfig = firesecConfig;

            FiresecManager.CurrentConfiguration.AllDevices = new List<Device>();
            FiresecManager.CurrentStates = new CurrentStates();
            FiresecManager.CurrentStates.DeviceStates = new List<DeviceState>();
            FiresecManager.CurrentStates.ZoneStates = new List<ZoneState>();

            AddZones();

            Firesec.CoreConfig.devType rootInnerDevice = FiresecManager.CoreConfig.dev[0];
            Device rootDevice = new Device();
            rootDevice.Parent = null;
            SetInnerDevice(rootDevice, rootInnerDevice);
            FiresecManager.CurrentConfiguration.AllDevices.Add(rootDevice);
            FiresecManager.CurrentStates.DeviceStates.Add(CreateDeviceState(rootDevice));
            AddDevice(rootInnerDevice, rootDevice);

            FiresecManager.CurrentConfiguration.RootDevice = rootDevice;
        }

        void AddDevice(Firesec.CoreConfig.devType parentInnerDevice, Device parentDevice)
        {
            parentDevice.Children = new List<Device>();
            if (parentInnerDevice.dev != null)
            {
                foreach (Firesec.CoreConfig.devType innerDevice in parentInnerDevice.dev)
                {
                    Device device = new Device();
                    device.Parent = parentDevice;
                    parentDevice.Children.Add(device);
                    SetInnerDevice(device, innerDevice);
                    FiresecManager.CurrentConfiguration.AllDevices.Add(device);
                    FiresecManager.CurrentStates.DeviceStates.Add(CreateDeviceState(device));
                    AddDevice(innerDevice, device);
                }
            }
        }

        DeviceState CreateDeviceState(Device device)
        {
            Firesec.Metadata.drvType driver = FiresecManager.CurrentConfiguration.Metadata.drv.First(x => x.id == device.DriverId);

            DeviceState deviceState = new DeviceState();
            deviceState.ChangeEntities = new ChangeEntities();
            deviceState.Id = device.Id;
            deviceState.PlaceInTree = device.PlaceInTree;

            deviceState.InnerStates = new List<InnerState>();
            foreach (Firesec.Metadata.stateType innerState in driver.state)
            {
                InnerState state = new InnerState()
                {
                    Id = innerState.id,
                    Name = innerState.name,
                    Priority = System.Convert.ToInt32(innerState.@class),
                    AffectChildren = innerState.affectChildren == "1" ? true : false
                };
                deviceState.InnerStates.Add(state);
            }

            deviceState.Parameters = new List<Parameter>();
            if (driver.paramInfo != null)
            foreach (Firesec.Metadata.paramInfoType parameterInfo in driver.paramInfo)
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
            Firesec.Metadata.drvType metadataDriver = FiresecManager.CurrentConfiguration.Metadata.drv.First(x => x.id == device.DriverId);

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
                if (innerDevice.param.Any(x => x.name == "DB$IDDevices"))
                {
                    device.DatabaseId = innerDevice.param.FirstOrDefault(x => x.name == "DB$IDDevices").value;
                }
            }

            device.Properties = new List<Property>();
            if (innerDevice.prop != null)
            {
                foreach (Firesec.CoreConfig.propType innerProperty in innerDevice.prop)
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
                        if (innerDevice.prop.Any(x => x.name == "SerialNo"))
                            device.Address = innerDevice.prop.FirstOrDefault(x => x.name == "SerialNo").value;
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
            FiresecManager.CurrentConfiguration.Zones = new List<Zone>();

            if (firesecConfig.zone != null)
            {
                foreach (Firesec.CoreConfig.zoneType innerZone in firesecConfig.zone)
                {
                    Zone zone = new Zone();
                    zone.Name = innerZone.name;
                    zone.No = innerZone.no;
                    zone.Description = innerZone.desc;
                    if (innerZone.param != null)
                    {
                        if (innerZone.param.Any(x => x.name == "ExitTime"))
                            zone.EvacuationTime = innerZone.param.FirstOrDefault(x => x.name == "ExitTime").value;
                        if (innerZone.param.Any(x => x.name == "FireDeviceCount"))
                            zone.DetectorCount = innerZone.param.FirstOrDefault(x => x.name == "FireDeviceCount").value;
                    }
                    FiresecManager.CurrentConfiguration.Zones.Add(zone);
                    FiresecManager.CurrentStates.ZoneStates.Add(CreateZoneState(zone));
                }
            }
        }

        ZoneState CreateZoneState(Zone zone)
        {
            ZoneState zoneState = new ZoneState();
            zoneState.No = zone.No;
            return zoneState;
        }
    }
}
