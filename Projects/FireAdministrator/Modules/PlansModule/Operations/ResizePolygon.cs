using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using FiresecAPI.Models;

namespace PlansModule.Resize
{
    public static class ResizePolygon
    {
        static UIElement resizeElement = null;
        static UIElement resizeElementTextBox = null;
        static Thumb thumb = null;
        static Canvas canvas;
        static Polygon polygon;
        public static Plan plan;

        public static void SetCanvas(Canvas _canvas)
        {
            canvas = _canvas;
        }

        public static void SetElementResize(UIElement _element, Thumb _thumb, UIElement textbox)
        {
            double test = Canvas.GetLeft(_element);
            if (resizeElement!=null)
            test = Canvas.GetLeft(resizeElement);
            resizeElement = _element;
            test = Canvas.GetLeft(resizeElement);
            thumb = _thumb;
            if (textbox != null) resizeElementTextBox = textbox;
            GetTypeElement();
        }

        public static void RemovePolygon()
        {
            canvas.Children.Remove(polygon);
            polygon = null;
            resizeElement = null;
        }
        public static PointCollection GetPolygonResize()
        {
            return polygon.Points;
        }

        static void GetTypeElement()
        {
            if (resizeElement is Polygon)
            {
                resizePolygon(resizeElement as Polygon);
            }
        }

        static void resizePolygon(Polygon _polygon)
        {
            double x = Canvas.GetLeft(thumb);
            double y = Canvas.GetTop(thumb);
            if (polygon == null)
            {
                polygon = new Polygon();

                
            }
            
            string name = thumb.Name;
            int indexThumb = int.Parse(name.Substring(5));
            polygon.Stroke = System.Windows.Media.Brushes.Red;

            int index = 0;
            polygon.Points.Clear();
            double dLeft = Canvas.GetLeft(_polygon);
            double dTop = Canvas.GetTop(_polygon);
            foreach (var _points in _polygon.Points)
            {
                Point point = _points;
                if (index == indexThumb)
                {
                    point.X = x - dLeft;
                    point.Y = y - dTop;
                };
                
                polygon.Points.Add(point);
                index++;
            }
            Canvas.SetLeft(polygon, dLeft);
            Canvas.SetTop(polygon, dTop);
            if (canvas.Children.IndexOf(polygon) == -1) canvas.Children.Add(polygon);
                
        }
    }
}
