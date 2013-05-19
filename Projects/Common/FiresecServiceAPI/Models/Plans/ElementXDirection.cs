using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementXDirection : ElementBaseRectangle, IPrimitive
	{
		[DataMember]
		public Guid DirectionUID { get; set; }

		public override ElementBase Clone()
		{
			var elementBase = new ElementXDirection();
			Copy(elementBase);
			return elementBase;
		}
		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementXDirection)element).DirectionUID = DirectionUID;
		}

		#region IPrimitive Members

		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.RectangleZone; }
		}

		#endregion

		public void SetZLayer(int zlayer)
		{
			ZLayer = zlayer;
		}
	}
}