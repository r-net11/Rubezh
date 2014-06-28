/********************************************************************
*	Copyright 2013, ZheJiang Dahua Technology Stock Co.Ltd.
* 						All Rights Reserved
*	File Name: EntranceGuardDemo.cpp	    	
*	Author: 
*	Description: 
*	Created:	        %2013%:%12%:%30%  
*	Revision Record:    date:author:modify sth
*********************************************************************/

#include "StdAfx.h"
#include "DevCtrl.h"
#include "DevConfig.h"

using namespace std;

#define MENU_OPTIONS_NUM     24 // 菜单选项数
std::string g_szMenu[MENU_OPTIONS_NUM] = {
	    "1:设备类型,软件版本号",
		"2:IP,MASK,GATE",
		"3:新增记录",
		"4:更新记录",
		"5:删除记录",
		"6:清除记录",
		"7:获取记录详情",
		"8:重启设备",
		"9:恢复默认",
		"10:开门",
		"11:获取日志条数",
		"12:获取MAC",
		"13:获取记录查询能力集",
		"14:获取记录条数",
		"15:当前时间获取->设置->获取",
		"16:查询日志列表(即报警记录)",
		"17:门禁卡信息查询",
		"18:门禁密码信息查询",
		"19:刷卡记录信息查询",
		"20:门禁基本配置",
		"21:门禁控制配置",
		"22:门禁时间段配置",
		"23:关门",
		"24:门禁状态"
};
#define MENU_OPTIONS_RECORD_TYPE_NUM     4 // 记录集类型数
std::string g_szRecordSetMenu[MENU_OPTIONS_RECORD_TYPE_NUM] = {
	"1:门卡信息",
	"2:门禁密码信息",
	"3:门禁出入记录信息",
	"4:假期信息"
};

// 展示菜单
void ShowMenu()
{
	printf("=====可测功能菜单=====:\n");
	for (unsigned int i = 0; i < MENU_OPTIONS_NUM; i++)
	{
		std::string strTempMenu = g_szMenu[i];
		printf("%s\n", strTempMenu.c_str());
	}
}

// 展示记录集类型子菜单
void ShowSubMenu()
{
	printf("=====记录集类型菜单=====:\n");
	for (unsigned int i = 0; i < MENU_OPTIONS_RECORD_TYPE_NUM; i++)
	{
		std::string strTempMenu = g_szRecordSetMenu[i];
		printf("%s\n", strTempMenu.c_str());
	}
}


// 获取用户选项
int GetUserChooseFromSubMenu()
{
	ShowSubMenu();
	int nInput = -1;
	do 
	{
		printf("=====请选择: 0.退出; 1-%d 记录集类型序号=====\n", MENU_OPTIONS_RECORD_TYPE_NUM);
		cin >> nInput;
	} while (0 > nInput || nInput > MENU_OPTIONS_RECORD_TYPE_NUM);
    return nInput;
}

// 获取用户选项
int GetUserChooseFromMenu()
{
	ShowMenu();
	int nInput = -1;
	do 
	{
		printf("=====请选择: 0.退出; 1-%d 待测功能序号=====\n", MENU_OPTIONS_NUM);
		cin >> nInput;
	} while (0 > nInput || nInput > MENU_OPTIONS_NUM);
    return nInput;
}

void CALLBACK DisConnectFunc(LLONG lLoginID, char *pchDVRIP, LONG nDVRPort, LDWORD dwUser)
{
	if(dwUser == 0)
	{
		return;
	}
}

