using System;
using System.Runtime.InteropServices;

namespace ChinaSKDDriverNativeApi
{
	public class NativeWrapper
	{
		#region Common
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void fDisConnectDelegate(Int32 lLoginID, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void fHaveReConnect(Int32 lLoginID, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate bool fMessCallBack(Int32 lCommand, Int32 lLoginID, IntPtr pBuf, Int32 dwBufLen, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser);

		[DllImport(@"dhnetsdk.dll")]
		public static extern Boolean CLIENT_Init(fDisConnectDelegate cbDisConnect, UInt32 dwUser);

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
		public static extern bool CLIENT_Cleanup();

		[DllImport(@"dhnetsdk.dll")]
		public static extern void CLIENT_SetAutoReconnect(fHaveReConnect cbAutoConnect, UInt32 dwUser);

		[DllImport(@"dhnetsdk.dll")]
		public static extern void CLIENT_SetDVRMessCallBack(fMessCallBack cbMessage, UInt32 dwUser);

		[DllImport(@"dhnetsdk.dll")]
		public static extern bool CLIENT_StartListenEx(Int32 lLoginID);

		public enum NET_ACCESS_CTL_EVENT_TYPE
		{
			NET_ACCESS_CTL_EVENT_UNKNOWN = 0,
			NET_ACCESS_CTL_EVENT_ENTRY,
			NET_ACCESS_CTL_EVENT_EXIT,
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ALARM_ACCESS_CTL_EVENT_INFO
		{
			public int dwSize;
			public int nDoor;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			public char[] szDoorName;
			public NET_TIME stuTime;
			public NET_ACCESS_CTL_EVENT_TYPE emEventType;
			public bool bStatus;
			public NET_ACCESSCTLCARD_TYPE emCardType;
			public NET_ACCESS_DOOROPEN_METHOD emOpenMethod;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public char[] szCardNo;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
			public char[] szPwd;
		}

		[DllImport(@"dhnetsdk.dll")]
		public static extern bool CLIENT_QueryDevState(Int32 lLoginID, Int32 nType, IntPtr pBuf, Int32 nBufLen, out Int32 pRetLen, Int32 waittime);

		[DllImport(@"dhnetsdk.dll")]
		public static extern bool CLIENT_GetNewDevConfig(Int32 lLoginID, string szCommand, Int32 nChannelID, char[] szOutBuffer, UInt32 dwOutBufferSize, out Int32 error, Int32 waittime);

		[StructLayout(LayoutKind.Sequential)]
		public struct CFG_CAP_RECORDFINDER_INFO
		{
			Int32 nMaxPageSize;
		}

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

		[DllImport(@"dhnetsdk.dll")]
		public static extern bool CLIENT_QueryDeviceTime(Int32 lLoginID, IntPtr pDeviceTime, Int32 waittime);

		[DllImport(@"dhnetsdk.dll")]
		public static extern bool CLIENT_SetupDeviceTime(Int32 lLoginID, IntPtr pDeviceTime);

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
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public char[] szDevType;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			public char[] szSoftWareVersion;
			public Int32 dwSoftwareBuildDate_1;
			public Int32 dwSoftwareBuildDate_2;
			public Int32 dwSoftwareBuildDate_3;
		}

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_DevConfig_TypeAndSoftInfo(Int32 lLoginID, out WRAP_DevConfig_TypeAndSoftInfo_Result result);

		[StructLayout(LayoutKind.Sequential)]
		public struct WRAP_CFG_NETWORK_INFO_Result
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
			public char[] szIP;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
			public char[] szSubnetMask;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
			public char[] szDefGateway;
			public Int32 nMTU;
		}

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_Get_DevConfig_IPMaskGate(int lLoginID, out WRAP_CFG_NETWORK_INFO_Result stuNetwork);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_Set_DevConfig_IPMaskGate(int lLoginID, string ip, string mask, string gate, int mtu);

		[StructLayout(LayoutKind.Sequential)]
		public struct WRAP_DevConfig_MAC_Result
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
			public char[] szMAC;
		}

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_DevConfig_MAC(int lLoginID, out WRAP_DevConfig_MAC_Result result);

