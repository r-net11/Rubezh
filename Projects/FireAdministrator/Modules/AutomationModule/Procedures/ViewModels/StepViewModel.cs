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
					Content = new SoundStepViewModel(this);
					break;

				case ProcedureStepType.ShowMessage:
					Content = new ShowMessageStepViewModel(this);
					break;

				case ProcedureStepType.Arithmetics:
					Content = new ArithmeticStepViewModel(this);
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

				case ProcedureStepType.Pause:
					Content = new PauseStepViewModel(this);
					break;
				
				case ProcedureStepType.ProcedureSelection:
					Content = new ProcedureSelectionStepViewModel(this);
					break;

				case ProcedureStepType.Exit:
					Content = new ExitStepViewModel(this);
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

				case ProcedureStepType.ControlSKDDevice:
					Content = new ControlSKDDeviceStepViewModel(this);
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

				case ProcedureStepType.ControlDoor:
					Content = new ControlDoorStepViewModel(this);
					break;

				case ProcedureStepType.ControlSKDZone:
					Content = new ControlSKDZoneStepViewModel(this);
					break;

				case ProcedureStepType.ControlCamera:
					Content = new ControlCameraStepViewModel(this);
					break;
				
				case ProcedureStepType.GetObjectProperty:
					Content = new GetObjectPropertyStepViewModel(this);
					break;

				case ProcedureStepType.SendEmail:
					Content = new SendEmailStepViewModel(this);
					break;

				case ProcedureStepType.RunProgramm:
					Content = new RunProgrammStepViewModel(this);
					break;
			}
			ServiceFactory.SaveService.AutomationChanged = automationChanged;
		}

		public void UpdateContent()
		{
			if (Content != null)
				Content.UpdateContent();
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