using System;
using System.Collections.ObjectModel;
using System.Linq;
using AutomationModule.Properties;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using FiresecClient;
using System.Windows.Input;
using StrazhAPI.Automation;
using KeyboardKey = System.Windows.Input.Key;
using Localization.Automation.Errors;

namespace AutomationModule.ViewModels
{
	public class GlobalVariablesViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public static GlobalVariablesViewModel Current { get; private set; }
		public GlobalVariablesViewModel()
		{
			Current = this;
			Menu = new GlobalVariablesMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, () => SelectedGlobalVariable != null);
			EditCommand = new RelayCommand(OnEdit, () => SelectedGlobalVariable != null);
			RegisterShortcuts();
		}

		public void Initialize()
		{
			GlobalVariables = new ObservableCollection<VariableViewModel>();
			foreach (var globalVariable in FiresecManager.SystemConfiguration.AutomationConfiguration.GlobalVariables)
			{
				var globalVariableViewModel = new VariableViewModel(globalVariable);
				GlobalVariables.Add(globalVariableViewModel);
			}
			SelectedGlobalVariable = GlobalVariables.FirstOrDefault();
		}

		ObservableCollection<VariableViewModel> _globalVariables;
		public ObservableCollection<VariableViewModel> GlobalVariables
		{
			get { return _globalVariables; }
			set
			{
				_globalVariables = value;
				OnPropertyChanged(() => GlobalVariables);
			}
		}

		VariableViewModel _selectedGlobalVariable;
		public VariableViewModel SelectedGlobalVariable
		{
			get { return _selectedGlobalVariable; }
			set
			{
				_selectedGlobalVariable = value;
				OnPropertyChanged(() => SelectedGlobalVariable);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var globalVariableDetailsViewModel = new VariableDetailsViewModel(null, "глобальная переменная", "Добавить глобальную переменную");

			if (!DialogService.ShowModalWindow(globalVariableDetailsViewModel)) return;

			if (IsExist(globalVariableDetailsViewModel.Variable))
			{
				MessageBoxService.ShowError(CommonErrors.VariableExist_Error);
				return;
			}

			globalVariableDetailsViewModel.Variable.IsGlobal = true;
			FiresecManager.SystemConfiguration.AutomationConfiguration.GlobalVariables.Add(globalVariableDetailsViewModel.Variable);
			var globalVariableViewModel = new VariableViewModel(globalVariableDetailsViewModel.Variable);
			GlobalVariables.Add(globalVariableViewModel);
			SelectedGlobalVariable = globalVariableViewModel;
			SelectedGlobalVariable.Update();
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		private bool IsExist(Variable variable)
		{
			return GlobalVariables.Any(x => string.Equals(x.Variable.Name, variable.Name));
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var index = GlobalVariables.IndexOf(SelectedGlobalVariable);
			FiresecManager.SystemConfiguration.AutomationConfiguration.GlobalVariables.Remove(SelectedGlobalVariable.Variable);
			GlobalVariables.Remove(SelectedGlobalVariable);
			index = Math.Min(index, GlobalVariables.Count - 1);
			if (index > -1)
				SelectedGlobalVariable = GlobalVariables[index];
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var globalVariableDetailsViewModel = new VariableDetailsViewModel(SelectedGlobalVariable.Variable, "глобальная переменная", "Редактировать глобальную переменную");
			if (DialogService.ShowModalWindow(globalVariableDetailsViewModel))
			{
				globalVariableDetailsViewModel.Variable.IsGlobal = true;
				PropertyCopy.Copy(globalVariableDetailsViewModel.Variable, SelectedGlobalVariable.Variable);
				SelectedGlobalVariable.Update();
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public void Select(Guid globalVariableUid)
		{
			if (globalVariableUid != Guid.Empty)
				SelectedGlobalVariable = GlobalVariables.FirstOrDefault(item => item.Variable.Uid == globalVariableUid);
		}

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
		}
	}
}