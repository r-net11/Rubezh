using System.Collections.ObjectModel;
using Common;
using Infrastructure;
using Infrastructure.Common;

namespace FiltersModule.ViewModels
{
    public class FiltersViewModel : RegionViewModel
    {
        public FiltersViewModel()
        {
            CreateCommand = new RelayCommand(OnCreate);
            EditCommand = new RelayCommand(OnEdit, CanEditOrRemove);
            RemoveCommand = new RelayCommand(OnRemove, CanEditOrRemove);
        }

        public void Initialize()
        {
            FilterViewModels = new ObservableCollection<FilterViewModel>();
            if (FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.IsNotNullOrEmpty())
            {
                FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.ForEach(journalFilter => FilterViewModels.Add(new FilterViewModel(journalFilter)));
            }
        }

        public ObservableCollection<FilterViewModel> FilterViewModels { get; private set; }
        public FilterViewModel SelectedFilter { get; set; }

        public RelayCommand CreateCommand { get; private set; }
        void OnCreate()
        {
            var filterDetailsViewModel = new FilterDetailsViewModel();
            if (ServiceFactory.UserDialogs.ShowModalWindow(filterDetailsViewModel))
            {
                var filter = filterDetailsViewModel.GetModel();
                FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.Add(filter);
                FilterViewModels.Add(new FilterViewModel(filter));
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

        bool CanEditOrRemove()
        {
            return SelectedFilter != null;
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.Remove(SelectedFilter.JournalFilter);
            FilterViewModels.Remove(SelectedFilter);
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