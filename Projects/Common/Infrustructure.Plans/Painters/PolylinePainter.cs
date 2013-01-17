using System.Windows.Media;
using Infrustructure.Plans.Elements;
using System.Windows;

namespace Infrustructure.Plans.Painters
{
	public class PolylinePainter : PolygonPainter
	{
		public override bool IsClosed
		{
			get { return false; }
		}
		protected override void InitializeBrushes(ElementBase element, Rect rect)
		{
		}
	}
}
