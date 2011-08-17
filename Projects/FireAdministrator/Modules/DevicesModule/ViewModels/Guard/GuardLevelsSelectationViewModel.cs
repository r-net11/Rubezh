using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecAPI.Models;
using System.Collections.ObjectModel;
using FiresecClient;

namespace DevicesModule.ViewModels
{
    public class GuardLevelsSelectationViewModel : DialogContent
    {
        public GuardLevelsSelectationViewModel()
        {
            Title = "Выбор уровней доступа";

            AddOneCommand = new RelayCommand(OnAddOne, CanAdd);
            RemoveOneCommand = new RelayCommand(OnRemoveOne, CanRemove);
            AddAllCommand = new RelayCommand(OnAddAll, CanAdd);
            RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemove);

            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);
        }

        public List<string> GuardLevels { get; private set; }


        public void Initialize(GuardUser guardUser)
        {
            GuardLevels = guardUser.GuardLevelNames;
            TargetGuardLevels = new ObservableCollection<GuardLevelViewModel>();
            SourceGuardLevels = new ObservableCollection<GuardLevelViewModel>();

            foreach (var guardLevel in FiresecManager.DeviceConfiguration.GuardLevels)
            {
                GuardLevelViewModel guardLevelViewModel = new GuardLevelViewModel(guardLevel);

                if (GuardLevels.Contains(guardLevel.Name))
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
            GuardLevels = new List<string>();
            foreach (var zoneViewModel in TargetGuardLevels)
            {
                GuardLevels.Add(zoneViewModel.GuardLevel.Name);
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
