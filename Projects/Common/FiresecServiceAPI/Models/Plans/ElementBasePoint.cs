using System.Runtime.Serialization;
using System.Windows;
using System.Xml.Serialization;

namespace StrazhAPI.Plans.Elements
{
	[DataContract]
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

		[XmlIgnore]
		public override ElementType Type
		{
			get { return ElementType.Point; }
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

		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementBasePoint)element).Left = Left;
			((ElementBasePoint)element).Top = Top;
		}
	}
}