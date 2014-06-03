using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementRectangleDoor : ElementBaseRectangle, IPrimitive
	{
		[DataMember]
		public Guid DoorUID { get; set; }

		public override ElementBase Clone()
		{
			var elementBase = new ElementRectangleDoor();
			Copy(elementBase);
			return elementBase;
		}
		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementRectangleDoor)element).DoorUID = DoorUID;
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