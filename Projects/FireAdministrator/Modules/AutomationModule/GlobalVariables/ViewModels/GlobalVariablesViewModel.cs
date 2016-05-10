using System.Windows;
using StrazhAPI.Automation;
using StrazhAPI.Models.Automation;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using KeyboardKey = System.Windows.Input.Key;
using Localization.Automation.Errors;

namespace AutomationModule.ViewModels
{
	public class GlobalVariablesViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		#region Properties
		public static GlobalVariablesViewModel Current { get; private set; }

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
		#endregion

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
			var globalVariables = FiresecManager.FiresecService.GetInitialGlobalVariables();
			foreach (var globalVariable in globalVariables.Result)
			{
				var globalVariableViewModel = new VariableViewModel(globalVariable);
				GlobalVariables.Add(globalVariableViewModel);
			}
			SelectedGlobalVariable = GlobalVariables.FirstOrDefault();
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var globalVariableDetailsViewModel = new AddGlobalVariableDialogViewModel(null);

			if (!DialogService.ShowModalWindow(globalVariableDetailsViewModel)) return;

			if (IsExist(globalVariableDetailsViewModel.Variable))
			{
				MessageBoxService.ShowError(CommonErrors.VariableExist_Error);
				return;
			}

			var saveResult = FiresecManager.FiresecService.SaveGlobalVariable(globalVariableDetailsViewModel.Variable);
			if (saveResult.Result)
			{
				var globalVariableViewModel = new VariableViewModel(globalVariableDetailsViewModel.Variable);
				GlobalVariables.Add(globalVariableViewModel);
				SelectedGlobalVariable = globalVariableViewModel;
				SelectedGlobalVariable.Update();
			}
		}

		private bool IsExist(IVariable variable)
		{
			return GlobalVariables.Any(x => string.Equals(x.Variable.Name, variable.Name));
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var index = GlobalVariables.IndexOf(SelectedGlobalVariable);
			FiresecManager.FiresecService.RemoveGlobalVariable(SelectedGlobalVariable.Variable as GlobalVariable);
			GlobalVariables.Remove(SelectedGlobalVariable);
			index = Math.Min(index, GlobalVariables.Count - 1);
			if (index > -1)
				SelectedGlobalVariable = GlobalVariables[index];
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var globalVariableDetailsViewModel = new AddGlobalVariableDialogViewModel(SelectedGlobalVariable.Variable as GlobalVariable);

			if (DialogService.ShowModalWindow(globalVariableDetailsViewModel))
			{
				var saveResult = FiresecManager.FiresecService.SaveGlobalVariable(globalVariableDetailsViewModel.Variable);
				if (saveResult.Result)
				{
					PropertyCopy.Copy(globalVariableDetailsViewModel.Variable, SelectedGlobalVariable.Variable);
					SelectedGlobalVariable.Update();
				}
			}
		}

		public void Select(Guid globalVariableUid)
		{
			if (globalVariableUid != Guid.Empty)
				SelectedGlobalVariable = GlobalVariables.FirstOrDefault(item => item.Variable.UID == globalVariableUid);
		}

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
		}
	}
}