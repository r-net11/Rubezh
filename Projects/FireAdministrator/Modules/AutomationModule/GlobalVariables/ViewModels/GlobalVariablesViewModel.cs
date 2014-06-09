using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;

namespace AutomationModule.ViewModels
{
	public class GlobalVariablesViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public GlobalVariablesViewModel()
		{
			Menu = new GlobalVariablesMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
		}

		public void Initialize()
		{
			GlobalVariables = new ObservableCollection<GlobalVariableViewModel>();
			if (FiresecClient.FiresecManager.SystemConfiguration.GlobalVariables == null)
				FiresecClient.FiresecManager.SystemConfiguration.GlobalVariables = new List<GlobalVariable>();
			foreach (var GlobalVariable in FiresecClient.FiresecManager.SystemConfiguration.GlobalVariables)
			{
				var globalVariableViewModel = new GlobalVariableViewModel(GlobalVariable);
				GlobalVariables.Add(globalVariableViewModel);
			}
			SelectedGlobalVariable = GlobalVariables.FirstOrDefault();
		}

		ObservableCollection<GlobalVariableViewModel> _globalVariables;
		public ObservableCollection<GlobalVariableViewModel> GlobalVariables
		{
			get { return _globalVariables; }
			set
			{
				_globalVariables = value;
				OnPropertyChanged("GlobalVariables");
			}
		}

		GlobalVariableViewModel _selectedGlobalVariable;
		public GlobalVariableViewModel SelectedGlobalVariable
		{
			get { return _selectedGlobalVariable; }
			set
			{
				_selectedGlobalVariable = value;
				OnPropertyChanged("SelectedGlobalVariable");
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var globalVariableDetailsViewModel = new GlobalVariableDetailsViewModel();
			if (DialogService.ShowModalWindow(globalVariableDetailsViewModel))
			{
				FiresecClient.FiresecManager.SystemConfiguration.GlobalVariables.Add(globalVariableDetailsViewModel.GlobalVariable);
				ServiceFactory.SaveService.AutomationChanged = true;
				var globalVariableViewModel = new GlobalVariableViewModel(globalVariableDetailsViewModel.GlobalVariable);
				GlobalVariables.Add(globalVariableViewModel);
				SelectedGlobalVariable = globalVariableViewModel;
			}
		}

		bool CanAdd()
		{
			return true;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			FiresecClient.FiresecManager.SystemConfiguration.GlobalVariables.Remove(SelectedGlobalVariable.GlobalVariable);
			GlobalVariables.Remove(SelectedGlobalVariable);
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var globalVariableDetailsViewModel = new GlobalVariableDetailsViewModel(SelectedGlobalVariable.GlobalVariable);
			if (DialogService.ShowModalWindow(globalVariableDetailsViewModel))
			{
				SelectedGlobalVariable.Update(globalVariableDetailsViewModel.GlobalVariable);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		bool CanEditDelete()
		{
			return SelectedGlobalVariable != null;
		}

		public void Select(Guid globalVariableUid)
		{
			if (globalVariableUid != Guid.Empty)
			{
				SelectedGlobalVariable = GlobalVariables.FirstOrDefault(item => item.GlobalVariable.Uid == globalVariableUid);
			}
		}
	}
}
