using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Windows;

namespace Infrustructure.Plans.Elements
{
	[DataContract]
	public abstract class ElementBaseRectangle : ElementBasePoint
	{
		public ElementBaseRectangle()
		{
			Height = 50;
			Width = 50;
			Type = ElementType.Rectangle;
		}

		[DataMember]
		public double Height { get; set; }
		[DataMember]
		public double Width { get; set; }

		public override Rect Rectangle
		{
			get { return new Rect(Left, Top, Width, Height); }
		}

		protected virtual void Copy(ElementBaseRectangle element)
		{
			base.Copy(element);
			element.Height = Height;
			element.Width = Width;
		}
	}
}
