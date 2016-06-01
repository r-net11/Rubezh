using StrazhAPI.Plans.Elements;
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
			var elementLine = new ElementPolyline();
			Copy(elementLine);
			return elementLine;
		}

		#region IPrimitive Members

		[XmlIgnore]
		public Primitive Primitive
		{
			get { return Primitive.Polyline; }
		}

		#endregion IPrimitive Members
	}
}