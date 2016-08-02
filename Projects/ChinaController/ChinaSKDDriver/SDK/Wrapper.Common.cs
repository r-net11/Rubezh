using StrazhDeviceSDK.API;
using StrazhDeviceSDK.NativeAPI;
using StrazhAPI.SKD;
using System;
using System.Runtime.InteropServices;

namespace StrazhDeviceSDK
{
	public partial class Wrapper
	{
		#region Helpers

		public static DateTime NET_TIMEToDateTime(NativeWrapper.NET_TIME netTime)
		{
			var dateTime = DateTime.MinValue;
			try
			{
				if (netTime.dwYear <= 0 || netTime.dwMonth <= 0 || netTime.dwDay <= 0)
					return new DateTime();
				dateTime = new DateTime(netTime.dwYear, netTime.dwMonth, netTime.dwDay, netTime.dwHour, netTime.dwMinute, netTime.dwSecond);
			}
			catch { }
			return dateTime;
		}

		#endregion Helpers

		#region CommonDevice

		/// <summary>
		/// Получает версию прошивки на контроллере
		/// </summary>
		/// <returns>Объект DeviceSoftwareInfo с описанием типа устройства и версии прошивки</returns>
		public DeviceSoftwareInfo GetDeviceSoftwareInfo()
		{
			NativeWrapper.WRAP_DevConfig_TypeAndSoftInfo_Result outResult;
			var result = NativeWrapper.WRAP_GetSoftwareInfo(LoginID, out outResult);

			DeviceSoftwareInfo deviceSoftwareInfo = null;
			if (result)
			{
				deviceSoftwareInfo = new DeviceSoftwareInfo
				{
					DeviceType = outResult.szDevType,
					SoftwareVersion = outResult.szSoftWareVersion
				};

				// Переопределяем тип устройства полученный с контроллера, на принятый в A.C.Tech
				switch (deviceSoftwareInfo.DeviceType.ToUpper())
				{
					case "BSC1202B":
					case "ASC1202B":
						deviceSoftwareInfo.DeviceType = "SR-NC004";
						break;
					case "BSC1201B":
					case "ASC1201B":
						deviceSoftwareInfo.DeviceType = "SR-NC002";
						break;
					case "BSC1221A":
					case "ASI1201A-D":
						deviceSoftwareInfo.DeviceType = "SR-NC101";
						break;
				}

				try
				{
					if (outResult.dwSoftwareBuildDate_Year > 0 && outResult.dwSoftwareBuildDate_Month > 0 && outResult.dwSoftwareBuildDate_Day > 0)
						deviceSoftwareInfo.SoftwareBuildDate = new DateTime(outResult.dwSoftwareBuildDate_Year, outResult.dwSoftwareBuildDate_Month, outResult.dwSoftwareBuildDate_Day);
				}
				catch { }
			}
			return deviceSoftwareInfo;
		}

		/// <summary>
		/// Получает сетевые настройки контроллера
		/// </summary>
		/// <returns>Объект SKDControllerNetworkSettings, содержащий сетевые настройки контроллера</returns>
		public SKDControllerNetworkSettings GetDeviceNetInfo()
		{
			NativeWrapper.WRAP_CFG_NETWORK_INFO_Result outResult;
			var result = NativeWrapper.WRAP_Get_NetInfo(LoginID, out outResult);

			return result
				? new SKDControllerNetworkSettings
				{
					Address = outResult.szIP,
					Mask = outResult.szSubnetMask,
					DefaultGateway = outResult.szDefGateway
				}
				: null;
		}

		/// <summary>
		/// Устанавливает сетевые настройки на контроллере
		/// </summary>
		/// <param name="controllerNetworkSettings">Сетевые настройки</param>
		/// <returns>true - операция завершилась успешно, false - операция завершилась с ошибкой</returns>
		public bool SetDeviceNetInfo(SKDControllerNetworkSettings controllerNetworkSettings)
		{
			return NativeWrapper.WRAP_Set_NetInfo(LoginID, controllerNetworkSettings.Address, controllerNetworkSettings.Mask, controllerNetworkSettings.DefaultGateway, 1500);
		}

