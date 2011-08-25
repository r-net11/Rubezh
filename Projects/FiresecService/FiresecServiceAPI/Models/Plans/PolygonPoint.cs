using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    public class PolygonPoint
    {
        //[XmlAttribute]
        [DataMember]
        public double X { get; set; }
        //[XmlAttribute]
        [DataMember]
        public double Y { get; set; }
    }
}
