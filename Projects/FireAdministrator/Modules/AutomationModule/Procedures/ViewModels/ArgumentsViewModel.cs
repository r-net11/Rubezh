using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using System.Collections.ObjectModel;
using Infrastructure;

namespace AutomationModule.ViewModels
{
	public class ArgumentsViewModel : BaseViewModel
	{
		public Procedure Procedure { get; private set; }

		public ArgumentsViewModel(Procedure procedure)
		{
			Procedure = procedure;
			Variables = new ObservableCollection<VariableViewModel>();
			foreach (var variable in procedure.Arguments)
			{
				var argumentViewModel = new VariableViewModel(variable);
				Variables.Add(argumentViewModel);
			}
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
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
			var variableDetailsViewModel = new VariableDetailsViewModel(null, "аргумент", "Добавить аргумент"); ;
			if (DialogService.ShowModalWindow(variableDetailsViewModel))
			{
				var varialbeViewModel = new VariableViewModel(variableDetailsViewModel.Variable);
				Procedure.Arguments.Add(varialbeViewModel.Variable);
				Variables.Add(varialbeViewModel);
				SelectedVariable = varialbeViewModel;
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
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

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var variableDetailsViewModel = new VariableDetailsViewModel(SelectedVariable.Variable, "аргумент", "Редактировать аргумент"); ;
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