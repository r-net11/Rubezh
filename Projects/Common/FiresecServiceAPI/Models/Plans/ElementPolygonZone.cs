using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementPolygonZone : ElementBasePolygon, IElementZone, IPrimitive, IElementZLayer
	{
		[DataMember]
		public int? ZoneNo { get; set; }

		public override ElementBase Clone()
		{
			ElementPolygonZone elementBase = new ElementPolygonZone()
			{
				ZoneNo = ZoneNo
			};
			Copy(elementBase);
			return elementBase;
		}

		#region IPrimitive Members

		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.PolygonZone; }
		}

		#endregion

		#region IElementZLayer Members

		public int ZLayerIndex { get; set; }

		#endregion
	}
}