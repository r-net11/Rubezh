using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class RunProgramStepViewModel : BaseStepViewModel
	{
		RunProgramStep RunProgramStep { get; set; }
		public ArgumentViewModel PathArgument { get; private set; }
		public ArgumentViewModel ParametersArgument { get; private set; }

		public RunProgramStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{

			RunProgramStep = (RunProgramStep)stepViewModel.Step;
			PathArgument = new ArgumentViewModel(RunProgramStep.PathArgument, stepViewModel.Update, UpdateContent);
			ParametersArgument = new ArgumentViewModel(RunProgramStep.ParametersArgument, stepViewModel.Update, UpdateContent);
		}

		public override void UpdateContent()
		{
			PathArgument.Update(Procedure, ExplicitType.String, isList: false);
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