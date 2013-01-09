using System.Windows;
using Infrustructure.Plans.Elements;
using System.Windows.Media;

namespace Infrustructure.Plans.Painters
{
	public interface IPainter
	{
		void Draw(DrawingContext drawingContext, ElementBase element, Rect rect);
	}
}
