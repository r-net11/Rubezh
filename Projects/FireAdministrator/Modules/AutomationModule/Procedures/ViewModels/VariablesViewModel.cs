using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using System.Collections.ObjectModel;
using Infrastructure;
using Infrastructure.ViewModels;
using System.Windows.Input;
using KeyboardKey = System.Windows.Input.Key;
using Infrustructure.Plans.Events;
using System;

namespace AutomationModule.ViewModels
{
	public class VariablesViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public Procedure Procedure { get; private set; }

		public VariablesViewModel(Procedure procedure)
		{
			Procedure = procedure;
			Variables = new ObservableCollection<VariableViewModel>();
			Menu = new VariablesMenuViewModel(this);
			foreach (var variable in procedure.Variables)
			{
				var variableViewModel = new VariableViewModel(variable);
				Variables.Add(variableViewModel);
			}
			RegisterShortcuts();
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
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
				PropertyCopy.Copy<Variable, Variable>(variableDetailsViewModel.Variable, SelectedVariable.Variable);
				SelectedVariable.Update();
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}
		bool CanEdit()
		{
			return SelectedVariable != null;
		}

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
		}

		public void Select(Guid variableUid)
		{
			if (variableUid != Guid.Empty)
			{
				SelectedVariable = Variables.FirstOrDefault(item => item.Variable.Uid == variableUid);
			}
		}

		//private void SubscribeEvents()
		//{
		//    ServiceFactory.Events.GetEvent<ElementAddedEvent>().Unsubscribe(OnElementChanged);
		//    ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(OnElementChanged);
		//    ServiceFactory.Events.GetEvent<ElementChangedEvent>().Unsubscribe(OnElementChanged);
		//    ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Unsubscribe(OnElementSelected);

		//    ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementChanged);
		//    ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementChanged);
		//    ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
		//    ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
		//}
	}
}