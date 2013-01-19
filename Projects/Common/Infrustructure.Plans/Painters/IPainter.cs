using System.Windows;
using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public interface IPainter
	{
		Rect Bounds { get; }
		bool IsVisible { get; set; }
		bool HitTest(Point point);
		void Draw(DrawingContext drawingContext, ElementBase element, Rect rect);
		void Redraw(ElementBase element, Rect rect);
		void Transform(ElementBase element, Rect rect);
	}
}
