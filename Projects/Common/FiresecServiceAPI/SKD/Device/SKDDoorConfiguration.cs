using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Localization;

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
			DoorDayIntervalsCollection = new DoorDayIntervalsCollection();

			// Включено всегда, отсутствует на интерфейсе пользователя
			IsDuressAlarmEnable = true;

			WeeklyIntervalID = -1;
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

		[DataMember]
		public int WeeklyIntervalID { get; set; }

		/// <summary>
		/// График работы замка
		/// </summary>
		[DataMember]
		public DoorDayIntervalsCollection DoorDayIntervalsCollection { get; set; }

		/// <summary>
		/// Закрывать замок при закрытии двери
		/// </summary>
		[DataMember]
		public bool IsCloseCheckSensor { get; set; }
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
		//[Description("Неизвестно")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Device.SKDDoorConfiguration), "CFG_DOOR_OPEN_METHOD_UNKNOWN")]
		CFG_DOOR_OPEN_METHOD_UNKNOWN = 0,

		//[Description("Пароль")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Device.SKDDoorConfiguration), "CFG_DOOR_OPEN_METHOD_PWD_ONLY")]
        CFG_DOOR_OPEN_METHOD_PWD_ONLY,

		//[Description("Карта")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Device.SKDDoorConfiguration), "CFG_DOOR_OPEN_METHOD_CARD")]
        CFG_DOOR_OPEN_METHOD_CARD,

		//[Description("Пароль или карта")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Device.SKDDoorConfiguration), "CFG_DOOR_OPEN_METHOD_PWD_OR_CARD")]
        CFG_DOOR_OPEN_METHOD_PWD_OR_CARD,

		//[Description("Сначала карта")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Device.SKDDoorConfiguration), "CFG_DOOR_OPEN_METHOD_CARD_FIRST")]
        CFG_DOOR_OPEN_METHOD_CARD_FIRST,

		//[Description("Сначала пароль")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Device.SKDDoorConfiguration), "CFG_DOOR_OPEN_METHOD_PWD_FIRST")]
        CFG_DOOR_OPEN_METHOD_PWD_FIRST,

		//[Description("График замка")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Device.SKDDoorConfiguration), "CFG_DOOR_OPEN_METHOD_SECTION")]
        CFG_DOOR_OPEN_METHOD_SECTION,

		//[Description("Только отпечаток пальца")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Device.SKDDoorConfiguration), "CFG_DOOR_OPEN_METHOD_FINGERPRINTONLY")]
        CFG_DOOR_OPEN_METHOD_FINGERPRINTONLY = 7,

		//[Description("Пароль или карта или отпечаток пальца")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Device.SKDDoorConfiguration), "CFG_DOOR_OPEN_METHOD_PWD_OR_CARD_OR_FINGERPRINT")]
        CFG_DOOR_OPEN_METHOD_PWD_OR_CARD_OR_FINGERPRINT = 8,

		//[Description("Карта и отпечаток пальца")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Device.SKDDoorConfiguration), "CFG_DOOR_OPEN_METHOD_CARD_AND_FINGERPRINT")]
        CFG_DOOR_OPEN_METHOD_CARD_AND_FINGERPRINT = 11,

		//[Description("Multiplayer Unlock")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Device.SKDDoorConfiguration), "CFG_DOOR_OPEN_METHOD_MULTI_PERSON")]
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
		public SKDDoorConfiguration_DoorOpenMethod DoorOpenMethod { get; set; }
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