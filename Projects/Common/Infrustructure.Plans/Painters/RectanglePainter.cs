using Infrustructure.Plans.Designer;
using RubezhAPI.Plans.Elements;
using System.Windows;
using System.Windows.Media;

namespace Infrustructure.Plans.Painters
{
	public class RectanglePainter : GeometryPainter<RectangleGeometry>
	{
		public RectanglePainter(CommonDesignerCanvas designerCanvas, ElementBase element)
			: base(designerCanvas, element)
		{
		}

		protected override RectangleGeometry CreateGeometry()
		{
			return new RectangleGeometry();
		}
		public override void Transform()
		{
			CalculateRectangle();
			Geometry.Rect = Rect;
		}
		public override Rect Bounds
		{
			get { return Pen == null ? Rect : new Rect(Rect.Left - Pen.Thickness / 2, Rect.Top - Pen.Thickness / 2, Rect.Width + Pen.Thickness, Rect.Height + Pen.Thickness); }
		}
	}
}