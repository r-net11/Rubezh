using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public class PointPainter : RectanglePainter
	{
		private TranslateTransform _transform;

		public PointPainter(ElementBase element)
			: base(element)
		{
			_transform = new TranslateTransform();
		}

		protected override RectangleGeometry CreateGeometry()
		{
			return PainterCache.PointGeometry;
		}
		protected override Pen GetPen()
		{
			return null;
		}
		public override void Transform()
		{
			CalculateRectangle();
			_transform.X = Rect.Left;
			_transform.Y = Rect.Top;
		}
		protected override void InnerDraw(DrawingContext drawingContext)
		{
			drawingContext.PushTransform(_transform);
			base.InnerDraw(drawingContext);
			drawingContext.Pop();
		}
	}
}
