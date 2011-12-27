using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

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

            Zones = new ObservableCollection<ZoneViewModel>(
                from zone in FiresecManager.DeviceConfiguration.Zones
                orderby zone.No
                select new ZoneViewModel(zone));

            if (Zones.Count > 0)
                SelectedZone = Zones[0];
        }

        public ObservableCollection<ZoneViewModel> Zones { get; private set; }

        ZoneViewModel _selectedZone;
        public ZoneViewModel SelectedZone
        {
            get { return _selectedZone; }
            set
            {
                _selectedZone = value;
                if (value != null)
                    ZoneDevices.Initialize(value.No.Value);
                else
                    ZoneDevices.Clear();

                OnPropertyChanged("SelectedZone");
            }
        }

        bool CanEditDelete()
        {
            return SelectedZone != null;
        }

        bool CanDeleteAll()
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
                Zones.Add(new ZoneViewModel(zoneDetailsViewModel._zone));

                DevicesModule.HasChanges = true;
            }
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            var dialogResult = DialogBox.DialogBox.Show("Вы уверены, что хотите удалить зону " + SelectedZone.PresentationName, MessageBoxButton.YesNo);
            if (dialogResult == MessageBoxResult.Yes)
            {
                FiresecManager.DeviceConfiguration.Zones.Remove(SelectedZone.Zone);
                FiresecManager.DeviceConfiguration.Devices.ForEach(x => { if ((x.ZoneNo != null) && (x.ZoneNo.Value == SelectedZone.Zone.No)) x.ZoneNo = null; });
                Zones.Remove(SelectedZone);
                SelectFirstZone();
                ZoneDevices.UpdateAvailableDevices();
                DevicesModule.HasChanges = true;
            }
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            var zoneDetailsViewModel = new ZoneDetailsViewModel();
            zoneDetailsViewModel.Initialize(SelectedZone.Zone);
            if (ServiceFactory.UserDialogs.ShowModalWindow(zoneDetailsViewModel))
            {
                SelectedZone.Zone = zoneDetailsViewModel._zone;
                SelectedZone.Update();

                DevicesModule.HasChanges = true;
            }
        }

        public RelayCommand DeleteAllCommand { get; private set; }
        void OnDeleteAll()
        {
            var dialogResult = DialogBox.DialogBox.Show("Вы уверены, что хотите удалить все зоны ?", MessageBoxButton.YesNo);
            if (dialogResult == MessageBoxResult.Yes)
            {
                FiresecManager.DeviceConfiguration.Zones.Clear();
                FiresecManager.DeviceConfiguration.Devices.ForEach(x => x.ZoneNo = null);
                Zones.Clear();
                SelectedZone = null;

                DevicesModule.HasChanges = true;
            }
        }

        public RelayCommand DeleteAllEmptyCommand { get; private set; }
        void OnDeleteAllEmpty()
        {
            var dialogResult = DialogBox.DialogBox.Show("Вы уверены, что хотите удалить все пустые зоны ?", MessageBoxButton.YesNo);
            if (dialogResult == MessageBoxResult.Yes)
            {
                var emptyZones = new List<ZoneViewModel>(
                    Zones.Where(zone => FiresecManager.DeviceConfiguration.Devices.Any(x => x.Driver.IsZoneDevice && x.ZoneNo == zone.No) == false)
                );
                foreach (var emptyZone in emptyZones)
                {
                    FiresecManager.DeviceConfiguration.Zones.Remove(emptyZone.Zone);
                    Zones.Remove(emptyZone);
                }

                SelectFirstZone();

                DevicesModule.HasChanges = true;
            }
        }

        void SelectFirstZone()
        {
            if (Zones.Count > 0)
                SelectedZone = Zones[0];
            else
                SelectedZone = null;
        }

        public override void OnShow()
        {
            SelectedZone = SelectedZone;
            ServiceFactory.Layout.ShowMenu(new ZonesMenuViewModel(this));
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}