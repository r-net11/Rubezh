/********************************************************************
*	Copyright 2013, ZheJiang Dahua Technology Stock Co.Ltd.
* 						All Rights Reserved
*	File Name: DevCtrl.cpp	    	
*	Author: 
*	Description: PTZ control which used to control ptz absolutely
*	Created:	        %2013%:%12%:%30%  
*	Revision Record:    date:author:modify sth
*********************************************************************/
#include "StdAfx.h"
#include "DevCtrl.h"

int g_nRecordNoCard = -1;
int g_nRecordNoPwd = -1;
int g_nRecordNoCardRec = -1;
int g_nRecordNoHoliday = -1;

#define QUERY_COUNT	(3)

void Insert_Card(LLONG lLoginID, int& nRecrdNo)
{
	if (NULL == lLoginID)
	{
		return ;
	}
	std::string strTemp;
	NET_RECORDSET_ACCESS_CTL_CARD stuCard = {sizeof(stuCard)};
	stuCard.bIsValid = TRUE;
	stuCard.emStatus = NET_ACCESSCTLCARD_STATE_NORMAL;
	stuCard.emType = NET_ACCESSCTLCARD_TYPE_GENERAL;
	stuCard.nTimeSectionNum = 1;
	stuCard.nUserTime = 10;

	stuCard.stuCreateTime.dwYear = 2013;
	stuCard.stuCreateTime.dwMonth = 12;
	stuCard.stuCreateTime.dwDay = 12;
	stuCard.stuCreateTime.dwHour = 11;
	stuCard.stuCreateTime.dwMinute = 30;
	stuCard.stuCreateTime.dwSecond = 30;

	stuCard.stuValidStartTime.dwYear = 2013;
	stuCard.stuValidStartTime.dwMonth = 12;
	stuCard.stuValidStartTime.dwDay = 12;
	stuCard.stuValidStartTime.dwHour = 11;
	stuCard.stuValidStartTime.dwMinute = 30;
	stuCard.stuValidStartTime.dwSecond = 30;
	
	stuCard.stuValidEndTime.dwYear = 2014;
	stuCard.stuValidEndTime.dwMonth = 12;
	stuCard.stuValidEndTime.dwDay = 12;
	stuCard.stuValidEndTime.dwHour = 11;
	stuCard.stuValidEndTime.dwMinute = 30;
	stuCard.stuValidEndTime.dwSecond = 30;
	
	strTemp = "123456";
	memcpy(stuCard.szCardNo, strTemp.c_str(), __min(DH_MAX_CARDNO_LEN,strTemp.length()));
	stuCard.nDoorNum = 2;
	stuCard.sznDoors[0] = 1;
	stuCard.sznDoors[1] = 2;
	stuCard.nTimeSectionNum = 2;
	stuCard.sznTimeSectionNo[0] = 1;
	stuCard.sznTimeSectionNo[1] = 2;
	strTemp = "12345";
	memcpy(stuCard.szPsw, strTemp.c_str(), __min(DH_MAX_CARDPWD_LEN,strTemp.length()));
	strTemp = "123";
	memcpy(stuCard.szUserID, strTemp.c_str(), __min(DH_MAX_USERID_LEN,strTemp.length()));
	
	NET_CTRL_RECORDSET_INSERT_PARAM stuInert = {sizeof(stuInert)};
	stuInert.stuCtrlRecordSetInfo.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_IN);
    stuInert.stuCtrlRecordSetInfo.emType = NET_RECORD_ACCESSCTLCARD;
	stuInert.stuCtrlRecordSetInfo.pBuf = &stuCard;
	stuInert.stuCtrlRecordSetInfo.nBufLen = sizeof(stuCard);
	
	stuInert.stuCtrlRecordSetResult.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_OUT);
	
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_INSERT, &stuInert, 3000);
	nRecrdNo = stuInert.stuCtrlRecordSetResult.nRecNo;
    printf("CLIENT_ControlDevice result: %d, nRecrdNo:%d", bResult, nRecrdNo);
	if (bResult)
	{
		g_nRecordNoCard = nRecrdNo;
	}
	return;
}

void Insert_Pwd(LLONG lLoginID, int& nRecrdNo)
{
	if (NULL == lLoginID)
	{
		return ;
	}
	std::string strTemp;
	NET_RECORDSET_ACCESS_CTL_PWD stuAccessCtlPwd = {sizeof(stuAccessCtlPwd)};
	stuAccessCtlPwd.stuCreateTime.dwYear = 2013;
	stuAccessCtlPwd.stuCreateTime.dwMonth = 12;
	stuAccessCtlPwd.stuCreateTime.dwDay = 12;
	stuAccessCtlPwd.stuCreateTime.dwHour = 11;
	stuAccessCtlPwd.stuCreateTime.dwMinute = 30;
	stuAccessCtlPwd.stuCreateTime.dwSecond = 30;
	//stuAccessCtlPwd.stuCreateTime;
	strTemp = "123456";
	memcpy(stuAccessCtlPwd.szUserID, strTemp.c_str(), __min(DH_MAX_USERID_LEN,strTemp.length()));
	strTemp = "123456";
	memcpy(stuAccessCtlPwd.szDoorOpenPwd, strTemp.c_str(), __min(DH_MAX_CARDPWD_LEN,strTemp.length()));
	strTemp = "123456";
	memcpy(stuAccessCtlPwd.szAlarmPwd, strTemp.c_str(), __min(DH_MAX_CARDPWD_LEN,strTemp.length()));
	stuAccessCtlPwd.nDoorNum = 2;
	stuAccessCtlPwd.sznDoors[0] = 1;
	stuAccessCtlPwd.sznDoors[1] = 2;

	NET_CTRL_RECORDSET_INSERT_PARAM stuInert = {sizeof(stuInert)};
	stuInert.stuCtrlRecordSetInfo.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_IN);
    stuInert.stuCtrlRecordSetInfo.emType = NET_RECORD_ACCESSCTLPWD;
	stuInert.stuCtrlRecordSetInfo.pBuf = &stuAccessCtlPwd;
	stuInert.stuCtrlRecordSetInfo.nBufLen = sizeof(stuAccessCtlPwd);
	
	stuInert.stuCtrlRecordSetResult.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_OUT);
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_INSERT, &stuInert, 3000);
	nRecrdNo = stuInert.stuCtrlRecordSetResult.nRecNo;
    printf("CLIENT_ControlDevice result: %d, nRecrdNo:%d", bResult, nRecrdNo);
	if (bResult)
	{
		g_nRecordNoPwd = nRecrdNo;
	}
	return;
}

