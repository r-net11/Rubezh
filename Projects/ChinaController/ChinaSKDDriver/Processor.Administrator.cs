using System;
using System.Collections.Generic;
using System.Linq;
using ChinaSKDDriverAPI;
using FiresecAPI;
using FiresecAPI.SKD;

namespace ChinaSKDDriver
{
	public static partial class Processor
	{
		public static OperationResult<SKDDeviceInfo> GetDeviceInfo(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<SKDDeviceInfo>.FromError("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				SKDDeviceInfo deviceInfo = new SKDDeviceInfo();
				var deviceSoftwareInfo = deviceProcessor.Wrapper.GetDeviceSoftwareInfo();
				if (deviceSoftwareInfo != null)
				{
					deviceInfo.DeviceType = deviceSoftwareInfo.DeviceType;
					deviceInfo.SoftwareBuildDate = deviceSoftwareInfo.SoftwareBuildDate;
					deviceInfo.SoftwareVersion = deviceSoftwareInfo.SoftwareVersion;
				}
				else
				{
					return OperationResult<SKDDeviceInfo>.FromError("Ошибка при запросе информации о версии из контроллера");
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
					return OperationResult<SKDDeviceInfo>.FromError("Ошибка при запросе сетевой информации из контроллера");
				}

				var dateTime = deviceProcessor.Wrapper.GetDateTime();
				if (dateTime > DateTime.MinValue)
				{
					deviceInfo.CurrentDateTime = deviceProcessor.Wrapper.GetDateTime();
				}
				else
				{
					return OperationResult<SKDDeviceInfo>.FromError("Ошибка при запросе текущего времени из контроллера");
				}

				return new OperationResult<SKDDeviceInfo>(deviceInfo);
			}
			return OperationResult<SKDDeviceInfo>.FromError("Не найден контроллер в конфигурации");
		}

		public static OperationResult<bool> SyncronyseTime(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<bool>.FromError("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var result = deviceProcessor.Wrapper.SetDateTime(DateTime.Now);
				if (result)
					return new OperationResult<bool>(true);
				else
					return OperationResult<bool>.FromError("Ошибка при выполнении операции в прибор");
			}
			return OperationResult<bool>.FromError("Не найден контроллер в конфигурации");
		}

		public static OperationResult<bool> ResetController(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<bool>.FromError("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var result = deviceProcessor.Wrapper.Reset();
				if (result)
					return new OperationResult<bool>(true);
				else
					return OperationResult<bool>.FromError("Ошибка при выполнении операции в приборе");
			}
			return OperationResult<bool>.FromError("Не найден контроллер в конфигурации");
		}

		public static OperationResult<bool> RebootController(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<bool>.FromError("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var result = deviceProcessor.Wrapper.Reboot();
				deviceProcessor.Reconnect();
				if (result)
					return new OperationResult<bool>(true);
				else
					return OperationResult<bool>.FromError("Ошибка при выполнении операции в приборе");
			}
			return OperationResult<bool>.FromError("Не найден контроллер в конфигурации");
		}

