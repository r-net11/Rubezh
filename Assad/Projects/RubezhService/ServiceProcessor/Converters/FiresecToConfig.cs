using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceApi;
using FiresecMetadata;
using Firesec;

namespace ServiseProcessor
{
    public class FiresecToConfig
    {
        StateConfiguration stateConfiguration;
        Firesec.CoreConfig.config firesecConfig;

        public StateConfiguration Convert(Firesec.CoreConfig.config firesecConfig)
        {
            this.firesecConfig = firesecConfig;
            stateConfiguration = new StateConfiguration();

            Services.Configuration.ShortDevices = new List<ShortDevice>();
            Services.Configuration.ShortStates = new ShortStates();
            Services.Configuration.ShortStates.ShortDeviceStates = new List<ShortDeviceState>();
            Services.Configuration.ShortStates.ShortZoneStates = new List<ShortZoneState>();

            AddZones();

            Firesec.CoreConfig.devType rootInnerDevice = Services.Configuration.CoreConfig.dev[0];
            ShortDevice rootShortDevice = new ShortDevice();
            rootShortDevice.Parent = null;
            SetInnerDevice(rootShortDevice, rootInnerDevice);
            Services.Configuration.ShortDevices.Add(rootShortDevice);
            Services.Configuration.ShortStates.ShortDeviceStates.Add(CreateShortDeviceState(rootShortDevice));
            AddDevice(rootInnerDevice, rootShortDevice);

            stateConfiguration.RootShortDevice = rootShortDevice;

            return stateConfiguration;
        }

        void AddDevice(Firesec.CoreConfig.devType parentInnerDevice, ShortDevice parentDevice)
        {
            parentDevice.Children = new List<ShortDevice>();
            if (parentInnerDevice.dev != null)
            {
                foreach (Firesec.CoreConfig.devType innerDevice in parentInnerDevice.dev)
                {
                    ShortDevice shortDevice = new ShortDevice();
                    shortDevice.Parent = parentDevice;
                    SetInnerDevice(shortDevice, innerDevice);
                    
                    parentDevice.Children.Add(shortDevice);
                    Services.Configuration.ShortDevices.Add(shortDevice);
                    Services.Configuration.ShortStates.ShortDeviceStates.Add(CreateShortDeviceState(shortDevice));
                    AddDevice(innerDevice, shortDevice);
                }
            }
        }

        ShortDeviceState CreateShortDeviceState(ShortDevice shortDevice)
        {
            Firesec.Metadata.drvType metadataDriver = Services.Configuration.Metadata.drv.First(x => x.id == shortDevice.DriverId);

            ShortDeviceState shortDeviceState = new ShortDeviceState();
            shortDeviceState.Path = shortDevice.Path;
            shortDeviceState.PlaceInTree = shortDevice.PlaceInTree;

            shortDeviceState.InnerStates = new List<State>();
            foreach (Firesec.Metadata.stateType innerState in metadataDriver.state)
            {
                State state = new State()
                {
                    Id = innerState.id,
                    Name = innerState.name,
                    Priority = System.Convert.ToInt32(innerState.@class),
                    AffectChildren = innerState.affectChildren == "1" ? true : false
                };
                shortDeviceState.InnerStates.Add(state);
            }

            return shortDeviceState;
        }

        void SetInnerDevice(ShortDevice shortDevice, Firesec.CoreConfig.devType innerDevice)
        {
            shortDevice.DriverId = firesecConfig.drv.FirstOrDefault(x => x.idx == innerDevice.drv).id;
            Firesec.Metadata.drvType metadataDriver = Services.Configuration.Metadata.drv.First(x => x.id == shortDevice.DriverId);

            // addrMask="[8(1)-15(2)];[0(1)-7(255)]"
            shortDevice.Address = innerDevice.addr;
            if (metadataDriver.addrMask != null)
            {
                int intAddress = System.Convert.ToInt32(shortDevice.Address);
                if (intAddress > 255)
                {
                    int intShleifAddress = intAddress / 255;
                    int intSelfAddress = intAddress % 256;
                    shortDevice.Address = intShleifAddress.ToString() + "." + intSelfAddress.ToString();
                }
            }

            if (innerDevice.param != null)
            {
                if (innerDevice.param.Any(x => x.name == "DB$IDDevices"))
                {
                    shortDevice.DatabaseId = innerDevice.param.FirstOrDefault(x => x.name == "DB$IDDevices").value;
                }
            }

            shortDevice.DeviceProperties = new List<DeviceProperty>();
            if (innerDevice.prop != null)
            {
                foreach (Firesec.CoreConfig.propType innerProperty in innerDevice.prop)
                {
                    DeviceProperty deviceProperty = new DeviceProperty();
                    deviceProperty.Name = innerProperty.name;
                    deviceProperty.Value = innerProperty.value;
                    shortDevice.DeviceProperties.Add(deviceProperty);
                }
            }

            shortDevice.Description = innerDevice.name;

            SetAddress(shortDevice, innerDevice);
            SetPath(shortDevice);
            SetPlaceInTree(shortDevice);
            SetZone(shortDevice, innerDevice);
        }

