using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;
using System.Xml.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementEllipse : ElementBaseRectangle, IPrimitive
	{
		public ElementEllipse()
		{
			PresentationName = "Эллипс";
		}

		public override ElementBase Clone()
		{
			ElementEllipse elementBase = new ElementEllipse();
			Copy(elementBase);
			return elementBase;
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