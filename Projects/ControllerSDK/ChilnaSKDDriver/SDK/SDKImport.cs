using System;
using System.Runtime.InteropServices;

namespace ControllerSDK.SDK
{
	public class SDKImport
	{
		//[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void fDisConnectDelegate(Int32 lLoginID, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser);

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


		[StructLayout(LayoutKind.Sequential)]
		public struct DHDEV_VERSION_INFO
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
			Char[] szDevSerialNo;
			Char byDevType;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			Char[] szDevType;
			int nProtocalVer;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			Char[] szSoftWareVersion;
			UInt16 dwSoftwareBuildDate;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			Char[] szPeripheralSoftwareVersion;
			UInt16 dwPeripheralSoftwareBuildDate;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			Char[] szGeographySoftwareVersion;
			UInt16 dwGeographySoftwareBuildDate;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			Char[] szHardwareVersion;
			UInt16 dwHardwareDate;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			Char[] szWebVersion;
			UInt16 dwWebBuildDate;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
			Char[] reserved;
		}

		[DllImport(@"dhnetsdk.dll")]
		public static extern bool CLIENT_QueryDevState(Int32 lLoginID, Int32 nType, IntPtr pBuf, Int32 nBufLen, out Int32 pRetLen, Int32 waittime);

		[DllImport(@"dhnetsdk.dll")]
		public static extern bool CLIENT_GetNewDevConfig(Int32 lLoginID, string szCommand, Int32 nChannelID, char[] szOutBuffer, UInt32 dwOutBufferSize, out Int32 error, Int32 waittime);

