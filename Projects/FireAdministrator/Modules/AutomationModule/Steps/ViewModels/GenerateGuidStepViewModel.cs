using StrazhAPI.Automation;
using Localization.Automation.ViewModels;

namespace AutomationModule.ViewModels
{
	public class GenerateGuidStepViewModel : BaseStepViewModel
	{
		public ArgumentViewModel ResultArgument { get; private set; }
		public GenerateGuidArguments GenerateGuidArguments { get; private set; }

		public GenerateGuidStepViewModel(StepViewModel stepViewModel): base(stepViewModel)
		{
			GenerateGuidArguments = stepViewModel.Step.GenerateGuidArguments;
			ResultArgument = new ArgumentViewModel(GenerateGuidArguments.ResultArgument, stepViewModel.Update, UpdateContent, false);
		}

		public override void UpdateContent()
		{
			ResultArgument.Update(Procedure, ExplicitType.String);
		}

		public override string Description
		{
			get
			{
                return string.Format(StepCommonViewModel.GenerateGuid, ResultArgument.Description);
			}
		}
	}
}
