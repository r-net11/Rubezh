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
        public string Sate { get; set; }

        [DataMember]
        public List<string> States { get; set; }

        [DataMember]
        public List<Parameter> Parameters { get; set; }

        [DataMember]
        public bool MustUpdate { get; set; }
    }
}
