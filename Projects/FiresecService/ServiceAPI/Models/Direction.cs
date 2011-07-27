using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecClient.Models
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
        public string DeviceRm { get; set; }

        [DataMember]
        public string DeviceButton { get; set; }
    }
}