		public static OperationResult<bool> SKDWriteTimeSheduleConfiguration(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<bool>.FromError("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var progressCallback = Processor.StartProgress("Запись графиков работ в прибор " + deviceProcessor.Device.Name, "", 128, true, GKProgressClientType.Administrator);

				for (int i = 0; i <= 127; i++)
				{
					var weeklyInterval = SKDManager.SKDConfiguration.TimeIntervalsConfiguration.WeeklyIntervals.FirstOrDefault(x => x.ID == i);
					if (weeklyInterval == null)
						weeklyInterval = new SKDWeeklyInterval();

					var timeShedules = new List<TimeShedule>();
					foreach (var weeklyIntervalPart in weeklyInterval.WeeklyIntervalParts.OrderBy(x=>x.DayOfWeek))
					{
						var timeShedule = new TimeShedule();
						var dayInterval = SKDManager.SKDConfiguration.TimeIntervalsConfiguration.DayIntervals.FirstOrDefault(x => x.UID == weeklyIntervalPart.DayIntervalUID);

						if (i == 0)
						{
							dayInterval = new SKDDayInterval();
							dayInterval.DayIntervalParts = new List<SKDDayIntervalPart>();
							var emptySKDTimeIntervalPart = new SKDDayIntervalPart();
							emptySKDTimeIntervalPart.StartMilliseconds = 0;
							emptySKDTimeIntervalPart.EndMilliseconds = 0;
							dayInterval.DayIntervalParts.Add(emptySKDTimeIntervalPart);
						}
						if (i == 1)
						{
							dayInterval = new SKDDayInterval();
							dayInterval.DayIntervalParts = new List<SKDDayIntervalPart>();
							var emptySKDTimeIntervalPart = new SKDDayIntervalPart();
							emptySKDTimeIntervalPart.StartMilliseconds = 0;
							emptySKDTimeIntervalPart.EndMilliseconds = new TimeSpan(23, 59, 59).TotalMilliseconds;
							dayInterval.DayIntervalParts.Add(emptySKDTimeIntervalPart);
						}

						if (dayInterval != null)
						{
							foreach (var dayIntervalPart in dayInterval.DayIntervalParts)
							{
								var timeSheduleInterval = new TimeSheduleInterval();
								timeSheduleInterval.BeginHours = TimeSpan.FromMilliseconds(dayIntervalPart.StartMilliseconds).Hours;
								timeSheduleInterval.BeginMinutes = TimeSpan.FromMilliseconds(dayIntervalPart.StartMilliseconds).Minutes;
								timeSheduleInterval.BeginSeconds = TimeSpan.FromMilliseconds(dayIntervalPart.StartMilliseconds).Seconds;
								timeSheduleInterval.EndHours = TimeSpan.FromMilliseconds(dayIntervalPart.EndMilliseconds).Hours;
								timeSheduleInterval.EndMinutes = TimeSpan.FromMilliseconds(dayIntervalPart.EndMilliseconds).Minutes;
								timeSheduleInterval.EndSeconds = TimeSpan.FromMilliseconds(dayIntervalPart.EndMilliseconds).Seconds;

								timeShedule.TimeSheduleIntervals.Add(timeSheduleInterval);
							}
							for (int j = timeShedule.TimeSheduleIntervals.Count; j < 4; j++)
							{
								var timeSheduleInterval = new TimeSheduleInterval();
								timeShedule.TimeSheduleIntervals.Add(timeSheduleInterval);
							}
						}
						timeShedules.Add(timeShedule);
					}

					if (progressCallback.IsCanceled)
						return OperationResult<bool>.FromError("Операция записи графиков прибора " + deviceProcessor.Device.Name + " отменена");
					Processor.DoProgress("Запись графика " + i, progressCallback);

					var result = deviceProcessor.Wrapper.SetTimeShedules(i, timeShedules);
					if (!result)
					{
						Processor.StopProgress(progressCallback);
						return OperationResult<bool>.FromError("Ошибка при выполнении операции в приборе");
					}
				}

				Processor.StopProgress(progressCallback);
				return new OperationResult<bool>(true);
			}
			return OperationResult<bool>.FromError("Не найден контроллер в конфигурации");
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
					return OperationResult<SKDDoorConfiguration>.FromError("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var nativeDoorConfiguration = deviceProcessor.Wrapper.GetDoorConfiguration(readerDevice.IntAddress);
				if (nativeDoorConfiguration == null)
					return OperationResult<SKDDoorConfiguration>.FromError("Ошибка при выполнении операции в приборе");

				var doorConfiguration = new SKDDoorConfiguration();
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

				doorConfiguration.OpenAlwaysTimeIndex = nativeDoorConfiguration.OpenAlwaysTimeIndex;
				doorConfiguration.HolidayTimeRecoNo = nativeDoorConfiguration.HolidayTimeRecoNo;
				doorConfiguration.IsRepeatEnterAlarmEnable = nativeDoorConfiguration.IsRepeatEnterAlarmEnable;

				return new OperationResult<SKDDoorConfiguration>(doorConfiguration);
			}
			return OperationResult<SKDDoorConfiguration>.FromError("Не найден контроллер в конфигурации");
		}

