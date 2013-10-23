using System;
using System.Collections.Generic;
using System.Windows;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKSDK
{
	public class DeviceViewModel : BaseViewModel
	{
		public DeviceViewModel(XDevice device)
		{
			AddToIgnoreListCommand = new RelayCommand(OnAddToIgnoreList, CanAddToIgnoreList);
			RemoveFromIgnoreListCommand = new RelayCommand(OnRemoveFromIgnoreList, CanRemoveFromIgnoreList);
			Device = device;
			DeviceState = device.DeviceState;
			_stateClass = DeviceState.StateClass;
			DeviceState.StateChanged += new Action(OnStateChanged);
			Name = Device.Driver.ShortName + " - " + Device.DottedAddress;

			DeviceCommands = new List<DeviceCommandViewModel>();
		}

		public void SafeCall(Action action)
		{
			if (Application.Current != null && Application.Current.Dispatcher != null)
				Application.Current.Dispatcher.BeginInvoke(action);
		}

		void OnStateChanged()
		{
			StateClass = DeviceState.StateClass;
		}

		public XDevice Device { get; private set; }
		public XDeviceState DeviceState { get; private set; }
		public string Name { get; private set; }

		XStateClass _stateClass;
		public XStateClass StateClass
		{
			get { return _stateClass; }
			set
			{
				_stateClass = value;
				OnPropertyChanged("StateClass");
			}
		}

		public RelayCommand AddToIgnoreListCommand { get; private set; }
		void OnAddToIgnoreList()
		{
		}
		bool CanAddToIgnoreList()
		{
			return true;
		}

		public RelayCommand RemoveFromIgnoreListCommand { get; private set; }
		void OnRemoveFromIgnoreList()
		{
		}
		bool CanRemoveFromIgnoreList()
		{
			return true;
		}

		public List<DeviceCommandViewModel> DeviceCommands { get; private set; }
	}
}