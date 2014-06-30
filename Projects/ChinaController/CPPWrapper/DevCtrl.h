/********************************************************************
*	Copyright 2013, ZheJiang Dahua Technology Stock Co.Ltd.
* 						All Rights Reserved
*	File Name: DevCtrl.h	    	
*	Author: 
*	Description: 
*	Created:	        %2013%:%12%:%30%  
*	Revision Record:    date:author:modify sth
*********************************************************************/

#if !defined(__DEVICE_CTRL_H__)
#define __DEVICE_CTRL_H__
// 新增
void Insert_Card(LLONG lLoginID, int& nRecrdNo);
void Insert_Pwd(LLONG lLoginID, int& nRecrdNo);
void Insert_CardRec(LLONG lLoginID, int& nRecrdNo);
void Insert_Holiday(LLONG lLoginID, int& nRecrdNo);

void ShowCardInfo(NET_RECORDSET_ACCESS_CTL_CARD& stuCard);
void ShowPwdInfo(NET_RECORDSET_ACCESS_CTL_PWD& stuAccessCtlPwd);
void ShowCardRecInfo(NET_RECORDSET_ACCESS_CTL_CARDREC& stuCardRec);
void ShowHolidayInfo(NET_RECORDSET_HOLIDAY& stuHoliday);

// 更新
void Update_Card(LLONG lLoginID);
void Update_Pwd(LLONG lLoginID);
void Update_CardRec(LLONG lLoginID);
void Update_Holiday(LLONG lLoginID);

// 新增成功返回记录编号,失败返回-1
int DevCtrl_InsertRecordSet(LLONG lLoginID, int nRecordSetType);
// 更新成功返回TRUE,失败返回FALSE
BOOL DevCtrl_UpdateRecordSet(LLONG lLoginID, int nRecordSetType);
// 删除记录
BOOL DevCtrl_RemoveRecordSet(LLONG lLoginID, int nRecordSetType);
// 清除记录
BOOL DevCtrl_ClearRecordSet(LLONG lLoginID, int nRecordSetType);
// 获取记录详情
BOOL DevCtrl_GetRecordSetInfo(LLONG lLoginID, int nRecordSetType);
// 获取记录条数
BOOL DevCtrl_GetRecordSetCount(LLONG lLoginID, int nRecordSetType);

// 开门
BOOL DevCtrl_OpenDoor(LLONG lLoginID);
// 重启设备
BOOL DevCtrl_ReBoot(LLONG lLoginID);
// 恢复默认配置
BOOL DevCtrl_DeleteCfgFile(LLONG lLoginID);
// 获取日志记录数
BOOL DevCtrl_GetLogCount(LLONG lLoginID);

// 开始查询，设置条件
// Card
void testRecordSetFind_Card(LLONG lLoginId, LLONG& lFinderId);
void testRecordSetFindNext_Card(LLONG lFinderId);

// Pwd
void testRecordSetFind_Pwd(LLONG lLoginId, LLONG& lFinderId);
void testRecordSetFindNext_Pwd(LLONG lFinderId);

// CardRec
void testRecordSetFind_CardRec(LLONG lLoginId, LLONG& lFinderId);
void testRecordSetFindNext_CardRec(LLONG lFinderId);

// Holiday ==>暂不支持
void testRecordSetFind_Holiday(LLONG lLoginId, LLONG& lFinderId);

// 获取记录数
void test_GetCountRecordSetFind(LLONG& lFinderId);

// 结束查询
void test_RecordSetFindClose(LLONG lFinderId);

void testRecordSetFinder_Card(LLONG lLoginId);
void testRecordSetFinder_Pwd(LLONG lLoginId);
void testRecordSetFinder_CardRec(LLONG lLoginId);
void testRecordSetFinder_Holiday(LLONG lLoginId);

// 关门
BOOL DevCtrl_CloseDoor(LLONG lLoginId);

// 门禁状态
void DevState_DoorStatus(LLONG lLoginId);

#endif // !defined(__DEVICE_CTRL_H__)
