/********************************************************************
*	Copyright 2013, ZheJiang Dahua Technology Stock Co.Ltd.
* 						All Rights Reserved
*	File Name: DevConfig.cpp	    	
*	Author: 
*	Description: 
*	Created:	        %2013%:%12%:%30%  
*	Revision Record:    date:author:modify sth
*********************************************************************/
#include "StdAfx.h"
#include "DevConfig.h"

#include <iostream>
#include <fstream>
using namespace std;

/************************************************************************/
/* ������Ľӿڲ��ԣ���Ϊ�������֣�                                     */
/* 1.�����޸ĵ����ã�ֱ�ӻ�ȡ��                                         */
/* 2.���޸ĵ����ã���ȡ->>�޸Ĳ����ֶ�->>����->>��ȡ->>�ȶԽ����       */
/************************************************************************/

// �豸���ͺ�����汾��Ϣ
BOOL DevConfig_TypeAndSoftInfo(LLONG lLoginID)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	// ��ȡ
	DHDEV_VERSION_INFO stuInfo = {0};
	int nRet = 0;
	BOOL bRet = CLIENT_QueryDevState(lLoginID, DH_DEVSTATE_SOFTWARE, (char*)&stuInfo, sizeof(stuInfo), &nRet, 3000);
	if (bRet)
	{
		printf("�豸��ϸ�ͺ�: %s\r\n����汾: %s\r\n����ʱ��: %04d-%02d-%02d\r\n",
			stuInfo.szDevType,
			stuInfo.szSoftWareVersion,
			((stuInfo.dwSoftwareBuildDate>>16) & 0xffff),
			((stuInfo.dwSoftwareBuildDate>>8) & 0xff),
			(stuInfo.dwSoftwareBuildDate & 0xff));
	}
	else
	{
		printf("CLIENT_QueryDevState(DH_DEVSTATE_SOFTWARE)ʧ��\n");
	}
	return TRUE;
}

// IP,MASK,GATE��Ϣ
BOOL DevConfig_IPMaskGate(LLONG lLoginID)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	// ��ȡ
	int nError = 0;
	char szBuffer[32 * 1024] = {0};
	BOOL bRet = CLIENT_GetNewDevConfig(lLoginID, CFG_CMD_NETWORK, -1, szBuffer, 32 * 1024, &nError, 3000);
	if (bRet)
	{
		printf("=====receive data: \n%s\n", szBuffer);
		int nRetLen = 0;
		CFG_NETWORK_INFO stuNetwork = {0};
		// ����
		bRet = CLIENT_ParseData(CFG_CMD_NETWORK, szBuffer,
			&stuNetwork, sizeof(stuNetwork), &nRetLen);
		if (bRet)
		{
			printf(" nInterfaceNum: %d\n szDefInterface: %s\n ", stuNetwork.nInterfaceNum, stuNetwork.szDefInterface);
			if (stuNetwork.nInterfaceNum)
			{
				printf(" szIP: %s\n szSubnetMask: %s\n szDefGateway: %s\n nMTU: %d\n", 
					stuNetwork.stuInterfaces->szIP, 
					stuNetwork.stuInterfaces->szSubnetMask, 
					stuNetwork.stuInterfaces->szDefGateway, 
					stuNetwork.stuInterfaces->nMTU);
			}
		}
		else
		{
			printf("Parse CFG_CMD_NETWORK config failed!");
		}
		// �����޸�
		std::string strTemp = "172.5.2.65";
		memset(stuNetwork.stuInterfaces[0].szIP, 0, MAX_ADDRESS_LEN);
		memcpy(stuNetwork.stuInterfaces[0].szIP, strTemp.c_str(), strTemp.length());
		strTemp = "255.255.0.0";
		memset(stuNetwork.stuInterfaces[0].szSubnetMask, 0, MAX_ADDRESS_LEN);
		memcpy(stuNetwork.stuInterfaces[0].szSubnetMask, strTemp.c_str(), strTemp.length());
		strTemp = "172.5.0.1";
		memset(stuNetwork.stuInterfaces[0].szDefGateway, 0, MAX_ADDRESS_LEN);
		memcpy(stuNetwork.stuInterfaces[0].szDefGateway, strTemp.c_str(), strTemp.length());
		stuNetwork.stuInterfaces->nMTU = 1000;

		// ��װ
		char szOutBuffer[32 * 1024] = {0};
		int nRestart = 0;
		bRet = CLIENT_PacketData(CFG_CMD_NETWORK, &stuNetwork, sizeof(stuNetwork), szOutBuffer, 32 * 1024);
		if (bRet)
		{
			// �·�
			bRet = CLIENT_SetNewDevConfig(lLoginID, CFG_CMD_NETWORK, -1, szOutBuffer, 32 * 1024, &nError, &nRestart, 3000);
			if (bRet)
			{
				printf("CLIENT_SetNewDevConfig CFG_CMD_NETWORK �ɹ�!\n");
			}
			else
			{
				printf("CLIENT_SetNewDevConfig CFG_CMD_NETWORK config ʧ��!\n");
			}

		}
		memset(szBuffer, 0, 32 * 1024);
		// �ٴλ�ȡ
		BOOL bRet = CLIENT_GetNewDevConfig(lLoginID, CFG_CMD_NETWORK, -1, szBuffer, 32 * 1024, &nError, 3000);
		if (bRet)
		{
			printf("=====receive data again: \n%s\n", szBuffer);
			int nRetLen = 0;
			CFG_NETWORK_INFO stuNetwork = {0};
			// ����
			bRet = CLIENT_ParseData(CFG_CMD_NETWORK, szBuffer,
				&stuNetwork, sizeof(stuNetwork), &nRetLen);
			if (bRet)
			{
				printf("nInterfaceNum: %d\n szDefInterface: %s\n ", stuNetwork.nInterfaceNum, stuNetwork.szDefInterface);
				if (stuNetwork.nInterfaceNum)
				{
					printf(" szIP: %s\n szSubnetMask: %s\n szDefGateway: %s\n nMTU: %d\n", 
						stuNetwork.stuInterfaces->szIP, 
						stuNetwork.stuInterfaces->szSubnetMask, 
						stuNetwork.stuInterfaces->szDefGateway, 
						stuNetwork.stuInterfaces->nMTU);
				}
			}
			else
			{
				printf("Parse CFG_CMD_NETWORK config failed!");
			}
		}
		else
		{
			printf("CLIENT_GetNewDevConfig(Network)ʧ��\n");
		}
	}
	return TRUE;
}

