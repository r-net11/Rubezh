#pragma once


// CDlgSearchDeivce �Ի���

class CDlgSearchDeivce : public CDialog
{
	DECLARE_DYNAMIC(CDlgSearchDeivce)

public:
	CDlgSearchDeivce(CWnd* pParent = NULL);   // ��׼���캯��
	virtual ~CDlgSearchDeivce();

// �Ի�������
	enum { IDD = IDD_DLGSEARCHDEIVCE };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV ֧��

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

	CString csIp;//IP��ַ
	CString csPort ;//�˿ں�

	void  CheckDevExist(DEVICE_NET_INFO_EX DevNetInfo);		
private:

	LONG                m_AutoSearchDeviceHandle;
 
	DEVICE_NET_INFO_EX  m_curSearchDev;
	CString				m_DeviceMacs;//�豸��mac��ַ��	

	CListCtrl	m_devicesList;///�豸�б� 
	int		m_nDeviceCounts;//�豸��Ŀ
	afx_msg void OnBnClickedSearchBtnStart();
public:
	afx_msg void OnBnClickedSearchBtnCancel();
	afx_msg void OnBnClickedSearchBtnOk();
};
