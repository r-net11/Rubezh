using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Windows;

namespace Infrustructure.Plans.Elements
{
	[DataContract]
	public abstract class ElementBasePoint : ElementBase
	{
		public ElementBasePoint()
		{
			Top = 0;
			Left = 0;
			Type = ElementType.Point;
		}

		[DataMember]
		public double Left { get; set; }
		[DataMember]
		public double Top { get; set; }

		public override Rect Rectangle
		{
			get { return new Rect(new Point(Left, Top), Size.Empty); }
		}

		protected virtual void Copy(ElementBasePoint element)
		{
			base.Copy(element);
			element.Left = Left;
			element.Top = Top;
		}
	}
}
