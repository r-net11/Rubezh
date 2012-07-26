using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementSubPlan : ElementBaseRectangle, IPrimitive, IElementZLayer
	{
		[DataMember]
		public Guid PlanUID { get; set; }

		[DataMember]
		public string Caption { get; set; }

		public override ElementBase Clone()
		{
			ElementSubPlan elementBase = new ElementSubPlan()
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

		#region IElementZLayer Members

		public int ZLayerIndex
		{
			get { return 1; }
		}

		#endregion
	}
}