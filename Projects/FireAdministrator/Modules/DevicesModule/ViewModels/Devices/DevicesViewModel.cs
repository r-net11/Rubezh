using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using FiresecAPI.Models;

namespace DevicesModule.ViewModels
{
    public class DevicesViewModel : RegionViewModel
    {
        public DevicesViewModel()
        {
            CopyCommand = new RelayCommand(OnCopy, CanCutCopy);
            CutCommand = new RelayCommand(OnCut, CanCutCopy);
            PasteCommand = new RelayCommand(OnPaste, CanPaste);
        }

        public void Initialize()
        {
            BuildTree();
            if (Devices.Count > 0)
            {
                CollapseChild(Devices[0]);
                ExpandChild(Devices[0]);
                SelectedDevice = Devices[0];
            }
        }

        #region DeviceSection
        public List<DeviceViewModel> AllDevices;

        public void FillAllDevices()
        {
            AllDevices = new List<DeviceViewModel>();
            AddChildPlainDevices(Devices[0]);
        }

        void AddChildPlainDevices(DeviceViewModel parentViewModel)
        {
            AllDevices.Add(parentViewModel);
            foreach (var childViewModel in parentViewModel.Children)
            {
                AddChildPlainDevices(childViewModel);
            }
        }

        public void Select(string id)
        {
            FillAllDevices();

            var deviceViewModel = AllDevices.FirstOrDefault(x => x.Id == id);
            if (deviceViewModel != null)
            {
                deviceViewModel.ExpantToThis();
            }
            SelectedDevice = deviceViewModel;
        }
        #endregion

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
                {
                    value.ExpantToThis();
                }
                OnPropertyChanged("SelectedDevice");
            }
        }

        void BuildTree()
        {
            Devices = new ObservableCollection<DeviceViewModel>();
            var device = FiresecManager.DeviceConfiguration.RootDevice;
            AddDevice(device, null);
        }

        DeviceViewModel AddDevice(Device device, DeviceViewModel parentDeviceViewModel)
        {
            DeviceViewModel deviceViewModel = new DeviceViewModel();
            deviceViewModel.Parent = parentDeviceViewModel;
            deviceViewModel.Initialize(device, Devices);

            var indexOf = Devices.IndexOf(parentDeviceViewModel);
            Devices.Insert(indexOf + 1, deviceViewModel);

            foreach (var childDevice in device.Children)
            {
                var childDeviceViewModel = AddDevice(childDevice, deviceViewModel);
                deviceViewModel.Children.Add(childDeviceViewModel);
            }

            return deviceViewModel;
        }

        void CollapseChild(DeviceViewModel parentDeviceViewModel)
        {
            parentDeviceViewModel.IsExpanded = false;

            foreach (var deviceViewModel in parentDeviceViewModel.Children)
            {
                CollapseChild(deviceViewModel);
            }
        }

        void ExpandChild(DeviceViewModel parentDeviceViewModel)
        {
            if (parentDeviceViewModel.Device.Driver.Category != DeviceCategoryType.Device)
            {
                parentDeviceViewModel.IsExpanded = true;
                foreach (var deviceViewModel in parentDeviceViewModel.Children)
                {
                    ExpandChild(deviceViewModel);
                }
            }
        }

        Device _deviceToCopy;
        bool _isFullCopy;

        bool CanCutCopy(object obj)
        {
            if (SelectedDevice == null)
                return false;

            if (SelectedDevice.Driver.IsAutoCreate)
                return false;

            if (SelectedDevice.Driver.DriverName == "Компьютер")
                return false;

            return true;
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
            FiresecManager.DeviceConfiguration.Update();
        }

        bool CanPaste(object obj)
        {
            if (SelectedDevice == null)
                return false;

            if (_deviceToCopy != null)
            {
                if (SelectedDevice.Driver.AvaliableChildren.Contains(_deviceToCopy.Driver.Id))
                {
                    return true;
                }
            }

            return false;
        }

        public RelayCommand PasteCommand { get; private set; }
        void OnPaste()
        {
            var pasteDevice = _deviceToCopy.Copy(_isFullCopy);
            SelectedDevice.Device.Children.Add(pasteDevice);
            pasteDevice.Parent = SelectedDevice.Device;
            var newDevice = AddDevice(pasteDevice, SelectedDevice);
            CollapseChild(newDevice);

            FiresecManager.DeviceConfiguration.Update();
        }

        public override void OnShow()
        {
            DevicesMenuViewModel devicesMenuViewModel = new DevicesMenuViewModel(CopyCommand, CutCommand, PasteCommand);
            ServiceFactory.Layout.ShowMenu(devicesMenuViewModel);
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}
