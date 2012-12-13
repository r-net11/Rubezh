using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MultiClientRunner
{
    [DataContract]
    public class MulticlientData
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Login { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string RemoteAddress { get; set; }

        [DataMember]
        public int RemotePort { get; set; }
    }
}