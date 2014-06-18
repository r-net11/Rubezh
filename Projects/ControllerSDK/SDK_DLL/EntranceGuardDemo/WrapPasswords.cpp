#include "StdAfx.h"
#include "WrapPasswords.h"

#include <iostream>
#include <fstream>
using namespace std;

#define QUERY_COUNT	(3)

int CALL_METHOD WRAP_Insert_Pwd(int lLoginID, NET_RECORDSET_ACCESS_CTL_PWD* stuAccessCtlPwd)
{
	if (NULL == lLoginID)
	{
		return 0;
	}

	NET_CTRL_RECORDSET_INSERT_PARAM stuInert = {sizeof(stuInert)};
	stuInert.stuCtrlRecordSetInfo.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_IN);
    stuInert.stuCtrlRecordSetInfo.emType = NET_RECORD_ACCESSCTLPWD;
	stuInert.stuCtrlRecordSetInfo.pBuf = stuAccessCtlPwd;
	stuInert.stuCtrlRecordSetInfo.nBufLen = sizeof(NET_RECORDSET_ACCESS_CTL_PWD);
	
	stuInert.stuCtrlRecordSetResult.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_OUT);
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_INSERT, &stuInert, 3000);
	int nRecrdNo = stuInert.stuCtrlRecordSetResult.nRecNo;
	if (bResult)
	{
		return nRecrdNo;
	}
	return 0;
}

BOOL CALL_METHOD WRAP_Update_Pwd(int lLoginID, NET_RECORDSET_ACCESS_CTL_PWD* stuAccessCtlPwd)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
    stuInert.emType = NET_RECORD_ACCESSCTLPWD;
	stuInert.pBuf = stuAccessCtlPwd;
	stuInert.nBufLen = sizeof(NET_RECORDSET_ACCESS_CTL_PWD);
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_UPDATE, &stuInert, SDK_API_WAITTIME);
    return bResult;
}

BOOL CALL_METHOD WRAP_GetPasswordInfo(int lLoginID, int nRecordNo, NET_RECORDSET_ACCESS_CTL_PWD* result)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	NET_RECORDSET_ACCESS_CTL_PWD stuAccessCtlPwd = {sizeof(stuAccessCtlPwd)};

	stuAccessCtlPwd.nRecNo = nRecordNo;
	stuInert.emType = NET_RECORD_ACCESSCTLPWD;
	stuInert.pBuf = &stuAccessCtlPwd;

	int nRet = 0;
	BOOL bRet = CLIENT_QueryDevState(lLoginID, DH_DEVSTATE_DEV_RECORDSET, (char*)&stuInert, sizeof(stuInert), &nRet, 3000);

	memcpy(result, &stuAccessCtlPwd, sizeof(stuAccessCtlPwd));
	return bRet;
}

BOOL CALL_METHOD WRAP_RemovePassword(int lLoginID, int nRecordNo)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	stuInert.pBuf = &nRecordNo;
	stuInert.nBufLen = sizeof(nRecordNo);
	stuInert.emType = NET_RECORD_ACCESSCTLPWD;
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_REMOVE, &stuInert, SDK_API_WAITTIME);
	return bResult;
}

BOOL CALL_METHOD WRAP_RemoveAllPasswords(int lLoginID)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	stuInert.emType = NET_RECORD_ACCESSCTLPWD;
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_CLEAR, &stuInert, SDK_API_WAITTIME);
	return bResult;
}

void WRAP_testRecordSetFind_Pwd(LLONG lLoginId, LLONG& lFinderId, FIND_RECORD_ACCESSCTLPWD_CONDITION stuParam)
{
	NET_IN_FIND_RECORD_PARAM stuIn = {sizeof(stuIn)};
	NET_OUT_FIND_RECORD_PARAM stuOut = {sizeof(stuOut)};
	
	stuIn.emType = NET_RECORD_ACCESSCTLPWD;
	
	stuIn.pQueryCondition = &stuParam;
	
	if (CLIENT_FindRecord(lLoginId, &stuIn, &stuOut, SDK_API_WAITTIME))
	{
		lFinderId = stuOut.lFindeHandle;
		printf("WRAP_testRecordSetFind_Pwd ok!\n");
	}
	else
	{
		printf("WRAP_testRecordSetFind_Pwd failed:0x%08x!\n", CLIENT_GetLastError());
	}
}

