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
using System.ComponentModel;

namespace StrazhModule.ViewModels
{
	public class LockPropertiesViewModel : SaveCancelDialogViewModel
	{
		public SKDDevice Device { get; private set; }
		bool HasChanged { get; set; }

		public LockPropertiesViewModel(SKDDevice device)
		{
			Title = "Параметры замка";
			Device = device;
			GetDoorConfigurationCommand = new RelayCommand(OnGetDoorConfiguration);
			SetDoorConfigurationCommand = new RelayCommand(OnSetDoorConfiguration);

			InitDoorOpenMethods();
			InitRemoteTimeoutDoorStatuses();

			SKDManager.InvalidateOneLockConfiguration(device);
			Update(device.SKDDoorConfiguration);
			HasChanged = false;
		}

		void Update(SKDDoorConfiguration doorConfiguration)
		{
			DoorOpenMethod = doorConfiguration.DoorOpenMethod;
			UnlockHoldInterval = doorConfiguration.UnlockHoldInterval;
			HandicapUnlockHoldInterval = doorConfiguration.HandicapTimeout.nUnlockHoldInterval;
			IsDuressAlarmEnable = doorConfiguration.IsDuressAlarmEnable;
			IsRemoteCheck = doorConfiguration.IsRemoteCheck;
			RemoteTimeout = doorConfiguration.RemoteDetail.TimeOut;
			RemoteTimeoutDoorStatus = doorConfiguration.RemoteDetail.TimeOutDoorStatus ? RemoteTimeoutDoorStatus.Open : RemoteTimeoutDoorStatus.Close;
			IsSensorEnable = doorConfiguration.IsSensorEnable;
			IsBreakInAlarmEnable = doorConfiguration.IsBreakInAlarmEnable;
			IsDoorNotClosedAlarmEnable = doorConfiguration.IsDoorNotClosedAlarmEnable;
			CloseTimeout = doorConfiguration.CloseTimeout;
			HandicapCloseTimeout = doorConfiguration.HandicapTimeout.nCloseTimeout;
		}

		public ObservableCollection<SKDDoorConfiguration_DoorOpenMethod> DoorOpenMethods { get; private set; }

		private void InitDoorOpenMethods()
		{
			DoorOpenMethods = new ObservableCollection<SKDDoorConfiguration_DoorOpenMethod>
			{
				SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_CARD,
				SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_PWD_ONLY,
				SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_CARD_FIRST
			};
		}

		SKDDoorConfiguration_DoorOpenMethod _doorOpenMethod;
		/// <summary>
		/// Метод открытия двери
		/// </summary>
		public SKDDoorConfiguration_DoorOpenMethod DoorOpenMethod
		{
			get { return _doorOpenMethod; }
			set
			{
				_doorOpenMethod = value;
				OnPropertyChanged(() => DoorOpenMethod);
				HasChanged = true;
			}
		}

		int _unlockHoldInterval;
		/// <summary>
		/// Время удержания
		/// </summary>
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

		int _handicapUnlockHoldInterval;
		/// <summary>
		/// Альтернативное время удержания
		/// </summary>
		public int HandicapUnlockHoldInterval
		{
			get { return _handicapUnlockHoldInterval; }
			set
			{
				_handicapUnlockHoldInterval = value;
				OnPropertyChanged(() => HandicapUnlockHoldInterval);
				HasChanged = true;
			}
		}

		bool _isDuressAlarmEnable;
		/// <summary>
		/// Тревога по принуждению
		/// </summary>
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

		bool _isRemoteCheck;
		/// <summary>
		/// Проход с подтверждением
		/// </summary>
		public bool IsRemoteCheck
		{
			get { return _isRemoteCheck; }
			set
			{
				_isRemoteCheck = value;
				OnPropertyChanged(() => IsRemoteCheck);
				HasChanged = true;
			}
		}

		int _remoteTimeout;
		/// <summary>
		/// Время ожидания (проход с подтверждением)
		/// </summary>
		public int RemoteTimeout
		{
			get { return _remoteTimeout; }
			set
			{
				_remoteTimeout = value;
				OnPropertyChanged(() => RemoteTimeout);
				HasChanged = true;
			}
		}

		public ObservableCollection<RemoteTimeoutDoorStatus> RemoteTimeoutDoorStatuses { get; private set; }

		private void InitRemoteTimeoutDoorStatuses()
		{
			RemoteTimeoutDoorStatuses = new ObservableCollection<RemoteTimeoutDoorStatus>
			{
				ViewModels.RemoteTimeoutDoorStatus.Close,
				ViewModels.RemoteTimeoutDoorStatus.Open
			};
		}

		RemoteTimeoutDoorStatus _remoteTimeoutDoorStatus;
		/// <summary>
		/// Состояние замка по истечению времени (проход с подтверждением)
		/// </summary>
		public RemoteTimeoutDoorStatus RemoteTimeoutDoorStatus
		{
			get { return _remoteTimeoutDoorStatus; }
			set
			{
				_remoteTimeoutDoorStatus = value;
				OnPropertyChanged(() => RemoteTimeoutDoorStatus);
				HasChanged = true;
			}
		}

		bool _isSensorEnable;
		/// <summary>
		/// Датчик контроля двери
		/// </summary>
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

		bool _isBreakInAlarmEnable;
		/// <summary>
		/// Тревога по взлому
		/// </summary>
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
		/// <summary>
		/// Тревога по незакрытию
		/// </summary>
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

		int _closeTimeout;
		/// <summary>
		/// Время закрытия
		/// </summary>
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

		int _handicapCloseTimeout;
		/// <summary>
		/// Альтернативное время закрытия
		/// </summary>
		public int HandicapCloseTimeout
		{
			get { return _handicapCloseTimeout; }
			set
			{
				_handicapCloseTimeout = value;
				OnPropertyChanged(() => HandicapCloseTimeout);
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
				if (doorConfiguration.DoorOpenMethod == SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_UNKNOWN)
				{
					MessageBoxService.ShowWarning("Для замка на контроллере не установлен метод открытия двери");
				}
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
			return new SKDDoorConfiguration()
			{
				DoorOpenMethod = this.DoorOpenMethod,
				UnlockHoldInterval = this.UnlockHoldInterval,
				HandicapTimeout = new HandicapTimeout()
				{
					nUnlockHoldInterval = this.HandicapUnlockHoldInterval,
					nCloseTimeout = this.HandicapCloseTimeout
				},
				IsDuressAlarmEnable = this.IsDuressAlarmEnable,
				IsRemoteCheck = this.IsRemoteCheck,
				RemoteDetail = new RemoteDetail()
				{
					TimeOut = this.RemoteTimeout,
					TimeOutDoorStatus = (this.RemoteTimeoutDoorStatus == RemoteTimeoutDoorStatus.Open ? true : false)
				},
				IsSensorEnable = this.IsSensorEnable,
				IsBreakInAlarmEnable = this.IsBreakInAlarmEnable,
				IsDoorNotClosedAlarmEnable = this.IsDoorNotClosedAlarmEnable,
				CloseTimeout = this.CloseTimeout
			};
		}

		protected override bool Save()
		{
			if (HasChanged)
			{
				if (!MessageBoxService.ShowConfirmation("Настройки не записаны в прибор. вы уверены, что хотите закрыть окно без записи в контроллер?"))
					return false;
			}
			Device.SKDDoorConfiguration = GetModel();
			return base.Save();
		}
	}

	public enum RemoteTimeoutDoorStatus
	{
		[Description("Закрыто")]
		Close,
		[Description("Открыто")]
		Open
	}
}