using Infrustructure.Plans.Designer;
using RubezhAPI.Plans.Elements;
using System.Windows.Media;

namespace Infrustructure.Plans.Painters
{
	public class PolygonPainter : GeometryPainter<PathGeometry>
	{
		public PolygonPainter(CommonDesignerCanvas designerCanvas, ElementBase element)
			: base(designerCanvas, element)
		{
		}

		public virtual bool IsClosed
		{
			get { return true; }
		}
		protected override PathGeometry CreateGeometry()
		{
			var geometry = new PathGeometry();
			geometry.FillRule = FillRule.EvenOdd;
			var figure = new PathFigure();
			figure.IsClosed = IsClosed;
			geometry.Figures.Add(figure);
			return geometry;
		}
		public override void Transform()
		{
			var figure = Geometry.Figures[0];
			var points = PainterHelper.GetRealPoints(Element);
			if (points.Count > 0)
			{
				figure.StartPoint = points[0];
				for (int i = figure.Segments.Count; i > points.Count - 1; i--)
					figure.Segments.RemoveAt(i - 1);
				for (int i = figure.Segments.Count; i < points.Count - 1; i++)
					figure.Segments.Add(new LineSegment());
				for (int i = 1; i < points.Count; i++)
				{
					LineSegment segment = (LineSegment)figure.Segments[i - 1];
					segment.Point = points[i];
				}
			}
		}
	}
}