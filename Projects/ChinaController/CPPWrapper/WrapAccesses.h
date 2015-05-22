#if !defined(__WRAP_ACCESSES_H__)
#define __WRAP_ACCESSES_H__

extern "C" SDK_CLIENT_API BOOL SDK_CALL_METHOD WRAP_Get_Access_Info(int loginID, int recordNo, NET_RECORDSET_ACCESS_CTL_CARDREC* result);

typedef struct tagNET_AccessesCollection
{
	int Count;
	NET_RECORDSET_ACCESS_CTL_CARDREC Accesses[10];
}AccessesCollection;

extern "C" SDK_CLIENT_API BOOL SDK_CALL_METHOD WRAP_BeginGetAll_Accesses(int loginID, int& finderID);
extern "C" SDK_CLIENT_API int SDK_CALL_METHOD WRAP_GetAll_Accesses(int finderID, AccessesCollection* result);

#endif // !defined(__WRAP_ACCESSES_H__)