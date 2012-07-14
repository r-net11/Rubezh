using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;

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
			ElementBase elementBase = new ElementSubPlan()
			{
				PlanUID = PlanUID,
				Caption = Caption
			};
			Copy(elementBase);
			return elementBase;
		}

		#region IPrimitive Members

		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.SubPlan; }
		}

		#endregion
	}
}