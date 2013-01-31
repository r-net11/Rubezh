using System;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public class DirectionDetailsViewModel : DialogViewModel, IWindowIdentity
    {
        public XDirection Direction { get; private set; }
        public XDirectionState DirectionState { get; private set; }
		public DirectionViewModel DirectionViewModel { get; private set; }

        public DirectionDetailsViewModel(XDirection direction)
        {
            Direction = direction;
            DirectionState = Direction.DirectionState;
			DirectionViewModel = new DirectionViewModel(DirectionState);
            DirectionState.StateChanged += new Action(OnStateChanged);

            SetAutomaticStateCommand = new RelayCommand(OnSetAutomaticState);
            SetManualStateCommand = new RelayCommand(OnSetManualState);
            SetIgnoreStateCommand = new RelayCommand(OnSetIgnoreState);
			TurnOnCommand = new RelayCommand(OnTurnOn);
			TurnOnNowCommand = new RelayCommand(OnTurnOnNow);
			TurnOffCommand = new RelayCommand(OnTurnOff);

            Title = Direction.PresentationName;
            TopMost = true;
        }

        void OnStateChanged()
        {
            OnPropertyChanged("DirectionState");
			OnPropertyChanged("ControlRegime");
			OnPropertyChanged("IsControlRegime");
        }

		public int InputZonesCount
		{
			get { return Direction.InputZones.Count; }
		}
		public int InputDevicesCount
		{
			get { return Direction.InputDevices.Count; }
		}
		public int OutputDevicesCount
		{
			get { return Direction.OutputDevices.Count; }
		}

        public DeviceControlRegime ControlRegime
        {
            get
            {
                if (DirectionState.States.Contains(XStateType.Ignore))
                    return DeviceControlRegime.Ignore;

                if (DirectionState.States.Contains(XStateType.Norm))
                    return DeviceControlRegime.Automatic;

                return DeviceControlRegime.Manual;
            }
        }

		public bool IsControlRegime
		{
			get { return ControlRegime == DeviceControlRegime.Manual; }
		}

        void SendControlCommand(byte code)
        {
            ObjectCommandSendHelper.SendControlCommand(Direction, code);
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

        #region IWindowIdentity Members
        public string Guid
        {
            get { return Direction.UID.ToString(); }
        }
        #endregion
    }
}