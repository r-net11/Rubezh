// DlgCfgAccessControl.cpp : implementation file
//

#include "stdafx.h"
#include "accesscontrol.h"
#include "DlgCfgAccessControl.h"
#include "SubDlgCfgDoorOpenTimeSection.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDlgCfgAccessControl dialog


CDlgCfgAccessControl::CDlgCfgAccessControl(CWnd* pParent /* = NULL */, LLONG lLoginID /* = 0 */)
	: CDialog(CDlgCfgAccessControl::IDD, pParent)
{
	//{{AFX_DATA_INIT(CDlgCfgAccessControl)
	//}}AFX_DATA_INIT
	m_lLoginID = lLoginID;
	memset(&m_stuInfo, 0, sizeof(m_stuInfo));
}


void CDlgCfgAccessControl::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDlgCfgAccessControl)
	DDX_Control(pDX, IDC_ACCESSCONTROL_CMB_CHANNEL2, m_cmbChannel);
	DDX_Control(pDX, IDC_ACCESSCONTROL_CMB_TIMEOUTDOORSTATUS, m_TimeOutDoorStatus);
	DDX_Control(pDX, IDC_ACCESSCONTROL_CHK_FIRSTENTER_REMOTECHECK, m_chkRemoteCheck);
	DDX_Control(pDX, IDC_ACCESSCONTROL_CHK_FIRSTENTER_ENABLE, m_chkFirstEnterEnable);
	DDX_Control(pDX, IDC_ACCESSCONTROL_CMB_FIRSTENTER_STATUS, m_cbFirstEnterStatus);
	DDX_Control(pDX, IDC_ACCESSCONTROL_CMB_OPENTIMEINDEX, m_cmbOpenTimeIndex);
	DDX_Control(pDX, IDC_ACCESSCONTROL_CMB_OPENMETHOD, m_cmbOpenMethod);
	DDX_Control(pDX, IDC_ACCESSCONTROL_CHK_SENSOR, m_chkSensor);
	DDX_Control(pDX, IDC_ACCESSCONTROL_CHK_REPEATENTERALARM, m_chkRepeatEnterAlarm);
	DDX_Control(pDX, IDC_ACCESSCONTROL_CHK_DURESSALARM, m_chkDuressAlarm);
	DDX_Control(pDX, IDC_ACCESSCONTROL_CHK_DOORNOTCLOSEALARM, m_chkDoorNotCloseAlarm);
	DDX_Control(pDX, IDC_ACCESSCONTROL_CHK_BREAKINALARM, m_chkBreakAlarm);
	//}}AFX_DATA_MAP
	DDX_Control(pDX, IDC_ACCESSCONTROL_CHK_FIRSTENTER_HANDICAPPEDCHECK, m_chkHandicappedCheck);
	DDX_Control(pDX, IDC_ACCESSCONTROL_CHK_REPEATENTERALARM2, m_chkCloseCheckSensor);
}


BEGIN_MESSAGE_MAP(CDlgCfgAccessControl, CDialog)
	//{{AFX_MSG_MAP(CDlgCfgAccessControl)
	ON_BN_CLICKED(IDC_ACCESSCONTROL_BTN_TIMESECTION, OnAccessControlBtnTimeSection)
	ON_BN_CLICKED(IDC_ACCESSCONTROL_BTN_GET, OnAccessControlBtnGet)
	ON_BN_CLICKED(IDC_ACCESSCONTROL_BTN_SET, OnAccessControlBtnSet)
	ON_CBN_SELENDCANCEL(IDC_ACCESSCONTROL_CMB_CHANNEL2, OnSelendcancelAccesscontrolCmbChannel)
	//}}AFX_MSG_MAP
	ON_BN_CLICKED(IDC_ACCESSCONTROL_CHK_FIRSTENTER_REMOTECHECK, OnBnClickedAccesscontrolChkFirstenterRemotecheck)
	ON_BN_CLICKED(IDC_ACCESSCONTROL_CHK_FIRSTENTER_HANDICAPPEDCHECK, &CDlgCfgAccessControl::OnBnClickedAccesscontrolChkFirstenterHandicappedcheck)
	ON_EN_CHANGE(IDC_ACCESSCONTROL_EDT_HANDICAPPEDUNLOCKHOLD, &CDlgCfgAccessControl::OnEnChangeAccesscontrolEdtHandicappedunlockhold)
	ON_EN_CHANGE(IDC_ACCESSCONTROL_EDT_HANDICAPPEDTIMEOUT2, &CDlgCfgAccessControl::OnEnChangeAccesscontrolEdtHandicappedtimeout2)
	ON_EN_UPDATE(IDC_ACCESSCONTROL_EDT_HANDICAPPEDUNLOCKHOLD, &CDlgCfgAccessControl::OnEnUpdateAccesscontrolEdtHandicappedunlockhold)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDlgCfgAccessControl private method

