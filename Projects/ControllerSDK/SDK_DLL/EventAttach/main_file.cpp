#include <iostream>
#include "dhnetsdk.h"
#if defined(WIN32) || defined(WIN64)
#pragma comment(lib, "Bin/dhnetsdk.lib")
#elif defined(__linux__)
#define Sleep(x) usleep(x*1000)
#endif

using namespace std;

void CALLBACK DisConnectFunc(LLONG lLoginID, char *pchDVRIP, LONG nDVRPort, LDWORD dwUser)
{
	printf("Device %s:%d disconnect...\n", pchDVRIP, nDVRPort);
    return;
}

void CALLBACK HaveReConnectFunc(LLONG lLoginID, char *pchDVRIP, LONG nDVRPort, LDWORD dwUser)
{
    return;
}

BOOL CALLBACK MessageCallBack(LONG lCommand, LLONG lLoginID, char *pBuf, DWORD dwBufLen, char *pchDVRIP, LONG nDVRPort, LDWORD dwUser)
{
	printf("Receive message: 0x%08x and buff len: %d\n", lCommand, dwBufLen);
	if (DH_ALARM_VEHICLE_CONFIRM == lCommand)
	{
		ALARM_VEHICEL_CONFIRM_INFO *pstAlarmInfo = (ALARM_VEHICEL_CONFIRM_INFO *)pBuf;
		cout << "Event Type:VehicelConfirm, dwStructSize=" << pstAlarmInfo->dwStructSize
			<< " and Year:"<< pstAlarmInfo->stGPSStatusInfo.revTime.dwYear << endl;
	}
	else if (DH_ALARM_VEHICLE_LARGE_ANGLE == lCommand)
	{
		ALARM_VEHICEL_LARGE_ANGLE *pstAlarmInfo = (ALARM_VEHICEL_LARGE_ANGLE *)pBuf;
		cout << "Event Type:VehicelLargeAngle, dwStructSize=" << pstAlarmInfo->dwStructSize
			<< " and Year:"<< pstAlarmInfo->stGPSStatusInfo.revTime.dwYear << endl;
	}
	//////////////////////////////////////////////////////////////////////////
	// TalkingInvite
	else if (DH_ALARM_TALKING_INVITE == lCommand)
	{
		ALARM_TALKING_INVITE_INFO* pstuAlarmInfo = (ALARM_TALKING_INVITE_INFO*)pBuf;
		cout << "Event Type:TalkingInvite, dwStructSize=" << pstuAlarmInfo->dwSize
			<< "The caller should be: " << pstuAlarmInfo->emCaller << endl;
	}
	// LocalAlarm
	else if (DH_ALARM_ALARM_EX2 == lCommand)
	{
		ALARM_ALARM_INFO_EX2* pstuAlarmInfo = (ALARM_ALARM_INFO_EX2*)pBuf;
		cout << "Event Type:LocalAlarmEx2, dwStructSize=" << pstuAlarmInfo->dwSize << endl;
		printf("%04d-%02d-%02d %02d:%02d:%02d Channel: %d, Act: %d, SenseType:%08x\n",
			pstuAlarmInfo->stuTime.dwYear, pstuAlarmInfo->stuTime.dwMonth, pstuAlarmInfo->stuTime.dwDay,
			pstuAlarmInfo->stuTime.dwHour, pstuAlarmInfo->stuTime.dwMinute, pstuAlarmInfo->stuTime.dwSecond,
			pstuAlarmInfo->nChannelID, pstuAlarmInfo->nAction, pstuAlarmInfo->emSenseType);
	}
	// AlarmExtend
	else if (DH_ALARM_ALARMEXTENDED == lCommand)
	{
		ALARM_ALARMEXTENDED_INFO* pstuAlarmInfo = (ALARM_ALARMEXTENDED_INFO*)pBuf;
		cout << "Event Type:LocalExtendAlarm, dwStructSize=" << pstuAlarmInfo->dwSize << endl;
		printf("%04d-%02d-%02d %02d:%02d:%02d Channel: %d, Act: %d\n",
			pstuAlarmInfo->stuTime.dwYear, pstuAlarmInfo->stuTime.dwMonth, pstuAlarmInfo->stuTime.dwDay,
			pstuAlarmInfo->stuTime.dwHour, pstuAlarmInfo->stuTime.dwMinute, pstuAlarmInfo->stuTime.dwSecond,
			pstuAlarmInfo->nChannelID, pstuAlarmInfo->nAction);
	}
	// Urgency
	else if (DH_URGENCY_ALARM_EX == lCommand)
	{
		cout << "Event Type:Urgency, dwStructSize=" << dwBufLen << endl;
	}
	// UrgencyEx2
	else if (DH_URGENCY_ALARM_EX2 == lCommand)
	{
		ALARM_URGENCY_ALARM_EX2* pstuAlarmInfo = (ALARM_URGENCY_ALARM_EX2*)pBuf;
		cout << "Event Type:Urgency(new), dwStructSize=" << pstuAlarmInfo->dwSize << endl;
		printf("%04d-%02d-%02d %02d:%02d:%02d\n",
			pstuAlarmInfo->stuTime.dwYear, pstuAlarmInfo->stuTime.dwMonth, pstuAlarmInfo->stuTime.dwDay,
			pstuAlarmInfo->stuTime.dwHour, pstuAlarmInfo->stuTime.dwMinute, pstuAlarmInfo->stuTime.dwSecond);
	}
	// BatteryLowPower
	else if (DH_ALARM_BATTERYLOWPOWER == lCommand)
	{
		ALARM_BATTERYLOWPOWER_INFO* pstuAlarmInfo = (ALARM_BATTERYLOWPOWER_INFO*)pBuf;
		cout << "Event Type:BatteryLowPower, dwStructSize=" << pstuAlarmInfo->dwSize << endl;
		printf("%04d-%02d-%02d %02d:%02d:%02d\n",
			pstuAlarmInfo->stTime.dwYear, pstuAlarmInfo->stTime.dwMonth, pstuAlarmInfo->stTime.dwDay,
			pstuAlarmInfo->stTime.dwHour, pstuAlarmInfo->stTime.dwMinute, pstuAlarmInfo->stTime.dwSecond);
	}
	// Temperature
	else if (DH_ALARM_TEMPERATURE == lCommand)
	{
		ALARM_TEMPERATURE_INFO* pstuAlarmInfo = (ALARM_TEMPERATURE_INFO*)pBuf;
		cout << "Event Type:Temperature, dwStructSize=" << pstuAlarmInfo->dwSize << endl;
		printf("%04d-%02d-%02d %02d:%02d:%02d\n",
			pstuAlarmInfo->stTime.dwYear, pstuAlarmInfo->stTime.dwMonth, pstuAlarmInfo->stTime.dwDay,
			pstuAlarmInfo->stTime.dwHour, pstuAlarmInfo->stTime.dwMinute, pstuAlarmInfo->stTime.dwSecond);
	}
	// PowerFault
	else if (DH_ALARM_POWERFAULT == lCommand)
	{
		ALARM_POWERFAULT_INFO* pstuAlarmInfo = (ALARM_POWERFAULT_INFO*)pBuf;
		cout << "Event Type:PowerFault, dwStructSize=" << pstuAlarmInfo->dwSize << endl;
		printf("%04d-%02d-%02d %02d:%02d:%02d power type:%s, Action:%s\n",
			pstuAlarmInfo->stuTime.dwYear, pstuAlarmInfo->stuTime.dwMonth, pstuAlarmInfo->stuTime.dwDay,
			pstuAlarmInfo->stuTime.dwHour, pstuAlarmInfo->stuTime.dwMinute, pstuAlarmInfo->stuTime.dwSecond,
			pstuAlarmInfo->emPowerType == EM_POWER_TYPE_MAIN ? "Main" : "Backup",
			pstuAlarmInfo->nAction == 0 ? "Start" : "Stop");
	}
	// ChassisIntruded
	else if (DH_ALARM_CHASSISINTRUDED == lCommand)
	{
		ALARM_CHASSISINTRUDED_INFO* pstuAlarmInfo = (ALARM_CHASSISINTRUDED_INFO*)pBuf;
		cout << "Event Type:ChassisIntruded, dwStructSize:" << pstuAlarmInfo->dwSize << endl;
		printf("%04d-%02d-%02d %02d:%02d:%02d, Channel:%d, Action:%s\n",
			pstuAlarmInfo->stuTime.dwYear, pstuAlarmInfo->stuTime.dwMonth, pstuAlarmInfo->stuTime.dwDay,
			pstuAlarmInfo->stuTime.dwHour, pstuAlarmInfo->stuTime.dwMinute, pstuAlarmInfo->stuTime.dwSecond,
			pstuAlarmInfo->nChannelID,
			pstuAlarmInfo->nAction == 0 ? "Start" : "Stop");
	}
	// AlarmInputSourceSignal
	else if (DH_ALARM_INPUT_SOURCE_SIGNAL == lCommand)
	{
		ALARM_INPUT_SOURCE_SIGNAL_INFO* pstuAlarmInfo = (ALARM_INPUT_SOURCE_SIGNAL_INFO*)pBuf;
		cout << "Event Type:AlarmInputSourceSignal, dwStructSize:" << pstuAlarmInfo->dwSize << endl;
		printf("%04d-%02d-%02d %02d:%02d:%02d Channel: %d, Act: %d\n",
			pstuAlarmInfo->stuTime.dwYear, pstuAlarmInfo->stuTime.dwMonth, pstuAlarmInfo->stuTime.dwDay,
			pstuAlarmInfo->stuTime.dwHour, pstuAlarmInfo->stuTime.dwMinute, pstuAlarmInfo->stuTime.dwSecond,
			pstuAlarmInfo->nChannelID, pstuAlarmInfo->nAction);
	}
	//////////////////////////////////////////////////////////////////////////
	// access event
	else if (DH_ALARM_ACCESS_CTL_EVENT == lCommand)
	{
		ALARM_ACCESS_CTL_EVENT_INFO* pstuAlarmInfo = (ALARM_ACCESS_CTL_EVENT_INFO*)pBuf;
		cout << "Event Type:AccessControlEvent.dwStructSize" << pstuAlarmInfo->dwSize << endl;
		printf("%04d-%02d-%02d %02d:%02d:%02d, Chn %d, EventType %d, %s, Status %s, CardType %d, OpenMethod %d, CardNo %s, Pwd %s\n",
			pstuAlarmInfo->stuTime.dwYear, pstuAlarmInfo->stuTime.dwMonth, pstuAlarmInfo->stuTime.dwDay,
			pstuAlarmInfo->stuTime.dwHour, pstuAlarmInfo->stuTime.dwMinute, pstuAlarmInfo->stuTime.dwSecond,
			pstuAlarmInfo->nDoor,
			pstuAlarmInfo->emEventType,
			pstuAlarmInfo->szDoorName,
			pstuAlarmInfo->bStatus ? "OK" : "Failed",
			pstuAlarmInfo->emCardType,
			pstuAlarmInfo->emOpenMethod,
			pstuAlarmInfo->szCardNo,
			pstuAlarmInfo->szPwd);
	}
	// door not close
	else if (DH_ALARM_ACCESS_CTL_NOT_CLOSE == lCommand)
	{
		ALARM_ACCESS_CTL_NOT_CLOSE_INFO* pstuAlarmInfo = (ALARM_ACCESS_CTL_NOT_CLOSE_INFO*)pBuf;
		cout << "Event Type:AccessControlNotClose.dwStructSize" << pstuAlarmInfo->dwSize << endl;
		printf("%04d-%02d-%02d %02d:%02d:%02d, Chn %d, %s, %s\n", 
			pstuAlarmInfo->stuTime.dwYear, pstuAlarmInfo->stuTime.dwMonth, pstuAlarmInfo->stuTime.dwDay,
			pstuAlarmInfo->stuTime.dwHour, pstuAlarmInfo->stuTime.dwMinute, pstuAlarmInfo->stuTime.dwSecond,
			pstuAlarmInfo->nDoor,
			pstuAlarmInfo->nAction == 0 ? "Start" : "Stop",
			pstuAlarmInfo->szDoorName);
	}
	// break in
	else if (DH_ALARM_ACCESS_CTL_BREAK_IN == lCommand)
	{
		ALARM_ACCESS_CTL_BREAK_IN_INFO* pstuAlarmInfo = (ALARM_ACCESS_CTL_BREAK_IN_INFO*)pBuf;
		cout << "Event Type:AccessControlBreakIn.dwStructSize" << pstuAlarmInfo->dwSize << endl;
		printf("%04d-%02d-%02d %02d:%02d:%02d, Chn %d, %s\n", 
			pstuAlarmInfo->stuTime.dwYear, pstuAlarmInfo->stuTime.dwMonth, pstuAlarmInfo->stuTime.dwDay,
			pstuAlarmInfo->stuTime.dwHour, pstuAlarmInfo->stuTime.dwMinute, pstuAlarmInfo->stuTime.dwSecond,
			pstuAlarmInfo->nDoor,
			pstuAlarmInfo->szDoorName);
	}
	// repeat enter
	else if (DH_ALARM_ACCESS_CTL_REPEAT_ENTER == lCommand)
	{
		ALARM_ACCESS_CTL_REPEAT_ENTER_INFO* pstuAlarmInfo = (ALARM_ACCESS_CTL_REPEAT_ENTER_INFO*)pBuf;
		cout << "Event Type:AccessControlRepeatEnter.dwStructSize" << pstuAlarmInfo->dwSize << endl;
		printf("%04d-%02d-%02d %02d:%02d:%02d, Chn %d, %s\n", 
			pstuAlarmInfo->stuTime.dwYear, pstuAlarmInfo->stuTime.dwMonth, pstuAlarmInfo->stuTime.dwDay,
			pstuAlarmInfo->stuTime.dwHour, pstuAlarmInfo->stuTime.dwMinute, pstuAlarmInfo->stuTime.dwSecond,
			pstuAlarmInfo->nDoor,
			pstuAlarmInfo->szDoorName);
	}
	// duress
	else if (DH_ALARM_ACCESS_CTL_DURESS == lCommand)
	{
		ALARM_ACCESS_CTL_DURESS_INFO* pstuAlarmInfo = (ALARM_ACCESS_CTL_DURESS_INFO*)pBuf;
		cout << "Event Type:AccessControlDuress.dwStructSize" << pstuAlarmInfo->dwSize << endl;
		printf("%04d-%02d-%02d %02d:%02d:%02d, Chn %d, CardNo %s, %s\n", 
			pstuAlarmInfo->stuTime.dwYear, pstuAlarmInfo->stuTime.dwMonth, pstuAlarmInfo->stuTime.dwDay,
			pstuAlarmInfo->stuTime.dwHour, pstuAlarmInfo->stuTime.dwMinute, pstuAlarmInfo->stuTime.dwSecond,
			pstuAlarmInfo->nDoor,
			pstuAlarmInfo->szCardNo,
			pstuAlarmInfo->szDoorName);
	}
	//
	//////////////////////////////////////////////////////////////////////////

	return TRUE;
}

