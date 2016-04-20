using Infrastructure.Plans.Designer;
using RubezhAPI.Plans.Elements;
using System.Windows;
using System.Windows.Media;

namespace Infrastructure.Plans.Painters
{
	public class DefaultPainter : RectanglePainter
	{
		public DefaultPainter(CommonDesignerCanvas designerCanvas, ElementBase element)
			: base(designerCanvas, element)
		{
		}

		protected override RectangleGeometry CreateGeometry()
		{
			CalculateRectangle();
			return Rect.Size == Size.Empty ? DesignerCanvas.PainterCache.PointGeometry : base.CreateGeometry();
		}
		public override void Transform()
		{
			if (Geometry != DesignerCanvas.PainterCache.PointGeometry)
				base.Transform();
		}
		protected override Pen GetPen()
		{
			return null;
		}
		protected override Brush GetBrush()
		{
			return PainterCache.BlackBrush;
		}
	}
}