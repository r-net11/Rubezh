using StrazhAPI.Plans.Elements;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace StrazhAPI.Models
{
	[DataContract]
	public class ElementPolygon : ElementBasePolygon, IPrimitive
	{
		public ElementPolygon()
		{
            PresentationName = Resources.Language.Models.Plans.ElementPolygon.PresentationName;
		}

		public override ElementBase Clone()
		{
			var elementBase = new ElementPolygon();
			Copy(elementBase);
			return elementBase;
		}

		#region IPrimitive Members

		[XmlIgnore]
		public Primitive Primitive
		{
			get { return Primitive.Polygon; }
		}

		#endregion IPrimitive Members
	}
}