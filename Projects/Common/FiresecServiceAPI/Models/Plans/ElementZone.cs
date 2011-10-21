using System.Runtime.Serialization;
using System.Windows.Media;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ElementZone : ElementPolygon
    {
        public ElementZone()
        {
            BackgroundColor = Colors.Green;
            BorderColor = Colors.Blue;
            BorderThickness = 1;
            PolygonPoints = new PointCollection();
        }

        [DataMember]
        public ulong? ZoneNo { get; set; }
    }
}