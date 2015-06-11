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
	CFG_ACCESS_STATE    emState;						// �Ž�״̬
	CFG_ACCESS_MODE     emMode;							// �Ž�ģʽ
	int					nEnableMode;					// �Ž�ʹ�ܵ�ƽֵ, 0:�͵�ƽ��Ч(�ϵ�����); 1:�ߵ�ƽ��Ч(ͨ������);
	BOOL                bSnapshotEnable;				// �¼�����ץͼʹ��
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
	CFG_DOOR_OPEN_METHOD	emDoorOpenMethod;			// ���ŷ�ʽ
	int					nUnlockHoldInterval;			// ��������ʱ��(�Զ�����ʱ��),��λ����,[250, 20000]
	int					nCloseTimeout;					// ���ų�ʱʱ��, ������ֵδ�ػᴥ����������λ��,[0,9999];0��ʾ����ⳬʱ
	int					nOpenAlwaysTimeIndex;			// ����ʱ���, ֵΪCFG_ACCESS_TIMESCHEDULE_INFO���õ������±�
	int					nHolidayTimeRecoNo;				// ������ʱ���, ֵΪ���ռ�¼���ļ�¼��ţ���ӦNET_RECORDSET_HOLIDAY��nRecNo
	BOOL				bBreakInAlarmEnable;			// ���뱨��ʹ��
	BOOL				bRepeatEnterAlarm;				// ��Ǳ����ʹ��
	BOOL				bDoorNotClosedAlarmEnable;		// ��δ�ر���ʹ��
	BOOL				bDuressAlarmEnable;				// в�ȱ���ʹ��
	BOOL				bSensorEnable;					// �Ŵ�ʹ��
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

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_DevCtrl_OpenDoor(int lLoginID);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_DevCtrl_ReBoot(int lLoginID);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_DevCtrl_DeleteCfgFile(int lLoginID);

extern "C" CLIENT_API int CALL_METHOD WRAP_DevCtrl_GetLogCount(int lLoginID, QUERY_DEVICE_LOG_PARAM* logParam);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_DevCtrl_RemoveRecordSet(int lLoginID, int nRecordNo, int nRecordSetType);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_DevCtrl_ClearRecordSet(int lLoginID, int nRecordSetType);

extern "C" CLIENT_API int CALL_METHOD WRAP_DevCtrl_Get_Card_RecordSetCount(int lLoginID, FIND_RECORD_ACCESSCTLCARD_CONDITION* stuParam);
extern "C" CLIENT_API int CALL_METHOD WRAP_DevCtrl_Get_Password_RecordSetCount(int lLoginID);
extern "C" CLIENT_API int CALL_METHOD WRAP_DevCtrl_Get_RecordSet_RecordSetCount(int lLoginID);
extern "C" CLIENT_API int CALL_METHOD WRAP_DevCtrl_Get_Holiday_RecordSetCount(int lLoginID);

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

#endif // !defined(__WRAP_H__)