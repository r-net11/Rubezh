#include "StdAfx.h"
#include "WrapCardRecs.h"

#include <iostream>
#include <fstream>
using namespace std;

#define QUERY_COUNT	(3)

int CALL_METHOD WRAP_Insert_CardRec(int loginID, NET_RECORDSET_ACCESS_CTL_CARDREC* stuCardRec)
{
	if (NULL == loginID)
	{
		return 0;
	}

	NET_CTRL_RECORDSET_INSERT_PARAM stuInert = {sizeof(stuInert)};
	stuInert.stuCtrlRecordSetInfo.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_IN);
    stuInert.stuCtrlRecordSetInfo.emType = NET_RECORD_ACCESSCTLCARDREC;
	stuInert.stuCtrlRecordSetInfo.pBuf = stuCardRec;
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

BOOL CALL_METHOD WRAP_Update_CardRec(int loginID, NET_RECORDSET_ACCESS_CTL_CARDREC* stuCardRec)
{
	if (NULL == loginID)
	{
		return FALSE;
	}

	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
    stuInert.emType = NET_RECORD_ACCESSCTLCARDREC;
	stuInert.pBuf = stuCardRec;
	stuInert.nBufLen = sizeof(NET_RECORDSET_ACCESS_CTL_CARDREC);
	
    BOOL bResult = CLIENT_ControlDevice(loginID, DH_CTRL_RECORDSET_UPDATE, &stuInert, SDK_API_WAITTIME);
    return bResult;
}

