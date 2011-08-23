using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using System.Collections.Generic;

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
                FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.ForEach(
                    journalFilter => FilterViewModels.Add(new FilterViewModel(journalFilter)));
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
                FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.Add(filterDetailsViewModel.GetModel());
                FilterViewModels.Add(new FilterViewModel(filterDetailsViewModel.GetModel()));
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

        bool CanEditOrRemove(object obj)
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

        public void Save()
        {
            if (FilterViewModels.IsNotNullOrEmpty())
            {
                FiresecClient.FiresecManager.SystemConfiguration.JournalFilters =
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