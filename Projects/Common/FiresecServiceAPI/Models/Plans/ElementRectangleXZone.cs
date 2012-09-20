using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementRectangleXZone : ElementBaseRectangle, IElementZone, IPrimitive, IElementZLayer
	{
		[DataMember]
        public Guid ZoneUID { get; set; }

		public override ElementBase Clone()
		{
			ElementRectangleZone elementBase = new ElementRectangleZone()
			{
                ZoneUID = ZoneUID
			};
			Copy(elementBase);
			return elementBase;
		}

		#region IPrimitive Members

		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.RectangleZone; }
		}

		#endregion

		#region IElementZLayer Members

		public int ZLayerIndex {get;set;}

		#endregion
	}
}