using ChinaSKDDriverAPI;
using Common;
using FiresecAPI;
using FiresecAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChinaSKDDriver
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
					return OperationResult<SKDDeviceInfo>.FromError(String.Format("Нет связи с контроллером. {0}", deviceProcessor.LoginFailureReason));

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
					return OperationResult<SKDDeviceInfo>.FromError("Ошибка при запросе информации о прошивке на контроллере");
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
					return OperationResult<SKDDeviceInfo>.FromError("Ошибка при запросе сетевой информации на контроллере");
				}

				var dateTime = deviceProcessor.Wrapper.GetDateTime();
				if (dateTime > DateTime.MinValue)
				{
					deviceInfo.CurrentDateTime = deviceProcessor.Wrapper.GetDateTime();
				}
				else
				{
					return OperationResult<SKDDeviceInfo>.FromError("Ошибка при запросе текущего времени на контроллере");
				}

				return new OperationResult<SKDDeviceInfo>(deviceInfo);
			}
			return OperationResult<SKDDeviceInfo>.FromError("Не найден контроллер в конфигурации");
		}

		/// <summary>
		/// Устанавливает на контроллере текущее системное время
		/// </summary>
		/// <param name="deviceUID">Идентификатор контроллера</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public static OperationResult<bool> SynchronizeTime(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<bool>.FromError(string.Format("Нет связи с контроллером. {0}", deviceProcessor.LoginFailureReason));

				if (deviceProcessor.Wrapper.SetDateTime(DateTime.Now))
					return new OperationResult<bool>(true);
				
				return OperationResult<bool>.FromError("Ошибка при выполнении операции синхронизации времени на контроллере");
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
					return OperationResult<bool>.FromError(String.Format("Нет связи с контроллером \"{0}\". {1}", deviceProcessor.Device.Name, deviceProcessor.LoginFailureReason));

				SKDProgressCallback progressCallback = null;
	
				// Показываем индикатор хода выполнения операции
				if (doProgress)
					progressCallback = StartProgress(String.Format("Запись графиков доступа на контроллер \"{0}\"", deviceProcessor.Device.Name), null, 128, true, SKDProgressClientType.Administrator);

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
						return OperationResult<bool>.FromCancel(String.Format("Операция записи графиков доступа на контроллер \"{0}\" отменена", deviceProcessor.Device.Name));
					
					
					// Обновляем индикатор хода выполнения операции
					if (progressCallback != null)
						DoProgress(null, progressCallback);

					var result = deviceProcessor.Wrapper.SetTimeShedules(i, timeShedules);
					if (!result)
					{
						// Останавливаем индикатор хода выполнения операции
						if (progressCallback != null)
							StopProgress(progressCallback);
						
						return OperationResult<bool>.FromError(String.Format("Ошибка при выполнении операции записи графиков доступа на контроллер \"{0}\"", deviceProcessor.Device.Name));
					}
				}

				// Останавливаем индикатор хода выполнения операции
				if (progressCallback != null)
					StopProgress(progressCallback);

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
				doorConfiguration.AccessState = (FiresecAPI.SKD.AccessState)nativeDoorConfiguration.AccessState;
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
			return OperationResult<SKDDoorConfiguration>.FromError("Не найден контроллер в конфигурации");
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
				return OperationResult<bool>.FromError("Не найден замок в конфигурации");
			var controllerDevice = lockDevice.Parent;
			if (controllerDevice == null)
				return OperationResult<bool>.FromError("Не найден контроллер в конфигурации");

			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == controllerDevice.UID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<bool>.FromError(String.Format("Нет связи с контроллером. {0}", deviceProcessor.LoginFailureReason));

				var nativeDoorConfiguration = new DoorConfiguration();
				nativeDoorConfiguration.AccessState = (ChinaSKDDriverAPI.AccessState)doorConfiguration.AccessState;
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
					: OperationResult<bool>.FromError("Ошибка при выполнении операции записи конфигурации замка на контроллер");
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

		public static OperationResult<SKDAntiPassBackConfiguration> GetAntiPassBackConfiguration(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<SKDAntiPassBackConfiguration>.FromError("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var antiPassBackConfiguration = deviceProcessor.Wrapper.GetAntiPassBackConfiguration();
				return new OperationResult<SKDAntiPassBackConfiguration>(new SKDAntiPassBackConfiguration() { IsActivated = antiPassBackConfiguration.IsActivated, CurrentAntiPassBackMode = (SKDAntiPassBackMode)antiPassBackConfiguration.CurrentAntiPassBackMode });
			}
			return OperationResult<SKDAntiPassBackConfiguration>.FromError("Не найден контроллер в конфигурации");
		}

		public static OperationResult<bool> SetAntiPassBackConfiguration(Guid deviceUID, SKDAntiPassBackConfiguration antiPassBackConfiguration)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<bool>.FromError("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var result = deviceProcessor.Wrapper.SetAntiPassBackConfiguration(new AntiPassBackConfiguration() { IsActivated = antiPassBackConfiguration.IsActivated, CurrentAntiPassBackMode = (AntiPassBackMode)antiPassBackConfiguration.CurrentAntiPassBackMode });
				if (result)
					return new OperationResult<bool>(true);
				else
					return OperationResult<bool>.FromError("Ошибка при выполнении операции в приборе");
			}
			return OperationResult<bool>.FromError("Не найден контроллер в конфигурации");
		}

		public static OperationResult<SKDInterlockConfiguration> GetInterlockConfiguration(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<SKDInterlockConfiguration>.FromError("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var interlockConfiguration = deviceProcessor.Wrapper.GetInterlockConfiguration();
				return new OperationResult<SKDInterlockConfiguration>(new SKDInterlockConfiguration() { IsActivated = interlockConfiguration.IsActivated, CurrentInterlockMode = (SKDInterlockMode)interlockConfiguration.CurrentInterlockMode });
			}
			return OperationResult<SKDInterlockConfiguration>.FromError("Не найден контроллер в конфигурации");
		}

		public static OperationResult<bool> SetInterlockConfiguration(Guid deviceUID, SKDInterlockConfiguration interlockConfiguration)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<bool>.FromError("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var result = deviceProcessor.Wrapper.SetInterlockConfiguration(new InterlockConfiguration() { IsActivated = interlockConfiguration.IsActivated, CurrentInterlockMode = (InterlockMode)interlockConfiguration.CurrentInterlockMode });
				if (result)
					return new OperationResult<bool>(true);
				return OperationResult<bool>.FromError("Ошибка при выполнении операции в приборе");
			}
			return OperationResult<bool>.FromError("Не найден контроллер в конфигурации");
		}
		
		public static OperationResult<bool> StartSearchDevices()
		{
			if (Wrapper.StartSearchDevices())
				return new OperationResult<bool>(true);
			else
				return OperationResult<bool>.FromError("Ошибка при выполнении операции");
		}

		public static OperationResult<bool> StopSearchDevices()
		{
			if (Wrapper.StopSearchDevices())
				return new OperationResult<bool>(true);
			else
				return OperationResult<bool>.FromError("Ошибка при выполнении операции");
		}

		#region <Пароли замков>

		public static OperationResult<IEnumerable<SKDLocksPassword>> GetControllerLocksPasswords(Guid deviceUid)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUid);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<IEnumerable<SKDLocksPassword>>.FromError(String.Format("Нет связи с контроллером. {0}", deviceProcessor.LoginFailureReason));

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
			return OperationResult<IEnumerable<SKDLocksPassword>>.FromError("Не найден контроллер в конфигурации");
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
					return OperationResult<bool>.FromError(String.Format("Нет связи с контроллером \"{0}\". {1}", deviceProcessor.Device.Name, deviceProcessor.LoginFailureReason));

				SKDProgressCallback progressCallback = null;

				// Показываем индикатор хода выполнения операции
				if (doProgress)
					progressCallback = StartProgress(String.Format("Запись паролей замков на контроллер \"{0}\"", deviceProcessor.Device.Name), null, locksPasswords.Count() + 1, true, SKDProgressClientType.Administrator);

				// Сначала удаляем с контроллера все пароли замков, записанные ранее
				if (!deviceProcessor.Wrapper.RemoveAllPasswords())
					return OperationResult<bool>.FromError(String.Format("Ошибка удаления паролей замков на контроллере \"{0}\"", deviceProcessor.Device.Name));

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
						return OperationResult<bool>.FromCancel(String.Format("Операция записи паролей замков на контроллер \"{0}\" отменена", deviceProcessor.Device.Name));
				}

				// Останавливаем индикатор хода выполнения операции
				if (progressCallback != null)
					StopProgress(progressCallback);

				return new OperationResult<bool>(true);
			}
			return OperationResult<bool>.FromError("Не найден контроллер в конфигурации");
		}
		
		#endregion </Пароли замков>
	}
}