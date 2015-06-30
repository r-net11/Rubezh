using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementRectangleGKDirection : ElementBaseRectangle, IPrimitive, IElementDirection, IElementReference
	{
		public ElementRectangleGKDirection()
		{
			PresentationName = "Направление";
		}

		[DataMember]
		public Guid DirectionUID { get; set; }

		[DataMember]
		public bool ShowState { get; set; }

		[DataMember]
		public bool ShowDelay { get; set; }

		public override ElementBase Clone()
		{
			var elementBase = new ElementRectangleGKDirection();
			Copy(elementBase);
			return elementBase;
		}
		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementRectangleGKDirection)element).DirectionUID = DirectionUID;
		}

		#region IPrimitive Members

		[XmlIgnore]
		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.RectangleZone; }
		}

		#endregion IPrimitive Members

		public void SetZLayer(int zlayer)
		{
			ZLayer = zlayer;
		}

		#region IElementReference Members

		Guid IElementReference.ItemUID
		{
			get { return DirectionUID; }
			set { DirectionUID = value; }
		}

		#endregion
	}
}