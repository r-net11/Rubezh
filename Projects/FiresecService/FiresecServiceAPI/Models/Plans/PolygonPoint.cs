using System.Xml.Serialization;

namespace FiresecAPI.Models
{
    public class PolygonPoint
    {
        [XmlAttribute]
        public double X { get; set; }
        [XmlAttribute]
        public double Y { get; set; }
    }
}
