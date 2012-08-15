using System.Collections.Generic;
using Infrastructure.Common;
using XFiresecAPI;
using Common.GK;

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
			return true;
		}

		public RelayCommand CancelDelayCommand { get; private set; }
		void OnCancelDelay()
		{
			SendControlCommand(0x8c);
		}
		bool CanCancelDelay()
		{
			return true;
		}

		public RelayCommand TurnOffCommand { get; private set; }
		void OnTurnOff()
		{
			SendControlCommand(0x8d);
		}
		bool CanTurnOff()
		{
			return true;
		}

		public RelayCommand StopCommand { get; private set; }
		void OnStop()
		{
			SendControlCommand(0x8e);
		}
		bool CanStop()
		{
			return true;
		}

		public RelayCommand CancelStartCommand { get; private set; }
		void OnCancelStart()
		{
			SendControlCommand(0x8f);
		}
		bool CanCancelStart()
		{
			return true;
		}

		public RelayCommand TurnOnNowCommand { get; private set; }
		void OnTurnOnNow()
		{
			SendControlCommand(0x90);
		}
		bool CanTurnOnNow()
		{
			return true;
		}

		public RelayCommand TurnOffNowCommand { get; private set; }
		void OnTurnOffNow()
		{
			SendControlCommand(0x91);
		}
		bool CanTurnOffNow()
		{
			return true;
		}

		public bool CanControl
		{
			get
			{
				return Device.Driver.IsDeviceOnShleif;

				if (Device.Parent != null)
				{
					switch (Device.Parent.Driver.DriverType)
					{
						case XDriverType.GK:
						case XDriverType.KAU:
							return true;
					}
				}
				return false;
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