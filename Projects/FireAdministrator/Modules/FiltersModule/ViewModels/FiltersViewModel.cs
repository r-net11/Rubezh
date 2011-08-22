using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
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
            if (FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.IsNotNullOrEmpry())
            {
                FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.ForEach(
                    journalFilter => FilterViewModels.Add(new FilterViewModel(journalFilter)));
            }
        }

        public bool HasChanges { get; set; }
        public ObservableCollection<FilterViewModel> FilterViewModels { get; private set; }
        public FilterViewModel SelectedFilter { get; set; }

        public RelayCommand CreateCommand { get; private set; }
        void OnCreate()
        {
            var existingNames = FilterViewModels.Select(x => x.JournalFilter.Name).ToList();
            var filterDetailsViewModel = new FilterDetailsViewModel(existingNames);

            if (ServiceFactory.UserDialogs.ShowModalWindow(filterDetailsViewModel))
            {
                FilterViewModels.Add(new FilterViewModel(filterDetailsViewModel.GetModel()));
                HasChanges = true;
            }
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            var existingNames = FilterViewModels.Where(x => x != SelectedFilter).
                Select(x => x.JournalFilter.Name).ToList();
            var filterDetailsViewModel = new FilterDetailsViewModel(SelectedFilter.JournalFilter, existingNames);

            if (ServiceFactory.UserDialogs.ShowModalWindow(filterDetailsViewModel))
            {
                SelectedFilter.JournalFilter = filterDetailsViewModel.GetModel();
                HasChanges = true;
            }
        }

        bool CanEditOrRemove(object obj)
        {
            return SelectedFilter != null;
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            FilterViewModels.Remove(SelectedFilter);
            HasChanges = true;
        }

        public void Save()
        {
            if (FilterViewModels.IsNotNullOrEmpry())
            {
                FiresecClient.FiresecManager.SystemConfiguration.JournalFilters = new List<JournalFilter>();
                    FilterViewModels.Select(x => x.JournalFilter).ToList();
            }
        }

        public override void OnShow()
        {
            var filtersMenuViewModel = new FiltersMenuViewModel(CreateCommand, EditCommand, RemoveCommand);
            ServiceFactory.Layout.ShowMenu(filtersMenuViewModel);
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}