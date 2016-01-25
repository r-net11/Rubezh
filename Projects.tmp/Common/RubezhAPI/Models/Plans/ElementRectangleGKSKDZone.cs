using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;

namespace RubezhAPI.Models
{
	[DataContract]
	public class ElementRectangleGKSKDZone : ElementBaseRectangle, IElementZone, IPrimitive, IElementReference
	{
		public ElementRectangleGKSKDZone()
		{
			PresentationName = "СКД Зона";
		}

		[DataMember]
		public Guid ZoneUID { get; set; }

		[DataMember]
		public bool ShowState { get; set; }

		[DataMember]
		public ElementZoneType ElementZoneType { get; set; }

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

		[XmlIgnore]
		public Guid ItemUID
		{
			get { return ZoneUID; }
			set { ZoneUID = value; }
		}

		#endregion
	}
}