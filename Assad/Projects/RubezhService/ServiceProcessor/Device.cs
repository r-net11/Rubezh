using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
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
        public List<string> Zones { get; set; }
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
        public bool StateChanged { get; set; }
        public bool StatesChanged { get; set; }
        public bool ParameterChanged { get; set; }
        public bool VisibleParameterChanged { get; set; }
        public string Path { get; set; }

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

        public ShortDevice ToShortDevice()
        {
            ShortDevice shortDevice = new ShortDevice();
            shortDevice.DriverId = this.DriverId;
            shortDevice.Address = this.Address;
            shortDevice.PlaceInTree = this.PlaceInTree;
            shortDevice.Path = this.Path;
            shortDevice.Description = this.Description;
            if ((this.Zones != null) && (this.Zones.Count > 0))
            {
            shortDevice.Zone = this.Zones[0];
            }
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
