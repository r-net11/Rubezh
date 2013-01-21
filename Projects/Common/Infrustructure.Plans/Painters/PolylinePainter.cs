using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public class PolylinePainter : PolygonPainter
	{
		public PolylinePainter(ElementBase element)
			: base(element)
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
