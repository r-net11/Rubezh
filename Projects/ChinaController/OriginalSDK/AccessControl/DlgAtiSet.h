#pragma once
#include "afxwin.h"


// CDlgAtiSet 对话框

class CDlgAtiSet : public CDialog
{
	DECLARE_DYNAMIC(CDlgAtiSet)

public:
	CDlgAtiSet(CWnd* pParent = NULL, LLONG hLoginID = 0,  int nDoorCount = 2);   // 标准构造函数
	virtual ~CDlgAtiSet();

// 对话框数据
	enum { IDD = IDD_DLG_ATISET };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 支持

	DECLARE_MESSAGE_MAP()
	virtual BOOL OnInitDialog(); 
	afx_msg void OnDestroy();  

	void			InitDlg();
private:
	LLONG			m_hLoginID;
	int	m_nDoorCount;
	CFG_OPEN_DOOR_ROUTE_INFO m_stuInfo ;
	CFG_ACCESS_EVENT_INFO	m_stuEventInfo;
	bool			GetNewDevConfig();//读取配置信息

	BOOL			GetConfigFromDevice(int nChn);//读取门的配置信息
	BOOL			SetConfigToDevice(int nChn);//写入门的配置信息


public:
	// 反潜使能
	CButton m_checkAtiEnable;
	// 读卡器-》读卡器2
	CButton m_checkR1R2; 
	CButton m_checkR1R2R3R4; 
	CButton m_checkR3R4; 
	afx_msg void OnBnClickedDlgAtiBtnOk();
	afx_msg void OnBnClickedDlgAtiBtnCancel();
	afx_msg void OnBnClickedDlgAtiCheckEnable();
};
