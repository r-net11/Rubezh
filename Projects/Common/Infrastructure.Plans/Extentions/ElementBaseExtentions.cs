using RubezhAPI;
using RubezhAPI.Plans.Elements;
using System;
using System.Windows;

namespace Infrastructure.Plans
{
	public static class ElementBaseExtentions
	{
		public static Rect GetRectangle(this ElementBasePoint elementBasePoint)
		{
			return new Rect(new Point(elementBasePoint.Left, elementBasePoint.Top), new Point(elementBasePoint.Left, elementBasePoint.Top));
		}
		public static Point GetPosition(this ElementBasePoint elementBasePoint)
		{
			Rect rect = GetRectangle(elementBasePoint);
			return new Point(rect.Left, rect.Top);
		}
		public static void SetPosition(this ElementBasePoint elementBasePoint, Point point)
		{
			elementBasePoint.Left = point.X;
			elementBasePoint.Top = point.Y;
		}

		public static Rect GetRectangle(this ElementBaseRectangle elementBaseRectangle)
		{
			return new Rect(elementBaseRectangle.Left, elementBaseRectangle.Top, elementBaseRectangle.Width, elementBaseRectangle.Height);
		}
		public static Point GetPosition(this ElementBaseRectangle elementBaseRectangle)
		{
			Rect rect = GetRectangle(elementBaseRectangle);
			return new Point(rect.Left, rect.Top);
		}

		static Point GetCenterPosition(this ElementBaseRectangle elementBaseRectangle)
		{
			Rect rect = GetRectangle(elementBaseRectangle);
			return new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
		}
		public static void SetPosition(this ElementBaseRectangle elementBaseRectangle, Point point)
		{
			elementBaseRectangle.Left = point.X;
			elementBaseRectangle.Top = point.Y;
		}

		public static Rect GetRectangle(this ElementBaseShape elementBaseShape)
		{
			double minLeft = double.MaxValue;
			double minTop = double.MaxValue;
			double maxLeft = 0;
			double maxTop = 0;
			Application.Current.Dispatcher.Invoke((Action)(() =>
			{
				if (elementBaseShape.Points == null)
					elementBaseShape.Points = new PointCollection();

				foreach (var point in elementBaseShape.Points)
				{
					if (point.X < minLeft)
						minLeft = point.X;
					if (point.Y < minTop)
						minTop = point.Y;
					if (point.X > maxLeft)
						maxLeft = point.X;
					if (point.Y > maxTop)
						maxTop = point.Y;
				}
				if (maxTop < minTop)
					minTop = maxTop;
				if (maxLeft < minLeft)
					minLeft = maxLeft;
			}));
			return new Rect(minLeft, minTop, maxLeft - minLeft, maxTop - minTop);
		}
		public static Point GetPosition(this ElementBaseShape elementBaseShape)
		{
			Rect rect = GetRectangle(elementBaseShape);
			return new Point(rect.Left, rect.Top);
		}
		static Point GetCenterPosition(this ElementBaseShape elementBaseShape)
		{
			Rect rect = GetRectangle(elementBaseShape);
			return new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
		}
		public static void SetPosition(this ElementBaseShape elementBaseShape, Point point)
		{
			Rect rect = GetRectangle(elementBaseShape);
			Vector shift = new Vector(point.X - rect.X, point.Y - rect.Y);
			Application.Current.Dispatcher.Invoke((Action)(() =>
			{
				for (int i = 0; i < elementBaseShape.Points.Count; i++)
					elementBaseShape.Points[i] = elementBaseShape.Points[i] + shift;
			}));
		}

		public static Rect GetRectangle(this ElementBase elementBase)
		{
			if (elementBase is ElementBaseRectangle)
				return GetRectangle((ElementBaseRectangle)elementBase);
			if (elementBase is ElementBaseShape)
				return GetRectangle((ElementBaseShape)elementBase);
			if (elementBase is ElementBasePoint)
				return GetRectangle((ElementBasePoint)elementBase);
			throw new ArgumentException("Метод GetRectangle определен только для типов ElementBasePoint, ElementBaseRectangle и ElementBaseShape.");
		}
		public static Point GetPosition(this ElementBase elementBase)
		{
			if (elementBase is ElementBaseRectangle)
				return GetPosition((ElementBaseRectangle)elementBase);
			if (elementBase is ElementBaseShape)
				return GetPosition((ElementBaseShape)elementBase);
			if (elementBase is ElementBasePoint)
				return GetPosition((ElementBasePoint)elementBase);
			throw new ArgumentException("Метод GetPosition определен только для типов ElementBasePoint, ElementBaseRectangle и ElementBaseShape.");
		}

		public static Point GetCenterPosition(this ElementBase elementBase)
		{
			if (elementBase is ElementBaseRectangle)
				return GetCenterPosition((ElementBaseRectangle)elementBase);
			if (elementBase is ElementBaseShape)
				return GetCenterPosition((ElementBaseShape)elementBase);
			if (elementBase is ElementBasePoint)
				return GetPosition((ElementBasePoint)elementBase);
			throw new ArgumentException("Метод GetPosition определен только для типов ElementBasePoint, ElementBaseRectangle и ElementBaseShape.");
		}
		public static void SetPosition(this ElementBase elementBase, Point point)
		{
			if (elementBase is ElementBaseRectangle)
			{
				SetPosition((ElementBaseRectangle)elementBase, point);
				return;
			}
			if (elementBase is ElementBaseShape)
			{
				SetPosition((ElementBaseShape)elementBase, point);
				return;
			}
			if (elementBase is ElementBasePoint)
			{
				SetPosition((ElementBasePoint)elementBase, point);
				return;
			}
			throw new ArgumentException("Метод SetPosition определен только для типов ElementBasePoint, ElementBaseRectangle и ElementBaseShape.");
		}
	}
}
