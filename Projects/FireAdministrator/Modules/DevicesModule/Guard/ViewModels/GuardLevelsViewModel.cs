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

            if (GuardLevels.Count > 0)
                SelectedGuardLevel = GuardLevels[0];
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
            if (ServiceFactory.UserDialogs.ShowModalWindow(guardLevelDetailsViewModel))
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
            if (ServiceFactory.UserDialogs.ShowModalWindow(guardLevelDetailsViewModel))
            {
                FiresecManager.DeviceConfiguration.GuardLevels.Add(guardLevelDetailsViewModel.GuardLevel);
                GuardLevels.Add(new GuardLevelViewModel(guardLevelDetailsViewModel.GuardLevel));

                DevicesModule.HasChanges = true;
            }
        }

        public override void OnShow()
        {
            ServiceFactory.Layout.ShowMenu(new GuardLevelsMenuViewModel(this));
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}