BOOL DevConfig_MAC(LLONG lLoginID)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	// ��ȡ
	DHDEV_NETINTERFACE_INFO stuInfo = {sizeof(stuInfo)};
	int nRet = 0;
	BOOL bRet = CLIENT_QueryDevState(lLoginID, DH_DEVSTATE_NETINTERFACE, (char*)&stuInfo, sizeof(stuInfo), &nRet, 3000);
	if (bRet)
	{
		printf("mac: %s\r\n", stuInfo.szMAC);
	}
	else
	{
		printf("CLIENT_QueryDevState(DH_DEVSTATE_NETINTERFACE)ʧ��\n");
	}
	return TRUE;
}

BOOL DevConfig_RecordFinderCaps(LLONG lLoginID)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	int nError = 0;
	char szBuffer[32 * 1024] = {0};
	memset(szBuffer, 0, 32 * 1024);
	// ��ȡ
	BOOL bRet = CLIENT_QueryNewSystemInfo(lLoginID, CFG_CAP_CMD_RECORDFINDER, -1, szBuffer, 32 * 1024, &nError, SDK_API_WAITTIME);
	if (bRet)
	{
		printf("=====receive data : \n%s\n", szBuffer);
		int nRetLen = 0;
		CFG_CAP_RECORDFINDER_INFO stuCaps = {0};
		// ����
		bRet = CLIENT_ParseData(CFG_CAP_CMD_RECORDFINDER, szBuffer,
			&stuCaps, sizeof(stuCaps), &nRetLen);
		if (bRet)
		{
			printf("stuCaps.nMaxPageSize: %d\n ", stuCaps.nMaxPageSize);
		}
		else
		{
			printf("Parse CFG_CAP_CMD_RECORDFINDER config failed!");
		}
	}
	else
	{
		printf("CLIENT_QueryNewSystemInfo(CFG_CAP_CMD_RECORDFINDER)ʧ��\n");
	}
	return TRUE;
}

