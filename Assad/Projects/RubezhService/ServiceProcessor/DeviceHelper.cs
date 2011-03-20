using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceApi;

namespace ServiseProcessor
{
    public static class DeviceHelper
    {
        public static void SetInnerDevice(Device device, Firesec.CoreConfig.devType innerDevice)
        {
            if (innerDevice == null)
                return;

            string coreConfigDriverId = innerDevice.drv;
            device.Address = innerDevice.addr;
            device.DriverId = Services.Configuration.CoreConfig.drv.FirstOrDefault(x => x.idx == coreConfigDriverId).id;

            Firesec.Metadata.drvType metadataDriver = Services.Configuration.Metadata.drv.First(x => x.id == device.DriverId);
            device.DriverName = FiresecMetadata.DriversHelper.GetDriverNameById(device.DriverId);
            if (metadataDriver.state != null)
            {
                device.States = new List<State>();
                foreach (Firesec.Metadata.stateType innerState in metadataDriver.state)
                {
                    State state = new State()
                    {
                        Id = innerState.id,
                        Name = innerState.name,
                        Priority = Convert.ToInt32(innerState.@class),
                        AffectChildren = innerState.affectChildren == "1" ? true : false
                    };
                    device.States.Add(state);
                }

                foreach (Firesec.Metadata.stateType innerState in metadataDriver.state)
                {
                    if (innerState.manualReset == "1")
                    {
                        if (device.AvailableFunctions == null)
                            device.AvailableFunctions = new List<string>();
                        device.AvailableFunctions.Add("Сброс " + innerState.name);
                    }
                }
            }

            device.Parameters = new List<Parameter>();
            if (metadataDriver.paramInfo != null)
            {
                foreach (Firesec.Metadata.paramInfoType paramInfo in metadataDriver.paramInfo)
                {
                    Parameter parameter = new Parameter();
                    parameter.Name = paramInfo.name;
                    parameter.Caption = paramInfo.caption;
                    parameter.Visible = ((paramInfo.hidden == "0") && (paramInfo.showOnlyInState == "0")) ? true : false;
                    device.Parameters.Add(parameter);
                }
            }

            SetAddress(device, innerDevice);
            SetPath(device);
            SetPlaceInTree(device);
            SetZone(device, innerDevice);
        }

        static void SetAddress(Device device, Firesec.CoreConfig.devType innerDevice)
        {
            device.PresentationAddress = innerDevice.addr;

            switch (device.DriverName)
            {
                case "Компьютер":
                    device.Address = "Компьютер";
                    break;

                case "Насосная Станция":
                    device.Address = "Насосная Станция";
                    break;

                case "Жокей-насос":
                    device.Address = "Жокей-насос";
                    break;

                case "Компрессор":
                    device.Address = "Компрессор";
                    break;

                case "Дренажный насос":
                    device.Address = "Дренажный насос";
                    break;

                case "Насос компенсации утечек":
                    device.Address = "Насос компенсации утечек";
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

                default:
                    device.Address = innerDevice.addr;
                    break;
            }
        }

        static void SetPath(Device device)
        {
            string currentPath = device.DriverId + ":" + device.Address;
            if (device.Parent != null)
            {
                device.Path = device.Parent.Path + @"/" + currentPath;
            }
            else
            {
                device.Path = currentPath;
            }
        }

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
            device.Zones = new List<string>();
            if (innerDevice.inZ != null)
            {
                foreach (Firesec.CoreConfig.inZType innerZone in innerDevice.inZ)
                {
                    string zoneId = innerZone.idz;
                    device.Zones.Add(zoneId);
                    Zone zone = Services.Configuration.Zones.FirstOrDefault(x => x.Id == zoneId);
                    zone.Devices.Add(device);
                }
            }
        }
    }
}
