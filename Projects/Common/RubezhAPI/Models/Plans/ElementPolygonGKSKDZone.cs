using RubezhAPI.Plans.Elements;
using RubezhAPI.Plans.Interfaces;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.Models
{
	[DataContract]
	public class ElementPolygonGKSKDZone : ElementBasePolygon, IElementZone, IPrimitive, IElementReference
	{
		public ElementPolygonGKSKDZone()
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
			get { return RubezhAPI.Plans.Elements.Primitive.PolygonZone; }
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