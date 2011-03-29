using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ServiceApi
{
    [DataContract]
    public class ShortDeviceState
    {
        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public string PlaceInTree { get; set; }

        public List<State> InnerStates { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public List<string> States { get; set; }

        [DataMember]
        public List<Parameter> Parameters { get; set; }

        [DataMember]
        public bool StateChanged { get; set; }

        [DataMember]
        public bool StatesChanged { get; set; }

        [DataMember]
        public bool ParameterChanged { get; set; }

        [DataMember]
        public bool VisibleParameterChanged { get; set; }

        [DataMember]
        public bool MustUpdate { get; set; }

        [DataMember]
        public bool IsNewEvent { get; set; }

        [IgnoreDataMember]
        public List<string> SelfStates { get; set; }

        [IgnoreDataMember]
        public List<State> ParentStates { get; set; }

        [IgnoreDataMember]
        public List<string> ParentStringStates { get; set; }

        [IgnoreDataMember]
        public int MinPriority { get; set; }

        [DataMember]
        public string SourceState { get; set; }
    }
}
