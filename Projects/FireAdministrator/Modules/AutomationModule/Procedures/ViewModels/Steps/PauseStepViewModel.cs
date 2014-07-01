using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class PauseStepViewModel : BaseViewModel, IStepViewModel
	{
		private Procedure Procedure { get; set; }
		public PauseArguments PauseArguments { get; private set; }
		public ArithmeticParameterViewModel Variable { get; set; }

		public PauseStepViewModel(PauseArguments pauseArguments, Procedure procedure)
		{
			PauseArguments = pauseArguments;
			Procedure = procedure;
			Variable = new ArithmeticParameterViewModel(PauseArguments.Variable, Procedure.Variables);
			UpdateContent();
		}

		public void UpdateContent()
		{
			Variable.Update(Procedure.Variables);
		}

		public string Description
		{
			get { return ""; }
		}
	}
}