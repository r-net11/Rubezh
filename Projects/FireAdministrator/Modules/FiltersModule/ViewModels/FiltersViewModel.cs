using System.Collections.ObjectModel;
using System.Linq;
using FiltersModule.Views;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace FiltersModule.ViewModels
{
	public class FiltersViewModel : ViewPartViewModel, IEditingViewModel
	{
		public FiltersViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEditRemove);
			DeleteCommand = new RelayCommand(OnDelete, CanEditRemove);
		}

		public void Initialize()
		{
			Filters = new ObservableCollection<FilterViewModel>(
				FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.Select(journalFilter => new FilterViewModel(journalFilter))
			);
		}

		ObservableCollection<FilterViewModel> _filters;
		public ObservableCollection<FilterViewModel> Filters
		{
			get { return _filters; }
			set
			{
				_filters = value;
				OnPropertyChanged("Filters");
			}
		}

		FilterViewModel _selectedFilter;
		public FilterViewModel SelectedFilter
		{
			get { return _selectedFilter; }
			set
			{
				_selectedFilter = value;
				OnPropertyChanged("SelectedFilter");
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var filterDetailsViewModel = new FilterDetailsViewModel();
			if (DialogService.ShowModalWindow(filterDetailsViewModel))
			{
				var filter = filterDetailsViewModel.GetModel();
				FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.Add(filter);
				Filters.Add(new FilterViewModel(filter));

				ServiceFactory.SaveService.FilterChanged = true;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var filterDetailsViewModel = new FilterDetailsViewModel(SelectedFilter.JournalFilter);
			if (DialogService.ShowModalWindow(filterDetailsViewModel))
			{
				FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.Remove(SelectedFilter.JournalFilter);
				FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.Add(filterDetailsViewModel.GetModel());
				SelectedFilter.JournalFilter = filterDetailsViewModel.GetModel();

				ServiceFactory.SaveService.FilterChanged = true;
			}
		}

		bool CanEditRemove()
		{
			return SelectedFilter != null;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.Remove(SelectedFilter.JournalFilter);
			Filters.Remove(SelectedFilter);

			ServiceFactory.SaveService.FilterChanged = true;
		}

		public override void OnShow()
		{
			var filtersMenuViewModel = new FiltersMenuViewModel(this);
			ServiceFactory.Layout.ShowMenu(filtersMenuViewModel);

			if (FilterMenuView.Current != null)
				FilterMenuView.Current.AcceptKeyboard = true;
		}

		public override void OnHide()
		{
			ServiceFactory.Layout.ShowMenu(null);

			if (FilterMenuView.Current != null)
				FilterMenuView.Current.AcceptKeyboard = false;
		}
	}
}