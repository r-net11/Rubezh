using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FiresecAPI.Models
{
    [DataContract]
    public abstract class ElementBasePolygon : ElementBasePointCollection
    {
        public ElementBasePolygon()
        {
            BackgroundColor = Colors.DarkRed;
            BorderColor = Colors.Orange;
            BorderThickness = 1;

            PolygonPoints = new PointCollection();
            PolygonPoints.Add(new Point(0, 0));
            PolygonPoints.Add(new Point(50, 0));
            PolygonPoints.Add(new Point(50, 50));
            PolygonPoints.Add(new Point(0, 50));
        }

        [DataMember]
        public byte[] BackgroundPixels { get; set; }

        [DataMember]
        public Color BackgroundColor { get; set; }

        [DataMember]
        public Color BorderColor { get; set; }

        [DataMember]
        public double BorderThickness { get; set; }

        public override FrameworkElement Draw()
        {
            var polygon = new Polygon()
            {
                Points = new PointCollection(PolygonPoints),
                Fill = new SolidColorBrush(BackgroundColor),
                Stroke = new SolidColorBrush(BorderColor),
                StrokeThickness = BorderThickness
            };

            if (BackgroundPixels != null)
            {
                polygon.Fill = PlanElementsHelper.CreateBrush(BackgroundPixels);
            }

            return polygon;
        }

        protected override void Copy(ElementBase elementBase)
        {
            ElementBasePolygon elementBasePolygon = elementBase as ElementBasePolygon;
            elementBasePolygon.BackgroundColor = BackgroundColor;
            elementBasePolygon.BorderColor = BorderColor;
            elementBasePolygon.BorderThickness = BorderThickness;
            elementBasePolygon.PolygonPoints = PolygonPoints.Clone();
            base.Copy(elementBasePolygon);
        }
    }
}