using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models.Skud
{
    [DataContract]
    public class Position
	{
        [DataMember]
        public Guid Uid;
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
	}
}
