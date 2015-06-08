#include "StdAfx.h"
#include <atlstr.h>
#include "WrapAntiPassBack.h"

/// <summary>
/// ѕолучает количество дверей на контроллере
/// <param name="loginID">залогированный в SDK клиент</param>
/// <param name="nCount">возвращаемое количество дверей</param>
/// </summary>
/// <returns>TRUE при успешном завершении, FALSE в случае неудачи</returns>
BOOL GetDoorsCount(int loginID, int& nCount)
{
	char szBuf[1024] = {0};
	int nError = 0;
	BOOL bRet = CLIENT_QueryNewSystemInfo(loginID, CFG_CAP_CMD_ACCESSCONTROLMANAGER, -1, szBuf, sizeof(szBuf), &nError, 3000);
	if (bRet)
	{
		CFG_CAP_ACCESSCONTROL stuCap = {0};
		DWORD dwRet = 0;
		bRet = CLIENT_ParseData(CFG_CAP_CMD_ACCESSCONTROLMANAGER, szBuf, &stuCap, sizeof(stuCap), &dwRet);
		if (bRet && dwRet == sizeof(CFG_CAP_ACCESSCONTROL))
		{
			nCount = stuCap.nAccessControlGroups;
		}
		else
		{
			return FALSE;
		}
	}
	else
	{
		return FALSE;
	}
	return TRUE;
}

/// <summary>
/// ѕолучает текущие настройки дл€ канала контроллера
/// <param name="loginID">залогированный в SDK клиент</param>
/// <param name="nChn">номер канала</param>
/// <param name="result">возвращаема€ структура CFG_ACCESS_EVENT_INFO</param>
/// </summary>
/// <returns>TRUE при успешном завершении, FALSE в случае неудачи</returns>
BOOL GetConfigFromDevice(int loginID, int nChn, CFG_ACCESS_EVENT_INFO* result)
{ 
	if (nChn == -1)
	{
		return FALSE;
	}
	CFG_ACCESS_EVENT_INFO stuEventInfo = {0};
	char szJsonBuf[1024 * 40] = {0};
	int nerror = 0;
	BOOL bRet = CLIENT_GetNewDevConfig((LLONG)loginID, CFG_CMD_ACCESS_EVENT, nChn, szJsonBuf, 1024*40, &nerror, 5000);

	if (bRet)
	{
		DWORD dwRetLen = 0;
		bRet = CLIENT_ParseData(CFG_CMD_ACCESS_EVENT, szJsonBuf, (void*)&stuEventInfo, sizeof(stuEventInfo), &dwRetLen);
		if (!bRet)
		{
			return FALSE;
		}
		memcpy(result, &stuEventInfo, sizeof(stuEventInfo));
	}
	else
	{			
		return FALSE;
	}
	return TRUE;
}

BOOL GetNewDevConfig(int loginID, CFG_OPEN_DOOR_ROUTE_INFO* result)
{
	int nRetLen = 0;

	char szJsonBuf[1024 * 40] = {0};	

	CFG_OPEN_DOOR_ROUTE_INFO stuInfo = {0};
	memset(szJsonBuf, 0, 1024*40);

	int nerror = 0;
	BOOL bRet = CLIENT_GetNewDevConfig((LLONG)loginID, CFG_CMD_OPEN_DOOR_ROUTE, -1, szJsonBuf, 1024*40, &nerror, 5000);
	if (bRet)
	{
		DWORD dwRetLen = 0;
		bRet = CLIENT_ParseData(CFG_CMD_OPEN_DOOR_ROUTE, szJsonBuf, (void*)&stuInfo, sizeof(CFG_OPEN_DOOR_ROUTE_INFO), &dwRetLen);
		if (bRet)
		{
			memcpy(result, &stuInfo, sizeof(stuInfo));
			return TRUE;
		}
	}
	return FALSE;
}

/// <summary>
/// «аписывает текущие настройки дл€ канала в контроллер
/// <param name="loginID">залогированный в SDK клиент</param>
/// <param name="nChn">номер канала</param>
/// <param name="result">настроенна€ структура CFG_ACCESS_EVENT_INFO</param>
/// </summary>
/// <returns>TRUE при успешном завершении, FALSE в случае неудачи</returns>
BOOL SetConfigToDevice(int loginID, int nChn, CFG_ACCESS_EVENT_INFO* param)
{
	if (nChn == -1)
	{
		return FALSE;
	}

	char szJsonBuf[1024 * 40] = {0};
	BOOL bRet = CLIENT_PacketData(CFG_CMD_ACCESS_EVENT, param, sizeof(*param), szJsonBuf, sizeof(szJsonBuf));
	if (!bRet)
	{
		return FALSE;
	} 
	else
	{		
		int nerror = 0;
		int nrestart = 0;

		bRet = CLIENT_SetNewDevConfig((LLONG)loginID, CFG_CMD_ACCESS_EVENT, nChn, szJsonBuf, 1024*40, &nerror, &nrestart, 5000);
		if (!bRet)
		{
			return FALSE;
		} 
	}
	return TRUE;
}

