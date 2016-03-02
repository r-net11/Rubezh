using System.Windows;
using System.Windows.Controls;

namespace AutomationModule.ViewModels
{
	public class StepTypeTemplateSelector : DataTemplateSelector
	{
		public DataTemplate SoundsTemplate { get; set; }
		public DataTemplate ShowMessageTemplate { get; set; }
		public DataTemplate JournalTemplate { get; set; }
		public DataTemplate ArithmeticTemplate { get; set; }
		public DataTemplate ConditionTemplate { get; set; }
		public DataTemplate FindObjectTemplate { get; set; }
		public DataTemplate ForeachTemplate { get; set; }
		public DataTemplate ForTemplate { get; set; }
		public DataTemplate PauseTemplate { get; set; }
		public DataTemplate ProcedureSelectionTemplate { get; set; }
		public DataTemplate SetValueTemplate { get; set; }
		public DataTemplate IncrementValueTemplate { get; set; }
		public DataTemplate ControlGKDeviceTemplate { get; set; }
		public DataTemplate ControlGKFireZoneTemplate { get; set; }
		public DataTemplate ControlGKGuardZoneTemplate { get; set; }
		public DataTemplate ControlDirectionTemplate { get; set; }
		public DataTemplate ControlGKDoorTemplate { get; set; }
		public DataTemplate ControlPumpStationTemplate { get; set; }
		public DataTemplate ControlMPTTemplate { get; set; }
		public DataTemplate ControlOpcDaTagTemplate { get; set; }
		public DataTemplate GetObjectPropertyTemplate { get; set; }
		public DataTemplate SendEmailTemplate { get; set; }
		public DataTemplate RunProgramTemplate { get; set; }
		public DataTemplate RandomTemplate { get; set; }
		public DataTemplate ChangeListTemplate { get; set; }
		public DataTemplate CheckPermissionTemplate { get; set; }
		public DataTemplate GetListCountTemplate { get; set; }
		public DataTemplate GetListItemTemplate { get; set; }
		public DataTemplate GetJournalItemTemplate { get; set; }
		public DataTemplate ControlVisualTemplate { get; set; }
		public DataTemplate ControlPlanTemplate { get; set; }
		public DataTemplate ShowDialogTemplate { get; set; }
		public DataTemplate CloseDialogTemplate { get; set; }
		public DataTemplate ControlDelayTemplate { get; set; }
		public DataTemplate ShowPropertyTemplate { get; set; }
		public DataTemplate ExportJournalTemplate { get; set; }
		public DataTemplate GenerateGuidTemplate { get; set; }
		public DataTemplate ExportOrganisationTemplate { get; set; }
		public DataTemplate ExportConfigurationTemplate { get; set; }
		public DataTemplate ImportOrganisationTemplate { get; set; }
		public DataTemplate ExportOrganisationListTemplate { get; set; }
		public DataTemplate ImportOrganisationListTemplate { get; set; }
		public DataTemplate SetJournalItemTemplate { get; set; }
		public DataTemplate PtzTemplate { get; set; }
		public DataTemplate StartRecordTemplate { get; set; }
		public DataTemplate StopRecordTemplate { get; set; }
		public DataTemplate RviAlarmTemplate { get; set; }
		public DataTemplate RviOpenWindowTemplate { get; set; }
		public DataTemplate NowTemplate { get; set; }
		public DataTemplate HttpRequestTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item is SoundStepViewModel)
				return SoundsTemplate;
			if (item is ShowMessageStepViewModel)
				return ShowMessageTemplate;
			if (item is JournalStepViewModel)
				return JournalTemplate;
			if (item is ArithmeticStepViewModel)
				return ArithmeticTemplate;
			if (item is ConditionStepViewModel)
				return ConditionTemplate;
			if (item is FindObjectStepViewModel)
				return FindObjectTemplate;
			if (item is ForeachStepViewModel)
				return ForeachTemplate;
			if (item is ForStepViewModel)
				return ForTemplate;
			if (item is PauseStepViewModel)
				return PauseTemplate;
			if (item is ProcedureSelectionStepViewModel)
				return ProcedureSelectionTemplate;
			if (item is SetValueStepViewModel)
				return SetValueTemplate;
			if (item is IncrementValueStepViewModel)
				return IncrementValueTemplate;
			if (item is ControlGKDeviceStepViewModel)
				return ControlGKDeviceTemplate;
			if (item is ControlGKFireZoneStepViewModel)
				return ControlGKFireZoneTemplate;
			if (item is ControlGKGuardZoneStepViewModel)
				return ControlGKGuardZoneTemplate;
			if (item is ControlDirectionStepViewModel)
				return ControlDirectionTemplate;
			if (item is ControlGKDoorStepViewModel)
				return ControlGKDoorTemplate;
			if (item is ControlPumpStationStepViewModel)
				return ControlPumpStationTemplate;
			if (item is ControlMPTStepViewModel)
				return ControlMPTTemplate;
			if (item is ControlDelayStepViewModel)
				return ControlDelayTemplate;
			if (item is ControlOpcDaTagStepViewModel)
				return ControlOpcDaTagTemplate;
			if (item is GetObjectPropertyStepViewModel)
				return GetObjectPropertyTemplate;
			if (item is SendEmailStepViewModel)
				return SendEmailTemplate;
			if (item is RunProgramStepViewModel)
				return RunProgramTemplate;
			if (item is RandomStepViewModel)
				return RandomTemplate;
			if (item is ChangeListStepViewModel)
				return ChangeListTemplate;
			if (item is CheckPermissionStepViewModel)
				return CheckPermissionTemplate;
			if (item is GetListCountStepViewModel)
				return GetListCountTemplate;
			if (item is GetListItemStepViewModel)
				return GetListItemTemplate;
			if (item is GetJournalItemStepViewModel)
				return GetJournalItemTemplate;
			if (item is ControlVisualStepViewModel)
				return ControlVisualTemplate;
			if (item is ControlPlanStepViewModel)
				return ControlPlanTemplate;
			if (item is ShowDialogStepViewModel)
				return ShowDialogTemplate;
			if (item is CloseDialogStepViewModel)
				return CloseDialogTemplate;
			if (item is ShowPropertyStepViewModel)
				return ShowPropertyTemplate;
			if (item is ExportJournalStepViewModel)
				return ExportJournalTemplate;
			if (item is ExportOrganisationStepViewModel)
				return ExportOrganisationTemplate;
			if (item is GenerateGuidStepViewModel)
				return GenerateGuidTemplate;
			if (item is SetJournalItemGuidStepViewModel)
				return SetJournalItemTemplate;
			if (item is ExportConfigurationStepViewModel)
				return ExportConfigurationTemplate;
			if (item is ImportOrganisationStepViewModel)
				return ImportOrganisationTemplate;
			if (item is ExportOrganisationListStepViewModel)
				return ExportOrganisationListTemplate;
			if (item is ImportOrganisationListStepViewModel)
				return ImportOrganisationListTemplate;
			if (item is PtzStepViewModel)
				return PtzTemplate;
			if (item is StartRecordStepViewModel)
				return StartRecordTemplate;
			if (item is StopRecordStepViewModel)
				return StopRecordTemplate;
			if (item is RviAlarmStepViewModel)
				return RviAlarmTemplate;
			if (item is RviOpenWindowStepViewModel)
				return RviOpenWindowTemplate;
			if (item is NowStepViewModel)
				return NowTemplate;
			if (item is HttpRequestStepViewModel)
				return HttpRequestTemplate;
			return null;
		}
	}
}