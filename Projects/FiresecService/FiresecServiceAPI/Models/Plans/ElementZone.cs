using System.Collections.Generic;
using System.Xml.Serialization;

namespace FiresecAPI.Models
{
    public class ElementZone
    {
        public List<PolygonPoint> PolygonPoints { get; set; }
        [XmlAttribute]
        public string ZoneNo { get; set; }
    }
}
