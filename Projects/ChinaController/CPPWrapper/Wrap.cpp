#include "StdAfx.h"
#include "Wrap.h"

#include <iostream>
#include <fstream>
using namespace std;
 
#define QUERY_COUNT	(3)

BOOL CALL_METHOD WRAP_GetSoftwareInfo(int loginID, WRAP_DevConfig_TypeAndSoftInfo_Result* result)
{
	if (NULL == loginID)
	{
		return FALSE;
	}

	DHDEV_VERSION_INFO stuInfo = {0};
	int nRet = 0;
	BOOL bRet = CLIENT_QueryDevState(loginID, DH_DEVSTATE_SOFTWARE, (char*)&stuInfo, sizeof(stuInfo), &nRet, 3000);
	if (bRet)
	{
		strncpy(result->szDevType, stuInfo.szDevType, sizeof(stuInfo.szDevType));
		strncpy(result->szSoftWareVersion, stuInfo.szSoftWareVersion, sizeof(stuInfo.szSoftWareVersion));
		result->dwSoftwareBuildDate_Year = (stuInfo.dwSoftwareBuildDate>>16) & 0xffff;
		result->dwSoftwareBuildDate_Month = (stuInfo.dwSoftwareBuildDate>>8) & 0xff;
		result->dwSoftwareBuildDate_Day = stuInfo.dwSoftwareBuildDate & 0xff;
	}
	return bRet;
}

BOOL CALL_METHOD WRAP_Get_NetInfo(int loginID, WRAP_CFG_NETWORK_INFO_Result* result)
{
	if (NULL == loginID)
	{
		return FALSE;
	}

	int nError = 0;
	char szBuffer[32 * 1024] = {0};
	BOOL bRet = CLIENT_GetNewDevConfig(loginID, CFG_CMD_NETWORK, -1, szBuffer, 32 * 1024, &nError, 3000);
	if (bRet)
	{
		int nRetLen = 0;
		CFG_NETWORK_INFO stuNetwork = {0};
		bRet = CLIENT_ParseData(CFG_CMD_NETWORK, szBuffer, &stuNetwork, sizeof(stuNetwork), &nRetLen);

		strncpy(result->szIP, stuNetwork.stuInterfaces->szIP, sizeof(stuNetwork.stuInterfaces->szIP));
		strncpy(result->szSubnetMask, stuNetwork.stuInterfaces->szSubnetMask, sizeof(stuNetwork.stuInterfaces->szSubnetMask));
		strncpy(result->szDefGateway, stuNetwork.stuInterfaces->szDefGateway, sizeof(stuNetwork.stuInterfaces->szDefGateway));
		result->nMTU = stuNetwork.stuInterfaces->nMTU;
		return bRet;
	}
	return FALSE;
}

BOOL CALL_METHOD WRAP_Set_NetInfo(int loginID, char* ip, char* mask, char* gate, int mtu)
{
	if (NULL == loginID)
	{
		return FALSE;
	}

	int nError = 0;
	CFG_NETWORK_INFO stuNetwork = {0};
	memset(stuNetwork.stuInterfaces[0].szIP, 0, MAX_ADDRESS_LEN);
	memcpy(stuNetwork.stuInterfaces[0].szIP, ip, sizeof(ip));
	memset(stuNetwork.stuInterfaces[0].szSubnetMask, 0, MAX_ADDRESS_LEN);
	memcpy(stuNetwork.stuInterfaces[0].szSubnetMask, mask, sizeof(mask));
	memset(stuNetwork.stuInterfaces[0].szDefGateway, 0, MAX_ADDRESS_LEN);
	memcpy(stuNetwork.stuInterfaces[0].szDefGateway, gate, sizeof(gate));
	stuNetwork.stuInterfaces->nMTU = mtu;

	char szOutBuffer[32 * 1024] = {0};
	int nRestart = 0;
	BOOL bRet = CLIENT_PacketData(CFG_CMD_NETWORK, &stuNetwork, sizeof(stuNetwork), szOutBuffer, 32 * 1024);
	if (bRet)
	{
		bRet = CLIENT_SetNewDevConfig(loginID, CFG_CMD_NETWORK, -1, szOutBuffer, 32 * 1024, &nError, &nRestart, 3000);
		return bRet;
	}
	return FALSE;
}

