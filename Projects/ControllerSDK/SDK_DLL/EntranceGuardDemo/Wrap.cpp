#include "StdAfx.h"
#include "Wrap.h"

#include <iostream>
#include <fstream>
using namespace std;
 
#define QUERY_COUNT	(3)

BOOL CALL_METHOD WRAP_DevConfig_TypeAndSoftInfo(int lLoginID, WRAP_DevConfig_TypeAndSoftInfo_Result* result)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}

	DHDEV_VERSION_INFO stuInfo = {0};
	int nRet = 0;
	BOOL bRet = CLIENT_QueryDevState(lLoginID, DH_DEVSTATE_SOFTWARE, (char*)&stuInfo, sizeof(stuInfo), &nRet, 3000);
	if (bRet)
	{
		strncpy(result->szDevType, stuInfo.szDevType, sizeof(stuInfo.szDevType));
		strncpy(result->szSoftWareVersion, stuInfo.szSoftWareVersion, sizeof(stuInfo.szSoftWareVersion));
		result->dwSoftwareBuildDate_1 = (stuInfo.dwSoftwareBuildDate>>16) & 0xffff;
		result->dwSoftwareBuildDate_2 = (stuInfo.dwSoftwareBuildDate>>8) & 0xff;
		result->dwSoftwareBuildDate_3 = stuInfo.dwSoftwareBuildDate & 0xff;
	}
	return bRet;
}

BOOL CALL_METHOD WRAP_Get_DevConfig_IPMaskGate(int lLoginID, WRAP_CFG_NETWORK_INFO_Result* result)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}

	int nError = 0;
	char szBuffer[32 * 1024] = {0};
	BOOL bRet = CLIENT_GetNewDevConfig(lLoginID, CFG_CMD_NETWORK, -1, szBuffer, 32 * 1024, &nError, 3000);
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

BOOL CALL_METHOD WRAP_Set_DevConfig_IPMaskGate(int lLoginID, char* ip, char* mask, char* gate, int mtu)
{
	if (NULL == lLoginID)
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
		bRet = CLIENT_SetNewDevConfig(lLoginID, CFG_CMD_NETWORK, -1, szOutBuffer, 32 * 1024, &nError, &nRestart, 3000);
		return bRet;
	}
	return FALSE;
}

BOOL CALL_METHOD WRAP_DevConfig_MAC(int lLoginID, WRAP_DevConfig_MAC_Result* result)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}

	DHDEV_NETINTERFACE_INFO stuInfo = {sizeof(stuInfo)};
	int nRet = 0;
	BOOL bRet = CLIENT_QueryDevState(lLoginID, DH_DEVSTATE_NETINTERFACE, (char*)&stuInfo, sizeof(stuInfo), &nRet, 3000);
	if (bRet)
	{
		strncpy(result->szMAC, stuInfo.szMAC, sizeof(stuInfo.szMAC));
	}
	return bRet;
}

BOOL CALL_METHOD WRAP_DevConfig_RecordFinderCaps(int lLoginID, WRAP_DevConfig_RecordFinderCaps_Result* result)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	int nError = 0;
	char szBuffer[32 * 1024] = {0};
	memset(szBuffer, 0, 32 * 1024);
	BOOL bRet = CLIENT_QueryNewSystemInfo(lLoginID, CFG_CAP_CMD_RECORDFINDER, -1, szBuffer, 32 * 1024, &nError, SDK_API_WAITTIME);
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

BOOL CALL_METHOD WRAP_DevConfig_GetCurrentTime(int lLoginID, NET_TIME* result)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	NET_TIME stuNetTime = {0};
	BOOL bRet = CLIENT_QueryDeviceTime(lLoginID, &stuNetTime, SDK_API_WAITTIME);
	result->dwYear = stuNetTime.dwYear;
	result->dwMonth = stuNetTime.dwMonth;
	result->dwDay = stuNetTime.dwDay;
	result->dwHour = stuNetTime.dwHour;
	result->dwMinute = stuNetTime.dwMinute;
	result->dwSecond = stuNetTime.dwSecond;
	return bRet;
}

BOOL CALL_METHOD WRAP_DevConfig_SetCurrentTime(int lLoginID, int dwYear, int dwMonth, int dwDay, int dwHour, int dwMinute, int dwSecond)
{
	if (NULL == lLoginID)
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
	BOOL bRet = CLIENT_SetupDeviceTime(lLoginID, &stuNetTime);
	return bRet;
}

