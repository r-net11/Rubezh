using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementRectangleXZone : ElementBaseRectangle, IElementZone, IPrimitive
	{
		[DataMember]
		public Guid ZoneUID { get; set; }
		[DataMember]
		public bool IsHiddenZone { get; set; }

		public override ElementBase Clone()
		{
			ElementRectangleXZone elementBase = new ElementRectangleXZone();
			Copy(elementBase);
			return elementBase;
		}
		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			Copy((ElementRectangleXZone)element);
		}
		private void Copy(IElementZone element)
		{
			element.ZoneUID = ZoneUID;
			element.IsHiddenZone = IsHiddenZone;
		}

		#region IPrimitive Members

		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.RectangleZone; }
		}

		#endregion

		public void SetZLayer(int zlayer)
		{
			ZLayer = zlayer;
		}
	}
}