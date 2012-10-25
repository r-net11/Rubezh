using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Common;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using FiresecClient;
using GKModule.ViewModels;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;

namespace GKModule.ViewModels
{
    public class XLibraryViewModel : ViewPartViewModel
    {
        public XLibraryViewModel()
        {
            AddXDeviceCommand = new RelayCommand(OnAddXDevice);
            RemoveXDeviceCommand = new RelayCommand(OnRemoveXDevice, CanRemoveXDevice);
            AddXStateCommand = new RelayCommand(OnAddXState, CanAddXState);
            RemoveXStateCommand = new RelayCommand(OnRemoveXState, CanRemoveState);

            //foreach (var libraryXDevice in FiresecManager.XDeviceLibraryConfiguration.XDevices)
            //{
            //    var driver = FiresecClient.FiresecManager.XDrivers.First(x => x.UID == libraryXDevice.XDriverId);
            //    if (driver != null)
            //    {
            //        libraryXDevice.XDriver = driver;
            //    }
            //    else
            //    {
            //        Logger.Error("XLibraryViewModel.Initialize driver = null " + libraryXDevice.XDriverId.ToString());
            //    }
            //}
            //var xdevices = from LibraryXDevice libraryXDevice in FiresecManager.XDeviceLibraryConfiguration.XDevices orderby libraryXDevice.XDriver.DeviceClassName select libraryXDevice;
            XDevices = new ObservableCollection<XDeviceViewModel>();
            //foreach (var xdevice in xdevices)
            //{
            //    var xdeviceViewModel = new XDeviceViewModel(xdevice);
            //    XDevices.Add(xdeviceViewModel);
            //}
            //SelectedXDevice = XDevices.FirstOrDefault();
        }

        ObservableCollection<XDeviceViewModel> _xdevices;
        public ObservableCollection<XDeviceViewModel> XDevices
        {
            get { return _xdevices; }
            set
            {
                _xdevices = value;
                OnPropertyChanged("XDevices");
            }
        }

        XDeviceViewModel _selectedXDevice;
        public XDeviceViewModel SelectedXDevice
        {
            get { return _selectedXDevice; }
            set
            {
                var oldSelectedXStateType = XStateType.No;
                if (SelectedXState != null)
                {
                    oldSelectedXStateType = SelectedXState.XState.XStateType;
                }
                _selectedXDevice = value;
                OnPropertyChanged("SelectedXDevice");

                if (value != null)
                {
                    var driver = FiresecManager.XDrivers.FirstOrDefault(x => x.UID == SelectedXDevice.LibraryXDevice.XDriverId);
                    XStates = new ObservableCollection<XStateViewModel>();
                    var libraryXStates = from LibraryXState libraryXState in SelectedXDevice.LibraryXDevice.XStates orderby libraryXState.XStateType descending select libraryXState;
                    foreach (var libraryXState in libraryXStates)
                    {
                        var stateViewModel = new XStateViewModel(libraryXState, driver);
                        XStates.Add(stateViewModel);
                    }
                    SelectedXState = XStates.FirstOrDefault(x => x.XState.XStateType == oldSelectedXStateType);
                    if (SelectedXState == null)
                        SelectedXState = XStates.FirstOrDefault();
                }
                else
                {
                    SelectedXState = null;
                }
            }
        }

        public RelayCommand AddXDeviceCommand { get; private set; }
        void OnAddXDevice()
        {
            var addDeviceViewModel = new XDeviceDetailsViewModel();
            if (DialogService.ShowModalWindow(addDeviceViewModel))
            {
                //FiresecManager.XDeviceLibraryConfiguration.XDevices.Add(addDeviceViewModel.SelectedXDevice.LibraryXDevice);
                XDevices.Add(addDeviceViewModel.SelectedXDevice);
                SelectedXDevice = XDevices.LastOrDefault();
                ServiceFactory.SaveService.LibraryChanged = true;
            }
        }

        public RelayCommand RemoveXDeviceCommand { get; private set; }
        void OnRemoveXDevice()
        {
            FiresecManager.XDeviceLibraryConfiguration.XDevices.Remove(SelectedXDevice.LibraryXDevice);
            XDevices.Remove(SelectedXDevice);
            SelectedXDevice = XDevices.FirstOrDefault();
            ServiceFactory.SaveService.LibraryChanged = true;
        }
        bool CanRemoveXDevice()
        {
            return SelectedXDevice != null;
        }

        ObservableCollection<XStateViewModel> _xstates;
        public ObservableCollection<XStateViewModel> XStates
        {
            get { return _xstates; }
            set
            {
                _xstates = value;
                OnPropertyChanged("XStates");
            }
        }

        XStateViewModel _selectedXState;
        public XStateViewModel SelectedXState
        {
            get { return _selectedXState; }
            set
            {
                _selectedXState = value;
                OnPropertyChanged("SelectedXState");
                OnPropertyChanged("XDeviceControl");
            }
        }

        public RelayCommand AddXStateCommand { get; private set; }
        void OnAddXState()
        {
            var xstateDetailsViewModel = new XStateDetailsViewModel(SelectedXDevice.LibraryXDevice);
            if (DialogService.ShowModalWindow(xstateDetailsViewModel))
            {
                SelectedXDevice.LibraryXDevice.XStates.Add(xstateDetailsViewModel.SelectedXState.XState);
                XStates.Add(xstateDetailsViewModel.SelectedXState);
                SelectedXState = XStates.LastOrDefault();
                ServiceFactory.SaveService.LibraryChanged = true;
            }
        }
        bool CanAddXState()
        {
            return SelectedXDevice != null;
        }

        public RelayCommand RemoveXStateCommand { get; private set; }
        void OnRemoveXState()
        {
            SelectedXDevice.LibraryXDevice.XStates.Remove(SelectedXState.XState);
            XStates.Remove(SelectedXState);
            SelectedXState = XStates.FirstOrDefault();
            ServiceFactory.SaveService.LibraryChanged = true;
        }
        bool CanRemoveState()
        {
            return (SelectedXState != null && SelectedXState.XState.XStateType != XStateType.No);
        }

        public DeviceControls.DeviceControl XDeviceControl
        {
            get
            {
                if (SelectedXDevice == null)
                    return null;
                if (SelectedXState == null)
                    return null;

                var xdeviceControl = new DeviceControls.DeviceControl()
                {
                    DriverId = SelectedXDevice.LibraryXDevice.XDriverId
                };
                xdeviceControl.XStateType = SelectedXState.XState.XStateType;

                xdeviceControl.XUpdate();
                return xdeviceControl;
            }
        }

        public bool IsDebug
        {
            get { return ServiceFactory.AppSettings.IsDebug; }
        }
    }
}