BOOL CALL_METHOD WRAP_Dev_QueryLogList(int lLoginID, WRAP_Dev_QueryLogList_Result* result)
{
	printf("WRAP_Dev_QueryLogList");

	if (NULL == lLoginID)
	{
		return FALSE;
	}
	QUERY_DEVICE_LOG_PARAM stuGetLog;
	memset(&stuGetLog, 0, sizeof(QUERY_DEVICE_LOG_PARAM));
	stuGetLog.emLogType = DHLOG_ALL;
	stuGetLog.stuStartTime.dwYear = 2013;
	stuGetLog.stuStartTime.dwMonth = 10;
	stuGetLog.stuStartTime.dwDay = 1;
	stuGetLog.stuStartTime.dwHour = 0;
	stuGetLog.stuStartTime.dwMinute = 0;
	stuGetLog.stuStartTime.dwSecond = 0;
	
	stuGetLog.stuEndTime.dwYear = 2015;
	stuGetLog.stuEndTime.dwMonth = 10;
	stuGetLog.stuEndTime.dwDay = 7;
	stuGetLog.stuEndTime.dwHour = 0;
	stuGetLog.stuEndTime.dwMinute = 0;
	stuGetLog.stuEndTime.dwSecond = 0;
	
	stuGetLog.nLogStuType = 1;


	int nMaxNum = 10;
	stuGetLog.nStartNum = 0;
	stuGetLog.nEndNum = nMaxNum - 1;
	
	DH_DEVICE_LOG_ITEM_EX* szLogInfos = new DH_DEVICE_LOG_ITEM_EX[32];
	int nRetLogNum = 0;
	BOOL bRet = CLIENT_QueryDeviceLog(lLoginID, &stuGetLog, (char*)szLogInfos, 32 * sizeof(DH_DEVICE_LOG_ITEM_EX), &nRetLogNum, SDK_API_WAITTIME);
	if (bRet)
	{
		if (nRetLogNum <= 0)
		{
			//printf("No Log record!");
		}
		else
		{
			for (unsigned int i = 0; i < nRetLogNum; i++)
			{
				result->Test = 123;
				result->Logs[i].nLogType = szLogInfos[i].nLogType;
				result->Logs[i].stuOperateTime.day = szLogInfos[i].stuOperateTime.day;
				result->Logs[i].stuOperateTime.hour = szLogInfos[i].stuOperateTime.hour;
				result->Logs[i].stuOperateTime.minute = szLogInfos[i].stuOperateTime.minute;
				result->Logs[i].stuOperateTime.month = szLogInfos[i].stuOperateTime.month;
				result->Logs[i].stuOperateTime.second = szLogInfos[i].stuOperateTime.second;
				result->Logs[i].stuOperateTime.year = szLogInfos[i].stuOperateTime.year;
				strncpy(result->Logs[i].szOperator, szLogInfos[i].szOperator, sizeof(szLogInfos[i].szOperator));
				strncpy(result->Logs[i].szOperation, szLogInfos[i].szOperation, sizeof(szLogInfos[i].szOperation));
				strncpy(result->Logs[i].szDetailContext, szLogInfos[i].szDetailContext, sizeof(szLogInfos[i].szDetailContext));
			}
		}
		
		if (nRetLogNum < nMaxNum)
		{
			memset(szLogInfos, 0, sizeof(DH_DEVICE_LOG_ITEM_EX) * 32);
			stuGetLog.nStartNum += nRetLogNum;  
			nRetLogNum = 0;
			bRet = CLIENT_QueryDeviceLog(lLoginID, &stuGetLog, (char*)szLogInfos, 32 * sizeof(DH_DEVICE_LOG_ITEM_EX), &nRetLogNum, SDK_API_WAITTIME);
			if (bRet)
			{
				if (nRetLogNum <= 0)
				{
					//printf("No more Log record!");
				}
				else
				{
					for (unsigned int i = 0; i < nRetLogNum; i++)
					{
						//DisPlayLogInfo(szLogInfos[i], i);
					}
				}
			}
		}
	}
	delete[] szLogInfos;
	return TRUE;
}

