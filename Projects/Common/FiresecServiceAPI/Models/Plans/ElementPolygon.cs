using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementPolygon : ElementBasePolygon, IElementZIndex, IPrimitive
	{
		public ElementPolygon()
		{
		}

		[DataMember]
		public int ZIndex { get; set; }

		public override ElementBase Clone()
		{
			ElementPolygon elementBase = new ElementPolygon();
			Copy(elementBase);
			return elementBase;
		}

		#region IPrimitive Members

		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.Polygon; }
		}

		#endregion
	}
}