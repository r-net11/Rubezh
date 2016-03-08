using Infrastructure;
using Infrastructure.Common.TreeList;
using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class StepViewModel : TreeNodeViewModel<StepViewModel>
	{
		public ProcedureStep Step { get; private set; }
		public Procedure Procedure { get; private set; }
		public string ImageSource { get; private set; }

		public StepViewModel(ProcedureStep step, Procedure procedure)
		{
			Procedure = procedure;
			Step = step;
			var automationChanged = ServiceFactory.SaveService.AutomationChanged;
			if ((step.ProcedureStepType == ProcedureStepType.ControlDirection)
				|| (step.ProcedureStepType == ProcedureStepType.ControlGKDevice) || (step.ProcedureStepType == ProcedureStepType.ControlGKDoor)
				|| (step.ProcedureStepType == ProcedureStepType.ControlGKFireZone) || (step.ProcedureStepType == ProcedureStepType.ControlGKGuardZone)
				|| (step.ProcedureStepType == ProcedureStepType.ControlPumpStation) || (step.ProcedureStepType == ProcedureStepType.ControlMPT)
				|| (step.ProcedureStepType == ProcedureStepType.ControlDelay) || (step.ProcedureStepType == ProcedureStepType.Ptz)
				|| (step.ProcedureStepType == ProcedureStepType.StartRecord) || (step.ProcedureStepType == ProcedureStepType.StopRecord) || (step.ProcedureStepType == ProcedureStepType.RviAlarm) || (step.ProcedureStepType == ProcedureStepType.RviOpenWindow)
				|| (step.ProcedureStepType == ProcedureStepType.ControlOpcDaTagGet) || (step.ProcedureStepType == ProcedureStepType.ControlOpcDaTagSet))
				ImageSource = "/Controls;component/StepIcons/Control.png";
			else
				ImageSource = "/Controls;component/StepIcons/" + step.ProcedureStepType + ".png";
			switch (step.ProcedureStepType)
			{
				case ProcedureStepType.PlaySound:
					Content = new SoundStepViewModel(this);
					break;

				case ProcedureStepType.ShowMessage:
					Content = new ShowMessageStepViewModel(this);
					break;

				case ProcedureStepType.Arithmetics:
					Content = new ArithmeticStepViewModel(this);
					break;

				case ProcedureStepType.CreateColor:
					Content = new CreateColorStepViewModel(this);
					break;

				case ProcedureStepType.If:
					Content = new ConditionStepViewModel(this);
					break;

				case ProcedureStepType.AddJournalItem:
					Content = new JournalStepViewModel(this);
					break;

				case ProcedureStepType.FindObjects:
					Content = new FindObjectStepViewModel(this);
					break;

				case ProcedureStepType.Foreach:
					Content = new ForeachStepViewModel(this);
					break;

				case ProcedureStepType.For:
					Content = new ForStepViewModel(this);
					break;

				case ProcedureStepType.While:
					Content = new ConditionStepViewModel(this);
					break;

				case ProcedureStepType.Pause:
					Content = new PauseStepViewModel(this);
					break;

				case ProcedureStepType.ProcedureSelection:
					Content = new ProcedureSelectionStepViewModel(this);
					break;

				case ProcedureStepType.SetValue:
					Content = new SetValueStepViewModel(this);
					break;

				case ProcedureStepType.IncrementValue:
					Content = new IncrementValueStepViewModel(this);
					break;

				case ProcedureStepType.Random:
					Content = new RandomStepViewModel(this);
					break;

				case ProcedureStepType.ControlGKDevice:
					Content = new ControlGKDeviceStepViewModel(this);
					break;

				case ProcedureStepType.ControlGKFireZone:
					Content = new ControlGKFireZoneStepViewModel(this);
					break;

				case ProcedureStepType.ControlGKGuardZone:
					Content = new ControlGKGuardZoneStepViewModel(this);
					break;

				case ProcedureStepType.ControlDirection:
					Content = new ControlDirectionStepViewModel(this);
					break;

				case ProcedureStepType.ControlGKDoor:
					Content = new ControlGKDoorStepViewModel(this);
					break;

				case ProcedureStepType.ControlDelay:
					Content = new ControlDelayStepViewModel(this);
					break;

				case ProcedureStepType.ControlPumpStation:
					Content = new ControlPumpStationStepViewModel(this);
					break;

				case ProcedureStepType.ControlMPT:
					Content = new ControlMPTStepViewModel(this);
					break;

				case ProcedureStepType.ControlOpcDaTagGet:
					Content = new ControlOpcDaTagStepViewModel(this, ControlElementType.Get);
					break;

				case ProcedureStepType.ControlOpcDaTagSet:
					Content = new ControlOpcDaTagStepViewModel(this, ControlElementType.Set);
					break;

				case ProcedureStepType.GetObjectProperty:
					Content = new GetObjectPropertyStepViewModel(this);
					break;

				case ProcedureStepType.SendEmail:
					Content = new SendEmailStepViewModel(this);
					break;

				case ProcedureStepType.RunProgram:
					Content = new RunProgramStepViewModel(this);
					break;

				case ProcedureStepType.ChangeList:
					Content = new ChangeListStepViewModel(this);
					break;

				case ProcedureStepType.CheckPermission:
					Content = new CheckPermissionStepViewModel(this);
					break;

				case ProcedureStepType.GetListCount:
					Content = new GetListCountStepViewModel(this);
					break;

				case ProcedureStepType.GetListItem:
					Content = new GetListItemStepViewModel(this);
					break;

				case ProcedureStepType.GetJournalItem:
					Content = new GetJournalItemStepViewModel(this);
					break;

				case ProcedureStepType.ControlVisualGet:
					Content = new ControlVisualStepViewModel(this, ControlElementType.Get);
					break;

				case ProcedureStepType.ControlVisualSet:
					Content = new ControlVisualStepViewModel(this, ControlElementType.Set);
					break;

				case ProcedureStepType.ControlPlanGet:
					Content = new ControlPlanStepViewModel(this, ControlElementType.Get);
					break;

				case ProcedureStepType.ControlPlanSet:
					Content = new ControlPlanStepViewModel(this, ControlElementType.Set);
					break;

				case ProcedureStepType.ShowDialog:
					Content = new ShowDialogStepViewModel(this);
					break;

				case ProcedureStepType.CloseDialog:
					Content = new CloseDialogStepViewModel(this);
					break;

				case ProcedureStepType.ShowProperty:
					Content = new ShowPropertyStepViewModel(this);
					break;

				case ProcedureStepType.GenerateGuid:
					Content = new GenerateGuidStepViewModel(this);
					break;

				case ProcedureStepType.SetJournalItemGuid:
					Content = new SetJournalItemGuidStepViewModel(this);
					break;

				case ProcedureStepType.ExportJournal:
					Content = new ExportJournalStepViewModel(this);
					break;

				case ProcedureStepType.ExportOrganisation:
					Content = new ExportOrganisationStepViewModel(this);
					break;

				case ProcedureStepType.ExportConfiguration:
					Content = new ExportConfigurationStepViewModel(this);
					break;

				case ProcedureStepType.ImportOrganisation:
					Content = new ImportOrganisationStepViewModel(this);
					break;

				case ProcedureStepType.ExportOrganisationList:
					Content = new ExportOrganisationListStepViewModel(this);
					break;

				case ProcedureStepType.ImportOrganisationList:
					Content = new ImportOrganisationListStepViewModel(this);
					break;

				case ProcedureStepType.Ptz:
					Content = new PtzStepViewModel(this);
					break;

				case ProcedureStepType.StartRecord:
					Content = new StartRecordStepViewModel(this);
					break;

				case ProcedureStepType.StopRecord:
					Content = new StopRecordStepViewModel(this);
					break;

				case ProcedureStepType.RviAlarm:
					Content = new RviAlarmStepViewModel(this);
					break;

				case ProcedureStepType.RviOpenWindow:
					Content = new RviOpenWindowStepViewModel(this);
					break;

				case ProcedureStepType.Now:
					Content = new NowStepViewModel(this);
					break;

				case ProcedureStepType.HttpRequest:
					Content = new HttpRequestStepViewModel(this);
					break;
			}
			UpdateContent();
			ServiceFactory.SaveService.AutomationChanged = automationChanged;
		}

		public void UpdateContent()
		{
			if (Content != null)
				Content.UpdateContent();
		}

		public void Update()
		{
			OnPropertyChanged(() => Content);
			OnPropertyChanged(() => HasChildren);
		}

		public BaseStepViewModel Content { get; private set; }

		public bool IsVirtual
		{
			get
			{
				return
					Step.ProcedureStepType == ProcedureStepType.IfYes ||
					Step.ProcedureStepType == ProcedureStepType.IfNo ||
					Step.ProcedureStepType == ProcedureStepType.ForeachBody;
			}
		}
	}
}