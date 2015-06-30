using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementRectangleGKGuardZone : ElementBaseRectangle, IElementZone, IPrimitive, IElementReference
	{
		public ElementRectangleGKGuardZone()
		{
			PresentationName = "Охранная зона";
		}

		[DataMember]
		public Guid ZoneUID { get; set; }

		[DataMember]
		public bool ShowState { get; set; }

		[DataMember]
		public ElementZoneType ElementZoneType { get; set; }

		public override ElementBase Clone()
		{
			var elementBase = new ElementRectangleGKGuardZone();
			Copy(elementBase);
			return elementBase;
		}
		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			CopyZone((IElementZone)element);
		}
		private void CopyZone(IElementZone element)
		{
			element.ZoneUID = ZoneUID;
		}

		#region IPrimitive Members

		[XmlIgnore]
		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.RectangleZone; }
		}

		#endregion

		public void SetZLayer(int zlayer)
		{
			ZLayer = zlayer;
		}

		#region IElementReference Members

		Guid IElementReference.ItemUID
		{
			get { return ZoneUID; }
			set { ZoneUID = value; }
		}

		#endregion
	}
}