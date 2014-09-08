using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using System;
using ValueType = FiresecAPI.Automation.ValueType;

namespace AutomationModule.ViewModels
{
	public class PersonInspectionStepViewModel : BaseStepViewModel
	{
		public ArithmeticParameterViewModel CardNumber { get; set; }
		public Procedure Procedure { get; private set; }

		public PersonInspectionStepViewModel(PersonInspectionArguments personInspectionArguments, Procedure procedure, Action updateDescriptionHandler)
			: base(updateDescriptionHandler)
		{
			Procedure = procedure;
			CardNumber = new ArithmeticParameterViewModel(personInspectionArguments.CardNumber);
			UpdateContent();
		}

		public new void UpdateContent()
		{
			CardNumber.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ValueType == ValueType.Integer && !x.IsList));
		}

		public override string Description 
		{ 
			get { return ""; } 
		}
	}
}