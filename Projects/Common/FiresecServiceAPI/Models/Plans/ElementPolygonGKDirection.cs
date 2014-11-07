using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;
using System.Xml.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementPolygonGKDirection : ElementBasePolygon, IPrimitive, IElementDirection, IElementReference
	{
		public ElementPolygonGKDirection()
		{
			PresentationName = "Многоугольник направление";
		}

		[DataMember]
		public Guid DirectionUID { get; set; }

		public override ElementBase Clone()
		{
			var elementBase = new ElementPolygonGKDirection();
			Copy(elementBase);
			return elementBase;
		}
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

		Guid IElementReference.ItemUID
		{
			get { return DirectionUID; }
			set { DirectionUID = value; }
		}

		#endregion
	}
}