using RubezhAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ProcedureTooltipViewModel : BaseViewModel
	{
		public Procedure Procedure { get; private set; }

		public ProcedureTooltipViewModel(Procedure procedure)
		{
			Procedure = procedure;
		}

		public void Update()
		{
			OnPropertyChanged(() => Procedure);
		}
	}
}
