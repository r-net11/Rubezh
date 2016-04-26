using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Localization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ControlVisualArguments : UIArguments
	{
		public ControlVisualArguments()
		{
			Argument = new Argument();
		}

		public Guid Layout
		{
			get
			{
				CheckFilter();
				return LayoutFilter.LayoutsUIDs[0];
			}
			set
			{
				CheckFilter();
				LayoutFilter.LayoutsUIDs[0] = value;
			}
		}

		[DataMember]
		public Guid LayoutPart { get; set; }

		[DataMember]
		public LayoutPartPropertyName? Property { get; set; }

		[DataMember]
		public Argument Argument { get; set; }

		[DataMember]
		public bool StoreOnServer { get; set; }

		private void CheckFilter()
		{
			if (LayoutFilter == null)
				LayoutFilter = new ProcedureLayoutCollection();
			if (LayoutFilter.LayoutsUIDs.Count == 0)
				LayoutFilter.LayoutsUIDs.Add(Guid.Empty);
		}
	}

	public enum ControlElementType
	{
		//[Description("Чтение свойства")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlVisualArguments), "Get")]
		Get,

		//[Description("Установка свойства")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlVisualArguments), "Set")]
        Set
	}

	public enum LayoutPartPropertyName
	{
		//[Description("Заголовок")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlVisualArguments), "Title")]
        Title,

		//[Description("Отступ")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlVisualArguments), "Margin")]
        Margin,

		//[Description("Цвет фона")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlVisualArguments), "BackgroundColor")]
        BackgroundColor,

		//[Description("Цвет границы")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlVisualArguments), "BorderColor")]
        BorderColor,

		//[Description("Ширина границы")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlVisualArguments), "BorderThickness")]
        BorderThickness,

		//[Description("Текст")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlVisualArguments), "Text")]
        Text,
	}
}