void WRAP_testRecordSetFindNext_Pwd(LLONG lFinderId)
{
	int i = 0, j = 0;
	
	NET_IN_FIND_NEXT_RECORD_PARAM stuIn = {sizeof(stuIn)};
	stuIn.lFindeHandle = lFinderId;
	stuIn.nFileCount = QUERY_COUNT;
	
	NET_OUT_FIND_NEXT_RECORD_PARAM stuOut = {sizeof(stuOut)};
	stuOut.nMaxRecordNum = stuIn.nFileCount;
	
	NET_RECORDSET_ACCESS_CTL_PWD stuPwd[QUERY_COUNT] = {0};
	for (i = 0; i < sizeof(stuPwd)/sizeof(stuPwd[0]); i++)
	{
		stuPwd[i].dwSize = sizeof(NET_RECORDSET_ACCESS_CTL_PWD);
	}
	stuOut.pRecordList = (void*)&stuPwd[0];
	
	if (CLIENT_FindNextRecord(&stuIn, &stuOut, SDK_API_WAITTIME) >= 0)
	{
		printf("WRAP_testRecordSetFindNext_Pwd ok!\n");
		
		char szDoorTemp[QUERY_COUNT][MAX_NAME_LEN] = {0};
		for (i = 0; i <  __min(QUERY_COUNT, stuOut.nRetRecordNum); i++)
		{
			NET_RECORDSET_ACCESS_CTL_PWD* pPwd = (NET_RECORDSET_ACCESS_CTL_PWD*)stuOut.pRecordList;
			for (j = 0; j < pPwd[i].nDoorNum; j++)
			{
				sprintf(szDoorTemp[i], "%s%d", szDoorTemp[i], pPwd[i].sznDoors[j]);
			}
		}
		
		for (j = 0; j < stuOut.nRetRecordNum; j++)
		{
			printf("DoorNum:%d, Doors:{%s}, RecNo:%d, AlarmPwd:%s, DoorOpenPwd:%s, UserID:%s\n",
				stuPwd[j].nDoorNum,
				szDoorTemp[j],
				stuPwd[j].nRecNo,
				stuPwd[j].szAlarmPwd,
				stuPwd[j].szDoorOpenPwd,
				stuPwd[j].szUserID);
		}
	}
	else
	{
		printf("WRAP_testRecordSetFindNext_Pwd failed:0x%08x!\n", CLIENT_GetLastError());
	}
}

void ClosePasswordsFinder(LLONG lFinderId)
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

int GetPaswordsCountRecordSetFind(LLONG& lFinderId)
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

int CALL_METHOD WRAP_Get_PasswordsCount(int lLoginID)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	LLONG lFindID = 0;

	FIND_RECORD_ACCESSCTLPWD_CONDITION stuParam = {sizeof(FIND_RECORD_ACCESSCTLPWD_CONDITION)};
	strcpy(stuParam.szUserID, "1357924680");

	WRAP_testRecordSetFind_Pwd(lLoginID, lFindID, stuParam);
    if (NULL != lFindID)
    {
		int count = GetPaswordsCountRecordSetFind(lFindID);
		ClosePasswordsFinder(lFindID);
		return count;
    }
	return -1;
}

BOOL CALL_METHOD WRAP_GetAllPasswords(int lLoginId, PasswordsCollection* result)
{
	PasswordsCollection passwordsCollection = {sizeof(PasswordsCollection)};

	LLONG lFinderID = 0;

	NET_IN_FIND_RECORD_PARAM stuIn = {sizeof(stuIn)};
	NET_OUT_FIND_RECORD_PARAM stuOut = {sizeof(stuOut)};
	
	stuIn.emType = NET_RECORD_ACCESSCTLPWD;
	
	FIND_RECORD_ACCESSCTLPWD_CONDITION stuParam = {sizeof(FIND_RECORD_ACCESSCTLPWD_CONDITION)};
	strcpy(stuParam.szUserID, "1357924680");
	
	stuIn.pQueryCondition = &stuParam;
	
	if (CLIENT_FindRecord(lLoginId, &stuIn, &stuOut, SDK_API_WAITTIME))
	{
		lFinderID = stuOut.lFindeHandle;

		int i = 0, j = 0;
	
		NET_IN_FIND_NEXT_RECORD_PARAM stuIn = {sizeof(stuIn)};
		stuIn.lFindeHandle = lFinderID;
		stuIn.nFileCount = QUERY_COUNT;
	
		NET_OUT_FIND_NEXT_RECORD_PARAM stuOut = {sizeof(stuOut)};
		stuOut.nMaxRecordNum = stuIn.nFileCount;
	
		NET_RECORDSET_ACCESS_CTL_PWD stuPwd[500] = {0};
		for (i = 0; i < sizeof(stuPwd)/sizeof(stuPwd[0]); i++)
		{
			stuPwd[i].dwSize = sizeof(NET_RECORDSET_ACCESS_CTL_PWD);
		}
		stuOut.pRecordList = (void*)&stuPwd[0];
	
		if (CLIENT_FindNextRecord(&stuIn, &stuOut, SDK_API_WAITTIME) >= 0)
		{
			for (i = 0; i <  __min(500, stuOut.nRetRecordNum); i++)
			{
				NET_RECORDSET_ACCESS_CTL_PWD* pPwd = (NET_RECORDSET_ACCESS_CTL_PWD*)stuOut.pRecordList;
				memcpy(&passwordsCollection.Passwords[i], &pPwd[i], sizeof(NET_RECORDSET_ACCESS_CTL_CARD));
			}
		}

		ClosePasswordsFinder(lFinderID);
	}

	memcpy(result, &passwordsCollection, sizeof(PasswordsCollection));
	return lFinderID != 0;
}