using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Infrustructure.Plans.Elements
{
	public abstract class ElementBasePoint : ElementBase
	{
		public ElementBasePoint()
		{
			Top = 0;
			Left = 0;
		}

		[DataMember]
		public double Left { get; set; }
		[DataMember]
		public double Top { get; set; }

		protected virtual void Copy(ElementBasePoint element)
		{
			base.Copy(element);
			element.Left = Left;
			element.Top = Top;
		}
	}
}
