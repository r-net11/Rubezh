using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace PlansModule
{
    [Serializable]
    public class Plan2
    {
        public Plan2 Parent { get; set; }
        public List<Plan2> Children { get; set; }

        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Caption { get; set; }
        [XmlAttribute]
        public string BackgroundImageName { get; set; }
        [XmlAttribute]
        public double Width { get; set; }
        [XmlAttribute]
        public double Height { get; set; }

        public List<SubPlan> SubPlans { get; set; }
        public List<PlanZone> PlanZones { get; set; }
        public List<PlanDevice> PlanDevices { get; set; }
    }

    public class SubPlan
    {
        public List<PolygonPoint> PolygonPoints { get; set; }
        [XmlAttribute]
        public string BackgroundImageName { get; set; }
    }

    public class PlanZone
    {
        public List<PolygonPoint> PolygonPoints { get; set; }
        [XmlAttribute]
        public string ZoneNo { get; set; }
    }

    public class PlanDevice
    {
        [XmlAttribute]
        public double Left { get; set; }
        [XmlAttribute]
        public double Top { get; set; }
        [XmlAttribute]
        public string Driver { get; set; }
    }

    public class PolygonPoint
    {
        [XmlAttribute]
        public double X { get; set; }
        [XmlAttribute]
        public double Y { get; set; }
    }
}
