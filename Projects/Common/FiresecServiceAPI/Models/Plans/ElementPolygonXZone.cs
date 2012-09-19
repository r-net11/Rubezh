using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementPolygonXZone : ElementBasePolygon, IElementZone, IPrimitive, IElementZLayer
	{
        [DataMember]
        public Guid ZoneUID { get; set; }

		public override ElementBase Clone()
		{
			ElementPolygonZone elementBase = new ElementPolygonZone()
			{
                ZoneUID = ZoneUID
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