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
        public ZoneDevicesViewModel ZoneDevices { get; set; }

        public ZonesViewModel()
        {
            AddCommand = new RelayCommand(OnAdd);
            DeleteCommand = new RelayCommand(OnDelete);
            EditCommand = new RelayCommand(OnEdit);
            ZoneDevices = new ZoneDevicesViewModel();
        }

        public void Initialize()
        {
            Zones = new ObservableCollection<ZoneViewModel>(
                from zone in FiresecManager.Configuration.Zones
                orderby (Convert.ToInt32(zone.No))
                select new ZoneViewModel(zone));
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
            var maxNo = (from zone in FiresecManager.Configuration.Zones select Convert.ToInt32(zone.No)).Max();
            newZone.No = (maxNo + 1).ToString();

            ZoneDetailsViewModel zoneDetailsViewModel = new ZoneDetailsViewModel(newZone);
            var result = ServiceFactory.UserDialogs.ShowModalWindow(zoneDetailsViewModel);
            if (result)
            {
                FiresecManager.Configuration.Zones.Add(newZone);
                ZoneViewModel zoneViewModel = new ZoneViewModel(newZone);
                Zones.Add(zoneViewModel);
            }
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            if (SelectedZone != null)
            {
                var dialogResult = MessageBox.Show("Вы уверены, что хотите удалить зону " + SelectedZone.PresentationName, "Подтверждение", MessageBoxButton.YesNo);
                if (dialogResult == MessageBoxResult.Yes)
                {
                    FiresecManager.Configuration.Zones.Remove(SelectedZone.Zone);
                    Zones.Remove(SelectedZone);
                }
            }
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            if (SelectedZone != null)
            {
                ZoneDetailsViewModel zoneDetailsViewModel = new ZoneDetailsViewModel(SelectedZone.Zone);
                bool result = ServiceFactory.UserDialogs.ShowModalWindow(zoneDetailsViewModel);
                SelectedZone.Update();
            }
        }

        public override void Dispose()
        {
        }
    }
}