		/// <summary>
		/// Получаем MAC-адрес контроллера
		/// </summary>
		/// <returns>MAC-адрес контроллера</returns>
		public string GetDeviceMacAddress()
		{
			NativeWrapper.WRAP_DevConfig_MAC_Result outResult;
			var result = NativeWrapper.WRAP_GetMacAddress(LoginID, out outResult);
			return result ? outResult.szMAC : null;
		}

		public int GetMaxPageSize()
		{
			NativeWrapper.WRAP_DevConfig_RecordFinderCaps_Result outResult;
			var result = NativeWrapper.WRAP_GetMaxPageSize(LoginID, out outResult);
			return result ? outResult.nMaxPageSize : -1;
		}

		/// <summary>
		/// Получает текущее время на контроллере
		/// </summary>
		/// <returns>Время на контроллере</returns>
		public DateTime GetDateTime()
		{
			NativeWrapper.NET_TIME outResult;
			var result = NativeWrapper.WRAP_GetCurrentTime(LoginID, out outResult);
			if (result)
			{
				try
				{
					var dateTime = new DateTime(outResult.dwYear, outResult.dwMonth, outResult.dwDay, outResult.dwHour, outResult.dwMinute, outResult.dwSecond);
					return dateTime;
				}
				catch { }
			}
			return DateTime.MinValue;
		}

