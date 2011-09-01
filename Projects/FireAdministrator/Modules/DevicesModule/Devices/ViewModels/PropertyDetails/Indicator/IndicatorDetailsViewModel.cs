using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class IndicatorDetailsViewModel : SaveCancelDialogContent
    {
        Device _indicatorDevice;
        List<string> _zones;

        public IndicatorDetailsViewModel()
        {
            Title = "Свойства индикатора";
            ShowZonesCommand = new RelayCommand(OnShowZones);
            ShowDevicesCommand = new RelayCommand(OnShowDevices);
            ResetDeviceCommand = new RelayCommand(OnResetDevice);
        }

        public void Initialize(Device device)
        {
            _zones = new List<string>();
            _indicatorDevice = device;

            if (device.IndicatorLogic == null)
                return;

            switch (device.IndicatorLogic.IndicatorLogicType)
            {
                case IndicatorLogicType.Zone:
                    IsZone = true;
                    if (device.IndicatorLogic.Zones != null)
                        _zones = device.IndicatorLogic.Zones;
                    break;

                case IndicatorLogicType.Device:
                    IsDevice = true;
                    OnColor = device.IndicatorLogic.OnColor;
                    OffColor = device.IndicatorLogic.OffColor;
                    FailureColor = device.IndicatorLogic.FailureColor;
                    ConnectionColor = device.IndicatorLogic.ConnectionColor;
                    SelectedDevice = device.IndicatorLogic.Device;
                    break;
            }
        }

        bool _isZone;
        public bool IsZone
        {
            get { return _isZone; }
            set
            {
                _isZone = value;
                OnPropertyChanged("IsZone");
            }
        }

        bool _isDevice;
        public bool IsDevice
        {
            get { return _isDevice; }
            set
            {
                _isDevice = value;
                OnPropertyChanged("IsDevice");
            }
        }

        public string PresenrationZones
        {
            get
            {
                string presenrationZones = "";
                for (int i = 0; i < _zones.Count; i++)
                {
                    if (i > 0)
                        presenrationZones += ",";
                    presenrationZones += _zones[i];
                }
                return presenrationZones;
            }
        }

        Device _selectedDevice;
        public Device SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }

        public List<IndicatorColorType> Colors
        {
            get { return Enum.GetValues(typeof(IndicatorColorType)).Cast<IndicatorColorType>().ToList(); }
        }

        IndicatorColorType _onColor;
        public IndicatorColorType OnColor
        {
            get { return _onColor; }
            set
            {
                _onColor = value;
                OnPropertyChanged("OnColor");
            }
        }

        IndicatorColorType _offColor;
        public IndicatorColorType OffColor
        {
            get { return _offColor; }
            set
            {
                _offColor = value;
                OnPropertyChanged("OffColor");
            }
        }

        IndicatorColorType _failureColor;
        public IndicatorColorType FailureColor
        {
            get { return _failureColor; }
            set
            {
                _failureColor = value;
                OnPropertyChanged("FailureColor");
            }
        }

        IndicatorColorType _connectionColor;
        public IndicatorColorType ConnectionColor
        {
            get { return _connectionColor; }
            set
            {
                _connectionColor = value;
                OnPropertyChanged("ConnectionColor");
            }
        }

        public RelayCommand ShowZonesCommand { get; private set; }
        void OnShowZones()
        {
            var indicatorZoneSelectionViewModel = new IndicatorZoneSelectionViewModel(_zones);
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(indicatorZoneSelectionViewModel);
            if (result)
            {
                _zones = indicatorZoneSelectionViewModel.Zones;
                OnPropertyChanged("PresenrationZones");
            }
        }

        public RelayCommand ShowDevicesCommand { get; private set; }
        void OnShowDevices()
        {
            var indicatorDeviceSelectionViewModel = new IndicatorDeviceSelectionViewModel();
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(indicatorDeviceSelectionViewModel);
            if (result)
            {
                SelectedDevice = indicatorDeviceSelectionViewModel.SelectedDevice.Device;
            }
        }

        public RelayCommand ResetDeviceCommand { get; private set; }
        void OnResetDevice()
        {
            SelectedDevice = null;
        }

        protected override void Save(ref bool cancel)
        {
            _indicatorDevice.IndicatorLogic = new IndicatorLogic();

            if (IsZone)
            {
                _indicatorDevice.IndicatorLogic.IndicatorLogicType = IndicatorLogicType.Zone;
                _indicatorDevice.IndicatorLogic.Zones = _zones;
            }

            if (IsDevice)
            {
                _indicatorDevice.IndicatorLogic.IndicatorLogicType = IndicatorLogicType.Device;
                _indicatorDevice.IndicatorLogic.Device = SelectedDevice;
                _indicatorDevice.IndicatorLogic.DeviceUID = SelectedDevice.UID;
                _indicatorDevice.IndicatorLogic.OnColor = OnColor;
                _indicatorDevice.IndicatorLogic.OffColor = OffColor;
                _indicatorDevice.IndicatorLogic.FailureColor = FailureColor;
                _indicatorDevice.IndicatorLogic.ConnectionColor = ConnectionColor;
            }
        }
    }
}