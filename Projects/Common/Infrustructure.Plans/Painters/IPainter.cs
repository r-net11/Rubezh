using System.Windows;
using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public interface IPainter
	{
		Rect Bounds { get; }
		void Draw(DrawingContext drawingContext);
		void Transform();
		//void Show();
		//void Hide();
		bool HitTest(Point point);
	}
}
