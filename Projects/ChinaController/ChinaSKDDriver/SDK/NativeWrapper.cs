using FiresecAPI.SKD;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using LocalizationConveters;
using StrazhAPI.SKD;

namespace ChinaSKDDriverNativeApi
{
	public class NativeWrapper
	{
		#region Common

		[DllImport(@"CPPWrapper.dll")]
		public static extern void WRAP_Initialize();

		[DllImport(@"CPPWrapper.dll")]
		public static extern void WRAP_Deinitialize();

		[DllImport(@"CPPWrapper.dll")]
		public static extern int WRAP_Connect(string ipAddress, int port, string userName, string password, out int error);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Disconnect(int loginID);

		[DllImport(@"CPPWrapper.dll")]
		public static extern int WRAP_Reconnect(string ipAddress, int port, string userName, string password);

		[StructLayout(LayoutKind.Sequential)]
		public struct NET_TIME
		{
			public Int32 dwYear;
			public Int32 dwMonth;
			public Int32 dwDay;
			public Int32 dwHour;
			public Int32 dwMinute;
			public Int32 dwSecond;
		}

		public enum DH_LOG_QUERY_TYPE
		{
			DHLOG_ALL = 0,
			DHLOG_SYSTEM,
			DHLOG_CONFIG,
			DHLOG_STORAGE,
			DHLOG_ALARM,
			DHLOG_RECORD,
			DHLOG_ACCOUNT,
			DHLOG_CLEAR,
			DHLOG_PLAYBACK,
			DHLOG_MANAGER
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct QUERY_DEVICE_LOG_PARAM
		{
			public DH_LOG_QUERY_TYPE emLogType;
			public NET_TIME stuStartTime;
			public NET_TIME stuEndTime;
			public Int32 nStartNum;
			public Int32 nEndNum;
			public Byte nLogStuType;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
			public Byte[] reserved;

			public UInt32 nChannelID;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
			public Byte[] bReserved;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct DHDEVTIME
		{
			public Int32 second;
			public Int32 minute;
			public Int32 hour;
			public Int32 day;
			public Int32 month;
			public Int32 year;
		}

		public enum CFG_ACCESS_STATE
		{
			ACCESS_STATE_NORMAL,
			ACCESS_STATE_CLOSEALWAYS,
			ACCESS_STATE_OPENALWAYS,
		}

		public enum CFG_ACCESS_MODE
		{
			ACCESS_MODE_HANDPROTECTED,
			ACCESS_MODE_SAFEROOM,
			ACCESS_MODE_OTHER,
		}

		public enum CFG_DOOR_OPEN_METHOD
		{
			CFG_DOOR_OPEN_METHOD_UNKNOWN = 0,
			CFG_DOOR_OPEN_METHOD_PWD_ONLY,
			CFG_DOOR_OPEN_METHOD_CARD,
			CFG_DOOR_OPEN_METHOD_PWD_OR_CARD,
			CFG_DOOR_OPEN_METHOD_CARD_FIRST,
			CFG_DOOR_OPEN_METHOD_PWD_FIRST,
			CFG_DOOR_OPEN_METHOD_SECTION,
			CFG_DOOR_OPEN_METHOD_FINGERPRINTONLY = 7,
			CFG_DOOR_OPEN_METHOD_PWD_OR_CARD_OR_FINGERPRINT = 8,
			CFG_DOOR_OPEN_METHOD_CARD_AND_FINGERPRINT = 11,
			CFG_DOOR_OPEN_METHOD_MULTI_PERSON = 12
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct CFG_TIME
		{
			public Int32 dwHour;
			public Int32 dwMinute;
			public Int32 dwSecond;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct CFG_TIME_PERIOD
		{
			public CFG_TIME stuStartTime;
			public CFG_TIME stuEndTime;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct CFG_DOOROPEN_TIMESECTION_INFO
		{
			public CFG_TIME_PERIOD stuTime;
			public CFG_DOOR_OPEN_METHOD emDoorOpenMethod;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct WRAP_DevConfig_TypeAndSoftInfo_Result
		{
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szDevType;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string szSoftWareVersion;

			public Int32 dwSoftwareBuildDate_Year;
			public Int32 dwSoftwareBuildDate_Month;
			public Int32 dwSoftwareBuildDate_Day;
		}

		/// <summary>
		/// Получает информацию о прошивке на контроллере
		/// </summary>
		/// <param name="loginID">Идентификатор текущей сессии</param>
		/// <param name="result">Структура WRAP_DevConfig_TypeAndSoftInfo_Result, в которую помещается запрошенная информация о прошивке на контроллере</param>
		/// <returns>true - операция завершилась успешно,
		/// false - операция завершилась с ошибками</returns>
		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_GetSoftwareInfo(Int32 loginID, out WRAP_DevConfig_TypeAndSoftInfo_Result result);

		[StructLayout(LayoutKind.Sequential)]
		public struct WRAP_CFG_NETWORK_INFO_Result
		{
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
			public string szIP;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
			public string szSubnetMask;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
			public string szDefGateway;

			public Int32 nMTU;
		}

		/// <summary>
		/// Получает сетевые настройки контроллера
		/// </summary>
		/// <param name="loginID">Идентификатор текущей сессии</param>
		/// <param name="stuNetwork">Структура WRAP_CFG_NETWORK_INFO_Result, в которую помещается запрошенная информация о сетевых настройках на контроллере</param>
		/// <returns>true - операция завершилась успешно,
		/// false - операция завершилась с ошибками</returns>
		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Get_NetInfo(int loginID, out WRAP_CFG_NETWORK_INFO_Result stuNetwork);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Set_NetInfo(int loginID, string ip, string mask, string gate, int mtu);

		[StructLayout(LayoutKind.Sequential)]
		public struct WRAP_DevConfig_MAC_Result
		{
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
			public string szMAC;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_GetMacAddress(int loginID, out WRAP_DevConfig_MAC_Result result);

		[StructLayout(LayoutKind.Sequential)]
		public struct WRAP_DevConfig_RecordFinderCaps_Result
		{
			public int nMaxPageSize;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_GetMaxPageSize(int loginID, out WRAP_DevConfig_RecordFinderCaps_Result result);

		/// <summary>
		/// Получает текущее время на контроллере
		/// </summary>
		/// <param name="loginID">Идентификатор текущей сессии</param>
		/// <param name="result">Структура NET_TIME, в которую помещается информация о текущем времени на контроллере</param>
		/// <returns>true - операция завершилась успешно,
		///  false - операция завершилась с ошибкой</returns>
		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_GetCurrentTime(int loginID, out NET_TIME result);

		/// <summary>
		/// Устанавливает время на контроллере
		/// </summary>
		/// <param name="loginID">Идентификатор текущей сессии</param>
		/// <param name="dwYear">Год</param>
		/// <param name="dwMonth">Месяц</param>
		/// <param name="dwDay">Число</param>
		/// <param name="dwHour">Часы</param>
		/// <param name="dwMinute">Минуты</param>
		/// <param name="dwSecond">Секунды</param>
		/// <returns>true - операция завершилась успешно,
		/// false - операция завершилась с ошибками</returns>
		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_SetCurrentTime(int loginID, int dwYear, int dwMonth, int dwDay, int dwHour, int dwMinute, int dwSecond);

		public enum CFG_ACCESS_PROPERTY_TYPE
		{
			CFG_ACCESS_PROPERTY_UNKNOWN = 0,
			CFG_ACCESS_PROPERTY_BIDIRECT,
			CFG_ACCESS_PROPERTY_UNIDIRECT,
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct WRAP_ControllerDirectionType
		{
			public CFG_ACCESS_PROPERTY_TYPE emAccessProperty;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_GetControllerDirectionType(int loginID, out WRAP_ControllerDirectionType result);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_SetControllerDirectionType(int loginID, CFG_ACCESS_PROPERTY_TYPE emAccessProperty);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_SetControllerPassword(int loginID, string name, string oldPassword, string password);

		public struct CFG_NTP_INFO
		{
			public bool bEnable;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
			public string szAddress;

			public int nPort;
			public int nUpdatePeriod;
			public SKDTimeZoneType emTimeZoneType;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string szTimeZoneDesc;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_GetControllerTimeConfiguration(int loginID, out CFG_NTP_INFO result);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_SetControllerTimeConfiguration(int loginID, CFG_NTP_INFO cfg_NTP_INFO);

		public enum CFG_ACCESS_FIRSTENTER_STATUS
		{
			ACCESS_FIRSTENTER_STATUS_UNKNOWN = 0,
			ACCESS_FIRSTENTER_STATUS_KEEPOPEN = 1,
			ACCESS_FIRSTENTER_STATUS_NORMAL = 2
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct CFG_ACCESS_FIRSTENTER_INFO
		{
			public bool bEnable;
			public CFG_ACCESS_FIRSTENTER_STATUS emStatus;
			public int nTimeIndex;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct CFG_REMOTE_DETAIL_INFO
		{
			public int nTimeOut;
			public bool bTimeOutDoorStatus;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct CFG_HANDICAP_TIMEOUT_INFO
		{
			public int nUnlockHoldInterval;
			public int nCloseTimeout;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct CFG_ACCESS_EVENT_INFO
		{
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string szChannelName;

			public CFG_ACCESS_STATE emState;
			public CFG_ACCESS_MODE emMode;
			public int nEnableMode;
			public bool bSnapshotEnable;

			public byte abDoorOpenMethod;
			public byte abUnlockHoldInterval;
			public byte abCloseTimeout;
			public byte abOpenAlwaysTimeIndex;
			public byte abHolidayTimeIndex;
			public byte abBreakInAlarmEnable;
			public byte abRepeatEnterAlarmEnable;
			public byte abDoorNotClosedAlarmEnable;
			public byte abDuressAlarmEnable;
			public byte abDoorTimeSection;
			public byte abSensorEnable;
			public byte abFirstEnterEnable;
			public byte abRemoteCheck;
			public byte abRemoteDetail;
			public byte abHandicapTimeOut;
			public byte abCheckCloseSensor;

			public CFG_DOOR_OPEN_METHOD emDoorOpenMethod;
			public int nUnlockHoldInterval;
			public int nCloseTimeout;
			public int nOpenAlwaysTimeIndex;
			public int nHolidayTimeRecoNo;
			public bool bBreakInAlarmEnable;
			public bool bRepeatEnterAlarm;
			public bool bDoorNotClosedAlarmEnable;
			public bool bDuressAlarmEnable;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 7 * 4)]
			public CFG_DOOROPEN_TIMESECTION_INFO[] stuDoorTimeSection;

			public bool bSensorEnable;

			public CFG_ACCESS_FIRSTENTER_INFO stuFirstEnterInfo;
			public bool bRemoteCheck;
			public CFG_REMOTE_DETAIL_INFO stuRemoteDetail;
			public CFG_HANDICAP_TIMEOUT_INFO stuHandicapTimeOut;
			public bool bCloseCheckSensor;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_GetDoorsCount(ref int nCount);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_GetDoorConfiguration(int loginID, int channelNo, IntPtr result);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_SetDoorConfiguration(int loginID, int channelNo, ref CFG_ACCESS_EVENT_INFO accessEventInfo);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_ReBoot(int loginID);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_DeleteCfgFile(int loginID);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_OpenDoor(int loginID, int channelNo);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_CloseDoor(int loginID, int channelNo);

		[DllImport(@"CPPWrapper.dll")]
		public static extern int WRAP_GetDoorStatus(int loginID, int channelNo);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_PromptWarning(int loginID, int channelNo);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Upgrade(int loginID, string fileName);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool TestStruct(IntPtr result);

		#endregion Common

		#region Cards

		public enum NET_ACCESSCTLCARD_STATE
		{
			NET_ACCESSCTLCARD_STATE_UNKNOWN = -1,
			NET_ACCESSCTLCARD_STATE_NORMAL = 0,                 // Normal
			NET_ACCESSCTLCARD_STATE_LOSE   = 0x01,              // Lose
			NET_ACCESSCTLCARD_STATE_LOGOFF = 0x02,              // Logoff
			NET_ACCESSCTLCARD_STATE_FREEZE = 0x04,              // Freeze
			NET_ACCESSCTLCARD_STATE_ARREARAGE = 0x08,           // Arrears
			NET_ACCESSCTLCARD_STATE_OVERDUE = 0x10,             // Overdue
		}

		public enum NET_ACCESSCTLCARD_TYPE
		{
			//[DescriptionAttribute("Неизвестно")]
            [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.SDK.NativeWrapper), "NET_ACCESSCTLCARD_TYPE_UNKNOWN")]
			NET_ACCESSCTLCARD_TYPE_UNKNOWN = -1,

            //[DescriptionAttribute("Обычный")]
            [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.SDK.NativeWrapper), "NET_ACCESSCTLCARD_TYPE_GENERAL")]
			NET_ACCESSCTLCARD_TYPE_GENERAL,

            //[DescriptionAttribute("VIP")]
            [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.SDK.NativeWrapper), "NET_ACCESSCTLCARD_TYPE_VIP")]
			NET_ACCESSCTLCARD_TYPE_VIP,

            //[DescriptionAttribute("Гостевой")]
            [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.SDK.NativeWrapper), "NET_ACCESSCTLCARD_TYPE_GUEST")]
			NET_ACCESSCTLCARD_TYPE_GUEST,

            //[DescriptionAttribute("Охрана")]
            [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.SDK.NativeWrapper), "NET_ACCESSCTLCARD_TYPE_PATROL")]
			NET_ACCESSCTLCARD_TYPE_PATROL,

            //[DescriptionAttribute("Черный список")]
            [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.SDK.NativeWrapper), "NET_ACCESSCTLCARD_TYPE_BLACKLIST")]
			NET_ACCESSCTLCARD_TYPE_BLACKLIST,

            //[DescriptionAttribute("Принуждение")]
            [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.SDK.NativeWrapper), "NET_ACCESSCTLCARD_TYPE_CORCE")]
			NET_ACCESSCTLCARD_TYPE_CORCE,

