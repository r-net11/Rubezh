using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;

namespace AutomationModule.ViewModels
{
	public class PersonInspectionStepViewModel : BaseViewModel, IStepViewModel
	{
		public ArithmeticParameterViewModel CardNumber { get; set; }
		public Procedure Procedure { get; private set; }

		public PersonInspectionStepViewModel(PersonInspectionArguments personInspectionArguments, Procedure procedure)
		{
			Procedure = procedure;
			CardNumber = new ArithmeticParameterViewModel(personInspectionArguments.CardNumber);
			UpdateContent();
		}

		public void UpdateContent()
		{
			CardNumber.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ValueType == ValueType.Integer && !x.IsList));
		}

		public string Description { get { return ""; } }
	}
}