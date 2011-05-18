using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Collections.ObjectModel;

namespace ServiceVisualizer
{
    public class ZoneLogicSectionViewModel : BaseViewModel
    {
        public ZoneLogicSectionViewModel()
        {
            SaveCommand = new RelayCommand(OnSaveCommand);
            AddOneCommand = new RelayCommand(OnAddOneCommand, CanAdd);
            RemoveOneCommand = new RelayCommand(OnRemoveOneCommand, CanRemove);
            AddAllCommand = new RelayCommand(OnAddAllCommand, CanAdd);
            RemoveAllCommand = new RelayCommand(OnRemoveAllCommand, CanRemove);
        }

        Firesec.ZoneLogic.clauseType clause;

        public void Initialize(Firesec.ZoneLogic.clauseType clause)
        {
            this.clause = clause;
            TargetZoneViewModels = new ObservableCollection<ZoneViewModel>();
            SourceZoneViewModels = new ObservableCollection<ZoneViewModel>();

            foreach (ZoneViewModel zoneViewModel in ViewModel.Current.ZoneViewModels)
            {
                if (clause.zone.Contains(zoneViewModel.ZoneNo))
                {
                    TargetZoneViewModels.Add(zoneViewModel);
                }
                else
                {
                    SourceZoneViewModels.Add(zoneViewModel);
                }
            }

            if (TargetZoneViewModels.Count > 0)
                SelectedTargetZone = TargetZoneViewModels[0];

            if (SourceZoneViewModels.Count > 0)
                SelectedSourceZone = SourceZoneViewModels[0];
        }

        public RelayCommand AddOneCommand { get; private set; }
        void OnAddOneCommand(object obj)
        {
            TargetZoneViewModels.Add(SelectedSourceZone);
            SelectedTargetZone = SelectedSourceZone;
            SourceZoneViewModels.Remove(SelectedSourceZone);

            if (SourceZoneViewModels.Count > 0)
                SelectedSourceZone = SourceZoneViewModels[0];
            
        }

        public RelayCommand RemoveOneCommand { get; private set; }
        void OnRemoveOneCommand(object obj)
        {
            SourceZoneViewModels.Add(SelectedTargetZone);
            SelectedSourceZone = SelectedTargetZone;
            TargetZoneViewModels.Remove(SelectedTargetZone);

            if (TargetZoneViewModels.Count > 0)
                SelectedTargetZone = TargetZoneViewModels[0];
        }

        public RelayCommand AddAllCommand { get; private set; }
        void OnAddAllCommand(object obj)
        {
            foreach (ZoneViewModel zoneViewModel in SourceZoneViewModels)
            {
                TargetZoneViewModels.Add(zoneViewModel);
            }
            SourceZoneViewModels.Clear();

            if (TargetZoneViewModels.Count > 0)
                SelectedTargetZone = TargetZoneViewModels[0];
        }

        public RelayCommand RemoveAllCommand { get; private set; }
        void OnRemoveAllCommand(object obj)
        {
            foreach (ZoneViewModel zoneViewModel in TargetZoneViewModels)
            {
                SourceZoneViewModels.Add(zoneViewModel);
            }
            TargetZoneViewModels.Clear();

            if (SourceZoneViewModels.Count > 0)
                SelectedSourceZone = SourceZoneViewModels[0];
        }

        bool CanAdd(object obj)
        {
            return SelectedSourceZone != null;
        }
        bool CanRemove(object obj)
        {
            return SelectedTargetZone != null;
        }

        ObservableCollection<ZoneViewModel> sourceZoneViewModels;
        public ObservableCollection<ZoneViewModel> SourceZoneViewModels
        {
            get { return sourceZoneViewModels; }
            set
            {
                sourceZoneViewModels = value;
                OnPropertyChanged("SourceZoneViewModels");
            }
        }

        ZoneViewModel selectedSourceZone;
        public ZoneViewModel SelectedSourceZone
        {
            get { return selectedSourceZone; }
            set
            {
                selectedSourceZone = value;
                OnPropertyChanged("SelectedSourceZone");
            }
        }

        ObservableCollection<ZoneViewModel> targetZoneViewModels;
        public ObservableCollection<ZoneViewModel> TargetZoneViewModels
        {
            get { return targetZoneViewModels; }
            set
            {
                targetZoneViewModels = value;
                OnPropertyChanged("TargetZoneViewModels");
            }
        }

        ZoneViewModel selectedTargetZone;
        public ZoneViewModel SelectedTargetZone
        {
            get { return selectedTargetZone; }
            set
            {
                selectedTargetZone = value;
                OnPropertyChanged("SelectedTargetZone");
            }
        }

        public Action RequestClose { get; set; }
        void OnRequestClose()
        {
            if (RequestClose != null)
                RequestClose();
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSaveCommand(object obj)
        {
            List<string> zoneIds = new List<string>();
            foreach (ZoneViewModel zoneViewModel in TargetZoneViewModels)
            {
                zoneIds.Add(zoneViewModel.ZoneNo);
            }

            clause.zone = zoneIds.ToArray();
            OnRequestClose();
        }
    }
}
