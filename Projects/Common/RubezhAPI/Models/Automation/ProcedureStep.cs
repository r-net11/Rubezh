using RubezhAPI.StepArguments;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ProcedureStep
	{
		public ProcedureStep()
		{
			UID = Guid.NewGuid();
			Children = new List<ProcedureStep>();
			SoundArguments = new SoundArguments();
			ShowMessageArguments = new ShowMessageArguments();
			ArithmeticArguments = new ArithmeticArguments();
			CreateColorArguments = new CreateColorArguments();
			ConditionArguments = new ConditionArguments();
			FindObjectArguments = new FindObjectArguments();
			ForeachArguments = new ForeachArguments();
			ForArguments = new ForArguments();
			PauseArguments = new PauseArguments();
			ProcedureSelectionArguments = new ProcedureSelectionArguments();
			SetValueArguments = new SetValueArguments();
			IncrementValueArguments = new IncrementValueArguments();
			ControlGKDeviceArguments = new ControlGKDeviceArguments();
			ControlGKFireZoneArguments = new ControlGKFireZoneArguments();
			ControlGKGuardZoneArguments = new ControlGKGuardZoneArguments();
			ControlDirectionArguments = new ControlDirectionArguments();
			ControlPumpStationArguments = new ControlPumpStationArguments();
			ControlMPTArguments = new ControlMPTArguments();
			JournalArguments = new JournalArguments();
			GetObjectPropertyArguments = new GetObjectPropertyArguments();
			SendEmailArguments = new SendEmailArguments();
			RunProgramArguments = new RunProgramArguments();
			RandomArguments = new RandomArguments();
			ChangeListArguments = new ChangeListArguments();
			GetListCountArguments = new GetListCountArguments();
			GetListItemArguments = new GetListItemArguments();
			CheckPermissionArguments = new CheckPermissionArguments();
			GetJournalItemArguments = new GetJournalItemArguments();
			ControlVisualArguments = new ControlVisualArguments();
			ControlPlanArguments = new ControlPlanArguments();
			ShowDialogArguments = new ShowDialogArguments();
			CloseDialogArguments = new CloseDialogArguments();
			ControlDelayArguments = new ControlDelayArguments();
			ShowPropertyArguments = new ShowPropertyArguments();
			ExportJournalArguments = new ExportJournalArguments();
			ExportOrganisationArguments = new ExportOrganisationArguments();
			ExportConfigurationArguments = new ExportConfigurationArguments();
			ImportOrganisationArguments = new ImportOrganisationArguments();
			GenerateGuidArguments = new GenerateGuidArguments();
			SetJournalItemGuidArguments = new SetJournalItemGuidArguments();
			PtzArguments = new PtzArguments();
			StartRecordArguments = new StartRecordArguments();
			StopRecordArguments = new StopRecordArguments();
			RviAlarmArguments = new RviAlarmArguments();
			RviOpenWindowArguments = new RviOpenWindowArguments();
			ControlGKDoorArguments = new ControlGKDoorArguments();
			NowArguments = new NowArguments();
			HttpRequestArguments = new HttpRequestArguments();
			ControlOpcDaTagArguments = new ControlOpcDaTagArguments();
		}

		[XmlIgnore]
		public ProcedureStep Parent { get; set; }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public List<ProcedureStep> Children { get; set; }

		[DataMember]
		public ProcedureStepType ProcedureStepType { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public SoundArguments SoundArguments { get; set; }

		[DataMember]
		public ShowMessageArguments ShowMessageArguments { get; set; }

		[DataMember]
		public ArithmeticArguments ArithmeticArguments { get; set; }

		[DataMember]
		public CreateColorArguments CreateColorArguments { get; set; }

		[DataMember]
		public ConditionArguments ConditionArguments { get; set; }

		[DataMember]
		public FindObjectArguments FindObjectArguments { get; set; }

		[DataMember]
		public ForeachArguments ForeachArguments { get; set; }

		[DataMember]
		public ForArguments ForArguments { get; set; }

		[DataMember]
		public PauseArguments PauseArguments { get; set; }

		[DataMember]
		public ProcedureSelectionArguments ProcedureSelectionArguments { get; set; }

		[DataMember]
		public GetListCountArguments GetListCountArguments { get; set; }

		[DataMember]
		public GetListItemArguments GetListItemArguments { get; set; }

		[DataMember]
		public SetValueArguments SetValueArguments { get; set; }

		[DataMember]
		public IncrementValueArguments IncrementValueArguments { get; set; }

		[DataMember]
		public ControlGKDeviceArguments ControlGKDeviceArguments { get; set; }

		[DataMember]
		public ControlGKFireZoneArguments ControlGKFireZoneArguments { get; set; }

		[DataMember]
		public ControlGKGuardZoneArguments ControlGKGuardZoneArguments { get; set; }

		[DataMember]
		public ControlDirectionArguments ControlDirectionArguments { get; set; }

		[DataMember]
		public ControlDelayArguments ControlDelayArguments { get; set; }

		[DataMember]
		public ControlPumpStationArguments ControlPumpStationArguments { get; set; }

		[DataMember]
		public ControlMPTArguments ControlMPTArguments { get; set; }

		[DataMember]
		public JournalArguments JournalArguments { get; set; }

		[DataMember]
		public GetObjectPropertyArguments GetObjectPropertyArguments { get; set; }

		[DataMember]
		public SendEmailArguments SendEmailArguments { get; set; }

		[DataMember]
		public RunProgramArguments RunProgramArguments { get; set; }

		[DataMember]
		public RandomArguments RandomArguments { get; set; }

		[DataMember]
		public ChangeListArguments ChangeListArguments { get; set; }

		[DataMember]
		public CheckPermissionArguments CheckPermissionArguments { get; set; }

		[DataMember]
		public GetJournalItemArguments GetJournalItemArguments { get; set; }

		[DataMember]
		public ControlVisualArguments ControlVisualArguments { get; set; }

		[DataMember]
		public ControlPlanArguments ControlPlanArguments { get; set; }

		[DataMember]
		public ShowDialogArguments ShowDialogArguments { get; set; }
		[DataMember]
		public CloseDialogArguments CloseDialogArguments { get; set; }

		[DataMember]
		public ShowPropertyArguments ShowPropertyArguments { get; set; }

		[DataMember]
		public ExportJournalArguments ExportJournalArguments { get; set; }

		[DataMember]
		public ExportOrganisationArguments ExportOrganisationArguments { get; set; }

		[DataMember]
		public ExportConfigurationArguments ExportConfigurationArguments { get; set; }

		[DataMember]
		public ImportOrganisationArguments ImportOrganisationArguments { get; set; }

		[DataMember]
		public GenerateGuidArguments GenerateGuidArguments { get; set; }

		[DataMember]
		public SetJournalItemGuidArguments SetJournalItemGuidArguments { get; set; }

		[DataMember]
		public PtzArguments PtzArguments { get; set; }

		[DataMember]
		public StartRecordArguments StartRecordArguments { get; set; }

		[DataMember]
		public StopRecordArguments StopRecordArguments { get; set; }

		[DataMember]
		public RviAlarmArguments RviAlarmArguments { get; set; }

		[DataMember]
		public RviOpenWindowArguments RviOpenWindowArguments { get; set; }

		[DataMember]
		public ControlGKDoorArguments ControlGKDoorArguments { get; set; }

		[DataMember]
		public NowArguments NowArguments { get; set; }

		[DataMember]
		public HttpRequestArguments HttpRequestArguments { get; set; }

		[DataMember]
		public ControlOpcDaTagArguments ControlOpcDaTagArguments { get; set; }
	}
}