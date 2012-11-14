using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public class InstructionDevicesViewModel : SaveCancelDialogViewModel
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
            AvailableDevices = new ObservableCollection<XDevice>();
            InstructionDevices = new ObservableCollection<XDevice>();
            SelectedAvailableDevice = null;
            SelectedInstructionDevice = null;

            foreach (var device in XManager.DeviceConfiguration.Devices)
            {
                if (device.IsRealDevice && device.Driver.IsDeviceOnShleif)
                {
                    if (InstructionDevicesList.Contains(device.UID))
                        InstructionDevices.Add(device);
                    else
                        AvailableDevices.Add(device);
                }
            }

			SelectedInstructionDevice = InstructionDevices.FirstOrDefault();
			SelectedAvailableDevice = AvailableDevices.FirstOrDefault();

            OnPropertyChanged("InstructionDevices");
            OnPropertyChanged("AvailableDevices");
        }

        public List<Guid> InstructionDevicesList { get; set; }
        public ObservableCollection<XDevice> AvailableDevices { get; private set; }
        public ObservableCollection<XDevice> InstructionDevices { get; private set; }

        XDevice _selectedAvailableDevice;
        public XDevice SelectedAvailableDevice
        {
            get { return _selectedAvailableDevice; }
            set
            {
                _selectedAvailableDevice = value;
                OnPropertyChanged("SelectedAvailableDevice");
            }
        }

        XDevice _selectedInstructionDevice;
        public XDevice SelectedInstructionDevice
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
            return (SelectedAvailableDevice != null);
        }

        public bool CanAddAll()
        {
            return (AvailableDevices.Count > 0);
        }

        public bool CanRemoveOne()
        {
            return (SelectedInstructionDevice != null);
        }

        public bool CanRemoveAll()
        {
            return (InstructionDevices.Count > 0);
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
            foreach (var deviceViewModel in AvailableDevices)
            {
                InstructionDevicesList.Add(deviceViewModel.UID);
            }
            UpdateDevices();
        }

        public RelayCommand RemoveOneCommand { get; private set; }
        void OnRemoveOne()
        {
            InstructionDevicesList.Remove(SelectedInstructionDevice.UID);
            UpdateDevices();
        }

        public RelayCommand RemoveAllCommand { get; private set; }
        void OnRemoveAll()
        {
            InstructionDevicesList.Clear();
            UpdateDevices();
        }
    }
}