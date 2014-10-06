using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class ExitStepViewModel : BaseStepViewModel
	{
		ExitArguments ExitArguments { get; set; }
		public ArgumentViewModel ExitCodeParameter { get; private set; }

		public ExitStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ExitArguments = stepViewModel.Step.ExitArguments;
			ExitCodeParameter = new ArgumentViewModel(ExitArguments.ExitCodeParameter, stepViewModel.Update);
			ExitCodeParameter.ExplicitType = ExplicitType.Integer;
		}

		public override void UpdateContent()
		{
			var allVariables = ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.Integer && !x.IsList);
			ExitCodeParameter.Update(allVariables);
		}

		public override string Description
		{
			get
			{
				return "Код выхода: " + ExitCodeParameter.Description;
			}
		}
	}
}