using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace PlansModule.Models
{
    public class ElementDevice
    {
        [XmlAttribute]
        public double Left { get; set; }
        [XmlAttribute]
        public double Top { get; set; }
        [XmlAttribute]
        public string Path { get; set; }
    }
}
