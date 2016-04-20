using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKSDK
{
	public class DeviceViewModel : BaseViewModel
	{
		public ObservableCollection<DeviceExecutableCommandViewModel> DeviceExecutableCommands { get; private set; }
		public DeviceViewModel(GKDevice device)
		{
			Device = device;
			DeviceState = device.State;
			_stateClass = DeviceState.StateClass;
			DeviceState.StateChanged += new Action(OnStateChanged);
			Name = Device.Driver.ShortName + " - " + Device.DottedAddress;

			DeviceCommands = new List<DeviceCommandViewModel>();
			DeviceExecutableCommands = new ObservableCollection<DeviceExecutableCommandViewModel>();
			foreach (var availableCommand in Device.Driver.AvailableCommandBits)
			{
				var deviceExecutableCommandViewModel = new DeviceExecutableCommandViewModel(Device, availableCommand);
				DeviceExecutableCommands.Add(deviceExecutableCommandViewModel);
			}
			SetAutomaticStateCommand = new RelayCommand(OnSetAutomaticState, CanSetAutomaticState);
			SetManualStateCommand = new RelayCommand(OnSetManualState, CanSetManualState);
			SetIgnoreStateCommand = new RelayCommand(OnSetIgnoreState, CanSetIgnoreState);
		}
		public DeviceControlRegime ControlRegime
		{
			get
			{
				if (DeviceState.StateClasses.Contains(XStateClass.Ignore))
					return DeviceControlRegime.Ignore;

				if (!DeviceState.StateClasses.Contains(XStateClass.AutoOff))
					return DeviceControlRegime.Automatic;

				return DeviceControlRegime.Manual;
			}
		}
		public bool IsControlRegime
		{
			get
			{
				if (Device.DriverType == GKDriverType.RSR2_Valve_DU || Device.DriverType == GKDriverType.RSR2_Valve_KV || Device.DriverType == GKDriverType.RSR2_Valve_KVMV)
				{
					var automaticParameter = Device.State.XMeasureParameterValues.FirstOrDefault(x => x.Name == "Управление с ГК");
					if (automaticParameter != null)
					{
						return automaticParameter.StringValue == "Р";
					}
					return false;
				}
				return ControlRegime == DeviceControlRegime.Manual;
			}
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

		public GKDevice Device { get; private set; }
		public GKState DeviceState { get; private set; }
		public string Name { get; private set; }

		XStateClass _stateClass;
		public XStateClass StateClass
		{
			get { return _stateClass; }
			set
			{
				_stateClass = value;
				OnPropertyChanged(() => StateClass);
			}
		}
		public List<DeviceCommandViewModel> DeviceCommands { get; private set; }

		public RelayCommand SetAutomaticStateCommand { get; private set; }
		void OnSetAutomaticState()
		{
			ClientManager.FiresecService.GKSetAutomaticRegime(Device);
		}
		bool CanSetAutomaticState()
		{
			return ControlRegime != DeviceControlRegime.Automatic;
		}
		public RelayCommand SetManualStateCommand { get; private set; }
		void OnSetManualState()
		{
			ClientManager.FiresecService.GKSetManualRegime(Device);
		}
		bool CanSetManualState()
		{
			return ControlRegime != DeviceControlRegime.Manual;
		}
		public RelayCommand SetIgnoreStateCommand { get; private set; }
		void OnSetIgnoreState()
		{
			ClientManager.FiresecService.GKSetIgnoreRegime(Device);
		}
		bool CanSetIgnoreState()
		{
			return ControlRegime != DeviceControlRegime.Ignore;
		}
		public bool IsTriStateControl
		{
			get { return Device.Driver.IsControlDevice && ClientManager.CheckPermission(PermissionType.Oper_Device_Control); }
		}
		public bool IsBiStateControl
		{
			get { return Device.Driver.IsDeviceOnShleif && !Device.Driver.IsControlDevice && ClientManager.CheckPermission(PermissionType.Oper_Device_Control); }
		}
	}
}