using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows;
using Infrustructure.Plans.Elements;
using System.Windows.Media;
using Infrustructure.Plans.Painters;
using System.Windows.Data;

namespace Infrustructure.Plans.Presenter
{
	public static class BorderHelper
	{
		public static Rectangle CreateBorderRectangle()
		{
			var border = new Rectangle();
			border.SetBinding(Shape.MarginProperty, new Binding("AdornerMargin"));
			SetStyle(border);
			return border;
		}
		public static Polygon CreateBorderPolyline(ElementBase element)
		{
			Polygon border = new Polygon() { Points = PainterHelper.GetPoints(element) };
			SetStyle(border);
			return border;
		}
		private static void SetStyle(Shape shape)
		{
			shape.Stroke = Brushes.Orange;
			shape.SetBinding(Shape.StrokeThicknessProperty, new Binding("AdornerThickness"));
		}
	}
}
