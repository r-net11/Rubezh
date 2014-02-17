using System.Windows.Media;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Designer;

namespace Infrustructure.Plans.Painters
{
	public class PolylinePainter : PolygonPainter
	{
		public PolylinePainter(CommonDesignerCanvas designerCanvas, ElementBase element)
			: base(designerCanvas, element)
		{
		}

		public override bool IsClosed
		{
			get { return false; }
		}
		protected override Brush GetBrush()
		{
			return null;
		}
	}
}
