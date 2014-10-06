using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;
using System.Xml.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementRectangle : ElementBaseRectangle, IPrimitive
	{
		public ElementRectangle()
		{
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