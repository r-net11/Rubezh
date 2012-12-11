using System.Windows;
using Infrustructure.Plans.Elements;
using System.Windows.Media;

namespace Infrustructure.Plans.Painters
{
	public interface IPainter
	{
		UIElement Draw(ElementBase element);
	}
}
