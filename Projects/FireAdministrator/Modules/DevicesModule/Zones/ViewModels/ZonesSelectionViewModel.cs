using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure;

namespace DevicesModule.ViewModels
{
    [SaveSizeAttribute]
    public class ZonesSelectionViewModel : SaveCancelDialogContent
    {
        public List<ulong> Zones { get; private set; }

        public ZonesSelectionViewModel(Device device, List<ulong> zones, ZoneLogicState zoneLogicState)
        {
            Title = "Выбор зон";

            AddOneCommand = new RelayCommand(OnAddOne, CanAdd);
            RemoveOneCommand = new RelayCommand(OnRemoveOne, CanRemove);
            AddAllCommand = new RelayCommand(OnAddAll, CanAdd);
            RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemove);

            Zones = zones;
            TargetZones = new ObservableCollection<ZoneViewModel>();
            SourceZones = new ObservableCollection<ZoneViewModel>();

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

            foreach (var zone in FiresecManager.DeviceConfiguration.Zones)
            {
                var zoneViewModel = new ZoneViewModel(zone);

                if (zone.ZoneType != zoneTypeFilter)
                {
                    SourceZones.Add(zoneViewModel);
                    continue;
                }

                if ((zoneLogicState == ZoneLogicState.MPTAutomaticOn) || (zoneLogicState == ZoneLogicState.MPTOn))
                {
                    if (device.Parent.Children.Any(x => x.Driver.DriverType == DriverType.MPT && x.ZoneNo == zone.No) == false)
                    {
                        SourceZones.Add(zoneViewModel);
                        continue;
                    }
                }

                if ((device.Parent.Driver.DriverType == DriverType.Rubezh_2OP) || (device.Parent.Driver.DriverType == DriverType.USB_Rubezh_2OP))
                {
                    if (device.Parent.Children.Any(x => x.ZoneNo == zone.No) == false)
                    {
                        SourceZones.Add(zoneViewModel);
                        continue;
                    }
                }

                if (Zones.Contains(zone.No))
                    TargetZones.Add(zoneViewModel);
                else
                    SourceZones.Add(zoneViewModel);
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

        public ObservableCollection<ZoneViewModel> TargetZones { get; private set; }

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

        protected override void Save(ref bool cancel)
        {
            Zones = new List<ulong>(TargetZones.Select(x => x.No));
        }
    }
}