		public static OperationResult<bool> SetDoorConfiguration(Guid deviceUID, SKDDoorConfiguration doorConfiguration)
		{
			var readerDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (readerDevice == null)
				return OperationResult<bool>.FromError("Не найден считыватель в конфигурации");
			var controllerDevice = readerDevice.Parent;
			if (controllerDevice == null)
				return OperationResult<bool>.FromError("Не найден контроллер в конфигурации");

			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == controllerDevice.UID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<bool>.FromError("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var nativeDoorConfiguration = new DoorConfiguration();
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

				nativeDoorConfiguration.OpenAlwaysTimeIndex = doorConfiguration.OpenAlwaysTimeIndex;
				nativeDoorConfiguration.HolidayTimeRecoNo = doorConfiguration.HolidayTimeRecoNo;
				nativeDoorConfiguration.IsRepeatEnterAlarmEnable = doorConfiguration.IsRepeatEnterAlarmEnable;

				var result = deviceProcessor.Wrapper.SetDoorConfiguration(nativeDoorConfiguration, readerDevice.IntAddress);
				if (result)
					return new OperationResult<bool>(true);
				else
					return OperationResult<bool>.FromError("Ошибка при выполнении операции в приборе");
			}
			return OperationResult<bool>.FromError("Не найден контроллер в конфигурации");
		}

		public static OperationResult<DoorType> GetControllerDoorType(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<DoorType>.FromError("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var doorType = deviceProcessor.Wrapper.GetControllerDoorType();
				return new OperationResult<DoorType>(doorType);
			}
			return OperationResult<DoorType>.FromError("Не найден контроллер в конфигурации");
		}

		public static OperationResult<bool> SetControllerDoorType(Guid deviceUID, DoorType doorType)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<bool>.FromError("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var result = deviceProcessor.Wrapper.SetControllerDoorType(doorType);
				//deviceProcessor.Reconnect();
				if (result)
					return new OperationResult<bool>(true);
				else
					return OperationResult<bool>.FromError("Ошибка при выполнении операции в приборе");
			}
			return OperationResult<bool>.FromError("Не найден контроллер в конфигурации");
		}

		public static OperationResult<bool> SetControllerPassword(Guid deviceUID, string name, string oldPassword, string password)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<bool>.FromError("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var result = deviceProcessor.Wrapper.SetControllerPassword(name, oldPassword, password);
				if (result)
					return new OperationResult<bool>(true);
				else
					return OperationResult<bool>.FromError("Ошибка при выполнении операции в приборе");
			}
			return OperationResult<bool>.FromError("Не найден контроллер в конфигурации");
		}

		public static OperationResult<SKDControllerTimeSettings> GetControllerTimeSettings(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<SKDControllerTimeSettings>.FromError("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var doorType = deviceProcessor.Wrapper.GetControllerTimeSettings();
				return new OperationResult<SKDControllerTimeSettings>(doorType);
			}
			return OperationResult<SKDControllerTimeSettings>.FromError("Не найден контроллер в конфигурации");
		}

		public static OperationResult<bool> SetControllerTimeSettings(Guid deviceUID, SKDControllerTimeSettings controllerTimeSettings)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<bool>.FromError("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var result = deviceProcessor.Wrapper.SetControllerTimeSettings(controllerTimeSettings);
				if (result)
					return new OperationResult<bool>(true);
				else
					return OperationResult<bool>.FromError("Ошибка при выполнении операции в приборе");
			}
			return OperationResult<bool>.FromError("Не найден контроллер в конфигурации");
		}

		public static OperationResult<SKDControllerNetworkSettings> GetControllerNetworkSettings(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<SKDControllerNetworkSettings>.FromError("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var controllerNetworkSettings = deviceProcessor.Wrapper.GetDeviceNetInfo();
				return new OperationResult<SKDControllerNetworkSettings>(controllerNetworkSettings);
			}
			return OperationResult<SKDControllerNetworkSettings>.FromError("Не найден контроллер в конфигурации");
		}

		public static OperationResult<bool> SetControllerNetworkSettings(Guid deviceUID, SKDControllerNetworkSettings controllerNetworkSettings)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<bool>.FromError("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var result = deviceProcessor.Wrapper.SetDeviceNetInfo(controllerNetworkSettings);
				if (result)
					return new OperationResult<bool>(true);
				else
					return OperationResult<bool>.FromError("Ошибка при выполнении операции в приборе");
			}
			return OperationResult<bool>.FromError("Не найден контроллер в конфигурации");
		}
	}
}