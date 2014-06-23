#if !defined(__WRAP_CARDS_H__)
#define __WRAP_CARDS_H__

extern "C" CLIENT_API int CALL_METHOD WRAP_Insert_Card(int loginID, NET_RECORDSET_ACCESS_CTL_CARD* stuCard);

extern "C" CLIENT_API int CALL_METHOD WRAP_Update_Card(int loginID, NET_RECORDSET_ACCESS_CTL_CARD* stuCard);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_Remove_Card(int loginID, int nRecordNo);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_RemoveAll_Cards(int loginID);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_Get_Card_Info(int loginID, int nRecordNo, NET_RECORDSET_ACCESS_CTL_CARD* result);

extern "C" CLIENT_API int CALL_METHOD WRAP_Get_Cards_Count(int loginID, FIND_RECORD_ACCESSCTLCARD_CONDITION* stuParam);

typedef struct tagNET_CardsCollection
{
	int Count;
	NET_RECORDSET_ACCESS_CTL_CARD Cards[1000];
}CardsCollection;

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetAll_Cards(int loginID, CardsCollection* result);

#endif // !defined(__WRAP_CARDS_H__)