#include "StdAfx.h"
#include "WrapHolidays.h"

#include <iostream>
#include <fstream>
using namespace std;

#define QUERY_COUNT	(3)

int CALL_METHOD WRAP_Insert_Holiday(int lLoginID, NET_RECORDSET_HOLIDAY* stuHoliday)
{
	if (NULL == lLoginID)
	{
		return 0;
	}
	
	NET_CTRL_RECORDSET_INSERT_PARAM stuInert = {sizeof(stuInert)};
	stuInert.stuCtrlRecordSetInfo.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_IN);
    stuInert.stuCtrlRecordSetInfo.emType = NET_RECORD_ACCESSCTLHOLIDAY;
	stuInert.stuCtrlRecordSetInfo.pBuf = stuHoliday;
	stuInert.stuCtrlRecordSetInfo.nBufLen = sizeof(NET_RECORDSET_HOLIDAY);
	
	stuInert.stuCtrlRecordSetResult.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_OUT);
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_INSERT, &stuInert, 3000);
	int nRecrdNo = stuInert.stuCtrlRecordSetResult.nRecNo;
	if (bResult)
	{
		return nRecrdNo;
	}
	return 0;
}

BOOL CALL_METHOD WRAP_Update_Holiday(int lLoginID, NET_RECORDSET_HOLIDAY* stuHoliday)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}

	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
    stuInert.emType = NET_RECORD_ACCESSCTLHOLIDAY;
	stuInert.pBuf = stuHoliday;
	stuInert.nBufLen = sizeof(NET_RECORDSET_HOLIDAY);
	
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_UPDATE, &stuInert, SDK_API_WAITTIME);
    return bResult;
}

BOOL CALL_METHOD WRAP_GetHolidayInfo(int lLoginID, int nRecordNo, NET_RECORDSET_HOLIDAY* result)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	NET_RECORDSET_HOLIDAY stuHoliday = {sizeof(stuHoliday)};

	stuHoliday.nRecNo = nRecordNo;
	stuInert.emType = NET_RECORD_ACCESSCTLHOLIDAY;
	stuInert.pBuf = &stuHoliday;

	int nRet = 0;
	BOOL bRet = CLIENT_QueryDevState(lLoginID, DH_DEVSTATE_DEV_RECORDSET, (char*)&stuInert, sizeof(stuInert), &nRet, 3000);

	memcpy(result, &stuHoliday, sizeof(stuHoliday));
	return bRet;
}

BOOL CALL_METHOD WRAP_RemoveHoliday(int lLoginID, int nRecordNo)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	stuInert.pBuf = &nRecordNo;
	stuInert.nBufLen = sizeof(nRecordNo);
	stuInert.emType = NET_RECORD_ACCESSCTLHOLIDAY;
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_REMOVE, &stuInert, SDK_API_WAITTIME);
	return bResult;
}

BOOL CALL_METHOD WRAP_RemoveAllHolidays(int lLoginID)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	stuInert.emType = NET_RECORD_ACCESSCTLHOLIDAY;
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_CLEAR, &stuInert, SDK_API_WAITTIME);
	return bResult;
}

void WRAP_testRecordSetFind_Holiday(LLONG lLoginId, LLONG& lFinderId)
{
	NET_IN_FIND_RECORD_PARAM stuIn = {sizeof(stuIn)};
	NET_OUT_FIND_RECORD_PARAM stuOut = {sizeof(stuOut)};
	
	stuIn.emType = NET_RECORD_ACCESSCTLHOLIDAY;

	stuIn.pQueryCondition = NULL;
	
	if (CLIENT_FindRecord(lLoginId, &stuIn, &stuOut, SDK_API_WAITTIME))
	{
		lFinderId = stuOut.lFindeHandle;
		printf("WRAP_testRecordSetFind_Holiday ok!\n");
	}
	else
	{
		printf("WRAP_testRecordSetFind_Holiday failed:0x%08x!\n", CLIENT_GetLastError());
	}		
}

void WRAP_testRecordSetFindNext_Holiday(LLONG lFinderId)
{
	NET_IN_FIND_NEXT_RECORD_PARAM stuIn = {sizeof(stuIn)};
	stuIn.lFindeHandle = lFinderId;
	stuIn.nFileCount = QUERY_COUNT;
	
	NET_OUT_FIND_NEXT_RECORD_PARAM stuOut = {sizeof(stuOut)};
	stuOut.nMaxRecordNum = stuIn.nFileCount;
	
	NET_RECORDSET_HOLIDAY stuHoliday[QUERY_COUNT] = {0};
	for (int i = 0; i < sizeof(stuHoliday)/sizeof(stuHoliday[0]); i++)
	{
		stuHoliday[i].dwSize = sizeof(NET_RECORDSET_HOLIDAY);
	}
	stuOut.pRecordList = (void*)&stuHoliday[0];
	
	if (CLIENT_FindNextRecord(&stuIn, &stuOut, SDK_API_WAITTIME) >= 0)
	{
		printf("WRAP_testRecordSetFindNext_Holiday ok!\n");
		
		for (int j = 0; j < stuOut.nRetRecordNum; j++)
		{
			printf("Enable:%s, RecNo:%d\n",
				stuHoliday[j].bEnable ? "Yes" : "No",
				stuHoliday[j].nRecNo);
		}
	}
	else
	{
		printf("WRAP_testRecordSetFindNext_Holiday failed:0x%08x!\n", CLIENT_GetLastError());
	}		
}

void CloseHolidaysFinder(LLONG lFinderId)
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

int GetHolidaysCountRecordSetFind(LLONG& lFinderId)
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

int CALL_METHOD WRAP_Get_HolidaysCount(int lLoginID)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	LLONG lFindID = 0;
	WRAP_testRecordSetFind_Holiday(lLoginID, lFindID);
    if (NULL != lFindID)
    {
		int count = GetHolidaysCountRecordSetFind(lFindID);
		CloseHolidaysFinder(lFindID);
		return count;
    }
	return -1;
}