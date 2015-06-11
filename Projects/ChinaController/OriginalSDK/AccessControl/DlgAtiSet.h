#pragma once
#include "afxwin.h"


// CDlgAtiSet �Ի���

class CDlgAtiSet : public CDialog
{
	DECLARE_DYNAMIC(CDlgAtiSet)

public:
	CDlgAtiSet(CWnd* pParent = NULL, LLONG hLoginID = 0,  int nDoorCount = 2);   // ��׼���캯��
	virtual ~CDlgAtiSet();

// �Ի�������
	enum { IDD = IDD_DLG_ATISET };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV ֧��

	DECLARE_MESSAGE_MAP()
	virtual BOOL OnInitDialog(); 
	afx_msg void OnDestroy();  

	void			InitDlg();
private:
	LLONG			m_hLoginID;
	int	m_nDoorCount;
	CFG_OPEN_DOOR_ROUTE_INFO m_stuInfo ;
	CFG_ACCESS_EVENT_INFO	m_stuEventInfo;
	bool			GetNewDevConfig();//��ȡ������Ϣ

	BOOL			GetConfigFromDevice(int nChn);//��ȡ�ŵ�������Ϣ
	BOOL			SetConfigToDevice(int nChn);//д���ŵ�������Ϣ


public:
	// ��Ǳʹ��
	CButton m_checkAtiEnable;
	// ������-��������2
	CButton m_checkR1R2; 
	CButton m_checkR1R2R3R4; 
	CButton m_checkR3R4; 
	afx_msg void OnBnClickedDlgAtiBtnOk();
	afx_msg void OnBnClickedDlgAtiBtnCancel();
	afx_msg void OnBnClickedDlgAtiCheckEnable();
};
