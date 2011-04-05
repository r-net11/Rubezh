using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ServiceApi
{
    [DataContract]
    public class DeviceState
    {
        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public string PlaceInTree { get; set; }

        public List<InnerState> InnerStates { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public List<string> States { get; set; }

        [DataMember]
        public List<Parameter> Parameters { get; set; }

        [DataMember]
        public ChangeEntities ChangeEntities { get; set; }

        [DataMember]
        public List<string> SelfStates { get; set; }

        [IgnoreDataMember]
        public List<InnerState> ParentInnerStates { get; set; }

        [DataMember]
        public List<string> ParentStringStates { get; set; }

        [IgnoreDataMember]
        public int MinPriority { get; set; }

        [DataMember]
        public string SourceState { get; set; }
    }
}
