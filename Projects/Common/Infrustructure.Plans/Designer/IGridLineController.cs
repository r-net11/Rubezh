using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows;

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
