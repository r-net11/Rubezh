#include "StdAfx.h"
#include "WrapCardRecs.h"

#include <iostream>
#include <fstream>
using namespace std;

#define QUERY_COUNT	(3)

int CALL_METHOD WRAP_Insert_CardRec(int lLoginID, NET_RECORDSET_ACCESS_CTL_CARDREC* stuCardRec)
{
	if (NULL == lLoginID)
	{
		return 0;
	}

	NET_CTRL_RECORDSET_INSERT_PARAM stuInert = {sizeof(stuInert)};
	stuInert.stuCtrlRecordSetInfo.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_IN);
    stuInert.stuCtrlRecordSetInfo.emType = NET_RECORD_ACCESSCTLCARDREC;
	stuInert.stuCtrlRecordSetInfo.pBuf = stuCardRec;
	stuInert.stuCtrlRecordSetInfo.nBufLen = sizeof(NET_RECORDSET_ACCESS_CTL_CARDREC);
	
	stuInert.stuCtrlRecordSetResult.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_OUT);
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_INSERT, &stuInert, 3000);
	int nRecrdNo = stuInert.stuCtrlRecordSetResult.nRecNo;
	if (bResult)
	{
		return nRecrdNo;
	}
	return 0;
}

BOOL CALL_METHOD WRAP_Update_CardRec(int lLoginID, NET_RECORDSET_ACCESS_CTL_CARDREC* stuCardRec)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}

	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
    stuInert.emType = NET_RECORD_ACCESSCTLCARDREC;
	stuInert.pBuf = stuCardRec;
	stuInert.nBufLen = sizeof(NET_RECORDSET_ACCESS_CTL_CARDREC);
	
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_UPDATE, &stuInert, SDK_API_WAITTIME);
    return bResult;
}

BOOL CALL_METHOD WRAP_GetCardRecInfo(int lLoginID, int nRecordNo, NET_RECORDSET_ACCESS_CTL_CARDREC* result)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	NET_RECORDSET_ACCESS_CTL_CARDREC stuCardRec = {sizeof(stuCardRec)};
	
	stuCardRec.nRecNo = nRecordNo;
	stuInert.emType = NET_RECORD_ACCESSCTLCARDREC;
	stuInert.pBuf = &stuCardRec;

	int nRet = 0;
	BOOL bRet = CLIENT_QueryDevState(lLoginID, DH_DEVSTATE_DEV_RECORDSET, (char*)&stuInert, sizeof(stuInert), &nRet, 3000);

	memcpy(result, &stuCardRec, sizeof(stuCardRec));
	return bRet;
}

BOOL CALL_METHOD WRAP_RemoveCardRec(int lLoginID, int nRecordNo)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	stuInert.pBuf = &nRecordNo;
	stuInert.nBufLen = sizeof(nRecordNo);
	stuInert.emType = NET_RECORD_ACCESSCTLCARDREC;
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_REMOVE, &stuInert, SDK_API_WAITTIME);
	return bResult;
}

BOOL CALL_METHOD WRAP_RemoveAllCardRecs(int lLoginID)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	stuInert.emType = NET_RECORD_ACCESSCTLCARDREC;
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_CLEAR, &stuInert, SDK_API_WAITTIME);
	return bResult;
}

void WRAP_testRecordSetFind_CardRec(LLONG lLoginId, LLONG& lFinderId)
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
	
	if (CLIENT_FindRecord(lLoginId, &stuIn, &stuOut, SDK_API_WAITTIME))
	{
		lFinderId = stuOut.lFindeHandle;
	}
}

BOOL CALL_METHOD WRAP_GetAllCardRecs(int lLoginId, CardRecsCollection* result)
{
	CardRecsCollection cardRecsCollection = {sizeof(CardRecsCollection)};

	LLONG lFinderID = 0;
	WRAP_testRecordSetFind_CardRec(lLoginId, lFinderID);
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

void WRAP_testRecordSetFind_CardRec(LLONG lLoginId, LLONG& lFinderId, FIND_RECORD_ACCESSCTLCARDREC_CONDITION stuParam)
{
	NET_IN_FIND_RECORD_PARAM stuIn = {sizeof(stuIn)};
	NET_OUT_FIND_RECORD_PARAM stuOut = {sizeof(stuOut)};

	stuIn.emType = NET_RECORD_ACCESSCTLCARDREC;

	stuIn.pQueryCondition = &stuParam;

	if (CLIENT_FindRecord(lLoginId, &stuIn, &stuOut, SDK_API_WAITTIME))
	{
		lFinderId = stuOut.lFindeHandle;
		printf("WRAP_testRecordSetFind_CardRec ok!\n");
	}
	else
	{
		printf("WRAP_testRecordSetFind_CardRec failed:0x%08x!\n", CLIENT_GetLastError());
	}	
}

void CloseCardRecsFinder(LLONG lFinderId)
{
	if (CLIENT_FindRecordClose(lFinderId))
	{
		printf("CLIENT_FindRecordClose_Card ok!\n");
	}
	else
	{
		printf("CLIENT_FindRecordClose_Card failed:0x%08x!\n", CLIENT_GetLastError());
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

int CALL_METHOD WRAP_Get_CardRecordsCount(int lLoginID)
{
	if (NULL == lLoginID)
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

	WRAP_testRecordSetFind_CardRec(lLoginID, lFindID, stuParam);
    if (NULL != lFindID)
    {
		int count = GetCardRecsCountRecordSetFind(lFindID);
		CloseCardRecsFinder(lFindID);
		return count;
    }
	return -1;
}

BOOL CALL_METHOD WRAP_GetAllCardRecords(int lLoginId, CardRecordsCollection* result)
{
	CardRecordsCollection cardRecordsCollection = {sizeof(CardRecordsCollection)};

	LLONG lFinderID = 0;

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
	
	if (CLIENT_FindRecord(lLoginId, &stuIn, &stuOut, SDK_API_WAITTIME))
	{
		lFinderID = stuOut.lFindeHandle;
	}

	if (lFinderID != 0)
	{
		NET_IN_FIND_NEXT_RECORD_PARAM stuIn = {sizeof(stuIn)};
		stuIn.lFindeHandle = lFinderID;
		stuIn.nFileCount = QUERY_COUNT;
	
		NET_OUT_FIND_NEXT_RECORD_PARAM stuOut = {sizeof(stuOut)};
		stuOut.nMaxRecordNum = stuIn.nFileCount;
	
		NET_RECORDSET_ACCESS_CTL_CARDREC stuCardRec[500] = {0};
		for (int i = 0; i < sizeof(stuCardRec)/sizeof(stuCardRec[0]); i++)
		{
			stuCardRec[i].dwSize = sizeof(NET_RECORDSET_ACCESS_CTL_CARDREC);
		}
		stuOut.pRecordList = (void*)&stuCardRec[0];
	
		if (CLIENT_FindNextRecord(&stuIn, &stuOut, SDK_API_WAITTIME) >= 0)
		{
			for (int j = 0; j < stuOut.nRetRecordNum; j++)
			{
				memcpy(&cardRecordsCollection.CardRecords[j], &stuCardRec[j], sizeof(NET_RECORDSET_ACCESS_CTL_CARDREC));
			}
		}
		
		CloseCardRecsFinder(lFinderID);
	}
	else
	{
		// testRecordSetFind_CardRec()本身会提示...
	}

	memcpy(result, &cardRecordsCollection, sizeof(CardRecordsCollection));
	return lFinderID != 0;
}