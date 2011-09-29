using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class GuardLevelsViewModel : RegionViewModel
    {
        public GuardLevelsViewModel()
        {
            DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
            EditCommand = new RelayCommand(OnEdit, CanEditDelete);
            AddCommand = new RelayCommand(OnAdd);
        }

        public void Initialize()
        {
            GuardLevels = new ObservableCollection<GuardLevelViewModel>();
            foreach (var guardLevel in FiresecManager.DeviceConfiguration.GuardLevels)
            {
                var guardLevelViewModel = new GuardLevelViewModel(guardLevel);
                GuardLevels.Add(guardLevelViewModel);
            }
        }

        public ObservableCollection<GuardLevelViewModel> GuardLevels { get; private set; }

        GuardLevelViewModel _selectedGuardLevel;
        public GuardLevelViewModel SelectedGuardLevel
        {
            get { return _selectedGuardLevel; }
            set
            {
                _selectedGuardLevel = value;
                OnPropertyChanged("SelectedGuardLevel");
            }
        }

        bool CanEditDelete()
        {
            return (SelectedGuardLevel != null);
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            FiresecManager.DeviceConfiguration.GuardLevels.Remove(SelectedGuardLevel.GuardLevel);
            GuardLevels.Remove(SelectedGuardLevel);
            DevicesModule.HasChanges = true;
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            var guardLevelDetailsViewModel = new GuardLevelDetailsViewModel();
            guardLevelDetailsViewModel.Initialize(SelectedGuardLevel.GuardLevel);
            var result = ServiceFactory.UserDialogs.ShowModalWindow(guardLevelDetailsViewModel);
            if (result)
            {
                SelectedGuardLevel.GuardLevel = guardLevelDetailsViewModel.GuardLevel;
                DevicesModule.HasChanges = true;
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var guardLevelDetailsViewModel = new GuardLevelDetailsViewModel();
            guardLevelDetailsViewModel.Initialize();
            var result = ServiceFactory.UserDialogs.ShowModalWindow(guardLevelDetailsViewModel);
            if (result)
            {
                FiresecManager.DeviceConfiguration.GuardLevels.Add(guardLevelDetailsViewModel.GuardLevel);
                var guardLevelViewModel = new GuardLevelViewModel(guardLevelDetailsViewModel.GuardLevel);
                GuardLevels.Add(guardLevelViewModel);
                DevicesModule.HasChanges = true;
            }
        }

        public override void OnShow()
        {
            var guardLevelsMenuViewModel = new GuardLevelsMenuViewModel(this);
            ServiceFactory.Layout.ShowMenu(guardLevelsMenuViewModel);
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}