void CDlgCfgAccessControl::InitDlg()
{
	if (!m_lLoginID)
	{
		MessageBox(ConvertString(CString("we haven't login a device yet!"), DLG_VERSION), ConvertString("Prompt")); 
		OnCancel();
		return;
	} 
	int i = 0;

	// channel
	m_cmbChannel.ResetContent();
	for (i = 0; i < 4; i++)
	{
		CString csInfo;
		csInfo.Format("%s%d", ConvertString("Channel", DLG_CFG_ACCESS_CONTROL), i + 1);
		m_cmbChannel.InsertString(-1, csInfo);
	}
	m_cmbChannel.SetCurSel(0);

	// door open method
	m_cmbOpenMethod.ResetContent();
	for (i = 0; i < sizeof(stuDemoOpenMethod)/sizeof(stuDemoOpenMethod[0]); i++)
	{
		m_cmbOpenMethod.InsertString(-1, ConvertString(stuDemoOpenMethod[i].szName));
	}
	m_cmbOpenMethod.SetCurSel(-1);
	m_stuInfo.abDoorOpenMethod = true;

	// door unlock hold time
	m_stuInfo.abUnlockHoldInterval = true;

	// door close timeout
	m_stuInfo.abCloseTimeout = true;

	// open time index in config of AccessTimeSchedule, start from 0
	m_cmbOpenTimeIndex.ResetContent();
	for (i = 0; i < 128; i++)
	{
		CString csInfo;
		csInfo.Format("%02d", i);
		m_cmbOpenTimeIndex.InsertString(-1, csInfo);
	}

	// first enter status
	m_cbFirstEnterStatus.ResetContent();
	static const char* szFirstEnterStat[] = 
	{	"Unknown", 
		"KeepOpen", 
		"Normal"
	};
	for (i=0; i<3; i++)
	{
		m_cbFirstEnterStatus.InsertString(-1, ConvertString(szFirstEnterStat[i], DLG_CFG_ACCESS_CONTROL));
	}
	m_cbFirstEnterStatus.SetCurSel((int)m_stuInfo.stuFirstEnterInfo.emStatus);

	SetDlgItemInt(IDC_ACCESSCONTROL_EDT_FIRSTENTER_TIMERINDEX, 0);

	m_cmbOpenTimeIndex.SetCurSel(-1);
	m_stuInfo.abOpenAlwaysTimeIndex = true;

	// holiday record set recNo
	m_stuInfo.abHolidayTimeIndex = true;

	// break in alarm enable
	//m_stuInfo.abBreakInAlarmEnable = true;
	m_chkBreakAlarm.SetCheck(BST_UNCHECKED);

	// repeat enter alarm enable
	//m_stuInfo.abRepeatEnterAlarmEnable = true;
	m_chkRepeatEnterAlarm.SetCheck(BST_UNCHECKED);

	// door not close enable
	//m_stuInfo.abDoorNotClosedAlarmEnable = true;
	m_chkDoorNotCloseAlarm.SetCheck(BST_UNCHECKED);

	// duress alarm enable
	//m_stuInfo.abDuressAlarmEnable = true;
	m_chkDuressAlarm.SetCheck(BST_UNCHECKED);

	// sensor alarm enable
	//m_stuInfo.abSensorEnable = true;
	m_chkSensor.SetCheck(BST_UNCHECKED);

	// first enter
	//m_stuInfo.abFirstEnterEnable = true;
	m_chkFirstEnterEnable.SetCheck(BST_UNCHECKED);

    // remote check
    //m_stuInfo.abRemoteCheck = true;
    m_chkRemoteCheck.SetCheck(BST_UNCHECKED);

	// first enter status
	m_TimeOutDoorStatus.ResetContent();
	m_TimeOutDoorStatus.Clear();
	static const char* szTimeOutDoorStatus[] = 
	{	 
	"True", 
	"False"
	};
	for (i=0; i<2; i++)
	{
		m_TimeOutDoorStatus.InsertString(-1, ConvertString(szTimeOutDoorStatus[i], DLG_CFG_ACCESS_CONTROL));
	}
}

