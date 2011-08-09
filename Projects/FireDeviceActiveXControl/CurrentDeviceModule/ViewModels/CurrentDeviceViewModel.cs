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
            }
        }

        CurrentDeviceView _deviceControlView;
        public CurrentDeviceView CurrentDeviceControlView
        {
            get { return _deviceControlView; }
            set { _deviceControlView = value; }
        }

        Device _device;
        public Device Device
        {
            get { return _device; }
            private set 
            {
                _device = value;
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

        bool _isCurrentDeviceSelected;
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
                IsCurrentDeviceSelected = true;
                Device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == DeviceId);
                CurrentDeviceControl.DriverId = DriverId;
                CurrentDeviceControl.StateId = State.Id.ToString();
            }
        }

        public void Inicialize(string deviceId)
        {
            DeviceId = deviceId;
            Device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == DeviceId);
            CurrentDeviceControl.DriverId = DriverId;
            CurrentDeviceControl.StateId = State.Id.ToString();
            IsCurrentDeviceSelected = true;
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
