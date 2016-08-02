using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class ControlPlanArguments : UIArguments
	{
		public ControlPlanArguments()
		{
			ValueArgument = new Argument();
		}

		[DataMember]
		public Guid PlanUid { get; set; }

		[DataMember]
		public Guid ElementUid { get; set; }

		[DataMember]
		public ElementPropertyType ElementPropertyType { get; set; }

		[DataMember]
		public Argument ValueArgument { get; set; }

		[DataMember]
		public bool StoreOnServer { get; set; }
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
		BackColor,

		[Description("Толщина границы")]
		BorderThickness,

		[Description("Отступ слева")]
		Left,

		[Description("Отступ сверху")]
		Top,

		[Description("Текст")]
		Text,

		[Description("Цвет текста")]
		ForegroundColor,

		[Description("Размер текста")]
		FontSize,

		[Description("Курсив")]
		FontItalic,

		[Description("Жирный")]
		FontBold,

		[Description("Растягивать")]
		Stretch,

		[Description("Переносить")]
		WordWrap
	}
}