using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;

namespace RubezhAPI.Models
{
	[DataContract()]
	public class ElementRectangleGKPumpStation : ElementBaseRectangle, IPrimitive, IElementPumpStation, IElementReference
	{
		public ElementRectangleGKPumpStation()
		{
			base.PresentationName = "Насосная станция";
		}

		[DataMember()]
		public Guid PumpStationUID { get; set; }

		[DataMember()]
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
			get { return Infrustructure.Plans.Elements.Primitive.RectangleZone; }
		}

		#endregion

		#region IElementReference Members

		[XmlIgnore]
		public Guid ItemUID
		{
			get { return this.PumpStationUID; }
			set { this.PumpStationUID = value; }
		}

		#endregion
	}
}