BOOL CALL_METHOD WRAP_Remove_CardRec(int loginID, int nRecordNo)
{
	if (NULL == loginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	stuInert.pBuf = &nRecordNo;
	stuInert.nBufLen = sizeof(nRecordNo);
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

BOOL CALL_METHOD WRAP_Get_CardRec_Info(int loginID, int nRecordNo, NET_RECORDSET_ACCESS_CTL_CARDREC* result)
{
	if (NULL == loginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	NET_RECORDSET_ACCESS_CTL_CARDREC stuCardRec = {sizeof(stuCardRec)};
	
	stuCardRec.nRecNo = nRecordNo;
	stuInert.emType = NET_RECORD_ACCESSCTLCARDREC;
	stuInert.pBuf = &stuCardRec;

	int nRet = 0;
	BOOL bRet = CLIENT_QueryDevState(loginID, DH_DEVSTATE_DEV_RECORDSET, (char*)&stuInert, sizeof(stuInert), &nRet, 3000);

	memcpy(result, &stuCardRec, sizeof(stuCardRec));
	return bRet;
}
void WRAP_testRecordSetFind_CardRec(LLONG loginID, LLONG& lFinderId)
{
	NET_IN_FIND_RECORD_PARAM stuIn = {sizeof(stuIn)};
	NET_OUT_FIND_RECORD_PARAM stuOut = {sizeof(stuOut)};
	
	stuIn.emType = NET_RECORD_ACCESSCTLCARDREC;
	
	FIND_RECORD_ACCESSCTLCARDREC_CONDITION stuParam = {sizeof(FIND_RECORD_ACCESSCTLCARDREC_CONDITION)};
	strcpy(stuParam.szCardNo, "987654321");
	stuParam.stStartTime.dwYear = 2013;
	stuParam.stStartTime.dwMonth = 1;
	stuParam.stStartTime.dwDay = 2;
	stuParam.stStartTime.dwHour = 3;
	stuParam.stStartTime.dwMinute = 4;
	stuParam.stStartTime.dwSecond = 5;
	stuParam.stEndTime.dwYear = 2014;
	stuParam.stEndTime.dwMonth = 2;
	stuParam.stEndTime.dwDay = 3;
	stuParam.stEndTime.dwHour = 4;
	stuParam.stEndTime.dwMinute = 5;
	stuParam.stEndTime.dwSecond = 6;
	
	stuIn.pQueryCondition = &stuParam;
	
	if (CLIENT_FindRecord(loginID, &stuIn, &stuOut, SDK_API_WAITTIME))
	{
		lFinderId = stuOut.lFindeHandle;
	}
}

void WRAP_testRecordSetFind_CardRec(LLONG loginID, LLONG& lFinderId, FIND_RECORD_ACCESSCTLCARDREC_CONDITION stuParam)
{
	NET_IN_FIND_RECORD_PARAM stuIn = {sizeof(stuIn)};
	NET_OUT_FIND_RECORD_PARAM stuOut = {sizeof(stuOut)};

	stuIn.emType = NET_RECORD_ACCESSCTLCARDREC;

	stuIn.pQueryCondition = &stuParam;

	if (CLIENT_FindRecord(loginID, &stuIn, &stuOut, SDK_API_WAITTIME))
	{
		lFinderId = stuOut.lFindeHandle;
		printf("WRAP_testRecordSetFind_CardRec ok!\n");
	}
	else
	{
		printf("WRAP_testRecordSetFind_CardRec failed:0x%08x!\n", CLIENT_GetLastError());
	}	
}

int GetCardRecsCountRecordSetFind(LLONG& lFinderId)
{
	NET_IN_QUEYT_RECORD_COUNT_PARAM stuIn = {sizeof(stuIn)};
	NET_OUT_QUEYT_RECORD_COUNT_PARAM stuOut = {sizeof(stuOut)};
	stuIn.lFindeHandle = lFinderId;
	if (CLIENT_QueryRecordCount(&stuIn, &stuOut, SDK_API_WAITTIME))
	{
		return stuOut.nRecordCount;
	}
	else
	{
		return 0;
	}
}

int CALL_METHOD WRAP_Get_CardRecs_Count(int loginID)
{
	if (NULL == loginID)
	{
		return FALSE;
	}
	LLONG lFindID = 0;

	FIND_RECORD_ACCESSCTLCARDREC_CONDITION stuParam = {sizeof(FIND_RECORD_ACCESSCTLCARDREC_CONDITION)};
	strcpy(stuParam.szCardNo, "987654321");
	stuParam.stStartTime.dwYear = 2013;
	stuParam.stStartTime.dwMonth = 1;
	stuParam.stStartTime.dwDay = 2;
	stuParam.stStartTime.dwHour = 3;
	stuParam.stStartTime.dwMinute = 4;
	stuParam.stStartTime.dwSecond = 5;
	stuParam.stEndTime.dwYear = 2014;
	stuParam.stEndTime.dwMonth = 2;
	stuParam.stEndTime.dwDay = 3;
	stuParam.stEndTime.dwHour = 4;
	stuParam.stEndTime.dwMinute = 5;
	stuParam.stEndTime.dwSecond = 6;

	WRAP_testRecordSetFind_CardRec(loginID, lFindID, stuParam);
    if (NULL != lFindID)
    {
		int count = GetCardRecsCountRecordSetFind(lFindID);
		CLIENT_FindRecordClose(lFindID);
		return count;
    }
	return -1;
}

BOOL CALL_METHOD WRAP_GetAll_CardRecs(int loginID, CardRecsCollection* result)
{
	CardRecsCollection cardRecsCollection = {sizeof(CardRecsCollection)};

	LLONG lFinderID = 0;
	WRAP_testRecordSetFind_CardRec(loginID, lFinderID);
	if (lFinderID != 0)
	{
		NET_IN_FIND_NEXT_RECORD_PARAM stuIn = {sizeof(stuIn)};
		stuIn.lFindeHandle = lFinderID;
		stuIn.nFileCount = QUERY_COUNT;
	
		NET_OUT_FIND_NEXT_RECORD_PARAM stuOut = {sizeof(stuOut)};
		stuOut.nMaxRecordNum = stuIn.nFileCount;
	
		NET_RECORDSET_ACCESS_CTL_CARDREC stuCardRec[QUERY_COUNT] = {0};
		for (int i = 0; i < sizeof(stuCardRec)/sizeof(stuCardRec[0]); i++)
		{
			stuCardRec[i].dwSize = sizeof(NET_RECORDSET_ACCESS_CTL_CARDREC);
		}
		stuOut.pRecordList = (void*)&stuCardRec[0];
	
		if (CLIENT_FindNextRecord(&stuIn, &stuOut, SDK_API_WAITTIME) >= 0)
		{
			printf("testRecordSetFindNext_CardRec ok!\n");
		
			for (int j = 0; j < stuOut.nRetRecordNum; j++)
			{
				NET_RECORDSET_ACCESS_CTL_CARDREC* pCardRec = (NET_RECORDSET_ACCESS_CTL_CARDREC*)stuOut.pRecordList;
				memcpy(&cardRecsCollection.CardRecs[j], &pCardRec[j], sizeof(NET_RECORDSET_ACCESS_CTL_CARD));
			}
		}

		CLIENT_FindRecordClose(lFinderID);
	}

	memcpy(result, &cardRecsCollection, sizeof(CardRecsCollection));
	return lFinderID != 0;
}