void Insert_CardRec(LLONG lLoginID, int& nRecrdNo)
{
	if (NULL == lLoginID)
	{
		return ;
	}
	std::string strTemp;
	NET_RECORDSET_ACCESS_CTL_CARDREC stuCardRec = {sizeof(stuCardRec)};
	strTemp = "123";
	memcpy(stuCardRec.szCardNo, strTemp.c_str(), __min(DH_MAX_CARDNO_LEN,strTemp.length()));
	strTemp = "123456";
	memcpy(stuCardRec.szPwd, strTemp.c_str(), __min(DH_MAX_CARDPWD_LEN,strTemp.length()));
	
	stuCardRec.stuTime.dwYear = 2013;
	stuCardRec.stuTime.dwMonth = 12;
	stuCardRec.stuTime.dwDay = 12;
	stuCardRec.stuTime.dwHour = 11;
	stuCardRec.stuTime.dwMinute = 30;
	stuCardRec.stuTime.dwSecond = 30;

	stuCardRec.bStatus = TRUE;
	stuCardRec.emMethod = NET_ACCESS_DOOROPEN_METHOD_CARD;
	stuCardRec.nDoor = 1;

	NET_CTRL_RECORDSET_INSERT_PARAM stuInert = {sizeof(stuInert)};
	stuInert.stuCtrlRecordSetInfo.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_IN);
    stuInert.stuCtrlRecordSetInfo.emType = NET_RECORD_ACCESSCTLCARDREC;
	stuInert.stuCtrlRecordSetInfo.pBuf = &stuCardRec;
	stuInert.stuCtrlRecordSetInfo.nBufLen = sizeof(stuCardRec);
	
	stuInert.stuCtrlRecordSetResult.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_OUT);
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_INSERT, &stuInert, 3000);
	nRecrdNo = stuInert.stuCtrlRecordSetResult.nRecNo;
    printf("CLIENT_ControlDevice result: %d, nRecrdNo:%d", bResult, nRecrdNo);
	if (bResult)
	{
		g_nRecordNoCardRec = nRecrdNo;
	}
	return;
}
void Insert_Holiday(LLONG lLoginID, int& nRecrdNo)
{
	if (NULL == lLoginID)
	{
		return ;
	}
	NET_RECORDSET_HOLIDAY stuHoliday = {sizeof(stuHoliday)};
	stuHoliday.nDoorNum = 2;
	stuHoliday.sznDoors[0] = 1;
	stuHoliday.sznDoors[1] = 2;

	stuHoliday.stuStartTime.dwYear = 2013;
	stuHoliday.stuStartTime.dwMonth = 10;
	stuHoliday.stuStartTime.dwDay = 1;
	stuHoliday.stuStartTime.dwHour = 0;
	stuHoliday.stuStartTime.dwMinute = 0;
	stuHoliday.stuStartTime.dwSecond = 0;
	
	stuHoliday.stuEndTime.dwYear = 2013;
	stuHoliday.stuEndTime.dwMonth = 10;
	stuHoliday.stuEndTime.dwDay = 7;
	stuHoliday.stuEndTime.dwHour = 0;
	stuHoliday.stuEndTime.dwMinute = 0;
	stuHoliday.stuEndTime.dwSecond = 0;
	stuHoliday.bEnable = TRUE;
	
	NET_CTRL_RECORDSET_INSERT_PARAM stuInert = {sizeof(stuInert)};
	stuInert.stuCtrlRecordSetInfo.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_IN);
    stuInert.stuCtrlRecordSetInfo.emType = NET_RECORD_ACCESSCTLHOLIDAY;
	stuInert.stuCtrlRecordSetInfo.pBuf = &stuHoliday;
	stuInert.stuCtrlRecordSetInfo.nBufLen = sizeof(stuHoliday);
	
	stuInert.stuCtrlRecordSetResult.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_OUT);
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_INSERT, &stuInert, 3000);
	nRecrdNo = stuInert.stuCtrlRecordSetResult.nRecNo;
    printf("CLIENT_ControlDevice result: %d, nRecrdNo:%d", bResult, nRecrdNo);
	if (bResult)
	{
		g_nRecordNoHoliday = nRecrdNo;
	}
	return;
}