BOOL CALL_METHOD WRAP_GetMacAddress(int loginID, WRAP_DevConfig_MAC_Result* result)
{
	if (NULL == loginID)
	{
		return FALSE;
	}

	DHDEV_NETINTERFACE_INFO stuInfo = {sizeof(stuInfo)};
	int nRet = 0;
	BOOL bRet = CLIENT_QueryDevState(loginID, DH_DEVSTATE_NETINTERFACE, (char*)&stuInfo, sizeof(stuInfo), &nRet, 3000);
	if (bRet)
	{
		strncpy(result->szMAC, stuInfo.szMAC, sizeof(stuInfo.szMAC));
	}
	return bRet;
}

BOOL CALL_METHOD WRAP_GetMaxPageSize(int loginID, WRAP_DevConfig_RecordFinderCaps_Result* result)
{
	if (NULL == loginID)
	{
		return FALSE;
	}
	int nError = 0;
	char szBuffer[32 * 1024] = {0};
	memset(szBuffer, 0, 32 * 1024);
	BOOL bRet = CLIENT_QueryNewSystemInfo(loginID, CFG_CAP_CMD_RECORDFINDER, -1, szBuffer, 32 * 1024, &nError, SDK_API_WAITTIME);
	if (bRet)
	{
		int nRetLen = 0;
		CFG_CAP_RECORDFINDER_INFO stuCaps = {0};
		bRet = CLIENT_ParseData(CFG_CAP_CMD_RECORDFINDER, szBuffer, &stuCaps, sizeof(stuCaps), &nRetLen);
		if (bRet)
		{
			result->nMaxPageSize = stuCaps.nMaxPageSize;
			return TRUE;
		}
	}
	return FALSE;
}

BOOL CALL_METHOD WRAP_GetCurrentTime(int loginID, NET_TIME* result)
{
	if (NULL == loginID)
	{
		return FALSE;
	}
	NET_TIME stuNetTime = {0};
	BOOL bRet = CLIENT_QueryDeviceTime(loginID, &stuNetTime, SDK_API_WAITTIME);
	result->dwYear = stuNetTime.dwYear;
	result->dwMonth = stuNetTime.dwMonth;
	result->dwDay = stuNetTime.dwDay;
	result->dwHour = stuNetTime.dwHour;
	result->dwMinute = stuNetTime.dwMinute;
	result->dwSecond = stuNetTime.dwSecond;
	return bRet;
}

BOOL CALL_METHOD WRAP_SetCurrentTime(int loginID, int dwYear, int dwMonth, int dwDay, int dwHour, int dwMinute, int dwSecond)
{
	if (NULL == loginID)
	{
		return FALSE;
	}
	NET_TIME stuNetTime = {0};
	stuNetTime.dwYear = dwYear;
	stuNetTime.dwMonth = dwMonth;
	stuNetTime.dwDay = dwDay;
	stuNetTime.dwHour = dwHour;
	stuNetTime.dwMinute = dwMinute;
	stuNetTime.dwSecond = dwSecond;
	BOOL bRet = CLIENT_SetupDeviceTime(loginID, &stuNetTime);
	return bRet;
}

