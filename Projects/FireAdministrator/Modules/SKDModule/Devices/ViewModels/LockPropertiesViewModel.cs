using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.SKD;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecClient;
using System.Collections.ObjectModel;

namespace SKDModule.ViewModels
{
	public class LockPropertiesViewModel : SaveCancelDialogViewModel
	{
		public SKDDevice Device { get; private set; }
		public LockIntervalsViewModel LockIntervalsViewModel { get; private set; }
		bool HasChanged { get; set; }

		public LockPropertiesViewModel(SKDDevice device)
		{
			Title = "Параметры замка";
			Device = device;
			GetDoorConfigurationCommand = new RelayCommand(OnGetDoorConfiguration);
			SetDoorConfigurationCommand = new RelayCommand(OnSetDoorConfiguration);

			AccessStates = new ObservableCollection<SKDDoorConfiguration_AccessState>();
			AccessStates.Add(SKDDoorConfiguration_AccessState.ACCESS_STATE_NORMAL);
			AccessStates.Add(SKDDoorConfiguration_AccessState.ACCESS_STATE_CLOSEALWAYS);
			AccessStates.Add(SKDDoorConfiguration_AccessState.ACCESS_STATE_OPENALWAYS);

			AccessModes = new ObservableCollection<SKDDoorConfiguration_AccessMode>();
			AccessModes.Add(SKDDoorConfiguration_AccessMode.ACCESS_MODE_HANDPROTECTED);
			AccessModes.Add(SKDDoorConfiguration_AccessMode.ACCESS_MODE_OTHER);
			AccessModes.Add(SKDDoorConfiguration_AccessMode.ACCESS_MODE_SAFEROOM);

			DoorOpenMethods = new ObservableCollection<SKDDoorConfiguration_DoorOpenMethod>();
			DoorOpenMethods.Add(SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_CARD);
			DoorOpenMethods.Add(SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_PWD_ONLY);
			DoorOpenMethods.Add(SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_CARD_FIRST);
			DoorOpenMethods.Add(SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_PWD_FIRST);
			DoorOpenMethods.Add(SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_PWD_OR_CARD);
			DoorOpenMethods.Add(SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_SECTION);

			SKDManager.InvalidateOneLockConfiguration(device);
			Update(device.SKDDoorConfiguration);
			HasChanged = false;
		}

		void Update(SKDDoorConfiguration doorConfiguration)
		{
			SelectedAccessMode = doorConfiguration.AccessMode;
			SelectedAccessState = doorConfiguration.AccessState;
			SelectedDoorOpenMethod = doorConfiguration.DoorOpenMethod;
			UnlockHoldInterval = doorConfiguration.UnlockHoldInterval;
			CloseTimeout = doorConfiguration.CloseTimeout;
			HolidayTimeRecoNo = doorConfiguration.HolidayTimeRecoNo;
			IsBreakInAlarmEnable = doorConfiguration.IsBreakInAlarmEnable;
			IsRepeatEnterAlarmEnable = doorConfiguration.IsRepeatEnterAlarmEnable;
			IsDoorNotClosedAlarmEnable = doorConfiguration.IsDoorNotClosedAlarmEnable;
			IsDuressAlarmEnable = doorConfiguration.IsDuressAlarmEnable;
			IsSensorEnable = doorConfiguration.IsSensorEnable;

			TimeSchedules = new ObservableCollection<SKDWeeklyInterval>();
			foreach (var weeklyInterval in SKDManager.TimeIntervalsConfiguration.WeeklyIntervals)
			{
				TimeSchedules.Add(weeklyInterval);
			}
			SelectedTimeSchedule = TimeSchedules.FirstOrDefault(x => x.ID == doorConfiguration.OpenAlwaysTimeIndex);

			LockIntervalsViewModel = new LockIntervalsViewModel(doorConfiguration);
			OnPropertyChanged(() => LockIntervalsViewModel);
		}

		public ObservableCollection<SKDDoorConfiguration_AccessState> AccessStates { get; private set; }

		SKDDoorConfiguration_AccessMode _selectedAccessMode;
		public SKDDoorConfiguration_AccessMode SelectedAccessMode
		{
			get { return _selectedAccessMode; }
			set
			{
				_selectedAccessMode = value;
				OnPropertyChanged(() => SelectedAccessMode);
				HasChanged = true;
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
				HasChanged = true;
				CanSelectDoorOpenMethod = value == SKDDoorConfiguration_AccessState.ACCESS_STATE_NORMAL;
			}
		}

		public ObservableCollection<SKDDoorConfiguration_AccessMode> AccessModes { get; private set; }

		public ObservableCollection<SKDDoorConfiguration_DoorOpenMethod> DoorOpenMethods { get; private set; }

		SKDDoorConfiguration_DoorOpenMethod _selectedDoorOpenMethod;
		public SKDDoorConfiguration_DoorOpenMethod SelectedDoorOpenMethod
		{
			get { return _selectedDoorOpenMethod; }
			set
			{
				_selectedDoorOpenMethod = value;
				OnPropertyChanged(() => SelectedDoorOpenMethod);
				HasChanged = true;
				CanSetTimeIntervals = value == SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_SECTION;
				CanSetAlwaysOpen = value != SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_SECTION;
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
				HasChanged = true;
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
				HasChanged = true;
			}
		}

		bool _canSetAlwaysOpen;
		public bool CanSetAlwaysOpen
		{
			get { return _canSetAlwaysOpen; }
			set
			{
				_canSetAlwaysOpen = value;
				OnPropertyChanged(() => CanSetAlwaysOpen);
				HasChanged = true;
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
				HasChanged = true;
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
				HasChanged = true;
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
				HasChanged = true;
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
				HasChanged = true;
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
				HasChanged = true;
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
				HasChanged = true;
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
				HasChanged = true;
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
				HasChanged = true;
			}
		}

		public ObservableCollection<SKDWeeklyInterval> TimeSchedules { get; private set; }

		SKDWeeklyInterval _selectedTimeSchedule;
		public SKDWeeklyInterval SelectedTimeSchedule
		{
			get { return _selectedTimeSchedule; }
			set
			{
				_selectedTimeSchedule = value;
				OnPropertyChanged(() => SelectedTimeSchedule);
				HasChanged = true;
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
				HasChanged = false;
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
			else
			{
				HasChanged = false;
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
			doorConfiguration.OpenAlwaysTimeIndex = SelectedTimeSchedule != null ? SelectedTimeSchedule.ID : -1;
			doorConfiguration.HolidayTimeRecoNo = HolidayTimeRecoNo;
			doorConfiguration.IsBreakInAlarmEnable = IsBreakInAlarmEnable;
			doorConfiguration.IsRepeatEnterAlarmEnable = IsRepeatEnterAlarmEnable;
			doorConfiguration.IsDoorNotClosedAlarmEnable = IsDoorNotClosedAlarmEnable;
			doorConfiguration.IsDuressAlarmEnable = IsDuressAlarmEnable;
			doorConfiguration.IsSensorEnable = IsSensorEnable;
			doorConfiguration.DoorDayIntervalsCollection = LockIntervalsViewModel.GetModel();
			return doorConfiguration;
		}

		protected override bool Save()
		{
			if (HasChanged)
			{
				if (!MessageBoxService.ShowConfirmation2("Настройки не записаны в прибор. вы уверены, что хотите закрыть окно без записи в прибор?"))
					return false;
			}
			Device.SKDDoorConfiguration = GetModel();
			return base.Save();
		}
	}
}