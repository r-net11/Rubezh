using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class RunProgrammStepViewModel : BaseStepViewModel
	{
		RunProgrammArguments RunProgrammArguments { get; set; }
		public ArgumentViewModel PathArgument { get; private set; }
		public ArgumentViewModel ParametersArgument { get; private set; }

		public RunProgrammStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			RunProgrammArguments = stepViewModel.Step.RunProgrammArguments;
			PathArgument = new ArgumentViewModel(RunProgrammArguments.PathArgument, stepViewModel.Update, UpdateContent);
			ParametersArgument = new ArgumentViewModel(RunProgrammArguments.ParametersArgument, stepViewModel.Update, UpdateContent);
		}

		public override void UpdateContent()
		{
			PathArgument.Update(Procedure, ExplicitType.String, isList:false);
			ParametersArgument.Update(Procedure, ExplicitType.String, isList: false);
		}

		public override string Description
		{
			get
			{
				return "Путь к программе: " + PathArgument.Description + " Параметры запуска: " + ParametersArgument.Description;
			}
		}
	}
}
