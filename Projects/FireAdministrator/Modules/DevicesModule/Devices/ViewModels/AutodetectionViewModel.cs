using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class AutodetectionViewModel : DialogContent
    {
        public ObservableCollection<DeviceViewModel> DeviceViewModels { get; set; }
        List<AutoDetectedDeviceViewModel> allDevices;

        public AutodetectionViewModel()
        {
            ContinueCommand = new RelayCommand(OnContinue);
            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);
        }

        public void Initialize(DeviceConfiguration autodetectedDeviceConfiguration)
        {
            allDevices = new List<AutoDetectedDeviceViewModel>();
            Devices = new List<AutoDetectedDeviceViewModel>();
            var root = AddDevice(autodetectedDeviceConfiguration.RootDevice, null);
            Devices.Add(root);
        }

        AutoDetectedDeviceViewModel AddDevice(Device device, AutoDetectedDeviceViewModel parentDeviceViewModel)
        {
            var deviceViewModel = new AutoDetectedDeviceViewModel(device);

            foreach (var childDevice in device.Children)
            {
                // устаревшие устройства
                if (childDevice.Driver == null)
                {
                    continue;
                }
                var childDeviceViewModel = AddDevice(childDevice, deviceViewModel);
                deviceViewModel.Children.Add(childDeviceViewModel);
            }

            allDevices.Add(deviceViewModel);
            return deviceViewModel;
        }

        public List<AutoDetectedDeviceViewModel> Devices { get; set; }

        public RelayCommand ContinueCommand { get; private set; }
        void OnContinue()
        {
            Close(true);
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            AddFromTree(Devices[0]);
            Close(false);
        }

        void AddFromTree(AutoDetectedDeviceViewModel parentAutoDetectedDevice)
        {
            foreach (var autodetectedDevice in parentAutoDetectedDevice.Children)
            {
                if (autodetectedDevice.IsSelected)
                {
                    AddAutoDevice(autodetectedDevice);
                    AddFromTree(autodetectedDevice);
                }
            }
        }

        void AddAutoDevice(AutoDetectedDeviceViewModel autoDetectedDevice)
        {
            var device = autoDetectedDevice.Device;
            var parentDevice = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == device.Parent.Id);
            parentDevice.Children.Add(device);

            var parentDeviceViewModel = DeviceViewModels.FirstOrDefault(x => x.UID == parentDevice.UID);

            var deviceViewModel = new DeviceViewModel();
            deviceViewModel.Initialize(device, parentDeviceViewModel.Source);
            deviceViewModel.Parent = parentDeviceViewModel;
            parentDeviceViewModel.Children.Add(deviceViewModel);

            parentDeviceViewModel.Update();

            FiresecManager.DeviceConfiguration.Update();
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}
