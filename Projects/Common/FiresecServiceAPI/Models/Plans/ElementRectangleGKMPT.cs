using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementRectangleGKMPT : ElementBaseRectangle, IPrimitive, IElementMPT, IElementReference
	{
		public ElementRectangleGKMPT()
		{
			PresentationName = "МПТ";
		}

		[DataMember]
		public Guid MPTUID { get; set; }

		public override ElementBase Clone()
		{
			var elementBase = new ElementRectangleGKMPT();
			Copy(elementBase);
			return elementBase;
		}
		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementRectangleGKMPT)element).MPTUID = MPTUID;
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
			get { return MPTUID; }
			set { MPTUID = value; }
		}

		#endregion
	}
}