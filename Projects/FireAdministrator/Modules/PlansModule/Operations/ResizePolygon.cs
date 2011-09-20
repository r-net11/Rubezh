using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using FiresecAPI.Models;

namespace PlansModule.Resize
{
    public static class PolygonResizer
    {
        static UIElement ResizeElement = null;
        static UIElement ResizeElementTextBox = null;
        static Thumb Thumb = null;
        static Canvas Canvas;
        static Polygon Polygon;
        public static Plan Plan;

        public static void SetCanvas(Canvas canvas)
        {
            Canvas = canvas;
        }

        public static void SetElementResize(UIElement element, Thumb thumb, UIElement textbox)
        {
            double test = Canvas.GetLeft(element);
            if (ResizeElement != null)
                test = Canvas.GetLeft(ResizeElement);
            ResizeElement = element;
            test = Canvas.GetLeft(ResizeElement);
            Thumb = thumb;
            if (textbox != null) ResizeElementTextBox = textbox;
            GetTypeElement();
        }

        public static void RemovePolygon()
        {
            Canvas.Children.Remove(Polygon);
            Polygon = null;
            ResizeElement = null;
        }

        public static PointCollection GetPolygonResize()
        {
            return Polygon.Points;
        }

        static void GetTypeElement()
        {
            if (ResizeElement is Polygon)
            {
                ResizePolygon(ResizeElement as Polygon);
            }
        }

        static void ResizePolygon(Polygon polygon)
        {
            if (Polygon == null)
            {
                Polygon = new Polygon() { Name = polygon.Name };
            }

            Polygon.Stroke = System.Windows.Media.Brushes.Red;
            Polygon.Points.Clear();
            int indexThumb = int.Parse(Thumb.Name.Substring(5));
            for (int i = 0; i < polygon.Points.Count; ++i)
            {
                if (i == indexThumb)
                {
                    var x = Canvas.GetLeft(Thumb) - Canvas.GetLeft(polygon);
                    var y = Canvas.GetTop(Thumb) - Canvas.GetTop(polygon);
                    Polygon.Points.Add(new Point(x, y));
                }
                else
                {
                    Polygon.Points.Add(new Point(polygon.Points[i].X, polygon.Points[i].Y));
                }
            }
            Canvas.SetLeft(Polygon, Canvas.GetLeft(polygon));
            Canvas.SetTop(Polygon, Canvas.GetTop(polygon));
            if (Canvas.Children.IndexOf(Polygon) == -1)
                Canvas.Children.Add(Polygon);
        }
    }
}