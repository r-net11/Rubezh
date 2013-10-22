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
				if (DeviceState.StateBits.Contains(XStateBit.Ignore))
					return DeviceControlRegime.Ignore;

				if (DeviceState.StateBits.Contains(XStateBit.Norm))
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
			ObjectCommandSendHelper.SetAutomaticRegimeForDevice(Device);
		}

		public RelayCommand SetManualStateCommand { get; private set; }
		void OnSetManualState()
		{
			ObjectCommandSendHelper.SetManualRegimeForDevice(Device);
		}

		public RelayCommand SetIgnoreStateCommand { get; private set; }
		void OnSetIgnoreState()
		{
			ObjectCommandSendHelper.SetIgnoreRegimeForDevice(Device);
		}

		public ObservableCollection<DeviceExecutableCommandViewModel> DeviceExecutableCommands { get; private set; }

		void OnStateChanged()
		{
			OnPropertyChanged("ControlRegime");
			OnPropertyChanged("IsControlRegime");
		}

		public bool HasReset
		{
			get { return Device.Driver.DriverType == XDriverType.AMP_1 || Device.Driver.DriverType == XDriverType.RSR2_MAP4; }
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			ObjectCommandSendHelper.ResetDevice(Device);
		}
		bool CanReset()
		{
			return DeviceState.StateBits.Contains(XStateBit.Fire2) || DeviceState.StateBits.Contains(XStateBit.Fire1);
		}

		#region IsMRO
		public bool IsMRO
		{
			get { return Device.Driver.DriverType == XDriverType.MRO_2; }
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
			//code2 = 18;
			code2 = 20;
			ObjectCommandSendHelper.SendControlCommandMRO(Device, (byte)code, (byte)code2);
		}
		#endregion
	}
}