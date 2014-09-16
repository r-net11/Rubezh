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
		public ArithmeticParameterViewModel ListParameter { get; private set; }
		public ArithmeticParameterViewModel ItemParameter { get; private set; }

		public ForeachStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ForeachArguments = stepViewModel.Step.ForeachArguments;
			ListParameter = new ArithmeticParameterViewModel(ForeachArguments.ListParameter, stepViewModel.Update, false);
			ListParameter.UpdateVariableHandler += UpdateItemVariable;
			ItemParameter = new ArithmeticParameterViewModel(ForeachArguments.ItemParameter, stepViewModel.Update, false);
			UpdateContent();
		}

		public override void UpdateContent()
		{
			ListParameter.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.IsList));
		}

		void UpdateItemVariable()
		{
			ItemParameter.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => !x.IsList && x.ExplicitType == ListParameter.SelectedVariable.ExplicitType
				&& x.ObjectType == ListParameter.SelectedVariable.ObjectType && x.EnumType == ListParameter.SelectedVariable.EnumType ));
		}

		public override string Description
		{
			get { return "Список: " + ListParameter.Description + " Элемент: " + ItemParameter.Description; }
		}

	}
}
