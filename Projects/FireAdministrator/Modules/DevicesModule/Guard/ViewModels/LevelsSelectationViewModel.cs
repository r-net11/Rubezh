using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class LevelsSelectationViewModel : SaveCancelDialogContent
    {
        public List<string> LevelNames { get; private set; }

        public LevelsSelectationViewModel(GuardUser guardUser)
        {
            Title = "Выбор уровней доступа";

            AddOneCommand = new RelayCommand(OnAddOne, CanAdd);
            RemoveOneCommand = new RelayCommand(OnRemoveOne, CanRemove);
            AddAllCommand = new RelayCommand(OnAddAll, CanAdd);
            RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemove);

            Initialize(guardUser);
        }

        void Initialize(GuardUser guardUser)
        {
            LevelNames = guardUser.LevelNames;
            TargetLevels = new ObservableCollection<LevelViewModel>();
            SourceLevels = new ObservableCollection<LevelViewModel>();

            foreach (var guardLevel in FiresecManager.DeviceConfiguration.GuardLevels)
            {
                if (LevelNames.Contains(guardLevel.Name))
                    TargetLevels.Add(new LevelViewModel(guardLevel));
                else
                    SourceLevels.Add(new LevelViewModel(guardLevel));
            }

            if (TargetLevels.Count > 0)
                SelectedTargetLevel = TargetLevels[0];

            if (SourceLevels.Count > 0)
                SelectedSourceLevel = SourceLevels[0];
        }

        public ObservableCollection<LevelViewModel> SourceLevels { get; private set; }

        LevelViewModel _selectedSourceLevel;
        public LevelViewModel SelectedSourceLevel
        {
            get { return _selectedSourceLevel; }
            set
            {
                _selectedSourceLevel = value;
                OnPropertyChanged("SelectedSourceLevel");
            }
        }

        public ObservableCollection<LevelViewModel> TargetLevels { get; private set; }

        LevelViewModel _selectedTargetLevel;
        public LevelViewModel SelectedTargetLevel
        {
            get { return _selectedTargetLevel; }
            set
            {
                _selectedTargetLevel = value;
                OnPropertyChanged("SelectedTargetLevel");
            }
        }

        public RelayCommand AddOneCommand { get; private set; }
        void OnAddOne()
        {
            TargetLevels.Add(SelectedSourceLevel);
            SelectedTargetLevel = SelectedSourceLevel;
            SourceLevels.Remove(SelectedSourceLevel);

            if (SourceLevels.Count > 0)
                SelectedSourceLevel = SourceLevels[0];
        }

        public RelayCommand RemoveOneCommand { get; private set; }
        void OnRemoveOne()
        {
            SourceLevels.Add(SelectedTargetLevel);
            SelectedSourceLevel = SelectedTargetLevel;
            TargetLevels.Remove(SelectedTargetLevel);

            if (TargetLevels.Count > 0)
                SelectedTargetLevel = TargetLevels[0];
        }

        public RelayCommand AddAllCommand { get; private set; }
        void OnAddAll()
        {
            foreach (var zoneViewModel in SourceLevels)
            {
                TargetLevels.Add(zoneViewModel);
            }
            SourceLevels.Clear();

            if (TargetLevels.Count > 0)
                SelectedTargetLevel = TargetLevels[0];
        }

        public RelayCommand RemoveAllCommand { get; private set; }
        void OnRemoveAll()
        {
            foreach (var zoneViewModel in TargetLevels)
            {
                SourceLevels.Add(zoneViewModel);
            }
            TargetLevels.Clear();

            if (SourceLevels.Count > 0)
                SelectedSourceLevel = SourceLevels[0];
        }

        bool CanAdd()
        {
            return SelectedSourceLevel != null;
        }

        bool CanRemove()
        {
            return SelectedTargetLevel != null;
        }

        protected override void Save(ref bool cancel)
        {
            LevelNames = new List<string>();
            foreach (var level in TargetLevels)
            {
                LevelNames.Add(level.GuardLevel.Name);
            }
        }
    }
}