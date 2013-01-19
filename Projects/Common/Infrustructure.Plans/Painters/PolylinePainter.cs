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
		protected override Brush CreateBrush(ElementBase element, Rect rect)
		{
			return null;
		}
	}
}
