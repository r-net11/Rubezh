using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FiresecAPI.Models
{
    [Serializable]
    public class Plan
    {
        public Plan Parent { get; set; }
        public List<Plan> Children { get; set; }

        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Caption { get; set; }
        [XmlAttribute]
        public string Description { get; set; }
        [XmlAttribute]
        public string BackgroundSource { get; set; }
        [XmlAttribute]
        public bool ShowBackgroundImage { get; set; }
        [XmlAttribute]
        public double Width { get; set; }
        [XmlAttribute]
        public double Height { get; set; }

        public List<ElementSubPlan> ElementSubPlans { get; set; }
        public List<ElementZone> ElementZones { get; set; }
        public List<ElementDevice> ElementDevices { get; set; }
    }
}
