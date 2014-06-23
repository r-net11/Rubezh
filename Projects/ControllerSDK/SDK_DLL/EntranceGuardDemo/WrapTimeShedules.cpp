#include "StdAfx.h"
#include "WrapTimeShedules.h"

#include <iostream>
#include <fstream>
using namespace std;

#define QUERY_COUNT	(3)

BOOL CALL_METHOD WRAP_GetTimeSchedule(int lLoginId, int index, CFG_ACCESS_TIMESCHEDULE_INFO* result)
{	
	char szJsonBuf[1024 * 40] = {0};
	int nerror = 0;
	int nChannel = index;
	BOOL bRet = CLIENT_GetNewDevConfig(lLoginId, CFG_CMD_ACCESSTIMESCHEDULE, nChannel, szJsonBuf, sizeof(szJsonBuf), &nerror, SDK_API_WAITTIME);
	int nRetLen = 0;
	CFG_ACCESS_TIMESCHEDULE_INFO stuInfo = {0};
	bRet = CLIENT_ParseData(CFG_CMD_ACCESSTIMESCHEDULE, szJsonBuf, &stuInfo, sizeof(stuInfo), &nRetLen);
	memcpy(result, &stuInfo, sizeof(stuInfo));
	return bRet;
}

BOOL CALL_METHOD WRAP_SetTimeSchedule(int lLoginId, int index, CFG_ACCESS_TIMESCHEDULE_INFO* stuInfo)
{	
	int nChannel = index;
	char szJsonBufSet[1024 * 40] = {0};

	BOOL bRet = CLIENT_PacketData(CFG_CMD_ACCESSTIMESCHEDULE, stuInfo, sizeof(CFG_ACCESS_TIMESCHEDULE_INFO), szJsonBufSet, sizeof(szJsonBufSet));
	int nerror = 0;
	int nrestart = 0;
	bRet = CLIENT_SetNewDevConfig(lLoginId, CFG_CMD_ACCESSTIMESCHEDULE, nChannel, szJsonBufSet, sizeof(szJsonBufSet), &nerror, &nrestart, SDK_API_WAITTIME);
	return bRet;
}

//ofstream myfile;
//myfile.open ("D://SDKOutput.txt");
//myfile << stuInfo.stuTime[i][j].nEndSec;
//myfile << "\n";
//myfile.close();
