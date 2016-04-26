using AutomationModule.Properties;
using FiresecAPI.Automation;
using FiresecAPI.Models.Automation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using System.Collections.ObjectModel;
using System.Linq;

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
			var variableDetailsViewModel = new VariableDetailsViewModel(null, "аргумент", "Добавить аргумент");

			if (!DialogService.ShowModalWindow(variableDetailsViewModel)) return;

			if (IsExist(variableDetailsViewModel.Variable))
			{
				MessageBoxService.ShowError(Resources.ArgumentExistError);
				return;
			}

			var variableViewModel = new VariableViewModel(variableDetailsViewModel.Variable);
			Procedure.Arguments.Add(variableViewModel.Variable);
			Variables.Add(variableViewModel);
			SelectedVariable = variableViewModel;
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		private bool IsExist(IVariable variable)
		{
			return Variables.Any(x => string.Equals(x.Variable.Name, variable.Name));
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
			var variableDetailsViewModel = new VariableDetailsViewModel(SelectedVariable.Variable, "аргумент", "Редактировать аргумент");
			if (DialogService.ShowModalWindow(variableDetailsViewModel))
			{
				PropertyCopy.Copy(variableDetailsViewModel.Variable, SelectedVariable.Variable);
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