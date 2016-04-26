using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class ExitStepViewModel : BaseStepViewModel
	{
		ExitArguments ExitArguments { get; set; }
		public ArgumentViewModel ExitCodeArgument { get; private set; }

		public ExitStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ExitArguments = stepViewModel.Step.ExitArguments;
			ExitCodeArgument = new ArgumentViewModel(ExitArguments.ExitCodeArgument, stepViewModel.Update, null);
		}

		public override void UpdateContent()
		{
			ExitCodeArgument.Update(Procedure, ExplicitType.Integer);
		}

		public override string Description
		{
			get
			{
				return "Код выхода: " + ExitCodeArgument.Description;
			}
		}
	}
}