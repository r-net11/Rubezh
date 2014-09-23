using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
using System;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class RunProgrammStepViewModel : BaseStepViewModel
	{
		RunProgrammArguments RunProgrammArguments { get; set; }
		public ArgumentViewModel PathParameter { get; private set; }
		public ArgumentViewModel ParametersParameter { get; private set; }

		public RunProgrammStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			RunProgrammArguments = stepViewModel.Step.RunProgrammArguments;
			PathParameter = new ArgumentViewModel(RunProgrammArguments.PathParameter, stepViewModel.Update);
			PathParameter.ExplicitType = ExplicitType.String;
			ParametersParameter = new ArgumentViewModel(RunProgrammArguments.ParametersParameter, stepViewModel.Update);
			ParametersParameter.ExplicitType = ExplicitType.String;
			UpdateContent();
		}

		public override void UpdateContent()
		{
			PathParameter.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.String && !x.IsList));
			ParametersParameter.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.String && !x.IsList));
		}

		public override string Description
		{
			get
			{
				return "Путь к программе: " + PathParameter.Description + " Параметры запуска: " + ParametersParameter.Description;
			}
		}
	}
}
