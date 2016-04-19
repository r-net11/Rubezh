using RubezhAPI.Plans.Elements;
using RubezhAPI.Plans.Interfaces;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.Models
{
	[DataContract]
	public class ElementPolygonGKMPT : ElementBasePolygon, IPrimitive, IElementMPT, IElementReference
	{
		public ElementPolygonGKMPT()
		{
			PresentationName = "МПТ";
		}

		[DataMember]
		public Guid MPTUID { get; set; }

		[DataMember()]
		public bool ShowState { get; set; }

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
			get { return MPTUID; }
			set { MPTUID = value; }
		}

		#endregion
	}
}