BOOL CALL_METHOD WRAP_GetProjectPassword(int lLoginId, WRAP_GeneralConfig_Password* result)
{
	char szJsonBuf[1024 * 40] = {0};
	int nerror = 0;
	
	BOOL bRet = CLIENT_GetNewDevConfig(lLoginId, CFG_CMD_ACCESS_GENERAL, -1, szJsonBuf, sizeof(szJsonBuf), &nerror, SDK_API_WAITTIME);

	if (bRet)
	{
		int nRetLen = 0;
		CFG_ACCESS_GENERAL_INFO stuInfo = {0};
		bRet = CLIENT_ParseData(CFG_CMD_ACCESS_GENERAL, szJsonBuf, &stuInfo, sizeof(stuInfo), &nRetLen);
		strcpy(result->szProjectPassword, stuInfo.szProjectPassword);
		return bRet;
	}
	return FALSE;
}

BOOL CALL_METHOD WRAP_SetProjectPassword(int lLoginId, char password[])
{
	char szJsonBufSet[1024 * 40] = {0};
	int nerror = 0;
	int nrestart = 0;

	CFG_ACCESS_GENERAL_INFO stuInfo = {sizeof(stuInfo)};
	stuInfo.abProjectPassword = true;
	strncpy(stuInfo.szProjectPassword, password, __min(strlen(password), sizeof(stuInfo.szProjectPassword)));
	BOOL bRet = CLIENT_PacketData(CFG_CMD_ACCESS_GENERAL, &stuInfo, sizeof(stuInfo), szJsonBufSet, sizeof(szJsonBufSet));
	bRet = CLIENT_SetNewDevConfig(lLoginId, CFG_CMD_ACCESS_GENERAL, -1, szJsonBufSet, sizeof(szJsonBufSet), &nerror, &nrestart, SDK_API_WAITTIME);
	return bRet;
}

BOOL CALL_METHOD WRAP_GetDoorConfiguration(int lLoginId, int channelNo, CFG_ACCESS_EVENT_INFO* result)
{
	char szJsonBuf[1024 * 40] = {0};
	int nerror = 0;

	BOOL bRet = CLIENT_GetNewDevConfig(lLoginId, CFG_CMD_ACCESS_EVENT, channelNo, szJsonBuf, sizeof(szJsonBuf), &nerror, SDK_API_WAITTIME);

	if (bRet)
	{
		int nRetLen = 0;
		CFG_ACCESS_EVENT_INFO stuGeneralInfo = {0};
		bRet = CLIENT_ParseData(CFG_CMD_ACCESS_EVENT, szJsonBuf, &stuGeneralInfo, sizeof(stuGeneralInfo), &nRetLen);
		memcpy(result, &stuGeneralInfo, sizeof(stuGeneralInfo));
		return bRet;
	}
	return FALSE;
}

BOOL CALL_METHOD WRAP_SetDoorConfiguration(int lLoginId, int channelNo, CFG_ACCESS_EVENT_INFO* stuGeneralInfo)
{
	char szJsonBufSet[1024 * 40] = {0};
			
	BOOL bRet = CLIENT_PacketData(CFG_CMD_ACCESS_EVENT, stuGeneralInfo, sizeof(CFG_ACCESS_EVENT_INFO), szJsonBufSet, sizeof(szJsonBufSet));
	if (bRet)
	{
		int nerror = 0;
		int nrestart = 0;
		bRet = CLIENT_SetNewDevConfig(lLoginId, CFG_CMD_ACCESS_EVENT, channelNo, szJsonBufSet, sizeof(szJsonBufSet), &nerror, &nrestart, SDK_API_WAITTIME);
		return bRet;
	}
}

BOOL CALL_METHOD WRAP_DevCtrl_ReBoot(int lLoginID)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_REBOOT, NULL, SDK_API_WAITTIME);
	return TRUE;
}

BOOL CALL_METHOD WRAP_DevCtrl_DeleteCfgFile(int lLoginID)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RESTOREDEFAULT, NULL, SDK_API_WAITTIME);
	return TRUE;
}

