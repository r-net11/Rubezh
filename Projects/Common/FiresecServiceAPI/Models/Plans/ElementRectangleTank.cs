using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;
using System.Xml.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementRectangleTank : ElementBaseRectangle, IPrimitive, IElementReference
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

		[XmlIgnore]
		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.RectangleZone; }
		}

		#endregion IPrimitive Members

		public override void UpdateZLayer()
		{
			ZIndex = 50;
		}

		#region IElementReference Members

		Guid IElementReference.ItemUID
		{
			get { return XDeviceUID; }
			set { XDeviceUID = value; }
		}

		#endregion
	}
}