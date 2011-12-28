using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class LevelsViewModel : RegionViewModel, IEditingViewModel
    {
        public LevelsViewModel()
        {
            DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
            EditCommand = new RelayCommand(OnEdit, CanEditDelete);
            AddCommand = new RelayCommand(OnAdd);

            Levels = new ObservableCollection<LevelViewModel>();
            foreach (var guardLevel in FiresecManager.DeviceConfiguration.GuardLevels)
            {
                var guardLevelViewModel = new LevelViewModel(guardLevel);
                Levels.Add(guardLevelViewModel);
            }

            if (Levels.Count > 0)
                SelectedLevel = Levels[0];
        }

        public ObservableCollection<LevelViewModel> Levels { get; private set; }

        LevelViewModel _selectedLevel;
        public LevelViewModel SelectedLevel
        {
            get { return _selectedLevel; }
            set
            {
                _selectedLevel = value;
                OnPropertyChanged("SelectedLevel");
            }
        }

        bool CanEditDelete()
        {
            return (SelectedLevel != null);
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            FiresecManager.DeviceConfiguration.GuardLevels.Remove(SelectedLevel.GuardLevel);
            Levels.Remove(SelectedLevel);
            DevicesModule.HasChanges = true;
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            var levelDetailsViewModel = new LevelDetailsViewModel(SelectedLevel.GuardLevel);
            if (ServiceFactory.UserDialogs.ShowModalWindow(levelDetailsViewModel))
            {
                SelectedLevel.GuardLevel = levelDetailsViewModel.GuardLevel;
                DevicesModule.HasChanges = true;
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var guardLevelDetailsViewModel = new LevelDetailsViewModel();
            if (ServiceFactory.UserDialogs.ShowModalWindow(guardLevelDetailsViewModel))
            {
                FiresecManager.DeviceConfiguration.GuardLevels.Add(guardLevelDetailsViewModel.GuardLevel);
                Levels.Add(new LevelViewModel(guardLevelDetailsViewModel.GuardLevel));
                DevicesModule.HasChanges = true;
            }
        }

        public override void OnShow()
        {
            ServiceFactory.Layout.ShowMenu(new LevelsMenuViewModel(this));
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}