void Update_Card(LLONG lLoginID)
{
	if (NULL == lLoginID)
	{
		return;
	}
	std::string strTemp;
	NET_RECORDSET_ACCESS_CTL_CARD stuCard = {sizeof(stuCard)};
	stuCard.nRecNo = 12;
	stuCard.bIsValid = TRUE;
	stuCard.emStatus = NET_ACCESSCTLCARD_STATE_NORMAL;
	stuCard.emType = NET_ACCESSCTLCARD_TYPE_GENERAL;
	stuCard.nTimeSectionNum = 1;
	stuCard.nUserTime = 10;
	stuCard.stuValidStartTime.dwYear = 2013;
	stuCard.stuValidStartTime.dwMonth = 12;
	stuCard.stuValidStartTime.dwDay = 12;
	stuCard.stuValidStartTime.dwHour = 11;
	stuCard.stuValidStartTime.dwMinute = 30;
	stuCard.stuValidStartTime.dwSecond = 30;
	
	stuCard.stuValidEndTime.dwYear = 2014;
	stuCard.stuValidEndTime.dwMonth = 12;
	stuCard.stuValidEndTime.dwDay = 12;
	stuCard.stuValidEndTime.dwHour = 11;
	stuCard.stuValidEndTime.dwMinute = 30;
	stuCard.stuValidEndTime.dwSecond = 30;
	
	strTemp = "123456";
	memcpy(stuCard.szCardNo, strTemp.c_str(), __min(DH_MAX_CARDNO_LEN,strTemp.length()));
	stuCard.nDoorNum = 2;
	stuCard.sznDoors[0] = 1;
	stuCard.sznDoors[1] = 2;
	stuCard.nTimeSectionNum = 2;
	stuCard.sznTimeSectionNo[0] = 1;
	stuCard.sznTimeSectionNo[1] = 2;
	strTemp = "123456";
	memcpy(stuCard.szPsw, strTemp.c_str(), __min(DH_MAX_CARDPWD_LEN,strTemp.length()));
	strTemp = "123";
	memcpy(stuCard.szUserID, strTemp.c_str(), __min(DH_MAX_USERID_LEN,strTemp.length()));
	
	stuCard.nRecNo = g_nRecordNoCard < 0 ? 12 : g_nRecordNoCard;
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
    stuInert.emType = NET_RECORD_ACCESSCTLCARD;
	stuInert.pBuf = &stuCard;
	stuInert.nBufLen = sizeof(stuCard);
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_UPDATE, &stuInert, SDK_API_WAITTIME);
    printf("CLIENT_ControlDevice Update result: %d", bResult);
}

void Update_Pwd(LLONG lLoginID)
{
	if (NULL == lLoginID)
	{
		return;
	}
	std::string strTemp;
	NET_RECORDSET_ACCESS_CTL_PWD stuAccessCtlPwd = {sizeof(stuAccessCtlPwd)};
	
	stuAccessCtlPwd.stuCreateTime.dwYear = 2013;
	stuAccessCtlPwd.stuCreateTime.dwMonth = 12;
	stuAccessCtlPwd.stuCreateTime.dwDay = 12;
	stuAccessCtlPwd.stuCreateTime.dwHour = 11;
	stuAccessCtlPwd.stuCreateTime.dwMinute = 30;
	stuAccessCtlPwd.stuCreateTime.dwSecond = 30;
	//stuAccessCtlPwd.stuCreateTime;
	strTemp = "123456";
	memcpy(stuAccessCtlPwd.szUserID, strTemp.c_str(), __min(DH_MAX_USERID_LEN,strTemp.length()));
	strTemp = "123456";
	memcpy(stuAccessCtlPwd.szDoorOpenPwd, strTemp.c_str(), __min(DH_MAX_CARDPWD_LEN,strTemp.length()));
	strTemp = "123456";
	memcpy(stuAccessCtlPwd.szAlarmPwd, strTemp.c_str(), __min(DH_MAX_CARDPWD_LEN,strTemp.length()));
	stuAccessCtlPwd.nDoorNum = 2;
	stuAccessCtlPwd.sznDoors[0] = 1;
	stuAccessCtlPwd.sznDoors[1] = 2;
	
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
    stuInert.emType = NET_RECORD_ACCESSCTLPWD;
	stuInert.pBuf = &stuAccessCtlPwd;
	stuInert.nBufLen = sizeof(stuAccessCtlPwd);
	stuAccessCtlPwd.nRecNo = g_nRecordNoPwd < 0 ? 12 : g_nRecordNoPwd;
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_UPDATE, &stuInert, SDK_API_WAITTIME);
    printf("CLIENT_ControlDevice Update result: %d", bResult);
}

void Update_CardRec(LLONG lLoginID)
{
	if (NULL == lLoginID)
	{
		return;
	}
	std::string strTemp;
	NET_RECORDSET_ACCESS_CTL_CARDREC stuCardRec = {sizeof(stuCardRec)};
	
	strTemp = "123";
	memcpy(stuCardRec.szCardNo, strTemp.c_str(), __min(DH_MAX_CARDNO_LEN,strTemp.length()));
	strTemp = "123456";
	memcpy(stuCardRec.szPwd, strTemp.c_str(), __min(DH_MAX_CARDPWD_LEN,strTemp.length()));
	
	stuCardRec.stuTime.dwYear = 2013;
	stuCardRec.stuTime.dwMonth = 12;
	stuCardRec.stuTime.dwDay = 12;
	stuCardRec.stuTime.dwHour = 11;
	stuCardRec.stuTime.dwMinute = 30;
	stuCardRec.stuTime.dwSecond = 30;
	
	stuCardRec.bStatus = TRUE;
	stuCardRec.emMethod = NET_ACCESS_DOOROPEN_METHOD_CARD;
	stuCardRec.nDoor = 1;
	stuCardRec.nRecNo = g_nRecordNoCardRec < 0 ? 12 : g_nRecordNoCardRec;
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
    stuInert.emType = NET_RECORD_ACCESSCTLCARDREC;
	stuInert.pBuf = &stuCardRec;
	stuInert.nBufLen = sizeof(stuCardRec);
	
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_UPDATE, &stuInert, SDK_API_WAITTIME);
    printf("CLIENT_ControlDevice Update result: %d", bResult);
}

