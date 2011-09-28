using System.Runtime.Serialization;
using System.Windows.Media;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ElementZone
    {
        public ElementZone()
        {
            PolygonPoints = new PointCollection();
        }

        public int idElementCanvas;

        [DataMember]
        public PointCollection PolygonPoints { get; set; }

        [DataMember]
        public string ZoneNo { get; set; }
    }
}
