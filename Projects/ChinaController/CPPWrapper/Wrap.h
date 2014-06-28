#if !defined(__WRAP_H__)
#define __WRAP_H__

typedef struct  
{
	char szDevType[DH_DEV_TYPE_LEN];
	char szSoftWareVersion[DH_MAX_URL_LEN];
	DWORD dwSoftwareBuildDate_Year;
	DWORD dwSoftwareBuildDate_Month;
	DWORD dwSoftwareBuildDate_Day;
} WRAP_DevConfig_TypeAndSoftInfo_Result;

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetSoftwareInfo(int loginID, WRAP_DevConfig_TypeAndSoftInfo_Result* result);

typedef struct  
{
	char szIP[MAX_ADDRESS_LEN];
	char szSubnetMask[MAX_ADDRESS_LEN];
	char szDefGateway[MAX_ADDRESS_LEN];
	int	nMTU;
} WRAP_CFG_NETWORK_INFO_Result;

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_Get_NetInfo(int loginID, WRAP_CFG_NETWORK_INFO_Result* result);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_Set_NetInfo(int loginID, char* ip, char* mask, char* gate, int mtu);

typedef struct
{
	char szMAC[DH_MACADDR_LEN];
} WRAP_DevConfig_MAC_Result;

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetMacAddress(int loginID, WRAP_DevConfig_MAC_Result* result);

typedef struct
{
	int nMaxPageSize;
} WRAP_DevConfig_RecordFinderCaps_Result;

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetMaxPageSize(int loginID, WRAP_DevConfig_RecordFinderCaps_Result* result);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetCurrentTime(int loginID, NET_TIME* result);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_SetCurrentTime(int loginID, int dwYear, int dwMonth, int dwDay, int dwHour, int dwMinute, int dwSecond);

typedef struct
{
	int	nLogType;
	DHDEVTIME stuOperateTime;
	char szOperator[16];
	char szOperation[32];
	char szDetailContext[4*1024];
} WRAP_LogItem;

typedef struct
{
	WRAP_LogItem Logs[10];
	int Test;
} WRAP_Dev_QueryLogList_Result;

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_QueryLogList(int loginID, WRAP_Dev_QueryLogList_Result* result);

typedef struct
{
	char szProjectPassword[MAX_PASSWORD_LEN];
} WRAP_GeneralConfig_Password;

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetProjectPassword(int loginID, WRAP_GeneralConfig_Password* result);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_SetProjectPassword(int loginID, char password[]);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetDoorConfiguration(int loginID, int channelNo, CFG_ACCESS_EVENT_INFO* result);

typedef struct tagCFG_ACCESS_EVENT_INFO_PART_1
{
	char szChannelName[MAX_NAME_LEN];
	CFG_ACCESS_STATE emState;
	CFG_ACCESS_MODE emMode;
	int nEnableMode;
	BOOL bSnapshotEnable;
	bool abDoorOpenMethod;
    bool abUnlockHoldInterval;
	bool abCloseTimeout;
	bool abOpenAlwaysTimeIndex;
	bool abHolidayTimeIndex;
	bool abBreakInAlarmEnable;
	bool abRepeatEnterAlarmEnable;
	bool abDoorNotClosedAlarmEnable;
	bool abDuressAlarmEnable;
	bool abDoorTimeSection;
	bool abSensorEnable;
	BYTE byReserved;
}CFG_ACCESS_EVENT_INFO_PART_1;

typedef struct tagCFG_ACCESS_EVENT_INFO_PART_2
{
	CFG_DOOR_OPEN_METHOD emDoorOpenMethod;
	int	 nUnlockHoldInterval;
	int	nCloseTimeout;
	int	nOpenAlwaysTimeIndex;
	int	nHolidayTimeRecoNo;
	BOOL bBreakInAlarmEnable;
	BOOL bRepeatEnterAlarm;
	BOOL bDoorNotClosedAlarmEnable;
	BOOL bDuressAlarmEnable;
	CFG_DOOROPEN_TIMESECTION_INFO stuDoorTimeSection[WEEK_DAY_NUM][MAX_DOOR_TIME_SECTION];
	BOOL bSensorEnable;
}CFG_ACCESS_EVENT_INFO_PART_2;

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_SetDoorConfiguration(int loginID, int channelNo, CFG_ACCESS_EVENT_INFO* stuGeneralInfo, CFG_ACCESS_EVENT_INFO_PART_1* stuShort_Part1, CFG_ACCESS_EVENT_INFO_PART_2* stuShort_Part2);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_ReBoot(int loginID);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_DeleteCfgFile(int loginID);

extern "C" CLIENT_API int CALL_METHOD WRAP_GetLogCount(int loginID, QUERY_DEVICE_LOG_PARAM* logParam);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_OpenDoor(int loginID, int channelNo);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_CloseDoor(int loginID, int channelNo);

extern "C" CLIENT_API int CALL_METHOD WRAP_GetDoorStatus(int loginID, int channelNo);

#endif // !defined(__WRAP_H__)