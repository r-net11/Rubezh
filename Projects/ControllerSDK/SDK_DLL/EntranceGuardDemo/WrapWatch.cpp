#include "StdAfx.h"
#include "WrapWatch.h"

#include <iostream>
#include <fstream>
using namespace std;
 
#define QUERY_COUNT	(3)

WRAP_ProgressCallback SavesdProgressCallback;

BOOL CALL_METHOD WRAP_StartProgress(WRAP_ProgressCallback progressCallback)
{
	SavesdProgressCallback = progressCallback;
	for(int i = 0; i < 100; i++)
	{
		if(progressCallback)
		{
			progressCallback(i);
		}
	}
	return TRUE;

	std::list<int> evntsList;
	evntsList.push_front(0);
}

WRAP_JournalItem WRAP_JournalItems[1000];
int JournalLastIndex = -1;

int CALL_METHOD WRAP_GetLastIndex()
{
	return JournalLastIndex;
}

BOOL CALL_METHOD WRAP_GetJournalItem(int index, WRAP_JournalItem* result)
{
	result->EventType = WRAP_JournalItems[index].EventType;
	result->DeviceDateTime = WRAP_JournalItems[index].DeviceDateTime;
	result->emCaller = WRAP_JournalItems[index].emCaller;
	result->nChannelID = WRAP_JournalItems[index].nChannelID;
	result->nAction = WRAP_JournalItems[index].nAction;
	result->emSenseType = WRAP_JournalItems[index].emSenseType;
	result->nBatteryLeft = WRAP_JournalItems[index].nBatteryLeft;
	result->fTemperature = WRAP_JournalItems[index].fTemperature;
	strcpy(result->szSensorName, WRAP_JournalItems[index].szSensorName);
	result->emPowerType = WRAP_JournalItems[index].emPowerType;
	result->emPowerFaultEvent = WRAP_JournalItems[index].emPowerFaultEvent;
	result->nDoor = WRAP_JournalItems[index].nDoor;
	strcpy(result->szDoorName, WRAP_JournalItems[index].szDoorName);
	result->emEventType = WRAP_JournalItems[index].emEventType;
	result->bStatus = WRAP_JournalItems[index].bStatus;
	result->emCardType = WRAP_JournalItems[index].emCardType;
	result->emOpenMethod = WRAP_JournalItems[index].emOpenMethod;
	strcpy(result->szCardNo, WRAP_JournalItems[index].szCardNo);
	strcpy(result->szPwd, WRAP_JournalItems[index].szPwd);

	return TRUE;
}