		/// <summary>
		/// Устанавливает время на контроллере
		/// </summary>
		/// <param name="dateTime">Время</param>
		/// <returns>true - операция завершилась успешно,
		/// false - операция завершилась с ошибками</returns>
		public bool SetDateTime(DateTime dateTime)
		{
			return NativeWrapper.WRAP_SetCurrentTime(LoginID, dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
		}

		/// <summary>
		/// Сбрасывает конфигурацию контроллера
		/// </summary>
		/// <returns>true - операция завершилась успешно,
		/// false - операция завершилась с ошибкой</returns>
		public bool Reset()
		{
			return NativeWrapper.WRAP_DeleteCfgFile(LoginID);
		}

		/// <summary>
		/// Перезагружает контроллер
		/// </summary>
		/// <returns>true - операция завершилась успешно,
		/// false- операция завершилась с ошибкой</returns>
		public bool Reboot()
		{
			return NativeWrapper.WRAP_ReBoot(LoginID);
		}

		/// <summary>
		/// Получает тип точки доступа
		/// </summary>
		/// <returns>Тип точки доступа</returns>
		public DoorType GetControllerDoorType()
		{
			NativeWrapper.WRAP_ControllerDirectionType outResult;
			var result = NativeWrapper.WRAP_GetControllerDirectionType(LoginID, out outResult);
			if (result)
			{
				var accessProperty = outResult.emAccessProperty;
				if (accessProperty == NativeWrapper.CFG_ACCESS_PROPERTY_TYPE.CFG_ACCESS_PROPERTY_BIDIRECT)
					return DoorType.TwoWay;
				if (accessProperty == NativeWrapper.CFG_ACCESS_PROPERTY_TYPE.CFG_ACCESS_PROPERTY_UNIDIRECT)
					return DoorType.OneWay;
			}
			return DoorType.TwoWay;
		}

		/// <summary>
		/// Устанавливает тип точки доступа
		/// </summary>
		/// <param name="doorType">Тип точки доступа</param>
		/// <returns>true - операция завершилась успешно,
		/// false - операция завершилась с ошибкой</returns>
		public bool SetControllerDoorType(DoorType doorType)
		{
			var nativeControllerDirectionType = NativeWrapper.CFG_ACCESS_PROPERTY_TYPE.CFG_ACCESS_PROPERTY_UNIDIRECT;
			if (doorType == DoorType.TwoWay)
				nativeControllerDirectionType = NativeWrapper.CFG_ACCESS_PROPERTY_TYPE.CFG_ACCESS_PROPERTY_BIDIRECT;
			if (doorType == DoorType.OneWay)
				nativeControllerDirectionType = NativeWrapper.CFG_ACCESS_PROPERTY_TYPE.CFG_ACCESS_PROPERTY_UNIDIRECT;
			return NativeWrapper.WRAP_SetControllerDirectionType(LoginID, nativeControllerDirectionType);
		}

		/// <summary>
		/// Устанавливает пароль для учетной записи
		/// </summary>
		/// <param name="name">Учетная запись</param>
		/// <param name="oldPassword">Старый пароль</param>
		/// <param name="password">Новый пароль</param>
		/// <returns>true - операция завершилась успешно,
		/// false - операция завершилась с ошибкой</returns>
		public bool SetControllerPassword(string name, string oldPassword, string password)
		{
			return NativeWrapper.WRAP_SetControllerPassword(LoginID, name, oldPassword, password);
		}

		/// <summary>
		/// Получает временные настройки контроллера
		/// </summary>
		/// <returns>Объект SKDControllerTimeSettings с временными настройками</returns>
		public SKDControllerTimeSettings GetControllerTimeSettings()
		{
			NativeWrapper.CFG_NTP_INFO outResult;
			var result = NativeWrapper.WRAP_GetControllerTimeConfiguration(LoginID, out outResult);
			var controllerTimeSettings = new SKDControllerTimeSettings();
			if (result)
			{
				controllerTimeSettings.IsEnabled = outResult.bEnable;
				controllerTimeSettings.Name = outResult.szAddress;
				controllerTimeSettings.Description = outResult.szTimeZoneDesc;
				controllerTimeSettings.Port = outResult.nPort;
				controllerTimeSettings.UpdatePeriod = outResult.nUpdatePeriod;
				controllerTimeSettings.TimeZone = outResult.emTimeZoneType;
			}
			return controllerTimeSettings;
		}

		/// <summary>
		/// Устанавливает временные настройки контроллера
		/// </summary>
		/// <param name="controllerTimeSettings">Временные настройки</param>
		/// <returns>true - операция завершилась успешно,
		/// false - операция завершилась с ошибкой</returns>
		public bool SetControllerTimeSettings(SKDControllerTimeSettings controllerTimeSettings)
		{
			var cfgNtpInfo = new NativeWrapper.CFG_NTP_INFO
			{
				bEnable = controllerTimeSettings.IsEnabled,
				szAddress = controllerTimeSettings.Name,
				szTimeZoneDesc = controllerTimeSettings.Description,
				nPort = controllerTimeSettings.Port,
				nUpdatePeriod = controllerTimeSettings.UpdatePeriod,
				emTimeZoneType = controllerTimeSettings.TimeZone
			};
			return NativeWrapper.WRAP_SetControllerTimeConfiguration(LoginID, cfgNtpInfo);
		}

		/// <summary>
		/// Получает конфигурацию двери
		/// </summary>
		/// <param name="doorNo">номер двери</param>
		/// <returns>объект типа DoorConfiguration</returns>
		public DoorConfiguration GetDoorConfiguration(int doorNo)
		{
			var structSize = Marshal.SizeOf(typeof(NativeWrapper.CFG_ACCESS_EVENT_INFO));
			var intPtr = Marshal.AllocCoTaskMem(structSize);

			var result = NativeWrapper.WRAP_GetDoorConfiguration(LoginID, doorNo, intPtr);

			var outResult = (NativeWrapper.CFG_ACCESS_EVENT_INFO)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.CFG_ACCESS_EVENT_INFO)));

			Marshal.FreeHGlobal(intPtr);
			intPtr = IntPtr.Zero;

			if (!result)
				return null;

			var doorConfiguration = new DoorConfiguration();

			// AccessLogItem chanel name
			doorConfiguration.ChannelName = outResult.szChannelName;
			// AccessLogItem status
			doorConfiguration.AccessState = (API.AccessState)outResult.emState;
			// AccessLogItem mode
			doorConfiguration.AccessMode = (AccessMode)outResult.emMode;
			// AccessLogItem enable mode
			doorConfiguration.EnableMode = outResult.nEnableMode;
			// Event linkage capture enabled
			doorConfiguration.IsSnapshotEnable = outResult.bSnapshotEnable;

