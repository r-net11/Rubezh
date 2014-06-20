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
					Content = new SoundStepViewModel(step);
					break;

				case ProcedureStepType.Arithmetics:
					Content = new ArithmeticStepViewModel(step, procedure);
					break;

				case ProcedureStepType.SendMessage:
					Content = new JournalStepViewModel(step);
					break;
			}
		}

		public void UpdateContent()
		{
			var content = Content as IStepViewModel;
			if (content != null)
				content.UpdateContent();
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
			OnPropertyChanged(() => Description);
			Update();
		}

		public void Update()
		{
			OnPropertyChanged(() => HasChildren);
		}

		public string Name
		{
			get { return Step.ProcedureStepType.ToDescription(); }
		}

		public string Description
		{
			get { return ""; }
		}

		public object Content { get; private set; }

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