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
		}

		[DataMember]
		public double Height { get; set; }
		[DataMember]
		public double Width { get; set; }

		public override ElementType Type
		{
			get { return ElementType.Rectangle; }
		}
		public override void SetDefault()
		{
			base.SetDefault();
			Height = 50;
			Width = 50;
		}
		public override Rect GetRectangle()
		{
			return new Rect(Left, Top, Width, Height);
		}
		protected override void SetPosition(Point point)
		{
			Left = point.X - Width / 2;
			Top = point.Y - Height / 2;
		}

		protected virtual void Copy(ElementBaseRectangle element)
		{
			base.Copy(element);
			element.Height = Height;
			element.Width = Width;
		}
	}
}