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
// ����
void Insert_Card(LLONG lLoginID, int& nRecrdNo);
void Insert_Pwd(LLONG lLoginID, int& nRecrdNo);
void Insert_CardRec(LLONG lLoginID, int& nRecrdNo);
void Insert_Holiday(LLONG lLoginID, int& nRecrdNo);

void ShowCardInfo(NET_RECORDSET_ACCESS_CTL_CARD& stuCard);
void ShowPwdInfo(NET_RECORDSET_ACCESS_CTL_PWD& stuAccessCtlPwd);
void ShowCardRecInfo(NET_RECORDSET_ACCESS_CTL_CARDREC& stuCardRec);
void ShowHolidayInfo(NET_RECORDSET_HOLIDAY& stuHoliday);

// ����
void Update_Card(LLONG lLoginID);
void Update_Pwd(LLONG lLoginID);
void Update_CardRec(LLONG lLoginID);
void Update_Holiday(LLONG lLoginID);

// �����ɹ����ؼ�¼���,ʧ�ܷ���-1
int DevCtrl_InsertRecordSet(LLONG lLoginID, int nRecordSetType);
// ���³ɹ�����TRUE,ʧ�ܷ���FALSE
BOOL DevCtrl_UpdateRecordSet(LLONG lLoginID, int nRecordSetType);
// ɾ����¼
BOOL DevCtrl_RemoveRecordSet(LLONG lLoginID, int nRecordSetType);
// �����¼
BOOL DevCtrl_ClearRecordSet(LLONG lLoginID, int nRecordSetType);
// ��ȡ��¼����
BOOL DevCtrl_GetRecordSetInfo(LLONG lLoginID, int nRecordSetType);
// ��ȡ��¼����
BOOL DevCtrl_GetRecordSetCount(LLONG lLoginID, int nRecordSetType);

// ����
BOOL DevCtrl_OpenDoor(LLONG lLoginID);
// �����豸
BOOL DevCtrl_ReBoot(LLONG lLoginID);
// �ָ�Ĭ������
BOOL DevCtrl_DeleteCfgFile(LLONG lLoginID);
// ��ȡ��־��¼��
BOOL DevCtrl_GetLogCount(LLONG lLoginID);

// ��ʼ��ѯ����������
// Card
void testRecordSetFind_Card(LLONG lLoginId, LLONG& lFinderId);
void testRecordSetFindNext_Card(LLONG lFinderId);

// Pwd
void testRecordSetFind_Pwd(LLONG lLoginId, LLONG& lFinderId);
void testRecordSetFindNext_Pwd(LLONG lFinderId);

// CardRec
void testRecordSetFind_CardRec(LLONG lLoginId, LLONG& lFinderId);
void testRecordSetFindNext_CardRec(LLONG lFinderId);

// Holiday ==>�ݲ�֧��
void testRecordSetFind_Holiday(LLONG lLoginId, LLONG& lFinderId);

// ��ȡ��¼��
void test_GetCountRecordSetFind(LLONG& lFinderId);

// ������ѯ
void test_RecordSetFindClose(LLONG lFinderId);

void testRecordSetFinder_Card(LLONG lLoginId);
void testRecordSetFinder_Pwd(LLONG lLoginId);
void testRecordSetFinder_CardRec(LLONG lLoginId);
void testRecordSetFinder_Holiday(LLONG lLoginId);

// ����
BOOL DevCtrl_CloseDoor(LLONG lLoginId);

// �Ž�״̬
void DevState_DoorStatus(LLONG lLoginId);

#endif // !defined(__DEVICE_CTRL_H__)
