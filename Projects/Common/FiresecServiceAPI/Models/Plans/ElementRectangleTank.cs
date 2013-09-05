using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementRectangleTank : ElementBaseRectangle, IPrimitive
	{
		[DataMember]
		public Guid XDeviceUID { get; set; }

		public override ElementBase Clone()
		{
			var elementBase = new ElementRectangleTank();
			Copy(elementBase);
			return elementBase;
		}
		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementRectangleTank)element).XDeviceUID = XDeviceUID;
		}

		#region IPrimitive Members

		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.RectangleZone; }
		}

		#endregion IPrimitive Members

		public override void UpdateZLayer()
		{
			ZIndex = 50;
		}
	}
}