using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace StrazhAPI.Models
{
	[DataContract]
	public class ElementSubPlan : ElementBaseRectangle, IPrimitive, IElementReference
	{
		public ElementSubPlan()
		{
            PresentationName = Resources.Language.Models.Plans.ElementSubPlan.PresentationName;
		}

		[DataMember]
		public Guid PlanUID { get; set; }

		[DataMember]
		public string Caption { get; set; }

		public override ElementBase Clone()
		{
			ElementSubPlan elementBase = new ElementSubPlan();
			Copy(elementBase);
			return elementBase;
		}

		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementSubPlan)element).PlanUID = PlanUID;
			((ElementSubPlan)element).Caption = Caption;
		}

		#region IPrimitive Members

		[XmlIgnore]
		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.SubPlan; }
		}

		#endregion IPrimitive Members

		#region IElementReference Members

		public Guid ItemUID
		{
			get { return PlanUID; }
			set { PlanUID = value; }
		}

		#endregion IElementReference Members
	}
}