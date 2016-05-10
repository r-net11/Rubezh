using FiresecAPI.Automation;
using Localization.Automation.ViewModels;

namespace AutomationModule.ViewModels
{
	public class RunProgramStepViewModel : BaseStepViewModel
	{
		RunProgramArguments RunProgramArguments { get; set; }
		public ArgumentViewModel PathArgument { get; private set; }
		public ArgumentViewModel ParametersArgument { get; private set; }

		public RunProgramStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			RunProgramArguments = stepViewModel.Step.RunProgramArguments;
			PathArgument = new ArgumentViewModel(RunProgramArguments.PathArgument, stepViewModel.Update, UpdateContent);
			ParametersArgument = new ArgumentViewModel(RunProgramArguments.ParametersArgument, stepViewModel.Update, UpdateContent);
		}

		public override void UpdateContent()
		{
			PathArgument.Update(Procedure, ExplicitType.String);
			ParametersArgument.Update(Procedure, ExplicitType.String);
		}

		public override string Description
		{
			get
			{
				return string.Format(StepCommonViewModel.RunProgram, PathArgument.Description,ParametersArgument.Description);
			}
		}
	}
}