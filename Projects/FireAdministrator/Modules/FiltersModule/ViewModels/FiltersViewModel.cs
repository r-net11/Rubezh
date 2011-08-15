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
            EditCommand = new RelayCommand(() => OnEdit(), x => SelectedFilter != null);
            RemoveCommand = new RelayCommand(() => OnRemove(), x => SelectedFilter != null);
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
            FilterDetailsViewModel filterDetailsViewModel = new FilterDetailsViewModel(existingNames);

            if (ServiceFactory.UserDialogs.ShowModalWindow(filterDetailsViewModel))
            {
                FilterViewModels.Add(new FilterViewModel(filterDetailsViewModel.GetModel()));
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
            }
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            FilterViewModels.Remove(SelectedFilter);
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