void Update_Holiday(LLONG lLoginID)
{
	if (NULL == lLoginID)
	{
		return;
	}
	NET_RECORDSET_HOLIDAY stuHoliday = {sizeof(stuHoliday)};
	
	stuHoliday.nDoorNum = 2;
	stuHoliday.sznDoors[0] = 1;
	stuHoliday.sznDoors[1] = 2;
	
	stuHoliday.stuStartTime.dwYear = 2013;
	stuHoliday.stuStartTime.dwMonth = 10;
	stuHoliday.stuStartTime.dwDay = 1;
	stuHoliday.stuStartTime.dwHour = 0;
	stuHoliday.stuStartTime.dwMinute = 0;
	stuHoliday.stuStartTime.dwSecond = 0;
	
	stuHoliday.stuEndTime.dwYear = 2013;
	stuHoliday.stuEndTime.dwMonth = 10;
	stuHoliday.stuEndTime.dwDay = 7;
	stuHoliday.stuEndTime.dwHour = 0;
	stuHoliday.stuEndTime.dwMinute = 0;
	stuHoliday.stuEndTime.dwSecond = 0;
	stuHoliday.bEnable = TRUE;
	stuHoliday.nRecNo = g_nRecordNoHoliday < 0 ? 12 : g_nRecordNoHoliday;
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
    stuInert.emType = NET_RECORD_ACCESSCTLHOLIDAY;
	stuInert.pBuf = &stuHoliday;
	stuInert.nBufLen = sizeof(stuHoliday);
	
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_UPDATE, &stuInert, SDK_API_WAITTIME);
    printf("CLIENT_ControlDevice Update result: %d", bResult);
}

BOOL DevCtrl_OpenDoor(LLONG lLoginID)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	NET_CTRL_ACCESS_OPEN stuInert = {sizeof(stuInert)};
	stuInert.nChannelID = 0;
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_ACCESS_OPEN, &stuInert, SDK_API_WAITTIME);
    printf("CLIENT_ControlDevice OpenDoor result: %d", bResult);
	return TRUE;
}

BOOL DevCtrl_ReBoot(LLONG lLoginID)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_REBOOT, NULL, SDK_API_WAITTIME);
    printf("CLIENT_ControlDevice RebootDevice result: %d", bResult);
	return TRUE;
}

BOOL DevCtrl_DeleteCfgFile(LLONG lLoginID)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RESTOREDEFAULT, NULL, SDK_API_WAITTIME);
    printf("CLIENT_ControlDevice RestoreConfig result: %d", bResult);
	return TRUE;
}

BOOL DevCtrl_GetLogCount(LLONG lLoginID)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	NET_IN_GETCOUNT_LOG_PARAM stuInLogCount = {sizeof(NET_IN_GETCOUNT_LOG_PARAM)};
	NET_OUT_GETCOUNT_LOG_PARAM stuOutLogCount = {sizeof(NET_OUT_GETCOUNT_LOG_PARAM)};

	QUERY_DEVICE_LOG_PARAM& stuGetLog = stuInLogCount.stuQueryCondition;
	//stuGetLog.emLogType = DHLOG_ALARM;
	stuGetLog.stuStartTime.dwYear = 2013;
	stuGetLog.stuStartTime.dwMonth = 10;
	stuGetLog.stuStartTime.dwDay = 1;
	stuGetLog.stuStartTime.dwHour = 0;
	stuGetLog.stuStartTime.dwMinute = 0;
	stuGetLog.stuStartTime.dwSecond = 0;
	
	stuGetLog.stuEndTime.dwYear = 2013;
	stuGetLog.stuEndTime.dwMonth = 10;
	stuGetLog.stuEndTime.dwDay = 7;
	stuGetLog.stuEndTime.dwHour = 0;
	stuGetLog.stuEndTime.dwMinute = 0;
	stuGetLog.stuEndTime.dwSecond = 0;

	stuGetLog.nStartNum = 10;
	stuGetLog.nLogStuType = 1;
	stuGetLog.nEndNum = 20;
	
	BOOL bResult = CLIENT_QueryDevLogCount(lLoginID, &stuInLogCount, &stuOutLogCount, SDK_API_WAITTIME);
    printf(" CLIENT_QueryDevLogCount result: %d\n stuOutLogCount: %d\n", bResult, stuOutLogCount.nLogCount);
 	return TRUE;
}
// 新增成功返回记录编号,失败返回-1
int DevCtrl_InsertRecordSet(LLONG lLoginID, int nRecordSetType)
{
	int nRecordNo = -1;
	if (NULL == lLoginID)
	{
		return nRecordNo;
	}
	switch (nRecordSetType)
	{
	case 1:
		Insert_Card(lLoginID, nRecordNo);
		break;
	case 2:
		Insert_Pwd(lLoginID, nRecordNo);
		break;
	case 3:
		Insert_CardRec(lLoginID, nRecordNo);
		break;
	case 4:
		Insert_Holiday(lLoginID, nRecordNo);
		break;
	default:
		break;
	}
	return nRecordNo;
}

BOOL DevCtrl_UpdateRecordSet(LLONG lLoginID, int nRecordSetType)
{
	int nRecordNo = -1;
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	switch (nRecordSetType)
	{
	case 1:
		Update_Card(lLoginID);
		break;
	case 2:
		Update_Pwd(lLoginID);
		break;
	case 3:
		Update_CardRec(lLoginID);
		break;
	case 4:
		Update_Holiday(lLoginID);
		break;
	default:
		break;
	}
	return TRUE;
}

BOOL DevCtrl_RemoveRecordSet(LLONG lLoginID, int nRecordSetType)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	int nRecordNo = 123;
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	stuInert.pBuf = &nRecordNo;
	stuInert.nBufLen = sizeof(nRecordNo);
	switch (nRecordSetType)
	{
	case 1:
		stuInert.emType = NET_RECORD_ACCESSCTLCARD;
		break;
	case 2:
		stuInert.emType = NET_RECORD_ACCESSCTLPWD;
		break;
	case 3:
		stuInert.emType = NET_RECORD_ACCESSCTLCARDREC;
		break;
	case 4:
		stuInert.emType = NET_RECORD_ACCESSCTLHOLIDAY;
		break;
	default:
		break;
	}
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_REMOVE, &stuInert, SDK_API_WAITTIME);
    printf("CLIENT_ControlDevice Remove result: %d", bResult);
	return TRUE;
}

