using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using System.Collections.ObjectModel;
using FiresecClient;

namespace DevicesModule.ViewModels
{
    public class ZonesViewModel : RegionViewModel
    {
        public ZonesViewModel()
        {
            AddZoneCommand = new RelayCommand(OnAddZone);
        }

        public void Initialize()
        {
            Zones = new ObservableCollection<ZoneViewModel>();

            foreach (Zone zone in FiresecManager.CurrentConfiguration.Zones)
            {
                ZoneViewModel zoneViewModel = new ZoneViewModel(zone);
                Zones.Add(zoneViewModel);
            }
        }

        ObservableCollection<ZoneViewModel> _zones;
        public ObservableCollection<ZoneViewModel> Zones
        {
            get { return _zones; }
            set
            {
                _zones = value;
                OnPropertyChanged("Zones");
            }
        }

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

        public RelayCommand AddZoneCommand { get; private set; }
        void OnAddZone()
        {
            Zone zone = new Zone();
            zone.No = "0";
            zone.Name = "новая зона";
            ZoneViewModel zoneViewModel = new ZoneViewModel(zone);
            Zones.Add(zoneViewModel);
        }

        public void Save()
        {
            CurrentConfiguration currentConfiguration = new CurrentConfiguration();
            currentConfiguration.Zones = new List<Zone>();
            foreach (ZoneViewModel zoneViewModel in Zones)
            {
                Zone zone = new Zone();
                zone.No = zoneViewModel.No;
                zone.Name = zoneViewModel.Name;
                zone.Description = zoneViewModel.Description;
                zone.DetectorCount = zoneViewModel.DetectorCount;
                zone.EvacuationTime = zoneViewModel.EvacuationTime;
                currentConfiguration.Zones.Add(zone);
            }
        }

        public override void Dispose()
        {
        }
    }
}
