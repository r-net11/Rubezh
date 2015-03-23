using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.Models
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

		public override ElementBase Clone()
		{
			var elementBase = new ElementPolygonGKMPT();
			Copy(elementBase);
			return elementBase;
		}
		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementPolygonGKMPT)element).MPTUID = MPTUID;
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
			get { return MPTUID; }
			set { MPTUID = value; }
		}

		#endregion
	}
}