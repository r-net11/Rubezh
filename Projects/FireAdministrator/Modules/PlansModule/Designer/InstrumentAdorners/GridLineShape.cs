using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using Infrustructure.Plans.Designer;
using System.Windows.Controls;
using System.Windows.Shapes;
using Infrustructure.Plans.Painters;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;

namespace PlansModule.InstrumentAdorners
{
	public class GridLineShape : Thumb
	{
		public GridLine GridLine { get; private set; }

		public GridLineShape(GridLine gridLine, double zoomFactor)
		{
			GridLine = gridLine;

			switch (GridLine.Orientation)
			{
				case Orientation.Horizontal:
					Canvas.SetTop(this, GridLine.Position);
					break;
				case Orientation.Vertical:
					Canvas.SetLeft(this, GridLine.Position);
					break;
			}
			UpdateZoom(zoomFactor);
			DragDelta += new DragDeltaEventHandler(GridLineShape_DragDelta);
			MouseDoubleClick += new MouseButtonEventHandler(GridLineShape_MouseDoubleClick);
		}

		public void UpdateZoom(double zoomFactor)
		{
			switch (GridLine.Orientation)
			{
				case Orientation.Horizontal:
					Height = 2 / zoomFactor;
					break;
				case Orientation.Vertical:
					Width = 2 / zoomFactor;
					break;
			}
		}
		public void UpdatePosition(Point point)
		{
			GridLine.UpdatePosition(point);
			switch (GridLine.Orientation)
			{
				case Orientation.Horizontal:
					Canvas.SetTop(this, GridLine.Position);
					break;
				case Orientation.Vertical:
					Canvas.SetLeft(this, GridLine.Position);
					break;
			}
		}
		private void GridLineShape_DragDelta(object sender, DragDeltaEventArgs e)
		{
			switch (GridLine.Orientation)
			{
				case Orientation.Horizontal:
					GridLine.Position += e.VerticalChange;
					Canvas.SetTop(this, GridLine.Position);
					break;
				case Orientation.Vertical:
					GridLine.Position += e.HorizontalChange;
					Canvas.SetLeft(this, GridLine.Position);
					break;
			}
		}
		private void GridLineShape_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var adorner = DataContext as GridLineAdorner;
			if (adorner != null)
				adorner.Remove(this);
		}
	}
}
