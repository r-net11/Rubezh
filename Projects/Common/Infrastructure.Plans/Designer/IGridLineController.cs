using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace Infrastructure.Plans.Designer
{
	public interface IGridLineController
	{
		bool IsVisible { get; set; }
		ObservableCollection<GridLine> GridLines { get; }
		void Render(DrawingContext drawingContext);
		void Invalidate();

		Vector Pull(Point point);
		Vector Pull(Rect rect);
		Vector Pull(Vector shift, Rect rect);
		void PullReset();
	}
}