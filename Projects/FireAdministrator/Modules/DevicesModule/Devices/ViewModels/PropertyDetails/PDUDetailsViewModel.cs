using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class PDUDetailsViewModel : SaveCancelDialogViewModel
    {
        Device _device;

        public PDUDetailsViewModel(Device device)
        {
            Title = "Свойства направление ПДУ";
            AddCommand = new RelayCommand(OnAdd, CanAdd);
            RemoveCommand = new RelayCommand(OnRemove, CanRemove);

            _device = device;

            InitializeDevices();
            InitializeAvailableDevices();
        }

        void InitializeDevices()
        {
            Devices = new ObservableCollection<PDUDeviceViewModel>();

            if (_device.PDUGroupLogic == null)
                return;

            foreach (var groupDevice in _device.PDUGroupLogic.Devices)
            {
                var pduDeviceViewModel = new PDUDeviceViewModel();
                pduDeviceViewModel.Initialize(groupDevice);
                Devices.Add(pduDeviceViewModel);
            }

            SelectedDevice = Devices.FirstOrDefault();
        }

        void InitializeAvailableDevices()
        {
            var devices = new HashSet<Device>();

            foreach (var device in FiresecManager.Devices)
            {
                if (Devices.Any(x => x.Device.UID == device.UID))
                    continue;

                if (
                    (device.Driver.DriverType == DriverType.RM_1) ||
                    (device.Driver.DriverType == DriverType.MDU) ||
                    (device.Driver.DriverType == DriverType.MRO) ||
                    //(device.Driver.DriverType == "Модуль пожаротушения") ||
                    (device.Driver.DriverType == DriverType.AM1_T)
                )
                {
                    device.AllParents.ForEach(x => { devices.Add(x); });
                    devices.Add(device);
                }
            }

            AvailableDevices = new ObservableCollection<DeviceViewModel>();
            foreach (var device in devices)
            {
                var deviceViewModel = new DeviceViewModel(device, AvailableDevices);
                deviceViewModel.IsExpanded = true;
                AvailableDevices.Add(deviceViewModel);
            }

            foreach (var device in AvailableDevices)
            {
                if (device.Device.Parent != null)
                {
                    var parent = AvailableDevices.FirstOrDefault(x => x.Device.UID == device.Device.Parent.UID);
                    device.Parent = parent;
                    parent.Children.Add(device);
                }
            }

            SelectedAvailableDevice = AvailableDevices.FirstOrDefault(x => x.HasChildren == false);
        }

        ObservableCollection<PDUDeviceViewModel> _devices;
        public ObservableCollection<PDUDeviceViewModel> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                OnPropertyChanged("Devices");
            }
        }

        PDUDeviceViewModel _selectedDevice;
        public PDUDeviceViewModel SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
                OnPropertyChanged("CanEditProperties");
            }
        }

        ObservableCollection<DeviceViewModel> _availableDevices;
        public ObservableCollection<DeviceViewModel> AvailableDevices
        {
            get { return _availableDevices; }
            set
            {
                _availableDevices = value;
                OnPropertyChanged("AvailableDevices");
            }
        }

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

        public bool CanEditProperties
        {
            get
            {
                return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.DriverType != DriverType.AM1_T));
            }
        }

        bool CanAdd()
        {
            if (SelectedAvailableDevice != null && SelectedAvailableDevice.HasChildren == false)
            {
                if ((SelectedAvailableDevice.Device.Driver.DriverType == DriverType.AM1_T) &&
                                        (Devices.Any(x => x.Device.Driver.DriverType == DriverType.AM1_T)))
                    return false;
                return true;
            }
            return false;
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var pduDeviceViewModel = new PDUDeviceViewModel();
            pduDeviceViewModel.Initialize(SelectedAvailableDevice.Device);
            Devices.Add(pduDeviceViewModel);
            InitializeAvailableDevices();
        }

        bool CanRemove()
        {
            if (SelectedDevice != null)
                return true;
            return false;
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            Devices.Remove(SelectedDevice);
            InitializeAvailableDevices();
        }

		protected override bool Save()
		{
			_device.PDUGroupLogic = new PDUGroupLogic();

			_device.PDUGroupLogic.AMTPreset = Devices.Any(x => x.Device.Driver.DriverType == DriverType.AM1_T);
			foreach (var device in Devices)
			{
				var pDUGroupDevice = new PDUGroupDevice()
				{
					DeviceUID = device.Device.UID,
					IsInversion = device.IsInversion,
					OnDelay = device.OnDelay,
					OffDelay = device.OffDelay
				};
				_device.PDUGroupLogic.Devices.Add(pDUGroupDevice);
			}
			return base.Save();
		}
    }
}