            //[DescriptionAttribute("Материнский")]
            [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.SDK.NativeWrapper), "NET_ACCESSCTLCARD_TYPE_MOTHERCARD")]
			NET_ACCESSCTLCARD_TYPE_MOTHERCARD = 0xff,
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct NET_ACCESSCTLCARD_FINGERPRINT_PACKET
		{
			public uint dwSize;
			public int nLength;
			public int nCount;
			public string pPacketData; // char*
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct NET_ACCESSCTLCARD_FINGERPRINT_PACKET_EX
		{
			public int nLength;
			public int nCount;
			public string pPacketData;
			public int nPacketLen;
			public int nRealPacketLen;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
			public byte[] byReverseed;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct NET_RECORDSET_ACCESS_CTL_CARD
		{
			public Int32 dwSize;
			public Int32 nRecNo;
			public NET_TIME stuCreateTime;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szCardNo;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szUserID;

			public NET_ACCESSCTLCARD_STATE emStatus;
			public NET_ACCESSCTLCARD_TYPE emType;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string szPsw;

			public Int32 nDoorNum;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public Int32[] sznDoors;

			public Int32 nTimeSectionNum;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public Int32[] sznTimeSectionNo;

			public Int32 nUserTime;
			public NET_TIME stuValidStartTime;
			public NET_TIME stuValidEndTime;
			public bool bIsValid;
			public NET_ACCESSCTLCARD_FINGERPRINT_PACKET stuFingerPrintInfo;
			public bool bFirstEnter;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string szCardName;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string szVTOPosition;

			public bool bHandicap;
			public bool bEnableExtended;
			public NET_ACCESSCTLCARD_FINGERPRINT_PACKET_EX stuFingerPrintInfoEx;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern int WRAP_Insert_Card(int loginID, ref NET_RECORDSET_ACCESS_CTL_CARD nativeCard);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Update_Card(int loginID, ref NET_RECORDSET_ACCESS_CTL_CARD nativeCard);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Remove_Card(int loginID, int recordNo);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Card_ClearRepeatEnter(int loginID, string recordNo);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_RemoveAll_Cards(int loginID);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Get_Card_Info(int loginID, int recordNo, IntPtr result);

		[StructLayout(LayoutKind.Sequential)]
		public struct CardsCollection
		{
			public int Count;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
			public NET_RECORDSET_ACCESS_CTL_CARD[] Cards;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_BeginGetAll_Cards(int loginID, ref int finderID);

		[DllImport(@"CPPWrapper.dll")]
		public static extern int WRAP_GetAll_Cards(int finderID, IntPtr result);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_EndGetAll(int finderID);

		[DllImport(@"CPPWrapper.dll")]
		public static extern int WRAP_GetAllCount(int finderID);

		#endregion Cards

		#region Passwords

		[StructLayout(LayoutKind.Sequential)]
		public struct NET_RECORDSET_ACCESS_CTL_PWD
		{
			public int dwSize;
			public int nRecNo;
			public NET_TIME stuCreateTime;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szUserID;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string szDoorOpenPwd;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string szAlarmPwd;

			public int nDoorNum;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public int[] sznDoors;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string szVTOPosition;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern int WRAP_Insert_Password(int loginID, ref NET_RECORDSET_ACCESS_CTL_PWD param);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Update_Password(int loginID, ref NET_RECORDSET_ACCESS_CTL_PWD param);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Remove_Password(int loginID, int recordNo);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_RemoveAll_Passwords(int loginID);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Get_Password_Info(int loginID, int recordNo, IntPtr result);

		[StructLayout(LayoutKind.Sequential)]
		public struct PasswordsCollection
		{
			public int Count;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
			public NET_RECORDSET_ACCESS_CTL_PWD[] Passwords;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_BeginGetAll_Passwords(int loginID, ref int finderID);

		[DllImport(@"CPPWrapper.dll")]
		public static extern int WRAP_GetAll_Passwords(int finderID, IntPtr result);

		#endregion Passwords

		#region Holidays

		[StructLayout(LayoutKind.Sequential)]
		public struct NET_RECORDSET_HOLIDAY
		{
			public int dwSize;
			public int nRecNo;
			public int nDoorNum;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public int[] sznDoors;

			public NET_TIME stuStartTime;
			public NET_TIME stuEndTime;
			public bool bEnable;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szHolidayNo;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern int WRAP_Insert_Holiday(int loginID, ref NET_RECORDSET_HOLIDAY nativeHoliday);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Update_Holiday(int loginID, ref NET_RECORDSET_HOLIDAY nativeHoliday);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Remove_Holiday(int loginID, int recordNo);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_RemoveAll_Holidays(int loginID);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Get_Holiday_Info(int loginID, int recordNo, IntPtr result); // NET_RECORDSET_HOLIDAY

		[DllImport(@"CPPWrapper.dll")]
		public static extern int WRAP_Get_Holidays_Count(int loginID);

		[StructLayout(LayoutKind.Sequential)]
		public struct HolidaysCollection
		{
			public int Count;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
			public NET_RECORDSET_HOLIDAY[] Holidays;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_GetAll_Holidays(int loginID, IntPtr result);

		#endregion Holidays

		#region Acceses

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_BeginGetAll_Accesses(int loginID, ref int finderID);

		[DllImport(@"CPPWrapper.dll")]
		public static extern int WRAP_GetAll_Accesses(int finderID, IntPtr result);

		[StructLayout(LayoutKind.Sequential)]
		public struct NET_RECORDSET_ACCESS_CTL_CARDREC
		{
			public int dwSize;
			public int nRecNo;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szCardNo;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string szPwd;

			public NET_TIME stuTime;
			public bool bStatus;
			public NET_ACCESS_DOOROPEN_METHOD emMethod;
			public int nDoor;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szUserID;

			public int nReaderID;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szSnapFtpUrl;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szReaderID;

			public NET_ACCESSCTLCARD_TYPE emCardType;
			public int nErrorCode;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct AccessesCollection
		{
			public int Count;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
			public NET_RECORDSET_ACCESS_CTL_CARDREC[] Accesses;
		}

		#endregion Acceses

		#region TimeShedules

		[StructLayout(LayoutKind.Sequential)]
		public struct CFG_TIME_SECTION
		{
			public Int32 dwRecordMask;
			public Int32 nBeginHour;
			public Int32 nBeginMin;
			public Int32 nBeginSec;
			public Int32 nEndHour;
			public Int32 nEndMin;
			public Int32 nEndSec;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct CFG_ACCESS_TIMESCHEDULE_INFO
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 7 * 4)]
			public CFG_TIME_SECTION[] stuTime;

			public bool bEnable;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_GetTimeSchedule(int loginID, int index, out CFG_ACCESS_TIMESCHEDULE_INFO result);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_SetTimeSchedule(int loginID, int index, ref CFG_ACCESS_TIMESCHEDULE_INFO param);

		#endregion TimeShedules

		#region Logs

		public struct WRAP_NET_LOG_INFO
		{
			public NET_TIME stuTime;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szUserName;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string szLogType;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
			public string szLogMessage;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct WRAP_Dev_QueryLogList_Result
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
			public WRAP_NET_LOG_INFO[] Logs;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern int WRAP_GetLogCount(int loginID, ref QUERY_DEVICE_LOG_PARAM logParam);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_QueryStart(int loginID);

		[DllImport(@"CPPWrapper.dll")]
		public static extern int WRAP_QueryNext(IntPtr result);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_QueryStop();

		#endregion Logs

		#region Events

		public enum NET_ACCESS_CTL_EVENT_TYPE
		{
            //[DescriptionAttribute("Неизвестно")]
            [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.SDK.NativeWrapper), "NET_ACCESS_CTL_EVENT_UNKNOWN")]
			NET_ACCESS_CTL_EVENT_UNKNOWN = 0,

            //[DescriptionAttribute("Вход")]
            [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.SDK.NativeWrapper), "NET_ACCESS_CTL_EVENT_ENTRY")]
			NET_ACCESS_CTL_EVENT_ENTRY,

            //[DescriptionAttribute("Выход")]
            [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.SDK.NativeWrapper), "NET_ACCESS_CTL_EVENT_EXIT")]
			NET_ACCESS_CTL_EVENT_EXIT
		}

		public enum NET_ACCESS_CTL_STATUS_TYPE
		{
			NET_ACCESS_CTL_STATUS_TYPE_UNKNOWN = 0,
			NET_ACCESS_CTL_STATUS_TYPE_OPEN,
			NET_ACCESS_CTL_STATUS_TYPE_CLOSE
		}

		public enum NET_ACCESS_DOOROPEN_METHOD
		{
            //[DescriptionAttribute("Неизвестно")]
            [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.SDK.NativeWrapper), "NET_ACCESS_DOOROPEN_METHOD_UNKNOWN")]
			NET_ACCESS_DOOROPEN_METHOD_UNKNOWN = 0,

            //[DescriptionAttribute("Пароль")]
            [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.SDK.NativeWrapper), "NET_ACCESS_DOOROPEN_METHOD_PWD_ONLY")]
			NET_ACCESS_DOOROPEN_METHOD_PWD_ONLY,

