using RubezhAPI.Plans.Elements;
using RubezhAPI.Plans.Interfaces;
using System;
using System.Xml.Serialization;

namespace RubezhAPI.Models
{
	public class ElementPolygonGKPumpStation : ElementBasePolygon, IPrimitive, IElementPumpStation, IElementReference
	{
		public ElementPolygonGKPumpStation()
		{
			this.PresentationName = "Насосная станция";
		}

		public Guid PumpStationUID { get; set; }

		public bool ShowDelay { get; set; }

		public bool ShowState { get; set; }

		public void SetZLayer(int zlayer)
		{
			this.ZLayer = zlayer;
		}

		#region IPrimitive Members

		[XmlIgnore()]
		public Primitive Primitive
		{
			get { return RubezhAPI.Plans.Elements.Primitive.PolygonZone; }
		}

		#endregion IPrimitive Members

		#region IElementReference Members

		[XmlIgnore()]
		public Guid ItemUID
		{
			get { return this.PumpStationUID; }
			set { this.PumpStationUID = value; }
		}

		#endregion

	}
}