BOOL DevCtrl_ClearRecordSet(LLONG lLoginID, int nRecordSetType)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	switch (nRecordSetType)
	{
	case 1:
		stuInert.emType = NET_RECORD_ACCESSCTLCARD;
		break;
	case 2:
		stuInert.emType = NET_RECORD_ACCESSCTLPWD;
		break;
	case 3:
		stuInert.emType = NET_RECORD_ACCESSCTLCARDREC;
		break;
	case 4:
		stuInert.emType = NET_RECORD_ACCESSCTLHOLIDAY;
		break;
	default:
		break;
	}
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_CLEAR, &stuInert, SDK_API_WAITTIME);
    printf("CLIENT_ControlDevice Clear result: %d", bResult);
	return TRUE;
}

//////////////////////////////////////////////////////////////////////////
//
// card
//
//////////////////////////////////////////////////////////////////////////
void testRecordSetFind_Card(LLONG lLoginId, LLONG& lFinderId)
{
	NET_IN_FIND_RECORD_PARAM stuIn = {sizeof(stuIn)};
	NET_OUT_FIND_RECORD_PARAM stuOut = {sizeof(stuOut)};
	
	stuIn.emType = NET_RECORD_ACCESSCTLCARD;
	
	FIND_RECORD_ACCESSCTLCARD_CONDITION stuParam = {sizeof(stuParam)};
	stuParam.bIsValid = TRUE;
	strcpy(stuParam.szCardNo, "123456");
	strcpy(stuParam.szUserID, "4567890");
	
	
	stuIn.pQueryCondition = &stuParam;
	
	if (CLIENT_FindRecord(lLoginId, &stuIn, &stuOut, SDK_API_WAITTIME))
	{
		lFinderId = stuOut.lFindeHandle;
		printf("CLIENT_FindRecord_Card ok!");
	}
	else
	{
		printf("CLIENT_FindRecord_Card failed:0x%08x!", CLIENT_GetLastError());
	}
}

void testRecordSetFindNext_Card(LLONG lFinderId)
{
	int i = 0, j = 0;
	
	NET_IN_FIND_NEXT_RECORD_PARAM stuIn = {sizeof(stuIn)};
	stuIn.lFindeHandle = lFinderId;
	stuIn.nFileCount = QUERY_COUNT;
	
	NET_OUT_FIND_NEXT_RECORD_PARAM stuOut = {sizeof(stuOut)};
	stuOut.nMaxRecordNum = stuIn.nFileCount;
	
	NET_RECORDSET_ACCESS_CTL_CARD stuCard[QUERY_COUNT] = {0};
	for (i = 0; i < sizeof(stuCard)/sizeof(stuCard[0]); i++)
	{
		stuCard[i].dwSize = sizeof(NET_RECORDSET_ACCESS_CTL_CARD);
	}
	stuOut.pRecordList = (void*)&stuCard[0];
	
	if (CLIENT_FindNextRecord(&stuIn, &stuOut, SDK_API_WAITTIME) >= 0)
	{
		printf("CLIENT_FindNextRecord_Card ok!\n");
		
		char szDoorTemp[QUERY_COUNT][MAX_NAME_LEN] = {0};
		for (i = 0; i <  __min(QUERY_COUNT, stuOut.nRetRecordNum); i++)
		{
			NET_RECORDSET_ACCESS_CTL_CARD* pCard = (NET_RECORDSET_ACCESS_CTL_CARD*)stuOut.pRecordList;
			for (j = 0; j < pCard[i].nDoorNum; j++)
			{
				sprintf(szDoorTemp[i], "%s%d", szDoorTemp[i], pCard[i].sznDoors[j]);
			}
		}
		
		for (j = 0; j < __min(stuOut.nMaxRecordNum, stuOut.nRetRecordNum); j++)
		{
			printf("IsValid:%s, Status:%d, Type:%d, DoorNum:%d, Doors:{%s}, RecNo:%d, TimeSectionNum:%d, UserTimes:%d, CardNo:%s, Pwd:%s, UserID:%s\n", 
				stuCard[j].bIsValid ? "Yes" : "No",
				stuCard[j].emStatus,
				stuCard[j].emType,
				stuCard[j].nDoorNum,
				szDoorTemp[j],
				stuCard[j].nRecNo,
				stuCard[j].nTimeSectionNum,
				stuCard[j].nUserTime,
				stuCard[j].szCardNo,
				stuCard[j].szPsw,
				stuCard[j].szUserID);
		}
	}
	else
	{
		printf("CLIENT_FindNextRecord_Card failed:0x%08x!\n", CLIENT_GetLastError());
	}
}
//////////////////////////////////////////////////////////////////////////
//
// Pwd
//
//////////////////////////////////////////////////////////////////////////
void testRecordSetFind_Pwd(LLONG lLoginId, LLONG& lFinderId)
{
	NET_IN_FIND_RECORD_PARAM stuIn = {sizeof(stuIn)};
	NET_OUT_FIND_RECORD_PARAM stuOut = {sizeof(stuOut)};
	
	stuIn.emType = NET_RECORD_ACCESSCTLPWD;
	
	FIND_RECORD_ACCESSCTLPWD_CONDITION stuParam = {sizeof(FIND_RECORD_ACCESSCTLPWD_CONDITION)};
	strcpy(stuParam.szUserID, "1357924680");
	
	stuIn.pQueryCondition = &stuParam;
	
	if (CLIENT_FindRecord(lLoginId, &stuIn, &stuOut, SDK_API_WAITTIME))
	{
		lFinderId = stuOut.lFindeHandle;
		printf("testRecordSetFind_Pwd ok!\n");
	}
	else
	{
		printf("testRecordSetFind_Pwd failed:0x%08x!\n", CLIENT_GetLastError());
	}
}

