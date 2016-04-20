using System.Linq;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.Automation;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using System.Collections.ObjectModel;
using Infrastructure;
using Infrastructure.Automation;

namespace AutomationModule.ViewModels
{
	public class ArgumentsViewModel : VariablesViewModel
	{
		public ArgumentsViewModel(Procedure procedure) : base(procedure)
		{
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			Variables = new ObservableCollection<VariableViewModel>();
			foreach (var variable in procedure.Arguments)
			{
				var argumentViewModel = new VariableViewModel(variable);
				Variables.Add(argumentViewModel);
			}
		}
		
		public new RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var variableDetailsViewModel = new VariableDetailsViewModel(null, AutomationHelper.GetLocalVariables(Procedure), "Добавить аргумент");
			if (DialogService.ShowModalWindow(variableDetailsViewModel))
			{
				var variableViewModel = new VariableViewModel(variableDetailsViewModel.Variable);
				Procedure.Arguments.Add(variableViewModel.Variable);
				Variables.Add(variableViewModel);
				SelectedVariable = variableViewModel;
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public new RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			Procedure.Arguments.Remove(SelectedVariable.Variable);
			Variables.Remove(SelectedVariable);
			SelectedVariable = Variables.FirstOrDefault();
			ServiceFactory.SaveService.AutomationChanged = true;
		}
		bool CanDelete()
		{
			return SelectedVariable != null;
		}

		public new RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var variableDetailsViewModel = new VariableDetailsViewModel(SelectedVariable.Variable, AutomationHelper.GetLocalVariables(Procedure), "Редактировать аргумент");
			if (DialogService.ShowModalWindow(variableDetailsViewModel))
			{
				PropertyCopy.Copy<Variable, Variable>(variableDetailsViewModel.Variable, SelectedVariable.Variable);
				SelectedVariable.Update();
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}
		bool CanEdit()
		{
			return SelectedVariable != null;
		}
	}
}