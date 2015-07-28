using System.Runtime.Serialization;
using System.Xml.Serialization;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementEllipse : ElementBaseRectangle, IPrimitive
	{
		public ElementEllipse()
		{
			PresentationName = "Эллипс";
		}

		#region IPrimitive Members

		[XmlIgnore]
		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.Ellipse; }
		}

		#endregion
	}
}