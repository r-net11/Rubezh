using RubezhAPI.Plans.Elements;
using RubezhAPI.Plans.Interfaces;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.Models
{
	[DataContract]
	public class ElementPolygonGKDirection : ElementBasePolygon, IPrimitive, IElementDirection, IElementReference
	{
		public ElementPolygonGKDirection()
		{
			PresentationName = "Направление";
		}

		[DataMember]
		public Guid DirectionUID { get; set; }

		[DataMember]
		public bool ShowState { get; set; }

		[DataMember]
		public bool ShowDelay { get; set; }

		#region IPrimitive Members

		[XmlIgnore]
		public Primitive Primitive
		{
			get { return RubezhAPI.Plans.Elements.Primitive.PolygonZone; }
		}

		#endregion IPrimitive Members

		public void SetZLayer(int zlayer)
		{
			ZLayer = zlayer;
		}

		#region IElementReference Members

		[XmlIgnore]
		public Guid ItemUID
		{
			get { return DirectionUID; }
			set { DirectionUID = value; }
		}

		#endregion
	}
}