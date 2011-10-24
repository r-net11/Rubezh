using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using DevicesModule.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace InstructionsModule.ViewModels
{
    public class InstructionDevicesViewModel : SaveCancelDialogContent
    {
        public InstructionDevicesViewModel()
        {
            Title = "Выбор устройства";
            InstructionDevices = new ObservableCollection<DeviceViewModel>();
            AvailableDevices = new ObservableCollection<DeviceViewModel>();
            InstructionDevicesList = new List<Guid>();

            AddOneCommand = new RelayCommand(OnAddOne, CanAddOne);
            RemoveOneCommand = new RelayCommand(OnRemoveOne, CanRemoveOne);
            AddAllCommand = new RelayCommand(OnAddAll, CanAddAll);
            RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);
        }

        public void Inicialize(List<Guid> instructionDevicesList)
        {
            if (instructionDevicesList.IsNotNullOrEmpty())
            {
                InstructionDevicesList = new List<Guid>(instructionDevicesList);
            }
            else
            {
                InstructionDevicesList = new List<Guid>();
            }

            UpdateDevices();
        }

        void UpdateDevices()
        {
            var availableDevices = new HashSet<Device>();
            var instructionDevices = new HashSet<Device>();

            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                if (device.Driver.IsZoneDevice)
                {
                    if (InstructionDevicesList.Contains(device.UID))
                    {
                        device.AllParents.ForEach(x => { instructionDevices.Add(x); });
                        instructionDevices.Add(device);
                    }
                    else
                    {
                        device.AllParents.ForEach(x => { availableDevices.Add(x); });
                        availableDevices.Add(device);
                    }
                }

                if (device.Driver.IsZoneLogicDevice)
                {
                    if (device.ZoneLogic != null)
                    {
                        foreach (var clause in device.ZoneLogic.Clauses)
                        {
                            if (InstructionDevicesList.Contains(device.UID))
                            {
                                device.AllParents.ForEach(x => { instructionDevices.Add(x); });
                                instructionDevices.Add(device);
                            }
                            else
                            {
                                device.AllParents.ForEach(x => { availableDevices.Add(x); });
                                availableDevices.Add(device);
                            }
                        }
                    }
                }
            }

            InstructionDevices.Clear();
            BuildTree(instructionDevices, InstructionDevices);

            AvailableDevices.Clear();
            BuildTree(availableDevices, AvailableDevices);

            if (InstructionDevices.IsNotNullOrEmpty())
            {
                CollapseChild(InstructionDevices[0]);
                ExpandChild(InstructionDevices[0]);
                SelectedInstructionDevice = InstructionDevices[0];
            }
            else
            {
                SelectedInstructionDevice = null;
            }

            if (AvailableDevices.IsNotNullOrEmpty())
            {
                CollapseChild(AvailableDevices[0]);
                ExpandChild(AvailableDevices[0]);
                SelectedAvailableDevice = AvailableDevices[0];
            }
            else
            {
                SelectedAvailableDevice = null;
            }
        }

        void BuildTree(HashSet<Device> hashSetDevices, ObservableCollection<DeviceViewModel> devices)
        {
            foreach (var device in hashSetDevices)
            {
                var deviceViewModel = new DeviceViewModel();
                deviceViewModel.Initialize(device, devices);
                deviceViewModel.IsExpanded = true;
                devices.Add(deviceViewModel);
            }

            foreach (var device in devices)
            {
                if (device.Device.Parent != null)
                {
                    var parent = devices.FirstOrDefault(x => x.Device.UID == device.Device.Parent.UID);
                    device.Parent = parent;
                    parent.Children.Add(device);
                }
            }
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

        public List<Guid> InstructionDevicesList { get; set; }
        public ObservableCollection<DeviceViewModel> AvailableDevices { get; set; }
        public ObservableCollection<DeviceViewModel> InstructionDevices { get; set; }

        DeviceViewModel _selectedAvailableDevice;
        public DeviceViewModel SelectedAvailableDevice
        {
            get { return _selectedAvailableDevice; }
            set
            {
                _selectedAvailableDevice = value;
                OnPropertyChanged("SelectedAvailableDevice");
            }
        }

        DeviceViewModel _selectedInstructionDevice;
        public DeviceViewModel SelectedInstructionDevice
        {
            get { return _selectedInstructionDevice; }
            set
            {
                _selectedInstructionDevice = value;
                OnPropertyChanged("SelectedInstructionDevice");
            }
        }

        public bool CanAddOne()
        {
            return ((SelectedAvailableDevice != null) &&
                (SelectedAvailableDevice.Driver.IsZoneDevice ||
                SelectedAvailableDevice.Driver.IsZoneLogicDevice));
        }

        public bool CanAddAll()
        {
            return (AvailableDevices.IsNotNullOrEmpty());
        }

        public bool CanRemoveOne()
        {
            return ((SelectedInstructionDevice != null) &&
                (SelectedInstructionDevice.Driver.IsZoneDevice ||
                SelectedInstructionDevice.Driver.IsZoneLogicDevice));
        }

        public bool CanRemoveAll()
        {
            return (InstructionDevices.IsNotNullOrEmpty());
        }

        public RelayCommand AddOneCommand { get; private set; }
        void OnAddOne()
        {
            InstructionDevicesList.Add(SelectedAvailableDevice.UID);
            UpdateDevices();
        }

        public RelayCommand AddAllCommand { get; private set; }
        void OnAddAll()
        {
            InstructionDevicesList.Clear();
            if (FiresecManager.DeviceConfiguration.Devices.IsNotNullOrEmpty() == false)
            {
                return;
            }
            foreach (var availableDevice in FiresecManager.DeviceConfiguration.Devices)
            {
                if ((availableDevice.Driver.IsZoneDevice) || (availableDevice.Driver.IsZoneLogicDevice))
                {
                    InstructionDevicesList.Add(availableDevice.UID);
                }
            }
            UpdateDevices();
        }

        public RelayCommand RemoveAllCommand { get; private set; }
        void OnRemoveAll()
        {
            InstructionDevicesList.Clear();
            UpdateDevices();
        }

        public RelayCommand RemoveOneCommand { get; private set; }
        void OnRemoveOne()
        {
            InstructionDevicesList.Remove(SelectedInstructionDevice.UID);
            UpdateDevices();
        }
    }
}