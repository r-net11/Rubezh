using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using System;

namespace AutomationModule.ViewModels
{
	public class PersonInspectionStepViewModel : BaseStepViewModel
	{
		public ArithmeticParameterViewModel CardNumberParameter { get; set; }

		public PersonInspectionStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			CardNumberParameter = new ArithmeticParameterViewModel(stepViewModel.Step.PersonInspectionArguments.CardNumberParameter, stepViewModel.Update);
			CardNumberParameter.ExplicitType = ExplicitType.Integer;
			UpdateContent();
		}

		public new void UpdateContent()
		{
			CardNumberParameter.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.Integer && !x.IsList));
		}

		public override string Description 
		{ 
			get 
			{ 
				return "Номер карты: " + CardNumberParameter.Description; 
			} 
		}
	}
}