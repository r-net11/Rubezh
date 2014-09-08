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
	CFG_ACCESS_PROPERTY_TYPE emAccessProperty;
} WRAP_ControllerDirectionType;

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetControllerDirectionType(int loginID, WRAP_ControllerDirectionType* result);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_SetControllerDirectionType(int loginID, CFG_ACCESS_PROPERTY_TYPE emAccessProperty);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_SetControllerPassword(int loginID, char name[], char oldPassword[], char password[]);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetControllerTimeConfiguration(int loginID, CFG_NTP_INFO* result);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_SetControllerTimeConfiguration(int loginID, CFG_NTP_INFO cfg_NTP_INFO);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetDoorConfiguration(int loginID, int channelNo, CFG_ACCESS_EVENT_INFO* result);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_SetDoorConfiguration(int loginID, int channelNo, CFG_ACCESS_EVENT_INFO* stuGeneralInfo);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_ReBoot(int loginID);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_DeleteCfgFile(int loginID);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_OpenDoor(int loginID, int channelNo);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_CloseDoor(int loginID, int channelNo);

extern "C" CLIENT_API int CALL_METHOD WRAP_GetDoorStatus(int loginID, int channelNo);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_Upgrade(int loginID, char fileName[256]);

typedef struct tagWRAP_NET_LOG_INFO
{
    NET_TIME stuTime;
    char szUserName[DH_COMMON_STRING_32];
    char szLogType[DH_COMMON_STRING_128];
	char szLogMessage[DH_COMMON_STRING_1024];
} WRAP_NET_LOG_INFO;

typedef struct
{
	WRAP_NET_LOG_INFO Logs[10];
} WRAP_Dev_QueryLogList_Result;

extern "C" CLIENT_API int CALL_METHOD WRAP_GetLogCount(int loginID, QUERY_DEVICE_LOG_PARAM* logParam);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_QueryStart(int loginID);

extern "C" CLIENT_API int CALL_METHOD WRAP_QueryNext(WRAP_Dev_QueryLogList_Result* result);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_QueryStop();



extern "C" CLIENT_API BOOL CALL_METHOD TestStruct(CFG_ACCESS_EVENT_INFO* result);

#endif // !defined(__WRAP_H__)