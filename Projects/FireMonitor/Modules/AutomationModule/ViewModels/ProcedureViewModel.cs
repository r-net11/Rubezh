using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.Automation;
using System.Linq;

namespace AutomationModule.ViewModels
{
	public class ProcedureViewModel : BaseViewModel
	{
		public Procedure Procedure { get; private set; }
		public ArgumentsViewModel ArgumentsViewModel { get; private set; }

		public ProcedureViewModel(Procedure procedure)
		{
			Procedure = procedure;
			ArgumentsViewModel = new ArgumentsViewModel(procedure);
			RunCommand = new RelayCommand(OnRun);
		}

		public RelayCommand RunCommand { get; private set; }
		void OnRun()
		{
			//ProcedureHelper.Run(Procedure, ArgumentsViewModel.Arguments.Select(x => x.Argument).ToList(), RubezhClient.ClientManager.CurrentUser);
		}

		public string Name
		{
			get { return Procedure.Name; }
		}
	}
}
