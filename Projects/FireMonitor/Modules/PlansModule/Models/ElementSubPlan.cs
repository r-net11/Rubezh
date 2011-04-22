using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace PlansModule.Models
{
    public class ElementSubPlan
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Caption { get; set; }
        public List<PolygonPoint> PolygonPoints { get; set; }
        [XmlAttribute]
        public string BackgroundSource { get; set; }
        [XmlAttribute]
        public bool ShowBackgroundImage { get; set; }
        [XmlAttribute]
        public string BorderColor { get; set; }
    }
}
