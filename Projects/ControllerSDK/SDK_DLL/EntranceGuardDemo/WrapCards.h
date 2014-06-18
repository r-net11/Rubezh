#if !defined(__WRAP_CARDS_H__)
#define __WRAP_CARDS_H__

extern "C" CLIENT_API int CALL_METHOD WRAP_Insert_Card(int lLoginID, NET_RECORDSET_ACCESS_CTL_CARD* stuCard);

extern "C" CLIENT_API int CALL_METHOD WRAP_Update_Card(int lLoginID, NET_RECORDSET_ACCESS_CTL_CARD* stuCard);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetCardInfo(int lLoginID, int nRecordNo, NET_RECORDSET_ACCESS_CTL_CARD* result);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_RemoveCard(int lLoginID, int nRecordNo);

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_RemoveAllCards(int lLoginID);

extern "C" CLIENT_API int CALL_METHOD WRAP_Get_CardsCount(int lLoginID, FIND_RECORD_ACCESSCTLCARD_CONDITION* stuParam);

typedef struct tagNET_CardsCollection
{
	int Count;
	NET_RECORDSET_ACCESS_CTL_CARD Cards[1000];
}CardsCollection;

extern "C" CLIENT_API BOOL CALL_METHOD WRAP_GetAllCards(int lLoginId, CardsCollection* result);

#endif // !defined(__WRAP_CARDS_H__)