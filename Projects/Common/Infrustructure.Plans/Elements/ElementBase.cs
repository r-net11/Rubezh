using System;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace Infrustructure.Plans.Elements
{
	[DataContract]
	public abstract class ElementBase
	{
		public ElementBase()
		{
			UID = Guid.NewGuid();
			BackgroundColor = Colors.White;
			Color = Colors.Black;
			Thickness = 1;
			BackgroundPixels = null;
		}

		public virtual string Name
		{
			get { return GetType().FullName; }
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public Color Color { get; set; }
		[DataMember]
		public double Thickness { get; set; }
		[DataMember]
		public Color BackgroundColor { get; set; }
		[DataMember]
		public byte[] BackgroundPixels { get; set; }

		public abstract ElementBase Clone();

		protected virtual void Copy(ElementBase element)
		{
			element.UID = UID;
			element.Color = Color;
			element.Thickness = Thickness;
			element.BackgroundColor = BackgroundColor;
			element.BackgroundPixels = BackgroundPixels;
		}
	}
}