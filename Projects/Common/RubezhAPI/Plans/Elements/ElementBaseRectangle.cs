using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.Plans.Elements
{
	[DataContract]
	public abstract class ElementBaseRectangle : ElementBasePoint
	{
		public ElementBaseRectangle()
			: base()
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
	}
}