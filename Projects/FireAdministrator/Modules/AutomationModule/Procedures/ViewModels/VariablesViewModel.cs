using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;

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
			var variableDetailsViewModel = new VariableDetailsViewModel(null, "локальная переменная", "Добавить локальную переменную");
			if (DialogService.ShowModalWindow(variableDetailsViewModel))
			{
				var varialbeViewModel = new VariableViewModel(variableDetailsViewModel.Variable);
				Procedure.Variables.Add(varialbeViewModel.Variable);
				Variables.Add(varialbeViewModel);
				SelectedVariable = varialbeViewModel;
				SelectedVariable.Update();
				ServiceFactory.SaveService.AutomationChanged = true;
			}
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
			var variableDetailsViewModel = new VariableDetailsViewModel(SelectedVariable.Variable, "локальная переменная", "Редактировать локальную переменную");
			if (DialogService.ShowModalWindow(variableDetailsViewModel))
			{
				PropertyCopy.Copy(variableDetailsViewModel.Variable, SelectedVariable.Variable);
				//PropertyCopy.Copy<ExplicitValue, ExplicitValue>(variableDetailsViewModel.Variable.DefaultExplicitValue, SelectedVariable.Variable.DefaultExplicitValue);
				//SelectedVariable.Variable.ExplicitValues = new List<ExplicitValue>(SelectedVariable.Variable.DefaultExplicitValues);
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