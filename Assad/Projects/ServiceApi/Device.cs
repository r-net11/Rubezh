using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using ComServer;
using System.Runtime.Serialization;

namespace ServiceApi
{
    [DataContract(IsReference=true)]
    public class Device
    {
        [DataMember]
        public string Description { get; set; }

        public string DriverName
        {
            get
            {
                return Common.DriversHelper.GetDriverNameById(DriverId);
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

        [IgnoreDataMember]
        public List<ComServer.Metadata.paramInfoType> Parameters { get; set; }
        [IgnoreDataMember]
        public List<ComServer.Metadata.propInfoType> Properties { get; set; }

        [IgnoreDataMember]
        public ComServer.CoreConfig.devType InnerDevice { get; set; }

        public List<string> AvailableFunctions { get; set; }
        public List<string> AvailableEvents { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public int MinPriority { get; set; }

        [DataMember]
        public string SourceState { get; set; }

        [DataMember]
        public bool AffectChildren { get; set; }

        [DataMember]
        public List<string> LastEvents { get; set; }

        // свойство, по которому можно идентифицировать устройство в текущей конфигурации

        public string PlaceInTree { get; set; }

        [DataMember]
        public string Address { get; set; }

        // главное всойство, по которому можно идентифицировать устройство в системе

        [DataMember]
        public string Path { get; set; }

        public void ExecuteCommand(string commandName)
        {
            commandName = commandName.Remove(0, "Сброс ".Length);
            State comState = States.First(x => x.Name == commandName);
            string id = comState.Id;

            ComServer.CoreState.config coreState = new ComServer.CoreState.config();
            coreState.dev = new ComServer.CoreState.devType[1];
            coreState.dev[0] = new ComServer.CoreState.devType();
            coreState.dev[0].name = PlaceInTree;
            coreState.dev[0].state = new ComServer.CoreState.stateType[1];
            coreState.dev[0].state[0] = new ComServer.CoreState.stateType();
            coreState.dev[0].state[0].id = id;

            ComServer.ComServer.ResetStates(coreState);
        }

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
