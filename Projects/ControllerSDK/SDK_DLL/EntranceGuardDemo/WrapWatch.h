#if !defined(__WRAP_WATCH_H__)
#define __WRAP_WATCH_H__

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

#endif // !defined(__WRAP_WATCH_H__)