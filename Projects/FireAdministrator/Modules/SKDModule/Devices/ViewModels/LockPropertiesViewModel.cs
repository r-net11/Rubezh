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
			SelectedDoorOpenMethod = doorConfiguration.DoorOpenMethod;
			UnlockHoldInterval = doorConfiguration.UnlockHoldInterval;
			CloseTimeout = doorConfiguration.CloseTimeout;
			IsBreakInAlarmEnable = doorConfiguration.IsBreakInAlarmEnable;
			IsDoorNotClosedAlarmEnable = doorConfiguration.IsDoorNotClosedAlarmEnable;
			IsSensorEnable = doorConfiguration.IsSensorEnable;
			IsDuressAlarmEnable = doorConfiguration.IsDuressAlarmEnable;
			IsRepeatEnterAlarmEnable = doorConfiguration.IsRepeatEnterAlarmEnable;

			LockIntervalsViewModel = new LockIntervalsViewModel(doorConfiguration);
			OnPropertyChanged(() => LockIntervalsViewModel);
		}

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

		bool _isBreakInAlarmEnable;
		public bool IsBreakInAlarmEnable
		{
			get { return _isBreakInAlarmEnable; }
			set
			{
				_isBreakInAlarmEnable = value;
				OnPropertyChanged(() => IsBreakInAlarmEnable);
				HasChanged = true;
				if (value)
				{
					IsSensorEnable = true;
				}
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
				if (value)
				{
					IsSensorEnable = true;
				}
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

				if (!value)
				{
					IsBreakInAlarmEnable = false;
					IsDoorNotClosedAlarmEnable = false;
				}
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

		public RelayCommand GetDoorConfigurationCommand { get; private set; }
		void OnGetDoorConfiguration()
		{
			var result = FiresecManager.FiresecService.SKDGetDoorConfiguration(Device);
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
			var result = FiresecManager.FiresecService.SKDSetDoorConfiguration(Device, doorConfiguration);
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
			doorConfiguration.DoorOpenMethod = SelectedDoorOpenMethod;
			doorConfiguration.UnlockHoldInterval = UnlockHoldInterval;
			doorConfiguration.CloseTimeout = CloseTimeout;
			doorConfiguration.IsBreakInAlarmEnable = IsBreakInAlarmEnable;
			doorConfiguration.IsDoorNotClosedAlarmEnable = IsDoorNotClosedAlarmEnable;
			doorConfiguration.IsSensorEnable = IsSensorEnable;
			doorConfiguration.IsRepeatEnterAlarmEnable = IsRepeatEnterAlarmEnable;
			doorConfiguration.IsDuressAlarmEnable = IsDuressAlarmEnable;
			doorConfiguration.DoorDayIntervalsCollection = LockIntervalsViewModel.GetModel();
			return doorConfiguration;
		}

		protected override bool Save()
		{
			if (HasChanged)
			{
				if (!MessageBoxService.ShowConfirmation2("Настройки не записаны в прибор. вы уверены, что хотите закрыть окно без записи в контроллер?"))
					return false;
			}
			Device.SKDDoorConfiguration = GetModel();
			return base.Save();
		}
	}
}