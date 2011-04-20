using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace PlansModule.Models
{
    public class ElementZone
    {
        public List<PolygonPoint> PolygonPoints { get; set; }
        [XmlAttribute]
        public string ZoneNo { get; set; }
    }
}
