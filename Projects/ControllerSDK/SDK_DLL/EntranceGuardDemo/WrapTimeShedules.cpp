#include "StdAfx.h"
#include "WrapTimeShedules.h"

#include <iostream>
#include <fstream>
using namespace std;

#define QUERY_COUNT	(3)

BOOL CALL_METHOD WRAP_GetDevConfig_AccessTimeSchedule(int lLoginId, CFG_ACCESS_TIMESCHEDULE_INFO* result)
{	
	char szJsonBuf[1024 * 40] = {0};
	int nerror = 0;
	int nChannel = 0;
	BOOL bRet = CLIENT_GetNewDevConfig(lLoginId, CFG_CMD_ACCESSTIMESCHEDULE, nChannel, szJsonBuf, sizeof(szJsonBuf), &nerror, SDK_API_WAITTIME);
	int nRetLen = 0;
	CFG_ACCESS_TIMESCHEDULE_INFO stuInfo = {0};
	bRet = CLIENT_ParseData(CFG_CMD_ACCESSTIMESCHEDULE, szJsonBuf, &stuInfo, sizeof(stuInfo), &nRetLen);
	memcpy(result, &stuInfo, sizeof(stuInfo));
	return bRet;
}

BOOL CALL_METHOD WRAP_SetDevConfig_AccessTimeSchedule(int lLoginId, CFG_ACCESS_TIMESCHEDULE_INFO* stuInfo)
{	
	int nChannel = 0;
	char szJsonBufSet[1024 * 40] = {0};

	BOOL bRet = CLIENT_PacketData(CFG_CMD_ACCESSTIMESCHEDULE, stuInfo, sizeof(CFG_ACCESS_TIMESCHEDULE_INFO), szJsonBufSet, sizeof(szJsonBufSet));
	int nerror = 0;
	int nrestart = 0;
	bRet = CLIENT_SetNewDevConfig(lLoginId, CFG_CMD_ACCESSTIMESCHEDULE, nChannel, szJsonBufSet, sizeof(szJsonBufSet), &nerror, &nrestart, SDK_API_WAITTIME);
	return bRet;
}

BOOL CALL_METHOD WRAP_GetAccessTimeSchedule(int lLoginId, CFG_ACCESS_TIMESCHEDULE_INFO* result)
{	
	ofstream myfile;
	myfile.open ("D://SDKOutput.txt");

	CFG_ACCESS_TIMESCHEDULE_INFO resultStuInfo = {0};

	char szJsonBuf[1024 * 40] = {0};
	int nerror = 0;
	int nChannel = 0;

	BOOL bRet = CLIENT_GetNewDevConfig(lLoginId, CFG_CMD_ACCESSTIMESCHEDULE, nChannel, 
		szJsonBuf, sizeof(szJsonBuf), &nerror, SDK_API_WAITTIME);
	if (bRet)
	{
		myfile << "Get AccessTimeSchedule Config ok!\n";
	}
	else
	{
		myfile << "Get AccessTimeSchedule Config failed...0x%08x\n" << CLIENT_GetLastError();
	}

	if (bRet)
	{
		int nRetLen = 0;
		CFG_ACCESS_TIMESCHEDULE_INFO stuInfo = {0};
		bRet = CLIENT_ParseData(CFG_CMD_ACCESSTIMESCHEDULE, szJsonBuf,
			&stuInfo, sizeof(stuInfo), &nRetLen);
		if (bRet)
		{
			myfile << "Parse AccessTimeSchedule Config ok!\n";
		}
		else
		{
			myfile << "Parse AccessTimeSchedule Config failed!\n";
		}


		for(int i = 0; i < 7; i++)
		{
			for(int j = 0; j < 4; j++)
			{
				resultStuInfo.stuTime[i][j].dwRecordMask = stuInfo.stuTime[i][j].dwRecordMask;
				resultStuInfo.stuTime[i][j].nBeginHour = stuInfo.stuTime[i][j].nBeginHour;
				resultStuInfo.stuTime[i][j].nBeginMin = stuInfo.stuTime[i][j].nBeginMin;
				resultStuInfo.stuTime[i][j].nBeginSec = stuInfo.stuTime[i][j].nBeginSec;
				resultStuInfo.stuTime[i][j].nEndHour = stuInfo.stuTime[i][j].nEndHour;
				resultStuInfo.stuTime[i][j].nEndMin = stuInfo.stuTime[i][j].nEndMin;
				resultStuInfo.stuTime[i][j].nEndSec = stuInfo.stuTime[i][j].nEndSec;

				DWORD hour = stuInfo.stuTime[i][j].nBeginHour;
				myfile << stuInfo.stuTime[i][j].dwRecordMask;
				myfile << stuInfo.stuTime[i][j].nBeginHour;
				myfile << stuInfo.stuTime[i][j].nBeginMin;
				myfile << stuInfo.stuTime[i][j].nBeginSec;
				myfile << stuInfo.stuTime[i][j].nEndHour;
				myfile << stuInfo.stuTime[i][j].nEndMin;
				myfile << stuInfo.stuTime[i][j].nEndSec;
				myfile << "\n";
			}
		}
	}

	myfile.close();

	memcpy(result, &resultStuInfo, sizeof(CFG_ACCESS_TIMESCHEDULE_INFO));
	return bRet;
}

BOOL CALL_METHOD WRAP_SetAccessTimeSchedule(int lLoginId, CFG_ACCESS_TIMESCHEDULE_INFO timeSheduleInfo)
{
	ofstream myfile;
	myfile.open ("D://SDKOutput.txt");

	CFG_ACCESS_TIMESCHEDULE_INFO stuInfo1 = timeSheduleInfo;

	for(int i = 0; i < 7; i++)
		{
			for(int j = 0; j < 4; j++)
			{
				myfile << stuInfo1.stuTime[i][j].dwRecordMask;
				myfile << stuInfo1.stuTime[i][j].nBeginHour;
				myfile << stuInfo1.stuTime[i][j].nBeginMin;
				myfile << stuInfo1.stuTime[i][j].nBeginSec;
				myfile << stuInfo1.stuTime[i][j].nEndHour;
				myfile << stuInfo1.stuTime[i][j].nEndMin;
				myfile << stuInfo1.stuTime[i][j].nEndSec;
				myfile << "\n";
			}
		}
	myfile.close();

	char szJsonBuf[1024 * 40] = {0};
	int nerror = 0;
	int nChannel = 0;

	int nRetLen = 0;
	CFG_ACCESS_TIMESCHEDULE_INFO stuInfo = timeSheduleInfo;
	{
		char szJsonBufSet[1024 * 40] = {0};
		BOOL bRet = CLIENT_PacketData(CFG_CMD_ACCESSTIMESCHEDULE, &stuInfo, sizeof(stuInfo), szJsonBufSet, sizeof(szJsonBufSet));
		if (!bRet)
		{
			printf("Packet AccessTimeSchedule Config failed!\n");
		} 
		else
		{
			printf("Packet AccessTimeSchedule Config ok!\n");
			int nerror = 0;
			int nrestart = 0;
			bRet = CLIENT_SetNewDevConfig(lLoginId, CFG_CMD_ACCESSTIMESCHEDULE, nChannel, szJsonBufSet, sizeof(szJsonBufSet), &nerror, &nrestart, SDK_API_WAITTIME);
			if (!bRet)
			{
				printf("Set AccessTimeSchedule Config failed...0x%08x\n", CLIENT_GetLastError());
			}
			else
			{
				printf("Set AccessTimeSchedule Config ok!\n");
			}
		}
	}

	return TRUE;
}