int main(int argc, char* argv[])
{
	// 初始化
	unsigned long ulUser = 0;
	printf("=====Init SDK Start=====\n");
    int nInitRet = CLIENT_Init(DisConnectFunc,ulUser);
    printf("CLIENT_Init: %d\n",nInitRet);
	printf("=====Init SDK End  =====\n\n");


	// 登录
	LLONG lRet = 0;
	printf("=====Login Start========================================\n");
	NET_DEVICEINFO deviceInfo = {0};
	int err = 0;
	while(1)
	{
		//printf("=====Please input IP,Port,Username,Password=====\n");
		//char szIP[25] = {0};
		//int nPort = 0;
		//char szUserName[25] = {0};
		//char szPwd[25] = {0};
		//std::cin >> szIP >> nPort >> szUserName >> szPwd;
		//lRet = CLIENT_Login(szIP,nPort,szUserName,szPwd,&deviceInfo,&err);

		lRet = CLIENT_Login("172.16.2.56",37777,"admin","admin",&deviceInfo,&err);

		std::cout << "CLIENT_Login return code: "<< lRet << " errorCode: "<< err << " byChanNum:" << deviceInfo.byChanNum<< std::endl;
		std::cout << "byDVRType(expect 54(NET_BSC_SERIAL)):" << (int)deviceInfo.byDVRType << std::endl;
		if (NULL == lRet)
		{
			printf("=====Login failed,Login again or not: 0-quit,1-continue;=====\n");
			int nLoginFlag = 0;
			std::cin >> nLoginFlag;
			if (nLoginFlag)
			{
				continue;
			}
			else
			{
				break;
			}
		}
		else
		{
			break;
		}
	}
	printf("=====Login End  ========================================\n\n");
	if (lRet)
    {
		// 测试业务
		printf("=====Test Start========================================\n");
		while (1)
		{
			int nChooseID = GetUserChooseFromMenu();
			if (0 == nChooseID)
			{
				break;
			}
			switch(nChooseID)
			{
				case DEVCFG_TYPE_SOFTVERSION:
					DevConfig_TypeAndSoftInfo(lRet);
                    break;
				case DEVCFG_IP_MASK_GATE:
					DevConfig_IPMaskGate(lRet);
					break;
				case DEVCTL_RECORDSET_INSERT:
					DevCtrl_InsertRecordSet(lRet, GetUserChooseFromSubMenu());
					break;
				case DEVCTL_RECORDSET_UPDATE:
					DevCtrl_UpdateRecordSet(lRet, GetUserChooseFromSubMenu());
					break;	
				case DEVCTL_RECORDSET_REMOVE:
					DevCtrl_RemoveRecordSet(lRet, GetUserChooseFromSubMenu());
					break;
				case DEVCTL_RECORDSET_CLEAR:
					DevCtrl_ClearRecordSet(lRet, GetUserChooseFromSubMenu());
					break;
				case DEVCTL_RECORDSET_GET:
					DevCtrl_GetRecordSetInfo(lRet, GetUserChooseFromSubMenu());
					break;
				case DEVCTL_REBOOT:
					DevCtrl_ReBoot(lRet);
					break;
				case DEVCTL_DELETE_CFGFILE:
					DevCtrl_DeleteCfgFile(lRet);
					break;
				case DEVCTL_OPENDOOR:
					DevCtrl_OpenDoor(lRet);
					break;
				case DEVCTL_GETLOGCOUNT:
					DevCtrl_GetLogCount(lRet);
					break;
				case DEVCFG_MAC:
					DevConfig_MAC(lRet);
					break;
				case DEVCAP_RECORDFINDER:
					DevConfig_RecordFinderCaps(lRet);
					break;
				case DEVGET_RECORDCOUNT:
					DevCtrl_GetRecordSetCount(lRet, GetUserChooseFromSubMenu());
					break;
				case DEVCFG_CURRENTTIME:
					DevConfig_CurrentTime(lRet);
					break;
				case DEVFIND_QUERYLOG:
					Dev_QueryLogList(lRet);
					break;
				case DEV_RECORDSET_FIND_CARD:
					testRecordSetFinder_Card(lRet);
					break;
				case DEV_RECORDSET_FIND_PWD:
					testRecordSetFinder_Pwd(lRet);
					break;
				case DEV_RECORDSET_FIND_CARDREC:
					testRecordSetFinder_CardRec(lRet);
					break;
				case DEVCFG_ACCESS_GENERAL:
					DevConfig_AccessGeneral(lRet);
					break;
				case DEVCFG_ACCESS_CONTROL:
					DevConfig_AccessControl(lRet);
					break;
				case DEVCFG_ACCESS_TIMESCHEDULE:
					DevConfig_AccessTimeSchedule(lRet);
					break;
				case DEVCTL_CLOSEDOOR:
					DevCtrl_CloseDoor(lRet);
					break;
				case DEV_DOOR_STATUS:
					DevState_DoorStatus(lRet);
					break;
				default:
					break;          
			}
			printf("\n=====Test Again========================================\n\n");
		}
		printf("=====Test End========================================\n\n");
    }

    // 登出
	if (lRet)
	{
		printf("=====Logout Start=======================================\n");
		BOOL bLogout = CLIENT_Logout(lRet);
		std::cout << "CLIENT_Logout return code: "<< bLogout << std::endl;
		printf("=====Logout End  =======================================\n\n");
	}
	// 释放资源
	CLIENT_Cleanup();
	printf("=====CLIENT_Cleanup=====\n");
	system("pause");
	return 0;
}

