using StrazhAPI.Plans.Elements;
using StrazhAPI.Plans.Interfaces;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace StrazhAPI.Models
{
	[DataContract]
	public class ElementRectangleTank : ElementBaseRectangle, IPrimitive, IElementReference
	{
		public ElementRectangleTank()
		{
            PresentationName = Resources.Language.Models.Plans.ElementRectangleTank.PresentationName;
		}

		[DataMember]
		public Guid DeviceUID { get; set; }

		public override ElementBase Clone()
		{
			var elementBase = new ElementRectangleTank();
			Copy(elementBase);
			return elementBase;
		}

		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementRectangleTank)element).DeviceUID = DeviceUID;
		}

		#region IPrimitive Members

		[XmlIgnore]
		public Primitive Primitive
		{
			get { return Primitive.RectangleZone; }
		}

		#endregion IPrimitive Members

		public override void UpdateZLayer()
		{
			ZIndex = 50;
		}

		#region IElementReference Members

		Guid IElementReference.ItemUID
		{
			get { return DeviceUID; }
			set { DeviceUID = value; }
		}

		#endregion IElementReference Members
	}
}