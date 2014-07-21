#include "StdAfx.h"
#include "WrapWatch.h"

#include <iostream>
#include <fstream>
using namespace std;
 
#define QUERY_COUNT	(3)

typedef list<WRAP_WatchInfo> WatchInfoList;
WatchInfoList watchInfoList;

WRAP_JournalInfo* JournalInfos;
int LastJournalIndex = -1;

void AddCustomJournalItem(int loginID, int extraEventType)
{
	LastJournalIndex++;
	int localLastJournalIndex = LastJournalIndex % 1000;
	WRAP_JournalInfo* journalInfo = &JournalInfos[localLastJournalIndex];
	journalInfo->Index = localLastJournalIndex;
	WRAP_JournalItem* journalItem = &journalInfo->JournalItem;
	journalItem->LoginID = loginID;
	journalItem->ExtraEventType = extraEventType;
}

void CALLBACK WRAP_DisConnectFunc(LLONG lLoginID, char *pchDVRIP, LONG nDVRPort, LDWORD dwUser)
{
	WatchInfoList::iterator i;
	for (i =  watchInfoList.begin(); i != watchInfoList.end(); ++i)
	{
		WRAP_WatchInfo watchInfo = *i;
		if(watchInfo.LoginId == lLoginID)
		{
			watchInfo.IsConnected = false;
		}
	}
	AddCustomJournalItem(lLoginID, 1);
}

void CALLBACK WRAP_HaveReConnectFunc(LLONG lLoginID, char *pchDVRIP, LONG nDVRPort, LDWORD dwUser)
{
	WatchInfoList::iterator i;
	for (i =  watchInfoList.begin(); i != watchInfoList.end(); ++i)
	{
		WRAP_WatchInfo watchInfo = *i;
		if(watchInfo.LoginId == lLoginID)
		{
			watchInfo.IsConnected = false;
			watchInfo.NeedToRestartListening = true;
		}
	}
	AddCustomJournalItem(lLoginID, 2);
}

BOOL CALLBACK WRAP_MessageCallBack(LONG lCommand, LLONG loginID, char *pBuf, DWORD dwBufLen, char *pchDVRIP, LONG nDVRPort, LDWORD dwUser)
{
	LastJournalIndex++;
	int localLastJournalIndex = LastJournalIndex % 1000;
	WRAP_JournalInfo* journalInfo = &JournalInfos[localLastJournalIndex];
	journalInfo->Index = localLastJournalIndex;

	WRAP_JournalItem* journalItem = &journalInfo->JournalItem;
	journalItem->LoginID = loginID;
	journalItem->EventType = lCommand;

	if (DH_ALARM_ACCESS_CTL_EVENT == lCommand)
	{
		ALARM_ACCESS_CTL_EVENT_INFO* pstuAlarmInfo = (ALARM_ACCESS_CTL_EVENT_INFO*)pBuf;
		journalItem->DeviceDateTime = pstuAlarmInfo->stuTime;
		journalItem->nDoor = pstuAlarmInfo->nDoor;
		journalItem->emEventType = pstuAlarmInfo->emEventType;
		journalItem->bStatus = pstuAlarmInfo->bStatus;
		journalItem->emCardType = pstuAlarmInfo->emCardType;
		journalItem->emOpenMethod = pstuAlarmInfo->emOpenMethod;
		strcpy(journalItem->szCardNo, pstuAlarmInfo->szCardNo);
		strcpy(journalItem->szPwd, pstuAlarmInfo->szPwd);
	}
	else if (DH_ALARM_ACCESS_CTL_NOT_CLOSE == lCommand)
	{
		ALARM_ACCESS_CTL_NOT_CLOSE_INFO* pstuAlarmInfo = (ALARM_ACCESS_CTL_NOT_CLOSE_INFO*)pBuf;
		journalItem->DeviceDateTime = pstuAlarmInfo->stuTime;
		journalItem->nDoor = pstuAlarmInfo->nDoor;
		journalItem->nAction = pstuAlarmInfo->nAction;
	}
	else if (DH_ALARM_ACCESS_CTL_BREAK_IN == lCommand)
	{
		ALARM_ACCESS_CTL_BREAK_IN_INFO* pstuAlarmInfo = (ALARM_ACCESS_CTL_BREAK_IN_INFO*)pBuf;
		journalItem->DeviceDateTime = pstuAlarmInfo->stuTime;
		journalItem->nDoor = pstuAlarmInfo->nDoor;
	}
	else if (DH_ALARM_ACCESS_CTL_REPEAT_ENTER == lCommand)
	{
		ALARM_ACCESS_CTL_REPEAT_ENTER_INFO* pstuAlarmInfo = (ALARM_ACCESS_CTL_REPEAT_ENTER_INFO*)pBuf;
		journalItem->DeviceDateTime = pstuAlarmInfo->stuTime;
		journalItem->nDoor = pstuAlarmInfo->nDoor;
	}
	else if (DH_ALARM_ACCESS_CTL_DURESS == lCommand)
	{
		ALARM_ACCESS_CTL_DURESS_INFO* pstuAlarmInfo = (ALARM_ACCESS_CTL_DURESS_INFO*)pBuf;
		journalItem->DeviceDateTime = pstuAlarmInfo->stuTime;
		journalItem->nDoor = pstuAlarmInfo->nDoor;
		strcpy(journalItem->szCardNo, pstuAlarmInfo->szCardNo);
	}
	else if (DH_ALARM_ACCESS_CTL_STATUS == lCommand)
	{
		ALARM_ACCESS_CTL_STATUS_INFO* pstuStatusnfo = (ALARM_ACCESS_CTL_STATUS_INFO*)pBuf;
		journalItem->DeviceDateTime = pstuStatusnfo->stuTime;
		journalItem->nDoor = pstuStatusnfo->nDoor;
		journalItem->emStatus = pstuStatusnfo->emStatus;
	}

	return TRUE;
}

