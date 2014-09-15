using System.Linq;
using FiresecAPI.Automation;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using System;

namespace AutomationModule.ViewModels
{
	public class ForeachStepViewModel : BaseStepViewModel
	{
		public ForeachArguments ForeachArguments { get; private set; }
		public ArithmeticParameterViewModel ListVariable { get; private set; }
		public ArithmeticParameterViewModel ItemVariable { get; private set; }

		public ForeachStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ForeachArguments = stepViewModel.Step.ForeachArguments;
			ListVariable = new ArithmeticParameterViewModel(ForeachArguments.ListVariable, stepViewModel.Update, false);
			ListVariable.UpdateVariableHandler += UpdateItemVariable;
			ItemVariable = new ArithmeticParameterViewModel(ForeachArguments.ItemVariable, stepViewModel.Update, false);
			UpdateContent();
		}

		public override void UpdateContent()
		{
			ListVariable.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.IsList));
		}

		void UpdateItemVariable()
		{
			ItemVariable.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => !x.IsList && x.ExplicitType == ListVariable.SelectedVariable.ExplicitType
				&& x.ObjectType == ListVariable.SelectedVariable.ObjectType && x.EnumType == ListVariable.SelectedVariable.EnumType ));
		}

		public override string Description
		{
			get { return ""; }
		}

	}
}
