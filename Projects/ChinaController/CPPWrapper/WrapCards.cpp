#include "StdAfx.h"
#include "WrapCards.h"

#include <iostream>
#include <fstream>
using namespace std;

#define QUERY_COUNT	(3)

int CALL_METHOD WRAP_Insert_Card(int loginID, NET_RECORDSET_ACCESS_CTL_CARD* param)
{
	if (NULL == loginID)
	{
		return 0;
	}

	NET_CTRL_RECORDSET_INSERT_PARAM stuInert = {sizeof(stuInert)};
	stuInert.stuCtrlRecordSetInfo.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_IN);
    stuInert.stuCtrlRecordSetInfo.emType = NET_RECORD_ACCESSCTLCARD;
	stuInert.stuCtrlRecordSetInfo.pBuf = (void*)param;
	stuInert.stuCtrlRecordSetInfo.nBufLen = sizeof(NET_RECORDSET_ACCESS_CTL_CARD);

	stuInert.stuCtrlRecordSetResult.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_OUT);

    BOOL bResult = CLIENT_ControlDevice(loginID, DH_CTRL_RECORDSET_INSERT, &stuInert, SDK_API_WAITTIME);
	int nRecrdNo = stuInert.stuCtrlRecordSetResult.nRecNo;
	if (bResult)
	{
		return nRecrdNo;
	}
	return 0;
}

int CALL_METHOD WRAP_Insert_Card_Fast(int loginID, NET_RECORDSET_ACCESS_CTL_CARD* param)
{
	if (NULL == loginID)
	{
		return 0;
	}

	NET_RECORDSET_ACCESS_CTL_CARD cards[] = { *param };
	int nRecordNo = 0;
	BOOL bResult = CLIENT_InsertAccessControlCards(loginID, 1, cards, &nRecordNo);
	if (bResult)
	{
		return nRecordNo;
	}
	return 0;
}

BOOL CALL_METHOD WRAP_Update_Card(int loginID, NET_RECORDSET_ACCESS_CTL_CARD* param)
{
	if (NULL == loginID)
	{
		return FALSE;
	}

	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
    stuInert.emType = NET_RECORD_ACCESSCTLCARD;
	stuInert.pBuf = (void*)param;
	stuInert.nBufLen = sizeof(NET_RECORDSET_ACCESS_CTL_CARD);
    BOOL bResult = CLIENT_ControlDevice(loginID, DH_CTRL_RECORDSET_UPDATE, &stuInert, SDK_API_WAITTIME);
	return bResult;
}

BOOL CALL_METHOD WRAP_Remove_Card(int loginID, int recordNo)
{
	if (NULL == loginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	stuInert.emType = NET_RECORD_ACCESSCTLCARD;
	stuInert.pBuf = (void*)&recordNo;
	stuInert.nBufLen = sizeof(recordNo);
    BOOL bResult = CLIENT_ControlDevice(loginID, DH_CTRL_RECORDSET_REMOVE, &stuInert, SDK_API_WAITTIME);
	return bResult;
}

BOOL CALL_METHOD WRAP_RemoveAll_Cards(int loginID)
{
	if (NULL == loginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	stuInert.emType = NET_RECORD_ACCESSCTLCARD;
    BOOL bResult = CLIENT_ControlDevice(loginID, DH_CTRL_RECORDSET_CLEAR, &stuInert, SDK_API_WAITTIME);
	return bResult;
}

BOOL CALL_METHOD WRAP_Get_Card_Info(int loginID, int recordNo, NET_RECORDSET_ACCESS_CTL_CARD* result)
{
	if (NULL == loginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	NET_RECORDSET_ACCESS_CTL_CARD stuCard = {sizeof(stuCard)};

	stuCard.nRecNo = recordNo;
	stuInert.emType = NET_RECORD_ACCESSCTLCARD;
	stuInert.pBuf = &stuCard;

	int nRet = 0;
	BOOL bRet = CLIENT_QueryDevState(loginID, DH_DEVSTATE_DEV_RECORDSET, (char*)&stuInert, sizeof(stuInert), &nRet, SDK_API_WAITTIME);

	memcpy(result, &stuCard, sizeof(stuCard));
	return bRet;
}

/// <summary>
/// Инициализирует процедуру получения информации о записанных на контроллере картах
/// </summary>
/// <returns>TRUE при успешном завершении</returns>
BOOL CALL_METHOD WRAP_BeginGetAll_Cards(int loginID, int& finderID)
{
	NET_IN_FIND_RECORD_PARAM stuIn = {sizeof(stuIn)};
	NET_OUT_FIND_RECORD_PARAM stuOut = {sizeof(stuOut)};

	stuIn.emType = NET_RECORD_ACCESSCTLCARD;

	if (CLIENT_FindRecord(loginID, &stuIn, &stuOut, SDK_API_WAITTIME))
	{
		finderID = stuOut.lFindeHandle;
	}
	return finderID > 0;
}

int CALL_METHOD WRAP_GetAll_Cards(int finderID, CardsCollection* result)
{
	CardsCollection cardsCollection = {sizeof(CardsCollection)};
	memset(&cardsCollection, 0, sizeof(CardsCollection));

	int i = 0, j = 0;
	int nMaxNum = 10;

	NET_IN_FIND_NEXT_RECORD_PARAM stuIn = {sizeof(stuIn)};
	stuIn.lFindeHandle = finderID;
	stuIn.nFileCount = nMaxNum;

	NET_OUT_FIND_NEXT_RECORD_PARAM stuOut = {sizeof(stuOut)};
	stuOut.nMaxRecordNum = nMaxNum;

	NET_RECORDSET_ACCESS_CTL_CARD* pstuCard = new NET_RECORDSET_ACCESS_CTL_CARD[nMaxNum];
	if (NULL == pstuCard)
	{
		return -1;
	}
	memset(pstuCard, 0, sizeof(NET_RECORDSET_ACCESS_CTL_CARD) * nMaxNum);

	for (i = 0; i < nMaxNum; i++)
	{
		pstuCard[i].dwSize = sizeof(NET_RECORDSET_ACCESS_CTL_CARD);
	}
	stuOut.pRecordList = (void*)pstuCard;

	if (CLIENT_FindNextRecord(&stuIn, &stuOut, SDK_API_WAITTIME) >= 0)
    {
		int itemsCount = __min(stuOut.nMaxRecordNum, stuOut.nRetRecordNum);
		for (i = 0; i < itemsCount; i++)
		{
			memcpy(&cardsCollection.Cards[i], &pstuCard[i], sizeof(NET_RECORDSET_ACCESS_CTL_CARD));
		}
		cardsCollection.Count = itemsCount;
	}

	delete[] pstuCard;
	pstuCard = NULL;

	memcpy(result, &cardsCollection, sizeof(CardsCollection));
	return stuOut.nRetRecordNum;
}

BOOL CALL_METHOD WRAP_Card_ClearRepeatEnter(int loginID, char* recordNo)
{
	if (NULL == loginID)
	{
		return FALSE;
	}
	BOOL bRet = FALSE;

	////Пыіэ·ґЗ±±ЁѕЇ ClearRepeatEnter RPCµчУГ
	NET_IN_CLEAR_REPEAT_ENTER stuInClearRepeatEnter;
	strcpy(stuInClearRepeatEnter.szCardNO, recordNo);
	NET_OUT_CLEAR_REPEAT_ENTER stuOutClearRepeatEnter;
	bRet = CLIENT_ClearRepeatEnter(loginID, &stuInClearRepeatEnter, &stuOutClearRepeatEnter, 3000);
	return bRet;
}