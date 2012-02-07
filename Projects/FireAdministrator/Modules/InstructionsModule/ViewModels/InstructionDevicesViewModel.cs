using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevicesModule.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace InstructionsModule.ViewModels
{
    public class InstructionDevicesViewModel : SaveCancelDialogContent
    {
        public InstructionDevicesViewModel(List<Guid> instructionDevicesList)
        {
            AddOneCommand = new RelayCommand(OnAddOne, CanAddOne);
            RemoveOneCommand = new RelayCommand(OnRemoveOne, CanRemoveOne);
            AddAllCommand = new RelayCommand(OnAddAll, CanAddAll);
            RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);
            Title = "Выбор устройства";

            InstructionDevicesList = new List<Guid>(instructionDevicesList);

            UpdateDevices();
        }

        void UpdateDevices()
        {
            var availableDevices = new HashSet<Device>();
            var instructionDevices = new HashSet<Device>();

            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                if (!device.Driver.Children.IsNotNullOrEmpty())
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

            InstructionDevices = new ObservableCollection<InstructionDeviceViewModel>();
            BuildTree(instructionDevices, InstructionDevices);

            AvailableDevices = new ObservableCollection<InstructionDeviceViewModel>();
            BuildTree(availableDevices, AvailableDevices);


            SelectedInstructionDevice = null;
            SelectedAvailableDevice = null;

            if (InstructionDevices.IsNotNullOrEmpty())
            {
                CollapseChild(InstructionDevices[0]);
                ExpandChild(InstructionDevices[0]);
                SelectedInstructionDevice = InstructionDevices[0];
            }

            if (AvailableDevices.IsNotNullOrEmpty())
            {
                CollapseChild(AvailableDevices[0]);
                ExpandChild(AvailableDevices[0]);
                SelectedAvailableDevice = AvailableDevices[0];
            }

            OnPropertyChanged("InstructionDevices");
            OnPropertyChanged("AvailableDevices");
        }

        void BuildTree(HashSet<Device> hashSetDevices, ObservableCollection<InstructionDeviceViewModel> devices)
        {
            foreach (var device in hashSetDevices)
            {
                var deviceViewModel = new InstructionDeviceViewModel(device, devices);
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

        void CollapseChild(InstructionDeviceViewModel parentDeviceViewModel)
        {
            parentDeviceViewModel.IsExpanded = false;

            foreach (var deviceViewModel in parentDeviceViewModel.Children)
            {
                CollapseChild(deviceViewModel);
            }
        }

        void ExpandChild(InstructionDeviceViewModel parentDeviceViewModel)
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

        void ExpandAllChild(InstructionDeviceViewModel parentDeviceViewModel)
        {
            parentDeviceViewModel.IsExpanded = true;
            foreach (var deviceViewModel in parentDeviceViewModel.Children)
            {
                ExpandAllChild(deviceViewModel);
            }
        }

        public List<Guid> InstructionDevicesList { get; set; }
        public ObservableCollection<InstructionDeviceViewModel> AvailableDevices { get; private set; }
        public ObservableCollection<InstructionDeviceViewModel> InstructionDevices { get; private set; }

        InstructionDeviceViewModel _selectedAvailableDevice;
        public InstructionDeviceViewModel SelectedAvailableDevice
        {
            get { return _selectedAvailableDevice; }
            set
            {
                _selectedAvailableDevice = value;
                OnPropertyChanged("SelectedAvailableDevice");
            }
        }

        InstructionDeviceViewModel _selectedInstructionDevice;
        public InstructionDeviceViewModel SelectedInstructionDevice
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
            return (SelectedAvailableDevice != null && !SelectedAvailableDevice.Device.Driver.Children.IsNotNullOrEmpty());
        }

        public bool CanAddAll()
        {
            return (AvailableDevices.IsNotNullOrEmpty());
        }

        public bool CanRemoveOne()
        {
            return ((SelectedInstructionDevice != null) && (!SelectedInstructionDevice.Device.Driver.Children.IsNotNullOrEmpty()));
        }

        public bool CanRemoveAll()
        {
            return (InstructionDevices.IsNotNullOrEmpty());
        }

        public RelayCommand AddOneCommand { get; private set; }
        void OnAddOne()
        {
            //ExpandChild(SelectedAvailableDevice);
            InstructionDevicesList.Add(SelectedAvailableDevice.UID);
            UpdateDevices();
        }

        public RelayCommand AddAllCommand { get; private set; }
        void OnAddAll()
        {
            ExpandAllChild(AvailableDevices[0]);
            foreach (var deviceViewModel in AvailableDevices)
            {
                if (!deviceViewModel.Children.IsNotNullOrEmpty())
                    InstructionDevicesList.Add(deviceViewModel.UID);
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