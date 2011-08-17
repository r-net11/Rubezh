using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    public class ElementSubPlan
    {
        //[XmlAttribute]
        [DataMember]
        public string Name { get; set; }
        //[XmlAttribute]
        [DataMember]
        public string Caption { get; set; }
        [DataMember]
        public List<PolygonPoint> PolygonPoints { get; set; }
        //[XmlAttribute]
        [DataMember]
        public string BackgroundSource { get; set; }
        //[XmlAttribute]
        [DataMember]
        public bool ShowBackgroundImage { get; set; }
        //[XmlAttribute]
        [DataMember]
        public string BorderColor { get; set; }
    }
}
