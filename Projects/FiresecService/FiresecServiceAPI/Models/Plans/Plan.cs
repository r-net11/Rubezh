using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace FiresecAPI.Models
{
    [DataContract]
    public class Plan
    {
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
        public byte[] Pixels { get; set; }

        [DataMember]
        public double Width { get; set; }

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
