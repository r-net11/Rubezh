#pragma once
#include "afxwin.h"
//#include "MyEdit.h"


// CDlgRWDevice 对话框

class  CDlgRWDevice : public CDialog
{
	DECLARE_DYNAMIC(CDlgRWDevice)

public:
	CDlgRWDevice(CWnd* pParent = NULL, LLONG hLoginID = 0, NET_DEVICE_TYPE emDevType = NET_PRODUCT_NONE);   // 标准构造函数
	virtual ~CDlgRWDevice();

// 对话框数据
	enum { IDD = IDD_DLG_RWDEVICE }; 
	CEdit m_deviceData;///设备数据存储 

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 支持

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
public:
	afx_msg void OnBnClickedDlgRwBtnRead();
	afx_msg void OnBnClickedDlgRwBtnWrite();
	afx_msg void OnEnChangeDlgRwEdtData();
};
