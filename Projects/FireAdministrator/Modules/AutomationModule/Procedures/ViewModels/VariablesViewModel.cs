using System.Linq;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.Automation;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using System.Collections.ObjectModel;
using Infrastructure;
using Infrastructure.ViewModels;
using System.Windows.Input;
using KeyboardKey = System.Windows.Input.Key;
using Infrustructure.Plans.Events;
using System;
using Infrastructure.Automation;

namespace AutomationModule.ViewModels
{
	public class VariablesViewModel : MenuViewPartViewModel, ISelectable<Guid>
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
			var variableDetailsViewModel = new VariableDetailsViewModel(null, AutomationHelper.GetLocalVariables(Procedure), "Добавить локальную переменную");
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
			var variableDetailsViewModel = new VariableDetailsViewModel(SelectedVariable.Variable, AutomationHelper.GetLocalVariables(Procedure), "Редактировать локальную переменную");
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