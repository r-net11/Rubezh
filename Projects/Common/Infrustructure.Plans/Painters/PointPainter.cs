using Infrustructure.Plans.Designer;
using StrazhAPI.Plans.Elements;
using System.Windows;
using System.Windows.Media;

namespace Infrustructure.Plans.Painters
{
	public class PointPainter : RectanglePainter
	{
		private readonly TranslateTransform _transform;

		public PointPainter(CommonDesignerCanvas designerCanvas, ElementBase element)
			: base(designerCanvas, element)
		{
			_transform = new TranslateTransform();
		}

		protected override RectangleGeometry CreateGeometry()
		{
			return DesignerCanvas.PainterCache.PointGeometry;
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

		public override bool HitTest(Point point)
		{
			return base.HitTest(_transform.Inverse.Transform(point));
		}

		public override Rect Bounds
		{
			get { return _transform.TransformBounds(DesignerCanvas.PainterCache.PointGeometry.Rect); }
		}
	}
}