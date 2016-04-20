using Infrastructure.Plans.Designer;
using RubezhAPI.Plans.Elements;
using System.Windows.Media;

namespace Infrastructure.Plans.Painters
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
