using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using FiresecAPI.Models;

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
            ZoneDevices = new ZoneDevicesViewModel();
        }

        public void Initialize()
        {
            Zones = new ObservableCollection<ZoneViewModel>(
                from zone in FiresecManager.DeviceConfiguration.Zones
                orderby (Convert.ToInt32(zone.No))
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

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            Zone newZone = new Zone();
            newZone.Name = "Новая зона";
            var maxNo = (from zone in FiresecManager.DeviceConfiguration.Zones select Convert.ToInt32(zone.No)).Max();
            newZone.No = (maxNo + 1).ToString();

            ZoneDetailsViewModel zoneDetailsViewModel = new ZoneDetailsViewModel(newZone);
            var result = ServiceFactory.UserDialogs.ShowModalWindow(zoneDetailsViewModel);
            if (result)
            {
                FiresecManager.DeviceConfiguration.Zones.Add(newZone);
                ZoneViewModel zoneViewModel = new ZoneViewModel(newZone);
                Zones.Add(zoneViewModel);
                DevicesModule.HasChanges = true;
            }
        }

        bool CanEditDelete(object obj)
        {
            return SelectedZone != null;
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            if (CanEditDelete(null))
            {
                var dialogResult = MessageBox.Show("Вы уверены, что хотите удалить зону " + SelectedZone.PresentationName, "Подтверждение", MessageBoxButton.YesNo);
                if (dialogResult == MessageBoxResult.Yes)
                {
                    FiresecManager.DeviceConfiguration.Zones.Remove(SelectedZone.Zone);
                    Zones.Remove(SelectedZone);
                    DevicesModule.HasChanges = true;
                }
            }
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            if (CanEditDelete(null))
            {
                ZoneDetailsViewModel zoneDetailsViewModel = new ZoneDetailsViewModel(SelectedZone.Zone);
                bool result = ServiceFactory.UserDialogs.ShowModalWindow(zoneDetailsViewModel);
                SelectedZone.Update();
                DevicesModule.HasChanges = true;
            }
        }

        public override void OnShow()
        {
            SelectedZone = SelectedZone;
            ZonesMenuViewModel zonesMenuViewModel = new ZonesMenuViewModel(AddCommand, DeleteCommand, EditCommand);
            ServiceFactory.Layout.ShowMenu(zonesMenuViewModel);
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}
