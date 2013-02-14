using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementEllipse : ElementBaseRectangle, IPrimitive
	{
		public ElementEllipse()
		{
		}

		public override ElementBase Clone()
		{
			ElementEllipse elementBase = new ElementEllipse();
			Copy(elementBase);
			return elementBase;
		}

		#region IPrimitive Members

		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.Ellipse; }
		}

		#endregion
	}
}