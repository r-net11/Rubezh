using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models.Skud
{
    [DataContract]
    public class Day
	{
        [DataMember]
        public Guid Uid { get; set; }
        [DataMember]
        public NamedInterval NamedInterval { get; set; }
        [DataMember]
        public int? Number { get; set; }
	}
}
