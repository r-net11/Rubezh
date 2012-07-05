﻿using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementRectangleZone : ElementBaseRectangle, IElementZone, IPrimitive
	{
		[DataMember]
		public ulong? ZoneNo { get; set; }

		public override ElementBase Clone()
		{
			ElementBase elementBase = new ElementRectangleZone()
			{
				ZoneNo = ZoneNo
			};
			Copy(elementBase);
			return elementBase;
		}

		#region IPrimitive Members

		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.RectangleZone; }
		}

		#endregion
	}
}