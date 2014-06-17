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

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_DevConfig_AccessGeneral(int lLoginId, CFG_ACCESS_GENERAL_INFO* result);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetDevConfig_AccessControl(int lLoginId, CFG_ACCESS_EVENT_INFO* result);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_SetDevConfig_AccessControl(int lLoginId, CFG_ACCESS_EVENT_INFO* stuGeneralInfo);


typedef struct tagCFG_ACCESS_EVENT_INFO2
{
	CFG_ACCESS_STATE    emState;						// 门禁状态
	CFG_ACCESS_MODE     emMode;							// 门禁模式
	int					nEnableMode;					// 门禁使能电平值, 0:低电平有效(断电启动); 1:高电平有效(通电启动);
	BOOL                bSnapshotEnable;				// 事件联动抓图使能
	bool                abDoorOpenMethod;
	bool                abUnlockHoldInterval;
	bool                abCloseTimeout;
	bool                abOpenAlwaysTimeIndex;
	bool                abHolidayTimeIndex;
	bool                abBreakInAlarmEnable;
	bool				abRepeatEnterAlarmEnable;
	bool                abDoorNotClosedAlarmEnable;
	bool				abDuressAlarmEnable;
	bool                abDoorTimeSection;
	bool				abSensorEnable;
	BYTE				byReserved;
	CFG_DOOR_OPEN_METHOD	emDoorOpenMethod;			// 开门方式
	int					nUnlockHoldInterval;			// 门锁保持时间(自动关门时间),单位毫秒,[250, 20000]
	int					nCloseTimeout;					// 关门超时时间, 超过阈值未关会触发报警，单位秒,[0,9999];0表示不检测超时
	int					nOpenAlwaysTimeIndex;			// 常开时间段, 值为CFG_ACCESS_TIMESCHEDULE_INFO配置的数组下标
	int					nHolidayTimeRecoNo;				// 假期内时间段, 值为假日记录集的记录编号，对应NET_RECORDSET_HOLIDAY的nRecNo
	BOOL				bBreakInAlarmEnable;			// 闯入报警使能
	BOOL				bRepeatEnterAlarm;				// 反潜报警使能
	BOOL				bDoorNotClosedAlarmEnable;		// 门未关报警使能
	BOOL				bDuressAlarmEnable;				// 胁迫报警使能
	BOOL				bSensorEnable;					// 门磁使能
}CFG_ACCESS_EVENT_INFO2;

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_SetDevConfig_AccessControl2(int lLoginId, CFG_ACCESS_EVENT_INFO* stuGeneralInfo);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetDevConfig_AccessTimeSchedule(int lLoginId, CFG_ACCESS_TIMESCHEDULE_INFO* result);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_SetDevConfig_AccessTimeSchedule(int lLoginId, CFG_ACCESS_TIMESCHEDULE_INFO* stuInfo);

extern "C" CLIENT_API int CALL_METHOD WRAP_Insert_Card(int lLoginID, NET_RECORDSET_ACCESS_CTL_CARD* stuCard);

extern "C" CLIENT_API int CALL_METHOD WRAP_Insert_Pwd(int lLoginID, NET_RECORDSET_ACCESS_CTL_PWD* stuAccessCtlPwd);

extern "C" CLIENT_API int CALL_METHOD WRAP_Insert_CardRec(int lLoginID, NET_RECORDSET_ACCESS_CTL_CARDREC* stuCardRec);

extern "C" CLIENT_API int CALL_METHOD WRAP_Insert_Holiday(int lLoginID, NET_RECORDSET_HOLIDAY* stuHoliday);

extern "C" CLIENT_API int CALL_METHOD WRAP_Update_Card(int lLoginID, NET_RECORDSET_ACCESS_CTL_CARD* stuCard);

extern "C" CLIENT_API int CALL_METHOD WRAP_Update_Pwd(int lLoginID, NET_RECORDSET_ACCESS_CTL_PWD* stuAccessCtlPwd);

extern "C" CLIENT_API int CALL_METHOD WRAP_Update_CardRec(int lLoginID, NET_RECORDSET_ACCESS_CTL_CARDREC* stuCardRec);

extern "C" CLIENT_API int CALL_METHOD WRAP_Update_Holiday(int lLoginID, NET_RECORDSET_HOLIDAY* stuHoliday);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_DevCtrl_ReBoot(int lLoginID);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_DevCtrl_DeleteCfgFile(int lLoginID);

extern "C" CLIENT_API int CALL_METHOD WRAP_DevCtrl_GetLogCount(int lLoginID, QUERY_DEVICE_LOG_PARAM* logParam);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_DevCtrl_RemoveRecordSet(int lLoginID, int nRecordNo, int nRecordSetType);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_DevCtrl_ClearRecordSet(int lLoginID, int nRecordSetType);

extern "C" CLIENT_API int CALL_METHOD WRAP_DevCtrl_Get_Card_RecordSetCount(int lLoginID, FIND_RECORD_ACCESSCTLCARD_CONDITION* stuParam);
extern "C" CLIENT_API int CALL_METHOD WRAP_DevCtrl_Get_Password_RecordSetCount(int lLoginID);
extern "C" CLIENT_API int CALL_METHOD WRAP_DevCtrl_Get_RecordSet_RecordSetCount(int lLoginID);
extern "C" CLIENT_API int CALL_METHOD WRAP_DevCtrl_Get_Holiday_RecordSetCount(int lLoginID);


extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetCardInfo(int lLoginID, int nRecordNo, NET_RECORDSET_ACCESS_CTL_CARD* result);
extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetCardRecInfo(int lLoginID, int nRecordNo, NET_RECORDSET_ACCESS_CTL_CARDREC* result);
extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetPasswordInfo(int lLoginID, int nRecordNo, NET_RECORDSET_ACCESS_CTL_PWD* result);
extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetHolidayInfo(int lLoginID, int nRecordNo, NET_RECORDSET_HOLIDAY* result);

typedef struct tagNET_CardsCollection
{
	int Count;
	NET_RECORDSET_ACCESS_CTL_CARD Cards[1000];
}CardsCollection;

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetAllCards(int lLoginId, CardsCollection* result);

typedef struct tagNET_PasswordsCollection
{
	int Count;
	NET_RECORDSET_ACCESS_CTL_PWD Passwords[1000];
}PasswordsCollection;

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetAllPasswords(int lLoginId, PasswordsCollection* result);

typedef struct tagNET_CardRecordsCollection
{
	int Count;
	NET_RECORDSET_ACCESS_CTL_CARDREC CardRecords[1000];
}CardRecordsCollection;

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetAllCardRecords(int lLoginId, CardRecordsCollection* result);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetAccessTimeSchedule(int lLoginId, CFG_ACCESS_TIMESCHEDULE_INFO* result);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_SetAccessTimeSchedule(int lLoginId, CFG_ACCESS_TIMESCHEDULE_INFO timeSheduleInfo);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_DevCtrl_OpenDoor(int lLoginID);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_DevCtrl_CloseDoor(int lLoginId);

extern "C" CLIENT_API int CALL_METHOD WRAP_DevState_DoorStatus(int lLoginId);

#endif // !defined(__WRAP_H__)