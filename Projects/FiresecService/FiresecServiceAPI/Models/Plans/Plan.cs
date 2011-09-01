using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.Models.Plans;

namespace FiresecAPI.Models
{
    [DataContract]
    public class Plan
    {
        public Plan()
        {
            Children = new List<Plan>();
            ElementSubPlans = new List<ElementSubPlan>();
            ElementZones = new List<ElementZone>();
            ElementDevices = new List<ElementDevice>();
            Rectangls = new List<RectangleBox>();
            TextBoxes = new List<CaptionBox>();
        }

        public Plan Parent { get; set; }

        [DataMember]
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
        public byte[] BackgroundPixels { get; set; }

        [DataMember]
        public double Width { get; set; }

        [DataMember]
        public double Height { get; set; }

        [DataMember]
        public List<ElementSubPlan> ElementSubPlans { get; set; }

        [DataMember]
        public List<ElementZone> ElementZones { get; set; }

        [DataMember]
        public List<RectangleBox> Rectangls { get; set; }

        [DataMember]
        public List<CaptionBox> TextBoxes { get; set; }

        [DataMember]
        public List<ElementDevice> ElementDevices { get; set; }
    }
}
