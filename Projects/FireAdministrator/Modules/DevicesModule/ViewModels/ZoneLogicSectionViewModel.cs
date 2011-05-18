using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using FiresecClient;

namespace DevicesModule.ViewModels
{
    public class ZoneLogicSectionViewModel : DialogContent
    {
        Firesec.ZoneLogic.clauseType clause;

        public ZoneLogicSectionViewModel()
        {
            Title = "Выбор зон";
            SaveCommand = new RelayCommand(OnSave);
            AddOneCommand = new RelayCommand(OnAddOne, CanAdd);
            RemoveOneCommand = new RelayCommand(OnRemoveOne, CanRemove);
            AddAllCommand = new RelayCommand(OnAddAll, CanAdd);
            RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemove);
        }

        public void Initialize(Firesec.ZoneLogic.clauseType clause)
        {
            this.clause = clause;
            TargetZones = new ObservableCollection<ZoneViewModel>();
            SourceZones = new ObservableCollection<ZoneViewModel>();

            foreach (Zone zone in FiresecManager.CurrentConfiguration.Zones)
            {
                ZoneViewModel zoneViewModel = new ZoneViewModel(zone);

                if (clause.zone.Contains(zone.No))
                {
                    TargetZones.Add(zoneViewModel);
                }
                else
                {
                    SourceZones.Add(zoneViewModel);
                }
            }

            if (TargetZones.Count > 0)
                SelectedTargetZone = TargetZones[0];

            if (SourceZones.Count > 0)
                SelectedSourceZone = SourceZones[0];
        }

        public RelayCommand AddOneCommand { get; private set; }
        void OnAddOne()
        {
            TargetZones.Add(SelectedSourceZone);
            SelectedTargetZone = SelectedSourceZone;
            SourceZones.Remove(SelectedSourceZone);

            if (SourceZones.Count > 0)
                SelectedSourceZone = SourceZones[0];

        }

        public RelayCommand RemoveOneCommand { get; private set; }
        void OnRemoveOne()
        {
            SourceZones.Add(SelectedTargetZone);
            SelectedSourceZone = SelectedTargetZone;
            TargetZones.Remove(SelectedTargetZone);

            if (TargetZones.Count > 0)
                SelectedTargetZone = TargetZones[0];
        }

        public RelayCommand AddAllCommand { get; private set; }
        void OnAddAll()
        {
            foreach (ZoneViewModel zoneViewModel in SourceZones)
            {
                TargetZones.Add(zoneViewModel);
            }
            SourceZones.Clear();

            if (TargetZones.Count > 0)
                SelectedTargetZone = TargetZones[0];
        }

        public RelayCommand RemoveAllCommand { get; private set; }
        void OnRemoveAll()
        {
            foreach (ZoneViewModel zoneViewModel in TargetZones)
            {
                SourceZones.Add(zoneViewModel);
            }
            TargetZones.Clear();

            if (SourceZones.Count > 0)
                SelectedSourceZone = SourceZones[0];
        }

        bool CanAdd(object obj)
        {
            return SelectedSourceZone != null;
        }
        bool CanRemove(object obj)
        {
            return SelectedTargetZone != null;
        }

        ObservableCollection<ZoneViewModel> _sourceZones;
        public ObservableCollection<ZoneViewModel> SourceZones
        {
            get { return _sourceZones; }
            set
            {
                _sourceZones = value;
                OnPropertyChanged("SourceZones");
            }
        }

        ZoneViewModel _selectedSourceZone;
        public ZoneViewModel SelectedSourceZone
        {
            get { return _selectedSourceZone; }
            set
            {
                _selectedSourceZone = value;
                OnPropertyChanged("SelectedSourceZone");
            }
        }

        ObservableCollection<ZoneViewModel> _targetZones;
        public ObservableCollection<ZoneViewModel> TargetZones
        {
            get { return _targetZones; }
            set
            {
                _targetZones = value;
                OnPropertyChanged("TargetZones");
            }
        }

        ZoneViewModel _selectedTargetZone;
        public ZoneViewModel SelectedTargetZone
        {
            get { return _selectedTargetZone; }
            set
            {
                _selectedTargetZone = value;
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
        void OnSave()
        {
            List<string> zoneIds = new List<string>();
            foreach (ZoneViewModel zoneViewModel in TargetZones)
            {
                zoneIds.Add(zoneViewModel.No);
            }

            clause.zone = zoneIds.ToArray();
            OnRequestClose();
        }
    }
}