BOOL CALL_METHOD WRAP_GetControllerDirectionType(int loginID, WRAP_ControllerDirectionType* result)
{
	CFG_ACCESS_GENERAL_INFO	m_stuInfo;
	char szJsonBuf[1024 * 40] = {0};
	int nerror = 0;
	BOOL bRet = CLIENT_GetNewDevConfig((LLONG)loginID, CFG_CMD_ACCESS_GENERAL, -1, szJsonBuf, 1024*40, &nerror, SDK_API_WAITTIME);
	
	if (bRet)
	{
		DWORD dwRetLen = 0;
		bRet = CLIENT_ParseData(CFG_CMD_ACCESS_GENERAL, szJsonBuf, (void*)&m_stuInfo, sizeof(m_stuInfo), &dwRetLen);
		result->emAccessProperty = m_stuInfo.emAccessProperty;
		return bRet;
	}
	return FALSE;
}

BOOL CALL_METHOD WRAP_SetControllerDirectionType(int loginID, CFG_ACCESS_PROPERTY_TYPE emAccessProperty)
{
	CFG_ACCESS_GENERAL_INFO	m_stuInfo;
	m_stuInfo.emAccessProperty = emAccessProperty;
	char szJsonBuf[1024 * 40] = {0};
	BOOL bRet = CLIENT_PacketData(CFG_CMD_ACCESS_GENERAL, &m_stuInfo, sizeof(m_stuInfo), szJsonBuf, sizeof(szJsonBuf));
	if (!bRet)
	{
		return FALSE;
	} 
	else
	{		
		int nerror = 0;
		int nrestart = 0;
		
		bRet = CLIENT_SetNewDevConfig((LLONG)loginID, CFG_CMD_ACCESS_GENERAL, -1, szJsonBuf, 1024*40, &nerror, &nrestart, SDK_API_WAITTIME);
		return bRet;
	}
}

BOOL CALL_METHOD WRAP_SetControllerPassword(int loginID, char name[], char oldPassword[], char password[])
{
	USER_INFO_NEW stuOldInfo = {sizeof(stuOldInfo)};
	strcpy(stuOldInfo.name, name);
	strcpy(stuOldInfo.passWord, oldPassword);

	USER_INFO_NEW stuModifiedInfo = {sizeof(stuModifiedInfo)};
	strcpy(stuModifiedInfo.passWord, password);

	BOOL bRet = CLIENT_OperateUserInfoNew(loginID, 6, &stuModifiedInfo, &stuOldInfo, NULL, SDK_API_WAITTIME);
	return bRet;
}

BOOL CALL_METHOD WRAP_GetDoorConfiguration(int loginID, int channelNo, CFG_ACCESS_EVENT_INFO* result)
{
	char szJsonBuf[1024 * 40] = {0};
	int nerror = 0;

	BOOL bRet = CLIENT_GetNewDevConfig(loginID, CFG_CMD_ACCESS_EVENT, channelNo, szJsonBuf, sizeof(szJsonBuf), &nerror, SDK_API_WAITTIME);

	if (bRet)
	{
		int nRetLen = 0;
		CFG_ACCESS_EVENT_INFO stuGeneralInfo = {0};
		bRet = CLIENT_ParseData(CFG_CMD_ACCESS_EVENT, szJsonBuf, &stuGeneralInfo, sizeof(stuGeneralInfo), &nRetLen);
		memcpy(result, &stuGeneralInfo, sizeof(CFG_ACCESS_EVENT_INFO));
		return bRet;
	}
	return FALSE;
}

