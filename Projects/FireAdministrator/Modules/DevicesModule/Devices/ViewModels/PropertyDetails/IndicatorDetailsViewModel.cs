using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class IndicatorDetailsViewModel : DialogContent
    {
        public IndicatorDetailsViewModel()
        {
            Title = "Свойства индикатора";

            AddOneCommand = new RelayCommand(OnAddOne, CanAdd);
            RemoveOneCommand = new RelayCommand(OnRemoveOne, CanRemove);
            AddAllCommand = new RelayCommand(OnAddAll, CanAdd);
            RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemove);

            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);
        }

        Device _device;

        public void Initialize(Device device)
        {
            _device = device;

            InitializeDevices();

            if (device.IndicatorLogic == null)
                return;

            Zones = new ObservableCollection<string>();

            switch (device.IndicatorLogic.IndicatorLogicType)
            {
                case IndicatorLogicType.Zone:
                    IsZone = true;

                    foreach (var zone in device.IndicatorLogic.Zones)
                        Zones.Add(zone);

                    InitializeZones(device.IndicatorLogic.Zones);
                    break;

                case IndicatorLogicType.Device:
                    IsDevice = true;

                    DeviceId = device.IndicatorLogic.DeviceUID;
                    OnColor = device.IndicatorLogic.OnColor;
                    OffColor = device.IndicatorLogic.OffColor;
                    FailureColor = device.IndicatorLogic.FailureColor;
                    ConnectionColor = device.IndicatorLogic.ConnectionColor;

                    InitializeDevices();
                    SelectedDevice = Devices.FirstOrDefault(x => x.Device.UID == DeviceId);
                    break;
            }
        }

        void InitializeDevices()
        {
            var devices = new HashSet<Device>();

            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                if (device.Driver.DriverName == "Выход")
                    continue;

                if ((device.Driver.IsOutDevice) || (device.Driver.IsZoneLogicDevice)
                    || (device.Driver.DriverName == "Технологическая адресная метка АМ1-Т")
                    || (device.Driver.DriverName == "Насос")
                    || (device.Driver.DriverName == "Жокей-насос")
                    || (device.Driver.DriverName == "Компрессор")
                    || (device.Driver.DriverName == "Дренажный насос")
                    || (device.Driver.DriverName == "Насос компенсации утечек")
                    )
                {
                    device.AllParents.ForEach(x => { devices.Add(x); });
                    devices.Add(device);
                }
            }

            Devices = new ObservableCollection<DeviceViewModel>();
            foreach (var device in devices)
            {
                var deviceViewModel = new DeviceViewModel();
                deviceViewModel.Initialize(device, Devices);
                deviceViewModel.IsExpanded = true;
                Devices.Add(deviceViewModel);
            }

            foreach (var device in Devices)
            {
                if (device.Device.Parent != null)
                {
                    var parent = Devices.FirstOrDefault(x => x.Device.Id == device.Device.Parent.Id);
                    device.Parent = parent;
                    parent.Children.Add(device);
                }
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

        string _logic;
        public string Logic
        {
            get { return _logic; }
            set
            {
                _logic = value;
                OnPropertyChanged("Logic");
            }
        }

        public ObservableCollection<string> Zones { get; private set; }

        void InitializeZones(List<string> zones)
        {
            TargetZones = new ObservableCollection<ZoneViewModel>();
            SourceZones = new ObservableCollection<ZoneViewModel>();

            foreach (Zone zone in FiresecManager.DeviceConfiguration.Zones)
            {
                var zoneViewModel = new ZoneViewModel(zone);

                if (Zones.Contains(zone.No))
                {
                    TargetZones.Add(zoneViewModel);
                }
                else
                {
                    SourceZones.Add(zoneViewModel);
                }
            }

            if (TargetZones.Count > 0)
                SelectedTargetZone = TargetZones[0];

            if (SourceZones.Count > 0)
                SelectedSourceZone = SourceZones[0];
        }

        ObservableCollection<ZoneViewModel> _sourceZones;
        public ObservableCollection<ZoneViewModel> SourceZones
        {
            get { return _sourceZones; }
            set
            {
                _sourceZones = value;
                OnPropertyChanged("SourceZones");
            }
        }

        ZoneViewModel _selectedSourceZone;
        public ZoneViewModel SelectedSourceZone
        {
            get { return _selectedSourceZone; }
            set
            {
                _selectedSourceZone = value;
                OnPropertyChanged("SelectedSourceZone");
            }
        }

        ObservableCollection<ZoneViewModel> _targetZones;
        public ObservableCollection<ZoneViewModel> TargetZones
        {
            get { return _targetZones; }
            set
            {
                _targetZones = value;
                OnPropertyChanged("TargetZones");
            }
        }

        ZoneViewModel _selectedTargetZone;
        public ZoneViewModel SelectedTargetZone
        {
            get { return _selectedTargetZone; }
            set
            {
                _selectedTargetZone = value;
                OnPropertyChanged("SelectedTargetZone");
            }
        }

        public RelayCommand AddOneCommand { get; private set; }
        void OnAddOne()
        {
            TargetZones.Add(SelectedSourceZone);
            SelectedTargetZone = SelectedSourceZone;
            SourceZones.Remove(SelectedSourceZone);

            if (SourceZones.Count > 0)
                SelectedSourceZone = SourceZones[0];
        }

        public RelayCommand RemoveOneCommand { get; private set; }
        void OnRemoveOne()
        {
            SourceZones.Add(SelectedTargetZone);
            SelectedSourceZone = SelectedTargetZone;
            TargetZones.Remove(SelectedTargetZone);

            if (TargetZones.Count > 0)
                SelectedTargetZone = TargetZones[0];
        }

        public RelayCommand AddAllCommand { get; private set; }
        void OnAddAll()
        {
            foreach (var zoneViewModel in SourceZones)
            {
                TargetZones.Add(zoneViewModel);
            }
            SourceZones.Clear();

            if (TargetZones.Count > 0)
                SelectedTargetZone = TargetZones[0];
        }

        public RelayCommand RemoveAllCommand { get; private set; }
        void OnRemoveAll()
        {
            foreach (var zoneViewModel in TargetZones)
            {
                SourceZones.Add(zoneViewModel);
            }
            TargetZones.Clear();

            if (SourceZones.Count > 0)
                SelectedSourceZone = SourceZones[0];
        }

        bool CanAdd(object obj)
        {
            return SelectedSourceZone != null;
        }

        bool CanRemove(object obj)
        {
            return SelectedTargetZone != null;
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

        string _deviceId;
        public string DeviceId
        {
            get { return _deviceId; }
            set
            {
                _deviceId = value;
                OnPropertyChanged("DeviceId");
            }
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            _device.IndicatorLogic = new IndicatorLogic();

            if (IsZone)
            {
                _device.IndicatorLogic.IndicatorLogicType = IndicatorLogicType.Zone;

                foreach (var zone in TargetZones)
                    _device.IndicatorLogic.Zones.Add(zone.Zone.No);
            }

            if (IsDevice)
            {
                _device.IndicatorLogic.IndicatorLogicType = IndicatorLogicType.Device;

                string uid = SelectedDevice.Device.UID;
                if (string.IsNullOrEmpty(uid))
                {
                    uid = Guid.NewGuid().ToString();
                    SelectedDevice.Device.UID = uid;
                }

                _device.IndicatorLogic.DeviceUID = SelectedDevice.Device.UID;
                _device.IndicatorLogic.OnColor = OnColor;
                _device.IndicatorLogic.OffColor = OffColor;
                _device.IndicatorLogic.FailureColor = FailureColor;
                _device.IndicatorLogic.ConnectionColor = ConnectionColor;
            }

            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}