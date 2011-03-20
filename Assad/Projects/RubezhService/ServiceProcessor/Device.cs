using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firesec;
using System.Runtime.Serialization;
using ServiceApi;

namespace ServiseProcessor
{
    public class Device
    {
        public Device()
        {
            SelfStates = new List<string>();
            ParentStringStates = new List<string>();
            States = new List<State>();
        }

        public string Description { get; set; }
        public string DriverName { get; set; }
        public string DriverId { get; set; }
        public string ValidationError { get; set; }
        public string Zone { get; set; }
        public List<State> States { get; set; }
        public List<DeviceProperty> DeviceProperties { get; set; }
        public List<Parameter> Parameters { get; set; }
        public Firesec.CoreConfig.devType InnerDevice { get; set; }
        public List<string> AvailableFunctions { get; set; }
        public int MinPriority { get; set; }
        public string State { get; set; }
        public string SourceState { get; set; }
        public List<string> ParentStringStates { get; set; }
        public List<string> SelfStates { get; set; }
        public bool AffectChildren { get; set; }
        public List<State> ParentStates { get; set; }
        public string PlaceInTree { get; set; }
        public string Address { get; set; }
        public string PresentationAddress { get; set; }
        public bool StateChanged { get; set; }
        public bool StatesChanged { get; set; }
        public bool ParameterChanged { get; set; }
        public bool VisibleParameterChanged { get; set; }
        public string Path { get; set; }
        Firesec.CoreConfig.devType innerDevice;

