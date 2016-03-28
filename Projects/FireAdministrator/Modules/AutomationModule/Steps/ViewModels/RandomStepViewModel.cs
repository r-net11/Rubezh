using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class RandomStepViewModel : BaseStepViewModel
	{
		public RandomStep RandomStep { get; private set; }
		public ArgumentViewModel MaxValueArgument { get; private set; }
		public ArgumentViewModel ResultArgument { get; private set; }

		public RandomStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			RandomStep = (RandomStep)stepViewModel.Step;
			MaxValueArgument = new ArgumentViewModel(RandomStep.MaxValueArgument, stepViewModel.Update, UpdateContent);
			MaxValueArgument.ExplicitValue.MinIntValue = 1;
			ResultArgument = new ArgumentViewModel(RandomStep.ResultArgument, stepViewModel.Update, UpdateContent, false);
		}

		public override void UpdateContent()
		{
			MaxValueArgument.Update(Procedure, ExplicitType.Integer, isList: false);
			ResultArgument.Update(Procedure, ExplicitType.Integer, isList: false);
		}

		public override string Description
		{
			get
			{
				return "Результат:" + ResultArgument.Description + " Максимальное значение: " + MaxValueArgument.Description;
			}
		}
	}
}