using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using System;

namespace AutomationModule.ViewModels
{
	public class PersonInspectionStepViewModel : BaseStepViewModel
	{
		public ArithmeticParameterViewModel CardNumber { get; set; }

		public PersonInspectionStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			CardNumber = new ArithmeticParameterViewModel(stepViewModel.Step.PersonInspectionArguments.CardNumber, stepViewModel.Update);
			CardNumber.ExplicitType = ExplicitType.Integer;
			UpdateContent();
		}

		public new void UpdateContent()
		{
			CardNumber.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.Integer && !x.IsList));
		}

		public override string Description 
		{ 
			get 
			{ 
				return "Номер карты: " + CardNumber.Description; 
			} 
		}
	}
}