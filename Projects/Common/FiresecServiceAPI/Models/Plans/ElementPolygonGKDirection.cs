using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.Models
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

		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementPolygonGKDirection)element).DirectionUID = DirectionUID;
		}

		#region IPrimitive Members

		[XmlIgnore]
		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.PolygonZone; }
		}

		#endregion IPrimitive Members

		public void SetZLayer(int zlayer)
		{
			ZLayer = zlayer;
		}

		#region IElementReference Members

		public Guid ItemUID
		{
			get { return DirectionUID; }
			set { DirectionUID = value; }
		}

		#endregion
	}
}