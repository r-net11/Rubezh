using System.IO;
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
		public string ImageSource { get; private set; }

		public StepViewModel(StepsViewModel stepsViewModel, ProcedureStep step, Procedure procedure)
		{
			Procedure = procedure;
			StepsViewModel = stepsViewModel;
			Step = step;
			var automationChanged = ServiceFactory.SaveService.AutomationChanged;
			ImageSource = "/Controls;component/StepIcons/Step.png";
			if ((step.ProcedureStepType == ProcedureStepType.Arithmetics) || (step.ProcedureStepType == ProcedureStepType.SetValue)
				|| (step.ProcedureStepType == ProcedureStepType.IncrementValue) || (step.ProcedureStepType == ProcedureStepType.FindObjects)
				|| (step.ProcedureStepType == ProcedureStepType.GetObjectProperty) || (step.ProcedureStepType == ProcedureStepType.Random)
				|| (step.ProcedureStepType == ProcedureStepType.ChangeList) || (step.ProcedureStepType == ProcedureStepType.GetListCount)
				|| (step.ProcedureStepType == ProcedureStepType.GetListItem) || (step.ProcedureStepType == ProcedureStepType.PlaySound)
				|| (step.ProcedureStepType == ProcedureStepType.AddJournalItem) || (step.ProcedureStepType == ProcedureStepType.SendEmail)
				|| (step.ProcedureStepType == ProcedureStepType.ShowMessage) 
				|| (step.ProcedureStepType == ProcedureStepType.ControlVisualGet) || (step.ProcedureStepType == ProcedureStepType.ControlVisualSet)
				|| (step.ProcedureStepType == ProcedureStepType.Exit) || (step.ProcedureStepType == ProcedureStepType.Pause)
				|| (step.ProcedureStepType == ProcedureStepType.ProcedureSelection) || (step.ProcedureStepType == ProcedureStepType.CheckPermission)
				|| (step.ProcedureStepType == ProcedureStepType.For) || (step.ProcedureStepType == ProcedureStepType.While)
				|| (step.ProcedureStepType == ProcedureStepType.Break) || (step.ProcedureStepType == ProcedureStepType.Continue)
				|| (step.ProcedureStepType == ProcedureStepType.GetJournalItem))
				ImageSource = "/Controls;component/StepIcons/" + step.ProcedureStepType + ".png";
			if ((step.ProcedureStepType == ProcedureStepType.ControlCamera) || (step.ProcedureStepType == ProcedureStepType.ControlDirection)
				|| (step.ProcedureStepType == ProcedureStepType.ControlDoor) || (step.ProcedureStepType == ProcedureStepType.ControlGKDevice)
				|| (step.ProcedureStepType == ProcedureStepType.ControlGKFireZone) || (step.ProcedureStepType == ProcedureStepType.ControlGKGuardZone)
				|| (step.ProcedureStepType == ProcedureStepType.ControlSKDDevice) || (step.ProcedureStepType == ProcedureStepType.ControlSKDZone))
				ImageSource = "/Controls;component/StepIcons/Control.png";
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
					Content = new ControlVisualStepViewModel(this, ControlVisualType.Get);
					break;

				case ProcedureStepType.ControlVisualSet:
					Content = new ControlVisualStepViewModel(this, ControlVisualType.Set);
					break;

				case ProcedureStepType.ControlPlan:
					Content = new ControlPlanStepViewModel(this);
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