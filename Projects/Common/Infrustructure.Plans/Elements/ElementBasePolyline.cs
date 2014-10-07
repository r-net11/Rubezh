using System.Runtime.Serialization;
using System.Windows;
using System.Xml.Serialization;

namespace Infrustructure.Plans.Elements
{
	[DataContract]
	public abstract class ElementBasePolyline : ElementBaseShape
	{
		public ElementBasePolyline()
		{
		}

		[XmlIgnore]
		public override ElementType Type
		{
			get { return ElementType.Polyline; }
		}
	}
}