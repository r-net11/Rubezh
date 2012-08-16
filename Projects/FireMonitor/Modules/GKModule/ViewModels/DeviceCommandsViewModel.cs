using System.Collections.Generic;
using Common.GK;
using Infrastructure.Common;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class DeviceCommandsViewModel
	{
		public XDevice Device { get; private set; }
		public XDeviceState DeviceState { get; private set; }

		public DeviceCommandsViewModel(XDeviceState deviceState)
		{
			DeviceState = deviceState;
			Device = deviceState.Device;
			SetIgnoreCommand = new RelayCommand(OnSetIgnore, CanSetIgnore);
			ResetIgnoreCommand = new RelayCommand(OnResetIgnore, CanResetIgnore);
			TurnOnCommand = new RelayCommand(OnTurnOn, CanTurnOn);
			CancelDelayCommand = new RelayCommand(OnCancelDelay, CanCancelDelay);
			TurnOffCommand = new RelayCommand(OnTurnOff, CanTurnOff);
			StopCommand = new RelayCommand(OnStop, CanStop);
			CancelStartCommand = new RelayCommand(OnCancelStart, CanCancelStart);
			TurnOnNowCommand = new RelayCommand(OnTurnOnNow, CanTurnOnNow);
			TurnOffNowCommand = new RelayCommand(OnTurnOffNow, CanTurnOffNow);
		}

		public RelayCommand SetIgnoreCommand { get; private set; }
		void OnSetIgnore()
		{
			SendControlCommand(0x86);
		}
		bool CanSetIgnore()
		{
			return !DeviceState.States.Contains(XStateType.Ignore);
		}

		public RelayCommand ResetIgnoreCommand { get; private set; }
		void OnResetIgnore()
		{
			SendControlCommand(0x06);
		}
		bool CanResetIgnore()
		{
			return DeviceState.States.Contains(XStateType.Ignore);
		}

		public RelayCommand TurnOnCommand { get; private set; }
		void OnTurnOn()
		{
			SendControlCommand(0x8b);
		}
		bool CanTurnOn()
		{
            return Device.Driver.IsControlDevice;
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
            return Device.Driver.IsControlDevice;
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
            return Device.Driver.IsControlDevice;
		}

		public RelayCommand TurnOffNowCommand { get; private set; }
		void OnTurnOffNow()
		{
			SendControlCommand(0x91);
		}
		bool CanTurnOffNow()
		{
            return Device.Driver.IsControlDevice;
		}

		public bool CanControl
		{
			get
			{
                switch(Device.Driver.DriverType)
                {
                    case XDriverType.System:
                    case XDriverType.GK:
                    case XDriverType.GKIndicator:
                    case XDriverType.GKLine:
                    case XDriverType.GKRele:
                    case XDriverType.KAU:
                    case XDriverType.KAUIndicator:
                        return false;
                }
                return true;
			}
		}

		void SendControlCommand(byte code)
		{
			if (Device.Driver.IsDeviceOnShleif)
			{
				var bytes = new List<byte>();
				bytes.AddRange(BytesHelper.ShortToBytes(Device.GetDatabaseNo(DatabaseType.Gk)));
				bytes.Add(code);
				SendManager.Send(Device.Parent, 3, 13, 0, bytes);
			}
		}
	}
}