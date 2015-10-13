using RubezhAPI.Automation;

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
			MaxValueArgument = new ArgumentViewModel(RandomArguments.MaxValueArgument, stepViewModel.Update, UpdateContent);
			MaxValueArgument.ExplicitValue.MinIntValue = 1;
			ResultArgument = new ArgumentViewModel(RandomArguments.ResultArgument, stepViewModel.Update, UpdateContent, false);
		}

		public override void UpdateContent()
		{
			MaxValueArgument.Update(Procedure, ExplicitType.Integer, isList:false);
			ResultArgument.Update(Procedure, ExplicitType.Integer, isList: false);
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