int CALL_METHOD WRAP_DevCtrl_GetLogCount(int lLoginID, QUERY_DEVICE_LOG_PARAM* logParam)
{
	if (NULL == lLoginID)
	{
		return 0;
	}
	NET_IN_GETCOUNT_LOG_PARAM stuInLogCount = {sizeof(NET_IN_GETCOUNT_LOG_PARAM)};
	NET_OUT_GETCOUNT_LOG_PARAM stuOutLogCount = {sizeof(NET_OUT_GETCOUNT_LOG_PARAM)};

	QUERY_DEVICE_LOG_PARAM& stuGetLog = stuInLogCount.stuQueryCondition;
	stuGetLog.stuStartTime.dwYear = logParam->stuStartTime.dwYear;
	stuGetLog.stuStartTime.dwMonth = logParam->stuStartTime.dwMonth;
	stuGetLog.stuStartTime.dwDay = logParam->stuStartTime.dwDay;
	stuGetLog.stuStartTime.dwHour = logParam->stuStartTime.dwHour;
	stuGetLog.stuStartTime.dwMinute = logParam->stuStartTime.dwMinute;
	stuGetLog.stuStartTime.dwSecond = logParam->stuStartTime.dwSecond;
	
	stuGetLog.stuEndTime.dwYear = logParam->stuEndTime.dwYear;
	stuGetLog.stuEndTime.dwMonth = logParam->stuEndTime.dwMonth;
	stuGetLog.stuEndTime.dwDay = logParam->stuEndTime.dwDay;
	stuGetLog.stuEndTime.dwHour = logParam->stuEndTime.dwHour;
	stuGetLog.stuEndTime.dwMinute = logParam->stuEndTime.dwMinute;
	stuGetLog.stuEndTime.dwSecond = logParam->stuEndTime.dwSecond;

	stuGetLog.nStartNum = logParam->nStartNum;
	stuGetLog.nLogStuType = logParam->nLogStuType;
	stuGetLog.nEndNum = logParam->nEndNum;
	
	BOOL bResult = CLIENT_QueryDevLogCount(lLoginID, &stuInLogCount, &stuOutLogCount, SDK_API_WAITTIME);
 	return stuOutLogCount.nLogCount;
}

BOOL CALL_METHOD WRAP_DevCtrl_RemoveRecordSet(int lLoginID, int nRecordNo, int nRecordSetType)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	stuInert.pBuf = &nRecordNo;
	stuInert.nBufLen = sizeof(nRecordNo);
	switch (nRecordSetType)
	{
	case 1:
		stuInert.emType = NET_RECORD_ACCESSCTLCARD;
		break;
	case 2:
		stuInert.emType = NET_RECORD_ACCESSCTLPWD;
		break;
	case 3:
		stuInert.emType = NET_RECORD_ACCESSCTLCARDREC;
		break;
	case 4:
		stuInert.emType = NET_RECORD_ACCESSCTLHOLIDAY;
		break;
	default:
		break;
	}
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_REMOVE, &stuInert, SDK_API_WAITTIME);
	return bResult;
}

BOOL CALL_METHOD WRAP_DevCtrl_ClearRecordSet(int lLoginID, int nRecordSetType)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	switch (nRecordSetType)
	{
	case 1:
		stuInert.emType = NET_RECORD_ACCESSCTLCARD;
		break;
	case 2:
		stuInert.emType = NET_RECORD_ACCESSCTLPWD;
		break;
	case 3:
		stuInert.emType = NET_RECORD_ACCESSCTLCARDREC;
		break;
	case 4:
		stuInert.emType = NET_RECORD_ACCESSCTLHOLIDAY;
		break;
	default:
		break;
	}
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_RECORDSET_CLEAR, &stuInert, SDK_API_WAITTIME);
	return bResult;
}



//////////////////////////////////////////////////////////////////////////
//
// Rec
//
//////////////////////////////////////////////////////////////////////////


