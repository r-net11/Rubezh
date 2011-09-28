using System.Runtime.Serialization;
using System.Windows.Media;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ElementSubPlan
    {
        public Plan Parent { get; set; }

        [DataMember]
        public int idElementCanvas;

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Caption { get; set; }

        [DataMember]
        public PointCollection PolygonPoints { get; set; }

        [DataMember]
        public string BackgroundSource { get; set; }

        [DataMember]
        public bool ShowBackgroundImage { get; set; }

        [DataMember]
        public string BorderColor { get; set; }
    }
}
