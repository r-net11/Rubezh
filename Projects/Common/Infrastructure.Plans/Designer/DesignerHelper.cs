using System.Windows;
using System.Windows.Shapes;

namespace Infrastructure.Plans.Designer
{
	public static class DesignerHelper
	{
		public static bool IsPointInPolygon(Point point, Polygon polygon)
		{
			if (polygon == null)
				return false;

			var j = polygon.Points.Count - 1;
			var oddNodes = false;

			for (var i = 0; i < polygon.Points.Count; i++)
			{
				if ((polygon.Points[i].Y < point.Y && polygon.Points[j].Y >= point.Y || polygon.Points[j].Y < point.Y && polygon.Points[i].Y >= point.Y) &&
					(polygon.Points[i].X + (point.Y - polygon.Points[i].Y) / (polygon.Points[j].Y - polygon.Points[i].Y) * (polygon.Points[j].X - polygon.Points[i].X) < point.X))
					oddNodes = !oddNodes;
				j = i;
			}

			return oddNodes;
		}
	}
}