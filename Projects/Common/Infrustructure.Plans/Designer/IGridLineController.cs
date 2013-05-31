using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace Infrustructure.Plans.Designer
{
	public interface IGridLineController
	{
		bool IsVisible { get; set; }
		ObservableCollection<GridLine> GridLines { get; }
		void Render(DrawingContext drawingContext);
		void Invalidate();
		void PullReset();
		Vector PullRectangle(Vector shift, Rect rect);
	}
}
