using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class GuardLevelsSelectationViewModel : SaveCancelDialogContent
    {
        public GuardLevelsSelectationViewModel()
        {
            Title = "Выбор уровней доступа";

            AddOneCommand = new RelayCommand(OnAddOne, CanAdd);
            RemoveOneCommand = new RelayCommand(OnRemoveOne, CanRemove);
            AddAllCommand = new RelayCommand(OnAddAll, CanAdd);
            RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemove);
        }

        public List<string> GuardLevels { get; private set; }

        public void Initialize(GuardUser guardUser)
        {
            GuardLevels = guardUser.GuardLevelNames;
            TargetLevels = new ObservableCollection<GuardLevelViewModel>();
            SourceLevels = new ObservableCollection<GuardLevelViewModel>();

            foreach (var guardLevel in FiresecManager.DeviceConfiguration.GuardLevels)
            {
                if (GuardLevels.Contains(guardLevel.Name))
                    TargetLevels.Add(new GuardLevelViewModel(guardLevel));
                else
                    SourceLevels.Add(new GuardLevelViewModel(guardLevel));
            }

            if (TargetLevels.Count > 0)
                SelectedTargetLevel = TargetLevels[0];

            if (SourceLevels.Count > 0)
                SelectedSourceLevel = SourceLevels[0];
        }

        public ObservableCollection<GuardLevelViewModel> SourceLevels { get; private set; }

        GuardLevelViewModel _selectedSourceLevel;
        public GuardLevelViewModel SelectedSourceLevel
        {
            get { return _selectedSourceLevel; }
            set
            {
                _selectedSourceLevel = value;
                OnPropertyChanged("SelectedSourceLevel");
            }
        }

        public ObservableCollection<GuardLevelViewModel> TargetLevels { get; private set; }

        GuardLevelViewModel _selectedTargetLevel;
        public GuardLevelViewModel SelectedTargetLevel
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
            GuardLevels = new List<string>();
            foreach (var zoneViewModel in TargetLevels)
            {
                GuardLevels.Add(zoneViewModel.GuardLevel.Name);
            }
        }
    }
}