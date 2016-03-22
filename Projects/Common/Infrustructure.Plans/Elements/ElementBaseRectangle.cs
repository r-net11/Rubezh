using System.Runtime.Serialization;
using System.Windows;
using System.Xml.Serialization;

namespace Infrustructure.Plans.Elements
{
	[DataContract]
	public abstract class ElementBaseRectangle : ElementBasePoint
	{
		public ElementBaseRectangle() : base()
		{
			Height = 50;
			Width = 50;
		}

		[DataMember]
		public double Height { get; set; }
		[DataMember]
		public double Width { get; set; }

		[XmlIgnore]
		public override ElementType Type
		{
			get { return ElementType.Rectangle; }
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
	}
}