BOOL DevConfig_CurrentTime(LLONG lLoginID)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	NET_TIME stuNetTime = {0};
	BOOL bRet = CLIENT_QueryDeviceTime(lLoginID, &stuNetTime, SDK_API_WAITTIME);
	if (bRet)
	{
		printf("Get Device Time succeed: %04d-%02d-%02d %02d:%02d:%02d\n", 
			stuNetTime.dwYear,
			stuNetTime.dwMonth,
			stuNetTime.dwDay,
			stuNetTime.dwHour,
			stuNetTime.dwMinute,
			stuNetTime.dwSecond);
		// �޸Ĳ���ֵ
		stuNetTime.dwYear = 2014;
		stuNetTime.dwMonth = 1;
		stuNetTime.dwDay = 17;
		stuNetTime.dwHour = 16;
		stuNetTime.dwMinute = 32;
		stuNetTime.dwSecond = 00;
		
		BOOL bRet = CLIENT_SetupDeviceTime(lLoginID, &stuNetTime);
		if (bRet)
		{
			printf("Set Device Time succeed!");
			// �ٴλ�ȡ
			memset(&stuNetTime, 0, sizeof(stuNetTime));
			bRet = CLIENT_QueryDeviceTime(lLoginID, &stuNetTime, SDK_API_WAITTIME);
			if (bRet)
			{
				printf("Get Device Time again succeed: %04d-%02d-%02d %02d:%02d:%02d\n", 
					stuNetTime.dwYear,
					stuNetTime.dwMonth,
					stuNetTime.dwDay,
					stuNetTime.dwHour,
					stuNetTime.dwMinute,
					stuNetTime.dwSecond);
			}
			else
			{
				printf("Get Device Time again failed: %08x...", CLIENT_GetLastError());
			}
		}
		else
		{
			printf("Set Device Time failed: %08x...", CLIENT_GetLastError());
		}
	}
	else
	{
		printf("Get Device Time failed: %08x...", CLIENT_GetLastError());
	}
	return TRUE;

}

void DisPlayLogInfo(DH_DEVICE_LOG_ITEM_EX& stuLogInfo, int nIndex)
{
//strLogTime.Format("%d-%d-%d %d:%d:%d", stuTime.year+2000, stuTime.month, stuTime.day, stuTime.hour, stuTime.minute, stuTime.second);
	printf("===��־��Ϣ:%d\n", nIndex + 1);
	printf("stuLogInfo.nLogType:%d\n", stuLogInfo.nLogType);

	printf("stuLogInfo.stuOperateTime: %04d-%02d-%02d %02d:%02d:%02d\n", 
		stuLogInfo.stuOperateTime.year + 2000,
		stuLogInfo.stuOperateTime.month,
		stuLogInfo.stuOperateTime.day,
		stuLogInfo.stuOperateTime.hour,
		stuLogInfo.stuOperateTime.minute,
		stuLogInfo.stuOperateTime.second);
	//printf("stuLogInfo.szOperator:%s\n", stuLogInfo.szOperator);
	printf("stuLogInfo.szOperation:%s\n", stuLogInfo.szOperation);
	printf("stuLogInfo.szDetailContext:%s\n", stuLogInfo.szDetailContext);				
	printf("===��־��Ϣ\n");
}

