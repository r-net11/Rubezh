using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Infrustructure.Plans.Designer
{
	public class GridLine
	{
		public double Position { get; set; }
		public Orientation Orientation { get; set; }

		public GridLine()
		{
		}
		public GridLine(Orientation orientation, Point point)
		{
			Orientation = orientation;
			UpdatePosition(point);
		}
		public void UpdatePosition(Point point)
		{
			switch (Orientation)
			{
				case Orientation.Horizontal:
					Position = point.Y;
					break;
				case Orientation.Vertical:
					Position = point.X;
					break;
			}
		}
	}
}
