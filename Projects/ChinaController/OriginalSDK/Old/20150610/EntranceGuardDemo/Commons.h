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

#define SDK_API_WAITTIME 5000 // ����SDK�ӿڳ�ʱʱ��

#define DEVCFG_TYPE_SOFTVERSION		1		// �豸���͡�����汾��
#define DEVCFG_IP_MASK_GATE		    2		// �豸IP,MASK,GATE

#define DEVCTL_RECORDSET_INSERT		3		// ������¼
#define DEVCTL_RECORDSET_UPDATE		4		// ���¼�¼
#define DEVCTL_RECORDSET_REMOVE     5		// ɾ����¼
#define DEVCTL_RECORDSET_CLEAR	    6		// �����¼
#define DEVCTL_RECORDSET_GET	    7		// ��ȡ��¼����
#define DEVCTL_REBOOT				8		// �����豸
#define DEVCTL_DELETE_CFGFILE		9		// �ָ�Ĭ��
#define DEVCTL_OPENDOOR		        10		// ����
#define DEVCTL_GETLOGCOUNT		    11		// ��ȡ��־����

#define DEVCFG_MAC					12		// ��ȡmac
#define DEVCAP_RECORDFINDER			13		// ��ȡ��¼��ѯ������
#define DEVGET_RECORDCOUNT			14		// ��ȡ��¼����
#define DEVCFG_CURRENTTIME			15		// ��ǰʱ���ȡ����
#define DEVFIND_QUERYLOG			16		// ��ѯ��־�б�

#define DEV_RECORDSET_FIND_CARD		17		// �Ž�����¼����ѯ
#define DEV_RECORDSET_FIND_PWD		18		// �Ž������¼����ѯ
#define DEV_RECORDSET_FIND_CARDREC	19		// �Ž�ˢ����¼��¼����ѯ

#define DEVCFG_ACCESS_GENERAL		20		// �Ž���������
#define DEVCFG_ACCESS_CONTROL		21		// �Ž���������
#define DEVCFG_ACCESS_TIMESCHEDULE	22		// �Ž�ʱ�������

#define DEVCTL_CLOSEDOOR			23		// ����
#define DEV_DOOR_STATUS				24		// �Ž�״̬


#endif // !defined(__COMMON_CONSTANTS_H__)