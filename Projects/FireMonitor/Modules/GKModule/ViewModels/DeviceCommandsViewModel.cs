using System.Collections.Generic;
using Common.GK;
using Infrastructure.Common;
using XFiresecAPI;
using FiresecClient;
using FiresecAPI.Models;

namespace GKModule.ViewModels
{
	public class DeviceCommandsViewModel
	{
		public XDeviceState DeviceState { get; private set; }
		public XDevice Device { get { return DeviceState.Device; } }

		public DeviceCommandsViewModel(XDeviceState deviceState)
		{
			DeviceState = deviceState;
			SetIgnoreCommand = new RelayCommand(OnSetIgnore, CanSetIgnore);
			ResetIgnoreCommand = new RelayCommand(OnResetIgnore, CanResetIgnore);
			SetAutomaticCommand = new RelayCommand(OnSetAutomatic, CanSetAutomatic);
			ResetAutomaticCommand = new RelayCommand(OnResetAutomatic, CanResetAutomatic);
			TurnOnCommand = new RelayCommand(OnTurnOn, CanTurnOn);
            TurnOffCommand = new RelayCommand(OnTurnOff, CanTurnOff);
            TurnOnNowCommand = new RelayCommand(OnTurnOnNow, CanTurnOnNow);
            TurnOffNowCommand = new RelayCommand(OnTurnOffNow, CanTurnOffNow);
			CancelDelayCommand = new RelayCommand(OnCancelDelay, CanCancelDelay);
			StopCommand = new RelayCommand(OnStop, CanStop);
			CancelStartCommand = new RelayCommand(OnCancelStart, CanCancelStart);
		}

		public RelayCommand SetIgnoreCommand { get; private set; }
		void OnSetIgnore()
		{
			SendControlCommand(0x86);
		}
		bool CanSetIgnore()
		{
            return !DeviceState.States.Contains(XStateType.Ignore) && FiresecManager.CheckPermission(PermissionType.Oper_AddToIgnoreList);
		}

		public RelayCommand ResetIgnoreCommand { get; private set; }
		void OnResetIgnore()
		{
			SendControlCommand(0x06);
		}
		bool CanResetIgnore()
		{
            return DeviceState.States.Contains(XStateType.Ignore) && FiresecManager.CheckPermission(PermissionType.Oper_RemoveFromIgnoreList);
		}

		public RelayCommand SetAutomaticCommand { get; private set; }
		void OnSetAutomatic()
		{
			SendControlCommand(0x80);
		}
		bool CanSetAutomatic()
		{
			return Device.Driver.IsControlDevice && !DeviceState.States.Contains(XStateType.Norm);
		}

		public RelayCommand ResetAutomaticCommand { get; private set; }
		void OnResetAutomatic()
		{
			SendControlCommand(0x00);
		}
		bool CanResetAutomatic()
		{
			return Device.Driver.IsControlDevice && DeviceState.States.Contains(XStateType.Norm);
		}

		public RelayCommand TurnOnCommand { get; private set; }
		void OnTurnOn()
		{
			SendControlCommand(0x8b);
		}
		bool CanTurnOn()
		{
            return Device.Driver.IsControlDevice && FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices);
		}

		public RelayCommand CancelDelayCommand { get; private set; }
		void OnCancelDelay()
		{
			SendControlCommand(0x8c);
		}
		bool CanCancelDelay()
		{
			return Device.Driver.IsControlDevice;
		}

		public RelayCommand TurnOffCommand { get; private set; }
		void OnTurnOff()
		{
			SendControlCommand(0x8d);
		}
		bool CanTurnOff()
		{
            return Device.Driver.IsControlDevice && FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices);
		}

		public RelayCommand StopCommand { get; private set; }
		void OnStop()
		{
			SendControlCommand(0x8e);
		}
		bool CanStop()
		{
			return Device.Driver.IsControlDevice;
		}

		public RelayCommand CancelStartCommand { get; private set; }
		void OnCancelStart()
		{
			SendControlCommand(0x8f);
		}
		bool CanCancelStart()
		{
			return Device.Driver.IsControlDevice;
		}

		public RelayCommand TurnOnNowCommand { get; private set; }
		void OnTurnOnNow()
		{
			SendControlCommand(0x90);
		}
		bool CanTurnOnNow()
		{
            return Device.Driver.IsControlDevice && FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices);
		}

		public RelayCommand TurnOffNowCommand { get; private set; }
		void OnTurnOffNow()
		{
			SendControlCommand(0x91);
		}
		bool CanTurnOffNow()
		{
            return Device.Driver.IsControlDevice && FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices);
		}

		void SendControlCommand(byte code)
		{
			if (Device.Driver.IsDeviceOnShleif)
			{
				ObjectCommandSendHelper.SendControlCommand(Device, code);
			}
		}

		public bool CanControl
		{
			get { return Device.Driver.IsDeviceOnShleif; }
		}
	}
}