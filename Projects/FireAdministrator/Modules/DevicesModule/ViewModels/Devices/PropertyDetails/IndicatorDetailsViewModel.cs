using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common;
using FiresecAPI.Models;

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
            //_device = device;

            //InitializeDevices();

            //if (device.Properties == null)
            //    return;

            //var indicatorProperty = device.Properties.FirstOrDefault(x => x.Name == "C4D7C1BE-02A3-4849-9717-7A3C01C23A24");
            //if (indicatorProperty == null)
            //{
            //    return;
            //}

            //Logic = indicatorProperty.Value;

            //var indicatorLogic = SerializerHelper.GetIndicatorLogic(Logic);
            //if (indicatorLogic == null)
            //    return;

            //Zones = new ObservableCollection<string>();
            //if (indicatorLogic.zone != null)
            //{
            //    IsZone = true;

            //    foreach (var zone in indicatorLogic.zone)
            //        Zones.Add(zone);
            //}
            //InitializeZones(indicatorLogic.zone.ToList());

            //if (indicatorLogic.device != null)
            //{
            //    IsDevice = true;

            //    var indicatorDevice = indicatorLogic.device[0];
            //    DeviceId = indicatorDevice.UID;
            //    OnColor = indicatorDevice.state1;
            //    OffColor = indicatorDevice.state2;
            //    FailureColor = indicatorDevice.state3;
            //    ConnectionColor = indicatorDevice.state4;

            //    InitializeDevices();
            //    SelectedDevice = Devices.FirstOrDefault(x => x.Device.UID == DeviceId);
            //}
        }

        void InitializeDevices()
        {
            HashSet<Device> devices = new HashSet<Device>();

            foreach (var device in FiresecManager.Configuration.Devices)
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
                DeviceViewModel deviceViewModel = new DeviceViewModel();
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

            foreach (Zone zone in FiresecManager.Configuration.Zones)
            {
                ZoneViewModel zoneViewModel = new ZoneViewModel(zone);

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

        public ObservableCollection<string> Colors
        {
            get
            {
                ObservableCollection<string> colors = new ObservableCollection<string>();
                colors.Add("0");
                colors.Add("1");
                colors.Add("2");
                colors.Add("3");
                colors.Add("4");
                colors.Add("5");
                colors.Add("6");
                return colors;
            }
        }

        string _onColor;
        public string OnColor
        {
            get { return _onColor; }
            set
            {
                _onColor = value;
                OnPropertyChanged("OnColor");
            }
        }

        string _offColor;
        public string OffColor
        {
            get { return _offColor; }
            set
            {
                _offColor = value;
                OnPropertyChanged("OffColor");
            }
        }

        string _failureColor;
        public string FailureColor
        {
            get { return _failureColor; }
            set
            {
                _failureColor = value;
                OnPropertyChanged("FailureColor");
            }
        }

        string _connectionColor;
        public string ConnectionColor
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
            //Firesec.Indicator.LEDProperties indicatorLogic = new Firesec.Indicator.LEDProperties();

            //if (IsZone)
            //{
            //    indicatorLogic.type = "0";
            //    indicatorLogic.device = null;

            //    indicatorLogic.zone = Zones.ToArray();
            //}
            //else
            //{
            //    indicatorLogic.type = "1";
            //    indicatorLogic.zone = null;

            //    indicatorLogic.device = new Firesec.Indicator.deviceType[1];
            //    indicatorLogic.device[0] = new Firesec.Indicator.deviceType();
            //    indicatorLogic.device[0].state1 = OnColor;
            //    indicatorLogic.device[0].state2 = OffColor;
            //    indicatorLogic.device[0].state3 = FailureColor;
            //    indicatorLogic.device[0].state4 = ConnectionColor;

            //    string uid = SelectedDevice.Device.UID;
            //    if (string.IsNullOrEmpty(uid))
            //    {
            //        uid = Guid.NewGuid().ToString();
            //        SelectedDevice.Device.UID = uid;
            //    }
            //    indicatorLogic.device[0].UID = uid;
            //}

            //string indicatorLogicValue = SerializerHelper.SetIndicatorLogic(indicatorLogic);
            //_device.Properties = new List<Property>();
            //_device.Properties.Add(new Property() { Name = "C4D7C1BE-02A3-4849-9717-7A3C01C23A24", Value = indicatorLogicValue });
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}
