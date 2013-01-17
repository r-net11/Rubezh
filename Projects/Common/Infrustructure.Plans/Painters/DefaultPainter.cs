using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public class DefaultPainter : RectanglePainter
	{
		protected override Brush GetBrush(ElementBase element)
		{
			return PainterCache.GetBrush(Colors.Black);
		}
	}
}