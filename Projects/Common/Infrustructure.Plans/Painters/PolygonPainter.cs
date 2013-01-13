using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public class PolygonPainter : ShapePainter
	{
		public virtual bool IsClosed
		{
			get { return true; }
		}
		protected override Geometry CreateShape(ElementBase element)
		{
			var points = PainterHelper.GetRealPoints(element);
			StreamGeometry geometry = new StreamGeometry();
			geometry.FillRule = FillRule.EvenOdd;
			if (points.Count > 0)
				using (StreamGeometryContext context = geometry.Open())
				{
					context.BeginFigure(points[0], true, IsClosed);
					for (int i = 1; i < points.Count; i++)
						context.LineTo(points[i], true, false);
					context.Close();
				}
			return geometry;
		}
	}
}