        Device parent;
        public Device Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                if (parent != null)
                {
                    parent.Children.Add(this);
                }
            }
        }

        List<Device> children;
        public List<Device> Children
        {
            get
            {
                if (children == null)
                    children = new List<Device>();
                return children;
            }
            set
            {
                children = value;
            }
        }

        public void SetInnerDevice(Firesec.CoreConfig.devType innerDevice)
        {
            this.innerDevice = innerDevice;
            if (innerDevice == null)
                return;

            string coreConfigDriverId = innerDevice.drv;
            Address = innerDevice.addr;
            DriverId = Services.Configuration.CoreConfig.drv.FirstOrDefault(x => x.idx == coreConfigDriverId).id;

            Firesec.Metadata.drvType metadataDriver = Services.Configuration.Metadata.drv.First(x => x.id == DriverId);
            DriverName = FiresecMetadata.DriversHelper.GetDriverNameById(DriverId);
            States = new List<State>();
            if (metadataDriver.state != null)
            {
                foreach (Firesec.Metadata.stateType innerState in metadataDriver.state)
                {
                    State state = new State()
                    {
                        Id = innerState.id,
                        Name = innerState.name,
                        Priority = Convert.ToInt32(innerState.@class),
                        AffectChildren = innerState.affectChildren == "1" ? true : false
                    };
                    States.Add(state);
                }

                foreach (Firesec.Metadata.stateType innerState in metadataDriver.state)
                {
                    if (innerState.manualReset == "1")
                    {
                        if (AvailableFunctions == null)
                            AvailableFunctions = new List<string>();
                        AvailableFunctions.Add("Сброс " + innerState.name);
                    }
                }
            }

            Parameters = new List<Parameter>();
            if (metadataDriver.paramInfo != null)
            {
                foreach (Firesec.Metadata.paramInfoType paramInfo in metadataDriver.paramInfo)
                {
                    Parameter parameter = new Parameter();
                    parameter.Name = paramInfo.name;
                    parameter.Caption = paramInfo.caption;
                    parameter.Visible = ((paramInfo.hidden == "0") && (paramInfo.showOnlyInState == "0")) ? true : false;
                    Parameters.Add(parameter);
                }
            }

            SetAddress();
            SetPath();
            SetPlaceInTree();
            SetZone();
        }

        void SetAddress()
        {
            PresentationAddress = innerDevice.addr;

            switch (DriverName)
            {
                case "Компьютер":
                    Address = "Компьютер";
                    break;

                case "Насосная Станция":
                    Address = "Насосная Станция";
                    break;

                case "Жокей-насос":
                    Address = "Жокей-насос";
                    break;

                case "Компрессор":
                    Address = "Компрессор";
                    break;

                case "Дренажный насос":
                    Address = "Дренажный насос";
                    break;

                case "Насос компенсации утечек":
                    Address = "Насос компенсации утечек";
                    break;

                case "USB преобразователь МС-1":
                case "USB преобразователь МС-2":
                    if (innerDevice.prop != null)
                    {
                        if (innerDevice.prop.Any(x => x.name == "SerialNo"))
                            Address = innerDevice.prop.FirstOrDefault(x => x.name == "SerialNo").value;
                    }
                    else
                        Address = "0";
                    break;

                default:
                    Address = innerDevice.addr;
                    break;
            }
        }

        void SetPath()
        {
            string currentPath = DriverId + ":" + Address;
            if (Parent != null)
            {
                Path = Parent.Path + @"/" + currentPath;
            }
            else
            {
                Path = currentPath;
            }
        }

        void SetPlaceInTree()
        {
            if (Parent == null)
            {
                PlaceInTree = "";
            }
            else
            {
                string localPlaceInTree = (Parent.Children.Count - 1).ToString();
                if (Parent.PlaceInTree == "")
                {
                    PlaceInTree = localPlaceInTree;
                }
                else
                {
                    PlaceInTree = Parent.PlaceInTree + @"\" + localPlaceInTree;
                }
            }
        }

        void SetZone()
        {
            if (innerDevice.inZ != null)
            {
                string zoneId = innerDevice.inZ[0].idz;
                Zone = zoneId;
                Zone zone = Services.Configuration.Zones.FirstOrDefault(x => x.Id == zoneId);
                zone.Devices.Add(this);
            }
        }

        public ShortDevice ToShortDevice()
        {
            ShortDevice shortDevice = new ShortDevice();
            shortDevice.DriverId = this.DriverId;
            shortDevice.Address = this.Address;
            shortDevice.PresentationAddress = this.PresentationAddress;
            shortDevice.PlaceInTree = this.PlaceInTree;
            shortDevice.Path = this.Path;
            shortDevice.Description = this.Description;
            shortDevice.Zone = this.Zone;
            if (this.Parameters != null)
            {
                shortDevice.Parameters = new List<Parameter>();
                foreach (Parameter parameter in this.Parameters)
                {
                    shortDevice.Parameters.Add(new Parameter()
                    {
                        Name = parameter.Name,
                        Caption = parameter.Caption,
                        Visible = parameter.Visible,
                        Value = parameter.Value
                    });
                }
            }

            return shortDevice;
        }

        public ShortDeviceState ToShortDeviceState()
        {
            ShortDeviceState shortDeviceState = new ShortDeviceState();
            shortDeviceState.Path = this.Path;

            shortDeviceState.State = this.State;

            shortDeviceState.States = new List<string>();
            foreach (string state in this.SelfStates)
            {
                shortDeviceState.States.Add(state);
            }
            foreach (string parentState in this.ParentStringStates)
            {
                shortDeviceState.States.Add(parentState);
            }
            shortDeviceState.Parameters = new List<Parameter>();
            foreach (Parameter parameter in this.Parameters)
            {
                shortDeviceState.Parameters.Add(new Parameter() { Caption = parameter.Caption, Value = parameter.Value });
            }
            shortDeviceState.StateChanged = this.StateChanged;
            shortDeviceState.StatesChanged = this.StatesChanged;
            shortDeviceState.ParameterChanged = this.ParameterChanged;
            shortDeviceState.VisibleParameterChanged = this.VisibleParameterChanged;
            shortDeviceState.MustUpdate = ((this.StateChanged) || (this.StatesChanged) || (this.VisibleParameterChanged));

            return shortDeviceState;
        }
    }
}