BOOL CDlgCfgAccessControl::SetConfigToDevice()
{
	int nChn = m_cmbChannel.GetCurSel();
	if (-1 == nChn)
	{
		return FALSE;
	}

	char szJsonBuf[1024 * 40] = {0};
	BOOL bRet = CLIENT_PacketData(CFG_CMD_ACCESS_EVENT, &m_stuInfo, sizeof(m_stuInfo), szJsonBuf, sizeof(szJsonBuf));
	if (!bRet)
	{
		MessageBox(ConvertString(CString("packet AccessControl error..."), DLG_CFG_ACCESS_CONTROL), ConvertString("Prompt"));
		return FALSE;
	} 
	else
	{		
		int nerror = 0;
		int nrestart = 0;
		
		bRet = CLIENT_SetNewDevConfig((LLONG)m_lLoginID, CFG_CMD_ACCESS_EVENT, nChn, szJsonBuf, 1024*40, &nerror, &nrestart, 5000);
		if (!bRet)
		{
			CString csErr;
			csErr.Format("%s 0x%08x...", ConvertString("SetupConfig AccessControl failed:", DLG_CFG_ACCESS_CONTROL), CLIENT_GetLastError());
			MessageBox(csErr, ConvertString("Prompt"));
			return FALSE;
		}
		else
		{
			MessageBox(ConvertString(CString("SetConfig AccessControl ok!"), DLG_CFG_ACCESS_CONTROL), ConvertString("Prompt"));
		}
	}
	return TRUE;
}

BOOL CDlgCfgAccessControl::GetConfigFromDevice()
{
	int nChn = m_cmbChannel.GetCurSel();
	if (-1 == nChn)
	{
		return FALSE;
	}

	memset(&m_stuInfo, 0, sizeof(m_stuInfo));
	char szJsonBuf[1024 * 40] = {0};
	int nerror = 0;
	BOOL bRet = CLIENT_GetNewDevConfig((LLONG)m_lLoginID, CFG_CMD_ACCESS_EVENT, nChn, szJsonBuf, 1024*40, &nerror, 5000);
	
	if (bRet)
	{
		DWORD dwRetLen = 0;
		bRet = CLIENT_ParseData(CFG_CMD_ACCESS_EVENT, szJsonBuf, (void*)&m_stuInfo, sizeof(m_stuInfo), &dwRetLen);
		if (!bRet)
		{
			MessageBox(ConvertString(CString("parse AccessControl error..."), DLG_CFG_ACCESS_CONTROL), ConvertString("Prompt"));
			return FALSE;
		}
	}
	else
	{			
		CString csErr;
		csErr.Format("%s 0x%08x...\r\n\r\n%s", ConvertString("QueryConfig AccessControl error:", DLG_CFG_ACCESS_CONTROL),
			CLIENT_GetLastError(), szJsonBuf);
		MessageBox(csErr, ConvertString("Prompt"));
		return FALSE;
	}
	return TRUE;
}

