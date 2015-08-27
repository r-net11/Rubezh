// DlgSearchDeivce.cpp : 实现文件
//

#include "stdafx.h"
#include "AccessControl.h"
#include "DlgSearchDeivce.h"


// CDlgSearchDeivce 对话框

IMPLEMENT_DYNAMIC(CDlgSearchDeivce, CDialog)

CDlgSearchDeivce::CDlgSearchDeivce(CWnd* pParent /*=NULL*/)
	: CDialog(CDlgSearchDeivce::IDD, pParent)
{

}

CDlgSearchDeivce::~CDlgSearchDeivce()
{
}

void CDlgSearchDeivce::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_SEARCH_LIST_DEVICES, m_devicesList);
}


BEGIN_MESSAGE_MAP(CDlgSearchDeivce, CDialog) 
	ON_NOTIFY(HDN_ITEMCHANGING, 0, &CDlgSearchDeivce::OnHdnItemchangingSearchListDevices)
	ON_BN_CLICKED(IDC_SEARCH_BTN_START, &CDlgSearchDeivce::OnBnClickedSearchBtnStart)
	ON_BN_CLICKED(IDC_SEARCH_BTN_CANCEL, &CDlgSearchDeivce::OnBnClickedSearchBtnCancel)
	ON_BN_CLICKED(IDC_SEARCH_BTN_OK, &CDlgSearchDeivce::OnBnClickedSearchBtnOk)
END_MESSAGE_MAP()

void CDlgSearchDeivce::OnHdnItemchangingSearchListDevices(NMHDR *pNMHDR, LRESULT *pResult)
{
	LPNMHEADER phdr = reinterpret_cast<LPNMHEADER>(pNMHDR);
	// TODO: 在此添加控件通知处理程序代码
	*pResult = 0;
}


BOOL CDlgSearchDeivce::OnInitDialog() 
{
	CDialog::OnInitDialog();

	// TODO: Add extra initialization here
	g_SetWndStaticText(this, DLG_SEARCHDEVICE);

	InitDlg();
	//OnQuerylogBtnTotalCount();

	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}


void CDlgSearchDeivce::InitDlg()
{
	m_devicesList.SetExtendedStyle(m_devicesList.GetExtendedStyle() | LVS_EX_FULLROWSELECT | LVS_EX_GRIDLINES);
	m_devicesList.InsertColumn(0, ConvertString("seq", DLG_SEARCHDEVICE), LVCFMT_LEFT,30,-1);
	m_devicesList.InsertColumn(1, ConvertString("type", DLG_SEARCHDEVICE), LVCFMT_LEFT,80,-1);
	m_devicesList.InsertColumn(2, ConvertString("mac", DLG_SEARCHDEVICE),LVCFMT_LEFT,100,-1);
	m_devicesList.InsertColumn(3, ConvertString("ip", DLG_SEARCHDEVICE),LVCFMT_LEFT,100,-1);
	m_devicesList.InsertColumn(4, ConvertString("subnet", DLG_SEARCHDEVICE),LVCFMT_LEFT,100,-1);
	m_devicesList.InsertColumn(5, ConvertString("gateway", DLG_SEARCHDEVICE),LVCFMT_LEFT,100,-1);
	m_devicesList.InsertColumn(6, ConvertString("port", DLG_SEARCHDEVICE),LVCFMT_LEFT,50,-1);

	GetDlgItem(IDC_SEARCH_BTN_OK)->EnableWindow(FALSE); 
	m_DeviceMacs = "";
}



void CDlgSearchDeivce::InsertListViewItem(int nStartNo, const CString& strDeviceType, const CString& strMac, const CString& strIP
											, const CString& strSubNet, const CString& strGateWay, const CString& strPort)
{
	int nIndex = m_devicesList.GetItemCount();

	CString strIndex;
	strIndex.Format("%d", nStartNo + 1);
	m_devicesList.InsertItem(nIndex, "");

	m_devicesList.SetItemText(nIndex, 0, strIndex);
	m_devicesList.SetItemText(nIndex, 1, strDeviceType);
	m_devicesList.SetItemText(nIndex, 2, strMac);
	m_devicesList.SetItemText(nIndex, 3, strIP);
	m_devicesList.SetItemText(nIndex, 4, strSubNet);
	m_devicesList.SetItemText(nIndex, 5, strGateWay);
	m_devicesList.SetItemText(nIndex, 6, strPort);
}

void CALLBACK SearchDevicesInfo(DEVICE_NET_INFO_EX *pDevNetInfo, void* pUserData)
{
	if (pDevNetInfo != NULL)
	{	
		if (strcmp(pDevNetInfo->szDeviceType,"BSC") == 0)
		{
			CDlgSearchDeivce* pDlg = (CDlgSearchDeivce*)pUserData;
			if (NULL != pDlg)
			{
				pDlg->CheckDevExist(*pDevNetInfo);				
			}			
		}
	}
}

void CDlgSearchDeivce::OnDestroy() 
{
	CDialog::OnDestroy();

	// TODO: Add your message handler code here
	/*if (m_lLogID != 0)
	{
		CLIENT_StopQueryLog(m_lLogID);
	}*/

	if (0 != m_AutoSearchDeviceHandle)
	{
		CLIENT_StopSearchDevices(m_AutoSearchDeviceHandle);		
	}
}



void CDlgSearchDeivce::OnBnClickedSearchBtnStart()
{
	// TODO: 在此添加控件通知处理程序代码

	if (0 != m_AutoSearchDeviceHandle)
	{
		CLIENT_StopSearchDevices(m_AutoSearchDeviceHandle);		
	}

	m_AutoSearchDeviceHandle = CLIENT_StartSearchDevices(SearchDevicesInfo,this);	
}



void  CDlgSearchDeivce::CheckDevExist(DEVICE_NET_INFO_EX DevNetInfo)
{
	memcpy(&m_curSearchDev,&DevNetInfo,sizeof(DEVICE_NET_INFO_EX));

	 

	CString strIP(DevNetInfo.szIP);

	 
	CString strMAC(DevNetInfo.szMac);
	CString strSubMask(DevNetInfo.szSubmask);
	CString strGateway(DevNetInfo.szGateway);
	CString strDeviceType(DevNetInfo.szDetailType);
	
	if (-1 == m_DeviceMacs.Find(strMAC + "/"))
	{

		m_DeviceMacs.Append(strMAC + "/");
		//DevNetInfo.
		CString strPort;
		if (DevNetInfo.nPort == 0)
			DevNetInfo.nPort = 37777;
		strPort.Format("%d", DevNetInfo.nPort);   
		InsertListViewItem(0,strDeviceType, strMAC , strIP,strSubMask,strGateway,strPort);
	}
	GetDlgItem(IDC_SEARCH_BTN_OK)->EnableWindow(TRUE);  
	 

}
void CDlgSearchDeivce::OnBnClickedSearchBtnCancel()
{
	// TODO: 在此添加控件通知处理程序代码
	this->OnCancel();
}

void CDlgSearchDeivce::OnBnClickedSearchBtnOk()
{
	// TODO: 在此添加控件通知处理程序代码 
	if (0 != m_AutoSearchDeviceHandle)
	{
		CLIENT_StopSearchDevices(m_AutoSearchDeviceHandle);		
	}
	int iIndex = m_devicesList.GetSelectionMark();
	if (iIndex  == -1 )
		iIndex = 0;
	if (m_devicesList.GetItemCount()> 0)
	{
		csIp = m_devicesList.GetItemText(iIndex, 3);
		csPort = m_devicesList.GetItemText(iIndex, 6);

	}

	this->OnOK();
}
