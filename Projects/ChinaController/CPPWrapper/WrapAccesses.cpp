
#include "StdAfx.h"
#include "WrapAccesses.h"

#include <iostream>
#include <fstream>
using namespace std;

#define QUERY_COUNT	(3)

BOOL CALL_METHOD WRAP_Get_Access_Info(int loginID, int recordNo, NET_RECORDSET_ACCESS_CTL_CARDREC* result)
{
	if (NULL == loginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	NET_RECORDSET_ACCESS_CTL_CARDREC stuAccess = {sizeof(stuAccess)};

	stuAccess.nRecNo = recordNo;
	stuInert.emType = NET_RECORD_ACCESSCTLCARDREC;
	stuInert.pBuf = &stuAccess;

	int nRet = 0;
	BOOL bRet = CLIENT_QueryDevState(loginID, DH_DEVSTATE_DEV_RECORDSET, (char*)&stuInert, sizeof(stuInert), &nRet, SDK_API_WAITTIME);

	memcpy(result, &stuAccess, sizeof(stuAccess));
	return bRet;
}

/// <summary>
/// Инициализирует процедуру получения информации о записанных на контроллере событиям по доступу
/// </summary>
/// <returns>TRUE при успешном завершении</returns>
BOOL CALL_METHOD WRAP_BeginGetAll_Accesses(int loginID, int& finderID)
{
	NET_IN_FIND_RECORD_PARAM stuIn = {sizeof(stuIn)};
	NET_OUT_FIND_RECORD_PARAM stuOut = {sizeof(stuOut)};
	
	stuIn.emType = NET_RECORD_ACCESSCTLCARDREC;
		
	if (CLIENT_FindRecord(loginID, &stuIn, &stuOut, SDK_API_WAITTIME))
	{
		finderID = stuOut.lFindeHandle;
	}
	return finderID > 0;
}

/// <summary>
/// Получает коллекцию всех записанных на контроллере событий по доступу
/// </summary>
/// <returns>количество записанных на контроллере событий по доступу</returns>
int CALL_METHOD WRAP_GetAll_Accesses(int finderID, AccessesCollection* result)
{
	AccessesCollection accessesCollection = {sizeof(AccessesCollection)};

	int i = 0, j = 0;
	int nMaxNum = 10;

	NET_IN_FIND_NEXT_RECORD_PARAM stuIn = {sizeof(stuIn)};
	stuIn.lFindeHandle = finderID;
	stuIn.nFileCount = nMaxNum;
	
	NET_OUT_FIND_NEXT_RECORD_PARAM stuOut = {sizeof(stuOut)};
	stuOut.nMaxRecordNum = nMaxNum;
	
	NET_RECORDSET_ACCESS_CTL_CARDREC* pstuAccess = new NET_RECORDSET_ACCESS_CTL_CARDREC[nMaxNum];
	if (NULL == pstuAccess)
	{
		return -1;
	}
	memset(pstuAccess, 0, sizeof(NET_RECORDSET_ACCESS_CTL_CARDREC) * nMaxNum);

	for (i = 0; i < nMaxNum; i++)
	{
		pstuAccess[i].dwSize = sizeof(NET_RECORDSET_ACCESS_CTL_CARDREC);
	}
	stuOut.pRecordList = (void*)pstuAccess;
	
	if (CLIENT_FindNextRecord(&stuIn, &stuOut, SDK_API_WAITTIME) >= 0)
    {
		for (i = 0; i < __min(stuOut.nMaxRecordNum, stuOut.nRetRecordNum); i++)
		{
			memcpy(&accessesCollection.Accesses[i], &pstuAccess[i], sizeof(NET_RECORDSET_ACCESS_CTL_CARDREC));
		}
	}

	delete[] pstuAccess;
	pstuAccess = NULL;

	memcpy(result, &accessesCollection, sizeof(AccessesCollection));
	return stuOut.nRetRecordNum;
}
