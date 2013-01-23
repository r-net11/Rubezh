using System.Windows;
using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public interface IPainter
	{
		Rect Bounds { get; }
		void Invalidate();
		void Draw(DrawingContext drawingContext);
		void Transform();
		bool HitTest(Point point);
	}
}
