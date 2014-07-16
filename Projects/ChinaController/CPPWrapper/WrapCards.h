#if !defined(__WRAP_CARDS_H__)
#define __WRAP_CARDS_H__

extern "C" CLIENT_API int CALL_METHOD WRAP_Insert_Card(int loginID, NET_RECORDSET_ACCESS_CTL_CARD* param);

extern "C" CLIENT_API int CALL_METHOD WRAP_Update_Card(int loginID, NET_RECORDSET_ACCESS_CTL_CARD* param);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_Remove_Card(int loginID, int recordNo);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_RemoveAll_Cards(int loginID);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_Get_Card_Info(int loginID, int recordNo, NET_RECORDSET_ACCESS_CTL_CARD* result);

typedef struct tagNET_CardsCollection
{
	int Count;
	NET_RECORDSET_ACCESS_CTL_CARD Cards[10];
}CardsCollection;

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_BeginGetAll_Cards(int loginID, int& finderID);
extern "C" CLIENT_API int CALL_METHOD WRAP_GetAll_Cards(int finderID, CardsCollection* result);

extern "C" CLIENT_API int CALL_METHOD WRAP_GetAllCount(int finderID);
extern "C" CLIENT_API BOOL CALL_METHOD WRAP_EndGetAll(int finderID);


#endif // !defined(__WRAP_CARDS_H__)