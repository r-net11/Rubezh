using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class ZonesSelectionViewModel : DialogContent
    {
        public ZonesSelectionViewModel()
        {
            Title = "Выбор уровней доступа";

            AddOneCommand = new RelayCommand(OnAddOne, CanAdd);
            RemoveOneCommand = new RelayCommand(OnRemoveOne, CanRemove);
            AddAllCommand = new RelayCommand(OnAddAll, CanAdd);
            RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemove);

            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);
        }

        public List<string> GuardLevelNames { get; private set; }

        public void Initialize(List<string> guardLevelNames)
        {
            GuardLevelNames = guardLevelNames;
            TargetGuardLevels = new ObservableCollection<GuardLevelViewModel>();
            SourceGuardLevels = new ObservableCollection<GuardLevelViewModel>();

            foreach (GuardLevel guardLevel in FiresecManager.DeviceConfiguration.GuardLevels)
            {
                var guardLevelViewModel = new GuardLevelViewModel(guardLevel);

                if (GuardLevelNames.Contains(guardLevel.Name))
                {
                    TargetGuardLevels.Add(guardLevelViewModel);
                }
                else
                {
                    SourceGuardLevels.Add(guardLevelViewModel);
                }
            }

            if (TargetGuardLevels.Count > 0)
                SelectedTargetGuardLevel = TargetGuardLevels[0];

            if (SourceGuardLevels.Count > 0)
                SelectedSourceGuardLevel = SourceGuardLevels[0];
        }

        public ObservableCollection<GuardLevelViewModel> SourceGuardLevels { get; private set; }

        GuardLevelViewModel _selectedSourceGuardLevel;
        public GuardLevelViewModel SelectedSourceGuardLevel
        {
            get { return _selectedSourceGuardLevel; }
            set
            {
                _selectedSourceGuardLevel = value;
                OnPropertyChanged("SelectedSourceGuardLevel");
            }
        }

        public ObservableCollection<GuardLevelViewModel> TargetGuardLevels { get; private set; }

        GuardLevelViewModel _selectedTargetGuardLevel;
        public GuardLevelViewModel SelectedTargetGuardLevel
        {
            get { return _selectedTargetGuardLevel; }
            set
            {
                _selectedTargetGuardLevel = value;
                OnPropertyChanged("SelectedTargetGuardLevel");
            }
        }

        public RelayCommand AddOneCommand { get; private set; }
        void OnAddOne()
        {
            TargetGuardLevels.Add(SelectedSourceGuardLevel);
            SelectedTargetGuardLevel = SelectedSourceGuardLevel;
            SourceGuardLevels.Remove(SelectedSourceGuardLevel);

            if (SourceGuardLevels.Count > 0)
                SelectedSourceGuardLevel = SourceGuardLevels[0];
        }

        public RelayCommand RemoveOneCommand { get; private set; }
        void OnRemoveOne()
        {
            SourceGuardLevels.Add(SelectedTargetGuardLevel);
            SelectedSourceGuardLevel = SelectedTargetGuardLevel;
            TargetGuardLevels.Remove(SelectedTargetGuardLevel);

            if (TargetGuardLevels.Count > 0)
                SelectedTargetGuardLevel = TargetGuardLevels[0];
        }

        public RelayCommand AddAllCommand { get; private set; }
        void OnAddAll()
        {
            foreach (var zoneViewModel in SourceGuardLevels)
            {
                TargetGuardLevels.Add(zoneViewModel);
            }
            SourceGuardLevels.Clear();

            if (TargetGuardLevels.Count > 0)
                SelectedTargetGuardLevel = TargetGuardLevels[0];
        }

        public RelayCommand RemoveAllCommand { get; private set; }
        void OnRemoveAll()
        {
            foreach (var zoneViewModel in TargetGuardLevels)
            {
                SourceGuardLevels.Add(zoneViewModel);
            }
            TargetGuardLevels.Clear();

            if (SourceGuardLevels.Count > 0)
                SelectedSourceGuardLevel = SourceGuardLevels[0];
        }

        bool CanAdd(object obj)
        {
            return SelectedSourceGuardLevel != null;
        }

        bool CanRemove(object obj)
        {
            return SelectedTargetGuardLevel != null;
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            GuardLevelNames = new List<string>();
            foreach (var zoneViewModel in TargetGuardLevels)
            {
                GuardLevelNames.Add(zoneViewModel.GuardLevel.Name);
            }

            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}