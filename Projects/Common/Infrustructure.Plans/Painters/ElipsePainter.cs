using Infrustructure.Plans.Designer;
using StrazhAPI.Plans.Elements;
using System.Windows;
using System.Windows.Media;

namespace Infrustructure.Plans.Painters
{
	public class ElipsePainter : GeometryPainter<EllipseGeometry>
	{
		public ElipsePainter(CommonDesignerCanvas designerCanvas, ElementBase element)
			: base(designerCanvas, element)
		{
		}

		protected override EllipseGeometry CreateGeometry()
		{
			return new EllipseGeometry();
		}

		public override void Transform()
		{
			CalculateRectangle();
			Geometry.Center = new Point(Rect.Left + Rect.Width / 2, Rect.Top + Rect.Height / 2);
			Geometry.RadiusX = Rect.Width / 2;
			Geometry.RadiusY = Rect.Height / 2;
		}

		public override Rect Bounds
		{
			get { return Pen == null ? Rect : new Rect(Rect.Left - Pen.Thickness / 2, Rect.Top - Pen.Thickness / 2, Rect.Width + Pen.Thickness, Rect.Height + Pen.Thickness); }
		}
	}
}