using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementPolygonGKZone : ElementBasePolygon, IElementZone, IPrimitive, IElementReference
	{
		public ElementPolygonGKZone()
		{
			PresentationName = "Зона";
		}

		[DataMember]
		public Guid ZoneUID { get; set; }
		[DataMember]
		public bool ShowState { get; set; }
		[DataMember]
		public ElementZoneType ElementZoneType { get; set; }

		#region IPrimitive Members

		[XmlIgnore]
		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.PolygonZone; }
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