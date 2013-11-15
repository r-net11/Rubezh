using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Collections.ObjectModel;
using FiresecClient;
using System.Collections.Generic;
using System;
using System.Linq;

namespace GKModule.ViewModels
{
	public class DeviceCommandsViewModel : BaseViewModel
	{
		public XDeviceState DeviceState { get; private set; }
		public XDevice Device { get { return DeviceState.Device; } }

		public DeviceCommandsViewModel(XDeviceState deviceState)
		{
			DeviceState = deviceState;
			DeviceState.StateChanged -= new System.Action(OnStateChanged);
			DeviceState.StateChanged += new System.Action(OnStateChanged);

			SetAutomaticStateCommand = new RelayCommand(OnSetAutomaticState);
			SetManualStateCommand = new RelayCommand(OnSetManualState);
			SetIgnoreStateCommand = new RelayCommand(OnSetIgnoreState);
			ResetCommand = new RelayCommand(OnReset, CanReset);
			ExecuteMROCommand = new RelayCommand(OnExecuteMRO);

			DeviceExecutableCommands = new ObservableCollection<DeviceExecutableCommandViewModel>();
			foreach (var availableCommand in Device.Driver.AvailableCommandBits)
			{
				var deviceExecutableCommandViewModel = new DeviceExecutableCommandViewModel(Device, availableCommand);
				DeviceExecutableCommands.Add(deviceExecutableCommandViewModel);
			}
			if (Device.DriverType == XDriverType.Pump && Device.IntAddress == 12)
			{
				var deviceExecutableCommandViewModel = new DeviceExecutableCommandViewModel(Device, XStateBit.ForbidStart_InManual);
				DeviceExecutableCommands.Add(deviceExecutableCommandViewModel);
			}
		}

		public bool IsTriStateControl
		{
			get { return Device.Driver.IsControlDevice; }
		}

		public bool IsBiStateControl
		{
			get { return Device.Driver.IsDeviceOnShleif && !Device.Driver.IsControlDevice; }
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
			get { return ControlRegime == DeviceControlRegime.Manual; }
		}

		public RelayCommand SetAutomaticStateCommand { get; private set; }
		void OnSetAutomaticState()
		{
			ObjectCommandSendHelper.SetAutomaticRegime(Device);
		}

		public RelayCommand SetManualStateCommand { get; private set; }
		void OnSetManualState()
		{
			ObjectCommandSendHelper.SetManualRegime(Device);
		}

		public RelayCommand SetIgnoreStateCommand { get; private set; }
		void OnSetIgnoreState()
		{
			ObjectCommandSendHelper.SetIgnoreRegime(Device);
		}

		public ObservableCollection<DeviceExecutableCommandViewModel> DeviceExecutableCommands { get; private set; }

		void OnStateChanged()
		{
			OnPropertyChanged("ControlRegime");
			OnPropertyChanged("IsControlRegime");
		}

		public bool HasReset
		{
			get { return Device.DriverType == XDriverType.AMP_1 || Device.DriverType == XDriverType.RSR2_MAP4; }
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			ObjectCommandSendHelper.Reset(Device);
		}
		bool CanReset()
		{
			return DeviceState.StateClasses.Contains(XStateClass.Fire2) || DeviceState.StateClasses.Contains(XStateClass.Fire1);
		}

		#region IsMRO
		public bool IsMRO
		{
			get
			{
#if DEBUG
				return Device.DriverType == XDriverType.MRO_2;
#endif
				return false;
			}
		}

		public List<ZoneLogicMROMessageNo> AvailableMROMessageNos
		{
			get { return Enum.GetValues(typeof(ZoneLogicMROMessageNo)).Cast<ZoneLogicMROMessageNo>().ToList(); }
		}

		ZoneLogicMROMessageNo _selectedMROMessageNo;
		public ZoneLogicMROMessageNo SelectedMROMessageNo
		{
			get { return _selectedMROMessageNo; }
			set
			{
				_selectedMROMessageNo = value;
				OnPropertyChanged("SelectedMROMessageNo");
			}
		}

		public List<ZoneLogicMROMessageType> AvailableMROMessageTypes
		{
			get { return Enum.GetValues(typeof(ZoneLogicMROMessageType)).Cast<ZoneLogicMROMessageType>().ToList(); }
		}

		ZoneLogicMROMessageType _selectedMROMessageType;
		public ZoneLogicMROMessageType SelectedMROMessageType
		{
			get { return _selectedMROMessageType; }
			set
			{
				_selectedMROMessageType = value;
				OnPropertyChanged("SelectedMROMessageType");
			}
		}

		public RelayCommand ExecuteMROCommand { get; private set; }
		void OnExecuteMRO()
		{
			var code = 0x80 + (int)XStateBit.TurnOnNow_InManual;
			var code2 = 0;
			code2 += ((byte)SelectedMROMessageNo << 1);
			code2 += ((byte)SelectedMROMessageType << 4);
			code2 = 18;
			code2 = 20;
            code2 = MROCode;
			ObjectCommandSendHelper.SendControlCommandMRO(Device, (byte)code, (byte)code2);
		}

        int _mroCode;
        public int MROCode
        {
            get { return _mroCode; }
            set
            {
                _mroCode = value;
                OnPropertyChanged("MROCode");
            }
        }
		#endregion
	}
}