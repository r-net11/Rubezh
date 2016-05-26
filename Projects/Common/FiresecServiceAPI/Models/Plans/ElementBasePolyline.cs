using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace StrazhAPI.Plans.Elements
{
	[DataContract]
	public abstract class ElementBasePolyline : ElementBaseShape
	{
		[XmlIgnore]
		public override ElementType Type
		{
			get { return ElementType.Polyline; }
		}
	}
}