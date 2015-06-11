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

// �豸���ͺ�����汾��Ϣ
BOOL DevConfig_TypeAndSoftInfo(LLONG lLoginID);

// IP,MASK,GATE��Ϣ
BOOL DevConfig_IPMaskGate(LLONG lLoginID);

// MAC
BOOL DevConfig_MAC(LLONG lLoginID);

// recordFinder������
BOOL DevConfig_RecordFinderCaps(LLONG lLoginID);

// ��ǰʱ��Ļ�ȡ����
BOOL DevConfig_CurrentTime(LLONG lLoginID);

// ��ѯ��־
BOOL Dev_QueryLogList(LLONG lLoginID);

// �Ž���������(CFG_CMD_ACCESS_GENERAL)
void DevConfig_AccessGeneral(LLONG lLoginId);

// �Ž�����(CFG_CMD_ACCESS_EVENT)
void DevConfig_AccessControl(LLONG lLoginId);

// �Ž�ʱ�������(CFG_CMD_ACCESSTIMESCHEDULE)
void DevConfig_AccessTimeSchedule(LLONG lLoginId);

#endif // !defined(__DEVICE_CONFIG_H__)
