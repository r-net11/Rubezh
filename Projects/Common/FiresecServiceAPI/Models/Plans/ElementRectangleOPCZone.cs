using StrazhAPI.Plans.Elements;
using StrazhAPI.Plans.Interfaces;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace StrazhAPI.Models
{
	public class ElementRectangleOPCZone : ElementBaseRectangle, IElementZone, IPrimitive
	{
		public ElementRectangleOPCZone()
		{
			PresentationName = "Зона ОПС";
		}

		public override ElementBase Clone()
		{
			var elementBase = new ElementRectangleOPCZone();
			Copy(elementBase);
			return elementBase;
		}

		[DataMember]
		public Guid ZoneUID { get; set; }
		public void SetZLayer(int zlayer)
		{
			ZLayer = zlayer;
		}

		[XmlIgnore]
		public bool IsHiddenZone
		{
			get { return false; }
			set { }
		}

		[DataMember]
		public ElementZoneType ElementZoneType { get; set; }

		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			CopyZone((IElementZone)element);
		}

		private void CopyZone(IElementZone element)
		{
			element.ZoneUID = ZoneUID;
			element.IsHiddenZone = IsHiddenZone;
		}

		#region IPrimitive Members

		[XmlIgnore]
		public Primitive Primitive
		{
			get { return Primitive.RectangleZone; }
		}

		#endregion IPrimitive Members

		#region IElementReference Members

		Guid IElementReference.ItemUID
		{
			get { return ZoneUID; }
			set { ZoneUID = value; }
		}

		#endregion IElementReference Members
	}
}
