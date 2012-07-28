using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class Direction
    {
        public Direction()
        {
			Zones = new List<int>();
        }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public Guid DeviceRm { get; set; }

        [DataMember]
        public Guid DeviceButton { get; set; }

        [DataMember]
		public List<int> Zones { get; set; }
    }
}