using System.Collections.ObjectModel;
using Infrastructure;
using Infrastructure.Common;

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
            foreach (var filter in FiresecAPI.Models.JournalManager.Filters)
            {
                filters.Add(new FilterViewModel(filter));
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
                FilterDetailsViewModel filterDetailsViewModel = new FilterDetailsViewModel(SelectedFilter.JournalFilter.Copy());
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
                FiresecAPI.Models.JournalManager.Filters =
                    new System.Collections.Generic.List<FiresecAPI.Models.JournalFilter>();
                foreach (var filterViewModel in FilterViewModels)
                {
                    FiresecAPI.Models.JournalManager.Filters.Add(filterViewModel.JournalFilter);
                }
                FiresecAPI.Models.JournalManager.Save();
            }
        }

        public override void OnShow()
        {
            FiltersMenuViewModel journalsMenuViewModel =
                new FiltersMenuViewModel(CreateCommand, EditCommand, RemoveCommand, SaveCommand);
            ServiceFactory.Layout.ShowMenu(journalsMenuViewModel);
        }
    }
}
