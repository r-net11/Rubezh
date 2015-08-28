// DlgAtiSet.cpp : 实现文件
//

#include "stdafx.h"
#include "AccessControl.h"
#include "DlgAtiSet.h"


// CDlgAtiSet 对话框

IMPLEMENT_DYNAMIC(CDlgAtiSet, CDialog)

CDlgAtiSet::CDlgAtiSet(CWnd* pParent /*=NULL*/, LLONG hLoginID , int nDoorCount)
	: CDialog(CDlgAtiSet::IDD, pParent)
{

	m_hLoginID = hLoginID;
	m_nDoorCount = nDoorCount;
}

CDlgAtiSet::~CDlgAtiSet()
{
}

void CDlgAtiSet::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_DLG_ATI_CHECK_ENABLE, m_checkAtiEnable);
	DDX_Control(pDX, IDC_DLG_ATI_CHECK_D1D2, m_checkR1R2); 
	DDX_Control(pDX, IDC_DLG_ATI_CHECK_D1D2D3D4, m_checkR1R2R3R4); 
	DDX_Control(pDX, IDC_DLG_ATI_CHECK_D3D4, m_checkR3R4); 
}


BEGIN_MESSAGE_MAP(CDlgAtiSet, CDialog)
	ON_BN_CLICKED(IDC_DLG_ATI_BTN_OK, &CDlgAtiSet::OnBnClickedDlgAtiBtnOk)
	ON_BN_CLICKED(IDC_DLG_ATI_BTN_CANCEL, &CDlgAtiSet::OnBnClickedDlgAtiBtnCancel)
	ON_BN_CLICKED(IDC_DLG_ATI_CHECK_ENABLE, &CDlgAtiSet::OnBnClickedDlgAtiCheckEnable)
END_MESSAGE_MAP()


// CDlgAtiSet 消息处理程序


BOOL CDlgAtiSet::OnInitDialog() 
{
	CDialog::OnInitDialog();

	// TODO: Add extra initialization here
	g_SetWndStaticText(this, DLG_CFG_AntiPassBack);

	InitDlg(); 
	// TODO: Add your control notification handler code here


	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}


void CDlgAtiSet::InitDlg()
{ 
	if(1 == m_nDoorCount)
	{
		m_checkR1R2.ShowWindow(1);
		m_checkR1R2R3R4.ShowWindow(0);
		m_checkR3R4.ShowWindow(0);

	}else  
	{ 

		m_checkR1R2.ShowWindow(1);
		m_checkR1R2R3R4.ShowWindow(1);
		m_checkR3R4.ShowWindow(1);

	} 
	m_checkAtiEnable.SetCheck(0);

	if (GetConfigFromDevice(0))
	{
		if (m_stuEventInfo.bRepeatEnterAlarm)
			m_checkAtiEnable.SetCheck(1); 
	}
	if (GetConfigFromDevice(1))
	{
		if (m_stuEventInfo.bRepeatEnterAlarm)
			m_checkAtiEnable.SetCheck(1); 
	}
	if(GetNewDevConfig())
	{

		if ((m_stuInfo.nDoorList > 0)&& (
			m_checkAtiEnable.GetCheck() == 1))
		{

			m_checkR1R2.EnableWindow(1);
			m_checkR1R2R3R4.EnableWindow(1);
			m_checkR3R4.EnableWindow(1); 
		}
		else
		{
			m_checkAtiEnable.SetCheck(0);



			m_checkR1R2.EnableWindow(0);
			m_checkR1R2R3R4.EnableWindow(0);
			m_checkR3R4.EnableWindow(0); 
		}   
		m_checkR1R2.SetCheck(0);
		m_checkR1R2R3R4.SetCheck(0);
		m_checkR3R4.SetCheck(0); 
		CString csInfo; 
		for(int i = 0; i < m_stuInfo.nDoorList; i ++)
		{
			CString csInfotp; 
			csInfotp.Format("%s%s%s%s",  m_stuInfo.stuDoorList[i].stuDoors[0].szReaderID
			, m_stuInfo.stuDoorList[i].stuDoors[1].szReaderID
			, m_stuInfo.stuDoorList[i].stuDoors[2].szReaderID
			, m_stuInfo.stuDoorList[i].stuDoors[3].szReaderID);
			csInfo = csInfo +  csInfotp;


		}
		if (csInfo == "12")
			m_checkR1R2.SetCheck(1); 
		else if (csInfo == "1324")
			m_checkR1R2R3R4.SetCheck(1);
		else if (csInfo == "34")
			m_checkR3R4.SetCheck(1);
		else if (csInfo == "2413")
			m_checkR3R4.SetCheck(1);
	}
}


void CDlgAtiSet::OnDestroy() 
{
	CDialog::OnDestroy();


}


