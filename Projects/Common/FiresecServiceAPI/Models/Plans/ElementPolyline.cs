using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ElementPolyline : ElementBasePointCollection, IZIndexedElement
    {
        public ElementPolyline()
        {
            BackgroundColor = Colors.Black;
            BorderThickness = 1;

            PolygonPoints = new PointCollection();
            PolygonPoints.Add(new Point(0, 0));
            PolygonPoints.Add(new Point(100, 100));
        }

        [DataMember]
        public Color BackgroundColor { get; set; }

        [DataMember]
        public double BorderThickness { get; set; }

        [DataMember]
        public int ZIndex { get; set; }

        public override FrameworkElement Draw()
        {
            var polyline = new Polyline()
            {
                Points = new PointCollection(PolygonPoints),
                Stroke = new SolidColorBrush(BackgroundColor),
                Fill = new SolidColorBrush(Colors.Transparent),
                StrokeThickness = BorderThickness
            };

            return polyline;
        }

        public override ElementBase Clone()
        {
            ElementPolyline elementLine = new ElementPolyline()
            {
                BackgroundColor = BackgroundColor,
                BorderThickness = BorderThickness,
                PolygonPoints = PolygonPoints.Clone()
            };
            Copy(elementLine);
            return elementLine;
        }
    }
}