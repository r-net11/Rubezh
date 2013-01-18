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
		protected override ImageBrush CreateImageBrush(ElementBase element, Rect rect)
		{
			return null;
		}
		protected override SolidColorBrush CreateSolidColorBrush(ElementBase element, Rect rect)
		{
			return null;
		}
	}
}
