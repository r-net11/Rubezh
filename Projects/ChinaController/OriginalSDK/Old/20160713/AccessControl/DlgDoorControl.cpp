// DlgDoorControl.cpp : implementation file
//

#include "stdafx.h"
#include "AccessControl.h"
#include "DlgDoorControl.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDlgDoorControl dialog


CDlgDoorControl::CDlgDoorControl(CWnd* pParent /* = NULL */, LLONG lLoginID /* = 0 */,int nOperType 
								 ,int nChanelID,EM_NET_DOOR_STATUS_TYPE   emStateType)
	: CDialog(CDlgDoorControl::IDD, pParent)
{
	//{{AFX_DATA_INIT(CDlgDoorControl)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
	m_lLoginID = lLoginID;
	m_nOperType = nOperType;
	m_nChanelID = nChanelID;
	m_emStateType = emStateType;
}


void CDlgDoorControl::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDlgDoorControl)
	DDX_Control(pDX, IDC_DOORCTL_CMB_STATUS, m_cmbDoorStatus);
	DDX_Control(pDX, IDC_DOORCTL_CMB_CHN, m_cmbChannel);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CDlgDoorControl, CDialog)
	//{{AFX_MSG_MAP(CDlgDoorControl)
	ON_BN_CLICKED(IDC_DOORCTL_BTN_OPEN, OnDoorctlBtnOpen)
	ON_BN_CLICKED(IDC_DOORCTL_BTN_CLOSE, OnDoorctlBtnClose)
	ON_BN_CLICKED(IDC_DOORCTL_BTN_QUERY, OnDoorctlBtnQuery)
	//}}AFX_MSG_MAP
	ON_BN_CLICKED(IDC_DOORCTL_BTN_Waring, &CDlgDoorControl::OnBnClickedDoorctlBtnWaring)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDlgDoorControl private method

void CDlgDoorControl::InitDlg()
{
	m_cmbChannel.ResetContent();
	for (int i = 0; i < 4; i++)
	{
		CString csInfo;
		csInfo.Format("%s%d", ConvertString("Channel", DLG_DOOR_CONTROL), i + 1);
		m_cmbChannel.InsertString(-1, csInfo);
	}
	m_cmbChannel.SetCurSel(0);

	m_cmbDoorStatus.ResetContent();
	for (int i = 0; i < sizeof(stuDemoDoorStatus)/sizeof(stuDemoDoorStatus[0]); i++)
	{
		m_cmbDoorStatus.InsertString(-1, ConvertString(stuDemoDoorStatus[i].szInfo, DLG_DOOR_CONTROL));
	}
	m_cmbDoorStatus.SetCurSel(-1);
}


/////////////////////////////////////////////////////////////////////////////
// CDlgDoorControl message handlers

