using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.Plans.Elements
{
	[DataContract]
	public abstract class ElementBasePolygon : ElementBaseShape
	{
		[XmlIgnore]
		public override ElementType Type
		{
			get { return ElementType.Polygon; }
		}
	}
}