void testRecordSetFindNext_Pwd(LLONG lFinderId)
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
		printf("testRecordSetFindNext_Pwd ok!\n");
		
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
		printf("testRecordSetFindNext_Pwd failed:0x%08x!\n", CLIENT_GetLastError());
	}
}
//////////////////////////////////////////////////////////////////////////
//
// Rec
//
//////////////////////////////////////////////////////////////////////////
void testRecordSetFind_CardRec(LLONG lLoginId, LLONG& lFinderId)
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
		printf("testRecordSetFind_CardRec ok!\n");
	}
	else
	{
		printf("testRecordSetFind_CardRec failed:0x%08x!\n", CLIENT_GetLastError());
	}
}

void testRecordSetFindNext_CardRec(LLONG lFinderId)
{
	NET_IN_FIND_NEXT_RECORD_PARAM stuIn = {sizeof(stuIn)};
	stuIn.lFindeHandle = lFinderId;
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
			printf("Status:%s, Method:%d, RecNo:%d, CardNo:%s, Pwd:%s\n",
				stuCardRec[j].bStatus ? "ok" : "fail",
				stuCardRec[j].emMethod,
				stuCardRec[j].nRecNo,
				stuCardRec[j].szCardNo,
				stuCardRec[j].szPwd);
		}
	}
	else
	{
		printf("testRecordSetFindNext_CardRec failed:0x%08x!\n", CLIENT_GetLastError());
	}	
}
//////////////////////////////////////////////////////////////////////////
//
// Holiday
//
//////////////////////////////////////////////////////////////////////////
void testRecordSetFind_Holiday(LLONG lLoginId, LLONG& lFinderId)
{
	NET_IN_FIND_RECORD_PARAM stuIn = {sizeof(stuIn)};
	NET_OUT_FIND_RECORD_PARAM stuOut = {sizeof(stuOut)};
	
	stuIn.emType = NET_RECORD_ACCESSCTLHOLIDAY;
	
	// 目前暂无查询条件
	
	stuIn.pQueryCondition = NULL;
	
	if (CLIENT_FindRecord(lLoginId, &stuIn, &stuOut, SDK_API_WAITTIME))
	{
		lFinderId = stuOut.lFindeHandle;
		printf("testRecordSetFind_Holiday ok!\n");
	}
	else
	{
		printf("testRecordSetFind_Holiday failed:0x%08x!\n", CLIENT_GetLastError());
	}		
}

void testRecordSetFindNext_Holiday(LLONG lFinderId)
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
		printf("testRecordSetFindNext_Holiday ok!\n");
		
		for (int j = 0; j < stuOut.nRetRecordNum; j++)
		{
			printf("Enable:%s, RecNo:%d\n",
				stuHoliday[j].bEnable ? "Yes" : "No",
				stuHoliday[j].nRecNo);
		}
	}
	else
	{
		printf("testRecordSetFindNext_Holiday failed:0x%08x!\n", CLIENT_GetLastError());
	}		
}

void test_GetCountRecordSetFind(LLONG& lFinderId)
{
	NET_IN_QUEYT_RECORD_COUNT_PARAM stuIn = {sizeof(stuIn)};
	NET_OUT_QUEYT_RECORD_COUNT_PARAM stuOut = {sizeof(stuOut)};
	stuIn.lFindeHandle = lFinderId;
	if (CLIENT_QueryRecordCount(&stuIn, &stuOut, SDK_API_WAITTIME))
	{
		printf("CLIENT_QueryRecordCount ok!, stuOut.nRecordCount: %d\n", stuOut.nRecordCount);
	}
	else
	{
		printf("CLIENT_QueryRecordCount failed:0x%08x!\n", CLIENT_GetLastError());
	}
}

//////////////////////////////////////////////////////////////////////////
//
// closeFind
//
//////////////////////////////////////////////////////////////////////////
void test_RecordSetFindClose(LLONG lFinderId)
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

void testRecordSetFinder_Card(LLONG lLoginId)
{
	LLONG lFinderID = 0;
	testRecordSetFind_Card(lLoginId, lFinderID);
	if (lFinderID != 0)
	{
		testRecordSetFindNext_Card(lFinderID);
		test_RecordSetFindClose(lFinderID);
	}
	else
	{
		// testRecordSetFind_Card()本身会提示...
	}
}

void testRecordSetFinder_Pwd(LLONG lLoginId)
{
	LLONG lFinderID = 0;
	testRecordSetFind_Pwd(lLoginId, lFinderID);
	if (lFinderID != 0)
	{
		testRecordSetFindNext_Pwd(lFinderID);
		test_RecordSetFindClose(lFinderID);
	}
	else
	{
		// testRecordSetFind_Pwd()本身会提示...
	}	
}

void testRecordSetFinder_CardRec(LLONG lLoginId)
{
	LLONG lFinderID = 0;
	testRecordSetFind_CardRec(lLoginId, lFinderID);
	if (lFinderID != 0)
	{
		testRecordSetFindNext_CardRec(lFinderID);
		test_RecordSetFindClose(lFinderID);
	}
	else
	{
		// testRecordSetFind_CardRec()本身会提示...
	}	
}

void testRecordSetFinder_Holiday(LLONG lLoginId)
{
	// 暂不支持
// 	LLONG lFinderID = 0;
// 	testRecordSetFind_Holiday(lLoginId, lFinderID);
// 	if (lFinderID != 0)
// 	{
// 		testRecordSetFindNext_Holiday(lFinderID);
// 		test_RecordSetFindClose(lFinderID);
// 	}
// 	else
// 	{
// 		// testRecordSetFind_Holiday()本身会提示...
// 	}	
}

BOOL DevCtrl_GetRecordSetCount(LLONG lLoginID, int nRecordSetType)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	LLONG lFindID = 0;
	switch (nRecordSetType)
	{
	case 1:
		testRecordSetFind_Card(lLoginID, lFindID);
		break;
	case 2:
		testRecordSetFind_Pwd(lLoginID, lFindID);
		break;
	case 3:
		testRecordSetFind_CardRec(lLoginID, lFindID);
		break;
	case 4:
		testRecordSetFind_Holiday(lLoginID, lFindID);
		break;
	default:
		break;
	}
    if (NULL != lFindID)
    {
		int nInput = -1;
		do 
		{
			test_GetCountRecordSetFind(lFindID);
			printf("===请选择: 0.退出; 其他-再次查询该记录数===\n");
			std::cin >> nInput;
		} while (0 != nInput);
		test_RecordSetFindClose(lFindID);
    }
	return TRUE;
}

