using System.Windows.Media;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Designer;

namespace Infrustructure.Plans.Painters
{
	public class SubPlanPainter : RectanglePainter
	{
		public SubPlanPainter(CommonDesignerCanvas designerCanvas, ElementBase element)
			: base(designerCanvas, element)
		{
		}

		protected override Brush GetBrush()
		{
			return DesignerCanvas.PainterCache.GetTransparentBrush(Element);
		}
		protected override Pen GetPen()
		{
			return DesignerCanvas.PainterCache.ZonePen;
		}
	}
}