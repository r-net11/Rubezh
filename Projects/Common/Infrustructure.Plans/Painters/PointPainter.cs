using System.Windows;
using System.Windows.Media;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public class PointPainter : RectanglePainter
	{
		protected TranslateTransform _translateTransform = new TranslateTransform();
		protected RotateTransform _rotateTransform = new RotateTransform();

		public PointPainter(CommonDesignerCanvas designerCanvas, ElementBase element)
			: base(designerCanvas, element)
		{
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
			_translateTransform.X = Rect.Left;
			_translateTransform.Y = Rect.Top;
		}
		protected override void InnerDraw(DrawingContext drawingContext)
		{
			drawingContext.PushTransform(_translateTransform);
			drawingContext.PushTransform(_rotateTransform);
			base.InnerDraw(drawingContext);
			drawingContext.Pop();
			drawingContext.Pop();
		}
		public override bool HitTest(System.Windows.Point point)
		{
			point = _translateTransform.Inverse.Transform(point);
			point = _rotateTransform.Inverse.Transform(point);
			return base.HitTest(point);
		}
		public override Rect Bounds
		{
			get
			{
				return _translateTransform.TransformBounds(
					_rotateTransform.TransformBounds(
						DesignerCanvas.PainterCache.PointGeometry.Rect));
			}
		}
	}
}