bool CDlgAtiSet::GetNewDevConfig()
{
	int nRetLen = 0;

	char szJsonBuf[1024 * 40] = {0};	

	memset(&m_stuInfo,0,sizeof(CFG_OPEN_DOOR_ROUTE_INFO));
	memset(szJsonBuf,0,1024 * 40);

	int nerror = 0;
	BOOL bRet =  CLIENT_GetNewDevConfig((LLONG)m_hLoginID, CFG_CMD_OPEN_DOOR_ROUTE, -1, szJsonBuf, 1024*40, &nerror, 5000);
	if (bRet)
	{
		DWORD dwRetLen = 0;
		bRet = CLIENT_ParseData(CFG_CMD_OPEN_DOOR_ROUTE, szJsonBuf, (void*)&m_stuInfo, sizeof(CFG_OPEN_DOOR_ROUTE_INFO), &dwRetLen);
		if (bRet)
		{

			return true;
		}
		else
		{

			MessageBox(ConvertString(CString("Packet open door route Config failed!"), DLG_CFG_AntiPassBack), ConvertString("Prompt"));
		}			
	}
	else
	{

		CString csInfo;
		csInfo.Format("%s failed:0x%08x", ConvertString("Get open door route", DLG_CFG_AntiPassBack), 
			CLIENT_GetLastError());
		MessageBox(csInfo, ConvertString("Prompt"));
		//csErr = "Opening door is failure!";
	}  
	return false;
}


void CDlgAtiSet::OnBnClickedDlgAtiBtnOk()
{
	// TODO: 在此添加控件通知处理程序代码
	if (!m_hLoginID)
	{
		MessageBox(ConvertString(CString("we haven't login a device yet!"), DLG_VERSION), ConvertString("Prompt")); 
		return;
	} 
	memset(&m_stuInfo,0,sizeof(CFG_OPEN_DOOR_ROUTE_INFO));  
	if (m_checkAtiEnable.GetCheck())
		m_stuInfo.nDoorList = 0;
	m_stuInfo.nDoorList = 0; 
	bool Door1Enable =false;
	bool Door2Enable =false;
	if (1 == m_checkR1R2.GetCheck())
	{	 
		m_stuInfo.nDoorList = 2;
		m_stuInfo.stuDoorList[0].nDoors = 1; 
		strncpy(m_stuInfo.stuDoorList[0].stuDoors[0].szReaderID, "1", MAX_READER_ID_LEN - 1); 
		m_stuInfo.stuDoorList[1].nDoors = 1; 
		strncpy(m_stuInfo.stuDoorList[1].stuDoors[0].szReaderID, "2", MAX_READER_ID_LEN - 1);   
		Door1Enable =true;
		Door2Enable =false;
	} else
	if (1 == m_checkR1R2R3R4.GetCheck())
	{	 
		m_stuInfo.nDoorList = 2;
		m_stuInfo.stuDoorList[0].nDoors = 2; 
		strncpy(m_stuInfo.stuDoorList[0].stuDoors[0].szReaderID, "1", MAX_READER_ID_LEN - 1);
		strncpy(m_stuInfo.stuDoorList[0].stuDoors[1].szReaderID, "3", MAX_READER_ID_LEN - 1);  
		m_stuInfo.stuDoorList[1].nDoors = 2; 
		strncpy(m_stuInfo.stuDoorList[1].stuDoors[0].szReaderID, "2", MAX_READER_ID_LEN - 1); 
		strncpy(m_stuInfo.stuDoorList[1].stuDoors[1].szReaderID, "4", MAX_READER_ID_LEN - 1);  
		Door1Enable =true;
		Door2Enable =true;
	}  else 
	if (1 == m_checkR3R4.GetCheck())
	{	 
		m_stuInfo.nDoorList = 2;
		m_stuInfo.stuDoorList[0].nDoors = 1; 
		strncpy(m_stuInfo.stuDoorList[0].stuDoors[0].szReaderID, "3", MAX_READER_ID_LEN - 1); 
		m_stuInfo.stuDoorList[1].nDoors = 1; 
		strncpy(m_stuInfo.stuDoorList[1].stuDoors[0].szReaderID, "4", MAX_READER_ID_LEN - 1);  
		Door1Enable =false;
		Door2Enable =true;
	} 
 

	if (!m_checkAtiEnable.GetCheck())
	{
		if (GetConfigFromDevice(0))
		{
			m_stuEventInfo.abRepeatEnterAlarmEnable =false;
			m_stuEventInfo.bRepeatEnterAlarm = FALSE;
			SetConfigToDevice(0);

		}

		if (GetConfigFromDevice(1))
		{

			m_stuEventInfo.abRepeatEnterAlarmEnable =false;
			m_stuEventInfo.bRepeatEnterAlarm = FALSE;
			SetConfigToDevice(1);
		}
	}else
	{
		if (GetConfigFromDevice(0))
		{

			if (Door1Enable)
			{

				m_stuEventInfo.abRepeatEnterAlarmEnable =true;
				m_stuEventInfo.bRepeatEnterAlarm = TRUE;
			}else
			{

				m_stuEventInfo.abRepeatEnterAlarmEnable =false;
				m_stuEventInfo.bRepeatEnterAlarm = FALSE;
			}
			SetConfigToDevice(0);
		}

		if (GetConfigFromDevice(1)) 
		{

			if (Door2Enable)
			{

				m_stuEventInfo.abRepeatEnterAlarmEnable =true;
				m_stuEventInfo.bRepeatEnterAlarm = TRUE;
			}else
			{

				m_stuEventInfo.abRepeatEnterAlarmEnable =false;
				m_stuEventInfo.bRepeatEnterAlarm = FALSE;
			}
			SetConfigToDevice(1);
		}
	}

	char szJsonBufSet[1024 * 40] = {0};

	BOOL bRet = CLIENT_PacketData(CFG_CMD_OPEN_DOOR_ROUTE, &m_stuInfo, sizeof(m_stuInfo), szJsonBufSet, sizeof(szJsonBufSet));

	if (bRet)
	{
		int nerror = 0;
		int nrestart = 0;
		bRet = CLIENT_SetNewDevConfig(m_hLoginID, CFG_CMD_OPEN_DOOR_ROUTE, 0, szJsonBufSet, 
			sizeof(szJsonBufSet), &nerror, &nrestart, SDK_API_WAITTIME);
		bRet = CLIENT_SetNewDevConfig(m_hLoginID, CFG_CMD_OPEN_DOOR_ROUTE, 1, szJsonBufSet, 
			sizeof(szJsonBufSet), &nerror, &nrestart, SDK_API_WAITTIME);		
			  
		if (bRet)
		{

			CString csInfo;
			csInfo.Format("%s  ok", ConvertString("Set open door route", DLG_CFG_AntiPassBack));
			MessageBox(csInfo, ConvertString("Prompt"));
		}
		else
		{
			CString csInfo;
			csInfo.Format("%s failed:0x%08x", ConvertString("Set open door route", DLG_CFG_AntiPassBack), 
				CLIENT_GetLastError());
			MessageBox(csInfo, ConvertString("Prompt"));
		}			
	}
	else
	{
		//csErr = "Opening door is failure!";
		MessageBox(ConvertString(CString("Packet open door route Config failed!"), DLG_CFG_AntiPassBack), ConvertString("Prompt"));
	} 

	OnOK();
}

