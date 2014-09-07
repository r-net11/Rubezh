using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using ValueType = FiresecAPI.Automation.ValueType;

namespace AutomationModule.ViewModels
{
	public class ExitStepViewModel: BaseViewModel, IStepViewModel
	{
		Procedure Procedure { get; set; }
		ExitArguments ExitArguments { get; set; }
		public ArithmeticParameterViewModel ExitCode { get; private set; }

		public ExitStepViewModel(ExitArguments exitArguments, Procedure procedure)
		{
			ExitArguments = exitArguments;
			Procedure = procedure;
			ExitCode = new ArithmeticParameterViewModel(exitArguments.ExitCode);
			UpdateContent();
		}

		public void UpdateContent()
		{
			var allVariables = ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ValueType == ValueType.Integer);
			ExitCode.Update(allVariables);
		}

		public string Description
		{
			get { return ""; }
		}
	}
}
