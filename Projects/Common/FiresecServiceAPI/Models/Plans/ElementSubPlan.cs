using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;
using System.Xml.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementSubPlan : ElementBaseRectangle, IPrimitive
	{
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

		#endregion
	}
}