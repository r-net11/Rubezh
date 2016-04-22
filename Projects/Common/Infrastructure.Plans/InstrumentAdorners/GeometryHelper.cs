using System;
using System.Windows;

namespace Infrastructure.Plans.InstrumentAdorners
{
	public static class GeometryHelper
	{
		public const double AngleConvert = 57.295779513082323;
		public const double AngleSegment = 45;

		public static double GetAngle(Point point1, Point point2, Point point3)
		{
			var vector1 = point1 - point2;
			var vector2 = point3 - point2;
			return Vector.AngleBetween(vector2, vector1);
		}
		public static Point TranslatePoint(Point point1, Point point2, Point point3, double angle)
		{
			var angle1 = Math.Atan2(point1.Y - point2.Y, point1.X - point2.X);
			var angle2 = angle1 - angle / AngleConvert;
			var length = (point3 - point2).Length;
			return new Point(point2.X + length * Math.Cos(angle2), point2.Y + length * Math.Sin(angle2));
		}

		public static Point TranslatePoint(Point point1, Point point2, Point point3)
		{
			var angle = GetAngle(point1, point2, point3);
			var finalAngle = Math.Round(angle / 45) * 45;
			return TranslatePoint(point1, point2, point3, finalAngle);
		}
	}
}