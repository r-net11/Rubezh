using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Automation;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace AutomationModule.ViewModels
{
	public class OpcDaTagFileterSectionViewModel : SaveCancelDialogViewModel
	{
		public OpcDaTagFileterSectionViewModel(Procedure procedure)
		{
			Title = "Выбор фильтра";
			Procedure = procedure;
			InitializeFilters();
			CreateFilterCommand = new RelayCommand(OnCreateFilter);
		}

		Procedure Procedure { get; set; }

		void InitializeFilters()
		{
			Filters = new ObservableCollection<OpcTagFilterViewModel>();
			foreach (var filter in ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTagFilters
				.FindAll(x => !Procedure.OpcDaTagFiltersUids.Contains(x.UID)))
			{
				var filterViewModel = new OpcTagFilterViewModel(filter);
				Filters.Add(filterViewModel);
			}
			OnPropertyChanged(() => Filters);
			SelectedFilter = Filters.FirstOrDefault();
		}

		public RelayCommand CreateFilterCommand { get; private set; }
		void OnCreateFilter()
		{
			var opcDaTagFilterCreationViewModel = new OpcDaTagFilterCreationViewModel(this);
			if (DialogService.ShowModalWindow(opcDaTagFilterCreationViewModel))
			{
				var filterViewModel = opcDaTagFilterCreationViewModel.OpcDaTagFilterResult;
				ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTagFilters.Add(filterViewModel.OpcTagFilter);
				ServiceFactory.SaveService.AutomationChanged = true;
				InitializeFilters();
			}
		}

		public ObservableCollection<OpcTagFilterViewModel> Filters { get; private set; }

		OpcTagFilterViewModel _selectedFilter;
		public OpcTagFilterViewModel SelectedFilter
		{
			get { return _selectedFilter; }
			set
			{
				_selectedFilter = value;
				OnPropertyChanged(() => SelectedFilter);
			}
		}

		protected override bool Save()
		{
			return SelectedFilter != null;
		}
	}
}