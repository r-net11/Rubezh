using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;

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
