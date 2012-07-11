using System;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Windows;

namespace Infrustructure.Plans.Elements
{
	[DataContract]
	public abstract class ElementBaseShape : ElementBase
	{
		public ElementBaseShape()
		{
			Points = new PointCollection();
		}

		[DataMember]
		public PointCollection Points { get; set; }

		public override Rect GetRectangle()
		{
			if (Points == null)
				Points = new PointCollection();

			if (Points.Count == 0)
				return new Rect(0, 0, 0, 0);

			double minLeft = double.MaxValue;
			double minTop = double.MaxValue;
			double maxLeft = 0;
			double maxTop = 0;

			foreach (var point in Points)
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
			return new Rect(minLeft, minTop, maxLeft - minLeft, maxTop - minTop);
		}
		protected override void SetPosition(Point point)
		{
			Rect rect = GetRectangle();
			Vector shift = new Vector(point.X - rect.Width / 2 - rect.X, point.Y - rect.Height / 2 - rect.Y);
			for (int i = 0; i < Points.Count; i++)
				Points[i] = Points[i] + shift;
		}

		protected virtual void Copy(ElementBaseShape element)
		{
			base.Copy(element);
			element.Points = Points.Clone();
		}
	}
}