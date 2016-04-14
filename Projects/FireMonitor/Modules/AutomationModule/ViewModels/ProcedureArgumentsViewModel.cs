using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Automation;
using RubezhClient;
using System.Collections.Generic;
using System.Linq;

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
				if (DialogService.ShowModalWindow(viewModel)) ;
				args = viewModel.ArgumentViewModels.Arguments.Select(x =>
					new Argument
					{
						VariableScope = VariableScope.ExplicitValue,
						Value = x.Argument.Value,
						ExplicitType = x.Argument.ExplicitType,
						EnumType = x.Argument.EnumType,
						ObjectType = x.Argument.ObjectType,
						IsList = x.Argument.IsList
					}).ToList();
			}
			ProcedureHelper.Run(procedure, args, ClientManager.CurrentUser);
		}
	}
}
