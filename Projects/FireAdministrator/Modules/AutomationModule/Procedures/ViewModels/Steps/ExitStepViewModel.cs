using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ExitStepViewModel: BaseStepViewModel
	{
		ExitArguments ExitArguments { get; set; }
		public ArithmeticParameterViewModel ExitCode { get; private set; }

		public ExitStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ExitArguments = stepViewModel.Step.ExitArguments;
			ExitCode = new ArithmeticParameterViewModel(ExitArguments.ExitCode, stepViewModel.Update);
			ExitCode.ExplicitType = ExplicitType.Integer;
			UpdateContent();
		}

		public override void UpdateContent()
		{
			var allVariables = ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.Integer && !x.IsList);
			ExitCode.Update(allVariables);
		}

		public override string Description
		{
			get 
			{ 
				return "Код выхода: " + ExitCode.Description; 
			}
		}
	}
}
