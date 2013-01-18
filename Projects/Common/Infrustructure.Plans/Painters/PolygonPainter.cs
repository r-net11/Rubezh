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
			var figure = new PathFigure();
			figure.IsClosed = IsClosed;
			geometry.Figures.Add(figure);
			return geometry;
		}
		protected override void InnerTransform(ElementBase element, Rect rect)
		{
			var figure = Geometry.Figures[0];
			var points = PainterHelper.GetRealPoints(element);
			if (points.Count > 0)
			{
				if (figure.StartPoint != points[0])
					figure.StartPoint = points[0];
				for (int i = figure.Segments.Count; i > points.Count - 1; i--)
					figure.Segments.RemoveAt(i - 1);
				for (int i = figure.Segments.Count; i < points.Count - 1; i++)
					figure.Segments.Add(new LineSegment());
				for (int i = 1; i < points.Count; i++)
				{
					LineSegment segment = (LineSegment)figure.Segments[i - 1];
					if (segment.Point != points[i])
						segment.Point = points[i];
				}
			}
		}
	}
}
