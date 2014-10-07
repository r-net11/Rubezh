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
		public static OperationResult<SKDDeviceInfo> GetdeviceInfo(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return new OperationResult<SKDDeviceInfo>("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

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
					return new OperationResult<SKDDeviceInfo>("Ошибка при запросе инфиормации о версии из контроллера");
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
					return new OperationResult<SKDDeviceInfo>("Ошибка при запросе сетевой информации из контроллера");
				}

				var dateTime = deviceProcessor.Wrapper.GetDateTime();
				if (dateTime > DateTime.MinValue)
				{
					deviceInfo.CurrentDateTime = deviceProcessor.Wrapper.GetDateTime();
				}
				else
				{
					return new OperationResult<SKDDeviceInfo>("Ошибка при запросе текущего времени из контроллера");
				}

				return new OperationResult<SKDDeviceInfo>() { Result = deviceInfo };
			}
			return new OperationResult<SKDDeviceInfo>("Не найден контроллер в конфигурации");
		}

		public static OperationResult<bool> SyncronyseTime(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return new OperationResult<bool>("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var result = deviceProcessor.Wrapper.SetDateTime(DateTime.Now);
				if (result)
					return new OperationResult<bool>() { Result = true };
				else
					return new OperationResult<bool>("Ошибка при выполнении операции в прибор");
			}
			return new OperationResult<bool>("Не найден контроллер в конфигурации");
		}

		public static OperationResult<bool> ResetController(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return new OperationResult<bool>("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var result = deviceProcessor.Wrapper.Reset();
				if (result)
					return new OperationResult<bool>() { Result = true };
				else
					return new OperationResult<bool>("Ошибка при выполнении операции в приборе");
			}
			return new OperationResult<bool>("Не найден контроллер в конфигурации");
		}

		public static OperationResult<bool> RebootController(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return new OperationResult<bool>("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var result = deviceProcessor.Wrapper.Reboot();
				deviceProcessor.Reconnect();
				if (result)
					return new OperationResult<bool>() { Result = true };
				else
					return new OperationResult<bool>("Ошибка при выполнении операции в приборе");
			}
			return new OperationResult<bool>("Не найден контроллер в конфигурации");
		}

		public static OperationResult<bool> SKDWriteTimeSheduleConfiguration(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return new OperationResult<bool>("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

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
						return new OperationResult<bool>("Операция обновления прибора " + deviceProcessor.Device.Name + " отменена");
					Processor.DoProgress("Запись графика " + i, progressCallback);

					var result = deviceProcessor.Wrapper.SetTimeShedules(i, timeShedules);
					if (!result)
					{
						Processor.StopProgress(progressCallback);
						return new OperationResult<bool>("Ошибка при выполнении операции в приборе");
					}
				}

				Processor.StopProgress(progressCallback);
				return new OperationResult<bool>() { Result = true };
			}
			return new OperationResult<bool>("Не найден контроллер в конфигурации");
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
					return new OperationResult<SKDDoorConfiguration>("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var nativeDoorConfiguration = deviceProcessor.Wrapper.GetDoorConfiguration(readerDevice.IntAddress);
				if (nativeDoorConfiguration == null)
					return new OperationResult<SKDDoorConfiguration>("Ошибка при выполнении операции в приборе");

				var doorConfiguration = new SKDDoorConfiguration();
				doorConfiguration.AccessState = (SKDDoorConfiguration_AccessState)nativeDoorConfiguration.AccessState;
				doorConfiguration.AccessMode = (SKDDoorConfiguration_AccessMode)nativeDoorConfiguration.AccessMode;
				doorConfiguration.DoorOpenMethod = (SKDDoorConfiguration_DoorOpenMethod)nativeDoorConfiguration.DoorOpenMethod;

				doorConfiguration.UnlockHoldInterval = nativeDoorConfiguration.UnlockHoldInterval;
				doorConfiguration.CloseTimeout = nativeDoorConfiguration.CloseTimeout;
				doorConfiguration.OpenAlwaysTimeIndex = nativeDoorConfiguration.OpenAlwaysTimeIndex;
				doorConfiguration.HolidayTimeRecoNo = nativeDoorConfiguration.HolidayTimeRecoNo;
				doorConfiguration.IsBreakInAlarmEnable = nativeDoorConfiguration.IsBreakInAlarmEnable;
				doorConfiguration.IsRepeatEnterAlarmEnable = nativeDoorConfiguration.IsRepeatEnterAlarmEnable;
				doorConfiguration.IsDoorNotClosedAlarmEnable = nativeDoorConfiguration.IsDoorNotClosedAlarmEnable;
				doorConfiguration.IsDuressAlarmEnable = nativeDoorConfiguration.IsDuressAlarmEnable;
				doorConfiguration.IsSensorEnable = nativeDoorConfiguration.IsSensorEnable;
				doorConfiguration.DoorDayIntervalsCollection = nativeDoorConfiguration.DoorDayIntervalsCollection;

				return new OperationResult<SKDDoorConfiguration>() { Result = doorConfiguration };
			}
			return new OperationResult<SKDDoorConfiguration>("Не найден контроллер в конфигурации");
		}

		public static OperationResult<bool> SetDoorConfiguration(Guid deviceUID, SKDDoorConfiguration doorConfiguration)
		{
			var readerDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (readerDevice == null)
				return new OperationResult<bool>("Не найден считыватель в конфигурации");
			var controllerDevice = readerDevice.Parent;
			if (controllerDevice == null)
				return new OperationResult<bool>("Не найден контроллер в конфигурации");

			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == controllerDevice.UID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return new OperationResult<bool>("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var nativeDoorConfiguration = new DoorConfiguration();
				nativeDoorConfiguration.AccessState = (AccessState)doorConfiguration.AccessState;
				nativeDoorConfiguration.AccessMode = (AccessMode)doorConfiguration.AccessMode;
				nativeDoorConfiguration.DoorOpenMethod = (DoorOpenMethod)doorConfiguration.DoorOpenMethod;

				nativeDoorConfiguration.UnlockHoldInterval = doorConfiguration.UnlockHoldInterval;
				nativeDoorConfiguration.CloseTimeout = doorConfiguration.CloseTimeout;
				nativeDoorConfiguration.OpenAlwaysTimeIndex = doorConfiguration.OpenAlwaysTimeIndex;
				nativeDoorConfiguration.HolidayTimeRecoNo = doorConfiguration.HolidayTimeRecoNo;
				nativeDoorConfiguration.IsBreakInAlarmEnable = doorConfiguration.IsBreakInAlarmEnable;
				nativeDoorConfiguration.IsRepeatEnterAlarmEnable = doorConfiguration.IsRepeatEnterAlarmEnable;
				nativeDoorConfiguration.IsDoorNotClosedAlarmEnable = doorConfiguration.IsDoorNotClosedAlarmEnable;
				nativeDoorConfiguration.IsDuressAlarmEnable = doorConfiguration.IsDuressAlarmEnable;
				nativeDoorConfiguration.IsSensorEnable = doorConfiguration.IsSensorEnable;
				nativeDoorConfiguration.DoorDayIntervalsCollection = doorConfiguration.DoorDayIntervalsCollection;

				var result = deviceProcessor.Wrapper.SetDoorConfiguration(nativeDoorConfiguration, readerDevice.IntAddress);
				if (result)
					return new OperationResult<bool>() { Result = true };
				else
					return new OperationResult<bool>("Ошибка при выполнении операции в приборе");
			}
			return new OperationResult<bool>("Не найден контроллер в конфигурации");
		}

		public static OperationResult<DoorType> GetControllerDoorType(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return new OperationResult<DoorType>("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var doorType = deviceProcessor.Wrapper.GetControllerDoorType();
				return new OperationResult<DoorType>() { Result = doorType };
			}
			return new OperationResult<DoorType>("Не найден контроллер в конфигурации");
		}

		public static OperationResult<bool> SetControllerDoorType(Guid deviceUID, DoorType doorType)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return new OperationResult<bool>("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var result = deviceProcessor.Wrapper.SetControllerDoorType(doorType);
				//deviceProcessor.Reconnect();
				if (result)
					return new OperationResult<bool>() { Result = true };
				else
					return new OperationResult<bool>("Ошибка при выполнении операции в приборе");
			}
			return new OperationResult<bool>("Не найден контроллер в конфигурации");
		}

		public static OperationResult<bool> SetControllerPassword(Guid deviceUID, string name, string oldPassword, string password)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return new OperationResult<bool>("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var result = deviceProcessor.Wrapper.SetControllerPassword(name, oldPassword, password);
				if (result)
					return new OperationResult<bool>() { Result = true };
				else
					return new OperationResult<bool>("Ошибка при выполнении операции в приборе");
			}
			return new OperationResult<bool>("Не найден контроллер в конфигурации");
		}

		public static OperationResult<SKDControllerTimeSettings> GetControllerTimeSettings(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return new OperationResult<SKDControllerTimeSettings>("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var doorType = deviceProcessor.Wrapper.GetControllerTimeSettings();
				return new OperationResult<SKDControllerTimeSettings>() { Result = doorType };
			}
			return new OperationResult<SKDControllerTimeSettings>("Не найден контроллер в конфигурации");
		}

		public static OperationResult<bool> SetControllerTimeSettings(Guid deviceUID, SKDControllerTimeSettings controllerTimeSettings)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return new OperationResult<bool>("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var result = deviceProcessor.Wrapper.SetControllerTimeSettings(controllerTimeSettings);
				if (result)
					return new OperationResult<bool>() { Result = true };
				else
					return new OperationResult<bool>("Ошибка при выполнении операции в приборе");
			}
			return new OperationResult<bool>("Не найден контроллер в конфигурации");
		}

		public static OperationResult<SKDControllerNetworkSettings> GetControllerNetworkSettings(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return new OperationResult<SKDControllerNetworkSettings>("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var controllerNetworkSettings = deviceProcessor.Wrapper.GetDeviceNetInfo();
				return new OperationResult<SKDControllerNetworkSettings>() { Result = controllerNetworkSettings };
			}
			return new OperationResult<SKDControllerNetworkSettings>("Не найден контроллер в конфигурации");
		}

		public static OperationResult<bool> SetControllerNetworkSettings(Guid deviceUID, SKDControllerNetworkSettings controllerNetworkSettings)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return new OperationResult<bool>("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var result = deviceProcessor.Wrapper.SetDeviceNetInfo(controllerNetworkSettings);
				if (result)
					return new OperationResult<bool>() { Result = true };
				else
					return new OperationResult<bool>("Ошибка при выполнении операции в приборе");
			}
			return new OperationResult<bool>("Не найден контроллер в конфигурации");
		}
	}
}