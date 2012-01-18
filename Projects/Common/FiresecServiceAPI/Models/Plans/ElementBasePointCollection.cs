using System;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;

namespace FiresecAPI.Models
{
    [DataContract]
    public abstract class ElementBasePointCollection : ElementBase
    {
        public ElementBasePointCollection()
        {
            PolygonPoints = new PointCollection();
        }

        [DataMember]
        public PointCollection PolygonPoints { get; set; }

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
            Left = minLeft;
            Top = minTop;
            Width = maxLeft - minLeft;
            Height = maxTop - minTop;
        }
    }
}
