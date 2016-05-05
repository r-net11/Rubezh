using FiresecAPI.SKD;
using System.ComponentModel;

namespace StrazhDeviceSDK.API
{
	public class DoorConfiguration
	{
		public DoorConfiguration()
		{
			// По умолчанию все Ability = true
			UseDoorOpenMethod = true;
			UseUnlockHoldInterval = true;
			UseCloseTimeout = true;
			UseOpenAlwaysTimeIndex = true;
			UseHolidayTimeIndex = true;
			UseBreakInAlarmEnable = true;
			UseRepeatEnterAlarmEnable = true;
			UseDoorNotClosedAlarmEnable = true;
			UseDuressAlarmEnable = true;
			UseDoorTimeSection = true;
			UseSensorEnable = true;
			UseFirstEnterEnable = true;
			UseRemoteCheck = true;
			UseRemoteDetail = true;
			UseHandicapTimeOut = true;
			UseCheckCloseSensor = true;

			DoorDayIntervalsCollection = new DoorDayIntervalsCollection();
			var firstEnterInfo = new DoorFirstEnterInfo();
			firstEnterInfo.emStatus = DoorFirstEnterStatus.ACCESS_FIRSTENTER_STATUS_NORMAL;
			FirstEnterInfo = firstEnterInfo;
			RemoteDetail = new RemoteDetailInfo();
			HandicapTimeout = new HandicapTimeoutInfo();
		}

		/// <summary>
		/// Access chanel name
		/// </summary>
		public string ChannelName { get; set; }

		/// <summary>
		/// Access status
		/// </summary>
		public AccessState AccessState { get; set; }

		/// <summary>
		/// Access mode
		/// </summary>
		public AccessMode AccessMode { get; set; }

		/// <summary>
		/// Access enable level value: 0 - low active (power start); 1 - active high (power-up)
		/// </summary>
		public int EnableMode { get; set; }

		/// <summary>
		/// Event linkage capture enabled
		/// </summary>
		public bool IsSnapshotEnable { get; set; }

		// Ability
		public bool UseDoorOpenMethod { get; set; }

		public bool UseUnlockHoldInterval { get; set; }

		public bool UseCloseTimeout { get; set; }

		public bool UseOpenAlwaysTimeIndex { get; set; }

		public bool UseHolidayTimeIndex { get; set; }

		public bool UseBreakInAlarmEnable { get; set; }

		public bool UseRepeatEnterAlarmEnable { get; set; }

		public bool UseDoorNotClosedAlarmEnable { get; set; }

		public bool UseDuressAlarmEnable { get; set; }

		public bool UseDoorTimeSection { get; set; }

		public bool UseSensorEnable { get; set; }

		public bool UseFirstEnterEnable { get; set; }

		public bool UseRemoteCheck { get; set; }

		public bool UseRemoteDetail { get; set; }

		public bool UseHandicapTimeOut { get; set; }

		public bool UseCheckCloseSensor { get; set; }

		/// <summary>
		/// Door open method
		/// </summary>
		public DoorOpenMethod DoorOpenMethod { get; set; }

		/// <summary>
		/// Lock hold time (automatic closing time), in milliseconds, [250, 20,000]
		/// </summary>
		public int UnlockHoldInterval { get; set; }

		/// <summary>
		/// Close timeout value does not exceed the threshold will trigger off an alarm, in seconds, [0,9999]; 0 indicates no detection timeout
		/// </summary>
		public int CloseTimeout { get; set; }

		/// <summary>
		/// Normally open period, the value CFG_ACCESS_TIMESCHEDULE_INFO configured array subscript
		/// </summary>
		public int OpenAlwaysTimeIndex { get; set; }

		/// <summary>
		/// Time holiday segment, is the holiday record set record number corresponding to the nRecNo NET_RECORDSET_HOLIDAY
		/// </summary>
		public int HolidayTimeRecoNo { get; set; }

		/// <summary>
		/// Intrusion alarm enabled
		/// </summary>
		public bool IsBreakInAlarmEnable { get; set; }

		/// <summary>
		/// Repeat enter alarm enabled
		/// </summary>
		public bool IsRepeatEnterAlarmEnable { get; set; }

		/// <summary>
		/// Door not closed alarm enabled
		/// </summary>
		public bool IsDoorNotClosedAlarmEnable { get; set; }

		/// <summary>
		/// Duress alarm enabled
		/// </summary>
		public bool IsDuressAlarmEnable { get; set; }

		/// <summary>
		/// Time section
		/// </summary>
		public DoorDayIntervalsCollection DoorDayIntervalsCollection { get; set; }

		/// <summary>
		/// Magnetic enabled
		/// </summary>
		public bool IsSensorEnable { get; set; }

		/// <summary>
		/// First enter info
		/// </summary>
		public DoorFirstEnterInfo FirstEnterInfo { get; set; }

		/// <summary>
		/// The need for platform verification, TRUE expressed the need for a platform after authentication to open after permissions,
		/// FALSE represents permission to open the door immediately after verification by
		/// </summary>
		public bool IsRemoteCheck { get; set; }

		/// <summary>
		/// BRemoteCheck used in conjunction with, if the remote verification is not answered,
		/// the device is set to timeout after a normal open or not to open the door
		/// </summary>
		public RemoteDetailInfo RemoteDetail { get; set; }

		/// <summary>
		/// Parameters for the disabled door
		/// </summary>
		public HandicapTimeoutInfo HandicapTimeout { get; set; }

		/// <summary>
		/// Закрывать замок при закрытии двери
		/// </summary>
		public bool IsCloseCheckSensor { get; set; }
	}

	public enum AccessState
	{
		[Description("Нормальный")]
		ACCESS_STATE_NORMAL,

		[Description("Всегда закрыто")]
		ACCESS_STATE_CLOSEALWAYS,

		[Description("Всегда открыто")]
		ACCESS_STATE_OPENALWAYS
	}

	public enum AccessMode
	{
		[Description("Под защитой")]
		ACCESS_MODE_HANDPROTECTED,

		[Description("Безопасная комната")]
		ACCESS_MODE_SAFEROOM,

		[Description("Прочее")]
		ACCESS_MODE_OTHER
	}

	public enum DoorOpenMethod
	{
		[Description("Не известно")]
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

		[Description("Секция")]
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

	public enum DoorFirstEnterStatus
	{
		[Description("Не известно")]
		ACCESS_FIRSTENTER_STATUS_UNKNOWN = 0,

		[Description("Держать открытой")]
		ACCESS_FIRSTENTER_STATUS_KEEPOPEN = 1,

		[Description("Нормальный")]
		ACCESS_FIRSTENTER_STATUS_NORMAL = 2
	}

	public struct DoorFirstEnterInfo
	{
		public bool bEnable;
		public DoorFirstEnterStatus emStatus;
		public int nTimeIndex;
	}

	public struct RemoteDetailInfo
	{
		public int nTimeOut;
		public bool bTimeOutDoorStatus;
	}

	public struct HandicapTimeoutInfo
	{
		public int nUnlockHoldInterval;
		public int nCloseTimeout;
	}
}