using FiresecAPI;
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

			switch(step.ProcedureStepType)
			{
				case ProcedureStepType.PlaySound:
					Content = new SoundStepViewModel(step.SoundArguments, Update);
					break;

				case ProcedureStepType.Arithmetics:
					Content = new ArithmeticStepViewModel(step.ArithmeticArguments, procedure, Update);
					break;

				case ProcedureStepType.If:
					Content = new ConditionStepViewModel(step.ConditionArguments, procedure);
					break;

				case ProcedureStepType.SendMessage:
					Content = new JournalStepViewModel(step);
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
			}
		}

		public void UpdateContent()
		{
			if (Content != null)
				Content.UpdateContent();
		}

		void OnChanged()
		{
			OnPropertyChanged("Name");
			OnPropertyChanged("Description");
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
					Step.ProcedureStepType == ProcedureStepType.ForeachBody ||
					Step.ProcedureStepType == ProcedureStepType.ForeachList ||
					Step.ProcedureStepType == ProcedureStepType.ForeachElement;
			}
		}
	}
}