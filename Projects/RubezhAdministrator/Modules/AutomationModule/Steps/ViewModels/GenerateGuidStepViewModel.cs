using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class GenerateGuidStepViewModel : BaseStepViewModel
	{
		public ArgumentViewModel ResultArgument { get; private set; }
		public GenerateGuidStep GenerateGuidStep { get; private set; }

		public GenerateGuidStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			GenerateGuidStep = (GenerateGuidStep)stepViewModel.Step;
			ResultArgument = new ArgumentViewModel(GenerateGuidStep.ResultArgument, stepViewModel.Update, UpdateContent, false);
		}

		public override void UpdateContent()
		{
			ResultArgument.Update(Procedure, ExplicitType.String, isList: false);
		}

		public override string Description
		{
			get
			{
				return "Результат: " + ResultArgument.Description;
			}
		}
	}
}
