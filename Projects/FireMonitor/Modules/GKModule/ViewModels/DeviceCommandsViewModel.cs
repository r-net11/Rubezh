using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Collections.ObjectModel;
using FiresecClient;
using System.Collections.Generic;
using System;
using System.Linq;
using GKProcessor;
using Infrastructure;

namespace GKModule.ViewModels
{
	public class DeviceCommandsViewModel : BaseViewModel
	{
		public XState DeviceState { get; private set; }
		public XDevice Device { get { return DeviceState.Device; } }

		public DeviceCommandsViewModel(XState deviceState)
		{
			DeviceState = deviceState;
			DeviceState.StateChanged -= new System.Action(OnStateChanged);
			DeviceState.StateChanged += new System.Action(OnStateChanged);

			SetAutomaticStateCommand = new RelayCommand(OnSetAutomaticState, CanSetAutomaticState);
			SetManualStateCommand = new RelayCommand(OnSetManualState, CanSetManualState);
			SetIgnoreStateCommand = new RelayCommand(OnSetIgnoreState, CanSetIgnoreState);
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
			get { return Device.Driver.IsControlDevice && FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices); }
		}

		public bool IsBiStateControl
		{
			get { return Device.Driver.IsDeviceOnShleif && !Device.Driver.IsControlDevice && FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices); }
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
            if (ServiceFactory.SecurityService.Validate())
            {
				FiresecManager.FiresecService.GKSetAutomaticRegime(Device);
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
				FiresecManager.FiresecService.GKSetManualRegime(Device);
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
				FiresecManager.FiresecService.GKSetIgnoreRegime(Device);
            }
        }
		bool CanSetIgnoreState()
		{
			return ControlRegime != DeviceControlRegime.Ignore;
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
            if (ServiceFactory.SecurityService.Validate())
            {
				FiresecManager.FiresecService.GKReset(Device);
            }
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
			Watcher.SendControlCommandMRO(Device, (byte)code, (byte)code2);
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