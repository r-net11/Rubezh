using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using ServerFS2;

namespace ClientFS2.ViewModels
{
    public class DevicesViewModel : DialogViewModel
    {
        private DeviceViewModel _rootDevice;
        private DeviceViewModel _selectedDevice;

        public DevicesViewModel()
        {
            GetParametersCommand = new RelayCommand(OnGetParameters, CanGetParameters);
            SetParametersCommand = new RelayCommand(OnSetParameters, CanSetParameters);
            BuildTree();
            OnPropertyChanged("RootDevices");
        }

        public DevicesViewModel(Device device)
        {
            BuildTree(device);
            if (RootDevice != null)
            {
                RootDevice.IsExpanded = true;
                SelectedDevice = RootDevice;
            }
            OnPropertyChanged("RootDevices");
        }

        public DeviceViewModel SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                if (value != null)
                    value.ExpantToThis();
                OnPropertyChanged("SelectedDevice");
            }
        }

        public DeviceViewModel RootDevice
        {
            get { return _rootDevice; }
            private set
            {
                _rootDevice = value;
                OnPropertyChanged("RootDevice");
            }
        }

        public DeviceViewModel[] RootDevices
        {
            get { return new[] {RootDevice}; }
        }

        public RelayCommand GetParametersCommand { get; private set; }
        public RelayCommand SetParametersCommand { get; private set; }

        private void BuildTree(Device device)
        {
            RootDevice = AddDeviceInternal(device, null);
        }

        private void BuildTree()
        {
            RootDevice = AddDeviceInternal(ConfigurationManager.DeviceConfiguration.RootDevice, null);
        }

        private void OnGetParameters()
        {
            ServerHelper.GetDeviceParameters(SelectedDevice.Device);
            SelectedDevice.Device.OnAUParametersChanged();
        }

        private bool CanGetParameters()
        {
            return SelectedDevice != null;
        }

        private void OnSetParameters()
        {
            ServerHelper.SetDeviceParameters(SelectedDevice.Device);
        }

        private bool CanSetParameters()
        {
            return SelectedDevice != null;
        }

        private static DeviceViewModel AddDeviceInternal(Device device,
                                                         TreeItemViewModel<DeviceViewModel> parentDeviceViewModel)
        {
            var deviceViewModel = new DeviceViewModel(device);
            if (parentDeviceViewModel != null)
                parentDeviceViewModel.Children.Add(deviceViewModel);

            foreach (Device childDevice in device.Children)
                AddDeviceInternal(childDevice, deviceViewModel);
            return deviceViewModel;
        }
    }
}