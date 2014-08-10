using System.Windows;
using System.Windows.Controls;

namespace AutomationModule.ViewModels
{
	public class StepTypeTemplateSelector : DataTemplateSelector
	{
		public DataTemplate SoundsTemplate { get; set; }
		public DataTemplate SendMessageTemplate { get; set; }
		public DataTemplate JournalTemplate { get; set; }
		public DataTemplate ArithmeticTemplate { get; set; }
		public DataTemplate ConditionTemplate { get; set; }
		public DataTemplate FindObjectTemplate { get; set; }
		public DataTemplate ForeachTemplate { get; set; }
		public DataTemplate PauseTemplate { get; set; }
		public DataTemplate ProcedureSelectionTemplate { get; set; }
		public DataTemplate ExitTemplate { get; set; }
		public DataTemplate PersonInspectionTemplate { get; set; }
		public DataTemplate SetGlobalValueTemplate { get; set; }
		public DataTemplate IncrementGlobalValueTemplate { get; set; }
		public DataTemplate ControlGKDeviceTemplate { get; set; }
		public DataTemplate ControlSKDDeviceTemplate { get; set; }
		public DataTemplate ControlGKFireZoneTemplate { get; set; }
		public DataTemplate ControlGKGuardZoneTemplate { get; set; }
		public DataTemplate ControlSKDZoneTemplate { get; set; }
		public DataTemplate ControlCameraTemplate { get; set; }
		public DataTemplate ControlDirectionTemplate { get; set; }
		public DataTemplate ControlDoorTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item is SoundStepViewModel)
			{
				return SoundsTemplate;
			}
			if (item is SendMessageStepViewModel)
			{
				return SendMessageTemplate;
			}
			if (item is JournalStepViewModel)
			{
				return JournalTemplate;
			}
			if (item is ArithmeticStepViewModel)
			{
				return ArithmeticTemplate;
			}
			if (item is ConditionStepViewModel)
			{
				return ConditionTemplate;
			}
			if (item is FindObjectStepViewModel)
			{
				return FindObjectTemplate;
			}
			if (item is ForeachStepViewModel)
			{
				return ForeachTemplate;
			}
			if (item is PauseStepViewModel)
			{
				return PauseTemplate;
			}
			if (item is ProcedureSelectionStepViewModel)
			{
				return ProcedureSelectionTemplate;
			}
			if (item is ExitStepViewModel)
			{
				return ExitTemplate;
			}
			if (item is PersonInspectionStepViewModel)
			{
				return PersonInspectionTemplate;
			}
			if (item is SetGlobalValueStepViewModel)
			{
				return SetGlobalValueTemplate;
			}
			if (item is IncrementGlobalValueStepViewModel)
			{
				return IncrementGlobalValueTemplate;
			}
			if (item is ControlGKDeviceStepViewModel)
			{
				return ControlGKDeviceTemplate;
			}
			if (item is ControlSKDDeviceStepViewModel)
			{
				return ControlSKDDeviceTemplate;
			}
			if (item is ControlGKFireZoneStepViewModel)
			{
				return ControlGKFireZoneTemplate;
			}
			if (item is ControlGKGuardZoneStepViewModel)
			{
				return ControlGKGuardZoneTemplate;
			}
			if (item is ControlSKDZoneStepViewModel)
			{
				return ControlSKDZoneTemplate;
			}
			if (item is ControlCameraStepViewModel)
			{
				return ControlCameraTemplate;
			}
			if (item is ControlDirectionStepViewModel)
			{
				return ControlDirectionTemplate;
			}
			if (item is ControlDoorStepViewModel)
			{
				return ControlDoorTemplate;
			}
			
			return null;
		}
	}
}