using RubezhAPI.Plans.Elements;
using RubezhAPI.Plans.Interfaces;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.Models
{
	[DataContract]
	public class ElementRectangleTank : ElementBaseRectangle, IPrimitive, IElementReference
	{
		public ElementRectangleTank()
		{
			PresentationName = "Бак";
		}

		[DataMember]
		public Guid DeviceUID { get; set; }

		#region IPrimitive Members

		[XmlIgnore]
		public Primitive Primitive
		{
			get { return RubezhAPI.Plans.Elements.Primitive.RectangleZone; }
		}

		#endregion IPrimitive Members

		public override void UpdateZLayer()
		{
			ZIndex = 50;
		}

		#region IElementReference Members

		[XmlIgnore]
		public Guid ItemUID
		{
			get { return DeviceUID; }
			set { DeviceUID = value; }
		}

		#endregion
	}
}