            //[DescriptionAttribute("Карта")]
            [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.SDK.NativeWrapper), "NET_ACCESS_DOOROPEN_METHOD_CARD")]
			NET_ACCESS_DOOROPEN_METHOD_CARD,

            //[DescriptionAttribute("Сначала карта")]
            [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.SDK.NativeWrapper), "NET_ACCESS_DOOROPEN_METHOD_CARD_FIRST")]
			NET_ACCESS_DOOROPEN_METHOD_CARD_FIRST,

            //[DescriptionAttribute("Сначала пароль")]
            [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.SDK.NativeWrapper), "NET_ACCESS_DOOROPEN_METHOD_PWD_FIRST")]
			NET_ACCESS_DOOROPEN_METHOD_PWD_FIRST,

            //[DescriptionAttribute("Удаленно")]
            [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.SDK.NativeWrapper), "NET_ACCESS_DOOROPEN_METHOD_REMOTE")]
			NET_ACCESS_DOOROPEN_METHOD_REMOTE,

            //[DescriptionAttribute("Кнопка")]
            [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.SDK.NativeWrapper), "NET_ACCESS_DOOROPEN_METHOD_BUTTON")]
			NET_ACCESS_DOOROPEN_METHOD_BUTTON
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct WRAP_JournalItem
		{
			public int LoginID;
			public int ExtraEventType;
			public int EventType;
			public NET_TIME DeviceDateTime;
			public int nDoor;
			public NET_ACCESS_CTL_EVENT_TYPE emEventType;
			public bool bStatus;
			public NET_ACCESSCTLCARD_TYPE emCardType;
			public NET_ACCESS_DOOROPEN_METHOD emOpenMethod;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szCardNo;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string szPwd;

			public int nAction;
			public NET_ACCESS_CTL_STATUS_TYPE emStatus;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern int WRAP_GetLastIndex();

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_GetJournalItem(int index, out WRAP_JournalItem journalItem);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_IsConnected(int loginID);

		#endregion Events

		#region Native

		public delegate void fDisConnectDelegate(Int32 lLoginID, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser);

		public delegate void fHaveReConnectDelegate(Int32 lLoginID, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser);

		public delegate bool fMessCallBackDelegate(Int32 lCommand, Int32 lLoginID, IntPtr pBuf, UInt32 dwBufLen, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser);

		[DllImport(@"dhnetsdk.dll")]
		public static extern Boolean CLIENT_Init(fDisConnectDelegate cbDisConnect, UInt32 dwUser);

		[DllImport(@"dhnetsdk.dll")]
		public static extern void CLIENT_SetAutoReconnect(fHaveReConnectDelegate cbAutoConnect, UInt32 dwUser);

		[DllImport(@"dhnetsdk.dll")]
		public static extern void CLIENT_SetDVRMessCallBack(fMessCallBackDelegate cbMessage, UInt32 dwUser);

		[StructLayout(LayoutKind.Sequential)]
		public struct NET_DEVICEINFO
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
			public Byte[] sSerialNumber;

			public Byte byAlarmInPortNum;
			public Byte byAlarmOutPortNum;
			public Byte byDiskNum;
			public Byte byDVRType;
			public Byte byChanNum;
		}

		[DllImport(@"dhnetsdk.dll")]
		public static extern Int32 CLIENT_Login(String pchDVRIP, UInt16 wDVRPort, String pchUserName, String pchPassword, out NET_DEVICEINFO lpDeviceInfo, out Int32 error);

		[DllImport(@"dhnetsdk.dll")]
		public static extern bool CLIENT_StartListenEx(Int32 lLoginID);

		[DllImport(@"dhnetsdk.dll")]
		public static extern bool CLIENT_StopListen(Int32 lLoginID);

		[DllImport(@"dhnetsdk.dll")]
		public static extern bool CLIENT_Cleanup();

		[StructLayout(LayoutKind.Sequential)]
		public struct ALARM_ACCESS_CTL_EVENT_INFO
		{
			public int dwSize;
			public int nDoor;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string szDoorName;

			public NET_TIME stuTime;
			public NET_ACCESS_CTL_EVENT_TYPE emEventType;
			public bool bStatus;
			public NET_ACCESSCTLCARD_TYPE emCardType;
			public NET_ACCESS_DOOROPEN_METHOD emOpenMethod;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szCardNo;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string szPwd;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szReaderID;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string szUserID;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
			public string szSnapURL;

			public int nErrorCode;
		};

		[StructLayout(LayoutKind.Sequential)]
		public struct ALARM_ACCESS_CTL_NOT_CLOSE_INFO
		{
			public int dwSize;
			public int nDoor;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string szDoorName;

			public NET_TIME stuTime;
			public int nAction;
		};

		[StructLayout(LayoutKind.Sequential)]
		public struct ALARM_ACCESS_CTL_BREAK_IN_INFO
		{
			public int dwSize;
			public int nDoor;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string szDoorName;

			public NET_TIME stuTime;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ALARM_ACCESS_CTL_REPEAT_ENTER_INFO
		{
			public int dwSize;
			public int nDoor;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string szDoorName;

			public NET_TIME stuTime;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szCardNo;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ALARM_ACCESS_CTL_DURESS_INFO
		{
			public int dwSize;
			public int nDoor;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string szDoorName;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szCardNo;

			public NET_TIME stuTime;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ALARM_ACCESS_CTL_STATUS_INFO
		{
			public int dwSize;
			public int nDoor;
			public NET_TIME stuTime;
			public NET_ACCESS_CTL_STATUS_TYPE emStatus;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ALARM_CHASSISINTRUDED_INFO
		{
			public int dwSize;
			public int nAction;
			public NET_TIME stuTime;
			public int nChannelID;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szReaderID;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ALARM_OPEN_DOOR_GROUP_INFO
		{
			public int dwSize;
			public int nChannelID;
			public NET_TIME stuTime;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ALARM_CAPTURE_FINGER_PRINT_INFO
		{
			public int dwSize;
			public int nChannelID;
			public NET_TIME stuTime;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szReaderID;

			public int nPacketLen;
			public int nPacketNum;
			public IntPtr szFingerPrintInfo;
		}

		public enum NET_SENSE_METHOD
		{
			NET_SENSE_UNKNOWN = -1,		//Unknowed type
			NET_SENSE_DOOR = 0,			//Door Contact
			NET_SENSE_PASSIVEINFRA,		//Passive Infrared
			NET_SENSE_GAS,				//Gase Induce)
			NET_SENSE_SMOKING,			//Smoking Induce
			NET_SENSE_WATER,			//Wwater Induce)
			NET_SENSE_ACTIVEFRA,		//Initiative Infrared
			NET_SENSE_GLASS,			//Glass Broken
			NET_SENSE_EMERGENCYSWITCH,	//Emergency switch
			NET_SENSE_SHOCK,			//Shock
			NET_SENSE_DOUBLEMETHOD,		//Double Method(Infrare+Microwave)
			NET_SENSE_THREEMETHOD,		//Three Method
			NET_SENSE_TEMP,				//Temperature
			NET_SENSE_HUMIDITY,			//Humidity
			NET_SENSE_WIND,             //Wind
			NET_SENSE_CALLBUTTON,		//Call button
			NET_SENSE_GASPRESSURE,      //Gas Pressure
			NET_SENSE_GASCONCENTRATION, //Gas Concentration
			NET_SENSE_GASFLOW,          //Gas Flow
			NET_SENSE_OTHER,			//Other
			NET_SENSE_OIL,              //oil detectionЎк?gasoline, diesel vehicles detection
			NET_SENSE_MILEAGE,          //mileage detection
			NET_SENSE_URGENCYBUTTON,    //Urgency button
			NET_SENSE_STEAL,            //Steal
			NET_SENSE_PERIMETER,        //Permeter
			NET_SENSE_PREVENTREMOVE,    //Prevent remove
			NET_SENSE_DOORBELL,         //Door bell
			NET_SENSE_ALTERVOLT,        //Alter voltage sensor
			NET_SENSE_DIRECTVOLT,       //Direct voltage sensor
			NET_SENSE_ALTERCUR,         //Alter current sensor
			NET_SENSE_DIRECTCUR,        //Direct current sensor
			NET_SENSE_RSUGENERAL,       //RSU general analog sensor, 4~20mA or 0~5V
			NET_SENSE_RSUDOOR,          //RSU door sensor
			NET_SENSE_RSUPOWEROFF,      //RSU power off sensor
			NET_SENSE_TEMP1500,        //1500 temperature sensor
			NET_SENSE_TEMPDS18B20,     //DS18B20 temperature sensor
			NET_SENSE_HUMIDITY1500,     //1500 humidity sensor
			NET_SENSE_NUM,				//Number of enumeration type
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ALARM_ALARM_INFO_EX2
		{
			public int dwSize;
			public int nChannelID;
			public int nAction;
			public NET_TIME stuTime;
			public NET_SENSE_METHOD emSenseType;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ALARM_ACCESS_LOCK_STATUS_INFO
		{
			public uint dwSize;
			public int nChannel;
			public NET_TIME stuTime;
			public NET_ACCESS_CTL_STATUS_TYPE emLockStatus; // typedef NET_ACCESS_CTL_STATUS_TYPE  NET_ACCESS_LOCK_STATUS_TYPE
		}

		#endregion Native

		#region <Anti-path Back>

		public enum WRAP_AntiPassBackMode
		{
			R1R2 = 0,
			R1R3R2R4 = 1,
			R3R4 = 2
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct WRAP_AntiPassBackModeAvailability
		{
			public WRAP_AntiPassBackMode AntiPassBackMode; // Режим Anti-pass Back
			public bool bIsAvailable; // Доступность режима Anti-pass Back
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct WRAP_AntiPassBackCfg
		{
			public int nDoorsCount; // Количество дверей на контроллере
			public bool bCanActivate; // Возможность активации Anti-pass Back
			public bool bIsActivated; // Anti-pass Back активирован?

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
			public WRAP_AntiPassBackModeAvailability[] AvailableAntiPassBackModes; // Доступность режимов Anti-pass Back

			public WRAP_AntiPassBackMode CurrentAntiPassBackMode; // Текущий режим Anti-pass Back
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_GetAntiPassBackCfg(int loginID, out WRAP_AntiPassBackCfg result);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_SetAntiPassBackCfg(int loginID, ref WRAP_AntiPassBackCfg cfg);

		#endregion <Anti-path Back>

		#region <Interlock>

		public enum WRAP_InterlockMode
		{
			L1L2 = 0,
			L1L2L3 = 1,
			L1L2L3L4 = 2,
			L2L3L4 = 3,
			L1L3_L2L4 = 4,
			L1L4_L2L3 = 5,
			L3L4 = 6
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct WRAP_InterlockModeAvailability
		{
			public WRAP_InterlockMode InterlockMode; // Режим Interlock
			public bool bIsAvailable; // Доступность режима Interlock
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct WRAP_InterlockCfg
		{
			public int nDoorsCount; // Количество дверей на контроллере
			public bool bCanActivate; // Возможность активации Interlock
			public bool bIsActivated; // Interlock активирован?

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
			public WRAP_InterlockModeAvailability[] AvailableInterlockModes; // Доступность режимов Interlock

			public WRAP_InterlockMode CurrentInterlockMode; // Текущий режим Interlock
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_GetInterlockCfg(int loginID, out WRAP_InterlockCfg result);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_SetInterlockCfg(int loginID, ref WRAP_InterlockCfg cfg);

		#endregion <Interlock>

		#region <Custom Data>

		[StructLayout(LayoutKind.Sequential)]
		public struct WRAP_CustomData
		{
			/// <summary>
			/// Фактическая длина пользовательских данных
			/// </summary>
			public int CustomDataLength;

			/// <summary>
			/// Пользовательские данные
			/// </summary>
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string CustomData;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_GetCustomData(int loginID, out WRAP_CustomData customData);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_SetCustomData(int loginID, ref WRAP_CustomData customData);

		#endregion <Custom Data>

		#region <Поиск устройств>

		public struct DEVICE_NET_INFO_EX
		{
			public int iIPVersion;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string szIP;

			public int nPort;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string szSubmask;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string szGateway;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
			public string szMac;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szDeviceType;

			public byte byManuFactory;
			public byte byDefinition;
			public byte bDhcpEn;
			public byte byReserved1;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 88)]
			public string verifyData;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 48)]
			public string szSerialNo;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string szDevSoftVersion;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szDetailType;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string szVendor;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string szDevName;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
			public string szUserName;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
			public string szPassWord;

			public ushort nHttpPort;
			public byte wVideoInputCh;
			public byte wRemoteVideoInputCh;
			public byte wVideoOutputCh;
			public byte wAlarmInputCh;
			public byte wAlarmOutputCh;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 244)]
			public string cReserved;
		}

		public delegate void fSearchDevicesCBDelegate(ref DEVICE_NET_INFO_EX pDevNetInfo, IntPtr pUserData);

		[DllImport(@"dhnetsdk.dll")]
		public static extern Int32 CLIENT_StartSearchDevices(fSearchDevicesCBDelegate cbSearchDevices, IntPtr pUserData, string szLocalIp = null);

		[DllImport(@"dhnetsdk.dll")]
		public static extern bool CLIENT_StopSearchDevices(Int32 lSearchHandle);

		#endregion <Поиск устройств>
	}
}