using System;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ElementPolygon : ElementBase
    {
        public ElementPolygon()
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
        public PointCollection PolygonPoints { get; set; }

        [DataMember]
        public byte[] BackgroundPixels { get; set; }

        [DataMember]
        public Color BackgroundColor { get; set; }

        [DataMember]
        public Color BorderColor { get; set; }

        [DataMember]
        public double BorderThickness { get; set; }

        public void Normalize()
        {
            double minLeft = double.MaxValue;
            double minTop = double.MaxValue;
            double maxLeft = 0;
            double maxTop = 0;

            foreach (var point in PolygonPoints)
            {
                minLeft = Math.Min(point.X, minLeft);
                minTop = Math.Min(point.Y, minTop);
                maxLeft = Math.Max(point.X, maxLeft);
                maxTop = Math.Max(point.Y, maxTop);
            }

            var pointCollection = new PointCollection();
            foreach (var point in PolygonPoints)
            {
                pointCollection.Add(new Point(point.X - minLeft, point.Y - minTop));
            }

            PolygonPoints = new PointCollection(pointCollection);
            //Left = Math.Min(Left, minLeft);
            //Top = Math.Min(Top, minTop);
            Left = minLeft;
            Top = minTop;
            Width = maxLeft - minLeft;
            Height = maxTop - minTop;
        }

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
    }
}