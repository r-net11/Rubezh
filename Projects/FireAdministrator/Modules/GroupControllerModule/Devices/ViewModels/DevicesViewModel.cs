using System.Collections.ObjectModel;
using GroupControllerModule.Models;
using Infrastructure;
using Infrastructure.Common;

namespace GroupControllerModule.ViewModels
{
    public class DevicesViewModel : RegionViewModel
    {
        public static DevicesViewModel Current { get; private set; }
        public DeviceCommandsViewModel DeviceCommandsViewModel { get; private set; }

        public DevicesViewModel()
        {
            Current = this;
            CopyCommand = new RelayCommand(OnCopy, CanCutCopy);
            CutCommand = new RelayCommand(OnCut, CanCutCopy);
            PasteCommand = new RelayCommand(OnPaste, CanPaste);
            DeviceCommandsViewModel = new DeviceCommandsViewModel(this);

            BuildTree();
            if (Devices.Count > 0)
            {
                CollapseChild(Devices[0]);
                ExpandChild(Devices[0]);
                SelectedDevice = Devices[0];
            }
        }

        public RelayCommand ConvertCommand { get; private set; }
        void OnConvert()
        {
        }

        ObservableCollection<DeviceViewModel> _devices;
        public ObservableCollection<DeviceViewModel> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                OnPropertyChanged("Devices");
            }
        }

        DeviceViewModel _selectedDevice;
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

        void BuildTree()
        {
            Devices = new ObservableCollection<DeviceViewModel>();
            var xRootDevice = XManager.DeviceConfiguration.RootDevice;
            AddDevice(xRootDevice, null);
        }

        public DeviceViewModel AddDevice(XDevice xDevice, DeviceViewModel parentDeviceViewModel)
        {
            var xDeviceViewModel = new DeviceViewModel(xDevice, Devices);
            xDeviceViewModel.Parent = parentDeviceViewModel;

            var indexOf = Devices.IndexOf(parentDeviceViewModel);
            Devices.Insert(indexOf + 1, xDeviceViewModel);

            foreach (var childDevice in xDevice.Children)
            {
                var childDeviceViewModel = AddDevice(childDevice, xDeviceViewModel);
                xDeviceViewModel.Children.Add(childDeviceViewModel);
            }

            return xDeviceViewModel;
        }

        public void CollapseChild(DeviceViewModel parentDeviceViewModel)
        {
            parentDeviceViewModel.IsExpanded = false;
            foreach (var deviceViewModel in parentDeviceViewModel.Children)
            {
                CollapseChild(deviceViewModel);
            }
        }

        public void ExpandChild(DeviceViewModel parentDeviceViewModel)
        {
            if ((parentDeviceViewModel.Device.Driver.DriverType == XDriverType.System) || (parentDeviceViewModel.Device.Driver.DriverType == XDriverType.GC))
            {
                parentDeviceViewModel.IsExpanded = true;
                foreach (var deviceViewModel in parentDeviceViewModel.Children)
                {
                    ExpandChild(deviceViewModel);
                }
            }
        }

        XDevice _deviceToCopy;
        bool _isFullCopy;

        bool CanCutCopy()
        {
            return !(SelectedDevice == null || SelectedDevice.Parent == null ||
                SelectedDevice.Driver.IsAutoCreate || SelectedDevice.Parent.Driver.AutoChild == SelectedDevice.Driver.UID);
        }

        public RelayCommand CopyCommand { get; private set; }
        void OnCopy()
        {
            _deviceToCopy = SelectedDevice.Device.Copy(_isFullCopy = false);
        }

        public RelayCommand CutCommand { get; private set; }
        void OnCut()
        {
            _deviceToCopy = SelectedDevice.Device.Copy(_isFullCopy = true);
            SelectedDevice.RemoveCommand.Execute();

            XManager.DeviceConfiguration.Update();
            ServiceFactory.SaveService.XDevicesChanged = true;
        }

        bool CanPaste()
        {
            return (SelectedDevice != null && _deviceToCopy != null && SelectedDevice.Driver.Children.Contains(_deviceToCopy.Driver.UID));
        }

        public RelayCommand PasteCommand { get; private set; }
        void OnPaste()
        {
            var pasteDevice = _deviceToCopy.Copy(_isFullCopy);
            PasteDevice(pasteDevice);
        }

        void PasteDevice(XDevice xDevice)
        {
            SelectedDevice.Device.Children.Add(xDevice);
            xDevice.Parent = SelectedDevice.Device;

            var newDevice = AddDevice(xDevice, SelectedDevice);
            SelectedDevice.Children.Add(newDevice);
            CollapseChild(newDevice);

            XManager.DeviceConfiguration.Update();
            ServiceFactory.SaveService.XDevicesChanged = true;
        }

        public override void OnShow()
        {
            var devicesMenuViewModel = new DevicesMenuViewModel(this);
            ServiceFactory.Layout.ShowMenu(devicesMenuViewModel);
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}