void CDlgCfgAccessControl::DlgToStu()
{
	// door open method
	m_stuInfo.emDoorOpenMethod = (CFG_DOOR_OPEN_METHOD)m_cmbOpenMethod.GetCurSel();
	
	// door unlock hold time
	m_stuInfo.nUnlockHoldInterval = GetDlgItemInt(IDC_ACCESSCONTROL_EDT_UNLOCKHOLD, NULL, TRUE);
	
	// door close timeout
	m_stuInfo.nCloseTimeout = GetDlgItemInt(IDC_ACCESSCONTROL_EDT_CLOSETIMEOUT, NULL, TRUE);
	
	// open time index in config of AccessTimeSchedule, start from 0
	m_stuInfo.nOpenAlwaysTimeIndex = m_cmbOpenTimeIndex.GetCurSel();
	
	// holiday record set recNo
	m_stuInfo.nHolidayTimeRecoNo = GetDlgItemInt(IDC_ACCESSCONTROL_EDT_HOLIDAYTIMERECNO, NULL, TRUE);

	// first enter status
	m_stuInfo.stuFirstEnterInfo.emStatus = static_cast<CFG_ACCESS_FIRSTENTER_STATUS>(m_cbFirstEnterStatus.GetCurSel());

	// first enter time index
	m_stuInfo.stuFirstEnterInfo.nTimeIndex = GetDlgItemInt(IDC_ACCESSCONTROL_EDT_FIRSTENTER_TIMERINDEX);
	
	// break in alarm enable
	if (m_chkBreakAlarm.GetCheck())
	{
        m_stuInfo.abBreakInAlarmEnable = true;
		m_stuInfo.bBreakInAlarmEnable = TRUE;
	}
	else
	{
		m_stuInfo.bBreakInAlarmEnable = FALSE;
	}
	
	// repeat enter alarm enable
	if (m_chkRepeatEnterAlarm.GetCheck())
	{
        m_stuInfo.abRepeatEnterAlarmEnable = true;
		m_stuInfo.bRepeatEnterAlarm = TRUE;
	}
	else
	{
		m_stuInfo.bRepeatEnterAlarm = FALSE;
	}
	
	// door not close enable
	if (m_chkDoorNotCloseAlarm.GetCheck())
	{
        m_stuInfo.abDoorNotClosedAlarmEnable = true;
		m_stuInfo.bDoorNotClosedAlarmEnable = TRUE;
	}
	else
	{
		m_stuInfo.bDoorNotClosedAlarmEnable = FALSE;
	}
	
	// duress alarm enable
	if (m_chkDuressAlarm.GetCheck())
	{
        m_stuInfo.abDuressAlarmEnable = true;
		m_stuInfo.bDuressAlarmEnable = TRUE;
	}
	else
	{
		m_stuInfo.bDuressAlarmEnable = FALSE;
	}
	
	// sensor alarm enable
	if (m_chkSensor.GetCheck())
	{
        m_stuInfo.abSensorEnable = true;
		m_stuInfo.bSensorEnable = TRUE;
	}
	else
	{
		m_stuInfo.bSensorEnable = FALSE;
	}

	// time section...

	// first enter enable
	if (m_chkFirstEnterEnable.GetCheck())
	{
        m_stuInfo.abFirstEnterEnable = true;
		m_stuInfo.stuFirstEnterInfo.bEnable = TRUE;
	}
	else
	{
		m_stuInfo.stuFirstEnterInfo.bEnable = FALSE;
	}

    // remote check
    if (m_chkRemoteCheck.GetCheck())
    {
        m_stuInfo.abRemoteCheck = true;
        m_stuInfo.bRemoteCheck = TRUE;

		m_stuInfo.stuRemoteDetail.nTimeOut = GetDlgItemInt(IDC_ACCESSCONTROL_EDT_FIRSTENTER_REMOTETIMEOUT);
		int nSel = m_TimeOutDoorStatus.GetCurSel() ;
		if (nSel  == 0)
			m_stuInfo.stuRemoteDetail.bTimeOutDoorStatus = TRUE;
		else
			m_stuInfo.stuRemoteDetail.bTimeOutDoorStatus = FALSE;

    }
    else
    {
        m_stuInfo.bRemoteCheck = FALSE;
	}

	// handicapped
	int Handicappedunlockhold  = GetDlgItemInt(IDC_ACCESSCONTROL_EDT_HANDICAPPEDUNLOCKHOLD);
	//[250, 60000] 
	if (Handicappedunlockhold > 60000)
		SetDlgItemInt(IDC_ACCESSCONTROL_EDT_HANDICAPPEDUNLOCKHOLD, 60000);  
	if (Handicappedunlockhold  < 250)
		SetDlgItemInt(IDC_ACCESSCONTROL_EDT_HANDICAPPEDUNLOCKHOLD, 250);  

	int Handicappedtimeout2  = GetDlgItemInt(IDC_ACCESSCONTROL_EDT_HANDICAPPEDTIMEOUT2);
	//[0,9999] 
	if (Handicappedtimeout2 > 9999)
		SetDlgItemInt(IDC_ACCESSCONTROL_EDT_HANDICAPPEDTIMEOUT2, 9999);  
	if (Handicappedtimeout2  < 0)
		SetDlgItemInt(IDC_ACCESSCONTROL_EDT_HANDICAPPEDTIMEOUT2, 0); 
	if (m_chkHandicappedCheck.GetCheck())
	{
		m_stuInfo.abHandicapTimeOut = true;
		m_stuInfo.stuHandicapTimeOut.nUnlockHoldInterval = GetDlgItemInt(IDC_ACCESSCONTROL_EDT_HANDICAPPEDUNLOCKHOLD);
		m_stuInfo.stuHandicapTimeOut.nCloseTimeout = GetDlgItemInt(IDC_ACCESSCONTROL_EDT_HANDICAPPEDTIMEOUT2);;
	}
	else
	{
		m_stuInfo.abHandicapTimeOut = false;
		m_stuInfo.stuHandicapTimeOut.nUnlockHoldInterval  =10000;
		m_stuInfo.stuHandicapTimeOut.nCloseTimeout  = 0;;
	}
	// bCloseCheckSensor
	if (m_chkCloseCheckSensor.GetCheck())
	{
		m_stuInfo.bCloseCheckSensor = TRUE; 
	}
	else
	{
		m_stuInfo.bCloseCheckSensor = FALSE; 
	}
	
}

