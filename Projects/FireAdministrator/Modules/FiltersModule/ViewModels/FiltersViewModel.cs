using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure;
using Infrastructure.Common;

namespace FiltersModule.ViewModels
{
    public class FiltersViewModel : RegionViewModel, IEditingViewModel
    {
        public FiltersViewModel()
        {
            AddCommand = new RelayCommand(OnAdd);
            EditCommand = new RelayCommand(OnEdit, CanEditRemove);
            DeleteCommand = new RelayCommand(OnDelete, CanEditRemove);

            Filters = new ObservableCollection<FilterViewModel>(
                FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.Select(journalFilter => new FilterViewModel(journalFilter))
            );
        }

        public ObservableCollection<FilterViewModel> Filters { get; private set; }

        FilterViewModel _selectedFilter;
        public FilterViewModel SelectedFilter
        {
            get { return _selectedFilter; }
            set
            {
                value = _selectedFilter;
                OnPropertyChanged("SelectedFilter");
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var filterDetailsViewModel = new FilterDetailsViewModel();
            if (ServiceFactory.UserDialogs.ShowModalWindow(filterDetailsViewModel))
            {
                var filter = filterDetailsViewModel.GetModel();
                FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.Add(filter);
                Filters.Add(new FilterViewModel(filter));

                FilterModule.HasChanges = true;
            }
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            var filterDetailsViewModel = new FilterDetailsViewModel(SelectedFilter.JournalFilter);
            if (ServiceFactory.UserDialogs.ShowModalWindow(filterDetailsViewModel))
            {
                FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.Remove(SelectedFilter.JournalFilter);
                FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.Add(filterDetailsViewModel.GetModel());
                SelectedFilter.JournalFilter = filterDetailsViewModel.GetModel();

                FilterModule.HasChanges = true;
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

            FilterModule.HasChanges = true;
        }

        public override void OnShow()
        {
            var filtersMenuViewModel = new FiltersMenuViewModel(this);
            ServiceFactory.Layout.ShowMenu(filtersMenuViewModel);
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}