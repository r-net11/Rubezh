using Infrustructure.Plans.Designer;
using StrazhAPI.Plans.Elements;
using System.Windows.Media;

namespace Infrustructure.Plans.Painters
{
	public class RectangleZonePainter : RectanglePainter
	{
		public RectangleZonePainter(CommonDesignerCanvas designerCanvas, ElementBase element)
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