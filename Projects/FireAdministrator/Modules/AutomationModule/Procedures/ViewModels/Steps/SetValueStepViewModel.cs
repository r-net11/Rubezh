using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;

namespace AutomationModule.ViewModels
{
	public class SetValueStepViewModel: BaseViewModel, IStepViewModel
	{
		SetValueArguments SetValueArguments { get; set; }
		public ArithmeticParameterViewModel Variable1;
		public ArithmeticParameterViewModel Result;
		Procedure Procedure { get; set; }

		public SetValueStepViewModel(SetValueArguments setValueArguments, Procedure procedure)
		{
			SetValueArguments = setValueArguments;
			Procedure = procedure;
			Variable1 = new ArithmeticParameterViewModel(SetValueArguments.Variable1, new List<VariableType> { VariableType.IsGlobalVariable, VariableType.IsLocalVariable, VariableType.IsValue });
			Result = new ArithmeticParameterViewModel(SetValueArguments.Variable1, new List<VariableType> { VariableType.IsGlobalVariable, VariableType.IsLocalVariable });
			UpdateContent();
		}

		public void UpdateContent()
		{
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => !x.IsList));
			Result.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => !x.IsList));
		}

		public string Description { get { return ""; } }
	}
}