/// <summary>
/// ѕолучает настройку Anti-pass Back на контроллере
/// <param name="loginID">залогированный в SDK клиент</param>
/// <param name="result">возвращаема€ настройка Anti-pass Back</param>
/// </summary>
/// <returns>TRUE при успешном завершении, FALSE в случае неудачи</returns>
BOOL SDK_CALL_METHOD WRAP_GetAntiPassBackCfg(int loginID, WRAP_AntiPassBackCfg* result)
{
	BOOL bRet;
	int nDoorsCount;
	CFG_ACCESS_EVENT_INFO stuEventInfo;
	CFG_OPEN_DOOR_ROUTE_INFO stuInfo;

	if (loginID == NULL)
	{
		return FALSE;
	}

	// ќпредел€ем количесво дверей на контроллере
	bRet = GetDoorsCount(loginID, nDoorsCount);
	if (!bRet)
	{
		return FALSE;
	}

	WRAP_AntiPassBackCfg cfg = {sizeof(WRAP_AntiPassBackCfg)};

	cfg.AvailableAntiPassBackModes[ANTIPASSBACK_MODE_R1R2].AntiPassBackMode = R1R2;
	cfg.AvailableAntiPassBackModes[ANTIPASSBACK_MODE_R1R3R2R4].AntiPassBackMode = R1R3R2R4;
	cfg.AvailableAntiPassBackModes[ANTIPASSBACK_MODE_R3R4].AntiPassBackMode = R3R4;

	cfg.nDoorsCount = nDoorsCount;

	// ƒл€ однодверника доступен только режим R1R2
	if (nDoorsCount == 1)
	{
		cfg.AvailableAntiPassBackModes[ANTIPASSBACK_MODE_R1R2].bIsAvailable = TRUE;
		cfg.AvailableAntiPassBackModes[ANTIPASSBACK_MODE_R1R3R2R4].bIsAvailable = FALSE;
		cfg.AvailableAntiPassBackModes[ANTIPASSBACK_MODE_R3R4].bIsAvailable = FALSE;
	}
	// ƒл€ двухдверника доступны все режимы - R1R2, R1R3R2R4 и R3R4
	else if (nDoorsCount == 2)
	{
		cfg.AvailableAntiPassBackModes[ANTIPASSBACK_MODE_R1R2].bIsAvailable = TRUE;
		cfg.AvailableAntiPassBackModes[ANTIPASSBACK_MODE_R1R3R2R4].bIsAvailable = TRUE;
		cfg.AvailableAntiPassBackModes[ANTIPASSBACK_MODE_R3R4].bIsAvailable = TRUE;
	}

	if (GetConfigFromDevice(loginID, 0, &stuEventInfo) && stuEventInfo.bRepeatEnterAlarm)
	{
		cfg.bIsActivated = TRUE;
	}
	if (GetConfigFromDevice(loginID, 1, &stuEventInfo) && stuEventInfo.bRepeatEnterAlarm)
	{
		cfg.bIsActivated = TRUE;
	}
	if (GetNewDevConfig(loginID, &stuInfo))
	{
		CString csInfo; 
		for(int i = 0; i < stuInfo.nDoorList; i ++)
		{
			CString csInfotp; 
			csInfotp.Format("%s%s%s%s",  stuInfo.stuDoorList[i].stuDoors[0].szReaderID
			, stuInfo.stuDoorList[i].stuDoors[1].szReaderID
			, stuInfo.stuDoorList[i].stuDoors[2].szReaderID
			, stuInfo.stuDoorList[i].stuDoors[3].szReaderID);
			csInfo = csInfo +  csInfotp;
		}
		if (csInfo == "12")
			cfg.CurrentAntiPassBackMode = R1R2;
		else if (csInfo == "1324")
			cfg.CurrentAntiPassBackMode = R1R3R2R4;
		else if (csInfo == "34")
			cfg.CurrentAntiPassBackMode = R3R4;
		else if (csInfo == "2413")
			cfg.CurrentAntiPassBackMode = R3R4;

		cfg.bCanActivate = TRUE;
	}
	
	memcpy(result, &cfg, sizeof(cfg));

	return TRUE;
}

