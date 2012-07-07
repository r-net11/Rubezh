using System;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Windows;

namespace Infrustructure.Plans.Elements
{
	[DataContract]
	public abstract class ElementBase
	{
		public ElementBase()
		{
			UID = Guid.NewGuid();
			BackgroundColor = Colors.White;
			BorderColor = Colors.Black;
			BorderThickness = 1;
			BackgroundPixels = null;
		}

		public virtual string Name
		{
			get { return GetType().FullName; }
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public Color BorderColor { get; set; }
		[DataMember]
		public double BorderThickness { get; set; }
		[DataMember]
		public Color BackgroundColor { get; set; }
		[DataMember]
		public byte[] BackgroundPixels { get; set; }

		public Point Position
		{
			get
			{
				Rect rect = GetRectangle();
				return new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
			}
			set { SetPosition(value); }
		}
		public abstract ElementType Type { get; }
		public abstract Rect GetRectangle();
		protected abstract void SetPosition(Point point);
		public abstract ElementBase Clone();
		public abstract void SetDefault();

		protected virtual void Copy(ElementBase element)
		{
			element.UID = UID;
			element.BorderColor = BorderColor;
			element.BorderThickness = BorderThickness;
			element.BackgroundColor = BackgroundColor;
			element.BackgroundPixels = BackgroundPixels;
		}
	}
}