//void WRAP_testRecordSetFindNext_CardRec(LLONG lFinderId)
//{
//	NET_IN_FIND_NEXT_RECORD_PARAM stuIn = {sizeof(stuIn)};
//	stuIn.lFindeHandle = lFinderId;
//	stuIn.nFileCount = QUERY_COUNT;
//	
//	NET_OUT_FIND_NEXT_RECORD_PARAM stuOut = {sizeof(stuOut)};
//	stuOut.nMaxRecordNum = stuIn.nFileCount;
//	
//	NET_RECORDSET_ACCESS_CTL_CARDREC stuCardRec[QUERY_COUNT] = {0};
//	for (int i = 0; i < sizeof(stuCardRec)/sizeof(stuCardRec[0]); i++)
//	{
//		stuCardRec[i].dwSize = sizeof(NET_RECORDSET_ACCESS_CTL_CARDREC);
//	}
//	stuOut.pRecordList = (void*)&stuCardRec[0];
//	
//	if (CLIENT_FindNextRecord(&stuIn, &stuOut, SDK_API_WAITTIME) >= 0)
//	{
//		printf("WRAP_testRecordSetFindNext_CardRec ok!\n");
//		
//		for (int j = 0; j < stuOut.nRetRecordNum; j++)
//		{
//			printf("Status:%s, Method:%d, RecNo:%d, CardNo:%s, Pwd:%s\n",
//				stuCardRec[j].bStatus ? "ok" : "fail",
//				stuCardRec[j].emMethod,
//				stuCardRec[j].nRecNo,
//				stuCardRec[j].szCardNo,
//				stuCardRec[j].szPwd);
//		}
//	}
//	else
//	{
//		printf("WRAP_testRecordSetFindNext_CardRec failed:0x%08x!\n", CLIENT_GetLastError());
//	}	
//}
//////////////////////////////////////////////////////////////////////////
//
// Holiday
//
//////////////////////////////////////////////////////////////////////////




//////////////////////////////////////////////////////////////////////////
//
// closeFind
//
//////////////////////////////////////////////////////////////////////////


//void WRAP_testRecordSetFinder_Pwd(LLONG lLoginId)
//{
//	LLONG lFinderID = 0;
//
//	FIND_RECORD_ACCESSCTLPWD_CONDITION stuParam = {sizeof(FIND_RECORD_ACCESSCTLPWD_CONDITION)};
//	strcpy(stuParam.szUserID, "1357924680");
//
//	WRAP_testRecordSetFind_Pwd(lLoginId, lFinderID, stuParam);
//	if (lFinderID != 0)
//	{
//		WRAP_testRecordSetFindNext_Pwd(lFinderID);
//		WRAP_test_RecordSetFindClose(lFinderID);
//	}	
//}

//void WRAP_testRecordSetFinder_CardRec(LLONG lLoginId)
//{
//	LLONG lFinderID = 0;
//
//	FIND_RECORD_ACCESSCTLCARDREC_CONDITION stuParam = {sizeof(FIND_RECORD_ACCESSCTLCARDREC_CONDITION)};
//	strcpy(stuParam.szCardNo, "987654321");
//	stuParam.stStartTime.dwYear = 2013;
//	stuParam.stStartTime.dwMonth = 1;
//	stuParam.stStartTime.dwDay = 2;
//	stuParam.stStartTime.dwHour = 3;
//	stuParam.stStartTime.dwMinute = 4;
//	stuParam.stStartTime.dwSecond = 5;
//	stuParam.stEndTime.dwYear = 2014;
//	stuParam.stEndTime.dwMonth = 2;
//	stuParam.stEndTime.dwDay = 3;
//	stuParam.stEndTime.dwHour = 4;
//	stuParam.stEndTime.dwMinute = 5;
//	stuParam.stEndTime.dwSecond = 6;
//
//	WRAP_testRecordSetFind_CardRec(lLoginId, lFinderID, stuParam);
//	if (lFinderID != 0)
//	{
//		WRAP_testRecordSetFindNext_CardRec(lFinderID);
//		WRAP_test_RecordSetFindClose(lFinderID);
//	}
//}

//void WRAP_testRecordSetFinder_Holiday(LLONG lLoginId)
//{
//	// 暂不支持
//// 	LLONG lFinderID = 0;
//// 	WRAP_testRecordSetFind_Holiday(lLoginId, lFinderID);
//// 	if (lFinderID != 0)
//// 	{
//// 		WRAP_testRecordSetFindNext_Holiday(lFinderID);
//// 		WRAP_test_RecordSetFindClose(lFinderID);
//// 	}	
//}





