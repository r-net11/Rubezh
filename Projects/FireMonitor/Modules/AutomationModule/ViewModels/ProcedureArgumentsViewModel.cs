using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Automation;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ProcedureArgumentsViewModel : SaveCancelDialogViewModel
	{
		public ProcedureArgumentsViewModel(Procedure procedure)
		{
			Title = procedure.Name;
			SaveCaption = "Выполнить";
			ArgumentViewModels = new ArgumentsViewModel(procedure);
		}

		public ArgumentsViewModel ArgumentViewModels { get; private set; }

		public static void Run(Procedure procedure)
		{
			List<Variable> args = null;
			if (procedure.Arguments.Count > 0)
			{
				var viewModel = new ProcedureArgumentsViewModel(procedure);
				if (DialogService.ShowModalWindow(viewModel))
					args = viewModel.ArgumentViewModels.Arguments.Select(x => x.Variable).ToList();
			}
			else
				args = new List<Variable>();
			if (args != null)
				using (new WaitWrapper())
					FiresecManager.FiresecService.RunProcedure(procedure.Uid, args);
		}
	}
}
