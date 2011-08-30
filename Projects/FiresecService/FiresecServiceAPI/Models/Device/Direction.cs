using System.Collections.Generic;
using System.Runtime.Serialization;
using System;

namespace FiresecAPI.Models
{
    [DataContract]
    public class Direction
    {
        public Direction()
        {
            Zones = new List<string>();
        }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Gid { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public List<string> Zones { get; set; }

        [DataMember]
        public Guid DeviceRm { get; set; }

        [DataMember]
        public Guid DeviceButton { get; set; }
    }
}