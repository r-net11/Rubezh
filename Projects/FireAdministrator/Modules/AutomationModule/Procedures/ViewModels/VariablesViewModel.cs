using StrazhAPI.Automation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Localization.Automation.Errors;
using Localization.Automation.ViewModels;

namespace AutomationModule.ViewModels
{
	public class VariablesViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public Procedure Procedure { get; private set; }

		public VariablesViewModel(Procedure procedure)
		{
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			Menu = new VariablesMenuViewModel(this);
			Procedure = procedure;

			Variables = new ObservableCollection<VariableViewModel>();
			foreach (var variable in procedure.Variables)
			{
				var variableViewModel = new VariableViewModel(variable);
				Variables.Add(variableViewModel);
			}
		}

		VariableViewModel _selectedVariable;
		public VariableViewModel SelectedVariable
		{
			get { return _selectedVariable; }
			set
			{
				_selectedVariable = value;
				OnPropertyChanged(() => SelectedVariable);
			}
		}

		ObservableCollection<VariableViewModel> _variables;
		public ObservableCollection<VariableViewModel> Variables
		{
			get { return _variables; }
			set
			{
				_variables = value;
				OnPropertyChanged(() => Variables);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var variableDetailsViewModel = new VariableDetailsViewModel(null, CommonViewModel.LocalVariable_DefaultName, CommonViewModel.LocalVariable_Add);

			if (!DialogService.ShowModalWindow(variableDetailsViewModel)) return;

			if (IsExist(variableDetailsViewModel.Variable))
			{
				MessageBoxService.ShowError(CommonErrors.VariableExist_Error);
				return;
			}

			var varialbeViewModel = new VariableViewModel(variableDetailsViewModel.Variable);
			Procedure.Variables.Add(varialbeViewModel.Variable);
			Variables.Add(varialbeViewModel);
			SelectedVariable = varialbeViewModel;
			SelectedVariable.Update();
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		private bool IsExist(Variable variable)
		{
			return Variables.Any(x => string.Equals(x.Variable.Name, variable.Name));
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			Procedure.Variables.Remove(SelectedVariable.Variable);
			Variables.Remove(SelectedVariable);
			SelectedVariable = Variables.FirstOrDefault();
			ServiceFactory.SaveService.AutomationChanged = true;
		}
		bool CanDelete()
		{
			return SelectedVariable != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var variableDetailsViewModel = new VariableDetailsViewModel(SelectedVariable.Variable, CommonViewModel.LocalVariable_DefaultName, CommonViewModel.LocalVariable_Edit);
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

		public void Select(Guid variableUid)
		{
			if (variableUid != Guid.Empty)
			{
				SelectedVariable = Variables.FirstOrDefault(item => item.Variable.Uid == variableUid);
			}
		}
	}
}