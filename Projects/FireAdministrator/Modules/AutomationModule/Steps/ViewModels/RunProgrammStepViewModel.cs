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
			PathArgument = new ArgumentViewModel(RunProgrammArguments.PathArgument, stepViewModel.Update);
			PathArgument.ExplicitType = ExplicitType.String;
			ParametersArgument = new ArgumentViewModel(RunProgrammArguments.ParametersArgument, stepViewModel.Update);
			ParametersArgument.ExplicitType = ExplicitType.String;
		}

		public override void UpdateContent()
		{
			PathArgument.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.String && !x.IsList));
			ParametersArgument.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.String && !x.IsList));
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
