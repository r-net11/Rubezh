using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infrastructure;
using Infrastructure.Common;
using FiresecAPI.Models;

namespace FiltersModule.ViewModels
{
    public class FiltersViewModel : RegionViewModel
    {
        public FiltersViewModel()
        {
            CreateCommand = new RelayCommand(OnCreate);
            EditCommand = new RelayCommand(OnEdit);
            RemoveCommand = new RelayCommand(OnRemove);
            SaveCommand = new RelayCommand(OnSave);
        }

        public void Initialize()
        {
            var filters = new ObservableCollection<FilterViewModel>();
            if (FiresecClient.FiresecManager.SystemConfiguration.JournalFilters != null)
            {
                foreach (var filter in FiresecClient.FiresecManager.SystemConfiguration.JournalFilters)
                {
                    filters.Add(new FilterViewModel(filter));
                }
            }
            FilterViewModels = filters;
        }

        public ObservableCollection<FilterViewModel> FilterViewModels { get; private set; }
        public FilterViewModel SelectedFilter { get; set; }

        public RelayCommand CreateCommand { get; private set; }
        void OnCreate()
        {
            FilterDetailsViewModel filterDetailsViewModel = new FilterDetailsViewModel();
            if (ServiceFactory.UserDialogs.ShowModalWindow(filterDetailsViewModel))
            {
                FilterViewModels.Add(new FilterViewModel(filterDetailsViewModel.GetModel()));
            }
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            if (SelectedFilter != null)
            {
                FilterDetailsViewModel filterDetailsViewModel = new FilterDetailsViewModel(SelectedFilter.JournalFilter);
                if (ServiceFactory.UserDialogs.ShowModalWindow(filterDetailsViewModel))
                {
                    SelectedFilter.JournalFilter = filterDetailsViewModel.GetModel();
                }
            }
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            if (SelectedFilter != null)
            {
                FilterViewModels.Remove(SelectedFilter);
            }
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            if (FilterViewModels != null)
            {
                FiresecClient.FiresecManager.SystemConfiguration.JournalFilters = new List<JournalFilter>();
                foreach (var filterViewModel in FilterViewModels)
                {
                    FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.Add(filterViewModel.JournalFilter);
                }
            }
        }

        public override void OnShow()
        {
            FiltersMenuViewModel journalsMenuViewModel = new FiltersMenuViewModel(CreateCommand, EditCommand, RemoveCommand, SaveCommand);
            ServiceFactory.Layout.ShowMenu(journalsMenuViewModel);
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}
