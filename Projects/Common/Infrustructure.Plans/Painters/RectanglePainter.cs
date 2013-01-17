using System.Windows.Media;
using Infrustructure.Plans.Elements;
using System.Windows;

namespace Infrustructure.Plans.Painters
{
	public class RectanglePainter : GeometryPainter<RectangleGeometry>
	{
		protected override RectangleGeometry CreateShape()
		{
			return new RectangleGeometry();
		}
		protected override void InnerTransform(ElementBase element, Rect rect)
		{
			Geometry.Rect = rect;
		}
	}
}