void CDlgCfgAccessControl::StuToDlg()
{
	// door open method
	m_cmbOpenMethod.SetCurSel((CFG_DOOR_OPEN_METHOD)m_stuInfo.emDoorOpenMethod);
	
	// door unlock hold time
	SetDlgItemInt(IDC_ACCESSCONTROL_EDT_UNLOCKHOLD, m_stuInfo.nUnlockHoldInterval);
	
	// door close timeout
	SetDlgItemInt(IDC_ACCESSCONTROL_EDT_CLOSETIMEOUT, m_stuInfo.nCloseTimeout);
	
	// open time index in config of AccessTimeSchedule, start from 0
	m_cmbOpenTimeIndex.SetCurSel(m_stuInfo.nOpenAlwaysTimeIndex);
	
	// holiday record set recNo
	SetDlgItemInt(IDC_ACCESSCONTROL_EDT_HOLIDAYTIMERECNO, m_stuInfo.nHolidayTimeRecoNo);

	// first enter time index
	SetDlgItemInt(IDC_ACCESSCONTROL_EDT_FIRSTENTER_TIMERINDEX, m_stuInfo.stuFirstEnterInfo.nTimeIndex);

	// first enter status
	m_cbFirstEnterStatus.SetCurSel((int)m_stuInfo.stuFirstEnterInfo.emStatus);

	
	// break in alarm enable
    if (m_stuInfo.abBreakInAlarmEnable)
    {
        if (m_stuInfo.bBreakInAlarmEnable)
        {
            m_chkBreakAlarm.SetCheck(BST_CHECKED);
        }
        else
        {
            m_chkBreakAlarm.SetCheck(BST_UNCHECKED);
        }
    }
    else
    {
        m_chkBreakAlarm.SetCheck(BST_UNCHECKED);
    }
	
	// repeat enter alarm enable
    if (m_stuInfo.abRepeatEnterAlarmEnable)
    {
        if (m_stuInfo.bRepeatEnterAlarm)
        {
            m_chkRepeatEnterAlarm.SetCheck(BST_CHECKED);
        }
        else
        {
            m_chkRepeatEnterAlarm.SetCheck(BST_UNCHECKED);
        }
    }
    else
    {
        m_chkRepeatEnterAlarm.SetCheck(BST_UNCHECKED);
    }
	
	// door not close enable
    if (m_stuInfo.abDoorNotClosedAlarmEnable)
    {
        if (m_stuInfo.bDoorNotClosedAlarmEnable)
        {
            m_chkDoorNotCloseAlarm.SetCheck(BST_CHECKED);
        }
        else
        {
            m_chkDoorNotCloseAlarm.SetCheck(BST_UNCHECKED);
        }
    } 
    else
    {
        m_chkDoorNotCloseAlarm.SetCheck(BST_UNCHECKED);
    }
	
	// duress alarm enable
    if (m_stuInfo.abDuressAlarmEnable)
    {
        if (m_stuInfo.bDuressAlarmEnable)
        {
            m_chkDuressAlarm.SetCheck(BST_CHECKED);
        }
        else
        {
            m_chkDuressAlarm.SetCheck(BST_UNCHECKED);
        }
    } 
    else
    {
        m_chkDuressAlarm.SetCheck(BST_UNCHECKED);
    }
	
	// sensor alarm enable
    if (m_stuInfo.abSensorEnable)
    {
        if (m_stuInfo.bSensorEnable)
        {
            m_chkSensor.SetCheck(BST_CHECKED);
        }
        else
        {
            m_chkSensor.SetCheck(BST_UNCHECKED);
        }
    }
    else
    {
        m_chkSensor.SetCheck(BST_UNCHECKED);
    }
	
	// time section...

	//first enter enable
    if (m_stuInfo.abFirstEnterEnable)
    {
        if (m_stuInfo.stuFirstEnterInfo.bEnable)
        {
            m_chkFirstEnterEnable.SetCheck(BST_CHECKED);
        }
        else
        {
            m_chkFirstEnterEnable.SetCheck(BST_UNCHECKED);
        }
    }
    else
    {
        m_chkFirstEnterEnable.SetCheck(BST_UNCHECKED);
    }

	// remote check
	GetDlgItem(IDC_ACCESSCONTROL_EDT_FIRSTENTER_REMOTETIMEOUT)->EnableWindow(FALSE); 
	m_TimeOutDoorStatus.EnableWindow(FALSE); 
    if (m_stuInfo.abRemoteCheck)
    {
        if (m_stuInfo.bRemoteCheck)
        {
            m_chkRemoteCheck.SetCheck(BST_CHECKED);

			GetDlgItem(IDC_ACCESSCONTROL_EDT_FIRSTENTER_REMOTETIMEOUT)->EnableWindow(TRUE); 
			m_TimeOutDoorStatus.EnableWindow(TRUE); 
        }
        else
        {
            m_chkRemoteCheck.SetCheck(BST_UNCHECKED);
        }
		SetDlgItemInt(IDC_ACCESSCONTROL_EDT_FIRSTENTER_REMOTETIMEOUT, m_stuInfo.stuRemoteDetail.nTimeOut); 
		if (m_stuInfo.stuRemoteDetail.bTimeOutDoorStatus)
			m_TimeOutDoorStatus.SetCurSel(0) ;
		else
			m_TimeOutDoorStatus.SetCurSel(1) ;
    }
    else
    {
        m_chkRemoteCheck.SetCheck(BST_UNCHECKED);
    }

	// handicapped 
	GetDlgItem(IDC_ACCESSCONTROL_EDT_HANDICAPPEDUNLOCKHOLD)->EnableWindow(FALSE); 
	GetDlgItem(IDC_ACCESSCONTROL_EDT_HANDICAPPEDTIMEOUT2)->EnableWindow(FALSE);  
	if (m_stuInfo.abHandicapTimeOut && ( m_stuInfo.stuHandicapTimeOut.nUnlockHoldInterval > 249))
	{
		//if (m_stuInfo.abHandicapTimeOut)
		//{
		m_chkHandicappedCheck.SetCheck(BST_CHECKED);

		GetDlgItem(IDC_ACCESSCONTROL_EDT_HANDICAPPEDUNLOCKHOLD)->EnableWindow(TRUE); 
		GetDlgItem(IDC_ACCESSCONTROL_EDT_HANDICAPPEDTIMEOUT2)->EnableWindow(TRUE);  
		/*}
		else
		{
			m_chkHandicappedCheck.SetCheck(BST_UNCHECKED);
		}*/
		SetDlgItemInt(IDC_ACCESSCONTROL_EDT_HANDICAPPEDUNLOCKHOLD, m_stuInfo.stuHandicapTimeOut.nUnlockHoldInterval);  
		SetDlgItemInt(IDC_ACCESSCONTROL_EDT_HANDICAPPEDTIMEOUT2, m_stuInfo.stuHandicapTimeOut.nCloseTimeout);  
	}
	else
	{
		m_chkHandicappedCheck.SetCheck(BST_UNCHECKED);
		SetDlgItemInt(IDC_ACCESSCONTROL_EDT_HANDICAPPEDUNLOCKHOLD, 10000);  
		SetDlgItemInt(IDC_ACCESSCONTROL_EDT_HANDICAPPEDTIMEOUT2, 0); 
	}


	// bCloseCheckSensor
	if (m_stuInfo.bCloseCheckSensor )
	{
		m_chkCloseCheckSensor.SetCheck(BST_CHECKED);
	}
	else
	{
		m_chkCloseCheckSensor.SetCheck(BST_UNCHECKED);
	}
}

