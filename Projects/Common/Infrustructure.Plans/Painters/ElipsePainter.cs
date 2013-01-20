using System.Windows.Media;
using Infrustructure.Plans.Elements;
using System.Windows;

namespace Infrustructure.Plans.Painters
{
	public class ElipsePainter : GeometryPainter<EllipseGeometry>
	{
		public ElipsePainter(ElementBase element)
			: base(element)
		{
		}

		protected override EllipseGeometry CreateGeometry()
		{
			return new EllipseGeometry();
		}
		public override void Transform()
		{
			CalculateRectangle();
			Geometry.Center = new Point(Rect.Left + Rect.Width / 2, Rect.Top + Rect.Height / 2);
			Geometry.RadiusX = Rect.Width / 2;
			Geometry.RadiusY = Rect.Height / 2;
		}
	}
}
