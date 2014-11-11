using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;
using System.Xml.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementPolygonGKZone : ElementBasePolygon, IElementZone, IPrimitive, IElementReference
	{
		[DataMember]
		public Guid ZoneUID { get; set; }
		[DataMember]
		public bool IsHiddenZone { get; set; }
		[DataMember]
		public ElementZoneType ElementZoneType { get; set; }

		public override ElementBase Clone()
		{
			var elementBase = new ElementPolygonGKZone();
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
			get { return Infrustructure.Plans.Elements.Primitive.PolygonZone; }
		}

		#endregion

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

		#endregion
	}
}