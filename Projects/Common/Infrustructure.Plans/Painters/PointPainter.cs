using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public class PointPainter : RectanglePainter
	{
		public PointPainter(ElementBase element)
			: base(element)
		{
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
		}
	}
}
