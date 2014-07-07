using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.SKD;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class LockPropertiesViewModel : SaveCancelDialogViewModel
	{
		public SKDDevice Device { get; private set; }
		public LockIntervalsViewModel LockIntervalsViewModel { get; private set; }

		public LockPropertiesViewModel(SKDDevice device)
		{
			Title = "Параметры двери";
			Device = device;
			GetDoorConfigurationCommand = new RelayCommand(OnGetDoorConfiguration);
			SetDoorConfigurationCommand = new RelayCommand(OnSetDoorConfiguration);

			AccessStates = new ObservableRangeCollection<SKDDoorConfiguration_AccessState>();
			AccessStates.Add(SKDDoorConfiguration_AccessState.ACCESS_STATE_NORMAL);
			AccessStates.Add(SKDDoorConfiguration_AccessState.ACCESS_STATE_CLOSEALWAYS);
			AccessStates.Add(SKDDoorConfiguration_AccessState.ACCESS_STATE_OPENALWAYS);

			AccessModes = new ObservableRangeCollection<SKDDoorConfiguration_AccessMode>();
			AccessModes.Add(SKDDoorConfiguration_AccessMode.ACCESS_MODE_HANDPROTECTED);
			AccessModes.Add(SKDDoorConfiguration_AccessMode.ACCESS_MODE_OTHER);
			AccessModes.Add(SKDDoorConfiguration_AccessMode.ACCESS_MODE_SAFEROOM);

			DoorOpenMethods = new ObservableRangeCollection<SKDDoorConfiguration_DoorOpenMethod>();
			DoorOpenMethods.Add(SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_CARD);
			DoorOpenMethods.Add(SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_PWD_ONLY);
			DoorOpenMethods.Add(SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_CARD_FIRST);
			DoorOpenMethods.Add(SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_PWD_FIRST);
			DoorOpenMethods.Add(SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_PWD_ONLY);
			DoorOpenMethods.Add(SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_PWD_OR_CARD);
			DoorOpenMethods.Add(SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_SECTION);

			if (device.SKDDoorConfiguration == null)
			{
				device.SKDDoorConfiguration = new SKDDoorConfiguration();
			}
			Update(device.SKDDoorConfiguration);
		}

		void Update(SKDDoorConfiguration doorConfiguration)
		{
			SelectedAccessMode = doorConfiguration.AccessMode;
			SelectedAccessState = doorConfiguration.AccessState;
			SelectedDoorOpenMethod = doorConfiguration.DoorOpenMethod;
			UnlockHoldInterval = doorConfiguration.UnlockHoldInterval;
			CloseTimeout = doorConfiguration.CloseTimeout;
			OpenAlwaysTimeIndex = doorConfiguration.OpenAlwaysTimeIndex;
			HolidayTimeRecoNo = doorConfiguration.HolidayTimeRecoNo;
			IsBreakInAlarmEnable = doorConfiguration.IsBreakInAlarmEnable;
			IsRepeatEnterAlarmEnable = doorConfiguration.IsRepeatEnterAlarmEnable;
			IsDoorNotClosedAlarmEnable = doorConfiguration.IsDoorNotClosedAlarmEnable;
			IsDuressAlarmEnable = doorConfiguration.IsDuressAlarmEnable;
			IsSensorEnable = doorConfiguration.IsSensorEnable;
			WeeklySheduleNo = doorConfiguration.WeeklySheduleNo;

			LockIntervalsViewModel = new LockIntervalsViewModel(doorConfiguration);
			OnPropertyChanged(() => LockIntervalsViewModel);
		}

		public ObservableRangeCollection<SKDDoorConfiguration_AccessState> AccessStates { get; private set; }

		SKDDoorConfiguration_AccessMode _selectedAccessMode;
		public SKDDoorConfiguration_AccessMode SelectedAccessMode
		{
			get { return _selectedAccessMode; }
			set
			{
				_selectedAccessMode = value;
				OnPropertyChanged(() => SelectedAccessMode);
			}
		}

		SKDDoorConfiguration_AccessState _selectedAccessState;
		public SKDDoorConfiguration_AccessState SelectedAccessState
		{
			get { return _selectedAccessState; }
			set
			{
				_selectedAccessState = value;
				OnPropertyChanged(() => SelectedAccessState);
				CanSelectDoorOpenMethod = value == SKDDoorConfiguration_AccessState.ACCESS_STATE_NORMAL;
			}
		}

		public ObservableRangeCollection<SKDDoorConfiguration_AccessMode> AccessModes { get; private set; }

		public ObservableRangeCollection<SKDDoorConfiguration_DoorOpenMethod> DoorOpenMethods { get; private set; }

		SKDDoorConfiguration_DoorOpenMethod _selectedDoorOpenMethod;
		public SKDDoorConfiguration_DoorOpenMethod SelectedDoorOpenMethod
		{
			get { return _selectedDoorOpenMethod; }
			set
			{
				_selectedDoorOpenMethod = value;
				OnPropertyChanged(() => SelectedDoorOpenMethod);
				CanSetTimeIntervals = value == SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_SECTION;
			}
		}

		bool _canSelectDoorOpenMethod;
		public bool CanSelectDoorOpenMethod
		{
			get { return _canSelectDoorOpenMethod; }
			set
			{
				_canSelectDoorOpenMethod = value;
				OnPropertyChanged(() => CanSelectDoorOpenMethod);
			}
		}

		bool _canSetTimeIntervals;
		public bool CanSetTimeIntervals
		{
			get { return _canSetTimeIntervals; }
			set
			{
				_canSetTimeIntervals = value;
				OnPropertyChanged(() => CanSetTimeIntervals);
			}
		}

		int _unlockHoldInterval;
		public int UnlockHoldInterval
		{
			get { return _unlockHoldInterval; }
			set
			{
				_unlockHoldInterval = value;
				OnPropertyChanged(() => UnlockHoldInterval);
			}
		}

		int _closeTimeout;
		public int CloseTimeout
		{
			get { return _closeTimeout; }
			set
			{
				_closeTimeout = value;
				OnPropertyChanged(() => CloseTimeout);
			}
		}

		int _openAlwaysTimeIndex;
		public int OpenAlwaysTimeIndex
		{
			get { return _openAlwaysTimeIndex; }
			set
			{
				_openAlwaysTimeIndex = value;
				OnPropertyChanged(() => OpenAlwaysTimeIndex);
			}
		}

		int _holidayTimeRecoNo;
		public int HolidayTimeRecoNo
		{
			get { return _holidayTimeRecoNo; }
			set
			{
				_holidayTimeRecoNo = value;
				OnPropertyChanged(() => HolidayTimeRecoNo);
			}
		}

		bool _isBreakInAlarmEnable;
		public bool IsBreakInAlarmEnable
		{
			get { return _isBreakInAlarmEnable; }
			set
			{
				_isBreakInAlarmEnable = value;
				OnPropertyChanged(() => IsBreakInAlarmEnable);
			}
		}

		bool _isRepeatEnterAlarmEnable;
		public bool IsRepeatEnterAlarmEnable
					{
			get { return _isRepeatEnterAlarmEnable; }
			set
			{
				_isRepeatEnterAlarmEnable = value;
				OnPropertyChanged(() => IsRepeatEnterAlarmEnable);
			}
		}

		bool _isDoorNotClosedAlarmEnable;
		public bool IsDoorNotClosedAlarmEnable
		{
			get { return _isDoorNotClosedAlarmEnable; }
			set
			{
				_isDoorNotClosedAlarmEnable = value;
				OnPropertyChanged(() => IsDoorNotClosedAlarmEnable);
			}
		}

		bool _isDuressAlarmEnable;
		public bool IsDuressAlarmEnable
		{
			get { return _isDuressAlarmEnable; }
			set
			{
				_isDuressAlarmEnable = value;
				OnPropertyChanged(() => IsDuressAlarmEnable);
			}
		}

		bool _isSensorEnable;
		public bool IsSensorEnable
		{
			get { return _isSensorEnable; }
			set
			{
				_isSensorEnable = value;
				OnPropertyChanged(() => IsSensorEnable);
			}
		}

		int _weeklySheduleNo;
		public int WeeklySheduleNo
		{
			get { return _weeklySheduleNo; }
			set
			{
				_weeklySheduleNo = value;
				OnPropertyChanged(() => WeeklySheduleNo);
			}
		}

		public RelayCommand GetDoorConfigurationCommand { get; private set; }
		void OnGetDoorConfiguration()
		{
			var result = FiresecManager.FiresecService.SKDGetDoorConfiguration(Device.UID);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}
			else
			{
				var doorConfiguration = result.Result;
				if (doorConfiguration.AccessState == SKDDoorConfiguration_AccessState.ACCESS_STATE_NORMAL && doorConfiguration.DoorOpenMethod == SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_UNKNOWN)
					MessageBoxService.ShowWarning("Неизвестный метод открытия двери");
				Update(doorConfiguration);
			}
		}

		public RelayCommand SetDoorConfigurationCommand { get; private set; }
		void OnSetDoorConfiguration()
		{
			var doorConfiguration = GetModel();
			var result = FiresecManager.FiresecService.SKDSetDoorConfiguration(Device.UID, doorConfiguration);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}
		}

		SKDDoorConfiguration GetModel()
		{
			var doorConfiguration = new SKDDoorConfiguration();
			doorConfiguration.AccessMode = SelectedAccessMode;
			doorConfiguration.AccessState = SelectedAccessState;
			doorConfiguration.DoorOpenMethod = SelectedDoorOpenMethod;
			doorConfiguration.UnlockHoldInterval = UnlockHoldInterval;
			doorConfiguration.CloseTimeout = CloseTimeout;
			doorConfiguration.OpenAlwaysTimeIndex = OpenAlwaysTimeIndex;
			doorConfiguration.HolidayTimeRecoNo = HolidayTimeRecoNo;
			doorConfiguration.IsBreakInAlarmEnable = IsBreakInAlarmEnable;
			doorConfiguration.IsRepeatEnterAlarmEnable = IsRepeatEnterAlarmEnable;
			doorConfiguration.IsDoorNotClosedAlarmEnable = IsDoorNotClosedAlarmEnable;
			doorConfiguration.IsDuressAlarmEnable = IsDuressAlarmEnable;
			doorConfiguration.IsSensorEnable = IsSensorEnable;
			doorConfiguration.WeeklySheduleNo = WeeklySheduleNo;
			return doorConfiguration;
		}

		protected override bool Save()
		{
			Device.SKDDoorConfiguration = GetModel();
			return base.Save();
		}
	}
}