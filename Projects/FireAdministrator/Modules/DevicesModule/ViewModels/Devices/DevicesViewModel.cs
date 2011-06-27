using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecClient;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.IO;
using FiresecApi;
using FiresecClient.Models;
using Infrastructure;
using Firesec.ZoneLogic;

namespace DevicesModule.ViewModels
{
    public class DevicesViewModel : RegionViewModel
    {
        public DevicesViewModel()
        {
            CopyCommand = new RelayCommand(OnCopy, CanCopy);
            CutCommand = new RelayCommand(OnCut, CanCut);
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
                OnPropertyChanged("SelectedDevice");
            }
        }

        void BuildTree()
        {
            Devices = new ObservableCollection<DeviceViewModel>();
            var device = FiresecManager.Configuration.RootDevice;
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
            if (parentDeviceViewModel.Device.Driver.Category != DeviceCategory.Device)
            {
                parentDeviceViewModel.IsExpanded = true;
                foreach (var deviceViewModel in parentDeviceViewModel.Children)
                {
                    ExpandChild(deviceViewModel);
                }
            }
        }

        bool CanCopy(object obj)
        {
            return true;
        }

        public RelayCommand CopyCommand { get; private set; }
        void OnCopy()
        {
            deviceToCopy = CopyDevice(SelectedDevice.Device);
        }

        Device deviceToCopy;

        Device CopyDevice(Device originDevice)
        {
            Device newDevice = new Device();
            newDevice.Driver = originDevice.Driver;
            newDevice.Address = originDevice.Address;
            newDevice.Description = originDevice.Description;
            newDevice.ZoneNo = originDevice.ZoneNo;

            if (true)
            {
                newDevice.DatabaseId = originDevice.DatabaseId;
            }

            newDevice.ZoneLogic = new Firesec.ZoneLogic.expr();
            List<clauseType> clauses = new List<clauseType>();
            if ((originDevice.ZoneLogic != null) && (originDevice.ZoneLogic.clause != null))
            {
                foreach (var clause in originDevice.ZoneLogic.clause)
                {
                    clauseType copyClause = new clauseType();
                    copyClause.joinOperator = clause.joinOperator;
                    copyClause.operation = clause.operation;
                    copyClause.state = clause.state;
                    copyClause.zone = (string[])clause.zone.Clone();
                    clauses.Add(copyClause);
                }

                newDevice.ZoneLogic.clause = clauses.ToArray();
            }

            List<Property> copyProperties = new List<Property>();
            foreach (var property in originDevice.Properties)
            {
                Property copyProperty = new Property();
                copyProperty.Name = property.Name;
                copyProperty.Value = property.Value;
                copyProperties.Add(copyProperty);
            }
            newDevice.Properties = copyProperties;

            newDevice.Children = new List<Device>();
            foreach (var childDevice in originDevice.Children)
            {
                Device newChildDevice = CopyDevice(childDevice);
                newChildDevice.Parent = newDevice;
                newDevice.Children.Add(newChildDevice);
            }

            return newDevice;
        }

        bool CanCut(object obj)
        {
            return true;
        }

        public RelayCommand CutCommand { get; private set; }
        void OnCut()
        {
            deviceToCopy = CopyDevice(SelectedDevice.Device);
            SelectedDevice.RemoveCommand.Execute();

            FiresecManager.Configuration.Update();
        }

        bool CanPaste(object obj)
        {
            return true;
        }

        public RelayCommand PasteCommand { get; private set; }
        void OnPaste()
        {
            var pasteDevice = CopyDevice(deviceToCopy);
            SelectedDevice.Device.Children.Add(pasteDevice);
            pasteDevice.Parent = SelectedDevice.Device;
            var newDevice = AddDevice(pasteDevice, SelectedDevice);
            CollapseChild(newDevice);

            FiresecManager.Configuration.Update();
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
