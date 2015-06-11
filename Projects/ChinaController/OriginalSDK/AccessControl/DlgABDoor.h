#pragma once
#include "afxwin.h"


// CDlgABDoor �Ի���

class CDlgABDoor : public CDialog
{
	DECLARE_DYNAMIC(CDlgABDoor)

public:
	CDlgABDoor(CWnd* pParent = NULL, LLONG hLoginID = 0, int nDoorCount = 2);   // ��׼���캯��
	virtual ~CDlgABDoor();

// �Ի�������
	enum { IDD = IDD_DLG_ABDOOR };
    



protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV ֧��

	DECLARE_MESSAGE_MAP()
	virtual BOOL OnInitDialog(); 
	afx_msg void OnDestroy();  

	void			InitDlg();
private:
	LLONG			m_hLoginID;
	int	m_nDoorCount;

	CFG_ACCESS_GENERAL_INFO  m_stuInfo ;


	CButton	m_chkABDoorEnable;//�Ƿ����ö��Ż���


	bool			GetNewDevConfig();//��ȡ������Ϣ

public:
	afx_msg void OnBnClickedOk();
	afx_msg void OnBnClickedDlgAbBtnOk();
	afx_msg void OnBnClickedDlgAbBtnCancel();
	afx_msg void OnBnClickedDlgAbCheckEnable();
	// Dooor1 door 2 lock
	CButton m_checkD1D2;
	// Door1 door2 door 3 lock
	CButton m_checkD1D2D3;
	// Door1 door2 door3 door4 lock
	CButton m_checkD1D2D3D4;
	// Door2 door3 door4 lock
	CButton m_checkD2D3D4;
	// door1 door3 door4 lock
	CButton m_checkD1D3ANDD2D4;
	// door1 door2 door4 lock
	CButton m_checkD1D4ANDD2D3;
	// door3  door4 lock
	CButton m_checkD3D4;
};
