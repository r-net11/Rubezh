using System;
using System.Collections.Generic;
using System.Windows;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient.Itv;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace ItvIntegration
{
    public class DeviceViewModel : BaseViewModel
    {
        public DeviceViewModel(Device device)
        {
            AddToIgnoreListCommand = new RelayCommand(OnAddToIgnoreList, CanAddToIgnoreList);
            RemoveFromIgnoreListCommand = new RelayCommand(OnRemoveFromIgnoreList, CanRemoveFromIgnoreList);
            Device = device;
            DeviceState = device.DeviceState;
            _stateType = DeviceState.StateType;
			ItvManager.DeviceStateChanged += new Action<DeviceState>((x) => { SafeCall(() => { OnDeviceStateChanged(x); }); });
            Name = Device.Driver.ShortName + " - " + Device.DottedAddress;

            DeviceCommands = new List<DeviceCommandViewModel>();
            foreach (var property in device.Driver.Properties)
            {
                if (property.IsControl)
                {
                    var deviceCommandViewModel = new DeviceCommandViewModel(device, property);
                    DeviceCommands.Add(deviceCommandViewModel);
                }
            }
        }

		public void SafeCall(Action action)
		{
			if (Application.Current != null && Application.Current.Dispatcher != null)
				Application.Current.Dispatcher.BeginInvoke(action);
		}

        void OnDeviceStateChanged(DeviceState deviceState)
        {
            if (DeviceState == deviceState)
            {
                StateType = DeviceState.StateType;
            }
        }

        public Device Device { get; private set; }
        public DeviceState DeviceState { get; private set; }
        public string Name { get; private set; }

        StateType _stateType;
        public StateType StateType
        {
            get { return _stateType; }
            set
            {
                _stateType = value;
                OnPropertyChanged("StateType");
            }
        }

        public RelayCommand AddToIgnoreListCommand { get; private set; }
        void OnAddToIgnoreList()
        {
            var devices = new List<Device>();
            devices.Add(Device);
            ItvManager.AddToIgnoreList(devices);
        }
        bool CanAddToIgnoreList()
        {
            return Device.Driver.CanDisable;
        }

        public RelayCommand RemoveFromIgnoreListCommand { get; private set; }
        void OnRemoveFromIgnoreList()
        {
            var devices = new List<Device>();
            devices.Add(Device);
            ItvManager.RemoveFromIgnoreList(devices);
        }
        bool CanRemoveFromIgnoreList()
        {
            return Device.Driver.CanDisable;
        }

        public List<DeviceCommandViewModel> DeviceCommands { get; private set; }
    }
}