#if !defined(__WRAP_WATCH_H__)
#define __WRAP_WATCH_H__

typedef void(__stdcall * WRAP_ProgressCallback)(int);

extern "C" CLIENT_API void CALL_METHOD WRAP_Initialize();

extern "C" CLIENT_API int CALL_METHOD WRAP_Connect(char ipAddress[25], int port, char userName[25], char password[25]);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_Disconnect(int loginID);

typedef struct tag_WRAP_JournalItem
{
	int LoginID;
	int	ExtraEventType;
	int	EventType;
	NET_TIME DeviceDateTime;
	int nDoor;
	NET_ACCESS_CTL_EVENT_TYPE emEventType;
	BOOL bStatus;
	NET_ACCESSCTLCARD_TYPE emCardType;
	NET_ACCESS_DOOROPEN_METHOD emOpenMethod;
	char szCardNo[DH_MAX_CARDNO_LEN];
	char szPwd[DH_MAX_CARDPWD_LEN];
	int	nAction;
} WRAP_JournalItem;

typedef struct tag_WRAP_JournalInfo
{
	int Index;
	WRAP_JournalItem JournalItem;
} WRAP_JournalInfo;

typedef struct tag_WRAP_WatchInfo
{
	int LoginId;
	//WRAP_JournalItem* WRAP_JournalItems;
	//int JournalLastIndex;
	BOOL IsConnected;
} WRAP_WatchInfo;

extern "C" CLIENT_API int CALL_METHOD WRAP_GetLastIndex();

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetJournalItem(int index, WRAP_JournalItem* result);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_IsConnected(int loginID);

#endif // !defined(__WRAP_WATCH_H__)