using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ElementPolygonZone : ElementBasePolygon, IElementZone
    {
        public Zone Zone { get; set; }

        [DataMember]
        public ulong? ZoneNo { get; set; }

        public override FrameworkElement Draw()
        {
            var polygon = new Polygon()
            {
                Points = new PointCollection(PolygonPoints),
                Fill = new SolidColorBrush(ElementZoneHelper.GetZoneColor(Zone)),
                Stroke = new SolidColorBrush(Colors.Blue),
                StrokeThickness = 1
            };
            return polygon;
        }

        public override ElementBase Clone()
        {
            ElementBase elementBase = new ElementPolygonZone()
            {
                ZoneNo = ZoneNo
            };
            Copy(elementBase);
            return elementBase;
        }
    }
}
