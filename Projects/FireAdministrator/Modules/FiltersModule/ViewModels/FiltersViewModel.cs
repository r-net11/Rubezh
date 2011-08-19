using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            EditCommand = new RelayCommand(OnEdit, CanEdit);
            RemoveCommand = new RelayCommand(OnRemove, CanRemove);
        }

        public void Initialize()
        {
            FilterViewModels = new ObservableCollection<FilterViewModel>();
            if (FiresecClient.FiresecManager.SystemConfiguration.JournalFilters != null)
            {
                foreach (var filter in
                    FiresecClient.FiresecManager.SystemConfiguration.JournalFilters)
                {
                    FilterViewModels.Add(new FilterViewModel(filter));
                }
            }
        }

        public bool HasChanges { get; set; }
        public ObservableCollection<FilterViewModel> FilterViewModels { get; private set; }
        public FilterViewModel SelectedFilter { get; set; }

        public RelayCommand CreateCommand { get; private set; }
        void OnCreate()
        {
            var existingNames = new List<string>();
            foreach (var filterViewModel in FilterViewModels)
            {
                existingNames.Add(filterViewModel.JournalFilter.Name);
            }
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
            var existingNames = new List<string>();
            foreach (var filterViewModel in FilterViewModels)
            {
                if (filterViewModel != SelectedFilter)
                    existingNames.Add(filterViewModel.JournalFilter.Name);
            }
            FilterDetailsViewModel filterDetailsViewModel =
                new FilterDetailsViewModel(SelectedFilter.JournalFilter, existingNames);

            if (ServiceFactory.UserDialogs.ShowModalWindow(filterDetailsViewModel))
            {
                SelectedFilter.JournalFilter = filterDetailsViewModel.GetModel();
                HasChanges = true;
            }
        }

        bool CanEdit(object obj)
        {
            return SelectedFilter != null;
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            FilterViewModels.Remove(SelectedFilter);
            HasChanges = true;
        }

        bool CanRemove(object obj)
        {
            return SelectedFilter != null;
        }

        public void Save()
        {
            if (FilterViewModels != null)
            {
                FiresecClient.FiresecManager.SystemConfiguration.JournalFilters =
                    new List<JournalFilter>();

                foreach (var filterViewModel in FilterViewModels)
                {
                    FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.Add(
                        filterViewModel.JournalFilter);
                }
            }
        }

        public override void OnShow()
        {
            FiltersMenuViewModel filtersMenuViewModel =
                new FiltersMenuViewModel(CreateCommand, EditCommand, RemoveCommand);
            ServiceFactory.Layout.ShowMenu(filtersMenuViewModel);
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}