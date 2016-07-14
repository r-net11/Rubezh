#pragma once


// CDlgSearchDeivce 对话框

class CDlgSearchDeivce : public CDialog
{
	DECLARE_DYNAMIC(CDlgSearchDeivce)

public:
	CDlgSearchDeivce(CWnd* pParent = NULL);   // 标准构造函数
	virtual ~CDlgSearchDeivce();

// 对话框数据
	enum { IDD = IDD_DLGSEARCHDEIVCE };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 支持

	DECLARE_MESSAGE_MAP()
	//{{AFX_MSG(CDlgSearchDeivce)
	virtual BOOL OnInitDialog(); 
	afx_msg void OnDestroy(); 
	//}}AFX_MSG 

	void			InitDlg();
	void			InsertListViewItem(int nStartNo, const CString& strDeviceType, const CString& strMac, const CString& strIP
		, const CString& strSubNet, const CString& strGateWay, const CString& strPort);

public: 
	afx_msg void OnHdnItemchangingSearchListDevices(NMHDR *pNMHDR, LRESULT *pResult);

	CString csIp;//IP地址
	CString csPort ;//端口号

	void  CheckDevExist(DEVICE_NET_INFO_EX DevNetInfo);		
private:

	LONG                m_AutoSearchDeviceHandle;
 
	DEVICE_NET_INFO_EX  m_curSearchDev;
	CString				m_DeviceMacs;//设备的mac地址集	

	CListCtrl	m_devicesList;///设备列表 
	int		m_nDeviceCounts;//设备数目
	afx_msg void OnBnClickedSearchBtnStart();
public:
	afx_msg void OnBnClickedSearchBtnCancel();
	afx_msg void OnBnClickedSearchBtnOk();
};
