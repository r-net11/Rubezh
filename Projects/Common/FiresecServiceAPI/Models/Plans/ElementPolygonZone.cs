using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ElementPolygonZone : ElementBasePolygon, IElementZone
    {
        public ElementPolygonZone()
        {
            BorderColor = Colors.Blue;
            BorderThickness = 1;
        }

        [DataMember]
        public ulong? ZoneNo { get; set; }

        public override FrameworkElement Draw()
        {
            var polygon = new Polygon()
            {
                Points = new PointCollection(PolygonPoints),
                Fill = new SolidColorBrush(BackgroundColor),
                Stroke = new SolidColorBrush(BorderColor),
                StrokeThickness = BorderThickness
            };
            return polygon;
        }

        public override ElementBase Clone()
        {
            ElementBase elementBase = new ElementPolygonZone()
            {
                ZoneNo = this.ZoneNo
            };
            Copy(elementBase);
            return elementBase;
        }
    }
}
