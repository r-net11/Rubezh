using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public class SubPlanPainter : RectanglePainter
	{
		public SubPlanPainter(ElementBase element)
			: base(element)
		{
		}

		protected override Brush GetBrush()
		{
			return PainterCache.GetTransparentBrush(Element);
		}
		protected override Pen GetPen()
		{
			return PainterCache.ZonePen;
		}
	}
}