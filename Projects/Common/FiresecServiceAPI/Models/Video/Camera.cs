using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class Camera
    {
        public Camera()
        {
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public ulong? ZoneNo { get; set; }
    }
}