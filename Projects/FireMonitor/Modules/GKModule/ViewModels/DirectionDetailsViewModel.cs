using System;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Diagnostics;

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
			OnPropertyChanged("ControlRegime");
			OnPropertyChanged("IsControlRegime");
			OnPropertyChanged("DirectionState");
			OnPropertyChanged("HasOnDelay");
			OnPropertyChanged("HasHoldDelay");
			OnPropertyChanged("HasOffDelay");

			Trace.WriteLine("DirectionState.OnDelay " + DirectionState.OnDelay);
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

		public RelayCommand SetAutomaticStateCommand { get; private set; }
		void OnSetAutomaticState()
		{
			ObjectCommandSendHelper.SendControlCommand(Direction, XStateType.SetRegime_Automatic);
		}

		public RelayCommand SetManualStateCommand { get; private set; }
		void OnSetManualState()
		{
			ObjectCommandSendHelper.SendControlCommand(Direction, XStateType.SetRegime_Manual);
		}

		public RelayCommand SetIgnoreStateCommand { get; private set; }
		void OnSetIgnoreState()
		{
			ObjectCommandSendHelper.SendControlCommand(Direction, XStateType.SetRegime_Off);
		}

		public RelayCommand TurnOnCommand { get; private set; }
		void OnTurnOn()
		{
			ObjectCommandSendHelper.SendControlCommand(Direction, XStateType.TurnOn_InManual);
		}

		public RelayCommand TurnOnNowCommand { get; private set; }
		void OnTurnOnNow()
		{
			ObjectCommandSendHelper.SendControlCommand(Direction, XStateType.TurnOnNow_InManual);
		}

		public RelayCommand TurnOffCommand { get; private set; }
		void OnTurnOff()
		{
			ObjectCommandSendHelper.SendControlCommand(Direction, XStateType.TurnOff_InManual);
		}

		public bool HasOnDelay
		{
			get { return DirectionState.OnDelay > 0; }
		}
		public bool HasHoldDelay
		{
			get { return DirectionState.HoldDelay > 0; }
		}
		public bool HasOffDelay
		{
			get { return DirectionState.OffDelay > 0; }
		}

		#region IWindowIdentity Members
		public string Guid
		{
			get { return Direction.UID.ToString(); }
		}
		#endregion
	}
}