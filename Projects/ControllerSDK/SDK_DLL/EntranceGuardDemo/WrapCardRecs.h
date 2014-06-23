#if !defined(__WRAP_CARDRECS_H__)
#define __WRAP_CARDRECS_H__

extern "C" CLIENT_API int CALL_METHOD WRAP_Insert_CardRec(int loginID, NET_RECORDSET_ACCESS_CTL_CARDREC* stuCardRec);

extern "C" CLIENT_API int CALL_METHOD WRAP_Update_CardRec(int loginID, NET_RECORDSET_ACCESS_CTL_CARDREC* stuCardRec);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_Remove_CardRec(int loginID, int nRecordNo);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_RemoveAll_CardRecs(int loginID);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_Get_CardRec_Info(int loginID, int nRecordNo, NET_RECORDSET_ACCESS_CTL_CARDREC* result);

extern "C" CLIENT_API int CALL_METHOD WRAP_Get_CardRecs_Count(int loginID);

typedef struct tagNET_CardRecsCollection
{
	int Count;
	NET_RECORDSET_ACCESS_CTL_CARDREC CardRecs[1000];
}CardRecsCollection;

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetAll_CardRecs(int loginID, CardRecsCollection* result);

#endif // !defined(__WRAP_CARDRECS_H__)