BOOL CALLBACK MessageCallBack(LONG lCommand, LLONG lLoginID, char *pBuf, DWORD dwBufLen, char *pchDVRIP, LONG nDVRPort, LDWORD dwUser)
{
	JournalLastIndex++;
	WRAP_JournalItems[JournalLastIndex].EventType = lCommand;

	if (DH_ALARM_VEHICLE_CONFIRM == lCommand)
	{
		ALARM_VEHICEL_CONFIRM_INFO *pstAlarmInfo = (ALARM_VEHICEL_CONFIRM_INFO *)pBuf;
		WRAP_JournalItems[JournalLastIndex].stGPSStatusInfo = pstAlarmInfo->stGPSStatusInfo;
		WRAP_JournalItems[JournalLastIndex].bEventAction = pstAlarmInfo->bEventAction;
		strcpy(WRAP_JournalItems[JournalLastIndex].szInfo, pstAlarmInfo->szInfo);
	}
	else if (DH_ALARM_VEHICLE_LARGE_ANGLE == lCommand)
	{
		ALARM_VEHICEL_LARGE_ANGLE *pstAlarmInfo = (ALARM_VEHICEL_LARGE_ANGLE *)pBuf;
		WRAP_JournalItems[JournalLastIndex].stGPSStatusInfo = pstAlarmInfo->stGPSStatusInfo;
		WRAP_JournalItems[JournalLastIndex].bEventAction = pstAlarmInfo->bEventAction;
	}
	//////////////////////////////////////////////////////////////////////////
	// TalkingInvite
	else if (DH_ALARM_TALKING_INVITE == lCommand)
	{
		ALARM_TALKING_INVITE_INFO* pstuAlarmInfo = (ALARM_TALKING_INVITE_INFO*)pBuf;
		WRAP_JournalItems[JournalLastIndex].emCaller = pstuAlarmInfo->emCaller;
		WRAP_JournalItems[JournalLastIndex].DeviceDateTime = pstuAlarmInfo->stuTime;
	}
	// LocalAlarm
	else if (DH_ALARM_ALARM_EX2 == lCommand)
	{
		ALARM_ALARM_INFO_EX2* pstuAlarmInfo = (ALARM_ALARM_INFO_EX2*)pBuf;
		WRAP_JournalItems[JournalLastIndex].nChannelID = pstuAlarmInfo->nChannelID;
		WRAP_JournalItems[JournalLastIndex].nAction = pstuAlarmInfo->nAction;
		WRAP_JournalItems[JournalLastIndex].emSenseType = pstuAlarmInfo->emSenseType;
		WRAP_JournalItems[JournalLastIndex].DeviceDateTime = pstuAlarmInfo->stuTime;
	}
	// AlarmExtend
	else if (DH_ALARM_ALARMEXTENDED == lCommand)
	{
		ALARM_ALARMEXTENDED_INFO* pstuAlarmInfo = (ALARM_ALARMEXTENDED_INFO*)pBuf;
		WRAP_JournalItems[JournalLastIndex].nChannelID = pstuAlarmInfo->nChannelID;
		WRAP_JournalItems[JournalLastIndex].nAction = pstuAlarmInfo->nAction;
		WRAP_JournalItems[JournalLastIndex].DeviceDateTime = pstuAlarmInfo->stuTime;
	}
	// Urgency
	else if (DH_URGENCY_ALARM_EX == lCommand)
	{
	}
	// UrgencyEx2
	else if (DH_URGENCY_ALARM_EX2 == lCommand)
	{
		ALARM_URGENCY_ALARM_EX2* pstuAlarmInfo = (ALARM_URGENCY_ALARM_EX2*)pBuf;
		WRAP_JournalItems[JournalLastIndex].DeviceDateTime = pstuAlarmInfo->stuTime;
	}
	// BatteryLowPower
	else if (DH_ALARM_BATTERYLOWPOWER == lCommand)
	{
		ALARM_BATTERYLOWPOWER_INFO* pstuAlarmInfo = (ALARM_BATTERYLOWPOWER_INFO*)pBuf;
		WRAP_JournalItems[JournalLastIndex].nAction = pstuAlarmInfo->nAction;
		WRAP_JournalItems[JournalLastIndex].nBatteryLeft = pstuAlarmInfo->nBatteryLeft;
		WRAP_JournalItems[JournalLastIndex].DeviceDateTime = pstuAlarmInfo->stTime;
	}
	// Temperature
	else if (DH_ALARM_TEMPERATURE == lCommand)
	{
		ALARM_TEMPERATURE_INFO* pstuAlarmInfo = (ALARM_TEMPERATURE_INFO*)pBuf;
		strcpy(WRAP_JournalItems[JournalLastIndex].szSensorName, pstuAlarmInfo->szSensorName);
		WRAP_JournalItems[JournalLastIndex].nChannelID = pstuAlarmInfo->nChannelID;
		WRAP_JournalItems[JournalLastIndex].nAction = pstuAlarmInfo->nAction;
		WRAP_JournalItems[JournalLastIndex].fTemperature = pstuAlarmInfo->fTemperature;
		WRAP_JournalItems[JournalLastIndex].DeviceDateTime = pstuAlarmInfo->stTime;
	}
	// PowerFault
	else if (DH_ALARM_POWERFAULT == lCommand)
	{
		ALARM_POWERFAULT_INFO* pstuAlarmInfo = (ALARM_POWERFAULT_INFO*)pBuf;
		WRAP_JournalItems[JournalLastIndex].emPowerType = pstuAlarmInfo->emPowerType;
		WRAP_JournalItems[JournalLastIndex].emPowerFaultEvent = pstuAlarmInfo->emPowerFaultEvent;
		WRAP_JournalItems[JournalLastIndex].nAction = pstuAlarmInfo->nAction;
		WRAP_JournalItems[JournalLastIndex].DeviceDateTime = pstuAlarmInfo->stuTime;
	}
	// ChassisIntruded
	else if (DH_ALARM_CHASSISINTRUDED == lCommand)
	{
		ALARM_CHASSISINTRUDED_INFO* pstuAlarmInfo = (ALARM_CHASSISINTRUDED_INFO*)pBuf;
		WRAP_JournalItems[JournalLastIndex].nAction = pstuAlarmInfo->nAction;
		WRAP_JournalItems[JournalLastIndex].nChannelID = pstuAlarmInfo->nChannelID;
		WRAP_JournalItems[JournalLastIndex].DeviceDateTime = pstuAlarmInfo->stuTime;
	}
	// AlarmInputSourceSignal
	else if (DH_ALARM_INPUT_SOURCE_SIGNAL == lCommand)
	{
		ALARM_INPUT_SOURCE_SIGNAL_INFO* pstuAlarmInfo = (ALARM_INPUT_SOURCE_SIGNAL_INFO*)pBuf;
		WRAP_JournalItems[JournalLastIndex].nAction = pstuAlarmInfo->nAction;
		WRAP_JournalItems[JournalLastIndex].nChannelID = pstuAlarmInfo->nChannelID;
		WRAP_JournalItems[JournalLastIndex].DeviceDateTime = pstuAlarmInfo->stuTime;
	}
	//////////////////////////////////////////////////////////////////////////
	// access event
	else if (DH_ALARM_ACCESS_CTL_EVENT == lCommand)
	{
		ALARM_ACCESS_CTL_EVENT_INFO* pstuAlarmInfo = (ALARM_ACCESS_CTL_EVENT_INFO*)pBuf;
		WRAP_JournalItems[JournalLastIndex].nDoor = pstuAlarmInfo->nDoor;
		WRAP_JournalItems[JournalLastIndex].emEventType = pstuAlarmInfo->emEventType;
		strcpy(WRAP_JournalItems[JournalLastIndex].szDoorName, pstuAlarmInfo->szDoorName);
		WRAP_JournalItems[JournalLastIndex].bStatus = pstuAlarmInfo->bStatus;
		WRAP_JournalItems[JournalLastIndex].emCardType = pstuAlarmInfo->emCardType;
		WRAP_JournalItems[JournalLastIndex].emOpenMethod = pstuAlarmInfo->emOpenMethod;
		strcpy(WRAP_JournalItems[JournalLastIndex].szCardNo, pstuAlarmInfo->szCardNo);
		strcpy(WRAP_JournalItems[JournalLastIndex].szPwd, pstuAlarmInfo->szPwd);
		WRAP_JournalItems[JournalLastIndex].DeviceDateTime = pstuAlarmInfo->stuTime;
	}
	// door not close
	else if (DH_ALARM_ACCESS_CTL_NOT_CLOSE == lCommand)
	{
		ALARM_ACCESS_CTL_NOT_CLOSE_INFO* pstuAlarmInfo = (ALARM_ACCESS_CTL_NOT_CLOSE_INFO*)pBuf;
		WRAP_JournalItems[JournalLastIndex].nDoor = pstuAlarmInfo->nDoor;
		WRAP_JournalItems[JournalLastIndex].nAction = pstuAlarmInfo->nAction;
		strcpy(WRAP_JournalItems[JournalLastIndex].szDoorName, pstuAlarmInfo->szDoorName);
		WRAP_JournalItems[JournalLastIndex].DeviceDateTime = pstuAlarmInfo->stuTime;
	}
	// break in
	else if (DH_ALARM_ACCESS_CTL_BREAK_IN == lCommand)
	{
		ALARM_ACCESS_CTL_BREAK_IN_INFO* pstuAlarmInfo = (ALARM_ACCESS_CTL_BREAK_IN_INFO*)pBuf;
		WRAP_JournalItems[JournalLastIndex].nDoor = pstuAlarmInfo->nDoor;
		strcpy(WRAP_JournalItems[JournalLastIndex].szDoorName, pstuAlarmInfo->szDoorName);
		WRAP_JournalItems[JournalLastIndex].DeviceDateTime = pstuAlarmInfo->stuTime;
	}
	// repeat enter
	else if (DH_ALARM_ACCESS_CTL_REPEAT_ENTER == lCommand)
	{
		ALARM_ACCESS_CTL_REPEAT_ENTER_INFO* pstuAlarmInfo = (ALARM_ACCESS_CTL_REPEAT_ENTER_INFO*)pBuf;
		WRAP_JournalItems[JournalLastIndex].nDoor = pstuAlarmInfo->nDoor;
		strcpy(WRAP_JournalItems[JournalLastIndex].szDoorName, pstuAlarmInfo->szDoorName);
		WRAP_JournalItems[JournalLastIndex].DeviceDateTime = pstuAlarmInfo->stuTime;
	}
	// duress
	else if (DH_ALARM_ACCESS_CTL_DURESS == lCommand)
	{
		ALARM_ACCESS_CTL_DURESS_INFO* pstuAlarmInfo = (ALARM_ACCESS_CTL_DURESS_INFO*)pBuf;
		WRAP_JournalItems[JournalLastIndex].nDoor = pstuAlarmInfo->nDoor;
		strcpy(WRAP_JournalItems[JournalLastIndex].szDoorName, pstuAlarmInfo->szDoorName);
		strcpy(WRAP_JournalItems[JournalLastIndex].szCardNo, pstuAlarmInfo->szCardNo);
		WRAP_JournalItems[JournalLastIndex].DeviceDateTime = pstuAlarmInfo->stuTime;
	}
	//
	//////////////////////////////////////////////////////////////////////////

	return TRUE;
}

BOOL CALL_METHOD CALL_METHOD WRAP_StartListen(int lLoginId)
{
	CLIENT_SetDVRMessCallBack(MessageCallBack, NULL);
	CLIENT_StartListenEx(lLoginId);
	return TRUE;
}