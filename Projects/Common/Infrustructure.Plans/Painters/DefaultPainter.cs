using System.Windows.Media;
using Infrustructure.Plans.Elements;
using System.Windows;

namespace Infrustructure.Plans.Painters
{
	public class DefaultPainter : RectanglePainter
	{
		public DefaultPainter(ElementBase element)
			: base(element)
		{
		}

		protected override RectangleGeometry CreateGeometry()
		{
			CalculateRectangle();
			return Rect.Size == Size.Empty ? PainterCache.PointGeometry : base.CreateGeometry();
		}
		public override void Transform()
		{
			if (Geometry != PainterCache.PointGeometry)
				base.Transform();
		}
		protected override Pen GetPen()
		{
			return null;
		}
		protected override Brush GetBrush()
		{
			return PainterCache.GetBrush(Colors.Black);
		}
	}
}