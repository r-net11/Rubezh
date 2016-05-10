using Infrustructure.Plans.Elements;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace StrazhAPI.Models
{
	[DataContract]
	public class ElementPolyline : ElementBasePolyline, IPrimitive
	{
		public ElementPolyline()
		{
            PresentationName = Resources.Language.Models.Plans.ElementPolyline.PresentationName;
		}

		public override ElementBase Clone()
		{
			ElementPolyline elementLine = new ElementPolyline();
			Copy(elementLine);
			return elementLine;
		}

		#region IPrimitive Members

		[XmlIgnore]
		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.Polyline; }
		}

		#endregion IPrimitive Members
	}
}