		[StructLayout(LayoutKind.Sequential)]
		public struct CFG_NETWORK_INTERFACE
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			Char[] szName;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			Char[] szIP;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			Char[] szSubnetMask;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			Char[] szDefGateway;
			bool bDhcpEnable;
			bool bDnsAutoGet;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2 * 256)]
			Char[] szDnsServers;
			Int32 nMTU;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct CFG_NETWORK_INFO
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			Char[] szHostName;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			Char[] szDomain;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			Char[] szDefInterface;
			int nInterfaceNum;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			CFG_NETWORK_INTERFACE[] stuInterfaces;
		}

		[DllImport(@"dhconfigsdk.dll")]
		public static extern bool CLIENT_ParseData(string szCommand, char[] szInBuffer, IntPtr lpOutBuffer, Int32 dwOutBufferSize, IntPtr pReserved);

		[DllImport(@"dhconfigsdk.dll")]
		public static extern bool CLIENT_PacketData(string szCommand, IntPtr lpInBuffer, Int32 dwInBufferSize, char[] szOutBuffer, Int32 dwOutBufferSize);

		[DllImport(@"dhnetsdk.dll")]
		public static extern bool CLIENT_SetNewDevConfig(Int32 lLoginID, string szCommand, Int32 nChannelID, char[] szInBuffer, Int32 dwInBufferSize, out Int32 error, out Int32 restart, Int32 waittime);

		[StructLayout(LayoutKind.Sequential)]
		public struct DHDEV_NETINTERFACE_INFO
		{
			Int32 dwSize;
			bool bValid;
			bool bVirtual;
			Int32 nSpeed;
			Int32 nDHCPState;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
			Char[] szName;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
			Char[] szType;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
			Char[] szMAC;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
			Char[] szSSID;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
			Char[] szConnStatus;
			Int32 nSupportedModeNum;
			//[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64 * 64)]
			//Char[][] szSupportedModes;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64 * 64)]
			Char[] szSupportedModes;
		}

		[DllImport(@"dhnetsdk.dll")]
		public static extern bool CLIENT_QueryDevState(Int32 lLoginID, Int32 nType, char[] pBuf, Int32 nBufLen, out Int32 pRetLen, Int32 waittime);

		[DllImport(@"dhnetsdk.dll")]
		public static extern bool CLIENT_QueryNewSystemInfo(Int32 lLoginID, String szCommand, Int32 nChannelID, char[] szOutBuffer, Int32 dwOutBufferSize, out Int32 error, Int32 waittime);

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

		[StructLayout(LayoutKind.Sequential)]
		public struct DH_LOG_ITEM
		{
			DHDEVTIME time;
			UInt16 type;
			char reserved;
			char data;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			char[] context;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct stuOldLog
		{
			DH_LOG_ITEM stuLog;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
			Byte[] bReserved;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct MyUnion
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
			Char[] szLogContext;
			stuOldLog stuOldLog;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct DH_DEVICE_LOG_ITEM_EX
		{
			Int32 nLogType;
			DHDEVTIME stuOperateTime;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			Char[] szOperator;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
			Byte[] bReserved;
			Byte bUnionType;
			MyUnion myUnion;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			Char[] szOperation;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4 * 1024)]
			Char[] szDetailContext;
		}

		[DllImport(@"dhnetsdk.dll")]
		public static extern bool CLIENT_QueryDeviceLog(Int32 lLoginID, IntPtr pQueryParam, IntPtr pLogBuffer, Int32 nLogBufferLen, out Int32 pRecLogNum, Int32 waittime);

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

		//[StructLayout(LayoutKind.Sequential)]
		//public struct CFG_ACCESS_EVENT_INFO
		//{
		//    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		//    Char[] szChannelName;
		//    CFG_ACCESS_STATE emState;
		//    CFG_ACCESS_MODE emMode;
		//    Int32 nEnableMode;
		//    bool bSnapshotEnable;
		//    bool abDoorOpenMethod;
		//    bool abUnlockHoldInterval;
		//    bool abCloseTimeout;
		//    bool abOpenAlwaysTimeIndex;
		//    bool abHolidayTimeIndex;
		//    bool abBreakInAlarmEnable;
		//    bool abRepeatEnterAlarmEnable;
		//    bool abDoorNotClosedAlarmEnable;
		//    bool abDuressAlarmEnable;
		//    bool abDoorTimeSection;
		//    bool abSensorEnable;
		//    Byte byReserved;
		//    CFG_DOOR_OPEN_METHOD emDoorOpenMethod;
		//    Int32 nUnlockHoldInterval;
		//    Int32 nCloseTimeout;
		//    Int32 nOpenAlwaysTimeIndex;
		//    Int32 nHolidayTimeRecoNo;
		//    bool bBreakInAlarmEnable;
		//    bool bRepeatEnterAlarm;
		//    bool bDoorNotClosedAlarmEnable;
		//    bool bDuressAlarmEnable;
		//    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7 * 4)]
		//    CFG_DOOROPEN_TIMESECTION_INFO[][] stuDoorTimeSection;
		//    bool bSensorEnable;
		//}		

		/////////////////////////////////////////////////////////////////////
		//////////////////////    ControllerSDK methods  ////////////////////
		/////////////////////////////////////////////////////////////////////

		public enum EM_NET_RECORD_TYPE
		{
			NET_RECORD_UNKNOWN,
			NET_RECORD_TRAFFICREDLIST,
			NET_RECORD_TRAFFICBLACKLIST,
			NET_RECORD_BURN_CASE,
			NET_RECORD_ACCESSCTLCARD,
			NET_RECORD_ACCESSCTLPWD,
			NET_RECORD_ACCESSCTLCARDREC,
			NET_RECORD_ACCESSCTLHOLIDAY,
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct NET_CTRL_RECORDSET_INSERT_IN
		{
			public Int32 dwSize;
			public EM_NET_RECORD_TYPE emType;
			public IntPtr pBuf;
			public Int32 nBufLen;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct NET_CTRL_RECORDSET_INSERT_OUT
		{
			public Int32 dwSize;
			public Int32 nRecNo;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct NET_CTRL_RECORDSET_INSERT_PARAM
		{
			public Int32 dwSize;
			public NET_CTRL_RECORDSET_INSERT_IN stuCtrlRecordSetInfo;
			public NET_CTRL_RECORDSET_INSERT_OUT stuCtrlRecordSetResult;
		}

		public enum CtrlType
		{
			DH_CTRL_REBOOT = 0,
			DH_CTRL_SHUTDOWN,
			DH_CTRL_DISK,
			DH_KEYBOARD_POWER = 3,
			DH_KEYBOARD_ENTER,
			DH_KEYBOARD_ESC,
			DH_KEYBOARD_UP,
			DH_KEYBOARD_DOWN,
			DH_KEYBOARD_LEFT,
			DH_KEYBOARD_RIGHT,
			DH_KEYBOARD_BTN0,
			DH_KEYBOARD_BTN1,
			DH_KEYBOARD_BTN2,
			DH_KEYBOARD_BTN3,
			DH_KEYBOARD_BTN4,
			DH_KEYBOARD_BTN5,
			DH_KEYBOARD_BTN6,
			DH_KEYBOARD_BTN7,
			DH_KEYBOARD_BTN8,
			DH_KEYBOARD_BTN9,
			DH_KEYBOARD_BTN10,
			DH_KEYBOARD_BTN11,
			DH_KEYBOARD_BTN12,
			DH_KEYBOARD_BTN13,
			DH_KEYBOARD_BTN14,
			DH_KEYBOARD_BTN15,
			DH_KEYBOARD_BTN16,
			DH_KEYBOARD_SPLIT,
			DH_KEYBOARD_ONE,
			DH_KEYBOARD_NINE,
			DH_KEYBOARD_ADDR,
			DH_KEYBOARD_INFO,
			DH_KEYBOARD_REC,
			DH_KEYBOARD_FN1,
			DH_KEYBOARD_FN2,
			DH_KEYBOARD_PLAY,
			DH_KEYBOARD_STOP,
			DH_KEYBOARD_SLOW,
			DH_KEYBOARD_FAST,
			DH_KEYBOARD_PREW,
			DH_KEYBOARD_NEXT,
			DH_KEYBOARD_JMPDOWN,
			DH_KEYBOARD_JMPUP,
			DH_TRIGGER_ALARM_IN = 100,
			DH_TRIGGER_ALARM_OUT,
			DH_CTRL_MATRIX,
			DH_CTRL_SDCARD,
			DH_BURNING_START,
			DH_BURNING_STOP,
			DH_BURNING_ADDPWD,
			DH_BURNING_ADDHEAD,
			DH_BURNING_ADDSIGN,
			DH_BURNING_ADDCURSTOMINFO,
			DH_CTRL_RESTOREDEFAULT,
			DH_CTRL_CAPTURE_START,
			DH_CTRL_CLEARLOG,
			DH_TRIGGER_ALARM_WIRELESS = 200,
			DH_MARK_IMPORTANT_RECORD,
			DH_CTRL_DISK_SUBAREA,
			DH_BURNING_ATTACH,
			DH_BURNING_PAUSE,
			DH_BURNING_CONTINUE,
			DH_BURNING_POSTPONE,
			DH_CTRL_OEMCTRL,
			DH_BACKUP_START,
			DH_BACKUP_STOP,
			DH_VIHICLE_WIFI_ADD,
			DH_VIHICLE_WIFI_DEC,
			DH_BUZZER_START,
			DH_BUZZER_STOP,
			DH_REJECT_USER,
			DH_SHIELD_USER,
			DH_RAINBRUSH,
			DH_MANUAL_SNAP,
			DH_MANUAL_NTP_TIMEADJUST,
			DH_NAVIGATION_SMS,
			DH_CTRL_ROUTE_CROSSING,
			DH_BACKUP_FORMAT,
			DH_DEVICE_LOCALPREVIEW_SLIPT,
			DH_CTRL_INIT_RAID,
			DH_CTRL_RAID,
			DH_CTRL_SAPREDISK,
			DH_WIFI_CONNECT,
			DH_WIFI_DISCONNECT,
			DH_CTRL_ARMED,
			DH_CTRL_IP_MODIFY,
			DH_CTRL_WIFI_BY_WPS,
			DH_CTRL_FORMAT_PATITION,
			DH_CTRL_EJECT_STORAGE,
			DH_CTRL_LOAD_STORAGE,
			DH_CTRL_CLOSE_BURNER,
			DH_CTRL_EJECT_BURNER,
			DH_CTRL_CLEAR_ALARM,
			DH_CTRL_MONITORWALL_TVINFO,
			DH_CTRL_START_VIDEO_ANALYSE,
			DH_CTRL_STOP_VIDEO_ANALYSE,
			DH_CTRL_UPGRADE_DEVICE,
			DH_CTRL_MULTIPLAYBACK_CHANNALES,
			DH_CTRL_SEQPOWER_OPEN,
			DH_CTRL_SEQPOWER_CLOSE,
			DH_CTRL_SEQPOWER_OPEN_ALL,
			DH_CTRL_SEQPOWER_CLOSE_ALL,
			DH_CTRL_PROJECTOR_RISE,
			DH_CTRL_PROJECTOR_FALL,
			DH_CTRL_PROJECTOR_STOP,
			DH_CTRL_INFRARED_KEY,

			DH_CTRL_START_PLAYAUDIO,
			DH_CTRL_STOP_PLAYAUDIO,
			DH_CTRL_START_ALARMBELL,
			DH_CTRL_STOP_ALARMBELL,
			DH_CTRL_ACCESS_OPEN,
			DH_CTRL_SET_BYPASS,

			DH_CTRL_RECORDSET_INSERT,
			DH_CTRL_RECORDSET_UPDATE,
			DH_CTRL_RECORDSET_REMOVE,
			DH_CTRL_RECORDSET_CLEAR,

			DH_CTRL_ACCESS_CLOSE,
		}

		[DllImport(@"dhnetsdk.dll")]
		public static extern bool CLIENT_ControlDevice(Int32 lLoginID, CtrlType type, IntPtr param, Int32 waittime);






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
		public struct CFG_ACCESS_GENERAL_INFO
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
			public Char[] szOpenDoorAudioPath;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
			public Char[] szCloseDoorAudioPath;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
			public Char[] szInUsedAuidoPath;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
			public Char[] szPauseUsedAudioPath;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
			public Char[] szNotClosedAudioPath;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
			public Char[] szWaitingAudioPath;
			public Int32 nUnlockReloadTime;
			public Int32 nUnlockHoldTime;
			public bool abProjectPassword;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
			public Byte[] byReserved;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
			public Char[] szProjectPassword;
		}

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_DevConfig_AccessGeneral(int lLoginId, out CFG_ACCESS_GENERAL_INFO result);

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

		[StructLayout(LayoutKind.Sequential)]
		public struct CFG_ACCESS_EVENT_INFO2
		{
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
			public bool bSensorEnable;
		}

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_GetDevConfig_AccessControl(int lLoginId, out CFG_ACCESS_EVENT_INFO result);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_SetDevConfig_AccessControl(int lLoginId, ref CFG_ACCESS_EVENT_INFO stuGeneralInfo);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_SetDevConfig_AccessControl2(int lLoginId, ref CFG_ACCESS_EVENT_INFO result);

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
		public static extern bool WRAP_GetDevConfig_AccessTimeSchedule(int lLoginId, out CFG_ACCESS_TIMESCHEDULE_INFO result);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_SetDevConfig_AccessTimeSchedule(int lLoginId, ref CFG_ACCESS_TIMESCHEDULE_INFO timeShedulrInfo);

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
		public static extern bool WRAP_Update_Card(int lLoginID, ref NET_RECORDSET_ACCESS_CTL_CARD stuCard);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_Update_Pwd(int lLoginID, ref NET_RECORDSET_ACCESS_CTL_PWD stuAccessCtlPwd);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_Update_CardRec(int lLoginID, ref NET_RECORDSET_ACCESS_CTL_CARDREC stuCardRec);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_Update_Holiday(int lLoginID, ref NET_RECORDSET_HOLIDAY stuHoliday);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_DevCtrl_ReBoot(int lLoginID);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_DevCtrl_DeleteCfgFile(int lLoginID);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern int WRAP_DevCtrl_GetLogCount(int lLoginID, ref QUERY_DEVICE_LOG_PARAM logParam);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_DevCtrl_RemoveRecordSet(int lLoginID, int nRecordNo, int nRecordSetType);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_DevCtrl_ClearRecordSet(int lLoginID, int nRecordSetType);

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
		public static extern int WRAP_DevCtrl_Get_Card_RecordSetCount(int lLoginID, ref FIND_RECORD_ACCESSCTLCARD_CONDITION stuParam);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern int WRAP_DevCtrl_Get_Password_RecordSetCount(int lLoginID);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern int WRAP_DevCtrl_Get_RecordSet_RecordSetCount(int lLoginID);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern int WRAP_DevCtrl_Get_Holiday_RecordSetCount(int lLoginID);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_GetCardInfo(int lLoginID, int recordNo, IntPtr result); // NET_RECORDSET_ACCESS_CTL_CARD

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_GetCardRecInfo(int lLoginID, int recordNo, IntPtr result); // NET_RECORDSET_ACCESS_CTL_CARDREC

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_GetPasswordInfo(int lLoginID, int recordNo, IntPtr result); // NET_RECORDSET_ACCESS_CTL_PWD

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_GetHolidayInfo(int lLoginID, int recordNo, IntPtr result); // NET_RECORDSET_HOLIDAY

		[StructLayout(LayoutKind.Sequential)]
		public struct CardsCollection
		{
			public int Count;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1000)]
			public NET_RECORDSET_ACCESS_CTL_CARD[] Cards;
		}

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_GetAllCards(int lLoginId, IntPtr result);

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
		public struct PasswordsCollection
		{
			public int Count;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1000)]
			public NET_RECORDSET_ACCESS_CTL_PWD[] Passwords;
		}

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_GetAllPasswords(int lLoginId, IntPtr result);

		[StructLayout(LayoutKind.Sequential)]
		public struct CardRecordsCollection
		{
			int Count;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1000)]
			NET_RECORDSET_ACCESS_CTL_CARDREC[] CardRecords;
		};

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_GetAllCardRecords(int lLoginId, IntPtr result);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_GetAccessTimeSchedule(int lLoginId, IntPtr result);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_SetAccessTimeSchedule(int lLoginId, CFG_ACCESS_TIMESCHEDULE_INFO timeShedule); // CFG_ACCESS_TIMESCHEDULE_INFO


		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_DevCtrl_OpenDoor(int lLoginID);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern bool WRAP_DevCtrl_CloseDoor(int lLoginId);

		[DllImport(@"EntranceGuardDemo.dll")]
		public static extern int WRAP_DevState_DoorStatus(int lLoginId);
	}
}