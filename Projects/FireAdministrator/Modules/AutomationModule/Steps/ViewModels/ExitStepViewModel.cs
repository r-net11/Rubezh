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
			ExitCodeArgument = new ArgumentViewModel(ExitArguments.ExitCodeArgument, stepViewModel.Update);
			ExitCodeArgument.ExplicitType = ExplicitType.Integer;
		}

		public override void UpdateContent()
		{
			var allVariables = ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.Integer && !x.IsList);
			ExitCodeArgument.Update(allVariables);
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