			// Ability
			doorConfiguration.UseDoorOpenMethod = Convert.ToBoolean(outResult.abDoorOpenMethod);
			doorConfiguration.UseUnlockHoldInterval = Convert.ToBoolean(outResult.abUnlockHoldInterval);
			doorConfiguration.UseCloseTimeout = Convert.ToBoolean(outResult.abCloseTimeout);
			doorConfiguration.UseOpenAlwaysTimeIndex = Convert.ToBoolean(outResult.abOpenAlwaysTimeIndex);
			doorConfiguration.UseHolidayTimeIndex = Convert.ToBoolean(outResult.abHolidayTimeIndex);
			doorConfiguration.UseBreakInAlarmEnable = Convert.ToBoolean(outResult.abBreakInAlarmEnable);
			doorConfiguration.UseRepeatEnterAlarmEnable = Convert.ToBoolean(outResult.abRepeatEnterAlarmEnable);
			doorConfiguration.UseDoorNotClosedAlarmEnable = Convert.ToBoolean(outResult.abDoorNotClosedAlarmEnable);
			doorConfiguration.UseDuressAlarmEnable = Convert.ToBoolean(outResult.abDuressAlarmEnable);
			doorConfiguration.UseDoorTimeSection = Convert.ToBoolean(outResult.abDoorTimeSection);
			doorConfiguration.UseSensorEnable = Convert.ToBoolean(outResult.abSensorEnable);
			doorConfiguration.UseFirstEnterEnable = Convert.ToBoolean(outResult.abFirstEnterEnable);
			doorConfiguration.UseRemoteCheck = Convert.ToBoolean(outResult.abRemoteCheck);
			doorConfiguration.UseRemoteDetail = Convert.ToBoolean(outResult.abRemoteDetail);
			doorConfiguration.UseHandicapTimeOut = Convert.ToBoolean(outResult.abHandicapTimeOut);
			doorConfiguration.UseCheckCloseSensor = Convert.ToBoolean(outResult.abCheckCloseSensor);

			// Door open method
			doorConfiguration.DoorOpenMethod = (DoorOpenMethod)outResult.emDoorOpenMethod;
			// Lock hold time
			doorConfiguration.UnlockHoldInterval = outResult.nUnlockHoldInterval;
			// Close timeout
			doorConfiguration.CloseTimeout = outResult.nCloseTimeout;
			// Normally open period
			doorConfiguration.OpenAlwaysTimeIndex = outResult.nOpenAlwaysTimeIndex;
			// Time holiday segment
			doorConfiguration.HolidayTimeRecoNo = outResult.nHolidayTimeRecoNo;
			// Intrusion alarm enabled
			doorConfiguration.IsBreakInAlarmEnable = outResult.bBreakInAlarmEnable;
			// Repeat enter alarm enabled
			doorConfiguration.IsRepeatEnterAlarmEnable = outResult.bRepeatEnterAlarm;
			// Door not closed alarm enabled
			doorConfiguration.IsDoorNotClosedAlarmEnable = outResult.bDoorNotClosedAlarmEnable;
			// Duress alarm enabled
			doorConfiguration.IsDuressAlarmEnable = outResult.bDuressAlarmEnable;

			// Time sections
			var doorDayIntervalsCollection = new DoorDayIntervalsCollection();
			for (var i = 0; i < 7; i++)
			{
				var doorDayInterval = new DoorDayInterval();
				for (var j = 0; j < 4; j++)
				{
					var dooropenTimesectionInfo = outResult.stuDoorTimeSection[i * 4 + j];
					var doorDayIntervalPart = new DoorDayIntervalPart
					{
						StartHour = dooropenTimesectionInfo.stuTime.stuStartTime.dwHour,
						StartMinute = dooropenTimesectionInfo.stuTime.stuStartTime.dwMinute,
						EndHour = dooropenTimesectionInfo.stuTime.stuEndTime.dwHour,
						EndMinute = dooropenTimesectionInfo.stuTime.stuEndTime.dwMinute,
						DoorOpenMethod = (SKDDoorConfiguration_DoorOpenMethod) dooropenTimesectionInfo.emDoorOpenMethod
					};
					doorDayInterval.DoorDayIntervalParts.Add(doorDayIntervalPart);
				}
				doorDayIntervalsCollection.DoorDayIntervals.Add(doorDayInterval);
			}
			doorConfiguration.DoorDayIntervalsCollection = doorDayIntervalsCollection;

			// Magnetic enabled
			doorConfiguration.IsSensorEnable = outResult.bSensorEnable;

