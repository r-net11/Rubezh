using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
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