using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;

namespace StrazhAPI.Plans.Elements
{
	[DataContract]
	public abstract class ElementBaseShape : ElementBase
	{
		protected ElementBaseShape()
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

			var minLeft = double.MaxValue;
			var minTop = double.MaxValue;
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
			if (maxTop < minTop)
				minTop = maxTop;
			if (maxLeft < minLeft)
				minLeft = maxLeft;
			return new Rect(minLeft, minTop, maxLeft - minLeft, maxTop - minTop);
		}

		protected override void SetPosition(Point point)
		{
			var rect = GetRectangle();
			var shift = new Vector(point.X - rect.Width / 2 - rect.X, point.Y - rect.Height / 2 - rect.Y);
			for (var i = 0; i < Points.Count; i++)
				Points[i] = Points[i] + shift;
		}

		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementBaseShape)element).Points = Points.Clone();
		}
	}
}