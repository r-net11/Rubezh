﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceApi;

namespace ComDevices
{
    public static class DeviceHelper
    {
        public static void SetDeviceInnerDevice(Device device, ComServer.CoreConfig.devType innerDevice)
        {
            if (innerDevice == null)
                return;

            string coreConfigDriverId = innerDevice.drv;
            device.Address = innerDevice.addr;
            device.DriverId = Services.Configuration.CoreConfig.drv.FirstOrDefault(x => x.idx == coreConfigDriverId).id;

            ComServer.Metadata.drvType metadataDriver = Services.Configuration.Metadata.drv.First(x => x.id == device.DriverId);
            if (metadataDriver.state != null)
            {
                device.States = new List<State>();
                foreach (ComServer.Metadata.stateType innerState in metadataDriver.state)
                {
                    State state = new State()
                    {
                        Id = innerState.id,
                        Name = innerState.name,
                        Priority = Convert.ToInt32(innerState.@class)
                    };
                    device.States.Add(state);
                }

                foreach (ComServer.Metadata.stateType innerState in metadataDriver.state)
                {
                    if (innerState.manualReset == "1")
                    {
                        if (device.AvailableFunctions == null)
                            device.AvailableFunctions = new List<string>();
                        device.AvailableFunctions.Add("Сброс " + innerState.name);
                    }
                }

                foreach (ComServer.Metadata.stateType innerState in metadataDriver.state)
                {
                    if (device.AvailableEvents == null)
                        device.AvailableEvents = new List<string>();
                    device.AvailableEvents.Add(innerState.name);
                    device.AvailableEvents.Add("Сброс " + innerState.name);
                }

            }
            if (metadataDriver.paramInfo != null)
                device.Parameters = new List<ComServer.Metadata.paramInfoType>(metadataDriver.paramInfo);
            if (metadataDriver.propInfo != null)
                device.Properties = new List<ComServer.Metadata.propInfoType>(metadataDriver.propInfo);

            SetAddress(device, innerDevice);
            SetPath(device);
            SetPlaceInTree(device);
        }

        static void SetAddress(Device device, ComServer.CoreConfig.devType innerDevice)
        {
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
    }
}