BOOL DevCtrl_GetRecordSetInfo(LLONG lLoginID, int nRecordSetType)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	NET_RECORDSET_ACCESS_CTL_CARD stuCard = {sizeof(stuCard)};
	NET_RECORDSET_ACCESS_CTL_PWD stuAccessCtlPwd = {sizeof(stuAccessCtlPwd)};
	NET_RECORDSET_ACCESS_CTL_CARDREC stuCardRec = {sizeof(stuCardRec)};
	NET_RECORDSET_HOLIDAY stuHoliday = {sizeof(stuHoliday)};
	switch (nRecordSetType)
	{
	case 1:
		stuCard.nRecNo = g_nRecordNoCard < 0 ? 12 : g_nRecordNoCard;
		stuInert.emType = NET_RECORD_ACCESSCTLCARD;
		stuInert.pBuf = &stuCard;
		break;
	case 2:
		stuAccessCtlPwd.nRecNo = g_nRecordNoPwd < 0 ? 12 : g_nRecordNoPwd;
		stuInert.emType = NET_RECORD_ACCESSCTLPWD;
		stuInert.pBuf = &stuAccessCtlPwd;
		break;
	case 3:
		stuCardRec.nRecNo = g_nRecordNoCardRec < 0 ? 12 : g_nRecordNoCardRec;
		stuInert.emType = NET_RECORD_ACCESSCTLCARDREC;
		stuInert.pBuf = &stuCardRec;
		break;
	case 4:
		stuHoliday.nRecNo = g_nRecordNoHoliday < 0 ? 12 : g_nRecordNoHoliday;
		stuInert.emType = NET_RECORD_ACCESSCTLHOLIDAY;
		stuInert.pBuf = &stuHoliday;
		break;
	default:
		break;
	}
	// 获取
	int nRet = 0;
	BOOL bRet = CLIENT_QueryDevState(lLoginID, DH_DEVSTATE_DEV_RECORDSET, (char*)&stuInert, sizeof(stuInert), &nRet, 3000);
	if (bRet)
	{
		printf("CLIENT_QueryDevState(DH_DEVSTATE_DEV_RECORDSET)成功\n");
		switch (nRecordSetType)
		{
		 	case 1:
				ShowCardInfo(stuCard);
		 		break;
		 	case 2:
				ShowPwdInfo(stuAccessCtlPwd);
		 		break;
		 	case 3:
				ShowCardRecInfo(stuCardRec);
		 		break;
		 	case 4:
				ShowHolidayInfo(stuHoliday);
		 		break;
		 	default:
		 		break;
		}
	}
	else
	{
		printf("CLIENT_QueryDevState(DH_DEVSTATE_DEV_RECORDSET)失败\n");
	}

	return TRUE;
}

void ShowCardInfo(NET_RECORDSET_ACCESS_CTL_CARD& stuCard)
{
	unsigned int i = 0;
	printf("===门禁卡信息:\n");
	printf("stuCard.nRecNo:%d\n", stuCard.nRecNo);
	printf("stuCard.stuCreateTime: %04d-%02d-%02d %02d:%02d:%02d\n", 
		stuCard.stuCreateTime.dwYear,
		stuCard.stuCreateTime.dwMonth,
		stuCard.stuCreateTime.dwDay,
		stuCard.stuCreateTime.dwHour,
		stuCard.stuCreateTime.dwMinute,
		stuCard.stuCreateTime.dwSecond);
	printf("stuCard.szCardNo:%s\n", stuCard.szCardNo);
	printf("stuCard.szUserID:%s\n", stuCard.szUserID);
	printf("stuCard.emStatus:%d\n", stuCard.emStatus);
	printf("stuCard.emType:%d\n", stuCard.emType);
	printf("stuCard.szPsw:%s\n", stuCard.szPsw);
	printf("stuCard.nDoorNum:%d\n", stuCard.nDoorNum);
	for (i = 0; i < stuCard.nDoorNum; i++)
	{
		printf("stuCard.nDoorNum[%d]:%d\n", i, stuCard.sznDoors[i]);
	}
	printf("stuCard.nTimeSectionNum:%d\n", stuCard.nTimeSectionNum);
	for (i = 0; i < stuCard.nTimeSectionNum; i++)
	{
		printf("stuCard.sznTimeSectionNo[%d]:%d\n", i, stuCard.sznTimeSectionNo[i]);
	}
	printf("stuCard.nUserTime:%d\n", stuCard.nUserTime);
	printf("stuCard.stuValidStartTime: %04d-%02d-%02d %02d:%02d:%02d\n", 
		stuCard.stuValidStartTime.dwYear,
		stuCard.stuValidStartTime.dwMonth,
		stuCard.stuValidStartTime.dwDay,
		stuCard.stuValidStartTime.dwHour,
		stuCard.stuValidStartTime.dwMinute,
		stuCard.stuValidStartTime.dwSecond);
	printf("stuCard.stuValidEndTime: %04d-%02d-%02d %02d:%02d:%02d\n", 
		stuCard.stuValidEndTime.dwYear,
		stuCard.stuValidEndTime.dwMonth,
		stuCard.stuValidEndTime.dwDay,
		stuCard.stuValidEndTime.dwHour,
		stuCard.stuValidEndTime.dwMinute,
		stuCard.stuValidEndTime.dwSecond);
	printf("stuCard.bIsValid:%d\n", stuCard.bIsValid);
	printf("===门禁卡信息\n");
}

