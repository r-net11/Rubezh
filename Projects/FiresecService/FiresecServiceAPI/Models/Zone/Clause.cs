using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class Clause
    {
        public Clause()
        {
            Zones = new List<string>();
        }

        [DataMember]
        public ZoneLogicState State { get; set; }

        [DataMember]
        public ZoneLogicOperation Operation { get; set; }

        [DataMember]
        public List<string> Zones { get; set; }
    }
}
