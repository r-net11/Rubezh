using System.Windows;
using System.Windows.Controls;

namespace Infrastructure.Plans.Designer
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
		public bool IsInside(double width, double height)
		{
			switch (Orientation)
			{
				case Orientation.Horizontal:
					return Position > 0 && Position < height;
				case Orientation.Vertical:
					return Position > 0 && Position < width;
			}
			return false;
		}
	}
}