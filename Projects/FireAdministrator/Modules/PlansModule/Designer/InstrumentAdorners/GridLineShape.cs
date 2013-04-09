using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using Infrustructure.Plans.Designer;
using System.Windows.Controls;
using System.Windows.Shapes;
using Infrustructure.Plans.Painters;

namespace PlansModule.InstrumentAdorners
{
	public class GridLineShape : Decorator
	{
		public GridLine GridLine { get; private set; }

		public GridLineShape(GridLine gridLine)
		{
			GridLine = gridLine;
			var line = new Line();
			line.StrokeThickness = PainterCache.GridLinePen.Thickness;
			line.Stroke = PainterCache.GridLinePen.Brush;
			switch (GridLine.Orientation)
			{
				case Orientation.Horizontal:
					line.X1 = GridLine.Position;
					break;
				case Orientation.Vertical:
					line.X2 = GridLine.Position;
					break;
			}
			Child = line;
		}
	}
}
