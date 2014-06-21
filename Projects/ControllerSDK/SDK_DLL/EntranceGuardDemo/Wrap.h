#if !defined(__WRAP_H__)
#define __WRAP_H__

typedef struct  
{
	char szDevType[DH_DEV_TYPE_LEN];
	char szSoftWareVersion[DH_MAX_URL_LEN];
	DWORD dwSoftwareBuildDate_1;
	DWORD dwSoftwareBuildDate_2;
	DWORD dwSoftwareBuildDate_3;
} WRAP_DevConfig_TypeAndSoftInfo_Result;

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_DevConfig_TypeAndSoftInfo(int lLoginID, WRAP_DevConfig_TypeAndSoftInfo_Result* result);

typedef struct  
{
	char szIP[MAX_ADDRESS_LEN];
	char szSubnetMask[MAX_ADDRESS_LEN];
	char szDefGateway[MAX_ADDRESS_LEN];
	int	nMTU;
} WRAP_CFG_NETWORK_INFO_Result;

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_Get_DevConfig_IPMaskGate(int lLoginID, WRAP_CFG_NETWORK_INFO_Result* result);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_Set_DevConfig_IPMaskGate(int lLoginID, char* ip, char* mask, char* gate, int mtu);

typedef struct
{
	char szMAC[DH_MACADDR_LEN];
} WRAP_DevConfig_MAC_Result;

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_DevConfig_MAC(int lLoginID, WRAP_DevConfig_MAC_Result* result);

typedef struct
{
	int nMaxPageSize;
} WRAP_DevConfig_RecordFinderCaps_Result;

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_DevConfig_RecordFinderCaps(int lLoginID, WRAP_DevConfig_RecordFinderCaps_Result* result);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_DevConfig_GetCurrentTime(int lLoginID, NET_TIME* result);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_DevConfig_SetCurrentTime(int lLoginID, int dwYear, int dwMonth, int dwDay, int dwHour, int dwMinute, int dwSecond);

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

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_Dev_QueryLogList(int lLoginID, WRAP_Dev_QueryLogList_Result* result);

typedef struct
{
	char szProjectPassword[MAX_PASSWORD_LEN];
} WRAP_GeneralConfig_Password;

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetProjectPassword(int lLoginId, WRAP_GeneralConfig_Password* result);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_SetProjectPassword(int lLoginId, char password[]);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetDoorConfiguration(int lLoginId, int channelNo, CFG_ACCESS_EVENT_INFO* result);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_SetDoorConfiguration(int lLoginId, int channelNo, CFG_ACCESS_EVENT_INFO* stuGeneralInfo);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_DevCtrl_ReBoot(int lLoginID);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_DevCtrl_DeleteCfgFile(int lLoginID);

extern "C" CLIENT_API int CALL_METHOD WRAP_DevCtrl_GetLogCount(int lLoginID, QUERY_DEVICE_LOG_PARAM* logParam);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_DevCtrl_RemoveRecordSet(int lLoginID, int nRecordNo, int nRecordSetType);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_DevCtrl_ClearRecordSet(int lLoginID, int nRecordSetType);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_DevCtrl_OpenDoor(int lLoginID);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_DevCtrl_CloseDoor(int lLoginId);

extern "C" CLIENT_API int CALL_METHOD WRAP_DevState_DoorStatus(int lLoginId);



typedef void(__stdcall * WRAP_ProgressCallback)(int);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_StartProgress(WRAP_ProgressCallback progressCallback);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_StartListen(int lLoginId);

typedef struct
{
	int	EventType;
	NET_TIME DeviceDateTime;
	NET_GPS_STATUS_INFO stGPSStatusInfo;
	BYTE bEventAction;
	char szInfo[128];
	EM_TALKING_CALLER emCaller;
	int	nChannelID;
	int	nAction;
	NET_SENSE_METHOD emSenseType;
	int nBatteryLeft;
	float fTemperature;
	char szSensorName[DH_MACHINE_NAME_NUM];
	EM_POWER_TYPE emPowerType;
	EM_POWERFAULT_EVENT_TYPE emPowerFaultEvent;
	int nDoor;
	char szDoorName[DH_MAX_DOORNAME_LEN];
	NET_ACCESS_CTL_EVENT_TYPE emEventType;
	BOOL bStatus;
	NET_ACCESSCTLCARD_TYPE emCardType;
	NET_ACCESS_DOOROPEN_METHOD emOpenMethod;
	char szCardNo[DH_MAX_CARDNO_LEN];
	char szPwd[DH_MAX_CARDPWD_LEN];
} WRAP_JournalItem;

extern "C" CLIENT_API int CALL_METHOD WRAP_GetLastIndex();

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetJournalItem(int index, WRAP_JournalItem* result);

#endif // !defined(__WRAP_H__)