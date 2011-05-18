using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure;
using System.Windows;

namespace DevicesModule.ViewModels
{
    public class ZonesViewModel : RegionViewModel
    {
        public ZonesViewModel()
        {
            AddCommand = new RelayCommand(OnAdd);
            DeleteCommand = new RelayCommand(OnDelete);
            EditCommand = new RelayCommand(OnEdit);
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

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            Zone zone = new Zone();
            zone.No = "0";
            zone.Name = "Новая зона";
            ZoneViewModel zoneViewModel = new ZoneViewModel(zone);
            Zones.Add(zoneViewModel);
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            if (SelectedZone != null)
            {
                var dialogResult = MessageBox.Show("Вы уверены, что хотите удалить зону " + SelectedZone.PresentationName, "Подтверждение", MessageBoxButton.YesNo);
                if (dialogResult == MessageBoxResult.Yes)
                    Zones.Remove(SelectedZone);
            }
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            if (SelectedZone != null)
            {
                ZoneDetailsViewModel zoneDetailsViewModel = new ZoneDetailsViewModel();
                zoneDetailsViewModel.Initialize(SelectedZone._zone);
                ServiceFactory.UserDialogs.ShowModalWindow(zoneDetailsViewModel);
                SelectedZone.Update();
            }
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
