using RubezhAPI.Plans.Elements;
using RubezhAPI.Plans.Interfaces;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.Models
{
	[DataContract]
	public class ElementRectangleSubPlan : ElementBaseRectangle, IElementSubPlan, IPrimitive, IElementReference
	{
		public ElementRectangleSubPlan()
		{
			PresentationName = "Ссылка на план";
		}

		[DataMember]
		public Guid PlanUID { get; set; }

		[DataMember]
		public string Caption { get; set; }

		#region IPrimitive Members

		[XmlIgnore]
		public Primitive Primitive
		{
			get { return RubezhAPI.Plans.Elements.Primitive.RectangleZone; }
		}

		#endregion

		#region IElementReference Members

		[XmlIgnore]
		public Guid ItemUID
		{
			get { return PlanUID; }
			set { PlanUID = value; }
		}

		#endregion
	}
}