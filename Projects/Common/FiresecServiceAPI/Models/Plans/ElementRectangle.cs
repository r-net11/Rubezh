using System.Runtime.Serialization;
using System.Xml.Serialization;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementRectangle : ElementBaseRectangle, IPrimitive
	{
		public ElementRectangle()
		{
			PresentationName = "Прямоугольник";
		}

		public override ElementBase Clone()
		{
			ElementRectangle elementBase = new ElementRectangle();
			Copy(elementBase);
			return elementBase;
		}

		#region IPrimitive Members

		[XmlIgnore]
		public virtual Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.Rectangle; }
		}

		#endregion
	}
}