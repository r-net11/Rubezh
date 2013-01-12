using System.Windows;
using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public interface IPainter
	{
		void Draw(DrawingContext drawingContext, ElementBase element, Rect rect);
	}
}
