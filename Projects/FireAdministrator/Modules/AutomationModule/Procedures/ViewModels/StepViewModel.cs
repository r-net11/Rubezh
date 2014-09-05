using FiresecAPI;
using Infrastructure;
using Infrastructure.Common.TreeList;
using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class StepViewModel : TreeNodeViewModel<StepViewModel>
	{
		public ProcedureStep Step { get; private set; }
		public StepsViewModel StepsViewModel { get; private set; }
		public Procedure Procedure { get; private set; }
		public StepViewModel(StepsViewModel stepsViewModel, ProcedureStep step, Procedure procedure)
		{
			Procedure = procedure;
			StepsViewModel = stepsViewModel;
			Step = step;
			var automationChanged = ServiceFactory.SaveService.AutomationChanged;
			switch(step.ProcedureStepType)
			{
				case ProcedureStepType.PlaySound:
					Content = new SoundStepViewModel(step.SoundArguments, Update);
					break;

				case ProcedureStepType.ShowMessage:
					Content = new ShowMessageStepViewModel(step.ShowMessageArguments, procedure, Update);
					break;

				case ProcedureStepType.Arithmetics:
					Content = new ArithmeticStepViewModel(step.ArithmeticArguments, procedure, Update);
					break;

				case ProcedureStepType.If:
					Content = new ConditionStepViewModel(step.ConditionArguments, procedure, Update);
					break;

				case ProcedureStepType.AddJournalItem:
					Content = new JournalStepViewModel(step.JournalArguments, Update, procedure);
					break;

				case ProcedureStepType.FindObjects:
					Content = new FindObjectStepViewModel(step.FindObjectArguments, procedure);
					break;

				case ProcedureStepType.Foreach:
					Content = new ForeachStepViewModel(step.ForeachArguments, procedure);
					break;

				case ProcedureStepType.Pause:
					Content = new PauseStepViewModel(step.PauseArguments, procedure);
					break;
				
				case ProcedureStepType.ProcedureSelection:
					Content = new ProcedureSelectionStepViewModel(step.ProcedureSelectionArguments, procedure);
					break;

				case ProcedureStepType.Exit:
					Content = new ExitStepViewModel(step.ExitArguments, procedure);
					break;
					
				case ProcedureStepType.PersonInspection:
					Content = new PersonInspectionStepViewModel(step.PersonInspectionArguments, procedure);
					break;
				
				case ProcedureStepType.SetValue:
					Content = new SetValueStepViewModel(step.SetValueArguments, procedure);
					break;
				
				case ProcedureStepType.IncrementValue:
					Content = new IncrementValueStepViewModel(step.IncrementValueArguments, procedure);
					break;

				case ProcedureStepType.ControlGKDevice:
					Content = new ControlGKDeviceStepViewModel(step.ControlGKDeviceArguments, procedure);
					break;

				case ProcedureStepType.ControlSKDDevice:
					Content = new ControlSKDDeviceStepViewModel(step.ControlSKDDeviceArguments, procedure);
					break;

				case ProcedureStepType.ControlGKFireZone:
					Content = new ControlGKFireZoneStepViewModel(step.ControlGKFireZoneArguments, procedure);
					break;

				case ProcedureStepType.ControlGKGuardZone:
					Content = new ControlGKGuardZoneStepViewModel(step.ControlGKGuardZoneArguments, procedure);
					break;

				case ProcedureStepType.ControlDirection:
					Content = new ControlDirectionStepViewModel(step.ControlDirectionArguments, procedure);
					break;

				case ProcedureStepType.ControlDoor:
					Content = new ControlDoorStepViewModel(step.ControlDoorArguments, procedure);
					break;

				case ProcedureStepType.ControlSKDZone:
					Content = new ControlSKDZoneStepViewModel(step.ControlSKDZoneArguments, procedure);
					break;

				case ProcedureStepType.ControlCamera:
					Content = new ControlCameraStepViewModel(step.ControlCameraArguments, procedure);
					break;
				
				case ProcedureStepType.GetObjectField:
					Content = new GetObjectFieldStepViewModel(step.GetObjectFieldArguments, procedure);
					break;

				case ProcedureStepType.SendEmail:
					Content = new SendEmailStepViewModel(step.SendEmailArguments, procedure, Update);
					break;

				case ProcedureStepType.RunProgramm:
					Content = new RunProgrammStepViewModel(step.RunProgrammArguments, procedure, Update);
					break;
			}
			ServiceFactory.SaveService.AutomationChanged = automationChanged;
		}

		public void UpdateContent()
		{
			if (Content != null)
				Content.UpdateContent();
		}

		void OnChanged()
		{
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
		}

		public void Update(ProcedureStep step)
		{
			Step = step;
			OnPropertyChanged(() => Step);
			OnPropertyChanged(() => Name);
			Update();
		}

		public void Update()
		{
			OnPropertyChanged(() => Description);
			OnPropertyChanged(() => HasChildren);
		}

		public string Name
		{
			get { return Step.ProcedureStepType.ToDescription(); }
		}

		public string Description
		{
			get
			{
				if (Content != null)
					return Content.Description;
				return "";
			}
		}

		public IStepViewModel Content { get; private set; }

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