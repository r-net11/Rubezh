using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;

namespace AutomationModule.ViewModels
{
	public class PauseStepViewModel : BaseViewModel, IStepViewModel
	{
		private Procedure Procedure { get; set; }
		public PauseArguments PauseArguments { get; private set; }
		public ArithmeticParameterViewModel Variable { get; set; }

		public PauseStepViewModel(PauseArguments pauseArguments, Procedure procedure)
		{
			PauseArguments = pauseArguments;
			Procedure = procedure;
			var variableTypes = new List<VariableType> { VariableType.IsGlobalVariable, VariableType.IsLocalVariable, VariableType.IsValue };
			Variable = new ArithmeticParameterViewModel(PauseArguments.Variable, variableTypes);
			UpdateContent();
		}

		public void UpdateContent()
		{
			Variable.Update(Procedure.Variables);
		}

		public string Description
		{
			get { return ""; }
		}
	}
}