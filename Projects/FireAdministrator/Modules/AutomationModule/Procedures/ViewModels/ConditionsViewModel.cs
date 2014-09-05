using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
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
			Initialize();
		}

		void Initialize()
		{
			Filters = new ObservableCollection<FilterViewModel>();
			foreach (var filter in FiresecManager.SystemConfiguration.JournalFilters)
			{
				if (Procedure.FiltersUids.Contains(filter.UID))
				{
					var filterViewModel = new FilterViewModel(filter);
					Filters.Add(filterViewModel);
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
	}
}