        void SetAddress(ShortDevice shortDevice, Firesec.CoreConfig.devType innerDevice)
        {
            shortDevice.PresentationAddress = shortDevice.Address;
            string DriverName = DriversHelper.GetDriverNameById(shortDevice.DriverId);

            switch (DriverName)
            {
                case "Компьютер":
                    shortDevice.Address = "Компьютер";
                    shortDevice.PresentationAddress = "";
                    break;

                case "Насосная Станция":
                    shortDevice.Address = "Насосная Станция";
                    shortDevice.PresentationAddress = "";
                    break;

                case "Жокей-насос":
                    shortDevice.Address = "Жокей-насос";
                    shortDevice.PresentationAddress = "";
                    break;

                case "Компрессор":
                    shortDevice.Address = "Компрессор";
                    shortDevice.PresentationAddress = "";
                    break;

                case "Дренажный насос":
                    shortDevice.Address = "Дренажный насос";
                    shortDevice.PresentationAddress = "";
                    break;

                case "Насос компенсации утечек":
                    shortDevice.Address = "Насос компенсации утечек";
                    shortDevice.PresentationAddress = "";
                    break;

                case "USB преобразователь МС-1":
                case "USB преобразователь МС-2":
                    if (innerDevice.prop != null)
                    {
                        if (innerDevice.prop.Any(x => x.name == "SerialNo"))
                            shortDevice.Address = innerDevice.prop.FirstOrDefault(x => x.name == "SerialNo").value;
                    }
                    else
                        shortDevice.Address = "0";
                    shortDevice.PresentationAddress = "";
                    break;
            }
        }

        void SetPath(ShortDevice shortDevice)
        {
            string currentPath = shortDevice.DriverId + ":" + shortDevice.Address;
            if (shortDevice.Parent != null)
            {
                shortDevice.Path = shortDevice.Parent.Path + @"/" + currentPath;
            }
            else
            {
                shortDevice.Path = currentPath;
            }
        }

        void SetPlaceInTree(ShortDevice shortDevice)
        {
            if (shortDevice.Parent == null)
            {
                shortDevice.PlaceInTree = "";
            }
            else
            {
                string localPlaceInTree = (shortDevice.Parent.Children.Count - 1).ToString();
                if (shortDevice.Parent.PlaceInTree == "")
                {
                    shortDevice.PlaceInTree = localPlaceInTree;
                }
                else
                {
                    shortDevice.PlaceInTree = shortDevice.Parent.PlaceInTree + @"\" + localPlaceInTree;
                }
            }
        }

        void SetZone(ShortDevice shortDevice, Firesec.CoreConfig.devType innerDevice)
        {
            if (innerDevice.inZ != null)
            {
                string zoneIdx = innerDevice.inZ[0].idz;
                string zoneNo = firesecConfig.zone.FirstOrDefault(x => x.idx == zoneIdx).no;
                shortDevice.ZoneNo = zoneNo;
            }
            if (innerDevice.prop != null)
            {
                if (innerDevice.prop.Any(x => x.name == "ExtendedZoneLogic"))
                {
                    string zoleLogicstring = innerDevice.prop.FirstOrDefault(x => x.name == "ExtendedZoneLogic").value;
                    Firesec.ZoneLogic.expr ZoneLogic = FiresecClient.GetZoneLogic(zoleLogicstring);
                    if (ZoneLogic != null)
                    {
                        string logic = "";

                        foreach (Firesec.ZoneLogic.clauseType clause in ZoneLogic.clause)
                        {
                            if (clause.joinOperator != null)
                            {
                                switch (clause.joinOperator)
                                {
                                    case "and":
                                        logic += " и ";
                                        break;
                                    case "or":
                                        logic += " или ";
                                        break;
                                }
                            }

                            string stringState = "";
                            switch (clause.state)
                            {
                                case "0":
                                    stringState = "Включение автоматики";
                                    break;

                                case "1":
                                    stringState = "Тревога";
                                    break;

                                case "2":
                                    stringState = "Пожар";
                                    break;

                                case "5":
                                    stringState = "Внимание";
                                    break;

                                case "6":
                                    stringState = "Включение модуля пожаротушения";
                                    break;
                            }

                            string stringOperation = "";
                            switch (clause.operation)
                            {
                                case "and":
                                    stringOperation = "во всех зонах из";
                                    break;

                                case "or":
                                    stringOperation = "в любой зонах из";
                                    break;
                            }

                            logic += "состояние " + stringState + " " + stringOperation + " [";

                            foreach (string zoneNo in clause.zone)
                            {
                                logic += zoneNo + ", ";
                            }
                            logic += "]";
                        }
                    }
                }
            }
        }

        void AddZones()
        {
            stateConfiguration.ShortZones = new List<ShortZone>();

            if (firesecConfig.zone != null)
            {
                foreach (Firesec.CoreConfig.zoneType innerZone in firesecConfig.zone)
                {
                    ShortZone shortZone = new ShortZone();
                    shortZone.Name = innerZone.name;
                    shortZone.No = innerZone.no;
                    shortZone.Description = innerZone.desc;
                    if (innerZone.param != null)
                    {
                        if (innerZone.param.Any(x => x.name == "ExitTime"))
                            shortZone.EvacuationTime = innerZone.param.FirstOrDefault(x => x.name == "ExitTime").value;
                        if (innerZone.param.Any(x => x.name == "FireDeviceCount"))
                            shortZone.DetectorCount = innerZone.param.FirstOrDefault(x => x.name == "FireDeviceCount").value;
                    }
                    stateConfiguration.ShortZones.Add(shortZone);
                }
            }
        }
    }
}