void CDlgAtiSet::OnBnClickedDlgAtiBtnCancel()
{
	// TODO: 在此添加控件通知处理程序代码
	OnCancel();
}

void CDlgAtiSet::OnBnClickedDlgAtiCheckEnable()
{
	// TODO: 在此添加控件通知处理程序代码

	if (1 == m_checkAtiEnable.GetCheck())
	{
		m_checkR1R2.EnableWindow(1);
		m_checkR1R2R3R4.EnableWindow(1);
		m_checkR3R4.EnableWindow(1); 

	}else
	{
 
		m_checkR1R2.EnableWindow(0);
		m_checkR1R2R3R4.EnableWindow(0);
		m_checkR3R4.EnableWindow(0); 
	}
}



BOOL CDlgAtiSet::SetConfigToDevice(int nChn)
{ 
	if (-1 == nChn)
	{
		return FALSE;
	}

	char szJsonBuf[1024 * 40] = {0};
	BOOL bRet = CLIENT_PacketData(CFG_CMD_ACCESS_EVENT, &m_stuEventInfo, sizeof(m_stuEventInfo), szJsonBuf, sizeof(szJsonBuf));
	if (!bRet)
	{
		MessageBox(ConvertString(CString("packet AccessControl error..."), DLG_CFG_ACCESS_CONTROL), ConvertString("Prompt"));
		return FALSE;
	} 
	else
	{		
		int nerror = 0;
		int nrestart = 0;

		bRet = CLIENT_SetNewDevConfig((LLONG)m_hLoginID, CFG_CMD_ACCESS_EVENT, nChn, szJsonBuf, 1024*40, &nerror, &nrestart, 5000);
		if (!bRet)
		{
			CString csErr;
			csErr.Format("%s 0x%08x...", ConvertString("SetupConfig AccessControl failed:", DLG_CFG_ACCESS_CONTROL), CLIENT_GetLastError());
			MessageBox(csErr, ConvertString("Prompt"));
			return FALSE;
		} 
	}
	return TRUE;
}

BOOL CDlgAtiSet::GetConfigFromDevice(int nChn)
{ 
	if (-1 == nChn)
	{
		return FALSE;
	}
	//m_stuEventInfo.
	char szJsonBuf[1024 * 40] = {0};
	int nerror = 0;
	BOOL bRet = CLIENT_GetNewDevConfig((LLONG)m_hLoginID, CFG_CMD_ACCESS_EVENT, nChn, szJsonBuf, 1024*40, &nerror, 5000);

	if (bRet)
	{
		DWORD dwRetLen = 0;
		bRet = CLIENT_ParseData(CFG_CMD_ACCESS_EVENT, szJsonBuf, (void*)&m_stuEventInfo, sizeof(m_stuEventInfo), &dwRetLen);
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