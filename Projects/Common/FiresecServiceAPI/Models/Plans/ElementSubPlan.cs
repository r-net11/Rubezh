﻿using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementSubPlan : ElementBaseRectangle, IPrimitive, IElementReference
	{
		public ElementSubPlan()
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
			get { return Infrustructure.Plans.Elements.Primitive.SubPlan; }
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