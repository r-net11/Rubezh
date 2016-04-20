using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using GKProcessor;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DeviceCommandsViewModel : BaseViewModel
	{
		public GKDevice Device { get; private set; }
		public GKState DeviceState
		{
			get { return Device.State; }
		}

		public DeviceCommandsViewModel(GKDevice device)
		{
			Device = device;
			DeviceState.StateChanged -= new System.Action(OnStateChanged);
			DeviceState.StateChanged += new System.Action(OnStateChanged);

			SetAutomaticStateCommand = new RelayCommand(OnSetAutomaticState, CanSetAutomaticState);
			SetManualStateCommand = new RelayCommand(OnSetManualState, CanSetManualState);
			SetIgnoreStateCommand = new RelayCommand(OnSetIgnoreState, CanSetIgnoreState);
			ResetCommand = new RelayCommand(OnReset, CanReset);

			DeviceExecutableCommands = new ObservableCollection<DeviceExecutableCommandViewModel>();
			foreach (var availableCommand in Device.Driver.AvailableCommandBits)
			{
				var deviceExecutableCommandViewModel = new DeviceExecutableCommandViewModel(Device, availableCommand);
				DeviceExecutableCommands.Add(deviceExecutableCommandViewModel);
			}
			if (Device.DriverType == GKDriverType.RSR2_Valve_DU || Device.DriverType == GKDriverType.RSR2_Valve_KV || Device.DriverType == GKDriverType.RSR2_Valve_KVMV)
			{
				Device.State.MeasureParametersChanged += new Action(() => { OnPropertyChanged(() => IsControlRegime); });
			}
		}

		public bool IsTriStateControl
		{
			get { return Device.Driver.IsControlDevice && ClientManager.CheckPermission(PermissionType.Oper_Device_Control); }
		}

		public bool IsBiStateControl
		{
			get { return Device.Driver.IsDeviceOnShleif && !Device.Driver.IsControlDevice && ClientManager.CheckPermission(PermissionType.Oper_Device_Control); }
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

		public RelayCommand SetAutomaticStateCommand { get; private set; }
		void OnSetAutomaticState()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKSetAutomaticRegime(Device);
			}
		}
		bool CanSetAutomaticState()
		{
			return ControlRegime != DeviceControlRegime.Automatic;
		}

		public RelayCommand SetManualStateCommand { get; private set; }
		void OnSetManualState()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKSetManualRegime(Device);
			}
		}
		bool CanSetManualState()
		{
			return ControlRegime != DeviceControlRegime.Manual;
		}

		public RelayCommand SetIgnoreStateCommand { get; private set; }
		void OnSetIgnoreState()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKSetIgnoreRegime(Device);
			}
		}
		bool CanSetIgnoreState()
		{
			return ControlRegime != DeviceControlRegime.Ignore;
		}

		public ObservableCollection<DeviceExecutableCommandViewModel> DeviceExecutableCommands { get; private set; }

		void OnStateChanged()
		{
			OnPropertyChanged(() => ControlRegime);
			OnPropertyChanged(() => IsControlRegime);
		}

		public bool HasReset
		{
			get { return Device.DriverType == GKDriverType.RSR2_MAP4; }
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKReset(Device);
			}
		}
		bool CanReset()
		{
			return DeviceState.StateClasses.Contains(XStateClass.Fire2) || DeviceState.StateClasses.Contains(XStateClass.Fire1);
		}
	}
}