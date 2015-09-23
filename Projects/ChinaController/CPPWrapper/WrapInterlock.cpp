#include "StdAfx.h"
#include <atlstr.h>
#include "WrapInterlock.h"
#include "Wrap.h"

BOOL GetNewDevConfig(int loginID, CFG_ACCESS_GENERAL_INFO* result)
{
	int nRetLen = 0;

	char szJsonBuf[1024 * 40] = {0};	
	memset(szJsonBuf,0,1024 * 40);

	int nerror = 0;
	BOOL bRet =  CLIENT_GetNewDevConfig((LLONG)loginID, CFG_CMD_ACCESS_GENERAL, -1, szJsonBuf, 1024*40, &nerror, 5000);
	if (bRet)
	{
		CFG_ACCESS_GENERAL_INFO stuInfo = {0};
		memset(&stuInfo, 0, sizeof(CFG_ACCESS_GENERAL_INFO));
		DWORD dwRetLen = 0;
		bRet = CLIENT_ParseData(CFG_CMD_ACCESS_GENERAL, szJsonBuf, (void*)&stuInfo, sizeof(CFG_ACCESS_GENERAL_INFO), &dwRetLen);
		if (bRet)
		{
			memcpy(result, &stuInfo, sizeof(stuInfo));
			return TRUE;
		}
	}
	return FALSE;
}

/// <summary>
/// Получает настройку Interlock на контроллере
/// <param name="loginID">залогированный в SDK клиент</param>
/// <param name="result">возвращаемая настройка Interlock</param>
/// </summary>
/// <returns>TRUE при успешном завершении, FALSE в случае неудачи</returns>
BOOL SDK_CALL_METHOD WRAP_GetInterlockCfg(int loginID, WRAP_InterlockCfg* result)
{
	if (loginID == NULL)
	{
		return FALSE;
	}

	// Определяем количесво дверей на контроллере
	int nDoorsCount;
	BOOL bRet = GetDoorsCount(loginID, nDoorsCount);
	if (!bRet)
	{
		return FALSE;
	}

	WRAP_InterlockCfg cfg = {0};
	memset(&cfg, 0, sizeof(WRAP_InterlockCfg));

	cfg.AvailableInterlockModes[INTERLOCK_MODE_L1L2].InterlockMode = L1L2;
	cfg.AvailableInterlockModes[INTERLOCK_MODE_L1L2L3].InterlockMode = L1L2L3;
	cfg.AvailableInterlockModes[INTERLOCK_MODE_L1L2L3L4].InterlockMode = L1L2L3L4;
	cfg.AvailableInterlockModes[INTERLOCK_MODE_L2L3L4].InterlockMode = L2L3L4;
	cfg.AvailableInterlockModes[INTERLOCK_MODE_L1L3_L2L4].InterlockMode = L1L3_L2L4;
	cfg.AvailableInterlockModes[INTERLOCK_MODE_L1L4_L2L3].InterlockMode = L1L4_L2L3;
	cfg.AvailableInterlockModes[INTERLOCK_MODE_L3L4].InterlockMode = L3L4;

	cfg.nDoorsCount = nDoorsCount;

	if (nDoorsCount == 1)
	{
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L1L2].bIsAvailable = FALSE;
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L1L2L3].bIsAvailable = FALSE;
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L1L2L3L4].bIsAvailable = FALSE;
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L2L3L4].bIsAvailable = FALSE;
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L1L3_L2L4].bIsAvailable = FALSE;
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L1L4_L2L3].bIsAvailable = FALSE;
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L3L4].bIsAvailable = FALSE;
	}
	else if (nDoorsCount == 2)
	{
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L1L2].bIsAvailable = TRUE;
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L1L2L3].bIsAvailable = FALSE;
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L1L2L3L4].bIsAvailable = FALSE;
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L2L3L4].bIsAvailable = FALSE;
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L1L3_L2L4].bIsAvailable = FALSE;
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L1L4_L2L3].bIsAvailable = FALSE;
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L3L4].bIsAvailable = FALSE;
	}
	else if (nDoorsCount == 4)
	{
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L1L2].bIsAvailable = TRUE;
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L1L2L3].bIsAvailable = TRUE;
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L1L2L3L4].bIsAvailable = TRUE;
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L2L3L4].bIsAvailable = TRUE;
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L1L3_L2L4].bIsAvailable = TRUE;
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L1L4_L2L3].bIsAvailable = TRUE;
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L3L4].bIsAvailable = TRUE;
	}
	else
	{
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L1L2].bIsAvailable = TRUE;
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L1L2L3].bIsAvailable = FALSE;
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L1L2L3L4].bIsAvailable = FALSE;
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L2L3L4].bIsAvailable = FALSE;
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L1L3_L2L4].bIsAvailable = FALSE;
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L1L4_L2L3].bIsAvailable = FALSE;
		cfg.AvailableInterlockModes[INTERLOCK_MODE_L3L4].bIsAvailable = FALSE;
	}
	CFG_ACCESS_GENERAL_INFO stuInfo = {0};
	if (GetNewDevConfig(loginID, &stuInfo)) 
	{

		if (stuInfo.stuABLockInfo.bEnable)
		{
			cfg.bIsActivated = TRUE;
		}
		else
		{
			cfg.bIsActivated = FALSE;
		} 
		CString csAllABInfo;
		for (int i = 0; i < stuInfo.stuABLockInfo.nDoors; i ++)
		{
			CString csInfo;
			csInfo.Format("%d%d%d%d%d", stuInfo.stuABLockInfo.stuDoors[i].nDoor, stuInfo.stuABLockInfo.stuDoors[i].anDoor[0]
			, stuInfo.stuABLockInfo.stuDoors[i].anDoor[1]
			, stuInfo.stuABLockInfo.stuDoors[i].anDoor[2]
			, stuInfo.stuABLockInfo.stuDoors[i].anDoor[3]);
			csAllABInfo.Append(csInfo);
			
		}

		if (csAllABInfo == "20100")
			cfg.CurrentInterlockMode = L1L2;
		else if (csAllABInfo == "30120")
			cfg.CurrentInterlockMode = L1L2L3;
		else if (csAllABInfo == "40123")
			cfg.CurrentInterlockMode = L1L2L3L4;
		else if (csAllABInfo == "31230")
			cfg.CurrentInterlockMode = L2L3L4;
		else if (csAllABInfo == "2020021300")
			cfg.CurrentInterlockMode = L1L3_L2L4;
		else if (csAllABInfo == "2030021200")
			cfg.CurrentInterlockMode = L1L4_L2L3;
		else if (csAllABInfo == "22300")
			cfg.CurrentInterlockMode = L3L4;

		cfg.bCanActivate = TRUE;
	}
	memcpy(result, &cfg, sizeof(cfg));
	return TRUE;
}

