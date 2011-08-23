using System.Collections.ObjectModel;
using Common;
using DevicesModule.ViewModels;
using Infrastructure.Common;

namespace InstructionsModule.ViewModels
{
    public class InstructionDevicesViewModel : DialogContent
    {
        public InstructionDevicesViewModel()
        {
            AddDeviceCommand = new RelayCommand(OnAddDevice, CanAddAvailableDevice);
            RemoveZoneCommand = new RelayCommand(OnRemoveZone, CanRemoveDevice);
            AddAllDeviceCommand = new RelayCommand(OnAddAllDevice, CanAddAllAvailableDevice);
            RemoveAllDeviceCommand = new RelayCommand(OnRemoveAllDevice, CanRemoveAllDevice);
            SaveCommand = new RelayCommand(OnSave, CanSaveInstruction);
            CancelCommand = new RelayCommand(OnCancel);
        }

        public ObservableCollection<DeviceViewModel> AvailableDevices { get; set; }

        public ObservableCollection<DeviceViewModel> InstructionDevices { get; set; }

        public DeviceViewModel SelectedAvailableDevice { get; set; }

        public DeviceViewModel SelectedInstructionDevice { get; set; }

        public bool CanAddAvailableDevice(object obj)
        {
            return (SelectedAvailableDevice != null);
        }

        public bool CanAddAllAvailableDevice(object obj)
        {
            return (AvailableDevices.IsNotNullOrEmpty());
        }

        public bool CanRemoveDevice(object obj)
        {
            return (SelectedInstructionDevice != null);
        }

        public bool CanRemoveAllDevice(object obj)
        {
            return (InstructionDevices.IsNotNullOrEmpty());
        }

        public bool CanSaveInstruction(object obj)
        {
            return (InstructionDevices.IsNotNullOrEmpty());
        }

        public RelayCommand AddDeviceCommand { get; private set; }
        void OnAddDevice()
        {
            if (CanAddAvailableDevice(null))
            {
                InstructionDevices.Add(SelectedAvailableDevice);
                AvailableDevices.Remove(SelectedAvailableDevice);
                if (AvailableDevices.Count != 0)
                {
                    SelectedAvailableDevice = AvailableDevices[0];
                }
                if (InstructionDevices.Count != 0)
                {
                    SelectedInstructionDevice = InstructionDevices[0];
                }
            }
        }

        public RelayCommand AddAllDeviceCommand { get; private set; }
        void OnAddAllDevice()
        {
            if (CanAddAllAvailableDevice(null))
            {
                foreach (var availableDevices in AvailableDevices)
                {
                    InstructionDevices.Add(availableDevices);
                }
                AvailableDevices.Clear();
                SelectedAvailableDevice = null;
                if (InstructionDevices.IsNotNullOrEmpty())
                {
                    SelectedInstructionDevice = InstructionDevices[0];
                }
            }
        }

        public RelayCommand RemoveAllDeviceCommand { get; private set; }
        void OnRemoveAllDevice()
        {
            if (CanRemoveAllDevice(null))
            {
                foreach (var instructionZone in InstructionDevices)
                {
                    AvailableDevices.Add(instructionZone);
                }
                InstructionDevices.Clear();
                SelectedInstructionDevice = null;
                if (AvailableDevices.IsNotNullOrEmpty())
                {
                    SelectedAvailableDevice = AvailableDevices[0];
                }
            }
        }

        public RelayCommand RemoveZoneCommand { get; private set; }
        void OnRemoveZone()
        {
            if (CanRemoveDevice(null))
            {
                AvailableDevices.Add(SelectedInstructionDevice);
                InstructionDevices.Remove(SelectedInstructionDevice);
                if (AvailableDevices.Count != 0)
                {
                    SelectedAvailableDevice = AvailableDevices[0];
                }
                if (InstructionDevices.Count != 0)
                {
                    SelectedInstructionDevice = InstructionDevices[0];
                }
            }
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            Close(false);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }

        //void BuildTree()
        //{
        //    Devices = new ObservableCollection<DeviceViewModel>();
        //    var device = FiresecManager.DeviceConfiguration.RootDevice;
        //    AddDevice(device, null);
        //}

        //DeviceViewModel AddDevice(Device device, DeviceViewModel parentDeviceViewModel)
        //{
        //    var deviceViewModel = new DeviceViewModel();
        //    deviceViewModel.Parent = parentDeviceViewModel;
        //    deviceViewModel.Initialize(device, Devices);

        //    var indexOf = Devices.IndexOf(parentDeviceViewModel);
        //    Devices.Insert(indexOf + 1, deviceViewModel);

        //    foreach (var childDevice in device.Children)
        //    {
        //        var childDeviceViewModel = AddDevice(childDevice, deviceViewModel);
        //        deviceViewModel.Children.Add(childDeviceViewModel);
        //    }

        //    return deviceViewModel;
        //}

        //void CollapseChild(DeviceViewModel parentDeviceViewModel)
        //{
        //    parentDeviceViewModel.IsExpanded = false;

        //    foreach (var deviceViewModel in parentDeviceViewModel.Children)
        //    {
        //        CollapseChild(deviceViewModel);
        //    }
        //}

        //void ExpandChild(DeviceViewModel parentDeviceViewModel)
        //{
        //    if (parentDeviceViewModel.Device.Driver.Category != DeviceCategoryType.Device)
        //    {
        //        parentDeviceViewModel.IsExpanded = true;
        //        foreach (var deviceViewModel in parentDeviceViewModel.Children)
        //        {
        //            ExpandChild(deviceViewModel);
        //        }
        //    }
        //}
    }
}