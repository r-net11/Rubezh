/********************************************************************
*	Copyright 2013, ZheJiang Dahua Technology Stock Co.Ltd.
* 						All Rights Reserved
*	File Name: Commons.h	    	
*	Author: 
*	Description: 
*	Created:	        %2013%:%12%:%30%  
*	Revision Record:    date:author:modify sth
*********************************************************************/

#if !defined(__COMMON_CONSTANTS_H__)
#define __COMMON_CONSTANTS_H__

#define SDK_API_WAITTIME 5000 // 调用SDK接口超时时间

#define DEVCFG_TYPE_SOFTVERSION		1		// 设备类型、软件版本号
#define DEVCFG_IP_MASK_GATE		    2		// 设备IP,MASK,GATE

#define DEVCTL_RECORDSET_INSERT		3		// 新增记录
#define DEVCTL_RECORDSET_UPDATE		4		// 更新记录
#define DEVCTL_RECORDSET_REMOVE     5		// 删除记录
#define DEVCTL_RECORDSET_CLEAR	    6		// 清除记录
#define DEVCTL_RECORDSET_GET	    7		// 获取记录详情
#define DEVCTL_REBOOT				8		// 重启设备
#define DEVCTL_DELETE_CFGFILE		9		// 恢复默认
#define DEVCTL_OPENDOOR		        10		// 开门
#define DEVCTL_GETLOGCOUNT		    11		// 获取日志条数

#define DEVCFG_MAC					12		// 获取mac
#define DEVCAP_RECORDFINDER			13		// 获取记录查询能力集
#define DEVGET_RECORDCOUNT			14		// 获取记录条数
#define DEVCFG_CURRENTTIME			15		// 当前时间获取设置
#define DEVFIND_QUERYLOG			16		// 查询日志列表

#define DEV_RECORDSET_FIND_CARD		17		// 门禁卡记录集查询
#define DEV_RECORDSET_FIND_PWD		18		// 门禁密码记录集查询
#define DEV_RECORDSET_FIND_CARDREC	19		// 门禁刷卡记录记录集查询

#define DEVCFG_ACCESS_GENERAL		20		// 门禁基本配置
#define DEVCFG_ACCESS_CONTROL		21		// 门禁控制配置
#define DEVCFG_ACCESS_TIMESCHEDULE	22		// 门禁时间段配置

#define DEVCTL_CLOSEDOOR			23		// 关门
#define DEV_DOOR_STATUS				24		// 门禁状态


#endif // !defined(__COMMON_CONSTANTS_H__)