		[StructLayout(LayoutKind.Sequential)]
		public struct WRAP_DevConfig_RecordFinderCaps_Result
		{
			public int nMaxPageSize;
		}

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_DevConfig_RecordFinderCaps(int lLoginID, out WRAP_DevConfig_RecordFinderCaps_Result result);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_DevConfig_GetCurrentTime(int lLoginID, out NET_TIME result);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_DevConfig_SetCurrentTime(int lLoginID, int dwYear, int dwMonth, int dwDay, int dwHour, int dwMinute, int dwSecond);

		[StructLayout(LayoutKind.Sequential)]
		public struct WRAP_LogItem
		{
			public int nLogType;
			public DHDEVTIME stuOperateTime;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			public char[] szOperator;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public char[] szOperation;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4 * 1024)]
			public char[] szDetailContext;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct WRAP_Dev_QueryLogList_Result
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
			public WRAP_LogItem[] Logs;
			public int Test;
		}

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_Dev_QueryLogList(int lLoginID, out WRAP_Dev_QueryLogList_Result result);

		[StructLayout(LayoutKind.Sequential)]
		public struct WRAP_GeneralConfig_Password
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
			public char[] szProjectPassword;
		}

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_GetProjectPassword(int lLoginId, out WRAP_GeneralConfig_Password result);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_SetProjectPassword(int lLoginId, string password);

		[StructLayout(LayoutKind.Sequential)]
		public struct CFG_ACCESS_EVENT_INFO
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			public char[] szChannelName;
			public CFG_ACCESS_STATE emState;
			public CFG_ACCESS_MODE emMode;
			public int nEnableMode;
			public bool bSnapshotEnable;
			public bool abDoorOpenMethod;
			public bool abUnlockHoldInterval;
			public bool abCloseTimeout;
			public bool abOpenAlwaysTimeIndex;
			public bool abHolidayTimeIndex;
			public bool abBreakInAlarmEnable;
			public bool abRepeatEnterAlarmEnable;
			public bool abDoorNotClosedAlarmEnable;
			public bool abDuressAlarmEnable;
			public bool abDoorTimeSection;
			public bool abSensorEnable;
			public byte byReserved;
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
		}

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_GetDoorConfiguration(int lLoginId, int channelNo, out CFG_ACCESS_EVENT_INFO result);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_SetDoorConfiguration(int lLoginId, int channelNo, ref CFG_ACCESS_EVENT_INFO accessEventInfo);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_DevCtrl_ReBoot(int lLoginID);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_DevCtrl_DeleteCfgFile(int lLoginID);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern int WRAP_DevCtrl_GetLogCount(int lLoginID, ref QUERY_DEVICE_LOG_PARAM logParam);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_OpenDoor(int lLoginID, int channelNo);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_CloseDoor(int lLoginId, int channelNo);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern int WRAP_GetDoorStatus(int lLoginId, int channelNo);
		#endregion

		#region Cards
		public enum NET_ACCESSCTLCARD_STATE
		{
			NET_ACCESSCTLCARD_STATE_UNKNOWN = -1,
			NET_ACCESSCTLCARD_STATE_NORMAL = 0,
			NET_ACCESSCTLCARD_STATE_LOSE = 0x01,
			NET_ACCESSCTLCARD_STATE_LOGOFF = 0x02,
			NET_ACCESSCTLCARD_STATE_FREEZE = 0x04,
		}

		public enum NET_ACCESSCTLCARD_TYPE
		{
			NET_ACCESSCTLCARD_TYPE_UNKNOWN = -1,
			NET_ACCESSCTLCARD_TYPE_GENERAL,
			NET_ACCESSCTLCARD_TYPE_VIP,
			NET_ACCESSCTLCARD_TYPE_GUEST,
			NET_ACCESSCTLCARD_TYPE_PATROL,
			NET_ACCESSCTLCARD_TYPE_BLACKLIST,
			NET_ACCESSCTLCARD_TYPE_CORCE,
			NET_ACCESSCTLCARD_TYPE_MOTHERCARD = 0xff,
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct NET_RECORDSET_ACCESS_CTL_CARD
		{
			public Int32 dwSize;
			public Int32 nRecNo;
			public NET_TIME stuCreateTime;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public Char[] szCardNo;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public Char[] szUserID;
			public NET_ACCESSCTLCARD_STATE emStatus;
			public NET_ACCESSCTLCARD_TYPE emType;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
			public Char[] szPsw;
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
		}

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern int WRAP_Insert_Card(int lLoginID, ref NET_RECORDSET_ACCESS_CTL_CARD stuCard);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_Update_Card(int lLoginID, ref NET_RECORDSET_ACCESS_CTL_CARD stuCard);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_RemoveCard(int lLoginID, int nRecordNo);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_RemoveAllCards(int lLoginID);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_GetCardInfo(int lLoginID, int recordNo, IntPtr result); // NET_RECORDSET_ACCESS_CTL_CARD

		[StructLayout(LayoutKind.Sequential)]
		public struct FIND_RECORD_ACCESSCTLCARD_CONDITION
		{
			public int dwSize;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public char[] szCardNo;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public char[] szUserID;
			public bool bIsValid;
		}

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern int WRAP_Get_CardsCount(int lLoginID, ref FIND_RECORD_ACCESSCTLCARD_CONDITION stuParam);

		[StructLayout(LayoutKind.Sequential)]
		public struct CardsCollection
		{
			public int Count;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1000)]
			public NET_RECORDSET_ACCESS_CTL_CARD[] Cards;
		}

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_GetAllCards(int lLoginId, IntPtr result);
		#endregion

		#region CardRecs
		public enum NET_ACCESS_DOOROPEN_METHOD
		{
			NET_ACCESS_DOOROPEN_METHOD_UNKNOWN = 0,
			NET_ACCESS_DOOROPEN_METHOD_PWD_ONLY,
			NET_ACCESS_DOOROPEN_METHOD_CARD,
			NET_ACCESS_DOOROPEN_METHOD_CARD_FIRST,
			NET_ACCESS_DOOROPEN_METHOD_PWD_FIRST,
			NET_ACCESS_DOOROPEN_METHOD_REMOTE,
			NET_ACCESS_DOOROPEN_METHOD_BUTTON,
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct NET_RECORDSET_ACCESS_CTL_CARDREC
		{
			public int dwSize;
			public int nRecNo;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public char[] szCardNo;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
			public char[] szPwd;
			public NET_TIME stuTime;
			public bool bStatus;
			public NET_ACCESS_DOOROPEN_METHOD emMethod;
			public int nDoor;
		}

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern int WRAP_Insert_CardRec(int lLoginID, ref NET_RECORDSET_ACCESS_CTL_CARDREC stuCardRec);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_Update_CardRec(int lLoginID, ref NET_RECORDSET_ACCESS_CTL_CARDREC stuCardRec);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_RemoveCardRec(int lLoginID, int nRecordNo);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_RemoveAllCardRecs(int lLoginID);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_GetCardRecInfo(int lLoginID, int recordNo, IntPtr result); // NET_RECORDSET_ACCESS_CTL_CARDREC

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern int WRAP_Get_CardRecordsCount(int lLoginID);

		[StructLayout(LayoutKind.Sequential)]
		public struct CardRecsCollection
		{
			public int Count;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1000)]
			public NET_RECORDSET_ACCESS_CTL_CARDREC[] CardRecs;
		}

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_GetAllCardRecs(int lLoginId, IntPtr result);

		[StructLayout(LayoutKind.Sequential)]
		public struct CardRecordsCollection
		{
			int Count;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1000)]
			NET_RECORDSET_ACCESS_CTL_CARDREC[] CardRecords;
		};

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_GetAllCardRecords(int lLoginId, IntPtr result);
		#endregion

		#region Passwords
		[StructLayout(LayoutKind.Sequential)]
		public struct NET_RECORDSET_ACCESS_CTL_PWD
		{
			public int dwSize;
			public int nRecNo;
			public NET_TIME stuCreateTime;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public char[] szUserID;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
			public char[] szDoorOpenPwd;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
			public char[] szAlarmPwd;
			public int nDoorNum;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public int[] sznDoors;
		}

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern int WRAP_Insert_Pwd(int lLoginID, ref NET_RECORDSET_ACCESS_CTL_PWD stuAccessCtlPwd);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_Update_Pwd(int lLoginID, ref NET_RECORDSET_ACCESS_CTL_PWD stuAccessCtlPwd);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_RemovePassword(int lLoginID, int nRecordNo);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_RemoveAllPasswords(int lLoginID);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_GetPasswordInfo(int lLoginID, int recordNo, IntPtr result); // NET_RECORDSET_ACCESS_CTL_PWD

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern int WRAP_Get_PasswordsCount(int lLoginID);

		[StructLayout(LayoutKind.Sequential)]
		public struct PasswordsCollection
		{
			public int Count;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1000)]
			public NET_RECORDSET_ACCESS_CTL_PWD[] Passwords;
		}

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_GetAllPasswords(int lLoginId, IntPtr result);
		#endregion

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
		}

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern int WRAP_Insert_Holiday(int lLoginID, ref NET_RECORDSET_HOLIDAY stuHoliday);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_Update_Holiday(int lLoginID, ref NET_RECORDSET_HOLIDAY stuHoliday);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_RemoveHoliday(int lLoginID, int nRecordNo);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_RemoveAllHolidays(int lLoginID);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_GetHolidayInfo(int lLoginID, int recordNo, IntPtr result); // NET_RECORDSET_HOLIDAY

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern int WRAP_Get_HolidaysCount(int lLoginID);
		#endregion

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

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_GetTimeSchedule(int lLoginId, int index, out CFG_ACCESS_TIMESCHEDULE_INFO result);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_SetTimeSchedule(int lLoginId, int index, ref CFG_ACCESS_TIMESCHEDULE_INFO timeSheduleInfo);
		#endregion

		#region Events
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void WRAP_ProgressCallback(Int32 index);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_StartProgress(WRAP_ProgressCallback progressCallback);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_StartListen(int lLoginId);

		[StructLayout(LayoutKind.Sequential)]
		public struct NET_GPS_STATUS_INFO
		{
			NET_TIME revTime;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
			char[] DvrSerial;
			double longitude;
			double latidude;
			double height;
			double angle;
			double speed;
			int starCount;
			bool antennaState;
			bool orientationState;
			bool workStae;
			int nAlarmCount;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			int[] nAlarmState;
			byte bOffline;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			byte[] byRserved;
		}

		public enum EM_TALKING_CALLER
		{
			EM_TALKING_CALLER_UNKNOWN = 0,
			EM_TALKING_CALLER_PLATFORM,
		}

		public enum NET_SENSE_METHOD
		{
			NET_SENSE_UNKNOWN = -1,
			NET_SENSE_DOOR = 0,
			NET_SENSE_PASSIVEINFRA,
			NET_SENSE_GAS,
			NET_SENSE_SMOKING,
			NET_SENSE_WATER,
			NET_SENSE_ACTIVEFRA,
			NET_SENSE_GLASS,
			NET_SENSE_EMERGENCYSWITCH,
			NET_SENSE_SHOCK,
			NET_SENSE_DOUBLEMETHOD,
			NET_SENSE_THREEMETHOD,
			NET_SENSE_TEMP,
			NET_SENSE_HUMIDITY,
			NET_SENSE_CALLBUTTON,
			NET_SENSE_OTHER,
			NET_SENSE_NUM,
		}

		public enum EM_POWER_TYPE
		{
			EM_POWER_TYPE_MAIN = 0,
			EM_POWER_TYPE_BACKUP,
		}

		public enum EM_POWERFAULT_EVENT_TYPE
		{
			EM_POWERFAULT_EVENT_LOST = 0
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct WRAP_JournalItem
		{
			public int EventType;
			public NET_TIME DeviceDateTime;
			public NET_GPS_STATUS_INFO stGPSStatusInfo;
			public byte bEventAction;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			public char[] szInfo;
			public EM_TALKING_CALLER emCaller;
			public int nChannelID;
			public int nAction;
			public NET_SENSE_METHOD emSenseType;
			public int nBatteryLeft;
			public float fTemperature;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
			public char[] szSensorName;
			public EM_POWER_TYPE emPowerType;
			public EM_POWERFAULT_EVENT_TYPE emPowerFaultEvent;
			public int nDoor;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			public char[] szDoorName;
			public NET_ACCESS_CTL_EVENT_TYPE emEventType;
			public bool bStatus;
			public NET_ACCESSCTLCARD_TYPE emCardType;
			public NET_ACCESS_DOOROPEN_METHOD emOpenMethod;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public char[] szCardNo;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
			public char[] szPwd;
		}

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern int WRAP_GetLastIndex();

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_GetJournalItem(int index, out WRAP_JournalItem journalItem);
		#endregion
	}
}