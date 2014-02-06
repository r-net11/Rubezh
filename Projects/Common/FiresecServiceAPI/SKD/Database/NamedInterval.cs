using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
    [DataContract]
    public class NamedInterval
	{
        [DataMember]
        public Guid Uid { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public List<Interval> Intervals { get; set; }
	}
}