//
//
//BOOL DevCtrl_GetRecordSetInfo(LLONG lLoginID, int nRecordSetType, int nRecordNo)
//{
//	if (NULL == lLoginID)
//	{
//		return FALSE;
//	}
//	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
//	NET_RECORDSET_ACCESS_CTL_CARD stuCard = {sizeof(stuCard)};
//	NET_RECORDSET_ACCESS_CTL_PWD stuAccessCtlPwd = {sizeof(stuAccessCtlPwd)};
//	NET_RECORDSET_ACCESS_CTL_CARDREC stuCardRec = {sizeof(stuCardRec)};
//	NET_RECORDSET_HOLIDAY stuHoliday = {sizeof(stuHoliday)};
//	switch (nRecordSetType)
//	{
//	case 1:
//		stuCard.nRecNo = nRecordNo;
//		stuInert.emType = NET_RECORD_ACCESSCTLCARD;
//		stuInert.pBuf = &stuCard;
//		break;
//	case 2:
//		stuAccessCtlPwd.nRecNo = nRecordNo;
//		stuInert.emType = NET_RECORD_ACCESSCTLPWD;
//		stuInert.pBuf = &stuAccessCtlPwd;
//		break;
//	case 3:
//		stuCardRec.nRecNo = nRecordNo;
//		stuInert.emType = NET_RECORD_ACCESSCTLCARDREC;
//		stuInert.pBuf = &stuCardRec;
//		break;
//	case 4:
//		stuHoliday.nRecNo = nRecordNo;
//		stuInert.emType = NET_RECORD_ACCESSCTLHOLIDAY;
//		stuInert.pBuf = &stuHoliday;
//		break;
//	default:
//		break;
//	}
//	// 获取
//	int nRet = 0;
//	BOOL bRet = CLIENT_QueryDevState(lLoginID, DH_DEVSTATE_DEV_RECORDSET, (char*)&stuInert, sizeof(stuInert), &nRet, 3000);
//	if (bRet)
//	{
//		printf("CLIENT_QueryDevState(DH_DEVSTATE_DEV_RECORDSET)成功\n");
//		//switch (nRecordSetType)
//		//{
//		// 	case 1:
//		//		ShowCardInfo(stuCard);
//		// 		break;
//		// 	case 2:
//		//		ShowPwdInfo(stuAccessCtlPwd);
//		// 		break;
//		// 	case 3:
//		//		ShowCardRecInfo(stuCardRec);
//		// 		break;
//		// 	case 4:
//		//		ShowHolidayInfo(stuHoliday);
//		// 		break;
//		// 	default:
//		// 		break;
//		//}
//	}
//	else
//	{
//		printf("CLIENT_QueryDevState(DH_DEVSTATE_DEV_RECORDSET)失败\n");
//	}
//
//	return TRUE;
//}
//
//
//
//



//
//
//void WRAP_ShowCardInfo(NET_RECORDSET_ACCESS_CTL_CARD& stuCard)
//{
//	unsigned int i = 0;
//	printf("===门禁卡信息:\n");
//	printf("stuCard.nRecNo:%d\n", stuCard.nRecNo);
//	printf("stuCard.stuCreateTime: %04d-%02d-%02d %02d:%02d:%02d\n", 
//		stuCard.stuCreateTime.dwYear,
//		stuCard.stuCreateTime.dwMonth,
//		stuCard.stuCreateTime.dwDay,
//		stuCard.stuCreateTime.dwHour,
//		stuCard.stuCreateTime.dwMinute,
//		stuCard.stuCreateTime.dwSecond);
//	printf("stuCard.szCardNo:%s\n", stuCard.szCardNo);
//	printf("stuCard.szUserID:%s\n", stuCard.szUserID);
//	printf("stuCard.emStatus:%d\n", stuCard.emStatus);
//	printf("stuCard.emType:%d\n", stuCard.emType);
//	printf("stuCard.szPsw:%s\n", stuCard.szPsw);
//	printf("stuCard.nDoorNum:%d\n", stuCard.nDoorNum);
//	for (i = 0; i < stuCard.nDoorNum; i++)
//	{
//		printf("stuCard.nDoorNum[%d]:%d\n", i, stuCard.sznDoors[i]);
//	}
//	printf("stuCard.nTimeSectionNum:%d\n", stuCard.nTimeSectionNum);
//	for (i = 0; i < stuCard.nTimeSectionNum; i++)
//	{
//		printf("stuCard.sznTimeSectionNo[%d]:%d\n", i, stuCard.sznTimeSectionNo[i]);
//	}
//	printf("stuCard.nUserTime:%d\n", stuCard.nUserTime);
//	printf("stuCard.stuValidStartTime: %04d-%02d-%02d %02d:%02d:%02d\n", 
//		stuCard.stuValidStartTime.dwYear,
//		stuCard.stuValidStartTime.dwMonth,
//		stuCard.stuValidStartTime.dwDay,
//		stuCard.stuValidStartTime.dwHour,
//		stuCard.stuValidStartTime.dwMinute,
//		stuCard.stuValidStartTime.dwSecond);
//	printf("stuCard.stuValidEndTime: %04d-%02d-%02d %02d:%02d:%02d\n", 
//		stuCard.stuValidEndTime.dwYear,
//		stuCard.stuValidEndTime.dwMonth,
//		stuCard.stuValidEndTime.dwDay,
//		stuCard.stuValidEndTime.dwHour,
//		stuCard.stuValidEndTime.dwMinute,
//		stuCard.stuValidEndTime.dwSecond);
//	printf("stuCard.bIsValid:%d\n", stuCard.bIsValid);
//	printf("===门禁卡信息\n");
//}

