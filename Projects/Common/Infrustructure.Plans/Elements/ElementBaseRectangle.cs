using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Infrustructure.Plans.Elements
{
	public abstract class ElementBaseRectangle : ElementBasePoint
	{
		public ElementBaseRectangle()
		{
			Height = 50;
			Width = 50;
		}

		[DataMember]
		public double Height { get; set; }
		[DataMember]
		public double Width { get; set; }

		protected virtual void Copy(ElementBaseRectangle element)
		{
			base.Copy(element);
			element.Height = Height;
			element.Width = Width;
		}
	}
}
