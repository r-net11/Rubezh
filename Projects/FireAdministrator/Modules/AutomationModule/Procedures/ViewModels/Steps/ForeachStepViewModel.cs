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
			ListVariable.UpdateVariableHandler += UpdateItemVariable;
			ItemVariable = new ArithmeticParameterViewModel(ForeachArguments.ItemVariable, false);
			UpdateContent();
		}

		public override void UpdateContent()
		{
			ListVariable.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.IsList));
		}

		void UpdateItemVariable()
		{
			ItemVariable.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => !x.IsList && x.ValueType == ListVariable.SelectedVariable.ValueType
				&& x.ObjectType == ListVariable.ObjectType && x.EnumType == ListVariable.EnumType ));
		}

		public override string Description
		{
			get { return ""; }
		}

	}
}
