using Common;
using Localization.StrazhDeviceSDK.Common;
using Localization.StrazhDeviceSDK.Errors;
using StrazhAPI;
using StrazhAPI.SKD;
using StrazhDeviceSDK.API;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StrazhDeviceSDK
{
	public static partial class Processor
	{
		/// <summary>
		/// Получает информацию о контроллере.
		/// Такую как версия прошивки, сетевые настройки, дата и время.
		/// </summary>
		/// <param name="deviceUID">Идентификатор контроллера</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public static OperationResult<SKDDeviceInfo> GetDeviceInfo(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<SKDDeviceInfo>.FromError(String.Format(CommonResources.NoLinkWithController, deviceProcessor.Device.Name, deviceProcessor.LoginFailureReason));

				var deviceInfo = new SKDDeviceInfo();
				var deviceSoftwareInfo = deviceProcessor.Wrapper.GetDeviceSoftwareInfo();
				if (deviceSoftwareInfo != null)
				{
					deviceInfo.DeviceType = deviceSoftwareInfo.DeviceType;
					deviceInfo.SoftwareBuildDate = deviceSoftwareInfo.SoftwareBuildDate;
					deviceInfo.SoftwareVersion = deviceSoftwareInfo.SoftwareVersion;
				}
				else
				{
					return OperationResult<SKDDeviceInfo>.FromError(CommonErrors.ExecuteOperationRequestInfoController_Error);
				}

				var deviceNetInfo = deviceProcessor.Wrapper.GetDeviceNetInfo();
				if (deviceNetInfo != null)
				{
					deviceInfo.IP = deviceNetInfo.Address;
					deviceInfo.SubnetMask = deviceNetInfo.Mask;
					deviceInfo.DefaultGateway = deviceNetInfo.DefaultGateway;
				}
				else
				{
					return OperationResult<SKDDeviceInfo>.FromError(CommonErrors.ExecuteOperationRequestNetController_Error);
				}

				var dateTime = deviceProcessor.Wrapper.GetDateTime();
				if (dateTime > DateTime.MinValue)
				{
					deviceInfo.CurrentDateTime = deviceProcessor.Wrapper.GetDateTime();
				}
				else
				{
					return OperationResult<SKDDeviceInfo>.FromError(CommonErrors.ExecuteOperationRequestTimeController_Error);
				}

				return new OperationResult<SKDDeviceInfo>(deviceInfo);
			}
			return OperationResult<SKDDeviceInfo>.FromError(CommonResources.ControllerNotFoundInConfig);
		}

		/// <summary>
		/// Устанавливает на контроллере текущее системное время
		/// </summary>
		/// <param name="deviceUID">Идентификатор контроллера</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public static OperationResult<bool> SynchronizeTime(Guid deviceUID)
		{
			if (DeviceProcessors == null)
			{
				Logger.Info("SynchronizeTime error. DeviceProcessors is null.");
				return new OperationResult<bool>(false);
			}

			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<bool>.FromError(string.Format(CommonResources.NoLinkWithController, deviceProcessor.Device.Name, deviceProcessor.LoginFailureReason));

				if (deviceProcessor.Wrapper.SetDateTime(DateTime.UtcNow.Add(TimeZoneTypeToTimeSpan(deviceProcessor.Wrapper.GetControllerTimeSettings().TimeZone))))
					return new OperationResult<bool>(true);

				return OperationResult<bool>.FromError(CommonErrors.ExecuteOperationSynchTime_Error);
			}
			return OperationResult<bool>.FromError(CommonResources.ControllerNotFoundInConfig);
		}

		public static OperationResult<bool> ResetController(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<bool>.FromError(string.Format(CommonResources.NoLinkWithController, deviceProcessor.Device.Name, deviceProcessor.LoginFailureReason));

				var result = deviceProcessor.Wrapper.Reset();
				if (result)
					return new OperationResult<bool>(true);
				else
					return OperationResult<bool>.FromError(CommonErrors.ExecuteOperationDevice_Error);
			}
			return OperationResult<bool>.FromError(CommonResources.ControllerNotFoundInConfig);
		}

		public static OperationResult<bool> RebootController(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<bool>.FromError(string.Format(CommonResources.NoLinkWithController, deviceProcessor.Device.Name, deviceProcessor.LoginFailureReason));

				var result = deviceProcessor.Wrapper.Reboot();
				deviceProcessor.Reconnect();
				if (result)
					return new OperationResult<bool>(true);
				else
					return OperationResult<bool>.FromError(CommonErrors.ExecuteOperationDevice_Error);
			}
			return OperationResult<bool>.FromError(CommonResources.ControllerNotFoundInConfig);
		}

		/// <summary>
		/// Записывает графики доступа на контроллер
		/// </summary>
		/// <param name="deviceUID">Идентификатор контроллера, на который производится запись</param>
		/// <param name="doProgress">Использовать ли индикатор выполнения задачи</param>
		/// <returns>Объект OperationResult, описывающий результат выполнения процедуры записи</returns>
		public static OperationResult<bool> SKDWriteTimeSheduleConfiguration(Guid deviceUID, bool doProgress = true)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
