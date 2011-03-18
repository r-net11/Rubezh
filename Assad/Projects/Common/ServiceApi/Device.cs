using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Firesec;
using System.Runtime.Serialization;

namespace ServiceApi
{
    [DataContract(IsReference=true)]
    public class Device
    {
        public Device()
        {
            SelfStates = new List<string>();
            ParentStringStates = new List<string>();
            States = new List<State>();
        }

        [DataMember]
        public string Description { get; set; }

        public string DriverName
        {
            get
            {
                return FiresecMetadata.DriversHelper.GetDriverNameById(DriverId);
            }
        }

        [DataMember]
        public string DriverId { get; set; }

        [DataMember]
        public string ValidationError { get; set; }

        [DataMember]
        public List<string> Zones { get; set; }

        [DataMember]
        public List<State> States { get; set; }

        [DataMember]
        public List<DeviceProperty> DeviceProperties { get; set; }

        [DataMember]
        public List<Parameter> Parameters { get; set; }

        [IgnoreDataMember]
        public Firesec.CoreConfig.devType InnerDevice { get; set; }

        [DataMember]
        public List<string> AvailableFunctions { get; set; }

        [DataMember]
        public int MinPriority { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string SourceState { get; set; }

        [DataMember]
        public List<string> ParentStringStates { get; set; }

        [DataMember]
        public List<string> SelfStates { get; set; }

        [DataMember]
        public bool AffectChildren { get; set; }

        public List<State> ParentStates { get; set; }

        // свойство, по которому можно идентифицировать устройство в текущей конфигурации

        public string PlaceInTree { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public bool StateChanged { get; set; }

        [DataMember]
        public bool StatesChanged { get; set; }

        [DataMember]
        public bool ParameterChanged { get; set; }

        [DataMember]
        public bool VisibleParameterChanged { get; set; }

        // главное всойство, по которому можно идентифицировать устройство в системе

        [DataMember]
        public string Path { get; set; }

        Device parent;
        [DataMember]
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
        [DataMember]
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
    }
}
