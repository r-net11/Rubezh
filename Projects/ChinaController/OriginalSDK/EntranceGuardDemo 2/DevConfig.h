/********************************************************************
*	Copyright 2013, ZheJiang Dahua Technology Stock Co.Ltd.
* 						All Rights Reserved
*	File Name: DevConfig.h	    	
*	Author: 
*	Description: 
*	Created:	        %2013%:%12%:%30%  
*	Revision Record:    date:author:modify sth
*********************************************************************/

#if !defined(__DEVICE_CONFIG_H__)
#define __DEVICE_CONFIG_H__

// 设备类型和软件版本信息
BOOL DevConfig_TypeAndSoftInfo(LLONG lLoginID);

// IP,MASK,GATE信息
BOOL DevConfig_IPMaskGate(LLONG lLoginID);

// MAC
BOOL DevConfig_MAC(LLONG lLoginID);

// recordFinder能力集
BOOL DevConfig_RecordFinderCaps(LLONG lLoginID);

// 当前时间的获取设置
BOOL DevConfig_CurrentTime(LLONG lLoginID);

// 查询日志
BOOL Dev_QueryLogList(LLONG lLoginID);

// 门禁基本配置(CFG_CMD_ACCESS_GENERAL)
void DevConfig_AccessGeneral(LLONG lLoginId);

// 门禁配置(CFG_CMD_ACCESS_EVENT)
void DevConfig_AccessControl(LLONG lLoginId);

// 门禁时间段配置(CFG_CMD_ACCESSTIMESCHEDULE)
void DevConfig_AccessTimeSchedule(LLONG lLoginId);

// 多人组合开门配置()
void DevConfig_OpenDoorGroup_Single(LLONG lLoginId);
void DevConfig_OpenDoorGroup_Array(LLONG lLoginId);

// 开门路线集合配置()
void DevConfig_OpenDoorRoute_Single(LLONG lLoginId);
void DevConfig_OpenDoorRoute_Array(LLONG lLoginId);

// 客户自定义数据配置
void DevConfig_ClientCustomData(LLONG lLoginId);

#endif // !defined(__DEVICE_CONFIG_H__)
