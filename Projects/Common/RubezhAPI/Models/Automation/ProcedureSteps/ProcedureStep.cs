using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	[XmlInclude(typeof(ArithmeticStep))]
	[XmlInclude(typeof(ChangeListStep))]
	[XmlInclude(typeof(CheckPermissionStep))]
	[XmlInclude(typeof(CloseDialogStep))]
	[XmlInclude(typeof(IfStep))]
	[XmlInclude(typeof(IfYesStep))]
	[XmlInclude(typeof(IfNoStep))]
	[XmlInclude(typeof(WhileStep))]
	[XmlInclude(typeof(ControlDelayStep))]
	[XmlInclude(typeof(ControlDirectionStep))]
	[XmlInclude(typeof(ControlGKDeviceStep))]
	[XmlInclude(typeof(ControlGKDoorStep))]
	[XmlInclude(typeof(ControlGKFireZoneStep))]
	[XmlInclude(typeof(ControlGKGuardZoneStep))]
	[XmlInclude(typeof(ControlMPTStep))]
	[XmlInclude(typeof(ControlOpcDaTagGetStep))]
	[XmlInclude(typeof(ControlOpcDaTagSetStep))]
	[XmlInclude(typeof(ControlPlanGetStep))]
	[XmlInclude(typeof(ControlPlanSetStep))]
	[XmlInclude(typeof(ControlPumpStationStep))]
	[XmlInclude(typeof(ControlVisualGetStep))]
	[XmlInclude(typeof(ControlVisualSetStep))]
	[XmlInclude(typeof(CreateColorStep))]
	[XmlInclude(typeof(ExportConfigurationStep))]
	[XmlInclude(typeof(ExportJournalStep))]
	[XmlInclude(typeof(ExportOrganisationListStep))]
	[XmlInclude(typeof(ExportOrganisationStep))]
	[XmlInclude(typeof(FindObjectStep))]
	[XmlInclude(typeof(ForeachStep))]
	[XmlInclude(typeof(ForeachBodyStep))]
	[XmlInclude(typeof(ForStep))]
	[XmlInclude(typeof(GenerateGuidStep))]
	[XmlInclude(typeof(GetJournalItemStep))]
	[XmlInclude(typeof(GetListCountStep))]
	[XmlInclude(typeof(GetListItemStep))]
	[XmlInclude(typeof(GetObjectPropertyStep))]
	[XmlInclude(typeof(HttpRequestStep))]
	[XmlInclude(typeof(ImportOrganisationListStep))]
	[XmlInclude(typeof(ImportOrganisationStep))]
	[XmlInclude(typeof(IncrementValueStep))]
	[XmlInclude(typeof(JournalStep))]
	[XmlInclude(typeof(NowStep))]
	[XmlInclude(typeof(PauseStep))]
	[XmlInclude(typeof(ProcedureSelectionStep))]
	[XmlInclude(typeof(PtzStep))]
	[XmlInclude(typeof(RandomStep))]
	[XmlInclude(typeof(RunProgramStep))]
	[XmlInclude(typeof(RviAlarmStep))]
	[XmlInclude(typeof(RviOpenWindowStep))]
	[XmlInclude(typeof(SendEmailStep))]
	[XmlInclude(typeof(SetJournalItemGuidStep))]
	[XmlInclude(typeof(SetValueStep))]
	[XmlInclude(typeof(ShowDialogStep))]
	[XmlInclude(typeof(ShowMessageStep))]
	[XmlInclude(typeof(ShowPropertyStep))]
	[XmlInclude(typeof(SoundStep))]
	[XmlInclude(typeof(StartRecordStep))]
	[XmlInclude(typeof(StopRecordStep))]
	[XmlInclude(typeof(ExitStep))]
	public class ProcedureStep
	{
		public ProcedureStep()
		{
			UID = Guid.NewGuid();
			Children = new List<ProcedureStep>();
		}

		public virtual ProcedureStepType ProcedureStepType { get { return ProcedureStepType.Null; } }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public List<ProcedureStep> Children { get; set; }

		public static T CreateStep<T>() where T : ProcedureStep, new()
		{
			var step = new T();
			if (step is UIStep)
				(step as UIStep).LayoutFilter.Add(Guid.Empty);
			return step;
		}

		public static ProcedureStep CreateStep(ProcedureStepType procedureStepType)
		{
			switch (procedureStepType)
			{
				case ProcedureStepType.If: return CreateStep<IfStep>();
				case ProcedureStepType.While: return CreateStep<WhileStep>();
				case ProcedureStepType.GenerateGuid: return CreateStep<GenerateGuidStep>();
				case ProcedureStepType.Foreach: return CreateStep<ForeachStep>();
				case ProcedureStepType.For: return CreateStep<ForStep>();
				case ProcedureStepType.ProcedureSelection: return CreateStep<ProcedureSelectionStep>();
				case ProcedureStepType.GetObjectProperty: return CreateStep<GetObjectPropertyStep>();
				case ProcedureStepType.Arithmetics: return CreateStep<ArithmeticStep>();
				case ProcedureStepType.CreateColor: return CreateStep<CreateColorStep>();
				case ProcedureStepType.PlaySound: return CreateStep<SoundStep>();
				case ProcedureStepType.Pause: return CreateStep<PauseStep>();
				case ProcedureStepType.AddJournalItem: return CreateStep<JournalStep>();
				case ProcedureStepType.ShowMessage: return CreateStep<ShowMessageStep>();
				case ProcedureStepType.FindObjects: return CreateStep<FindObjectStep>();
				case ProcedureStepType.ControlGKDevice: return CreateStep<ControlGKDeviceStep>();
				case ProcedureStepType.ControlGKFireZone: return CreateStep<ControlGKFireZoneStep>();
				case ProcedureStepType.ControlGKGuardZone: return CreateStep<ControlGKGuardZoneStep>();
				case ProcedureStepType.ControlDirection: return CreateStep<ControlDirectionStep>();
				case ProcedureStepType.ControlGKDoor: return CreateStep<ControlGKDoorStep>();
				case ProcedureStepType.ControlDelay: return CreateStep<ControlDelayStep>();
				case ProcedureStepType.ControlPumpStation: return CreateStep<ControlPumpStationStep>();
				case ProcedureStepType.ControlMPT: return CreateStep<ControlMPTStep>();
				case ProcedureStepType.IncrementValue: return CreateStep<IncrementValueStep>();
				case ProcedureStepType.SetValue: return CreateStep<SetValueStep>();
				case ProcedureStepType.Random: return CreateStep<RandomStep>();
				case ProcedureStepType.ChangeList: return CreateStep<ChangeListStep>();
				case ProcedureStepType.CheckPermission: return CreateStep<CheckPermissionStep>();
				case ProcedureStepType.GetListCount: return CreateStep<GetListCountStep>();
				case ProcedureStepType.GetListItem: return CreateStep<GetListItemStep>();
				case ProcedureStepType.GetJournalItem: return CreateStep<GetJournalItemStep>();
				case ProcedureStepType.ControlVisualGet: return CreateStep<ControlVisualGetStep>();
				case ProcedureStepType.ControlVisualSet: return CreateStep<ControlVisualSetStep>();
				case ProcedureStepType.ControlPlanGet: return CreateStep<ControlPlanGetStep>();
				case ProcedureStepType.ControlPlanSet: return CreateStep<ControlPlanSetStep>();
				case ProcedureStepType.ControlOpcDaTagGet: return CreateStep<ControlOpcDaTagGetStep>();
				case ProcedureStepType.ControlOpcDaTagSet: return CreateStep<ControlOpcDaTagSetStep>();
				case ProcedureStepType.ShowDialog: return CreateStep<ShowDialogStep>();
				case ProcedureStepType.CloseDialog: return CreateStep<CloseDialogStep>();
				case ProcedureStepType.ShowProperty: return CreateStep<ShowPropertyStep>();
				case ProcedureStepType.SendEmail: return CreateStep<SendEmailStep>();
				case ProcedureStepType.Exit: return CreateStep<ExitStep>();
				case ProcedureStepType.Break: return CreateStep<BreakStep>();
				case ProcedureStepType.Continue: return CreateStep<ContinueStep>();
				case ProcedureStepType.ExportJournal: return CreateStep<ExportJournalStep>();
				case ProcedureStepType.ExportOrganisation: return CreateStep<ExportOrganisationStep>();
				case ProcedureStepType.ImportOrganisation: return CreateStep<ImportOrganisationStep>();
				case ProcedureStepType.ExportOrganisationList: return CreateStep<ExportOrganisationListStep>();
				case ProcedureStepType.ImportOrganisationList: return CreateStep<ImportOrganisationListStep>();
				case ProcedureStepType.ExportConfiguration: return CreateStep<ExportConfigurationStep>();
				case ProcedureStepType.Ptz: return CreateStep<PtzStep>();
				case ProcedureStepType.StartRecord: return CreateStep<StartRecordStep>();
				case ProcedureStepType.StopRecord: return CreateStep<StopRecordStep>();
				case ProcedureStepType.RviAlarm: return CreateStep<RviAlarmStep>();
				case ProcedureStepType.RviOpenWindow: return CreateStep<RviOpenWindowStep>();
				case ProcedureStepType.Now: return CreateStep<NowStep>();
				case ProcedureStepType.RunProgram: return CreateStep<RunProgramStep>();
				case ProcedureStepType.HttpRequest: return CreateStep<HttpRequestStep>();
				default: return new ProcedureStep();
			}
		}

		[IgnoreDataMember]
		public virtual Argument[] Arguments { get { return new Argument[0]; } }
	}

	[DataContract]
	public class ExitStep : ProcedureStep { public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.Exit; } } }
	[DataContract]
	public class BreakStep : ProcedureStep { public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.Break; } } }
	[DataContract]
	public class ContinueStep : ProcedureStep { public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.Continue; } } }
}