#if DEBUG
				Logger.Info(String.Format("Выполняется Processor.SKDWriteTimeSheduleConfiguration для контроллера {0}", deviceProcessor.Device.Name));
#endif
				if (!deviceProcessor.IsConnected)
					return OperationResult<bool>.FromError(string.Format(CommonResources.NoLinkWithController, deviceProcessor.Device.Name, deviceProcessor.LoginFailureReason));

				SKDProgressCallback progressCallback = null;

				// Показываем индикатор хода выполнения операции
				if (doProgress)
					progressCallback = StartProgress(string.Format(CommonResources.WriteAccessSchedulesOnController, deviceProcessor.Device.Name), null, 128, true, SKDProgressClientType.Administrator);

				for (var i = 0; i <= 127; i++)
				{
					var weeklyInterval = SKDManager.SKDConfiguration.TimeIntervalsConfiguration.WeeklyIntervals.FirstOrDefault(x => x.ID == i) ??
										 new SKDWeeklyInterval();

					var timeShedules = new List<TimeShedule>();
					foreach (var weeklyIntervalPart in weeklyInterval.WeeklyIntervalParts.OrderBy(x => x.DayOfWeek))
					{
						var timeShedule = new TimeShedule();
						var dayInterval = SKDManager.SKDConfiguration.TimeIntervalsConfiguration.DayIntervals.FirstOrDefault(x => x.UID == weeklyIntervalPart.DayIntervalUID);

						if (i == 0)
						{
							dayInterval = new SKDDayInterval();
							dayInterval.DayIntervalParts = new List<SKDDayIntervalPart>();
							var emptySKDTimeIntervalPart = new SKDDayIntervalPart
							{
								StartMilliseconds = 0,
								EndMilliseconds = 0
							};
							dayInterval.DayIntervalParts.Add(emptySKDTimeIntervalPart);
						}
						if (i == 1)
						{
							dayInterval = new SKDDayInterval();
							dayInterval.DayIntervalParts = new List<SKDDayIntervalPart>();
							var emptySKDTimeIntervalPart = new SKDDayIntervalPart
							{
								StartMilliseconds = 0,
								EndMilliseconds = new TimeSpan(23, 59, 59).TotalMilliseconds
							};
							dayInterval.DayIntervalParts.Add(emptySKDTimeIntervalPart);
						}

						if (dayInterval != null)
						{
							foreach (var dayIntervalPart in dayInterval.DayIntervalParts)
							{
								var timeSheduleInterval = new TimeSheduleInterval
								{
									BeginHours = TimeSpan.FromMilliseconds(dayIntervalPart.StartMilliseconds).Hours,
									BeginMinutes = TimeSpan.FromMilliseconds(dayIntervalPart.StartMilliseconds).Minutes,
									BeginSeconds = TimeSpan.FromMilliseconds(dayIntervalPart.StartMilliseconds).Seconds,
									EndHours = TimeSpan.FromMilliseconds(dayIntervalPart.EndMilliseconds).Hours,
									EndMinutes = TimeSpan.FromMilliseconds(dayIntervalPart.EndMilliseconds).Minutes,
									EndSeconds = TimeSpan.FromMilliseconds(dayIntervalPart.EndMilliseconds).Seconds
								};

								timeShedule.TimeSheduleIntervals.Add(timeSheduleInterval);
							}
							for (var j = timeShedule.TimeSheduleIntervals.Count; j < 4; j++)
							{
								var timeSheduleInterval = new TimeSheduleInterval();
								timeShedule.TimeSheduleIntervals.Add(timeSheduleInterval);
							}
						}
						timeShedules.Add(timeShedule);
					}

					// Выполнение операции прервано пользователем
					if (progressCallback != null && progressCallback.IsCanceled)
						return OperationResult<bool>.FromCancel(String.Format(CommonResources.WriteAccessSchedulesOnControllerCanceled, deviceProcessor.Device.Name));


					// Обновляем индикатор хода выполнения операции
					if (progressCallback != null)
						DoProgress(null, progressCallback);

					var result = deviceProcessor.Wrapper.SetTimeShedules(i, timeShedules);
					if (!result)
					{
						// Останавливаем индикатор хода выполнения операции
						if (progressCallback != null)
							StopProgress(progressCallback);

						return OperationResult<bool>.FromError(String.Format(CommonErrors.ExecuteOperationWriteAccessSchedule_Error, deviceProcessor.Device.Name));
					}
				}

				// Останавливаем индикатор хода выполнения операции
				if (progressCallback != null)
					StopProgress(progressCallback);

				return new OperationResult<bool>(true);
			}
			return OperationResult<bool>.FromError(CommonResources.ControllerNotFoundInConfig);
		}

		public static OperationResult<SKDDoorConfiguration> GetDoorConfiguration(Guid deviceUID)
		{
			var readerDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (readerDevice == null)
				return null;
			var controllerDevice = readerDevice.Parent;
			if (controllerDevice == null)
				return null;

			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == controllerDevice.UID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<SKDDoorConfiguration>.FromError(string.Format(CommonResources.NoLinkWithController, deviceProcessor.Device.Name, deviceProcessor.LoginFailureReason));

				var nativeDoorConfiguration = deviceProcessor.Wrapper.GetDoorConfiguration(readerDevice.IntAddress);
				if (nativeDoorConfiguration == null)
					return OperationResult<SKDDoorConfiguration>.FromError(CommonErrors.ExecuteOperationDevice_Error);

				var doorConfiguration = new SKDDoorConfiguration();
				doorConfiguration.AccessState = (StrazhAPI.SKD.AccessState)nativeDoorConfiguration.AccessState;
				doorConfiguration.DoorOpenMethod = (SKDDoorConfiguration_DoorOpenMethod)nativeDoorConfiguration.DoorOpenMethod;
				doorConfiguration.UnlockHoldInterval = nativeDoorConfiguration.UnlockHoldInterval;
				doorConfiguration.HandicapTimeout.nUnlockHoldInterval = nativeDoorConfiguration.HandicapTimeout.nUnlockHoldInterval;
				doorConfiguration.IsDuressAlarmEnable = nativeDoorConfiguration.IsDuressAlarmEnable;
				doorConfiguration.IsRemoteCheck = nativeDoorConfiguration.IsRemoteCheck;
				doorConfiguration.RemoteDetail.TimeOut = nativeDoorConfiguration.RemoteDetail.nTimeOut;
				doorConfiguration.RemoteDetail.TimeOutDoorStatus = nativeDoorConfiguration.RemoteDetail.bTimeOutDoorStatus;
				doorConfiguration.IsSensorEnable = nativeDoorConfiguration.IsSensorEnable;
				doorConfiguration.IsBreakInAlarmEnable = nativeDoorConfiguration.IsBreakInAlarmEnable;
				doorConfiguration.IsDoorNotClosedAlarmEnable = nativeDoorConfiguration.IsDoorNotClosedAlarmEnable;
				doorConfiguration.CloseTimeout = nativeDoorConfiguration.CloseTimeout;
				doorConfiguration.HandicapTimeout.nCloseTimeout = nativeDoorConfiguration.HandicapTimeout.nCloseTimeout;

				doorConfiguration.HolidayTimeRecoNo = nativeDoorConfiguration.HolidayTimeRecoNo;
				doorConfiguration.IsRepeatEnterAlarmEnable = nativeDoorConfiguration.IsRepeatEnterAlarmEnable;
				doorConfiguration.DoorDayIntervalsCollection = nativeDoorConfiguration.DoorDayIntervalsCollection;
				doorConfiguration.IsCloseCheckSensor = nativeDoorConfiguration.IsCloseCheckSensor;

				return new OperationResult<SKDDoorConfiguration>(doorConfiguration);
			}
			return OperationResult<SKDDoorConfiguration>.FromError(CommonResources.ControllerNotFoundInConfig);
		}

		/// <summary>
		/// Записывает конфигурацию замка на контроллере
		/// </summary>
		/// <param name="deviceUID">Идентификатор замка на контроллере, на который записывается конфигурация</param>
		/// <param name="doorConfiguration">Записываемая конфигурация</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public static OperationResult<bool> SetDoorConfiguration(Guid deviceUID, SKDDoorConfiguration doorConfiguration)
		{
			var lockDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (lockDevice == null)
				return OperationResult<bool>.FromError(CommonResources.LockNotFoundInConfig);
			var controllerDevice = lockDevice.Parent;
			if (controllerDevice == null)
				return OperationResult<bool>.FromError(CommonResources.ControllerNotFoundInConfig);

			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == controllerDevice.UID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<bool>.FromError(String.Format(CommonResources.NoLinkWithController, deviceProcessor.Device.Name, deviceProcessor.LoginFailureReason));

				var nativeDoorConfiguration = new DoorConfiguration();
				nativeDoorConfiguration.AccessState = (API.AccessState)doorConfiguration.AccessState;
				nativeDoorConfiguration.DoorOpenMethod = (DoorOpenMethod)doorConfiguration.DoorOpenMethod;

				nativeDoorConfiguration.UnlockHoldInterval = doorConfiguration.UnlockHoldInterval;
				nativeDoorConfiguration.HandicapTimeout = new HandicapTimeoutInfo
				{
					nUnlockHoldInterval = doorConfiguration.HandicapTimeout.nUnlockHoldInterval,
					nCloseTimeout = doorConfiguration.HandicapTimeout.nCloseTimeout
				};
				nativeDoorConfiguration.IsDuressAlarmEnable = doorConfiguration.IsDuressAlarmEnable;
				nativeDoorConfiguration.IsRemoteCheck = doorConfiguration.IsRemoteCheck;
				nativeDoorConfiguration.RemoteDetail = new RemoteDetailInfo
				{
					nTimeOut = doorConfiguration.RemoteDetail.TimeOut,
					bTimeOutDoorStatus = doorConfiguration.RemoteDetail.TimeOutDoorStatus
				};
				nativeDoorConfiguration.IsSensorEnable = doorConfiguration.IsSensorEnable;
				nativeDoorConfiguration.IsBreakInAlarmEnable = doorConfiguration.IsBreakInAlarmEnable;
				nativeDoorConfiguration.IsDoorNotClosedAlarmEnable = doorConfiguration.IsDoorNotClosedAlarmEnable;
				nativeDoorConfiguration.CloseTimeout = doorConfiguration.CloseTimeout;

				nativeDoorConfiguration.OpenAlwaysTimeIndex = 255;
				nativeDoorConfiguration.HolidayTimeRecoNo = doorConfiguration.HolidayTimeRecoNo;
				nativeDoorConfiguration.IsRepeatEnterAlarmEnable = doorConfiguration.IsRepeatEnterAlarmEnable;
				nativeDoorConfiguration.DoorDayIntervalsCollection = doorConfiguration.DoorDayIntervalsCollection;
				nativeDoorConfiguration.IsCloseCheckSensor = doorConfiguration.IsCloseCheckSensor;

				var result = deviceProcessor.Wrapper.SetDoorConfiguration(nativeDoorConfiguration, lockDevice.IntAddress);
				return result
					? new OperationResult<bool>(true)
					: OperationResult<bool>.FromError(CommonErrors.ExecuteOperationWriteConfig_Error);
			}
			return OperationResult<bool>.FromError(CommonResources.ControllerNotFoundInConfig);
		}

		public static OperationResult<DoorType> GetControllerDoorType(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<DoorType>.FromError(string.Format(CommonResources.NoLinkWithController, deviceProcessor.Device.Name, deviceProcessor.LoginFailureReason));

				var doorType = deviceProcessor.Wrapper.GetControllerDoorType();
				return new OperationResult<DoorType>(doorType);
			}
			return OperationResult<DoorType>.FromError(CommonResources.ControllerNotFoundInConfig);
		}

		public static OperationResult<bool> SetControllerDoorType(Guid deviceUID, DoorType doorType)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<bool>.FromError(string.Format(CommonResources.NoLinkWithController, deviceProcessor.Device.Name, deviceProcessor.LoginFailureReason));

				var result = deviceProcessor.Wrapper.SetControllerDoorType(doorType);
				//deviceProcessor.Reconnect();
				if (result)
					return new OperationResult<bool>(true);
				else
					return OperationResult<bool>.FromError(CommonErrors.ExecuteOperationDevice_Error);
			}
			return OperationResult<bool>.FromError(CommonResources.ControllerNotFoundInConfig);
		}

		public static OperationResult<bool> SetControllerPassword(Guid deviceUID, string name, string oldPassword, string password)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<bool>.FromError(string.Format(CommonResources.NoLinkWithController, deviceProcessor.Device.Name, deviceProcessor.LoginFailureReason));

				var result = deviceProcessor.Wrapper.SetControllerPassword(name, oldPassword, password);
				if (result)
					return new OperationResult<bool>(true);
				else
					return OperationResult<bool>.FromError(CommonErrors.ExecuteOperationDevice_Error);
			}
			return OperationResult<bool>.FromError(CommonResources.ControllerNotFoundInConfig);
		}

		public static OperationResult<SKDControllerTimeSettings> GetControllerTimeSettings(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<SKDControllerTimeSettings>.FromError(string.Format(CommonResources.NoLinkWithController, deviceProcessor.Device.Name, deviceProcessor.LoginFailureReason));

				var doorType = deviceProcessor.Wrapper.GetControllerTimeSettings();
				return new OperationResult<SKDControllerTimeSettings>(doorType);
			}
			return OperationResult<SKDControllerTimeSettings>.FromError(CommonResources.ControllerNotFoundInConfig);
		}

		public static OperationResult<bool> SetControllerTimeSettings(Guid deviceUID, SKDControllerTimeSettings controllerTimeSettings)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<bool>.FromError(string.Format(CommonResources.NoLinkWithController, deviceProcessor.Device.Name, deviceProcessor.LoginFailureReason));

				var result = deviceProcessor.Wrapper.SetControllerTimeSettings(controllerTimeSettings);
				if (result)
				{
					SynchronizeTime(deviceUID);
					deviceProcessor.TimeSettings = controllerTimeSettings;
					return new OperationResult<bool>(true);
				}
				else
					return OperationResult<bool>.FromError(CommonErrors.ExecuteOperationDevice_Error);
			}
			return OperationResult<bool>.FromError(CommonResources.ControllerNotFoundInConfig);
		}

		public static OperationResult<SKDControllerNetworkSettings> GetControllerNetworkSettings(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<SKDControllerNetworkSettings>.FromError(string.Format(CommonResources.NoLinkWithController, deviceProcessor.Device.Name, deviceProcessor.LoginFailureReason));

				var controllerNetworkSettings = deviceProcessor.Wrapper.GetDeviceNetInfo();
				return new OperationResult<SKDControllerNetworkSettings>(controllerNetworkSettings);
			}
			return OperationResult<SKDControllerNetworkSettings>.FromError(CommonResources.ControllerNotFoundInConfig);
		}

		public static OperationResult<bool> SetControllerNetworkSettings(Guid deviceUID, SKDControllerNetworkSettings controllerNetworkSettings)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<bool>.FromError(string.Format(CommonResources.NoLinkWithController, deviceProcessor.Device.Name, deviceProcessor.LoginFailureReason));

				var result = deviceProcessor.Wrapper.SetDeviceNetInfo(controllerNetworkSettings);
				if (result)
					return new OperationResult<bool>(true);
				else
					return OperationResult<bool>.FromError(CommonErrors.ExecuteOperationDevice_Error);
			}
			return OperationResult<bool>.FromError(CommonResources.ControllerNotFoundInConfig);
		}

		public static OperationResult<SKDAntiPassBackConfiguration> GetAntiPassBackConfiguration(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<SKDAntiPassBackConfiguration>.FromError(string.Format(CommonResources.NoLinkWithController, deviceProcessor.Device.Name, deviceProcessor.LoginFailureReason));

				var antiPassBackConfiguration = deviceProcessor.Wrapper.GetAntiPassBackConfiguration();
				return new OperationResult<SKDAntiPassBackConfiguration>(new SKDAntiPassBackConfiguration() { IsActivated = antiPassBackConfiguration.IsActivated, CurrentAntiPassBackMode = (SKDAntiPassBackMode)antiPassBackConfiguration.CurrentAntiPassBackMode });
			}
			return OperationResult<SKDAntiPassBackConfiguration>.FromError(CommonResources.ControllerNotFoundInConfig);
		}

		public static OperationResult<bool> SetAntiPassBackConfiguration(Guid deviceUID, SKDAntiPassBackConfiguration antiPassBackConfiguration)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<bool>.FromError(string.Format(CommonResources.NoLinkWithController, deviceProcessor.Device.Name, deviceProcessor.LoginFailureReason));

				var result = deviceProcessor.Wrapper.SetAntiPassBackConfiguration(new AntiPassBackConfiguration() { IsActivated = antiPassBackConfiguration.IsActivated, CurrentAntiPassBackMode = (AntiPassBackMode)antiPassBackConfiguration.CurrentAntiPassBackMode });
				if (result)
					return new OperationResult<bool>(true);
				else
					return OperationResult<bool>.FromError(CommonErrors.ExecuteOperationDevice_Error);
			}
			return OperationResult<bool>.FromError(CommonResources.ControllerNotFoundInConfig);
		}

		public static OperationResult<SKDInterlockConfiguration> GetInterlockConfiguration(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<SKDInterlockConfiguration>.FromError(string.Format(CommonResources.NoLinkWithController, deviceProcessor.Device.Name, deviceProcessor.LoginFailureReason));

				var interlockConfiguration = deviceProcessor.Wrapper.GetInterlockConfiguration();
				return new OperationResult<SKDInterlockConfiguration>(new SKDInterlockConfiguration() { IsActivated = interlockConfiguration.IsActivated, CurrentInterlockMode = (SKDInterlockMode)interlockConfiguration.CurrentInterlockMode });
			}
			return OperationResult<SKDInterlockConfiguration>.FromError(CommonResources.ControllerNotFoundInConfig);
		}

		public static OperationResult<bool> SetInterlockConfiguration(Guid deviceUID, SKDInterlockConfiguration interlockConfiguration)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<bool>.FromError(string.Format(CommonResources.NoLinkWithController, deviceProcessor.Device.Name, deviceProcessor.LoginFailureReason));

				var result = deviceProcessor.Wrapper.SetInterlockConfiguration(new InterlockConfiguration() { IsActivated = interlockConfiguration.IsActivated, CurrentInterlockMode = (InterlockMode)interlockConfiguration.CurrentInterlockMode });
				if (result)
					return new OperationResult<bool>(true);
				return OperationResult<bool>.FromError(CommonErrors.ExecuteOperationDevice_Error);
			}
			return OperationResult<bool>.FromError(CommonResources.ControllerNotFoundInConfig);
		}

		public static OperationResult<bool> StartSearchDevices()
		{
			if (Wrapper.StartSearchDevices())
				return new OperationResult<bool>(true);
			else
				return OperationResult<bool>.FromError(CommonErrors.ExecuteOperation_Error);
		}

		public static OperationResult<bool> StopSearchDevices()
		{
			if (Wrapper.StopSearchDevices())
				return new OperationResult<bool>(true);
			else
				return OperationResult<bool>.FromError(CommonErrors.ExecuteOperation_Error);
		}

		public static TimeSpan TimeZoneTypeToTimeSpan(SKDTimeZoneType timeZone)
		{
			switch (timeZone)
			{
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_0:
				default:
					return new TimeSpan();
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_1:
					return TimeSpan.FromHours(1);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_2:
					return TimeSpan.FromHours(2);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_3:
					return TimeSpan.FromHours(3);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_4:
					return TimeSpan.FromHours(3.5);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_5:
					return TimeSpan.FromHours(4);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_6:
					return TimeSpan.FromHours(4.5);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_7:
					return TimeSpan.FromHours(5);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_8:
					return TimeSpan.FromHours(5.5);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_9:
					return TimeSpan.FromHours(5.75);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_10:
					return TimeSpan.FromHours(6);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_11:
					return TimeSpan.FromHours(6.5);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_12:
					return TimeSpan.FromHours(7);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_13:
					return TimeSpan.FromHours(8);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_14:
					return TimeSpan.FromHours(9);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_15:
					return TimeSpan.FromHours(9.5);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_16:
					return TimeSpan.FromHours(10);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_17:
					return TimeSpan.FromHours(11);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_18:
					return TimeSpan.FromHours(12);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_19:
					return TimeSpan.FromHours(13);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_20:
					return TimeSpan.FromHours(-1);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_21:
					return TimeSpan.FromHours(-2);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_22:
					return TimeSpan.FromHours(-3);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_23:
					return TimeSpan.FromHours(-3.5);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_24:
					return TimeSpan.FromHours(-4);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_25:
					return TimeSpan.FromHours(-5);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_26:
					return TimeSpan.FromHours(-6);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_27:
					return TimeSpan.FromHours(-7);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_28:
					return TimeSpan.FromHours(-8);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_29:
					return TimeSpan.FromHours(-9);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_30:
					return TimeSpan.FromHours(-10);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_31:
					return TimeSpan.FromHours(-11);
				case SKDTimeZoneType.EM_CFG_TIME_ZONE_32:
					return TimeSpan.FromHours(-12);
			}
		}

		#region <Пароли замков>

		public static OperationResult<IEnumerable<SKDLocksPassword>> GetControllerLocksPasswords(Guid deviceUid)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUid);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<IEnumerable<SKDLocksPassword>>.FromError(String.Format(CommonResources.NoLinkWithController, deviceProcessor.Device.Name, deviceProcessor.LoginFailureReason));

				IEnumerable<Password> passwords = deviceProcessor.Wrapper.GetAllPasswords();
				var locksPasswords = passwords.Select(password => new SKDLocksPassword()
				{
					Password = password.DoorOpenPassword,
					IsAppliedToLock1 = IsPasswordAppliedToLock(0, password.Doors, password.DoorsCount),
					IsAppliedToLock2 = IsPasswordAppliedToLock(1, password.Doors, password.DoorsCount),
					IsAppliedToLock3 = IsPasswordAppliedToLock(2, password.Doors, password.DoorsCount),
					IsAppliedToLock4 = IsPasswordAppliedToLock(3, password.Doors, password.DoorsCount)
				}).ToList();
				return new OperationResult<IEnumerable<SKDLocksPassword>>(locksPasswords);
			}
			return OperationResult<IEnumerable<SKDLocksPassword>>.FromError(CommonResources.ControllerNotFoundInConfig);
		}

		private static bool IsPasswordAppliedToLock(int lockIndex, IList<int> locks, int locksCountToCheck)
		{
			for (var i = 0; i < locksCountToCheck; i++)
				if (locks[i] == lockIndex)
					return true;
			return false;
		}

		public static OperationResult<bool> SetControllerLocksPasswords(Guid deviceUid, IEnumerable<SKDLocksPassword> locksPasswords, bool doProgress = true)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUid);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<bool>.FromError(String.Format(CommonResources.NoLinkWithController, deviceProcessor.Device.Name, deviceProcessor.LoginFailureReason));

				SKDProgressCallback progressCallback = null;

				// Показываем индикатор хода выполнения операции
				if (doProgress)
					progressCallback = StartProgress(String.Format(CommonResources.WriteLockPasswordsOnController, deviceProcessor.Device.Name), null, locksPasswords.Count() + 1, true, SKDProgressClientType.Administrator);

				// Сначала удаляем с контроллера все пароли замков, записанные ранее
				if (!deviceProcessor.Wrapper.RemoveAllPasswords())
					return OperationResult<bool>.FromError(String.Format(CommonErrors.ExecuteOperationDeletePasswordController_Error, deviceProcessor.Device.Name));

				// Обновляем индикатор хода выполнения операции
				if (progressCallback != null)
					DoProgress(null, progressCallback);

				// Потом записываем на контроллер новые пароли замков
				// Определяем количество активных замков/считывателей
				var activeLocksCount = 0;
				switch (deviceProcessor.Device.DriverType)
				{
					case SKDDriverType.ChinaController_2:
						activeLocksCount = 2;
						break;
					case SKDDriverType.ChinaController_4:
						activeLocksCount = 4;
						break;
				}
				// Контроллер сам учитывает неактивные замки в случае Двухсторонней точки доступа, поэтому можно закоментировать
				//switch (deviceProcessor.Device.DoorType)
				//{
				//	case DoorType.OneWay:
				//		break;
				//	case DoorType.TwoWay:
				//		activeLocksCount = (int) activeLocksCount / 2;
				//		break;
				//}

				foreach (var locksPassword in locksPasswords)
				{
					var doors = new List<int>();
					if (locksPassword.IsAppliedToLock1)
						doors.Add(0);
					if (locksPassword.IsAppliedToLock2)
						doors.Add(1);
					if (locksPassword.IsAppliedToLock3)
						doors.Add(2);
					if (locksPassword.IsAppliedToLock4)
						doors.Add(3);
					// Определяем количество замков, на которые нужно (согласно UI) и можно (согласно конфигурации контроллера) применить пароль
					var effectivlyAppliedLocksCount = Math.Min(locksPassword.AppliedLocksCount, activeLocksCount);

					// Если окажется, что количество элементов в списке (равно locksPassword.AppliedLocksCount) больше чем нужно (effectivlyAppliedLocksCount),
					// то удаляем лишние элементы
					while (doors.Count > effectivlyAppliedLocksCount)
						doors.RemoveAt(doors.Count - 1);

					var password = new Password()
					{
						UserID = locksPassword.Password,
						DoorOpenPassword = locksPassword.Password,
						DoorsCount = effectivlyAppliedLocksCount,
						Doors = doors
					};
					deviceProcessor.Wrapper.AddPassword(password);

					// Обновляем индикатор хода выполнения операции
					if (progressCallback != null)
						DoProgress(null, progressCallback);

					// Выполнение операции прервано пользователем
					if (progressCallback != null && progressCallback.IsCanceled)
						return OperationResult<bool>.FromCancel(String.Format(CommonResources.WriteLockPasswordsOnControllerCanceled, deviceProcessor.Device.Name));
				}

				// Останавливаем индикатор хода выполнения операции
				if (progressCallback != null)
					StopProgress(progressCallback);

				return new OperationResult<bool>(true);
			}
			return OperationResult<bool>.FromError(CommonResources.ControllerNotFoundInConfig);
		}

		#endregion </Пароли замков>
	}
}