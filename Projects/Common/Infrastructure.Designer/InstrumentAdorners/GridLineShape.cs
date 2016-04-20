using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Infrastructure.Plans.Designer;

namespace Infrastructure.Designer.InstrumentAdorners
{
	public class GridLineShape : Thumb
	{
		public GridLine GridLine { get; private set; }
		public GridLineAdorner GridLineAdorner
		{
			get { return DataContext as GridLineAdorner; }
		}

		public GridLineShape(GridLine gridLine, double thickness)
		{
			GridLine = gridLine;

			UpdateZoom(thickness);
			UpdatePosition();
			DragDelta += new DragDeltaEventHandler(GridLineShape_DragDelta);
			DragCompleted += new DragCompletedEventHandler(GridLineShape_DragCompleted);
			MouseDoubleClick += new MouseButtonEventHandler(GridLineShape_MouseDoubleClick);
		}

		public void UpdateZoom(double thickness)
		{
			switch (GridLine.Orientation)
			{
				case Orientation.Horizontal:
					Height = 6 * thickness;
					Padding = new Thickness(0, 2 * thickness, 0, 2 * thickness);
					break;
				case Orientation.Vertical:
					Width = 6 * thickness;
					Padding = new Thickness(2 * thickness, 0, 2 * thickness, 0);
					break;
			}
		}
		public void UpdatePosition(Point point)
		{
			GridLine.UpdatePosition(point);
			UpdatePosition();
		}
		private void UpdatePosition()
		{
			switch (GridLine.Orientation)
			{
				case Orientation.Horizontal:
					Canvas.SetTop(this, GridLine.Position - Height / 2);
					break;
				case Orientation.Vertical:
					Canvas.SetLeft(this, GridLine.Position - Width / 2);
					break;
			}
		}
		private void GridLineShape_DragDelta(object sender, DragDeltaEventArgs e)
		{
			switch (GridLine.Orientation)
			{
				case Orientation.Horizontal:
					GridLine.Position += e.VerticalChange;
					break;
				case Orientation.Vertical:
					GridLine.Position += e.HorizontalChange;
					break;
			}
			UpdatePosition();
		}
		private void GridLineShape_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			if (!GridLine.IsInside(GridLineAdorner.CanvasWidth, GridLineAdorner.CanvasHeight))
				GridLineAdorner.Remove(this);
		}
		private void GridLineShape_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (GridLineAdorner != null)
				GridLineAdorner.Remove(this);
		}
	}
}
