using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    public class ElementZone
    {
        [DataMember]
        public List<PolygonPoint> PolygonPoints { get; set; }
        [DataMember]
        //[XmlAttribute]
        public string ZoneNo { get; set; }
    }
}
