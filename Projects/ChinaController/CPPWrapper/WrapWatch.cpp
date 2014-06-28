#include "StdAfx.h"
#include "WrapWatch.h"

#include <iostream>
#include <fstream>
using namespace std;
 
#define QUERY_COUNT	(3)

WRAP_WatchInfo* WatchInfos;
int LastWatchInfoIndex = -1;

void CALL_METHOD WRAP_Initialize()
{
	WatchInfos = new WRAP_WatchInfo[1000];
	for(int i = 0; i < 1000; i++)
	{
		WatchInfos[i].WRAP_JournalItems = new WRAP_JournalItem[1000]();
		WatchInfos[i].IsConnected = FALSE;
		WatchInfos[i].LoginId = 0;
		WatchInfos[i].JournalLastIndex = -1;
	}
}

void CALLBACK WRAP_DisConnectFunc(LLONG lLoginID, char *pchDVRIP, LONG nDVRPort, LDWORD dwUser)
{
	for(int i = 0; i < 1000; i++)
	{
		WRAP_WatchInfo* watchInfo = &WatchInfos[i];
		if(watchInfo->LoginId == lLoginID)
		{
			watchInfo->IsConnected = false;

			watchInfo->JournalLastIndex++;
			WRAP_JournalItem* journalItem = &watchInfo->WRAP_JournalItems[watchInfo->JournalLastIndex];
			journalItem->ExtraEventType = 1;
		}
	}
	return;
}

void CALLBACK WRAP_HaveReConnectFunc(LLONG lLoginID, char *pchDVRIP, LONG nDVRPort, LDWORD dwUser)
{
	BOOL bRet = CLIENT_StartListenEx(lLoginID);
	for(int i = 0; i < 1000; i++)
	{
		WRAP_WatchInfo* watchInfo = &WatchInfos[i];
		if(watchInfo->LoginId == lLoginID)
		{
			watchInfo->IsConnected = false;

			watchInfo->JournalLastIndex++;
			WRAP_JournalItem* journalItem = &watchInfo->WRAP_JournalItems[watchInfo->JournalLastIndex];
			journalItem->ExtraEventType = 2;
		}
	}
    return;
}

BOOL CALLBACK WRAP_MessageCallBack(LONG lCommand, LLONG loginID, char *pBuf, DWORD dwBufLen, char *pchDVRIP, LONG nDVRPort, LDWORD dwUser)
{
	for(int i = 0; i < 1000; i++)
	{
		WRAP_WatchInfo* watchInfo = &WatchInfos[i];
		if(watchInfo->LoginId == loginID)
		{
			watchInfo->JournalLastIndex++;
			WRAP_JournalItem* journalItem = &watchInfo->WRAP_JournalItems[watchInfo->JournalLastIndex];
			journalItem->EventType = lCommand;

			// access event
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
			// door not close
			else if (DH_ALARM_ACCESS_CTL_NOT_CLOSE == lCommand)
			{
				ALARM_ACCESS_CTL_NOT_CLOSE_INFO* pstuAlarmInfo = (ALARM_ACCESS_CTL_NOT_CLOSE_INFO*)pBuf;
				journalItem->DeviceDateTime = pstuAlarmInfo->stuTime;
				journalItem->nDoor = pstuAlarmInfo->nDoor;
				journalItem->nAction = pstuAlarmInfo->nAction;
			}
			// break in
			else if (DH_ALARM_ACCESS_CTL_BREAK_IN == lCommand)
			{
				ALARM_ACCESS_CTL_BREAK_IN_INFO* pstuAlarmInfo = (ALARM_ACCESS_CTL_BREAK_IN_INFO*)pBuf;
				journalItem->DeviceDateTime = pstuAlarmInfo->stuTime;
				journalItem->nDoor = pstuAlarmInfo->nDoor;
			}
			// repeat enter
			else if (DH_ALARM_ACCESS_CTL_REPEAT_ENTER == lCommand)
			{
				ALARM_ACCESS_CTL_REPEAT_ENTER_INFO* pstuAlarmInfo = (ALARM_ACCESS_CTL_REPEAT_ENTER_INFO*)pBuf;
				journalItem->DeviceDateTime = pstuAlarmInfo->stuTime;
				journalItem->nDoor = pstuAlarmInfo->nDoor;
			}
			// duress
			else if (DH_ALARM_ACCESS_CTL_DURESS == lCommand)
			{
				ALARM_ACCESS_CTL_DURESS_INFO* pstuAlarmInfo = (ALARM_ACCESS_CTL_DURESS_INFO*)pBuf;
				journalItem->DeviceDateTime = pstuAlarmInfo->stuTime;
				journalItem->nDoor = pstuAlarmInfo->nDoor;
				strcpy(journalItem->szCardNo, pstuAlarmInfo->szCardNo);
			}
		}
	}

	return TRUE;
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

    CLIENT_Init(WRAP_DisConnectFunc, 0);
    CLIENT_SetAutoReconnect(WRAP_HaveReConnectFunc, 0);
	CLIENT_SetDVRMessCallBack(WRAP_MessageCallBack, NULL);

	memset(&stDevInfo, 0, sizeof(NET_DEVICEINFO));
	LONG lLoginHandle = CLIENT_Login(ipAddress, port, userName, password, &stDevInfo, &nError);

	LastWatchInfoIndex ++;
	WRAP_WatchInfo* watchInfo = &WatchInfos[LastWatchInfoIndex];

	if (0 == lLoginHandle)
	{
		watchInfo->IsConnected = false;
		return -1;
    }
	else
	{
		watchInfo->LoginId = lLoginHandle;
		watchInfo->IsConnected = true;
		watchInfo->JournalLastIndex = -1;
		watchInfo->IsConnected = true;
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
	for(int i = 0; i < 1000; i++)
	{
		WRAP_WatchInfo* watchInfo = &WatchInfos[i];
		if(watchInfo->LoginId == loginID)
		{
			watchInfo->IsConnected = false;
		}
	}

	BOOL result = CLIENT_Logout(loginID);
	CLIENT_Cleanup();
	return result;
}

int CALL_METHOD WRAP_GetLastIndex(int loginID)
{
	for(int i = 0; i < 1000; i++)
	{
		WRAP_WatchInfo* watchInfo = &WatchInfos[i];
		if(watchInfo->LoginId == loginID)
		{
			return watchInfo->JournalLastIndex;
		}
	}
	return -1;
}

BOOL CALL_METHOD WRAP_GetJournalItem(int loginID, int index, WRAP_JournalItem* result)
{
	for(int i = 0; i < 1000; i++)
	{
		WRAP_WatchInfo* watchInfo = &WatchInfos[i];
		if(watchInfo->LoginId == loginID)
		{
			WRAP_JournalItem journalItem = watchInfo->WRAP_JournalItems[index];
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
			return TRUE;
		}
	}
	return FALSE;
}

BOOL CALL_METHOD WRAP_IsConnected(int loginID)
{
	for(int i = 0; i < 1000; i++)
	{
		WRAP_WatchInfo* watchInfo = &WatchInfos[i];
		if(watchInfo->LoginId == loginID)
		{
			return watchInfo->IsConnected;
		}
	}
	return FALSE;
}