using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    public class ElementZone
    {
        [DataMember]
        public List<PolygonPoint> PolygonPoints { get; set; }

        [DataMember]
        public string ZoneNo { get; set; }
    }
}
