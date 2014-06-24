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
	ofstream myfile;
	myfile.open ("D://SDKOutput.txt");
	myfile << "WRAP_DisConnectFunc " << lLoginID << "\n";
	for(int i = 0; i < 1000; i++)
	{
		WRAP_WatchInfo* watchInfo = &WatchInfos[i];
		myfile << watchInfo->LoginId << "\n";
		if(watchInfo->LoginId == lLoginID)
		{
			myfile << "Found" << "\n";
			watchInfo->LoginId = 0;
			watchInfo->IsConnected = false;
		}
	}
	myfile.close();
	return;
}

void CALLBACK WRAP_HaveReConnectFunc(LLONG lLoginID, char *pchDVRIP, LONG nDVRPort, LDWORD dwUser)
{
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
			WRAP_JournalItem journalItem = watchInfo->WRAP_JournalItems[watchInfo->JournalLastIndex];
			watchInfo->WRAP_JournalItems[watchInfo->JournalLastIndex].EventType = lCommand;

			if (DH_ALARM_VEHICLE_CONFIRM == lCommand)
			{
				ALARM_VEHICEL_CONFIRM_INFO *pstAlarmInfo = (ALARM_VEHICEL_CONFIRM_INFO *)pBuf;
				journalItem.stGPSStatusInfo = pstAlarmInfo->stGPSStatusInfo;
				journalItem.bEventAction = pstAlarmInfo->bEventAction;
				strcpy(journalItem.szInfo, pstAlarmInfo->szInfo);
			}
			else if (DH_ALARM_VEHICLE_LARGE_ANGLE == lCommand)
			{
				ALARM_VEHICEL_LARGE_ANGLE *pstAlarmInfo = (ALARM_VEHICEL_LARGE_ANGLE *)pBuf;
				journalItem.stGPSStatusInfo = pstAlarmInfo->stGPSStatusInfo;
				journalItem.bEventAction = pstAlarmInfo->bEventAction;
			}
			//////////////////////////////////////////////////////////////////////////
			// TalkingInvite
			else if (DH_ALARM_TALKING_INVITE == lCommand)
			{
				ALARM_TALKING_INVITE_INFO* pstuAlarmInfo = (ALARM_TALKING_INVITE_INFO*)pBuf;
				journalItem.emCaller = pstuAlarmInfo->emCaller;
				journalItem.DeviceDateTime = pstuAlarmInfo->stuTime;
			}
			// LocalAlarm
			else if (DH_ALARM_ALARM_EX2 == lCommand)
			{
				ALARM_ALARM_INFO_EX2* pstuAlarmInfo = (ALARM_ALARM_INFO_EX2*)pBuf;
				journalItem.nChannelID = pstuAlarmInfo->nChannelID;
				journalItem.nAction = pstuAlarmInfo->nAction;
				journalItem.emSenseType = pstuAlarmInfo->emSenseType;
				journalItem.DeviceDateTime = pstuAlarmInfo->stuTime;
			}
			// AlarmExtend
			else if (DH_ALARM_ALARMEXTENDED == lCommand)
			{
				ALARM_ALARMEXTENDED_INFO* pstuAlarmInfo = (ALARM_ALARMEXTENDED_INFO*)pBuf;
				journalItem.nChannelID = pstuAlarmInfo->nChannelID;
				journalItem.nAction = pstuAlarmInfo->nAction;
				journalItem.DeviceDateTime = pstuAlarmInfo->stuTime;
			}
			// Urgency
			else if (DH_URGENCY_ALARM_EX == lCommand)
			{
			}
			// UrgencyEx2
			else if (DH_URGENCY_ALARM_EX2 == lCommand)
			{
				ALARM_URGENCY_ALARM_EX2* pstuAlarmInfo = (ALARM_URGENCY_ALARM_EX2*)pBuf;
				journalItem.DeviceDateTime = pstuAlarmInfo->stuTime;
			}
			// BatteryLowPower
			else if (DH_ALARM_BATTERYLOWPOWER == lCommand)
			{
				ALARM_BATTERYLOWPOWER_INFO* pstuAlarmInfo = (ALARM_BATTERYLOWPOWER_INFO*)pBuf;
				journalItem.nAction = pstuAlarmInfo->nAction;
				journalItem.nBatteryLeft = pstuAlarmInfo->nBatteryLeft;
				journalItem.DeviceDateTime = pstuAlarmInfo->stTime;
			}
			// Temperature
			else if (DH_ALARM_TEMPERATURE == lCommand)
			{
				ALARM_TEMPERATURE_INFO* pstuAlarmInfo = (ALARM_TEMPERATURE_INFO*)pBuf;
				strcpy(journalItem.szSensorName, pstuAlarmInfo->szSensorName);
				journalItem.nChannelID = pstuAlarmInfo->nChannelID;
				journalItem.nAction = pstuAlarmInfo->nAction;
				journalItem.fTemperature = pstuAlarmInfo->fTemperature;
				journalItem.DeviceDateTime = pstuAlarmInfo->stTime;
			}
			// PowerFault
			else if (DH_ALARM_POWERFAULT == lCommand)
			{
				ALARM_POWERFAULT_INFO* pstuAlarmInfo = (ALARM_POWERFAULT_INFO*)pBuf;
				journalItem.emPowerType = pstuAlarmInfo->emPowerType;
				journalItem.emPowerFaultEvent = pstuAlarmInfo->emPowerFaultEvent;
				journalItem.nAction = pstuAlarmInfo->nAction;
				journalItem.DeviceDateTime = pstuAlarmInfo->stuTime;
			}
			// ChassisIntruded
			else if (DH_ALARM_CHASSISINTRUDED == lCommand)
			{
				ALARM_CHASSISINTRUDED_INFO* pstuAlarmInfo = (ALARM_CHASSISINTRUDED_INFO*)pBuf;
				journalItem.nAction = pstuAlarmInfo->nAction;
				journalItem.nChannelID = pstuAlarmInfo->nChannelID;
				journalItem.DeviceDateTime = pstuAlarmInfo->stuTime;
			}
			// AlarmInputSourceSignal
			else if (DH_ALARM_INPUT_SOURCE_SIGNAL == lCommand)
			{
				ALARM_INPUT_SOURCE_SIGNAL_INFO* pstuAlarmInfo = (ALARM_INPUT_SOURCE_SIGNAL_INFO*)pBuf;
				journalItem.nAction = pstuAlarmInfo->nAction;
				journalItem.nChannelID = pstuAlarmInfo->nChannelID;
				journalItem.DeviceDateTime = pstuAlarmInfo->stuTime;
			}
			//////////////////////////////////////////////////////////////////////////
			// access event
			else if (DH_ALARM_ACCESS_CTL_EVENT == lCommand)
			{
				ALARM_ACCESS_CTL_EVENT_INFO* pstuAlarmInfo = (ALARM_ACCESS_CTL_EVENT_INFO*)pBuf;
				journalItem.nDoor = pstuAlarmInfo->nDoor;
				journalItem.emEventType = pstuAlarmInfo->emEventType;
				strcpy(journalItem.szDoorName, pstuAlarmInfo->szDoorName);
				journalItem.bStatus = pstuAlarmInfo->bStatus;
				journalItem.emCardType = pstuAlarmInfo->emCardType;
				journalItem.emOpenMethod = pstuAlarmInfo->emOpenMethod;
				strcpy(journalItem.szCardNo, pstuAlarmInfo->szCardNo);
				strcpy(journalItem.szPwd, pstuAlarmInfo->szPwd);
				journalItem.DeviceDateTime = pstuAlarmInfo->stuTime;
			}
			// door not close
			else if (DH_ALARM_ACCESS_CTL_NOT_CLOSE == lCommand)
			{
				ALARM_ACCESS_CTL_NOT_CLOSE_INFO* pstuAlarmInfo = (ALARM_ACCESS_CTL_NOT_CLOSE_INFO*)pBuf;
				journalItem.nDoor = pstuAlarmInfo->nDoor;
				journalItem.nAction = pstuAlarmInfo->nAction;
				strcpy(journalItem.szDoorName, pstuAlarmInfo->szDoorName);
				journalItem.DeviceDateTime = pstuAlarmInfo->stuTime;
			}
			// break in
			else if (DH_ALARM_ACCESS_CTL_BREAK_IN == lCommand)
			{
				ALARM_ACCESS_CTL_BREAK_IN_INFO* pstuAlarmInfo = (ALARM_ACCESS_CTL_BREAK_IN_INFO*)pBuf;
				journalItem.nDoor = pstuAlarmInfo->nDoor;
				strcpy(journalItem.szDoorName, pstuAlarmInfo->szDoorName);
				journalItem.DeviceDateTime = pstuAlarmInfo->stuTime;
			}
			// repeat enter
			else if (DH_ALARM_ACCESS_CTL_REPEAT_ENTER == lCommand)
			{
				ALARM_ACCESS_CTL_REPEAT_ENTER_INFO* pstuAlarmInfo = (ALARM_ACCESS_CTL_REPEAT_ENTER_INFO*)pBuf;
				journalItem.nDoor = pstuAlarmInfo->nDoor;
				strcpy(journalItem.szDoorName, pstuAlarmInfo->szDoorName);
				journalItem.DeviceDateTime = pstuAlarmInfo->stuTime;
			}
			// duress
			else if (DH_ALARM_ACCESS_CTL_DURESS == lCommand)
			{
				ALARM_ACCESS_CTL_DURESS_INFO* pstuAlarmInfo = (ALARM_ACCESS_CTL_DURESS_INFO*)pBuf;
				journalItem.nDoor = pstuAlarmInfo->nDoor;
				strcpy(journalItem.szDoorName, pstuAlarmInfo->szDoorName);
				strcpy(journalItem.szCardNo, pstuAlarmInfo->szCardNo);
				journalItem.DeviceDateTime = pstuAlarmInfo->stuTime;
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

	ofstream myfile;
	myfile.open ("D://SDKOutput.txt");

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

		myfile << watchInfo->LoginId;
		myfile << "\n";
	}


	for(int i = 0; i < 1000; i++)
	{
		WRAP_WatchInfo __watchInfo = WatchInfos[i];
		myfile << __watchInfo.LoginId;
		myfile << "\n";
	}
	myfile.close();


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
			result->EventType = watchInfo->WRAP_JournalItems[index].EventType;
			result->DeviceDateTime = watchInfo->WRAP_JournalItems[index].DeviceDateTime;
			result->emCaller = watchInfo->WRAP_JournalItems[index].emCaller;
			result->nChannelID = watchInfo->WRAP_JournalItems[index].nChannelID;
			result->nAction = watchInfo->WRAP_JournalItems[index].nAction;
			result->emSenseType = watchInfo->WRAP_JournalItems[index].emSenseType;
			result->nBatteryLeft = watchInfo->WRAP_JournalItems[index].nBatteryLeft;
			result->fTemperature = watchInfo->WRAP_JournalItems[index].fTemperature;
			strcpy(result->szSensorName, watchInfo->WRAP_JournalItems[index].szSensorName);
			result->emPowerType = watchInfo->WRAP_JournalItems[index].emPowerType;
			result->emPowerFaultEvent = watchInfo->WRAP_JournalItems[index].emPowerFaultEvent;
			result->nDoor = watchInfo->WRAP_JournalItems[index].nDoor;
			strcpy(result->szDoorName, watchInfo->WRAP_JournalItems[index].szDoorName);
			result->emEventType = watchInfo->WRAP_JournalItems[index].emEventType;
			result->bStatus = watchInfo->WRAP_JournalItems[index].bStatus;
			result->emCardType = watchInfo->WRAP_JournalItems[index].emCardType;
			result->emOpenMethod = watchInfo->WRAP_JournalItems[index].emOpenMethod;
			strcpy(result->szCardNo, watchInfo->WRAP_JournalItems[index].szCardNo);
			strcpy(result->szPwd, watchInfo->WRAP_JournalItems[index].szPwd);
			return TRUE;
		}
	}
	return FALSE;
}

BOOL CALL_METHOD WRAP_IsConnected(int loginID)
{
	ofstream myfile;
	myfile.open ("D://SDKOutput.txt");
	myfile << "WRAP_IsConnected " << loginID << "\n";
	for(int i = 0; i < 1000; i++)
	{
		WRAP_WatchInfo* watchInfo = &WatchInfos[i];
		myfile << watchInfo->LoginId << watchInfo->IsConnected << "\n";
		myfile << watchInfo->IsConnected << "\n";
		if(watchInfo->LoginId == loginID)
		{
			myfile << "Found" << "\n";
			return watchInfo->IsConnected;
		}
	}
	myfile.close();
	return FALSE;
}