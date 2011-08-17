using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;


namespace FiresecAPI.Models
{
    //[Serializable]
    [DataContract]
    public class Plan
    {
        public Plan Parent { get; set; }
        [DataMember]
        public List<Plan> Children { get; set; }

        //[XmlAttribute]
        [DataMember]
        public string Name { get; set; }
        //[XmlAttribute]
        [DataMember]
        public string Caption { get; set; }
        //[XmlAttribute]
        [DataMember]
        public string Description { get; set; }
        //[XmlAttribute]
        [DataMember]
        public string BackgroundSource { get; set; }
        //[XmlAttribute]
        [DataMember]
        public bool ShowBackgroundImage { get; set; }
        //[XmlAttribute]
        [DataMember]
        public double Width { get; set; }
        //[XmlAttribute]
        [DataMember]
        public double Height { get; set; }
        [DataMember]
        public List<ElementSubPlan> ElementSubPlans { get; set; }
        [DataMember]
        public List<ElementZone> ElementZones { get; set; }
        [DataMember]
        public List<ElementDevice> ElementDevices { get; set; }
    }
}
