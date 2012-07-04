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
			//Left = point.X;
			//Top = point.Y;
		}

		protected virtual void Copy(ElementBaseShape element)
		{
			base.Copy(element);
			element.Points = Points.Clone();
		}
	}
}