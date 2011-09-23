using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;

namespace DevicesModule.ViewModels
{
    public class DevicesViewModel : RegionViewModel
    {
        public DevicesViewModel()
        {
            CopyCommand = new RelayCommand(OnCopy, CanCutCopy);
            CutCommand = new RelayCommand(OnCut, CanCutCopy);
            PasteCommand = new RelayCommand(OnPaste, CanPaste);
            AutoDetectCommand = new RelayCommand(OnAutoDetect, CanAutoDetect);
            ReadDeviceCommand = new RelayCommand(OnReadDevice, CanReadDevice);
            WriteDeviceCommand = new RelayCommand(OnWriteDevice, CanWriteDevice);
            WriteAllDeviceCommand = new RelayCommand(OnWriteAllDevice, CanWriteAllDevice);
            SynchronizeDeviceCommand = new RelayCommand(OnSynchronizeDevice, CanSynchronizeDevice);
            RebootDeviceCommand = new RelayCommand(OnRebootDevice, CanRebootDevice);
            UpdateSoftCommand = new RelayCommand(OnUpdateSoft, CanUpdateSoft);
            GetDescriptionCommand = new RelayCommand(OnGetDescription, CanGetDescription);
            GetDeveceJournalCommand = new RelayCommand(OnGetDeveceJournal, CanGetDeveceJournal);
            SetPasswordCommand = new RelayCommand(OnSetPassword, CanSetPassword);
            BindMsCommand = new RelayCommand(OnBindMs, CanBindMs);
            ShowAdditionalPropertiesCommand = new RelayCommand(OnShowAdditionalProperties, CanShowAdditionalProperties);
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

        #region DeviceSelection

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

        public void Select(Guid deviceUID)
        {
            FillAllDevices();

            var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
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
            var deviceViewModel = new DeviceViewModel();
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

        bool CanCutCopy()
        {
            if (SelectedDevice == null)
                return false;

            if (SelectedDevice.Driver.IsAutoCreate)
                return false;

            if (SelectedDevice.Driver.DriverType == DriverType.Computer)
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
            DevicesModule.HasChanges = true;
        }

        bool CanPaste()
        {
            if (SelectedDevice == null)
                return false;

            if (_deviceToCopy != null)
            {
                if (SelectedDevice.Driver.AvaliableChildren.Contains(_deviceToCopy.Driver.UID))
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
            DevicesModule.HasChanges = true;
        }

        public RelayCommand AutoDetectCommand { get; private set; }
        void OnAutoDetect()
        {
            AutoSearchHelper.Run(SelectedDevice);
        }

        bool CanAutoDetect()
        {
            return true;
            //return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanAutoDetect));
        }

        public RelayCommand ReadDeviceCommand { get; private set; }
        void OnReadDevice()
        {
            FiresecManager.DeviceReadConfiguration(SelectedDevice.Device.UID);
        }

        bool CanReadDevice()
        {
            return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanReadDatabase));
        }

        public RelayCommand WriteDeviceCommand { get; private set; }
        void OnWriteDevice()
        {
            FiresecManager.DeviceWriteConfiguration(SelectedDevice.Device.UID);
        }

        bool CanWriteDevice()
        {
            return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanWriteDatabase));
        }

        public RelayCommand WriteAllDeviceCommand { get; private set; }
        void OnWriteAllDevice()
        {
            FiresecManager.WriteAllDeviceConfiguration();
        }

        bool CanWriteAllDevice()
        {
            return Devices.Count > 0;
        }

        public RelayCommand SynchronizeDeviceCommand { get; private set; }
        void OnSynchronizeDevice()
        {
            FiresecManager.SynchronizeDevice(SelectedDevice.Device.UID);
        }

        bool CanSynchronizeDevice()
        {
            return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanSynchonize));
        }

        public RelayCommand RebootDeviceCommand { get; private set; }
        void OnRebootDevice()
        {
            FiresecManager.DeviceRestart(SelectedDevice.Device.UID);
        }

        bool CanRebootDevice()
        {
            return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanReboot));
        }

        public RelayCommand UpdateSoftCommand { get; private set; }
        void OnUpdateSoft()
        {
            FirmwareUpdateHelper.Run(SelectedDevice.Device);
        }

        bool CanUpdateSoft()
        {
            return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanUpdateSoft));
        }

        public RelayCommand GetDescriptionCommand { get; private set; }
        void OnGetDescription()
        {
            GetInformationHelper.Run(SelectedDevice.Device);
        }

        bool CanGetDescription()
        {
            return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanGetDescription));
        }

        public RelayCommand GetDeveceJournalCommand { get; private set; }
        void OnGetDeveceJournal()
        {
            //string content = "<head>" + "\n" + "<meta http-equiv='Content-Type' content='text/html;charset=UTF-8'>" + "\n" + "</head>" +
            //    "<h2>Журнал событий устройства Рубеж-2AM 1.30</h2>";

            //var deviceJournalViewModel = new DeviceJournalViewModel(content);
            //ServiceFactory.UserDialogs.ShowModalWindow(deviceJournalViewModel);
            //return;

            DeviceJournalHelper.Run(SelectedDevice.Device);
        }

        bool CanGetDeveceJournal()
        {
            return true;
        }

        public RelayCommand SetPasswordCommand { get; private set; }
        void OnSetPassword()
        {
            var setPasswordViewModel = new SetPasswordViewModel(SelectedDevice.Device.UID);
            ServiceFactory.UserDialogs.ShowModalWindow(setPasswordViewModel);
        }

        bool CanSetPassword()
        {
            return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanSetPassword));
        }

        public RelayCommand BindMsCommand { get; private set; }
        void OnBindMs()
        {
            GetSerialsHelper.Run(SelectedDevice.Device);
            return;

            bool canBind = ((SelectedDevice != null) && (SelectedDevice.Device.Driver.DriverType == (DriverType.MS_1 | DriverType.MS_2)));

            //var bindMsViewModel = new BindMsViewModel(SelectedDevice.Device);
            //ServiceFactory.UserDialogs.ShowModalWindow(bindMsViewModel);
        }

        bool CanBindMs()
        {
            return true;
            return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.DriverType == (DriverType.MS_1 | DriverType.MS_2)));
        }

        public RelayCommand ShowAdditionalPropertiesCommand { get; private set; }
        void OnShowAdditionalProperties()
        {
            var additionalPropertiesViewModel = new AdditionalPropertiesViewModel(SelectedDevice.Device.UID);
            ServiceFactory.UserDialogs.ShowModalWindow(additionalPropertiesViewModel);
        }

        bool CanShowAdditionalProperties()
        {
            return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanSetPassword));
        }

        public override void OnShow()
        {
            var devicesMenuViewModel = new DevicesMenuViewModel()
                {
                    CopyCommand = CopyCommand,
                    CutCommand = CutCommand,
                    PasteCommand = PasteCommand,
                    AutoDetectCommand = AutoDetectCommand,
                    ReadDeviceCommand = ReadDeviceCommand,
                    WriteDeviceCommand = WriteDeviceCommand,
                    WriteAllDeviceCommand = WriteAllDeviceCommand,
                    SynchronizeDeviceCommand = SynchronizeDeviceCommand,
                    RebootDeviceCommand = RebootDeviceCommand,
                    UpdateSoftCommand = UpdateSoftCommand,
                    GetDescriptionCommand = GetDescriptionCommand,
                    SetPasswordCommand = SetPasswordCommand,
                    GetDeveceJournalCommand = GetDeveceJournalCommand,
                    BindMsCommand = BindMsCommand
                };
            ServiceFactory.Layout.ShowMenu(devicesMenuViewModel);
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}