BOOL CALL_METHOD WRAP_SetDoorConfiguration(int loginID, int channelNo, CFG_ACCESS_EVENT_INFO* stuGeneralInfo)
{
	stuGeneralInfo->bSnapshotEnable = true;
	stuGeneralInfo->abDoorOpenMethod = true;
	stuGeneralInfo->abUnlockHoldInterval = true;
	stuGeneralInfo->abCloseTimeout = true;
	stuGeneralInfo->abOpenAlwaysTimeIndex = true;
	stuGeneralInfo->abHolidayTimeIndex = true;
	stuGeneralInfo->abBreakInAlarmEnable = true;
	stuGeneralInfo->abRepeatEnterAlarmEnable = true;
	stuGeneralInfo->abDoorNotClosedAlarmEnable = true;
	stuGeneralInfo->abDuressAlarmEnable = true;
	stuGeneralInfo->abDoorTimeSection = true;
	stuGeneralInfo->abSensorEnable = true;

	char szJsonBufSet[1024 * 40] = {0};
			
	BOOL bRet = CLIENT_PacketData(CFG_CMD_ACCESS_EVENT, stuGeneralInfo, sizeof(CFG_ACCESS_EVENT_INFO), szJsonBufSet, sizeof(szJsonBufSet));
	if (bRet)
	{
		int nerror = 0;
		int nrestart = 0;
		bRet = CLIENT_SetNewDevConfig(loginID, CFG_CMD_ACCESS_EVENT, channelNo, szJsonBufSet, sizeof(szJsonBufSet), &nerror, &nrestart, SDK_API_WAITTIME);
		return bRet;
	}
	return FALSE;
}

BOOL CALL_METHOD WRAP_ReBoot(int loginID)
{
	if (NULL == loginID)
	{
		return FALSE;
	}
    BOOL bResult = CLIENT_ControlDevice(loginID, DH_CTRL_REBOOT, NULL, SDK_API_WAITTIME);
	return TRUE;
}

BOOL CALL_METHOD WRAP_DeleteCfgFile(int loginID)
{
	if (NULL == loginID)
	{
		return FALSE;
	}
	BOOL bResult = CLIENT_ControlDevice(loginID, DH_CTRL_RESTOREDEFAULT, NULL, SDK_API_WAITTIME);
	return TRUE;
}

BOOL CALL_METHOD WRAP_OpenDoor(int loginID, int channelNo)
{
	if (NULL == loginID)
	{
		return FALSE;
	}
	NET_CTRL_ACCESS_OPEN stuInert = {sizeof(stuInert)};
	stuInert.nChannelID = channelNo;
    BOOL bResult = CLIENT_ControlDevice(loginID, DH_CTRL_ACCESS_OPEN, &stuInert, SDK_API_WAITTIME);
	return bResult;
}

BOOL CALL_METHOD WRAP_CloseDoor(int loginID, int channelNo)
{
	if (NULL == loginID)
	{
		return FALSE;
	}
	NET_CTRL_ACCESS_CLOSE stuParam = {sizeof(stuParam)};
	stuParam.nChannelID = channelNo;
    BOOL bResult = CLIENT_ControlDevice(loginID, DH_CTRL_ACCESS_CLOSE, &stuParam, SDK_API_WAITTIME);
	return bResult;
}

int CALL_METHOD WRAP_GetDoorStatus(int loginID, int channelNo)
{
	if (NULL == loginID)
	{
		return - 1;
	}
	NET_DOOR_STATUS_INFO stuParam = {sizeof(stuParam)};
	stuParam.nChannel = channelNo;
	int nRetLen = 0;
	BOOL bResult = CLIENT_QueryDevState(loginID, DH_DEVSTATE_DOOR_STATE, (char*)&stuParam, sizeof(stuParam), &nRetLen, SDK_API_WAITTIME);
	if (bResult)
	{
		switch (stuParam.emStateType)
		{
		case EM_NET_DOOR_STATUS_OPEN:
			return 1;
			break;
		case EM_NET_DOOR_STATUS_CLOSE:
			return 2;
			break;
		default:
			return 0;
			break;
		}
	}
	else
	{
		return - 1;
	}
}

LLONG m_hUpgradeId;

void WINAPI UpgradeCallBack(LLONG lLoginID, LLONG lUpgradechannel, int nTotalSize, int nSendSize, LDWORD dwUser)
{
}

