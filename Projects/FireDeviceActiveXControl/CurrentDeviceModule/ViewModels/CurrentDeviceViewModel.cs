using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecAPI.Models;
//using FireDeviceActiveXControl.ViewModels;
using CurrentDeviceModule.Views;
using DeviceControls;
using FiresecClient;

namespace CurrentDeviceModule.ViewModels
{
    public class CurrentDeviceViewModel : RegionViewModel
    {
        public CurrentDeviceViewModel()
        {
            FiresecEventSubscriber.DeviceStateChangedEvent += new Action<string>(OnDeviceStateChanged);
            CurrentDeviceControl = new DeviceControl();
            Device = new Device();
            IsCurrentDeviceSelected = false;
        }

        DeviceControl _deviceControl;
        public DeviceControl CurrentDeviceControl
        {
            get { return _deviceControl; }
            set
            {
                _deviceControl = value;
                OnPropertyChanged("DeviceControl");
            }
        }

        Device _device;
        public Device Device
        {
            get { return _device; }
            private set 
            {
                _device = value;
                //OnPropertyChanged("Device");
            }
        }

        public State State
        {
            get
            {
                DeviceState deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == DeviceId);
                return deviceState.State;
            }
        }

        string _deviceId;
        public string DeviceId
        {
            get { return _deviceId; }
            set 
            {
                _deviceId = value;
                //OnPropertyChanged("DeviceId");
            }
        }

        public string DriverId
        {
            get { return Device.Driver.Id; }
        }

        public bool DriverCanDisable
        {
            get { return Device.Driver.CanDisable; }
        }

        private bool _isCurrentDeviceSelected;
        public bool IsCurrentDeviceSelected
        {
            get { return _isCurrentDeviceSelected; }
            set { _isCurrentDeviceSelected = value; }
        }

        public void SelectDevice()
        {
            DeviceTreeViewModel newDeviceTreeViewModel = new DeviceTreeViewModel();
            newDeviceTreeViewModel.Initialize();
            DeviceTreeView newDeviceTreeView = new DeviceTreeView();
            newDeviceTreeView.DataContext = newDeviceTreeViewModel;
            newDeviceTreeViewModel.Closing += newDeviceTreeView.Close;
            newDeviceTreeView.ShowDialog();
            if (!string.IsNullOrWhiteSpace(newDeviceTreeViewModel.DeviceId))
            {
                DeviceId = string.Copy(newDeviceTreeViewModel.DeviceId);
            }
            IsCurrentDeviceSelected = true;
        }

        public void Inicialize()
        {
            if (IsCurrentDeviceSelected)
            {
                Device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == DeviceId);
                CurrentDeviceControl.DriverId = DriverId;
                CurrentDeviceControl.StateId = State.Id.ToString();
            }
        }

        void OnDeviceStateChanged(string id)
        {
            if (DeviceId == id) ;
            {
                Update();
            }
        }

        void Update()
        {
            //
        }
    }
}
