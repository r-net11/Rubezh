using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.Plans.Elements
{
	[DataContract]
	public abstract class ElementBasePoint : ElementBase
	{
		public ElementBasePoint()
			: base()
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
	}
}