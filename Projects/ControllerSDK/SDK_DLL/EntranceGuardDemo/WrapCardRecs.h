#if !defined(__WRAP_CARDRECS_H__)
#define __WRAP_CARDRECS_H__

extern "C" CLIENT_API int CALL_METHOD WRAP_Insert_CardRec(int lLoginID, NET_RECORDSET_ACCESS_CTL_CARDREC* stuCardRec);

extern "C" CLIENT_API int CALL_METHOD WRAP_Update_CardRec(int lLoginID, NET_RECORDSET_ACCESS_CTL_CARDREC* stuCardRec);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetCardRecInfo(int lLoginID, int nRecordNo, NET_RECORDSET_ACCESS_CTL_CARDREC* result);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_RemoveCardRec(int lLoginID, int nRecordNo);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_RemoveAllCardRecs(int lLoginID);

extern "C" CLIENT_API int CALL_METHOD WRAP_Get_CardRecordsCount(int lLoginID);

typedef struct tagNET_CardRecsCollection
{
	int Count;
	NET_RECORDSET_ACCESS_CTL_CARDREC CardRecs[1000];
}CardRecsCollection;

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetAllCardRecs(int lLoginId, CardRecsCollection* result);

typedef struct tagNET_CardRecordsCollection
{
	int Count;
	NET_RECORDSET_ACCESS_CTL_CARDREC CardRecords[1000];
}CardRecordsCollection;

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetAllCardRecords(int lLoginId, CardRecordsCollection* result);

#endif // !defined(__WRAP_CARDRECS_H__)