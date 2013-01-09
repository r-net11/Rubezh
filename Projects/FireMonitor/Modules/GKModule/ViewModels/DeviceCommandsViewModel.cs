using System.Collections.Generic;
using Common.GK;
using Infrastructure.Common;
using XFiresecAPI;
using FiresecClient;
using FiresecAPI.Models;
using System.ComponentModel;
using Infrastructure.Common.Windows.ViewModels;
using System.Diagnostics;

namespace GKModule.ViewModels
{
	public class DeviceCommandsViewModel : BaseViewModel
	{
		public XDeviceState DeviceState { get; private set; }
		public XDevice Device { get { return DeviceState.Device; } }

		public DeviceCommandsViewModel(XDeviceState deviceState)
		{
            SetAutomaticStateCommand = new RelayCommand(OnSetAutomaticState);
            SetManualStateCommand = new RelayCommand(OnSetManualState);
            SetIgnoreStateCommand = new RelayCommand(OnSetIgnoreState);
            SetOnStateCommand = new RelayCommand(OnSetOnState);
            SetOffStateCommand = new RelayCommand(OnSetOffState);
			TurnOnCommand = new RelayCommand(OnTurnOn);
			TurnOnNowCommand = new RelayCommand(OnTurnOnNow);
			TurnOffCommand = new RelayCommand(OnTurnOff);

			DeviceState = deviceState;
			DeviceState.StateChanged -= new System.Action(OnStateChanged);
			DeviceState.StateChanged += new System.Action(OnStateChanged);
		}

		//SetIgnoreCommand 0x86
        //ResetIgnoreCommand 0x06
        //SetAutomaticCommand 0x80
        //ResetAutomaticCommand 0x00
        //TurnOnCommand 0x8b
		//CancelDelayCommand 0x8c
		//TurnOffCommand 0x8d
        //StopCommand 0x8e
        //CancelStartCommand 0x8f
        //TurnOnNowCommand 0x90
        //TurnOffNowCommand 0x91

		void SendControlCommand(byte code)
		{
			if (Device.Driver.IsDeviceOnShleif)
			{
				ObjectCommandSendHelper.SendControlCommand(Device, code);
			}
		}

        public bool IsTriStateControl
        {
            get { return Device.Driver.IsDeviceOnShleif && Device.Driver.IsControlDevice; }
        }

        public bool IsBiStateControl
        {
            get { return Device.Driver.IsDeviceOnShleif && !Device.Driver.IsControlDevice; }
        }

        public DeviceControlRegime ControlRegime
        {
            get
            {
                if (DeviceState.States.Contains(XStateType.Ignore))
                    return DeviceControlRegime.Ignore;

                if (DeviceState.States.Contains(XStateType.Norm))
                    return DeviceControlRegime.Automatic;

                return DeviceControlRegime.Manual;
            }
        }

		public bool IsControlRegime
		{
            get { return ControlRegime == DeviceControlRegime.Manual; }
		}

        public RelayCommand SetAutomaticStateCommand { get; private set; }
        void OnSetAutomaticState()
        {
            SendControlCommand(0x80);
            SendControlCommand(0x06);
        }

        public RelayCommand SetManualStateCommand { get; private set; }
        void OnSetManualState()
        {
            SendControlCommand(0x00);
            SendControlCommand(0x06);
        }

        public RelayCommand SetIgnoreStateCommand { get; private set; }
        void OnSetIgnoreState()
        {
            SendControlCommand(0x86);
            SendControlCommand(0x00);
        }

        public RelayCommand SetOnStateCommand { get; private set; }
        void OnSetOnState()
        {
            SendControlCommand(0x06);
        }

        public RelayCommand SetOffStateCommand { get; private set; }
        void OnSetOffState()
        {
            SendControlCommand(0x86);
        }

		public RelayCommand TurnOnCommand { get; private set; }
		void OnTurnOn()
        {
			SendControlCommand(0x8b);
        }

		public RelayCommand TurnOnNowCommand { get; private set; }
		void OnTurnOnNow()
        {
			SendControlCommand(0x90);
        }

		public RelayCommand TurnOffCommand { get; private set; }
		void OnTurnOff()
        {
			SendControlCommand(0x8d);
        }

		void OnStateChanged()
		{
			OnPropertyChanged("ControlRegime");
			OnPropertyChanged("IsControlRegime");
		}
	}
}