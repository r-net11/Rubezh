using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
using System;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class RunProgrammStepViewModel : BaseStepViewModel
	{
		RunProgrammArguments RunProgrammArguments { get; set; }
		public ArithmeticParameterViewModel Path { get; private set; }
		public ArithmeticParameterViewModel Parameters { get; private set; }

		public RunProgrammStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			RunProgrammArguments = stepViewModel.Step.RunProgrammArguments;
			Path = new ArithmeticParameterViewModel(RunProgrammArguments.Path, stepViewModel.Update);
			Path.ExplicitType = ExplicitType.String;
			Parameters = new ArithmeticParameterViewModel(RunProgrammArguments.Parameters, stepViewModel.Update);
			Parameters.ExplicitType = ExplicitType.String;
			UpdateContent();
		}

		public override void UpdateContent()
		{
			Path.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.String && !x.IsList));
			Parameters.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.String && !x.IsList));
		}

		public override string Description
		{
			get
			{
				return "Путь к программе: " + Path.Description + " Параметры запуска: " + Parameters.Description;
			}
		}
	}
}
