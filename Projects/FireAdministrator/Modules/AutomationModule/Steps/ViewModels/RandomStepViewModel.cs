using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class RandomStepViewModel : BaseStepViewModel
	{
		public RandomArguments RandomArguments { get; private set; }
		public ArgumentViewModel MaxValueArgument { get; private set; }
		public ArgumentViewModel ResultArgument { get; private set; }

		public RandomStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			RandomArguments = stepViewModel.Step.RandomArguments;
			MaxValueArgument = new ArgumentViewModel(RandomArguments.MaxValueArgument, stepViewModel.Update);
			MaxValueArgument.ExplicitType = ExplicitType.Integer;
			ResultArgument = new ArgumentViewModel(RandomArguments.ResultArgument, stepViewModel.Update, false);
			ResultArgument.ExplicitType = ExplicitType.Integer;
		}

		public override void UpdateContent()
		{
			var allVariables = ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.Integer && !x.IsList);
			MaxValueArgument.Update(allVariables);
			ResultArgument.Update(allVariables);
		}

		public override string Description
		{
			get 
			{
				return "Максимальное значение: " + MaxValueArgument.Description;
			}
		}
	}
}