using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class Plan
    {
        public Plan Parent { get; set; }
        public List<Plan> Children { get; set; }

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Caption { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string BackgroundSource { get; set; }
        [DataMember]
        public bool ShowBackgroundImage { get; set; }
        [DataMember]
        public double Width { get; set; }
        [DataMember]
        public double Height { get; set; }

        public List<ElementSubPlan> ElementSubPlans { get; set; }
        public List<ElementZone> ElementZones { get; set; }
        public List<ElementDevice> ElementDevices { get; set; }
    }
}