			// First enter info
			doorConfiguration.FirstEnterInfo = new DoorFirstEnterInfo
			{
				bEnable = outResult.stuFirstEnterInfo.bEnable,
				emStatus = (DoorFirstEnterStatus)outResult.stuFirstEnterInfo.emStatus,
				nTimeIndex = outResult.stuFirstEnterInfo.nTimeIndex
			};

			// Remote check
			doorConfiguration.IsRemoteCheck = outResult.bRemoteCheck;

			// Remote detail
			doorConfiguration.RemoteDetail = new RemoteDetailInfo
			{
				nTimeOut = outResult.stuRemoteDetail.nTimeOut,
				bTimeOutDoorStatus = outResult.stuRemoteDetail.bTimeOutDoorStatus
			};

			// Handicap timeout
			doorConfiguration.HandicapTimeout = new HandicapTimeoutInfo
			{
				nUnlockHoldInterval = outResult.stuHandicapTimeOut.nUnlockHoldInterval,
				nCloseTimeout = outResult.stuHandicapTimeOut.nCloseTimeout
			};

			// Закрывать замок при закрытии двери
			doorConfiguration.IsCloseCheckSensor = outResult.bCloseCheckSensor;

			return doorConfiguration;
		}

		/// <summary>
		/// Устанавливает конфигурацию для двери
		/// </summary>
		/// <param name="doorConfiguration">конфигурация</param>
		/// <param name="doorNo">номер двери, для которой устанавливается конфигурация</param>
		/// <returns>true в случае успеха, false в противном случае</returns>
		public bool SetDoorConfiguration(DoorConfiguration doorConfiguration, int doorNo)
		{
			var info = new NativeWrapper.CFG_ACCESS_EVENT_INFO();

			// AccessLogItem chanel name
			info.szChannelName = doorConfiguration.ChannelName;
			// AccessLogItem status
			info.emState = (NativeWrapper.CFG_ACCESS_STATE)doorConfiguration.AccessState;
			// AccessLogItem mode
			info.emMode = (NativeWrapper.CFG_ACCESS_MODE)doorConfiguration.AccessMode;
			// AccessLogItem enable mode
			info.nEnableMode = doorConfiguration.EnableMode;
			// Event linkage capture enabled
			info.bSnapshotEnable = doorConfiguration.IsSnapshotEnable;

			// Ability
			info.abDoorOpenMethod = Convert.ToByte(doorConfiguration.UseDoorOpenMethod);
			info.abUnlockHoldInterval = Convert.ToByte(doorConfiguration.UseUnlockHoldInterval);
			info.abCloseTimeout = Convert.ToByte(doorConfiguration.UseCloseTimeout);
			info.abOpenAlwaysTimeIndex = Convert.ToByte(doorConfiguration.UseOpenAlwaysTimeIndex);
			info.abHolidayTimeIndex = Convert.ToByte(doorConfiguration.UseHolidayTimeIndex);
			info.abBreakInAlarmEnable = Convert.ToByte(doorConfiguration.UseBreakInAlarmEnable);
			info.abRepeatEnterAlarmEnable = Convert.ToByte(doorConfiguration.UseRepeatEnterAlarmEnable);
			info.abDoorNotClosedAlarmEnable = Convert.ToByte(doorConfiguration.UseDoorNotClosedAlarmEnable);
			info.abDuressAlarmEnable = Convert.ToByte(doorConfiguration.UseDuressAlarmEnable);
			info.abDoorTimeSection = Convert.ToByte(doorConfiguration.UseDoorTimeSection);
			info.abSensorEnable = Convert.ToByte(doorConfiguration.UseSensorEnable);
			info.abFirstEnterEnable = Convert.ToByte(doorConfiguration.UseFirstEnterEnable);
			info.abRemoteCheck = Convert.ToByte(doorConfiguration.UseRemoteCheck);
			info.abRemoteDetail = Convert.ToByte(doorConfiguration.UseRemoteDetail);
			info.abHandicapTimeOut = Convert.ToByte(doorConfiguration.UseHandicapTimeOut);
			info.abCheckCloseSensor = Convert.ToByte(doorConfiguration.UseCheckCloseSensor);

			// Door open method
			info.emDoorOpenMethod = (NativeWrapper.CFG_DOOR_OPEN_METHOD)doorConfiguration.DoorOpenMethod;
			// Lock hold time
			info.nUnlockHoldInterval = doorConfiguration.UnlockHoldInterval;
			// Close timeout
			info.nCloseTimeout = doorConfiguration.CloseTimeout;
			// Normally open period
			info.nOpenAlwaysTimeIndex = 255;
			// Time holiday segment
			info.nHolidayTimeRecoNo = 255;
			// Intrusion alarm enabled
			info.bBreakInAlarmEnable = doorConfiguration.IsBreakInAlarmEnable;
			// Repeat enter alarm enabled
			info.bRepeatEnterAlarm = doorConfiguration.IsRepeatEnterAlarmEnable;
			// Door not closed alarm enabled
			info.bDoorNotClosedAlarmEnable = doorConfiguration.IsDoorNotClosedAlarmEnable;
			// Duress alarm enabled
			info.bDuressAlarmEnable = doorConfiguration.IsDuressAlarmEnable;

			// Time sections
			info.stuDoorTimeSection = new NativeWrapper.CFG_DOOROPEN_TIMESECTION_INFO[7 * 4];
			for (var i = 0; i < 7; i++)
			{
				for (var j = 0; j < 4; j++)
				{
					var doorDayIntervalPart = doorConfiguration.DoorDayIntervalsCollection.DoorDayIntervals[i].DoorDayIntervalParts[j];
					info.stuDoorTimeSection[i * 4 + j].stuTime.stuStartTime.dwHour = doorDayIntervalPart.StartHour;
					info.stuDoorTimeSection[i * 4 + j].stuTime.stuStartTime.dwMinute = doorDayIntervalPart.StartMinute;
					info.stuDoorTimeSection[i * 4 + j].stuTime.stuStartTime.dwSecond = 0;
					info.stuDoorTimeSection[i * 4 + j].stuTime.stuEndTime.dwHour = doorDayIntervalPart.EndHour;
					info.stuDoorTimeSection[i * 4 + j].stuTime.stuEndTime.dwMinute = doorDayIntervalPart.EndMinute;
					info.stuDoorTimeSection[i * 4 + j].stuTime.stuEndTime.dwSecond = 0;
					info.stuDoorTimeSection[i * 4 + j].emDoorOpenMethod = (NativeWrapper.CFG_DOOR_OPEN_METHOD)doorDayIntervalPart.DoorOpenMethod;
				}
			}

			// Magnetic enabled
			info.bSensorEnable = doorConfiguration.IsSensorEnable;

			// First enter info
			info.stuFirstEnterInfo.bEnable = doorConfiguration.FirstEnterInfo.bEnable;
			info.stuFirstEnterInfo.emStatus = (NativeWrapper.CFG_ACCESS_FIRSTENTER_STATUS)doorConfiguration.FirstEnterInfo.emStatus;
			info.stuFirstEnterInfo.nTimeIndex = doorConfiguration.FirstEnterInfo.nTimeIndex;

			// Remote check
			info.bRemoteCheck = doorConfiguration.IsRemoteCheck;

			// Remote detail
			info.stuRemoteDetail.nTimeOut = doorConfiguration.RemoteDetail.nTimeOut;
			info.stuRemoteDetail.bTimeOutDoorStatus = doorConfiguration.RemoteDetail.bTimeOutDoorStatus;

			// Handicap timeout
			info.stuHandicapTimeOut.nUnlockHoldInterval = doorConfiguration.HandicapTimeout.nUnlockHoldInterval;
			info.stuHandicapTimeOut.nCloseTimeout = doorConfiguration.HandicapTimeout.nCloseTimeout;

			// Закрывать замок при закрытии двери
			info.bCloseCheckSensor = doorConfiguration.IsCloseCheckSensor;

			var result = NativeWrapper.WRAP_SetDoorConfiguration(LoginID, doorNo, ref info);
			return result;
		}

		/// <summary>
		/// Обновляет прошивку на контроллере
		/// </summary>
		/// <param name="fileName">Путь к файлу прошивки</param>
		/// <returns>true - операция завершилась успешно,
		/// false - операция завершилась с ошибкой</returns>
		public bool UpdateFirmware(string fileName)
		{
			return NativeWrapper.WRAP_Upgrade(LoginID, fileName);
		}

		#endregion CommonDevice
	}
}