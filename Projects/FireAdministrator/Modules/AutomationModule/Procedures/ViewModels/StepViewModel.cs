using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.TreeList;

namespace AutomationModule.ViewModels
{
	public class StepViewModel : TreeNodeViewModel<StepViewModel>
	{
		public ProcedureStep Step { get; private set; }
		public StepsViewModel StepsViewModel { get; private set; }
		public Procedure Procedure { get; private set; }
		public string ImageSource { get; private set; }

		public StepViewModel(StepsViewModel stepsViewModel, ProcedureStep step, Procedure procedure)
		{
			Procedure = procedure;
			StepsViewModel = stepsViewModel;
			Step = step;
			var automationChanged = ServiceFactory.SaveService.AutomationChanged;
			ImageSource = ProcedureHelper.GetIconForProcedure(step.ProcedureStepType);
			Content = GetContentForStep(step);

			UpdateContent();
			ServiceFactory.SaveService.AutomationChanged = automationChanged;
		}

		private BaseStepViewModel GetContentForStep(ProcedureStep step)
		{
			switch (step.ProcedureStepType)
			{
				case ProcedureStepType.PlaySound:
					return new SoundStepViewModel(this);

				case ProcedureStepType.ShowMessage:
					return new ShowMessageStepViewModel(this);

				case ProcedureStepType.Arithmetics:
					return new ArithmeticStepViewModel(this);

				case ProcedureStepType.If:
					return new ConditionStepViewModel(this);

				case ProcedureStepType.AddJournalItem:
					return new JournalStepViewModel(this);

				case ProcedureStepType.FindObjects:
					return new FindObjectStepViewModel(this);

				case ProcedureStepType.Foreach:
					return new ForeachStepViewModel(this);

				case ProcedureStepType.For:
					return new ForStepViewModel(this);

				case ProcedureStepType.While:
					return new ConditionStepViewModel(this);

				case ProcedureStepType.Pause:
					return new PauseStepViewModel(this);

				case ProcedureStepType.ProcedureSelection:
					return new ProcedureSelectionStepViewModel(this);

				case ProcedureStepType.Exit:
					return new ExitStepViewModel(this);

				case ProcedureStepType.SetValue:
					return new SetValueStepViewModel(this);

				case ProcedureStepType.IncrementValue:
					return new IncrementValueStepViewModel(this);

				case ProcedureStepType.Random:
					return new RandomStepViewModel(this);

				case ProcedureStepType.ControlSKDDevice:
					return new ControlSKDDeviceStepViewModel(this);

				case ProcedureStepType.ControlDoor:
					return new ControlDoorStepViewModel(this);

				case ProcedureStepType.ControlSKDZone:
					return new ControlSKDZoneStepViewModel(this);

				case ProcedureStepType.GetObjectProperty:
					return new GetObjectPropertyStepViewModel(this);

				case ProcedureStepType.SendEmail:
					return new SendEmailStepViewModel(this);

				case ProcedureStepType.RunProgram:
					return new RunProgramStepViewModel(this);

				case ProcedureStepType.ChangeList:
					return new ChangeListStepViewModel(this);

				case ProcedureStepType.CheckPermission:
					return new CheckPermissionStepViewModel(this);

				case ProcedureStepType.GetListCount:
					return new GetListCountStepViewModel(this);

				case ProcedureStepType.GetListItem:
					return new GetListItemStepViewModel(this);

				case ProcedureStepType.GetJournalItem:
					return new GetJournalItemStepViewModel(this);

				case ProcedureStepType.ControlVisualGet:
					return new ControlVisualStepViewModel(this, ControlElementType.Get);

				case ProcedureStepType.ControlVisualSet:
					return new ControlVisualStepViewModel(this, ControlElementType.Set);

				case ProcedureStepType.ControlPlanGet:
					return new ControlPlanStepViewModel(this, ControlElementType.Get);

				case ProcedureStepType.ControlPlanSet:
					return new ControlPlanStepViewModel(this, ControlElementType.Set);

				case ProcedureStepType.ShowDialog:
					return new ShowDialogStepViewModel(this);

				case ProcedureStepType.ShowProperty:
					return new ShowPropertyStepViewModel(this);

				case ProcedureStepType.GenerateGuid:
					return new GenerateGuidStepViewModel(this);

				case ProcedureStepType.SetJournalItemGuid:
					return new SetJournalItemGuidStepViewModel(this);

				case ProcedureStepType.ExportReport:
					return new ExportReportStepViewModel(this);

				case ProcedureStepType.ExportJournal:
					return new ExportJournalStepViewModel(this);

				case ProcedureStepType.ExportOrganisation:
					return new ExportOrganisationStepViewModel(this);

				case ProcedureStepType.ExportConfiguration:
					return new ExportConfigurationStepViewModel(this);

				case ProcedureStepType.ImportOrganisation:
					return new ImportOrganisationStepViewModel(this);

				case ProcedureStepType.ExportOrganisationList:
					return new ExportOrganisationListStepViewModel(this);

				case ProcedureStepType.ImportOrganisationList:
					return new ImportOrganisationListStepViewModel(this);

				case ProcedureStepType.Ptz:
					return new PtzStepViewModel(this);

				case ProcedureStepType.StartRecord:
					return new StartRecordStepViewModel(this);

				case ProcedureStepType.StopRecord:
					return new StopRecordStepViewModel(this);

				case ProcedureStepType.RviAlarm:
					return new RviAlarmStepViewModel(this);
				default:
					return null;
			}
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