void CALL_METHOD WRAP_Initialize()
{
	CLIENT_Init(WRAP_DisConnectFunc, 0);
    CLIENT_SetAutoReconnect(WRAP_HaveReConnectFunc, 0);
	CLIENT_SetDVRMessCallBack(WRAP_MessageCallBack, NULL);

	JournalInfos = new WRAP_JournalInfo[1000];
	for(int i = 0; i < 1000; i++)
	{
		JournalInfos[i].Index = -1;
	}
}

void CALL_METHOD WRAP_Deinitialize()
{
	CLIENT_Cleanup();
}

int CALL_METHOD WRAP_Connect(char ipAddress[25], int port, char userName[25], char password[25])
{
	BOOL bTemp = FALSE;
	int nError = 0;
	NET_DEVICEINFO stDevInfo = {0};
	char pbuf[1024] = {0};
	int nBufLen = 1024;
	int pRetLen = 0;
    int nChannel = 0;
    char szChoose[32] = {0};
    int i = 0;

	memset(&stDevInfo, 0, sizeof(NET_DEVICEINFO));
	LONG lLoginHandle = CLIENT_Login(ipAddress, port, userName, password, &stDevInfo, &nError);

	if (0 == lLoginHandle)
	{
		return -1;
    }
	else
	{
		WRAP_WatchInfo watchInfo;
		watchInfo.LoginId = lLoginHandle;
		watchInfo.IsConnected = true;
		watchInfoList.insert(watchInfoList.end(), watchInfo);
	}

	if (0 == lLoginHandle)
	{
		return -1;
    }

	BOOL bRet = TRUE;
	bRet = CLIENT_StartListenEx(lLoginHandle);
	return lLoginHandle;
}

BOOL CALL_METHOD WRAP_Disconnect(int loginID)
{
	WatchInfoList::iterator i = watchInfoList.begin();
	while (i != watchInfoList.end())
	{
		WRAP_WatchInfo watchInfo = *i;
		if(watchInfo.LoginId == loginID)
		{
			watchInfoList.erase(i++);
		}
		else
		{
			++i;
		}
	}
	//watchInfoList.remove_if([](WRAP_WatchInfo x){ return x.LoginId == loginID; });
	BOOL result = CLIENT_Logout(loginID);
	return result;
}

int CALL_METHOD WRAP_GetLastIndex()
{
	WatchInfoList::iterator i;
	for (i =  watchInfoList.begin(); i != watchInfoList.end(); ++i)
	{
		WRAP_WatchInfo watchInfo = *i;
		if(watchInfo.NeedToRestartListening == true)
		{
			CLIENT_StopListen(watchInfo.LoginId);
			BOOL bRet = CLIENT_StartListenEx(watchInfo.LoginId);
			watchInfo.NeedToRestartListening = !bRet;
		}
	}

	return LastJournalIndex;
}

BOOL CALL_METHOD WRAP_GetJournalItem(int index, WRAP_JournalItem* result)
{
	int localIndex = index % 1000;
	WRAP_JournalInfo* journalInfo = &JournalInfos[localIndex];
	WRAP_JournalItem journalItem = journalInfo->JournalItem;
	result->LoginID = journalItem.LoginID;
	result->ExtraEventType = journalItem.ExtraEventType;
	result->EventType = journalItem.EventType;
	result->DeviceDateTime = journalItem.DeviceDateTime;
	result->nDoor = journalItem.nDoor;
	result->emEventType = journalItem.emEventType;
	result->bStatus = journalItem.bStatus;
	result->emCardType = journalItem.emCardType;
	result->emOpenMethod = journalItem.emOpenMethod;
	strcpy(result->szCardNo, journalItem.szCardNo);
	strcpy(result->szPwd, journalItem.szPwd);
	result->nAction = journalItem.nAction;
	result->emStatus = journalItem.emStatus;
	return TRUE;
}

BOOL CALL_METHOD WRAP_IsConnected(int loginID)
{
	WatchInfoList::iterator i;
	for (i =  watchInfoList.begin(); i != watchInfoList.end(); ++i)
	{
		WRAP_WatchInfo watchInfo = *i;
		if(watchInfo.LoginId == loginID)
		{
			return watchInfo.IsConnected;
		}
	}
	return FALSE;
}
