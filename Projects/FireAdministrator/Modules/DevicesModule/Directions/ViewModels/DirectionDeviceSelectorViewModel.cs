using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using System.Collections.ObjectModel;

namespace DevicesModule.ViewModels
{
    public class DirectionDeviceSelectorViewModel : SaveCancelDialogContent
    {
        public DirectionDeviceSelectorViewModel()
        {
            Title = "Выбор устройства";
        }

        public void Initialize(Direction direction, bool isRm)
        {
            string driverName = isRm ? "Релейный исполнительный модуль РМ-1" : "Кнопка разблокировки автоматики ШУЗ в направлении";

            var devices = new HashSet<Device>();

            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                {
                    if (device.Driver.DriverName == driverName)
                    {
                        bool canAdd = device.Parent.Children.Any(x => (x.Driver.IsZoneDevice) && (direction.Zones.Contains(x.ZoneNo)));
                        if (canAdd)
                        {
                            device.AllParents.ForEach(x => { devices.Add(x); });
                            devices.Add(device);
                        }
                    }
                }
            }

            Devices = new ObservableCollection<DeviceViewModel>();
            foreach (var device in devices)
            {
                var deviceViewModel = new DeviceViewModel();
                deviceViewModel.Initialize(device, Devices);
                deviceViewModel.IsExpanded = true;
                Devices.Add(deviceViewModel);
            }

            foreach (var device in Devices)
            {
                if (device.Device.Parent != null)
                {
                    var parent = Devices.FirstOrDefault(x => x.Device.Id == device.Device.Parent.Id);
                    device.Parent = parent;
                    parent.Children.Add(device);
                }
            }

            SelectedDevice = Devices.FirstOrDefault(x => x.HasChildren == false);
        }

        public ObservableCollection<DeviceViewModel> Devices { get; private set; }

        DeviceViewModel _selectedDevice;
        public DeviceViewModel SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }

        protected override bool CanSave()
        {
            if (SelectedDevice != null)
            {
                return (SelectedDevice.HasChildren == false);
            }
            return false;
        }
    }
}