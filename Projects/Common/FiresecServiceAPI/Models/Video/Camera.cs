using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class Camera
    {
        public Camera()
        {
            Zones = new List<ulong>();
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public List<ulong> Zones { get; set; }
    }
}