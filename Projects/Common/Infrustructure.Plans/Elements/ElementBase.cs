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

		public ElementType Type { get; protected set; }

		public abstract FrameworkElement Draw();
		public abstract Rect Rectangle { get; }

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

		public abstract ElementBase Clone();

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