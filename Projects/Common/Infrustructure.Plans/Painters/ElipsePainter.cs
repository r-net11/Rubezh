using System.Windows.Media;
using Infrustructure.Plans.Elements;
using System.Windows;

namespace Infrustructure.Plans.Painters
{
	public class ElipsePainter : GeometryPainter<EllipseGeometry>
	{
		protected override EllipseGeometry CreateShape()
		{
			return new EllipseGeometry();
		}
		protected override void InnerTransform(ElementBase element, Rect rect)
		{
			if (Geometry.Bounds != rect)
			{
				Geometry.RadiusX = rect.Width / 2;
				Geometry.RadiusY = rect.Height / 2;
				Geometry.Center = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
			}
		}
	}
}
