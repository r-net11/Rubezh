using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class PersonInspectionStepViewModel: BaseViewModel, IStepViewModel
	{
		public ArithmeticParameterViewModel CardNumber { get; set; }
		public Procedure Procedure { get; private set; }

		public PersonInspectionStepViewModel(PersonInspectionArguments personInspectionArguments, Procedure procedure)
		{
			Procedure = procedure;
			CardNumber = new ArithmeticParameterViewModel(personInspectionArguments.CardNumber, procedure.Variables);
		}

		public void UpdateContent()
		{
			CardNumber.Update(Procedure.Variables);
		}

		public string Description { get { return ""; } }
	}
}
