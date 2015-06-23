﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	/// <summary>
	/// Описывает конфигурацию двери/замка
	/// </summary>
	[DataContract]
	public class SKDDoorConfiguration
	{
		public SKDDoorConfiguration()
		{
			AccessState = AccessState.Normal;
			DoorOpenMethod = SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_CARD;
			UnlockHoldInterval = 2000;
			CloseTimeout = 0;

			RemoteDetail = new RemoteDetail();
			HandicapTimeout = new HandicapTimeout();

			// Включено всегда, отсутствует на интерфейсе пользователя
			IsDuressAlarmEnable = true;
		}

		[DataMember]
		public AccessState AccessState { get; set; }

		/// <summary>
		/// Метод открытия двери
		/// </summary>
		[DataMember]
		public SKDDoorConfiguration_DoorOpenMethod DoorOpenMethod { get; set; }

		/// <summary>
		/// Время удержания
		/// </summary>
		[DataMember]
		public int UnlockHoldInterval { get; set; }

		/// <summary>
		/// Время закрытия
		/// </summary>
		[DataMember]
		public int CloseTimeout { get; set; }

		[DataMember]
		public int OpenAlwaysTimeIndex { get; set; }

		[DataMember]
		public int HolidayTimeRecoNo { get; set; }

		/// <summary>
		/// Тревога по взлому
		/// </summary>
		[DataMember]
		public bool IsBreakInAlarmEnable { get; set; }

		[DataMember]
		public bool IsRepeatEnterAlarmEnable { get; set; }

		/// <summary>
		/// Тревога по незакрытию двери
		/// </summary>
		[DataMember]
		public bool IsDoorNotClosedAlarmEnable { get; set; }

		/// <summary>
		/// Тревога по принуждению
		/// </summary>
		[DataMember]
		public bool IsDuressAlarmEnable { get; set; }

		[DataMember]
		public bool IsSensorEnable { get; set; }

		/// <summary>
		/// Проход с подтверждением
		/// </summary>
		[DataMember]
		public bool IsRemoteCheck { get; set; }

		/// <summary>
		/// Параметры режима "Проход с подтверждением"
		/// </summary>
		[DataMember]
		public RemoteDetail RemoteDetail { get; set; }

		/// <summary>
		/// Параметры режима работы замка для людей с ограниченными возможностями
		/// </summary>
		[DataMember]
		public HandicapTimeout HandicapTimeout { get; set; }
	}


	public enum AccessState
	{
		Normal,
		CloseAlways,
		OpenAlways
	}

	/// <summary>
	/// Возможные методы открытия двери
	/// </summary>
	public enum SKDDoorConfiguration_DoorOpenMethod
	{
		[Description("Неизвестно")]
		CFG_DOOR_OPEN_METHOD_UNKNOWN = 0,
		[Description("Только пароль")]
		CFG_DOOR_OPEN_METHOD_PWD_ONLY,
		[Description("Карта")]
		CFG_DOOR_OPEN_METHOD_CARD,
		[Description("Пароль или карта")]
		CFG_DOOR_OPEN_METHOD_PWD_OR_CARD,
		[Description("Сначала карта")]
		CFG_DOOR_OPEN_METHOD_CARD_FIRST,
		[Description("Сначала пароль")]
		CFG_DOOR_OPEN_METHOD_PWD_FIRST,
		[Description("Недельный график")]
		CFG_DOOR_OPEN_METHOD_SECTION,
		[Description("Только отпечаток пальца")]
		CFG_DOOR_OPEN_METHOD_FINGERPRINTONLY = 7,
		[Description("Пароль или карта или отпечаток пальца")]
		CFG_DOOR_OPEN_METHOD_PWD_OR_CARD_OR_FINGERPRINT = 8,
		[Description("Карта и отпечаток пальца")]
		CFG_DOOR_OPEN_METHOD_CARD_AND_FINGERPRINT = 11,
		[Description("Multiplayer Unlock")]
		CFG_DOOR_OPEN_METHOD_MULTI_PERSON = 12
	}

	[DataContract]
	public class DoorDayIntervalsCollection
	{
		public DoorDayIntervalsCollection()
		{
			DoorDayIntervals = new List<DoorDayInterval>();
		}

		[DataMember]
		public List<DoorDayInterval> DoorDayIntervals { get; set; }
	}

	[DataContract]
	public class DoorDayInterval
	{
		public DoorDayInterval()
		{
			DoorDayIntervalParts = new List<DoorDayIntervalPart>();
		}

		[DataMember]
		public List<DoorDayIntervalPart> DoorDayIntervalParts { get; set; }
	}

	[DataContract]
	public class DoorDayIntervalPart
	{
		public DoorDayIntervalPart()
		{
			DoorOpenMethod = SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_CARD;
		}

		[DataMember]
		public int StartHour { get; set; }

		[DataMember]
		public int StartMinute { get; set; }

		[DataMember]
		public int EndHour { get; set; }

		[DataMember]
		public int EndMinute { get; set; }

		[DataMember]
		public SKDDoorConfiguration_DoorOpenMethod? DoorOpenMethod { get; set; }
	}

	/// <summary>
	/// Описывает параметры режима "Проход с подтверждением"
	/// </summary>
	[DataContract]
	public class RemoteDetail
	{
		/// <summary>
		/// Время ожидания
		/// </summary>
		[DataMember]
		public int TimeOut { get; set; }

		/// <summary>
		/// Состояние замка по истечению времени ожидания
		/// </summary>
		[DataMember]
		public bool TimeOutDoorStatus { get; set; }
	}

	/// <summary>
	/// Описывает параметры режима работы замка для людей с ограниченными возможностями
	/// </summary>
	[DataContract]
	public class HandicapTimeout
	{
		/// <summary>
		/// Альтернативное время удержания
		/// </summary>
		[DataMember]
		public int nUnlockHoldInterval;

		/// <summary>
		/// Альтернативное время закрытия
		/// </summary>
		[DataMember]
		public int nCloseTimeout;
	}
}