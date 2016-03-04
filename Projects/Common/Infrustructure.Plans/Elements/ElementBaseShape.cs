using System;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;

namespace Infrustructure.Plans.Elements
{
	[DataContract]
	public abstract class ElementBaseShape : ElementBase
	{
		public ElementBaseShape()
		{
			Points = new System.Windows.Media.PointCollection();
			RGPoints = new RGPointCollection();
		}

		[DataMember]
		public PointCollection Points { get; set; }

		[DataMember]
		public RGPointCollection RGPoints { get; set; }

		public void LoadRGPoints()
		{
			Points = new PointCollection();
			foreach (var point in RGPoints.Points)
			{
				Points.Add(new Point(point.X, point.Y));
			}
		}

		public void SaveRGPoints()
		{
			RGPoints = new RGPointCollection();
			foreach (var point in Points)
			{
				RGPoints.Points.Add(new RGPoint() { X = point.X, Y = point.Y });
			}
		}

		public override Rect GetRectangle()
		{
			double minLeft = double.MaxValue;
			double minTop = double.MaxValue;
			double maxLeft = 0;
			double maxTop = 0;
			Application.Current.Dispatcher.Invoke((Action)(() =>
				{
					if (Points == null)
						Points = new PointCollection();

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
				}));
			return new Rect(minLeft, minTop, maxLeft - minLeft, maxTop - minTop);
		}
		protected override void SetPosition(Point point)
		{
			Rect rect = GetRectangle();
			Vector shift = new Vector(point.X - rect.Width / 2 - rect.X, point.Y - rect.Height / 2 - rect.Y);
			Application.Current.Dispatcher.Invoke((Action)(() =>
				{
					for (int i = 0; i < Points.Count; i++)
						Points[i] = Points[i] + shift;
				}));
		}
	}
}