using System.Windows;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public interface IPainter
	{
		FrameworkElement Draw(ElementBase element);
	}
}