BOOL CALL_METHOD WRAP_Upgrade(int loginID, char fileName[256])
{
	if (m_hUpgradeId)
	{
		CLIENT_StopUpgrade(m_hUpgradeId);
		m_hUpgradeId = NULL;
	}
	
	m_hUpgradeId = CLIENT_StartUpgradeEx(loginID, (EM_UPGRADE_TYPE)0, fileName, UpgradeCallBack, (LLONG)0);

	if (m_hUpgradeId)
	{
		int nRet = CLIENT_SendUpgrade(m_hUpgradeId);
		return TRUE;
	}
	return FALSE;
}

int CALL_METHOD WRAP_GetLogCount(int loginID, QUERY_DEVICE_LOG_PARAM* logParam)
{
	if (NULL == loginID)
	{
		return 0;
	}
	NET_IN_GETCOUNT_LOG_PARAM stuInLogCount = {sizeof(NET_IN_GETCOUNT_LOG_PARAM)};
	NET_OUT_GETCOUNT_LOG_PARAM stuOutLogCount = {sizeof(NET_OUT_GETCOUNT_LOG_PARAM)};
	
	BOOL bResult = CLIENT_QueryDevLogCount(loginID, &stuInLogCount, &stuOutLogCount, SDK_API_WAITTIME);
 	return stuOutLogCount.nLogCount;
}


LLONG m_lLogID;

BOOL CALL_METHOD WRAP_QueryStart(int loginID)
{
    NET_IN_START_QUERYLOG stuIn = {sizeof(stuIn)};
    NET_OUT_START_QUERYLOG stuOut = {sizeof(stuOut)};
    LLONG lLogID = CLIENT_StartQueryLog(loginID, &stuIn, &stuOut, SDK_API_WAITTIME);
    if (lLogID != NULL)
    {
		m_lLogID = lLogID;
        return TRUE;
    }
    return FALSE;
}

int CALL_METHOD WRAP_QueryNext(WRAP_Dev_QueryLogList_Result* result)
{
    NET_IN_QUERYNEXTLOG stuIn = {sizeof(stuIn)};
    stuIn.nGetCount = 10;

    NET_OUT_QUERYNEXTLOG stuOut = {sizeof(stuOut)};
    stuOut.nMaxCount = 10;
    stuOut.pstuLogInfo = new NET_LOG_INFO[10];
    if (NULL == stuOut.pstuLogInfo)
    {
        return -1;
    }
    memset(stuOut.pstuLogInfo, 0, sizeof(NET_LOG_INFO) * 10);
    int i = 0;
    for (i = 0; i < 10; i++)
    {
        stuOut.pstuLogInfo[i].dwSize = sizeof(NET_LOG_INFO);
        stuOut.pstuLogInfo[i].stuLogMsg.dwSize = sizeof(NET_LOG_MESSAGE);
    }
    if (CLIENT_QueryNextLog(m_lLogID, &stuIn, &stuOut, SDK_API_WAITTIME))
    {
        for (i = 0; i < __min(stuOut.nMaxCount, stuOut.nRetCount); i++)
        {
			result->Logs[i].stuTime = stuOut.pstuLogInfo[i].stuTime;
			strncpy(result->Logs[i].szUserName, stuOut.pstuLogInfo[i].szUserName, sizeof(stuOut.pstuLogInfo[i].szUserName));
			strncpy(result->Logs[i].szLogType, stuOut.pstuLogInfo[i].szLogType, sizeof(stuOut.pstuLogInfo[i].szLogType));
			strncpy(result->Logs[i].szLogMessage, stuOut.pstuLogInfo[i].stuLogMsg.szLogMessage, sizeof(stuOut.pstuLogInfo[i].stuLogMsg.szLogMessage));
        }
        return stuOut.nRetCount;
    }
    return 0;
}

BOOL CALL_METHOD WRAP_QueryStop()
{
    return CLIENT_StopQueryLog(m_lLogID);
}













BOOL CALL_METHOD TestStruct(CFG_ACCESS_EVENT_INFO* result)
{
	int size1 = sizeof(CFG_ACCESS_EVENT_INFO);
	int closeTimeout = result->nCloseTimeout;
	return TRUE;
}