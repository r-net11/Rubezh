using Infrastructure.Common;
using GroupControllerModule.Models;
using FiresecClient;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.ObjectModel;

namespace GroupControllerModule.ViewModels
{
    public class GroupControllerViewModel : RegionViewModel
    {
        ConfigurationConverter ConfigurationConverter;

        public GroupControllerViewModel()
        {
            ConvertCommand = new RelayCommand(OnConvert);
        }

        public RelayCommand ConvertCommand { get; private set; }
        void OnConvert()
        {
            ConfigurationConverter = new ConfigurationConverter();
            ConfigurationConverter.Convert();
            BuildTree();
        }

        ObservableCollection<GCDeviceViewModel> _devices;
        public ObservableCollection<GCDeviceViewModel> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                OnPropertyChanged("Devices");
            }
        }

        GCDeviceViewModel _selectedDevice;
        public GCDeviceViewModel SelectedDevice
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
            Devices = new ObservableCollection<GCDeviceViewModel>();
            var GCRootDevice = ConfigurationConverter.GCDeviceConfiguration.RootDevice;
            AddDevice(GCRootDevice, null);
        }

        public GCDeviceViewModel AddDevice(GCDevice gCDevice, GCDeviceViewModel parentDeviceViewModel)
        {
            var gCDeviceViewModel = new GCDeviceViewModel(gCDevice, Devices);
            gCDeviceViewModel.Parent = parentDeviceViewModel;

            var indexOf = Devices.IndexOf(parentDeviceViewModel);
            Devices.Insert(indexOf + 1, gCDeviceViewModel);

            foreach (var childDevice in gCDevice.Children)
            {
                var childDeviceViewModel = AddDevice(childDevice, gCDeviceViewModel);
                gCDeviceViewModel.Children.Add(childDeviceViewModel);
            }

            return gCDeviceViewModel;
        }
    }
}