/////////////////////////////////////////////////////////////////////////////
// CDlgCfgAccessControl message handlers

BOOL CDlgCfgAccessControl::OnInitDialog() 
{
	CDialog::OnInitDialog();
	g_SetWndStaticText(this, DLG_CFG_ACCESS_CONTROL);
	// TODO: Add extra initialization here
	InitDlg();
	if (GetConfigFromDevice())
	{
		StuToDlg();
	}
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CDlgCfgAccessControl::OnAccessControlBtnTimeSection() 
{
	// TODO: Add your control notification handler code here
	CSubDlgCfgDoorOpenTimeSection dlg(this);
	dlg.SetTimeSection(&m_stuInfo.stuDoorTimeSection[0][0]);
	if (IDOK == dlg.DoModal())
	{
		for (int i = 0; i < WEEK_DAY_NUM; i++)
		{
			for (int j = 0; j < MAX_DOOR_TIME_SECTION; j++)
			{
				const CFG_DOOROPEN_TIMESECTION_INFO* pstuTimeSection = dlg.GetTimeSection(i, j);
				if (pstuTimeSection)
				{
					memcpy(&m_stuInfo.stuDoorTimeSection[i][j], pstuTimeSection, sizeof(CFG_DOOROPEN_TIMESECTION_INFO));
				}
			}
		}
	}
}

void CDlgCfgAccessControl::OnAccessControlBtnGet() 
{
	// TODO: Add your control notification handler code here
	if (GetConfigFromDevice())
	{
		StuToDlg();
	}
}

void CDlgCfgAccessControl::OnAccessControlBtnSet() 
{
	// TODO: Add your control notification handler code here
	DlgToStu();
	SetConfigToDevice();
}

void CDlgCfgAccessControl::OnSelendcancelAccesscontrolCmbChannel() 
{
	// TODO: Add your control notification handler code here
	OnAccessControlBtnGet();
	
}

void CDlgCfgAccessControl::OnBnClickedAccesscontrolChkFirstenterRemotecheck()
{
	// TODO: 在此添加控件通知处理程序代码

	if (1 == m_chkRemoteCheck.GetCheck())
	{
		m_TimeOutDoorStatus.EnableWindow(1);

		GetDlgItem(IDC_ACCESSCONTROL_EDT_FIRSTENTER_REMOTETIMEOUT)->EnableWindow(TRUE); 

	}else
	{


		m_TimeOutDoorStatus.EnableWindow(0);

		GetDlgItem(IDC_ACCESSCONTROL_EDT_FIRSTENTER_REMOTETIMEOUT)->EnableWindow(FALSE); 
	}
}

void CDlgCfgAccessControl::OnBnClickedAccesscontrolChkFirstenterHandicappedcheck()
{
	// TODO: 在此添加控件通知处理程序代码


	if (1 == m_chkHandicappedCheck.GetCheck())
	{ 

		GetDlgItem(IDC_ACCESSCONTROL_EDT_HANDICAPPEDUNLOCKHOLD)->EnableWindow(TRUE); 
		GetDlgItem(IDC_ACCESSCONTROL_EDT_HANDICAPPEDTIMEOUT2)->EnableWindow(TRUE);

	}else
	{
 
 
		GetDlgItem(IDC_ACCESSCONTROL_EDT_HANDICAPPEDUNLOCKHOLD)->EnableWindow(FALSE); 
		GetDlgItem(IDC_ACCESSCONTROL_EDT_HANDICAPPEDTIMEOUT2)->EnableWindow(FALSE);
	}
}

void CDlgCfgAccessControl::OnEnChangeAccesscontrolEdtHandicappedunlockhold()
{
	// TODO:  如果该控件是 RICHEDIT 控件，则它将不会
	// 发送该通知，除非重写 CDialog::OnInitDialog()
	// 函数并调用 CRichEditCtrl().SetEventMask()，
	// 同时将 ENM_CHANGE 标志“或”运算到掩码中。

	// TODO:  在此添加控件通知处理程序代码

	

	int Handicappedunlockhold  = GetDlgItemInt(IDC_ACCESSCONTROL_EDT_HANDICAPPEDUNLOCKHOLD);
	//[250, 60000] 
	if (Handicappedunlockhold > 60000)
		SetDlgItemInt(IDC_ACCESSCONTROL_EDT_HANDICAPPEDUNLOCKHOLD, 60000);  
}

void CDlgCfgAccessControl::OnEnChangeAccesscontrolEdtHandicappedtimeout2()
{
	// TODO:  如果该控件是 RICHEDIT 控件，则它将不会
	// 发送该通知，除非重写 CDialog::OnInitDialog()
	// 函数并调用 CRichEditCtrl().SetEventMask()，
	// 同时将 ENM_CHANGE 标志“或”运算到掩码中。

	// TODO:  在此添加控件通知处理程序代码
	int Handicappedtimeout2  = GetDlgItemInt(IDC_ACCESSCONTROL_EDT_HANDICAPPEDTIMEOUT2);
	//[0,9999] 
	if (Handicappedtimeout2 > 9999)
		SetDlgItemInt(IDC_ACCESSCONTROL_EDT_HANDICAPPEDTIMEOUT2, 9999);  
}

void CDlgCfgAccessControl::OnEnUpdateAccesscontrolEdtHandicappedunlockhold()
{
	// TODO:  如果该控件是 RICHEDIT 控件，则它将不会
	// 发送该通知，除非重写 CDialog::OnInitDialog()
	// 函数，将 EM_SETEVENTMASK 消息发送到控件，
	// 同时将 ENM_UPDATE 标志“或”运算到 lParam 掩码中。

	// TODO:  在此添加控件通知处理程序代码
}
