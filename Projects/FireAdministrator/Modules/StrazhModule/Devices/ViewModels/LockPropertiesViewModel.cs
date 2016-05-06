using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Documents;
using StrazhAPI.SKD;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using StrazhModule.Properties;

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
			InitAvailableWeeklyIntervals();
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
			if (doorConfiguration.WeeklyIntervalID >= 0)
				WeeklyInterval = CanSetTimeIntervals ? AvailableWeeklyIntervals.FirstOrDefault(x => x.ID == doorConfiguration.WeeklyIntervalID) : null;
			IsCloseCheckSensorEnable = doorConfiguration.IsCloseCheckSensor;
			IsRepeatEnterAlarmEnable = doorConfiguration.IsRepeatEnterAlarmEnable;
		}

		public ObservableCollection<SKDDoorConfiguration_DoorOpenMethod> DoorOpenMethods { get; private set; }

		private void InitDoorOpenMethods()
		{
			DoorOpenMethods = new ObservableCollection<SKDDoorConfiguration_DoorOpenMethod>
			{
				SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_CARD,
				SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_PWD_ONLY,
				SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_CARD_FIRST,
				SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_SECTION
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
				if (_doorOpenMethod == value)
					return;
				_doorOpenMethod = value;
				OnPropertyChanged(() => DoorOpenMethod);
				HasChanged = true;
				CanSetTimeIntervals = (value == SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_SECTION);
				WeeklyInterval = CanSetTimeIntervals
					? AvailableWeeklyIntervals.FirstOrDefault(x => x.Name == TimeIntervalsConfiguration.PredefinedIntervalNameCard)
					: null;
			}
		}

		public ObservableCollection<SKDDoorWeeklyInterval> AvailableWeeklyIntervals { get; private set; }

		private void InitAvailableWeeklyIntervals()
		{
			AvailableWeeklyIntervals = new ObservableCollection<SKDDoorWeeklyInterval>(
				SKDManager.SKDConfiguration.TimeIntervalsConfiguration.DoorWeeklyIntervals
			);
		}

		SKDDoorWeeklyInterval _weeklyInterval;
		public SKDDoorWeeklyInterval WeeklyInterval
		{
			get { return _weeklyInterval; }
			set
			{
				_weeklyInterval = value;
				OnPropertyChanged(() => WeeklyInterval);
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
					IsCloseCheckSensorEnable = false;
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

		bool _isCloseCheckSensorEnable;
		/// <summary>
		/// Закрывать замок при закрытии двери
		/// </summary>
		public bool IsCloseCheckSensorEnable
		{
			get { return _isCloseCheckSensorEnable; }
			set
			{
				if (_isCloseCheckSensorEnable == value)
					return;
				_isCloseCheckSensorEnable = value;
				OnPropertyChanged(() => IsCloseCheckSensorEnable);
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
				if (!DoorOpenMethods.Any(doorOpenMethod => doorOpenMethod.Equals(doorConfiguration.DoorOpenMethod)))
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
			if (DoorOpenMethod == SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_SECTION && !CheckDoorOpenMethodBySection())
				return;

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
				CloseTimeout = this.CloseTimeout,
				
				WeeklyIntervalID = this.CanSetTimeIntervals ? this.WeeklyInterval.ID : -1,
				DoorDayIntervalsCollection = GetDayIntervalsCollectionFromConfig(),
				IsCloseCheckSensor = this.IsCloseCheckSensorEnable,
				IsRepeatEnterAlarmEnable = this.IsRepeatEnterAlarmEnable
			};
		}

		private DoorDayIntervalsCollection GetDayIntervalsCollectionFromConfig()
		{
			const int daysPerWeek = 7;
			const int sectionsPerDay = 4;

			var doorDayIntervals = new List<DoorDayInterval>();
			for (var i = 0; i < daysPerWeek; i++)
			{
				var doorDayInterval = new DoorDayInterval();
				for (var j = 0; j < sectionsPerDay; j++)
				{
					doorDayInterval.DoorDayIntervalParts.Add(new DoorDayIntervalPart());
				}
				doorDayIntervals.Add(doorDayInterval);
			}
			var result = new DoorDayIntervalsCollection()
			{
				DoorDayIntervals = doorDayIntervals
			};

			if (!CanSetTimeIntervals || WeeklyInterval == null)
				return result;

			foreach (var weeklyIntervalPart in WeeklyInterval.WeeklyIntervalParts)
			{
				var doorDayInterval = doorDayIntervals[(int) weeklyIntervalPart.DayOfWeek - 1];
				var dayInterval = SKDManager.SKDConfiguration.TimeIntervalsConfiguration.DoorDayIntervals.FirstOrDefault(x => x.UID == weeklyIntervalPart.DayIntervalUID);
				if (dayInterval != null)
				{
					
					for (var i = 0; i < dayInterval.DayIntervalParts.Count; i++)
					{
						var dayIntervalPart = dayInterval.DayIntervalParts[i];
						var doorDayIntervalPart = doorDayInterval.DoorDayIntervalParts[i];

						var startTime = TimeSpan.FromMilliseconds(dayIntervalPart.StartMilliseconds);
						doorDayIntervalPart.StartHour = startTime.Hours;
						doorDayIntervalPart.StartMinute = startTime.Minutes;
		
						var endTime = TimeSpan.FromMilliseconds(dayIntervalPart.EndMilliseconds);
						doorDayIntervalPart.EndHour = endTime.Hours;
						doorDayIntervalPart.EndMinute = endTime.Minutes;

						doorDayIntervalPart.DoorOpenMethod = dayIntervalPart.DoorOpenMethod;
					}
				}
			}
			return result;
		}

		private bool CheckDoorOpenMethodBySection()
		{
			if (DoorOpenMethod == SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_SECTION && WeeklyInterval == null)
			{
				MessageBoxService.ShowWarning("График замка не выбран");
				return false;
			}
			return true;
		}

		protected override bool Save()
		{
			if (DoorOpenMethod == SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_SECTION && !CheckDoorOpenMethodBySection())
				return false;

			if (HasChanged)
			{
				if (!MessageBoxService.ShowConfirmation(Resources.SaveConfigurationControllerWarning))
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