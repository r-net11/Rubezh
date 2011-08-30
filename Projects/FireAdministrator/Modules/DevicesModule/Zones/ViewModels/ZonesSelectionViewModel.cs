using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class ZonesSelectionViewModel : SaveCancelDialogContent
    {
        public ZonesSelectionViewModel()
        {
            Title = "Выбор зон";

            AddOneCommand = new RelayCommand(OnAddOne, CanAdd);
            RemoveOneCommand = new RelayCommand(OnRemoveOne, CanRemove);
            AddAllCommand = new RelayCommand(OnAddAll, CanAdd);
            RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemove);
        }

        public List<string> Zones { get; private set; }

        public void Initialize(Device device, List<string> zones, ZoneLogicState zoneLogicState)
        {
            Zones = zones;
            TargetZones = new ObservableCollection<ZoneViewModel>();
            SourceZones = new ObservableCollection<ZoneViewModel>();

            foreach (var zone in FiresecManager.DeviceConfiguration.Zones)
            {
                var zoneTypeFilter = ZoneType.Fire;

                switch (zoneLogicState)
                {
                    case ZoneLogicState.Alarm:
                    case ZoneLogicState.GuardSet:
                    case ZoneLogicState.GuardUnSet:
                    case ZoneLogicState.PCN:
                    case ZoneLogicState.Lamp:
                        zoneTypeFilter = ZoneType.Guard;
                        break;
                }

                if (zone.ZoneType != zoneTypeFilter)
                    continue;

                if ((zoneLogicState == ZoneLogicState.MPTAutomaticOn) || (zoneLogicState == ZoneLogicState.MPTOn))
                {
                    var canAdd = false;
                    var mptDevices = device.Parent.Children.FindAll(x=>x.Driver.DriverName == "Модуль пожаротушения");
                    if (mptDevices != null)
                    {
                        foreach(var mptDevice in mptDevices)
                        {
                            if (mptDevice.ZoneNo == zone.No)
                                canAdd = true;
                        }
                    }
                    if (canAdd == false)
                        continue;
                }

                if ((device.Parent.Driver.DriverName == "Прибор Рубеж-2ОП") || (device.Parent.Driver.DriverName == "USB Рубеж-2ОП"))
                {
                    var canAdd = false;
                    foreach (var guardDevice in device.Parent.Children)
                    {
                        if (guardDevice.ZoneNo == zone.No)
                            canAdd = true;
                    }
                    if (canAdd == false)
                        continue;
                }

                var zoneViewModel = new ZoneViewModel(zone);

                if (Zones.Contains(zone.No))
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

        public ObservableCollection<ZoneViewModel> SourceZones { get; private set; }

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
            foreach (var zoneViewModel in SourceZones)
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
            foreach (var zoneViewModel in TargetZones)
            {
                SourceZones.Add(zoneViewModel);
            }
            TargetZones.Clear();

            if (SourceZones.Count > 0)
                SelectedSourceZone = SourceZones[0];
        }

        bool CanAdd()
        {
            return SelectedSourceZone != null;
        }

        bool CanRemove()
        {
            return SelectedTargetZone != null;
        }

        protected override void Save()
        {
            Zones = new List<string>();
            foreach (var zoneViewModel in TargetZones)
            {
                Zones.Add(zoneViewModel.No);
            }
        }
    }
}
