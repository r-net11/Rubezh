using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementRectangleZone : ElementBaseRectangle, IElementZone, IPrimitive
	{
		[DataMember]
        public Guid ZoneUID { get; set; }
		[DataMember]
		public bool IsHidden { get; set; }

		public override ElementBase Clone()
		{
			ElementRectangleZone elementBase = new ElementRectangleZone()
			{
                ZoneUID = ZoneUID,
				IsHidden = IsHidden
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

		private int _zlayer;
		public override int ZLayer
		{
			get { return _zlayer; }
		}
		public void SetZLayer(int zlayer)
		{
			_zlayer = zlayer;
		}
	}
}