using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementRectangleXDirection : ElementBaseRectangle, IPrimitive, IElementDirection, IElementReference
	{
		[DataMember]
		public Guid DirectionUID { get; set; }

		public override ElementBase Clone()
		{
			var elementBase = new ElementRectangleXDirection();
			Copy(elementBase);
			return elementBase;
		}
		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementRectangleXDirection)element).DirectionUID = DirectionUID;
		}

		#region IPrimitive Members

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