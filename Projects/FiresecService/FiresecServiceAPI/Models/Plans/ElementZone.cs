using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ElementZone
    {
        public int idElementCanvas;

        [DataMember]
        public List<PolygonPoint> PolygonPoints { get; set; }

        [DataMember]
        public string ZoneNo { get; set; }
    }
}