/// <summary>
/// «аписывает настройки Anti-pass Back на контроллер
/// <param name="loginID">залогированный в SDK клиент</param>
/// <param name="cfg">сохран€ема€ настройка Anti-pass Back</param>
/// </summary>
/// <returns>TRUE при успешном завершении, FALSE в случае неудачи</returns>
BOOL SDK_CALL_METHOD WRAP_SetAntiPassBackCfg(int loginID, WRAP_AntiPassBackCfg* cfg)
{
	CFG_OPEN_DOOR_ROUTE_INFO stuInfo = {0};
	CFG_ACCESS_EVENT_INFO stuEventInfo = {0};
	bool bDoor1Enable = false;
	bool bDoor2Enable = false;
	
	if (cfg->CurrentAntiPassBackMode == R1R2)
	{	 
		stuInfo.nDoorList = 2;
		stuInfo.stuDoorList[0].nDoors = 1; 
		strncpy(stuInfo.stuDoorList[0].stuDoors[0].szReaderID, "1", MAX_READER_ID_LEN - 1); 
		stuInfo.stuDoorList[1].nDoors = 1; 
		strncpy(stuInfo.stuDoorList[1].stuDoors[0].szReaderID, "2", MAX_READER_ID_LEN - 1);   
		bDoor1Enable = true;
		bDoor2Enable = false;
	}
	else if (cfg->CurrentAntiPassBackMode == R1R3R2R4)
	{	 
		stuInfo.nDoorList = 2;
		stuInfo.stuDoorList[0].nDoors = 2; 
		strncpy(stuInfo.stuDoorList[0].stuDoors[0].szReaderID, "1", MAX_READER_ID_LEN - 1);
		strncpy(stuInfo.stuDoorList[0].stuDoors[1].szReaderID, "3", MAX_READER_ID_LEN - 1);  
		stuInfo.stuDoorList[1].nDoors = 2; 
		strncpy(stuInfo.stuDoorList[1].stuDoors[0].szReaderID, "2", MAX_READER_ID_LEN - 1); 
		strncpy(stuInfo.stuDoorList[1].stuDoors[1].szReaderID, "4", MAX_READER_ID_LEN - 1);  
		bDoor1Enable = true;
		bDoor2Enable = true;
	}
	else if (cfg->CurrentAntiPassBackMode == R3R4)
	{	 
		stuInfo.nDoorList = 2;
		stuInfo.stuDoorList[0].nDoors = 1; 
		strncpy(stuInfo.stuDoorList[0].stuDoors[0].szReaderID, "3", MAX_READER_ID_LEN - 1); 
		stuInfo.stuDoorList[1].nDoors = 1; 
		strncpy(stuInfo.stuDoorList[1].stuDoors[0].szReaderID, "4", MAX_READER_ID_LEN - 1);  
		bDoor1Enable = false;
		bDoor2Enable = false; 
	}

	if (!cfg->bIsActivated)
	{
		if (GetConfigFromDevice(loginID, 0, &stuEventInfo))
		{
			stuEventInfo.abRepeatEnterAlarmEnable = false;
			stuEventInfo.bRepeatEnterAlarm = FALSE;
			SetConfigToDevice(loginID, 0, &stuEventInfo);

		}
		if (GetConfigFromDevice(loginID, 1, &stuEventInfo))
		{
			stuEventInfo.abRepeatEnterAlarmEnable = false;
			stuEventInfo.bRepeatEnterAlarm = FALSE;
			SetConfigToDevice(loginID, 1, &stuEventInfo);
		}
	}
	else
	{
		if (GetConfigFromDevice(loginID, 0, &stuEventInfo))
		{

			if (bDoor1Enable)
			{

				stuEventInfo.abRepeatEnterAlarmEnable = true;
				stuEventInfo.bRepeatEnterAlarm = TRUE;
			}
			else
			{

				stuEventInfo.abRepeatEnterAlarmEnable = false;
				stuEventInfo.bRepeatEnterAlarm = FALSE;
			}
			SetConfigToDevice(loginID, 0, &stuEventInfo);
		}

		if (GetConfigFromDevice(loginID, 1, &stuEventInfo)) 
		{
			if (bDoor2Enable)
			{
				stuEventInfo.abRepeatEnterAlarmEnable = true;
				stuEventInfo.bRepeatEnterAlarm = TRUE;
			}
			else
			{
				stuEventInfo.abRepeatEnterAlarmEnable = false;
				stuEventInfo.bRepeatEnterAlarm = FALSE;
			}
			SetConfigToDevice(loginID, 1, &stuEventInfo);
		}
	}

	char szJsonBufSet[1024 * 40] = {0};

	BOOL bRet = CLIENT_PacketData(CFG_CMD_OPEN_DOOR_ROUTE, &stuInfo, sizeof(stuInfo), szJsonBufSet, sizeof(szJsonBufSet));

	if (bRet)
	{
		int nerror = 0;
		int nrestart = 0;
		CLIENT_SetNewDevConfig(loginID, CFG_CMD_OPEN_DOOR_ROUTE, 0, szJsonBufSet, 
			sizeof(szJsonBufSet), &nerror, &nrestart, SDK_API_WAITTIME);
		CLIENT_SetNewDevConfig(loginID, CFG_CMD_OPEN_DOOR_ROUTE, 1, szJsonBufSet, 
			sizeof(szJsonBufSet), &nerror, &nrestart, SDK_API_WAITTIME);		
	}

	return TRUE;
}
