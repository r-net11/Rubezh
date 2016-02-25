using System.Collections.ObjectModel;
using System.Linq;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Automation;
using Infrastructure.Common;

namespace AutomationModule.ViewModels
{
	public class ConditionsViewModel : BaseViewModel
	{
		public Procedure Procedure { get; private set; }
		public ConditionsViewModel(Procedure procedure)
		{
			Procedure = procedure;
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			AddOpcTagFilterCommand = new RelayCommand(OnAddOpcTagFilter);
			DeleteOpcTagFilterCommand = new RelayCommand(OnDeleteOpcTagFilter, CanDeleteOpcTagFilter);
			Initialize();
		}

		void Initialize()
		{
			Filters = new ObservableCollection<FilterViewModel>();
			foreach (var filter in ClientManager.SystemConfiguration.JournalFilters)
			{
				if (Procedure.FiltersUids.Contains(filter.UID))
				{
					var filterViewModel = new FilterViewModel(filter);
					Filters.Add(filterViewModel);
				}
			}

			OpcTagFilters = new ObservableCollection<OpcTagFilterViewModel>();
			foreach (var opcFilter in ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTagFilters)
			{
				if (Procedure.OpcDaTagFiltersUids.Contains(opcFilter.UID))
				{
					var opcFilterViewModel = new OpcTagFilterViewModel(opcFilter);
					OpcTagFilters.Add(opcFilterViewModel);
				}
			}
		}

		public ObservableCollection<FilterViewModel> Filters { get; private set; }

		FilterViewModel _selectedFilter;
		public FilterViewModel SelectedFilter
		{
			get { return _selectedFilter; }
			set
			{
				_selectedFilter = value;
				OnPropertyChanged(() => SelectedFilter);
			}
		}

		public ObservableCollection<OpcTagFilterViewModel> OpcTagFilters { get; private set; }

		OpcTagFilterViewModel _selectedOpcDaTagFilter;
		public OpcTagFilterViewModel SelectedOpcDaTagFilter
		{
			get { return _selectedOpcDaTagFilter; }
			set
			{
				_selectedOpcDaTagFilter = value;
				OnPropertyChanged(() => SelectedOpcDaTagFilter);
			}
		}

		public void UpdateContent()
		{
			foreach (var filter in Filters.Where(x => !ClientManager.SystemConfiguration.JournalFilters.Any(y => y.UID == x.Filter.UID)).ToList())
				Filters.Remove(filter);
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var procedureFilterDetailsViewModel = new FilterSelectionViewModel(Procedure);
			if (DialogService.ShowModalWindow(procedureFilterDetailsViewModel))
			{
				var filterViewModel = procedureFilterDetailsViewModel.SelectedFilter;
				Filters.Add(filterViewModel);
				SelectedFilter = filterViewModel;
				Procedure.FiltersUids.Add(filterViewModel.Filter.UID);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			Procedure.FiltersUids.Remove(SelectedFilter.Filter.UID);
			Filters.Remove(SelectedFilter);
			SelectedFilter = Filters.FirstOrDefault();
			ServiceFactory.SaveService.AutomationChanged = true;
		}
		bool CanDelete()
		{
			return SelectedFilter != null;
		}

		public RelayCommand AddOpcTagFilterCommand { get; private set; }
		void OnAddOpcTagFilter()
		{
			var opcDaTagFileterSectionViewModel = new OpcDaTagFileterSectionViewModel(Procedure);
			if (DialogService.ShowModalWindow(opcDaTagFileterSectionViewModel))
			{
				if (Procedure.OpcDaTagFiltersUids.Any(s => 
					s == opcDaTagFileterSectionViewModel.SelectedFilter.OpcTagFilter.UID))
					return;
				Procedure.OpcDaTagFiltersUids.Add(opcDaTagFileterSectionViewModel.SelectedFilter.OpcTagFilter.UID);
				OpcTagFilters.Add(opcDaTagFileterSectionViewModel.SelectedFilter);
				SelectedOpcDaTagFilter = opcDaTagFileterSectionViewModel.SelectedFilter;
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public RelayCommand DeleteOpcTagFilterCommand { get; private set; }
		void OnDeleteOpcTagFilter()
		{
			Procedure.OpcDaTagFiltersUids.Remove(SelectedOpcDaTagFilter.OpcTagFilter.UID);
			ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTagFilters
				.Remove(SelectedOpcDaTagFilter.OpcTagFilter);
			OpcTagFilters.Remove(SelectedOpcDaTagFilter);
			SelectedOpcDaTagFilter = OpcTagFilters.FirstOrDefault();
			ServiceFactory.SaveService.AutomationChanged = true;
		}
		bool CanDeleteOpcTagFilter()
		{
			return SelectedOpcDaTagFilter != null;
		}
	}
}