//void WRAP_ShowPwdInfo(NET_RECORDSET_ACCESS_CTL_PWD& stuAccessCtlPwd)
//{
//	unsigned int i = 0;
//	printf("===门禁密码信息:\n");
//	printf("stuAccessCtlPwd.nRecNo:%d\n", stuAccessCtlPwd.nRecNo);
//	printf("stuAccessCtlPwd.stuCreateTime: %04d-%02d-%02d %02d:%02d:%02d\n", 
//		stuAccessCtlPwd.stuCreateTime.dwYear,
//		stuAccessCtlPwd.stuCreateTime.dwMonth,
//		stuAccessCtlPwd.stuCreateTime.dwDay,
//		stuAccessCtlPwd.stuCreateTime.dwHour,
//		stuAccessCtlPwd.stuCreateTime.dwMinute,
//		stuAccessCtlPwd.stuCreateTime.dwSecond);
//	printf("stuAccessCtlPwd.szUserID:%s\n", stuAccessCtlPwd.szUserID);
//	printf("stuAccessCtlPwd.szDoorOpenPwd:%s\n", stuAccessCtlPwd.szDoorOpenPwd);
//	printf("stuAccessCtlPwd.szAlarmPwd:%s\n", stuAccessCtlPwd.szAlarmPwd);				
//	printf("stuAccessCtlPwd.nDoorNum:%d\n", stuAccessCtlPwd.nDoorNum);
//	for (i = 0; i < stuAccessCtlPwd.nDoorNum; i++)
//	{
//		printf("stuAccessCtlPwd.nDoorNum[%d]:%d\n", i, stuAccessCtlPwd.sznDoors[i]);
//	}
//	printf("===门禁密码信息\n");
//}

//void WRAP_ShowCardRecInfo(NET_RECORDSET_ACCESS_CTL_CARDREC& stuCardRec)
//{
//	unsigned int i = 0;
//	printf("===门禁刷卡记录信息:\n");
//	printf("stuCardRec.nRecNo:%d\n", stuCardRec.nRecNo);
//	printf("stuCardRec.szCardNo:%s\n", stuCardRec.szCardNo);
//	printf("stuCardRec.szPwd:%s\n", stuCardRec.szPwd);
//	printf("stuCardRec.stuTime: %04d-%02d-%02d %02d:%02d:%02d\n", 
//		stuCardRec.stuTime.dwYear,
//		stuCardRec.stuTime.dwMonth,
//		stuCardRec.stuTime.dwDay,
//		stuCardRec.stuTime.dwHour,
//		stuCardRec.stuTime.dwMinute,
//		stuCardRec.stuTime.dwSecond);	
//	printf("stuCardRec.bStatus:%d\n", stuCardRec.bStatus);
//	printf("stuCardRec.emMethod:%d\n", stuCardRec.emMethod);
//	printf("stuCardRec.nDoor:%d\n", stuCardRec.nDoor);
//	printf("===门禁刷卡记录信息\n");
//}