void ShowPwdInfo(NET_RECORDSET_ACCESS_CTL_PWD& stuAccessCtlPwd)
{
	unsigned int i = 0;
	printf("===门禁密码信息:\n");
	printf("stuAccessCtlPwd.nRecNo:%d\n", stuAccessCtlPwd.nRecNo);
	printf("stuAccessCtlPwd.stuCreateTime: %04d-%02d-%02d %02d:%02d:%02d\n", 
		stuAccessCtlPwd.stuCreateTime.dwYear,
		stuAccessCtlPwd.stuCreateTime.dwMonth,
		stuAccessCtlPwd.stuCreateTime.dwDay,
		stuAccessCtlPwd.stuCreateTime.dwHour,
		stuAccessCtlPwd.stuCreateTime.dwMinute,
		stuAccessCtlPwd.stuCreateTime.dwSecond);
	printf("stuAccessCtlPwd.szUserID:%s\n", stuAccessCtlPwd.szUserID);
	printf("stuAccessCtlPwd.szDoorOpenPwd:%s\n", stuAccessCtlPwd.szDoorOpenPwd);
	printf("stuAccessCtlPwd.szAlarmPwd:%s\n", stuAccessCtlPwd.szAlarmPwd);				
	printf("stuAccessCtlPwd.nDoorNum:%d\n", stuAccessCtlPwd.nDoorNum);
	for (i = 0; i < stuAccessCtlPwd.nDoorNum; i++)
	{
		printf("stuAccessCtlPwd.nDoorNum[%d]:%d\n", i, stuAccessCtlPwd.sznDoors[i]);
	}
	printf("===门禁密码信息\n");
}

void ShowCardRecInfo(NET_RECORDSET_ACCESS_CTL_CARDREC& stuCardRec)
{
	unsigned int i = 0;
	printf("===门禁刷卡记录信息:\n");
	printf("stuCardRec.nRecNo:%d\n", stuCardRec.nRecNo);
	printf("stuCardRec.szCardNo:%s\n", stuCardRec.szCardNo);
	printf("stuCardRec.szPwd:%s\n", stuCardRec.szPwd);
	printf("stuCardRec.stuTime: %04d-%02d-%02d %02d:%02d:%02d\n", 
		stuCardRec.stuTime.dwYear,
		stuCardRec.stuTime.dwMonth,
		stuCardRec.stuTime.dwDay,
		stuCardRec.stuTime.dwHour,
		stuCardRec.stuTime.dwMinute,
		stuCardRec.stuTime.dwSecond);	
	printf("stuCardRec.bStatus:%d\n", stuCardRec.bStatus);
	printf("stuCardRec.emMethod:%d\n", stuCardRec.emMethod);
	printf("stuCardRec.nDoor:%d\n", stuCardRec.nDoor);
	printf("===门禁刷卡记录信息\n");
}

void ShowHolidayInfo(NET_RECORDSET_HOLIDAY& stuHoliday)
{
	unsigned int i = 0;
	printf("===假日记录信息:\n");
	printf("stuHoliday.nRecNo:%d\n", stuHoliday.nRecNo);
	printf("stuHoliday.nDoorNum:%d\n", stuHoliday.nDoorNum);
	for (i = 0; i < stuHoliday.nDoorNum; i++)
	{
		printf("stuHoliday.nDoorNum[%d]:%d\n", i, stuHoliday.sznDoors[i]);
	}
	printf("stuHoliday.stuStartTime: %04d-%02d-%02d %02d:%02d:%02d\n", 
		stuHoliday.stuStartTime.dwYear,
		stuHoliday.stuStartTime.dwMonth,
		stuHoliday.stuStartTime.dwDay,
		stuHoliday.stuStartTime.dwHour,
		stuHoliday.stuStartTime.dwMinute,
		stuHoliday.stuStartTime.dwSecond);
	printf("stuHoliday.stuEndTime: %04d-%02d-%02d %02d:%02d:%02d\n", 
		stuHoliday.stuEndTime.dwYear,
		stuHoliday.stuEndTime.dwMonth,
		stuHoliday.stuEndTime.dwDay,
		stuHoliday.stuEndTime.dwHour,
		stuHoliday.stuEndTime.dwMinute,
		stuHoliday.stuEndTime.dwSecond);
	printf("stuHoliday.bEnable:%d\n", stuHoliday.bEnable);
	printf("===假日记录信息\n");

}

BOOL DevCtrl_CloseDoor(LLONG lLoginId)
{
	if (NULL == lLoginId)
	{
		return FALSE;
	}
	NET_CTRL_ACCESS_CLOSE stuParam = {sizeof(stuParam)};
	stuParam.nChannelID = 12;
    BOOL bResult = CLIENT_ControlDevice(lLoginId, DH_CTRL_ACCESS_CLOSE, &stuParam, SDK_API_WAITTIME);
    printf("CLIENT_ControlDevice CloseDoor result: %s\n", bResult ? "success" : "failed");
	return TRUE;
}

void DevState_DoorStatus(LLONG lLoginId)
{
	if (NULL == lLoginId)
	{
		return;
	}
	NET_DOOR_STATUS_INFO stuParam = {sizeof(stuParam)};
	stuParam.nChannel = 14;
	int nRetLen = 0;
	BOOL bResult = CLIENT_QueryDevState(lLoginId, DH_DEVSTATE_DOOR_STATE, (char*)&stuParam, sizeof(stuParam), &nRetLen, SDK_API_WAITTIME);
	if (bResult)
	{
		switch (stuParam.emStateType)
		{
		case EM_NET_DOOR_STATUS_OPEN:
			printf("Door %d is open!\n", stuParam.nChannel);
			break;
		case EM_NET_DOOR_STATUS_CLOSE:
			printf("Door %d is closed!\n", stuParam.nChannel);
			break;
		default:
			printf("Door %d status is unknown!\n", stuParam.nChannel);
			break;
		}
	}
	else
	{
		printf("CLIENT_ControlDevice query door %d status failed:%08x\n", stuParam.nChannel, CLIENT_GetLastError());
	}
}
