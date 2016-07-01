using StrazhAPI.Plans.Elements;
using StrazhAPI.Plans.Interfaces;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace StrazhAPI.Models
{
	[DataContract]
	public class ElementPolygonSKDZone : ElementBasePolygon, IElementZone, IPrimitive
	{
		public ElementPolygonSKDZone()
		{
            PresentationName = Resources.Language.Models.Plans.ElementPolygonSKDZones.PresentationName;
		}

		[DataMember]
		public Guid ZoneUID { get; set; }

		[XmlIgnore]
		public bool IsHiddenZone //TODO: Remove it
		{
			get { return false; }
			set { }
		}

		[DataMember]
		public ElementZoneType ElementZoneType { get; set; }

		public override ElementBase Clone()
		{
			var elementBase = new ElementPolygonSKDZone();
			Copy(elementBase);
			return elementBase;
		}

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
			get { return Primitive.PolygonZone; }
		}

		#endregion IPrimitive Members

		public void SetZLayer(int zlayer)
		{
			ZLayer = zlayer;
		}

		#region IElementReference Members

		Guid IElementReference.ItemUID
		{
			get { return ZoneUID; }
			set { ZoneUID = value; }
		}

		#endregion IElementReference Members
	}
}