/// <summary>
/// Записывает настройки Interlock на контроллер
/// <param name="loginID">залогированный в SDK клиент</param>
/// <param name="cfg">сохраняемая настройка Interlock</param>
/// </summary>
/// <returns>TRUE при успешном завершении, FALSE в случае неудачи</returns>
BOOL SDK_CALL_METHOD WRAP_SetInterlockCfg(int loginID, WRAP_InterlockCfg* cfg)
{
	if (!loginID)
	{
		return FALSE;
	} 
	CFG_ACCESS_GENERAL_INFO stuInfo = {0};

	if(!GetNewDevConfig(loginID, &stuInfo))
		return FALSE;
	stuInfo.abABLockInfo = true;
	stuInfo.stuABLockInfo.bEnable = cfg->bIsActivated;

	stuInfo.stuABLockInfo.nDoors = 0;
	int iDoorIndex = 0;
	if (cfg->CurrentInterlockMode == L1L2)
	{	 
		stuInfo.stuABLockInfo.nDoors++;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].nDoor = 2;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[0] = 0;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[1] = 1;
		iDoorIndex ++ ;
	}
	else if (cfg->CurrentInterlockMode == L1L2L3)
	{	 
		stuInfo.stuABLockInfo.nDoors++;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].nDoor = 3;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[0] = 0;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[1] = 1;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[2] = 2;
		iDoorIndex ++ ;
	}
	else if (cfg->CurrentInterlockMode == L1L2L3L4)
	{	 
		stuInfo.stuABLockInfo.nDoors++;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].nDoor = 4;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[0] = 0;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[1] = 1;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[2] = 2;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[3] = 3;
		iDoorIndex ++ ;
	}
	else if (cfg->CurrentInterlockMode == L2L3L4)
	{	 
		stuInfo.stuABLockInfo.nDoors++;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].nDoor = 3;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[0] = 1;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[1] = 2;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[2] = 3;
		iDoorIndex ++ ;
	}
	else if (cfg->CurrentInterlockMode == L1L3_L2L4)
	{	 
		stuInfo.stuABLockInfo.nDoors++;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].nDoor = 2;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[0] = 0;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[1] = 2;
		iDoorIndex ++ ;

		stuInfo.stuABLockInfo.nDoors++;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].nDoor = 2;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[0] = 1;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[1] = 3;
		iDoorIndex ++ ; 

	}
	else if (cfg->CurrentInterlockMode == L1L4_L2L3)
	{	 
		stuInfo.stuABLockInfo.nDoors++;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].nDoor = 2;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[0] = 0;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[1] = 3;
		iDoorIndex ++ ;

		stuInfo.stuABLockInfo.nDoors++;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].nDoor = 2;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[0] = 1;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[1] = 2;
		iDoorIndex ++ ;
	}
	else if (cfg->CurrentInterlockMode == L3L4)
	{	 
		stuInfo.stuABLockInfo.nDoors++;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].nDoor = 2;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[0] = 2;
		stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[1] = 3;
		iDoorIndex ++ ;
	} 
	 
	char szJsonBufSet[1024 * 40] = {0};

	BOOL bRet = CLIENT_PacketData(CFG_CMD_ACCESS_GENERAL, &stuInfo, sizeof(stuInfo), szJsonBufSet, sizeof(szJsonBufSet));

	if (bRet)
	{
		int nerror = 0;
		int nrestart = 0;
		bRet = CLIENT_SetNewDevConfig(loginID, CFG_CMD_ACCESS_GENERAL, -1, szJsonBufSet, sizeof(szJsonBufSet), &nerror, &nrestart, SDK_API_WAITTIME);
	} 
	return bRet;
}
