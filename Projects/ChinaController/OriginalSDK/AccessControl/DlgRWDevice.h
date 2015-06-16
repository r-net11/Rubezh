#pragma once
#include "afxwin.h"
//#include "MyEdit.h"


// CDlgRWDevice �Ի���

class  CDlgRWDevice : public CDialog
{
	DECLARE_DYNAMIC(CDlgRWDevice)

public:
	CDlgRWDevice(CWnd* pParent = NULL, LLONG hLoginID = 0, NET_DEVICE_TYPE emDevType = NET_PRODUCT_NONE);   // ��׼���캯��
	virtual ~CDlgRWDevice();

// �Ի�������
	enum { IDD = IDD_DLG_RWDEVICE }; 
	CEdit m_deviceData;///�豸���ݴ洢 

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV ֧��

	DECLARE_MESSAGE_MAP()
	//{{AFX_MSG(CDlgSearchDeivce)
	virtual BOOL OnInitDialog(); 
	afx_msg void OnDestroy(); 
	//}}AFX_MSG 

	void			InitDlg();
	//void			InsertData(int nStartNo, const CString& strDeviceData );

private:
	LLONG			m_hLoginID;
	NET_DEVICE_TYPE	m_emDevType;

	CFG_CLIENT_CUSTOM_INFO	m_stuInfo;

	void ReadNative();
	void ReadRefactored();

	void WriteNative();
	void WriteRefactored();
public:
	afx_msg void OnBnClickedDlgRwBtnRead();
	afx_msg void OnBnClickedDlgRwBtnWrite();
	afx_msg void OnEnChangeDlgRwEdtData();
};
