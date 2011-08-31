using System.Collections.ObjectModel;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class DirectionViewModel : BaseViewModel
    {
        public DirectionViewModel(Direction direction)
        {
            AddZoneCommand = new RelayCommand(OnAddZone, CanAdd);
            RemoveZoneCommand = new RelayCommand(OnRemoveZone, CanRemove);

            Direction = direction;
            Initialize();
        }

        public Direction Direction { get; private set; }

        void Initialize()
        {
            Zones = new ObservableCollection<ZoneViewModel>();
            SourceZones = new ObservableCollection<ZoneViewModel>();

            foreach (var zone in FiresecManager.DeviceConfiguration.Zones)
            {
                var zoneViewModel = new ZoneViewModel(zone);
                if (Direction.Zones.Contains(zone.No))
                {
                    Zones.Add(zoneViewModel);
                }
                else
                {
                    SourceZones.Add(zoneViewModel);
                }
            }

            if (Zones.Count > 0)
                SelectedZone = Zones[0];

            if (SourceZones.Count > 0)
                SelectedSourceZone = SourceZones[0];
        }

        public void Update()
        {
            OnPropertyChanged("Direction");
        }

        public ObservableCollection<ZoneViewModel> Zones { get; private set; }

        ZoneViewModel _selectedZone;
        public ZoneViewModel SelectedZone
        {
            get { return _selectedZone; }
            set
            {
                _selectedZone = value;
                OnPropertyChanged("SelectedZone");
            }
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

        bool CanAdd()
        {
            return SelectedSourceZone != null;
        }

        public RelayCommand AddZoneCommand { get; private set; }
        void OnAddZone()
        {
            int oldIndex = SourceZones.IndexOf(SelectedSourceZone);

            Direction.Zones.Add(SelectedSourceZone.No);
            Zones.Add(SelectedSourceZone);
            SourceZones.Remove(SelectedSourceZone);

            if (SourceZones.Count > 0)
            {
                int newIndex = System.Math.Min(oldIndex, SourceZones.Count - 1);
                SelectedSourceZone = SourceZones[newIndex];
            }
        }

        bool CanRemove()
        {
            return SelectedZone != null;
        }

        public RelayCommand RemoveZoneCommand { get; private set; }
        void OnRemoveZone()
        {
            int oldIndex = Zones.IndexOf(SelectedZone);

            Direction.Zones.Remove(SelectedZone.No);
            SourceZones.Add(SelectedZone);
            Zones.Remove(SelectedZone);

            if (Zones.Count > 0)
            {
                int newIndex = System.Math.Min(oldIndex, Zones.Count - 1);
                SelectedZone = Zones[newIndex];
            }
        }
    }
}