using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementPolygonZone : ElementBasePolygon, IElementZone, IPrimitive
	{
		[DataMember]
		public ulong? ZoneNo { get; set; }

		public override ElementBase Clone()
		{
			ElementBase elementBase = new ElementPolygonZone()
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
	}
}