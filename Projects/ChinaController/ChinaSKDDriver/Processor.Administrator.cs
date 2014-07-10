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
				if(!deviceProcessor.IsConnected)
					return new OperationResult<SKDDeviceInfo>("Нет связи с контроллером");

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
					deviceInfo.IP = deviceNetInfo.IP;
					deviceInfo.SubnetMask = deviceNetInfo.SubnetMask;
					deviceInfo.DefaultGateway = deviceNetInfo.DefaultGateway;
					deviceInfo.MTU = deviceNetInfo.MTU;
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
					return new OperationResult<bool>("Нет связи с контроллером");

				var result = deviceProcessor.Wrapper.SetDateTime(DateTime.Now);
				if (result)
					return new OperationResult<bool>() { Result = true };
				else
					return new OperationResult<bool>("Ошибка при выполнении операции в прибор");
			}
			return new OperationResult<bool>("Не найден контроллер в конфигурации");
		}

		public static OperationResult<string> GetPassword(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return new OperationResult<string>("Нет связи с контроллером");

				var result = deviceProcessor.Wrapper.GetProjectPassword();
				if (!string.IsNullOrEmpty(result))
				{
					if (result == "-1")
						result = "";
					return new OperationResult<string>() { Result = result };
				}
				else
					return new OperationResult<string>("Ошибка при выполнении операции в приборе");
			}
			return new OperationResult<string>("Не найден контроллер в конфигурации");
		}

		public static OperationResult<bool> SetPassword(Guid deviceUID, string password)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return new OperationResult<bool>("Нет связи с контроллером");

				var result = deviceProcessor.Wrapper.SetProjectPassword(password);
				if (result)
					return new OperationResult<bool>() { Result = true };
				else
					return new OperationResult<bool>("Ошибка при выполнении операции в приборе");
			}
			return new OperationResult<bool>("Не найден контроллер в конфигурации");
		}

		public static OperationResult<bool> ResetController(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return new OperationResult<bool>("Нет связи с контроллером");

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
					return new OperationResult<bool>("Нет связи с контроллером");

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
				//if (!deviceProcessor.IsConnected)
				//    return new OperationResult<bool>("Нет связи с контроллером");

				var progressCallback = Processor.StartProgress("ТЕСТ. Запись графиков работ в прибор " + deviceProcessor.Device.Name, "", 128, true, GKProgressClientType.Administrator);

				for (int i = 1; i <= 128; i++)
				{
					var weeklyInterval = SKDManager.SKDConfiguration.TimeIntervalsConfiguration.WeeklyIntervals.FirstOrDefault(x => x.ID == i);
					if (weeklyInterval == null)
						weeklyInterval = new SKDWeeklyInterval();

					var timeShedules = new List<TimeShedule>();
					foreach (var weeklyIntervalPart in weeklyInterval.WeeklyIntervalParts)
					{
						if (!weeklyIntervalPart.IsHolliday)
						{
							var timeShedule = new TimeShedule();
							var timeInterval = SKDManager.SKDConfiguration.TimeIntervalsConfiguration.TimeIntervals.FirstOrDefault(x => x.ID == weeklyIntervalPart.TimeIntervalID);
							if (timeInterval != null)
							{
								foreach (var timeIntervalPart in timeInterval.TimeIntervalParts)
								{
									var timeSheduleInterval = new TimeSheduleInterval();
									timeSheduleInterval.BeginHours = timeIntervalPart.StartTime.Hour;
									timeSheduleInterval.BeginMinutes = timeIntervalPart.StartTime.Minute;
									timeSheduleInterval.BeginSeconds = timeIntervalPart.StartTime.Second;
									timeSheduleInterval.EndHours = timeIntervalPart.EndTime.Hour;
									timeSheduleInterval.EndMinutes = timeIntervalPart.EndTime.Minute;
									timeSheduleInterval.EndSeconds = timeIntervalPart.EndTime.Second;
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
					}

					if (progressCallback.IsCanceled)
						return new OperationResult<bool>("ТЕСТ. Операция обновления прибора " + deviceProcessor.Device.Name + " отменена");
					Processor.DoProgress("ТЕСТ. Запись графика " + i, progressCallback);

					System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(100));
					var result = true;
					//var result = deviceProcessor.Wrapper.SetTimeShedules(i, timeShedules);
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
					return new OperationResult<SKDDoorConfiguration>("Нет связи с контроллером");

				var nativeDoorConfiguration = deviceProcessor.Wrapper.GetDoorConfiguration(readerDevice.IntAddress);
				if(nativeDoorConfiguration == null)
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
					return new OperationResult<bool>("Нет связи с контроллером");

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

				nativeDoorConfiguration.UseDoorOpenMethod = true;
				nativeDoorConfiguration.UseUnlockHoldInterval = true;
				nativeDoorConfiguration.UseCloseTimeout = true;
				nativeDoorConfiguration.UseOpenAlwaysTimeIndex = true;
				nativeDoorConfiguration.UseHolidayTimeIndex = true;
				nativeDoorConfiguration.UseBreakInAlarmEnable = true;
				nativeDoorConfiguration.UseRepeatEnterAlarmEnable = true;
				nativeDoorConfiguration.UseDoorNotClosedAlarmEnable = true;
				nativeDoorConfiguration.UseDuressAlarmEnable = true;
				nativeDoorConfiguration.UseDoorTimeSection = false;
				nativeDoorConfiguration.UseSensorEnable = true;

				var result = deviceProcessor.Wrapper.SetDoorConfiguration(nativeDoorConfiguration, readerDevice.IntAddress);
				if (result)
					return new OperationResult<bool>() { Result = true };
				else
					return new OperationResult<bool>("Ошибка при выполнении операции в приборе");
			}
			return new OperationResult<bool>("Не найден контроллер в конфигурации");
		}
	}
}