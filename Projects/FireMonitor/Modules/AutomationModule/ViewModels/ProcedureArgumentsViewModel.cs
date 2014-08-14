using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
using Infrastructure.Common.Windows;
using FiresecClient;
using Infrastructure.Common;

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
			List<Argument> args = null;
			if (procedure.Arguments.Count > 0)
			{
				var viewModel = new ProcedureArgumentsViewModel(procedure);
				if (DialogService.ShowModalWindow(viewModel))
					args = viewModel.ArgumentViewModels.Arguments.Select(x => x.Argument).ToList();
			}
			else
				args = new List<Argument>();
			if (args != null)
				using (new WaitWrapper())
					FiresecManager.FiresecService.RunProcedure(procedure.Uid, args);
		}
	}
}