BOOL Dev_QueryLogList(LLONG lLoginID)
{
	if (NULL == lLoginID)
	{
		return FALSE;
	}
	// �����ѯ����
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
	
	stuGetLog.nLogStuType = 1;// �Ž�����ΪEX��־�ṹ��,��1����(���ݴ���)

	// ָ��Ҫ������
	int nMaxNum = 50;
	stuGetLog.nStartNum = 40;  // ��һ�β�ѯ����0��ʼ
	stuGetLog.nEndNum = nMaxNum - 1;
	
	DH_DEVICE_LOG_ITEM_EX* szLogInfos = new DH_DEVICE_LOG_ITEM_EX[32];
	int nRetLogNum = 0;
	BOOL bRet = CLIENT_QueryDeviceLog(lLoginID, &stuGetLog, (char*)szLogInfos, 32 * sizeof(DH_DEVICE_LOG_ITEM_EX), &nRetLogNum, SDK_API_WAITTIME);
	if (bRet)
	{
		//display log info
		if (nRetLogNum <= 0)
		{
			printf("No Log record!");
		}
		else
		{
			for (unsigned int i = 0; i < nRetLogNum; i++)
			{
				DisPlayLogInfo(szLogInfos[i], i);
			}
		}
		
		// δ���꣬������ʼ��ţ��ٴβ�ѯ
		if (nRetLogNum < nMaxNum)
		{
			memset(szLogInfos, 0, sizeof(DH_DEVICE_LOG_ITEM_EX) * 32);
			stuGetLog.nStartNum += nRetLogNum;  
			nRetLogNum = 0;
			bRet = CLIENT_QueryDeviceLog(lLoginID, &stuGetLog, (char*)szLogInfos, 32 * sizeof(DH_DEVICE_LOG_ITEM_EX), &nRetLogNum, SDK_API_WAITTIME);
			if (bRet)
			{
				// û�м�¼
				if (nRetLogNum <= 0)
				{
					printf("No more Log record!");
				}
				else
				{
					for (unsigned int i = 0; i < nRetLogNum; i++)
					{
						DisPlayLogInfo(szLogInfos[i], i);
					}
				}
			}
		}
	}
	delete[] szLogInfos;
	return TRUE;
}
//////////////////////////////////////////////////////////////////////////
//
// AccessGeneral config
//
//////////////////////////////////////////////////////////////////////////
void DevConfig_AccessGeneral(LLONG lLoginId)
{
	char szJsonBuf[1024 * 40] = {0};
	int nerror = 0;
	
	// ��ȡ
	BOOL bRet = CLIENT_GetNewDevConfig(lLoginId, CFG_CMD_ACCESS_GENERAL, -1, 
		szJsonBuf, sizeof(szJsonBuf), &nerror, SDK_API_WAITTIME);
	if (bRet)
	{
		printf("Get Access General Config ok!\n");
	}
	else
	{
		printf("Get Access General Config failed...0x%08x\n", CLIENT_GetLastError());
	}
	
	// ����
	if (bRet)
	{
		int nRetLen = 0;
		CFG_ACCESS_GENERAL_INFO stuInfo = {0};
		bRet = CLIENT_ParseData(CFG_CMD_ACCESS_GENERAL, szJsonBuf,
			&stuInfo, sizeof(stuInfo), &nRetLen);
		if (bRet)
		{
			printf("Parse Access General Config ok!\n");
		}
		else
		{
			printf("Parse Access General Config failed!\n");
		}
		
		// ����
		if (bRet)
		{
			char szJsonBufSet[1024 * 40] = {0};
			
			{
				// �޸Ĳ�����������
				stuInfo.abProjectPassword = true;

				const char szPwd[] = "11111111";
				strncpy(stuInfo.szProjectPassword, szPwd, __min(strlen(szPwd), sizeof(stuInfo.szProjectPassword)));
			}
			
			BOOL bRet = CLIENT_PacketData(CFG_CMD_ACCESS_GENERAL, &stuInfo, sizeof(stuInfo), szJsonBufSet, sizeof(szJsonBufSet));
			if (!bRet)
			{
				printf("Packet Access General Config failed!\n");
			} 
			else
			{
				printf("Packet Access General Config ok!\n");
				int nerror = 0;
				int nrestart = 0;
				bRet = CLIENT_SetNewDevConfig(lLoginId, CFG_CMD_ACCESS_GENERAL, -1, szJsonBufSet, sizeof(szJsonBufSet), &nerror, &nrestart, SDK_API_WAITTIME);
				if (!bRet)
				{
					printf("Set AccessT General Config failed...0x%08x\n", CLIENT_GetLastError());
				}
				else
				{
					printf("Set Access General Config ok!\n");
				}
			}
		}
	}
}
//////////////////////////////////////////////////////////////////////////
//
// AccessControl config
//
//////////////////////////////////////////////////////////////////////////
void DevConfig_AccessControl(LLONG lLoginId)
{
	char szJsonBuf[1024 * 40] = {0};
	int nerror = 0;
	int nChannel = 0;
	
	// ��ȡ
	BOOL bRet = CLIENT_GetNewDevConfig(lLoginId, CFG_CMD_ACCESS_EVENT, nChannel, 
		szJsonBuf, sizeof(szJsonBuf), &nerror, SDK_API_WAITTIME);
	if (bRet)
	{
		printf("Get Access Control Config ok!\n");
	}
	else
	{
		printf("Get Access Control Config failed...0x%08x\n", CLIENT_GetLastError());
	}
	
	// ����
	if (bRet)
	{
		int nRetLen = 0;
		CFG_ACCESS_EVENT_INFO stuGeneralInfo = {0};
		bRet = CLIENT_ParseData(CFG_CMD_ACCESS_EVENT, szJsonBuf,
			&stuGeneralInfo, sizeof(stuGeneralInfo), &nRetLen);
		if (bRet)
		{
			printf("Parse Access Control Config ok!\n");
		}
		else
		{
			printf("Parse Access Control Config failed!\n");
		}
		
		// ����
		if (bRet)
		{
			char szJsonBufSet[1024 * 40] = {0};
			
			{
				stuGeneralInfo.abDoorOpenMethod = FALSE;
				stuGeneralInfo.nUnlockHoldInterval = 555;
			}
			
			BOOL bRet = CLIENT_PacketData(CFG_CMD_ACCESS_EVENT, &stuGeneralInfo, sizeof(stuGeneralInfo), szJsonBufSet, sizeof(szJsonBufSet));
			if (!bRet)
			{
				printf("Packet Access Control Config failed!\n");
			} 
			else
			{
				printf("Packet Access Control Config ok!\n");
				int nerror = 0;
				int nrestart = 0;
				bRet = CLIENT_SetNewDevConfig(lLoginId, CFG_CMD_ACCESS_EVENT, nChannel, szJsonBufSet, sizeof(szJsonBufSet), &nerror, &nrestart, SDK_API_WAITTIME);
				if (!bRet)
				{
					printf("Set Access Control Config failed...0x%08x\n", CLIENT_GetLastError());
				}
				else
				{
					printf("Set Access Control Config ok!\n");
				}
			}
		}
	}
}
//////////////////////////////////////////////////////////////////////////
//
// AccessTimeSchedule config
//
//////////////////////////////////////////////////////////////////////////
void DevConfig_AccessTimeSchedule(LLONG lLoginId)
{	
	char szJsonBuf[1024 * 40] = {0};
	int nerror = 0;
	int nChannel = 0;

	// ��ȡ
	BOOL bRet = CLIENT_GetNewDevConfig(lLoginId, CFG_CMD_ACCESSTIMESCHEDULE, nChannel, 
		szJsonBuf, sizeof(szJsonBuf), &nerror, SDK_API_WAITTIME);
	if (bRet)
	{
		printf("Get AccessTimeSchedule Config ok!\n");
	}
	else
	{
		printf("Get AccessTimeSchedule Config failed...0x%08x\n", 
			CLIENT_GetLastError());
	}

	// ����
	if (bRet)
	{
		int nRetLen = 0;
		CFG_ACCESS_TIMESCHEDULE_INFO stuInfo = {0};
		bRet = CLIENT_ParseData(CFG_CMD_ACCESSTIMESCHEDULE, szJsonBuf,
			&stuInfo, sizeof(stuInfo), &nRetLen);
		if (bRet)
		{
			printf("Parse AccessTimeSchedule Config ok!\n");
		}
		else
		{
			printf("Parse AccessTimeSchedule Config failed!\n");
		}

	ofstream myfile;
	myfile.open ("D://EXE_SDK_Output.txt");
		
	for(int i = 0; i < 7; i++)
	{
		//CFG_TIME_SECTION xxx[4];
		//xxx[0].nBeginHour = stuInfo.stuTime[0, 0]->nBeginHour;
		for(int j = 0; j < 4; j++)
		{
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

	myfile.close();
		
		// ����
		//if (bRet)
		{
			char szJsonBufSet[1024 * 40] = {0};


			for(int i = 0; i < 7; i++)
			{
				for(int j = 0; j < 4; j++)
				{
					stuInfo.stuTime[i][j].nBeginHour = 7;
					stuInfo.stuTime[i][j].nBeginMin = 20;
					stuInfo.stuTime[i][j].nBeginSec;
					stuInfo.stuTime[i][j].nEndHour = 20;
					stuInfo.stuTime[i][j].nEndMin = 30;
					stuInfo.stuTime[i][j].nEndSec;
				}
			}


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
	}
}