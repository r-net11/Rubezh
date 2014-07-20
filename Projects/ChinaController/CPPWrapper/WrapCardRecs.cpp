#include "StdAfx.h"
#include "WrapCardRecs.h"

#include <iostream>
#include <fstream>
using namespace std;

#define QUERY_COUNT	(3)

int CALL_METHOD WRAP_Insert_CardRec(int loginID, NET_RECORDSET_ACCESS_CTL_CARDREC* param)
{
	if (NULL == loginID)
	{
		return 0;
	}

	NET_CTRL_RECORDSET_INSERT_PARAM stuInert = {sizeof(stuInert)};
	stuInert.stuCtrlRecordSetInfo.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_IN);
    stuInert.stuCtrlRecordSetInfo.emType = NET_RECORD_ACCESSCTLCARDREC;
	stuInert.stuCtrlRecordSetInfo.pBuf = param;
	stuInert.stuCtrlRecordSetInfo.nBufLen = sizeof(NET_RECORDSET_ACCESS_CTL_CARDREC);
	
	stuInert.stuCtrlRecordSetResult.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_OUT);
    BOOL bResult = CLIENT_ControlDevice(loginID, DH_CTRL_RECORDSET_INSERT, &stuInert, 3000);
	int nRecrdNo = stuInert.stuCtrlRecordSetResult.nRecNo;
	if (bResult)
	{
		return nRecrdNo;
	}
	return 0;
}

BOOL CALL_METHOD WRAP_Update_CardRec(int loginID, NET_RECORDSET_ACCESS_CTL_CARDREC* param)
{
	if (NULL == loginID)
	{
		return FALSE;
	}

	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
    stuInert.emType = NET_RECORD_ACCESSCTLCARDREC;
	stuInert.pBuf = param;
	stuInert.nBufLen = sizeof(NET_RECORDSET_ACCESS_CTL_CARDREC);
	
    BOOL bResult = CLIENT_ControlDevice(loginID, DH_CTRL_RECORDSET_UPDATE, &stuInert, SDK_API_WAITTIME);
    return bResult;
}

BOOL CALL_METHOD WRAP_Remove_CardRec(int loginID, int recordNo)
{
	if (NULL == loginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	stuInert.pBuf = &recordNo;
	stuInert.nBufLen = sizeof(recordNo);
	stuInert.emType = NET_RECORD_ACCESSCTLCARDREC;
    BOOL bResult = CLIENT_ControlDevice(loginID, DH_CTRL_RECORDSET_REMOVE, &stuInert, SDK_API_WAITTIME);
	return bResult;
}

BOOL CALL_METHOD WRAP_RemoveAll_CardRecs(int loginID)
{
	if (NULL == loginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	stuInert.emType = NET_RECORD_ACCESSCTLCARDREC;
    BOOL bResult = CLIENT_ControlDevice(loginID, DH_CTRL_RECORDSET_CLEAR, &stuInert, SDK_API_WAITTIME);
	return bResult;
}

BOOL CALL_METHOD WRAP_Get_CardRec_Info(int loginID, int recordNo, NET_RECORDSET_ACCESS_CTL_CARDREC* result)
{
	if (NULL == loginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	NET_RECORDSET_ACCESS_CTL_CARDREC stuCardRec = {sizeof(stuCardRec)};
	
	stuCardRec.nRecNo = recordNo;
	stuInert.emType = NET_RECORD_ACCESSCTLCARDREC;
	stuInert.pBuf = &stuCardRec;

	int nRet = 0;
	BOOL bRet = CLIENT_QueryDevState(loginID, DH_DEVSTATE_DEV_RECORDSET, (char*)&stuInert, sizeof(stuInert), &nRet, 3000);

	memcpy(result, &stuCardRec, sizeof(stuCardRec));
	return bRet;
}

BOOL CALL_METHOD WRAP_BeginGetAll_CardRecs(int loginID, int& finderID)
{
	NET_IN_FIND_RECORD_PARAM stuIn = {sizeof(stuIn)};
	NET_OUT_FIND_RECORD_PARAM stuOut = {sizeof(stuOut)};
	
	stuIn.emType = NET_RECORD_ACCESSCTLCARDREC;
	
	if (CLIENT_FindRecord(loginID, &stuIn, &stuOut, SDK_API_WAITTIME))
	{
		loginID = stuOut.lFindeHandle;
	}

	return loginID > 0;
}

int CALL_METHOD WRAP_GetAll_CardRecs(int finderID, CardRecsCollection* result)
{
	CardRecsCollection cardRecsCollection = {sizeof(CardRecsCollection)};

	int i = 0, j = 0;
	int nMaxNum = 10;
	
	NET_IN_FIND_NEXT_RECORD_PARAM stuIn = {sizeof(stuIn)};
	stuIn.lFindeHandle = finderID;
	stuIn.nFileCount = nMaxNum;
	
	NET_OUT_FIND_NEXT_RECORD_PARAM stuOut = {sizeof(stuOut)};
	stuOut.nMaxRecordNum = nMaxNum;
	
	NET_RECORDSET_ACCESS_CTL_CARDREC* pstuCardRec = new NET_RECORDSET_ACCESS_CTL_CARDREC[nMaxNum];
	if (NULL == pstuCardRec)
	{
		return -1;
	}
	memset(pstuCardRec, 0, sizeof(NET_RECORDSET_ACCESS_CTL_CARDREC) * nMaxNum);
	
	for (i = 0; i < nMaxNum; i++)
	{
		pstuCardRec[i].dwSize = sizeof(NET_RECORDSET_ACCESS_CTL_CARDREC);
	}
	stuOut.pRecordList = (void*)pstuCardRec;
	
	if (CLIENT_FindNextRecord(&stuIn, &stuOut, SDK_API_WAITTIME) >= 0)
	{
		for (i = 0; i < __min(stuOut.nMaxRecordNum, stuOut.nRetRecordNum); i++)
		{
			memcpy(&cardRecsCollection.CardRecs[i], &pstuCardRec[i], sizeof(NET_RECORDSET_ACCESS_CTL_CARDREC));
		}
	}
	
	delete[] pstuCardRec;
	pstuCardRec = NULL;

	memcpy(result, &cardRecsCollection, sizeof(CardRecsCollection));
	return stuOut.nRetRecordNum;
}