int main()
{
	LLONG            lLoginHandle = 0;
	int              nError       = 0;
	BOOL             bTemp        = FALSE;
	NET_DEVICEINFO   stDevInfo    = {0};
	char             pbuf[1024]    = {0};
	int              nBufLen      = 1024;
	int              pRetLen      = 0;
    int              nChannel     = 0;
    char			szChoose[32] = {0};
    int             i = 0;

	/* init first */
    CLIENT_Init(DisConnectFunc, 0);
	
    /* set reconnect callback function */
    CLIENT_SetAutoReconnect(HaveReConnectFunc, 0);

	CLIENT_SetDVRMessCallBack(MessageCallBack, NULL);

	/* Login */
	memset(&stDevInfo, 0, sizeof(NET_DEVICEINFO));
	printf("=====Please input IP,Port,Username,Password=====\n");
	char szIP[25] = {0};
	int nPort = 0;
	char szUserName[25] = {0};
	char szPwd[25] = {0};
	std::cin >> szIP >> nPort >> szUserName >> szPwd;
	lLoginHandle = CLIENT_Login(szIP, nPort, szUserName, szPwd, &stDevInfo, &nError);
	if (0 == lLoginHandle)
	{
        cout << "Login failed! Press any key to continue..." << endl;
		getchar();
		return -1;
    }

	BOOL bRet = TRUE;
	int nErr = 0;
	DWORD dwReturn = 0;
	int nRetLen = 0;
	Sleep(5);

	cout << "/********* Start Listening *********/" << endl;

	bRet = CLIENT_StartListenEx(lLoginHandle);
	if (TRUE != bRet)
	{
		cout << "Listen failed" << endl;
	}

	while(1)
	{
		Sleep(5);
	}
    /* Logout */
	bTemp = CLIENT_Logout(lLoginHandle);
	if (TRUE != bTemp)
	{
        cout << "Failed to logout! Press any key to continue..." << endl;
		getchar();
		return -2;
    }
	return 0;
}