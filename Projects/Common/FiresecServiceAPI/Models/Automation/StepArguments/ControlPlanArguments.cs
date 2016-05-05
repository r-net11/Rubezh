using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using LocalizationConveters;

namespace FiresecAPI.Automation
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
		//[Description("Высота")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlPlanArguments), "Height")]
		Height,

		//[Description("Ширина")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlPlanArguments), "Width")]
        Width,

		//[Description("Цвет")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlPlanArguments), "Color")]
        Color,

		//[Description("Цвет фона")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlPlanArguments), "BackColor")]
        BackColor,

		//[Description("Толщина границы")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlPlanArguments), "BorderThickness")]
        BorderThickness,

		//[Description("Отступ слева")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlPlanArguments), "Left")]
        Left,

		//[Description("Отступ сверху")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlPlanArguments), "Top")]
        Top,

		//[Description("Текст")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlPlanArguments), "Text")]
        Text,

		//[Description("Цвет текста")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlPlanArguments), "ForegroundColor")]
        ForegroundColor,

		//[Description("Размер текста")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlPlanArguments), "FontSize")]
        FontSize,

		//[Description("Курсив")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlPlanArguments), "FontItalic")]
        FontItalic,

		//[Description("Жирный")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlPlanArguments), "FontBold")]
        FontBold,

		//[Description("Растягивать")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlPlanArguments), "Stretch")]
        Stretch,

		//[Description("Переносить")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlPlanArguments), "WordWrap")]
        WordWrap
	}
}