using RubezhAPI.Plans.Elements;
using RubezhAPI.Plans.Interfaces;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.Models
{
	[DataContract()]
	public class ElementRectangleGKDelay : ElementBaseRectangle, IPrimitive, IElementDelay, IElementReference
	{
		public ElementRectangleGKDelay()
		{
			PresentationName = "Задержка";
		}

		[DataMember()]
		public Guid DelayUID { get; set; }

		[DataMember()]
		public bool ShowState { get; set; }

		[DataMember]
		public bool ShowDelay { get; set; }

		#region IPrimitive Members

		[XmlIgnore()]
		public Primitive Primitive
		{
			get { return RubezhAPI.Plans.Elements.Primitive.RectangleZone; }
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
			get { return this.DelayUID; }
			set { this.DelayUID = value; }
		}

		#endregion
	}
}