using StrazhAPI.Plans.Elements;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace StrazhAPI.Models
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
			var elementBase = new ElementRectangle();
			Copy(elementBase);
			return elementBase;
		}

		#region IPrimitive Members

		[XmlIgnore]
		public virtual Primitive Primitive
		{
			get { return Primitive.Rectangle; }
		}

		#endregion IPrimitive Members
	}
}