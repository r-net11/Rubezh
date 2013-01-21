using System.Windows.Media;
using Infrustructure.Plans.Elements;
using System.Windows;

namespace Infrustructure.Plans.Painters
{
	public class RectanglePainter : GeometryPainter<RectangleGeometry>
	{
		public RectanglePainter(ElementBase element)
			: base(element)
		{
		}

		protected override RectangleGeometry CreateGeometry()
		{
			return new RectangleGeometry();
		}
		public override void Transform()
		{
			CalculateRectangle();
			Geometry.Rect = Rect;
		}
	}
}
