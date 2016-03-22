using System.Runtime.Serialization;
using System.Windows;
using System.Xml.Serialization;

namespace Infrustructure.Plans.Elements
{
	[DataContract]
	public abstract class ElementBasePoint : ElementBase
	{
		public ElementBasePoint() : base()
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
	}
}