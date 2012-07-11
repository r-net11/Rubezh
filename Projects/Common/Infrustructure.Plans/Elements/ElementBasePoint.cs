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
		}

		[DataMember]
		public double Left { get; set; }
		[DataMember]
		public double Top { get; set; }

		public override ElementType Type
		{
			get { return ElementType.Point; }
		}
		public override void SetDefault()
		{
			Top = 0;
			Left = 0;
		}
		public override Rect GetRectangle()
		{
			return new Rect(new Point(Left, Top), new Point(Left, Top));
		}
		protected override void SetPosition(Point point)
		{
			Left = point.X;
			Top = point.Y;
		}

		protected virtual void Copy(ElementBasePoint element)
		{
			base.Copy(element);
			element.Left = Left;
			element.Top = Top;
		}
	}
}