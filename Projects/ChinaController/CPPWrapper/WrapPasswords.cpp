#include "StdAfx.h"
#include "WrapPasswords.h"

#include <iostream>
#include <fstream>
using namespace std;

#define QUERY_COUNT	(3)

int CALL_METHOD WRAP_Insert_Password(int loginID, NET_RECORDSET_ACCESS_CTL_PWD* param)
{
	if (NULL == loginID)
	{
		return 0;
	}

	NET_CTRL_RECORDSET_INSERT_PARAM stuInert = {sizeof(stuInert)};
	stuInert.stuCtrlRecordSetInfo.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_IN);
    stuInert.stuCtrlRecordSetInfo.emType = NET_RECORD_ACCESSCTLPWD;
	stuInert.stuCtrlRecordSetInfo.pBuf = param;
	stuInert.stuCtrlRecordSetInfo.nBufLen = sizeof(NET_RECORDSET_ACCESS_CTL_PWD);
	
	stuInert.stuCtrlRecordSetResult.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_OUT);
    BOOL bResult = CLIENT_ControlDevice(loginID, DH_CTRL_RECORDSET_INSERT, &stuInert, 3000);
	int nRecrdNo = stuInert.stuCtrlRecordSetResult.nRecNo;
	if (bResult)
	{
		return nRecrdNo;
	}
	return 0;
}

BOOL CALL_METHOD WRAP_Update_Password(int loginID, NET_RECORDSET_ACCESS_CTL_PWD* param)
{
	if (NULL == loginID)
	{
		return FALSE;
	}
	
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
    stuInert.emType = NET_RECORD_ACCESSCTLPWD;
	stuInert.pBuf = param;
	stuInert.nBufLen = sizeof(NET_RECORDSET_ACCESS_CTL_PWD);
    BOOL bResult = CLIENT_ControlDevice(loginID, DH_CTRL_RECORDSET_UPDATE, &stuInert, SDK_API_WAITTIME);
    return bResult;
}

BOOL CALL_METHOD WRAP_Remove_Password(int loginID, int recordNo)
{
	if (NULL == loginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	stuInert.pBuf = &recordNo;
	stuInert.nBufLen = sizeof(recordNo);
	stuInert.emType = NET_RECORD_ACCESSCTLPWD;
    BOOL bResult = CLIENT_ControlDevice(loginID, DH_CTRL_RECORDSET_REMOVE, &stuInert, SDK_API_WAITTIME);
	return bResult;
}

BOOL CALL_METHOD WRAP_RemoveAll_Passwords(int loginID)
{
	if (NULL == loginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	stuInert.emType = NET_RECORD_ACCESSCTLPWD;
    BOOL bResult = CLIENT_ControlDevice(loginID, DH_CTRL_RECORDSET_CLEAR, &stuInert, SDK_API_WAITTIME);
	return bResult;
}

BOOL CALL_METHOD WRAP_Get_Password_Info(int loginID, int recordNo, NET_RECORDSET_ACCESS_CTL_PWD* result)
{
	if (NULL == loginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	NET_RECORDSET_ACCESS_CTL_PWD stuAccessCtlPwd = {sizeof(stuAccessCtlPwd)};

	stuAccessCtlPwd.nRecNo = recordNo;
	stuInert.emType = NET_RECORD_ACCESSCTLPWD;
	stuInert.pBuf = &stuAccessCtlPwd;

	int nRet = 0;
	BOOL bRet = CLIENT_QueryDevState(loginID, DH_DEVSTATE_DEV_RECORDSET, (char*)&stuInert, sizeof(stuInert), &nRet, 3000);

	memcpy(result, &stuAccessCtlPwd, sizeof(stuAccessCtlPwd));
	return bRet;
}

BOOL CALL_METHOD WRAP_BeginGetAll_Passwords(int loginID, int& finderID)
{
	NET_IN_FIND_RECORD_PARAM stuIn = {sizeof(stuIn)};
	NET_OUT_FIND_RECORD_PARAM stuOut = {sizeof(stuOut)};
	
	stuIn.emType = NET_RECORD_ACCESSCTLPWD;
		
	if (CLIENT_FindRecord(loginID, &stuIn, &stuOut, SDK_API_WAITTIME))
	{
		finderID = stuOut.lFindeHandle;
	}

	return finderID > 0;
}

int CALL_METHOD WRAP_GetAll_Passwords(int finderID, PasswordsCollection* result)
{
	PasswordsCollection passwordsCollection = {sizeof(PasswordsCollection)};

	int i = 0, j = 0;
	int nMaxNum = 10;
	
	NET_IN_FIND_NEXT_RECORD_PARAM stuIn = {sizeof(stuIn)};
	stuIn.lFindeHandle = finderID;
	stuIn.nFileCount = nMaxNum;
	
	NET_OUT_FIND_NEXT_RECORD_PARAM stuOut = {sizeof(stuOut)};
	stuOut.nMaxRecordNum = nMaxNum;
	
	NET_RECORDSET_ACCESS_CTL_PWD* pstuPwd = new NET_RECORDSET_ACCESS_CTL_PWD[nMaxNum];
	if (NULL == pstuPwd)
	{
		return -1;
	}
	memset(pstuPwd, 0, sizeof(NET_RECORDSET_ACCESS_CTL_PWD) * nMaxNum);
	
	for (i = 0; i < nMaxNum; i++)
	{
		pstuPwd[i].dwSize = sizeof(NET_RECORDSET_ACCESS_CTL_PWD);
	}
	stuOut.pRecordList = (void*)pstuPwd;
	
	if (CLIENT_FindNextRecord(&stuIn, &stuOut, SDK_API_WAITTIME) >= 0)
	{
		for (i = 0; i < __min(stuOut.nMaxRecordNum, stuOut.nRetRecordNum); i++)
		{
			memcpy(&passwordsCollection.Passwords[i], &pstuPwd[i], sizeof(NET_RECORDSET_ACCESS_CTL_PWD));
		}
	}
	
	delete[] pstuPwd;
	pstuPwd = NULL;

	memcpy(result, &passwordsCollection, sizeof(PasswordsCollection));
	return stuOut.nRetRecordNum;
}