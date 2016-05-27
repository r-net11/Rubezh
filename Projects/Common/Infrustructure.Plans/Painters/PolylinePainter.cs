using Infrustructure.Plans.Designer;
using StrazhAPI.Plans.Elements;
using System.Windows.Media;

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