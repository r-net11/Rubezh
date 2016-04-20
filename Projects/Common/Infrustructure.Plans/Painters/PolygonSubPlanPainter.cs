using Infrustructure.Plans.Designer;
using RubezhAPI.Plans.Elements;
using System.Windows.Media;

namespace Infrustructure.Plans.Painters
{
	public class PolygonSubPlanPainter : PolygonPainter
	{
		public PolygonSubPlanPainter(CommonDesignerCanvas designerCanvas, ElementBase element)
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