//void WRAP_ShowHolidayInfo(NET_RECORDSET_HOLIDAY& stuHoliday)
//{
//	unsigned int i = 0;
//	printf("===假日记录信息:\n");
//	printf("stuHoliday.nRecNo:%d\n", stuHoliday.nRecNo);
//	printf("stuHoliday.nDoorNum:%d\n", stuHoliday.nDoorNum);
//	for (i = 0; i < stuHoliday.nDoorNum; i++)
//	{
//		printf("stuHoliday.nDoorNum[%d]:%d\n", i, stuHoliday.sznDoors[i]);
//	}
//	printf("stuHoliday.stuStartTime: %04d-%02d-%02d %02d:%02d:%02d\n", 
//		stuHoliday.stuStartTime.dwYear,
//		stuHoliday.stuStartTime.dwMonth,
//		stuHoliday.stuStartTime.dwDay,
//		stuHoliday.stuStartTime.dwHour,
//		stuHoliday.stuStartTime.dwMinute,
//		stuHoliday.stuStartTime.dwSecond);
//	printf("stuHoliday.stuEndTime: %04d-%02d-%02d %02d:%02d:%02d\n", 
//		stuHoliday.stuEndTime.dwYear,
//		stuHoliday.stuEndTime.dwMonth,
//		stuHoliday.stuEndTime.dwDay,
//		stuHoliday.stuEndTime.dwHour,
//		stuHoliday.stuEndTime.dwMinute,
//		stuHoliday.stuEndTime.dwSecond);
//	printf("stuHoliday.bEnable:%d\n", stuHoliday.bEnable);
//	printf("===假日记录信息\n");
//
//}

//BOOL WRAP_DevCtrl_CloseDoor(LLONG lLoginId)
//{
//	if (NULL == lLoginId)
//	{
//		return FALSE;
//	}
//	NET_CTRL_ACCESS_CLOSE stuParam = {sizeof(stuParam)};
//	stuParam.nChannelID = 12;
//    BOOL bResult = CLIENT_ControlDevice(lLoginId, DH_CTRL_ACCESS_CLOSE, &stuParam, SDK_API_WAITTIME);
//    printf("CLIENT_ControlDevice CloseDoor result: %s\n", bResult ? "success" : "failed");
//	return TRUE;
//}
//
//void WRAP_DevState_DoorStatus(LLONG lLoginId)
//{
//	if (NULL == lLoginId)
//	{
//		return;
//	}
//	NET_DOOR_STATUS_INFO stuParam = {sizeof(stuParam)};
//	stuParam.nChannel = 14;
//	int nRetLen = 0;
//	BOOL bResult = CLIENT_QueryDevState(lLoginId, DH_DEVSTATE_DOOR_STATE, (char*)&stuParam, sizeof(stuParam), &nRetLen, SDK_API_WAITTIME);
//	if (bResult)
//	{
//		switch (stuParam.emStateType)
//		{
//		case EM_NET_DOOR_STATUS_OPEN:
//			printf("Door %d is open!\n", stuParam.nChannel);
//			break;
//		case EM_NET_DOOR_STATUS_CLOSE:
//			printf("Door %d is closed!\n", stuParam.nChannel);
//			break;
//		default:
//			printf("Door %d status is unknown!\n", stuParam.nChannel);
//			break;
//		}
//	}
//	else
//	{
//		printf("CLIENT_ControlDevice query door %d status failed:%08x\n", stuParam.nChannel, CLIENT_GetLastError());
//	}
//}

BOOL CALL_METHOD WRAP_DevCtrl_OpenDoor(int lLoginID)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	NET_CTRL_ACCESS_OPEN stuInert = {sizeof(stuInert)};
	stuInert.nChannelID = 0;
    BOOL bResult = CLIENT_ControlDevice(lLoginID, DH_CTRL_ACCESS_OPEN, &stuInert, SDK_API_WAITTIME);
	return bResult;
}

BOOL CALL_METHOD WRAP_DevCtrl_CloseDoor(int lLoginId)
{
	if (NULL == lLoginId)
	{
		return FALSE;
	}
	NET_CTRL_ACCESS_CLOSE stuParam = {sizeof(stuParam)};
	stuParam.nChannelID = 0;
    BOOL bResult = CLIENT_ControlDevice(lLoginId, DH_CTRL_ACCESS_CLOSE, &stuParam, SDK_API_WAITTIME);
	return bResult;
}

int CALL_METHOD WRAP_DevState_DoorStatus(int lLoginId)
{
	if (NULL == lLoginId)
	{
		return - 1;
	}
	NET_DOOR_STATUS_INFO stuParam = {sizeof(stuParam)};
	stuParam.nChannel = 0;
	int nRetLen = 0;
	BOOL bResult = CLIENT_QueryDevState(lLoginId, DH_DEVSTATE_DOOR_STATE, (char*)&stuParam, sizeof(stuParam), &nRetLen, SDK_API_WAITTIME);
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