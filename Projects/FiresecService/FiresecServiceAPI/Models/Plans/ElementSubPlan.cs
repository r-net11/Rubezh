using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    public class ElementSubPlan
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Caption { get; set; }

        [DataMember]
        public List<PolygonPoint> PolygonPoints { get; set; }

        [DataMember]
        public string BackgroundSource { get; set; }

        [DataMember]
        public bool ShowBackgroundImage { get; set; }

        [DataMember]
        public string BorderColor { get; set; }
    }
}