BOOL CDlgDoorControl::OnInitDialog() 
{
	CDialog::OnInitDialog();
	g_SetWndStaticText(this, DLG_DOOR_CONTROL);
	// TODO: Add extra initialization here
	InitDlg();

	if (0 == m_nOperType )//
	{

		GetDlgItem(IDC_DOORCTL_BTN_Waring)->EnableWindow(TRUE);
		GetDlgItem(IDC_DOORCTL_BTN_CLOSE)->EnableWindow(TRUE);
		GetDlgItem(IDC_DOORCTL_BTN_OPEN)->EnableWindow(TRUE);
		GetDlgItem(IDC_DOORCTL_BTN_QUERY)->EnableWindow(TRUE);

		GetDlgItem(IDC_DOORCTL_BTN_Waring)->ShowWindow(TRUE);
		GetDlgItem(IDC_DOORCTL_BTN_CLOSE)->ShowWindow(TRUE);
		GetDlgItem(IDC_DOORCTL_BTN_OPEN)->ShowWindow(TRUE);
		GetDlgItem(IDC_DOORCTL_BTN_QUERY)->ShowWindow(TRUE);
	}else if (1 == m_nOperType)
	{

		GetDlgItem(IDC_DOORCTL_BTN_Waring)->EnableWindow(FALSE);
		GetDlgItem(IDC_DOORCTL_BTN_CLOSE)->EnableWindow(FALSE);
		GetDlgItem(IDC_DOORCTL_BTN_OPEN)->EnableWindow(TRUE);
		GetDlgItem(IDC_DOORCTL_BTN_QUERY)->EnableWindow(FALSE); 


		GetDlgItem(IDC_DOORCTL_BTN_Waring)->ShowWindow(FALSE);
		GetDlgItem(IDC_DOORCTL_BTN_CLOSE)->ShowWindow(FALSE);
		GetDlgItem(IDC_DOORCTL_BTN_OPEN)->ShowWindow(TRUE);
		GetDlgItem(IDC_DOORCTL_BTN_QUERY)->ShowWindow(FALSE);

		GetDlgItem(IDC_DOORCTL_BTN_OPEN)->SetWindowPos(NULL, 130,100,0,0, SWP_NOZORDER|SWP_NOSIZE);
	}else if (2 == m_nOperType)
	{

		GetDlgItem(IDC_DOORCTL_BTN_Waring)->EnableWindow(TRUE);
		GetDlgItem(IDC_DOORCTL_BTN_CLOSE)->EnableWindow(FALSE);
		GetDlgItem(IDC_DOORCTL_BTN_OPEN)->EnableWindow(FALSE);
		GetDlgItem(IDC_DOORCTL_BTN_QUERY)->EnableWindow(FALSE); 
		GetDlgItem(IDC_DOORCTL_BTN_Waring)->ShowWindow(TRUE);
		GetDlgItem(IDC_DOORCTL_BTN_CLOSE)->ShowWindow(FALSE);
		GetDlgItem(IDC_DOORCTL_BTN_OPEN)->ShowWindow(FALSE);
		GetDlgItem(IDC_DOORCTL_BTN_QUERY)->ShowWindow(FALSE);
		GetDlgItem(IDC_DOORCTL_BTN_Waring)->SetWindowPos(NULL, 130,100,0,0 , SWP_NOZORDER|SWP_NOSIZE);
	}else
	{

		GetDlgItem(IDC_DOORCTL_BTN_Waring)->EnableWindow(TRUE);
		GetDlgItem(IDC_DOORCTL_BTN_CLOSE)->EnableWindow(TRUE);
		GetDlgItem(IDC_DOORCTL_BTN_OPEN)->EnableWindow(TRUE);
		GetDlgItem(IDC_DOORCTL_BTN_QUERY)->EnableWindow(TRUE);

		GetDlgItem(IDC_DOORCTL_BTN_Waring)->ShowWindow(TRUE);
		GetDlgItem(IDC_DOORCTL_BTN_CLOSE)->ShowWindow(TRUE);
		GetDlgItem(IDC_DOORCTL_BTN_OPEN)->ShowWindow(TRUE);
		GetDlgItem(IDC_DOORCTL_BTN_QUERY)->ShowWindow(TRUE);
 
	}
	if (-1 == m_nChanelID)
	{

		m_cmbChannel.SetCurSel(0);
		m_cmbDoorStatus.SetCurSel(0); 
	}else
	{

		m_cmbChannel.SetCurSel(m_nChanelID);
		m_cmbDoorStatus.SetCurSel((int)m_emStateType);
	}
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CDlgDoorControl::OnDoorctlBtnOpen() 
{
	// TODO: Add your control notification handler code here
	NET_CTRL_ACCESS_OPEN stuParam = {sizeof(stuParam)};
	stuParam.nChannelID = m_cmbChannel.GetCurSel();
	BOOL bRet = CLIENT_ControlDevice(m_lLoginID, DH_CTRL_ACCESS_OPEN, &stuParam, 3000);
	if (bRet)
	{
		CString csInfo;
		csInfo.Format("%s %d ok", ConvertString("Open door", DLG_DOOR_CONTROL), stuParam.nChannelID + 1);
		MessageBox(csInfo, ConvertString("Prompt"));
	}
	else
	{
		CString csInfo;
		csInfo.Format("%s %d failed:0x%08x", ConvertString("Open door", DLG_DOOR_CONTROL), stuParam.nChannelID + 1, CLIENT_GetLastError());
		MessageBox(csInfo, ConvertString("Prompt"));
	}
	if (0 != m_nOperType)
		this->OnOK();
}

void CDlgDoorControl::OnDoorctlBtnClose() 
{
	// TODO: Add your control notification handler code here
	NET_CTRL_ACCESS_CLOSE stuParam = {sizeof(stuParam)};
	stuParam.nChannelID = m_cmbChannel.GetCurSel();
	BOOL bRet = CLIENT_ControlDevice(m_lLoginID, DH_CTRL_ACCESS_CLOSE, &stuParam, 3000);
	if (bRet)
	{
		CString csInfo;
		csInfo.Format("%s %d ok", ConvertString("Close door", DLG_DOOR_CONTROL), stuParam.nChannelID + 1);
		MessageBox(csInfo, ConvertString("Prompt"));
	}
	else
	{
		CString csInfo;
		csInfo.Format("%s %d failed:0x%08x", ConvertString("Close door", DLG_DOOR_CONTROL), stuParam.nChannelID + 1, CLIENT_GetLastError());
		MessageBox(csInfo, ConvertString("Prompt"));
	}	
}

void CDlgDoorControl::OnDoorctlBtnQuery() 
{
	// TODO: Add your control notification handler code here
	NET_DOOR_STATUS_INFO stuParam = {sizeof(stuParam)};
	stuParam.nChannel = m_cmbChannel.GetCurSel();
	int nRetLen = 0;
	BOOL bRet = CLIENT_QueryDevState(m_lLoginID, DH_DEVSTATE_DOOR_STATE, (char*)&stuParam, sizeof(stuParam), &nRetLen, 3000);
	if (bRet)
	{
		m_cmbDoorStatus.SetCurSel((int)stuParam.emStateType);
	} 
	else
	{
		CString csInfo;
		csInfo.Format("%s:0x%08x", ConvertString("Query door status failed", DLG_DOOR_CONTROL), CLIENT_GetLastError());
		MessageBox(csInfo, ConvertString("Prompt"));
	}
}

void CDlgDoorControl::OnBnClickedDoorctlBtnWaring()
{
	// TODO: 在此添加控件通知处理程序代码
	NET_CTRL_CLEAR_ALARM stuParam = {sizeof(stuParam)};
	//stuParam.emAlarmType    = NET_ALARM_LOCAL;
	stuParam.bEventType     = TRUE;
	stuParam.nEventType     = DH_ALARM_ACCESS_CTL_BREAK_IN;
	stuParam.nChannelID     =  m_cmbChannel.GetCurSel();;

	//     stuParam.szDevPwd       = g_szPwd;
	BOOL bRet = CLIENT_ControlDevice(m_lLoginID, DH_CTRL_CLEAR_ALARM, &stuParam);
	if (bRet)
	{
		MessageBox(ConvertString("Confirm OK!"));
	} 
	else
	{
		MessageBox(ConvertString("Confirm error!"));
	}
	if (0 != m_nOperType)
		this->OnOK();

}
