using System;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ControlPlanArguments : UIArguments
	{
		public ControlPlanArguments()
		{
			ValueArgument = new Argument();
			LayoutUid = new Guid();
			PlanUid = new Guid();
			ElementUid = new Guid();
		}

		[DataMember]
		public Guid LayoutUid { get; set; }

		[DataMember]
		public Guid PlanUid { get; set; }

		[DataMember]
		public Guid ElementUid { get; set; }

		[DataMember]
		public ControlVisualType ControlVisualType { get; set; }

		[DataMember]
		public ElementPropertyType ElementPropertyType { get; set; }

		[DataMember]
		public Argument ValueArgument { get; set; }
	}

	public enum ElementPropertyType
	{
		[Description("Высота")]
		Height,

		[Description("Ширина")]
		Width,
		
		[Description("Цвет")]
		Color,

		[Description("Цвет фона")]
		BackColor
	}
}