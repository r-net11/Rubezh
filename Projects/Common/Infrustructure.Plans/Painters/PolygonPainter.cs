using System.Windows.Media;
using Infrustructure.Plans.Elements;
using System.Windows;

namespace Infrustructure.Plans.Painters
{
	public class PolygonPainter : GeometryPainter<PathGeometry>
	{
		public virtual bool IsClosed
		{
			get { return true; }
		}
		protected override PathGeometry CreateShape()
		{
			var geometry = new PathGeometry();
			geometry.FillRule = FillRule.EvenOdd;
			return geometry;
		}
		protected override void InnerTransform(ElementBase element, Rect rect)
		{
			//var points = PainterHelper.GetRealPoints(element);
			//            geometry.
			//if (points.Count > 0)
			//    using (StreamGeometryContext context = geometry.Open())
			//    {
			//        context.BeginFigure(points[0], true, IsClosed);
			//        for (int i = 1; i < points.Count; i++)
			//            context.LineTo(points[i], true, false);
			//        context.Close();
			//    }
		}
	}
}
