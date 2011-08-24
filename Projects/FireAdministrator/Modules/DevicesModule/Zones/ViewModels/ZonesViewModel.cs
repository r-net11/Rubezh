using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using System.Collections.Generic;

namespace DevicesModule.ViewModels
{
    public class ZonesViewModel : RegionViewModel
    {
        public ZoneDevicesViewModel ZoneDevices { get; set; }

        public ZonesViewModel()
        {
            AddCommand = new RelayCommand(OnAdd);
            DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
            EditCommand = new RelayCommand(OnEdit, CanEditDelete);
            DeleteAllCommand = new RelayCommand(OnDeleteAll, CanDeleteAll);
            DeleteAllEmptyCommand = new RelayCommand(OnDeleteAllEmpty, CanDeleteAll);
            ZoneDevices = new ZoneDevicesViewModel();
        }

        public void Initialize()
        {
            Zones = new ObservableCollection<ZoneViewModel>(
                from zone in FiresecManager.DeviceConfiguration.Zones
                orderby (int.Parse(zone.No))
                select new ZoneViewModel(zone));

            if (Zones.Count > 0)
                SelectedZone = Zones[0];
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

                if (value != null)
                {
                    ZoneDevices.Initialize(value.No);
                }

                OnPropertyChanged("SelectedZone");
            }
        }

        bool CanEditDelete(object obj)
        {
            return SelectedZone != null;
        }

        bool CanDeleteAll(object obj)
        {
            return Zones.Count > 0;
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var zoneDetailsViewModel = new ZoneDetailsViewModel();
            zoneDetailsViewModel.Initialize();
            if (ServiceFactory.UserDialogs.ShowModalWindow(zoneDetailsViewModel))
            {
                FiresecManager.DeviceConfiguration.Zones.Add(zoneDetailsViewModel._zone);
                var zoneViewModel = new ZoneViewModel(zoneDetailsViewModel._zone);
                Zones.Add(zoneViewModel);
                DevicesModule.HasChanges = true;
            }
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            var dialogResult = MessageBox.Show("Вы уверены, что хотите удалить зону " + SelectedZone.PresentationName, "Подтверждение", MessageBoxButton.YesNo);
            if (dialogResult == MessageBoxResult.Yes)
            {
                //FiresecManager.DeviceConfiguration.Zones.Remove(SelectedZone.Zone);
                Zones.Remove(SelectedZone);
                ZoneDevices.DropDevicesZoneNo();
                ZoneDevices.Clear();
                if (Zones.Count > 0)
                    SelectedZone = Zones[0];
                DevicesModule.HasChanges = true;
            }
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            var zoneDetailsViewModel = new ZoneDetailsViewModel();
            zoneDetailsViewModel.Initialize(SelectedZone.Zone);
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(zoneDetailsViewModel);
            if (result)
            {
                SelectedZone.Zone = zoneDetailsViewModel._zone;
                SelectedZone.Update();
                DevicesModule.HasChanges = true;
            }
        }

        public RelayCommand DeleteAllCommand { get; private set; }
        void OnDeleteAll()
        {
            var dialogResult = MessageBox.Show("Вы уверены, что хотите удалить все зоны ?", "Подтверждение", MessageBoxButton.YesNo);
            if (dialogResult == MessageBoxResult.Yes)
            {
                FiresecManager.DeviceConfiguration.Zones.Clear();
                Zones.Clear();
                foreach (var device in FiresecManager.DeviceConfiguration.Devices)
                {
                    device.ZoneNo = null;
                }
                ZoneDevices.Clear();
                DevicesModule.HasChanges = true;
            }
        }

        public RelayCommand DeleteAllEmptyCommand { get; private set; }
        void OnDeleteAllEmpty()
        {
            var dialogResult = MessageBox.Show("Вы уверены, что хотите удалить все пустые зоны ?", "Подтверждение", MessageBoxButton.YesNo);
            if (dialogResult == MessageBoxResult.Yes)
            {
                var devices = FiresecManager.DeviceConfiguration.Devices;
                var emptyZones = new List<ZoneViewModel>();
                foreach (var zone in Zones)
                {
                    var findDevice = devices.FirstOrDefault(x => ((x.Driver.IsZoneDevice) && (x.ZoneNo == zone.No)));
                    if (findDevice == null)
                    {
                        emptyZones.Add(zone);
                    }
                }

                foreach (var emptyZone in emptyZones)
                {
                    FiresecManager.DeviceConfiguration.Zones.Remove(emptyZone.Zone);
                    Zones.Remove(emptyZone);
                }

                if (Zones.Count > 0)
                    SelectedZone = Zones[0];
                DevicesModule.HasChanges = true;
            }
        }

        public override void OnShow()
        {
            SelectedZone = SelectedZone;
            var zonesMenuViewModel = new ZonesMenuViewModel(AddCommand, DeleteCommand, EditCommand, DeleteAllCommand, DeleteAllEmptyCommand);
            ServiceFactory.Layout.ShowMenu(zonesMenuViewModel);
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}