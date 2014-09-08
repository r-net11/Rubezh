using System.Linq;
using FiresecAPI.Automation;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using System;
using ValueType = FiresecAPI.Automation.ValueType;

namespace AutomationModule.ViewModels
{
	public class ForeachStepViewModel : BaseStepViewModel
	{
		public ForeachArguments ForeachArguments { get; private set; }
		Procedure Procedure { get; set; }
		public ArithmeticParameterViewModel ListVariable { get; private set; }
		public ArithmeticParameterViewModel ItemVariable { get; private set; }

		public ForeachStepViewModel(ForeachArguments foreachArguments, Procedure procedure, Action updateDescriptionHandler)
			: base(updateDescriptionHandler)
		{
			ForeachArguments = foreachArguments;
			Procedure = procedure;
			ListVariable = new ArithmeticParameterViewModel(ForeachArguments.ListVariable, false);
			ItemVariable = new ArithmeticParameterViewModel(ForeachArguments.ItemVariable, false);
			UpdateContent();
		}

		public void UpdateContent()
		{
			ListVariable.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ValueType == ValueType.Object && x.IsList));
			ItemVariable.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ValueType == ValueType.Object && !x.IsList));
		}

		public override string Description
		{
			get { return ""; }
		}

	}
}
