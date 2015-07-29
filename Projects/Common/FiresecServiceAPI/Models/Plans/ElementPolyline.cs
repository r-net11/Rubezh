using System.Runtime.Serialization;
using System.Xml.Serialization;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementPolyline : ElementBasePolyline, IPrimitive
	{
		public ElementPolyline()
		{
			PresentationName = "Линия";
		}

		#region IPrimitive Members

		[XmlIgnore]
		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.Polyline; }
		}

		#endregion
	}
}