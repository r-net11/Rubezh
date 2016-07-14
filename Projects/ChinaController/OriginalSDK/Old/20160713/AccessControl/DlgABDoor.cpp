// DlgOpenGroup.cpp : 实现文件
//

#include "stdafx.h"
#include "AccessControl.h"
#include "DlgABDoor.h"


// CDlgABDoor 对话框

IMPLEMENT_DYNAMIC(CDlgABDoor, CDialog)

CDlgABDoor::CDlgABDoor(CWnd* pParent /*=NULL*/, LLONG hLoginID , int nDoorCount)
	: CDialog(CDlgABDoor::IDD, pParent)
{
	m_hLoginID = hLoginID;
	m_nDoorCount = nDoorCount;

}

CDlgABDoor::~CDlgABDoor()
{
}

void CDlgABDoor::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_DLG_AB_CHECK_ENABLE, m_chkABDoorEnable);
	DDX_Control(pDX, IDC_DLG_AB_CHECK_D1D2, m_checkD1D2);
	DDX_Control(pDX, IDC_DLG_AB_CHECK_D1D2D3, m_checkD1D2D3);
	DDX_Control(pDX, IDC_DLG_AB_CHECK_D1D2D3D4, m_checkD1D2D3D4);
	DDX_Control(pDX, IDC_DLG_AB_CHECK_D2D3D4, m_checkD2D3D4);
	DDX_Control(pDX, IDC_DLG_AB_CHECK_D1D3ANDD2D4, m_checkD1D3ANDD2D4);
	DDX_Control(pDX, IDC_DLG_AB_CHECK_D1D4ANDD2D3, m_checkD1D4ANDD2D3);
	DDX_Control(pDX, IDC_DLG_AB_CHECK_D3D4, m_checkD3D4);
}


BEGIN_MESSAGE_MAP(CDlgABDoor, CDialog)
	ON_BN_CLICKED(IDOK, &CDlgABDoor::OnBnClickedOk)
	ON_BN_CLICKED(IDC_DLG_AB_BTN_OK, &CDlgABDoor::OnBnClickedDlgAbBtnOk)
	ON_BN_CLICKED(IDC_DLG_AB_BTN_CANCEL, &CDlgABDoor::OnBnClickedDlgAbBtnCancel)
	ON_BN_CLICKED(IDC_DLG_AB_CHECK_ENABLE, &CDlgABDoor::OnBnClickedDlgAbCheckEnable)
END_MESSAGE_MAP()


// CDlgABDoor 消息处理程序

BOOL CDlgABDoor::OnInitDialog() 
{
	CDialog::OnInitDialog();

	// TODO: Add extra initialization here
	g_SetWndStaticText(this, DLG_CFG_ABDOOR);

	InitDlg(); 
	// TODO: Add your control notification handler code here


	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}


void CDlgABDoor::InitDlg()
{ 
	//GetDlgItem(IDC_DLG_RW_BTN_WRITE)->EnableWindow(FALSE); 
	// only one door ,can't set ab door
	if(1 == m_nDoorCount)
	{
		m_checkD1D2.ShowWindow(0);
		m_checkD1D2D3.ShowWindow(0);
		m_checkD1D2D3D4.ShowWindow(0);
		m_checkD2D3D4.ShowWindow(0);
		m_checkD1D3ANDD2D4.ShowWindow(0);
		m_checkD1D4ANDD2D3.ShowWindow(0);
		m_checkD3D4.ShowWindow(0);

	}else if (2 == m_nDoorCount)
	{ 
		m_checkD1D2.ShowWindow(1);
		m_checkD1D2D3.ShowWindow(0);
		m_checkD1D2D3D4.ShowWindow(0);
		m_checkD2D3D4.ShowWindow(0);
		m_checkD1D3ANDD2D4.ShowWindow(0);
		m_checkD1D4ANDD2D3.ShowWindow(0);
		m_checkD3D4.ShowWindow(0);

	}else if (4 == m_nDoorCount)
	{ 
		m_checkD1D2.ShowWindow(1);
		m_checkD1D2D3.ShowWindow(1);
		m_checkD1D2D3D4.ShowWindow(1);
		m_checkD2D3D4.ShowWindow(1);
		m_checkD1D3ANDD2D4.ShowWindow(1);
		m_checkD1D4ANDD2D3.ShowWindow(1);
		m_checkD3D4.ShowWindow(1);

	}else
	{
		m_checkD1D2.ShowWindow(1);
		m_checkD1D2D3.ShowWindow(0);
		m_checkD1D2D3D4.ShowWindow(0);
		m_checkD2D3D4.ShowWindow(0);
		m_checkD1D3ANDD2D4.ShowWindow(0);
		m_checkD1D4ANDD2D3.ShowWindow(0);
		m_checkD3D4.ShowWindow(0);

	}

	if (!m_hLoginID)
	{
		MessageBox(ConvertString(CString("we haven't login a device yet!"), DLG_VERSION), ConvertString("Prompt")); 
		OnCancel();
		return;
	} 
	if(GetNewDevConfig()) 
	{

		if (m_stuInfo.stuABLockInfo.bEnable)
		{
			m_chkABDoorEnable.SetCheck(1);
			m_checkD1D2.EnableWindow(1);
			m_checkD1D2D3.EnableWindow(1);
			m_checkD1D2D3D4.EnableWindow(1);
			m_checkD2D3D4.EnableWindow(1);
			m_checkD1D3ANDD2D4.EnableWindow(1);
			m_checkD1D4ANDD2D3.EnableWindow(1);
			m_checkD3D4.EnableWindow(1);
		}
		else
		{
			m_chkABDoorEnable.SetCheck(0);

			m_checkD1D2.EnableWindow(0);
			m_checkD1D2D3.EnableWindow(0);
			m_checkD1D2D3D4.EnableWindow(0);
			m_checkD2D3D4.EnableWindow(0);
			m_checkD1D3ANDD2D4.EnableWindow(0);
			m_checkD1D4ANDD2D3.EnableWindow(0);
			m_checkD3D4.EnableWindow(0);
		} 
		m_checkD1D2.SetCheck(0);
		m_checkD1D2D3.SetCheck(0);
		m_checkD1D2D3D4.SetCheck(0);
		m_checkD2D3D4.SetCheck(0);
		m_checkD1D3ANDD2D4.SetCheck(0);
		m_checkD1D4ANDD2D3.SetCheck(0);
		m_checkD3D4.SetCheck(0);
		CString csAllABInfo;
		for(int i = 0; i < m_stuInfo.stuABLockInfo.nDoors; i ++)
		{
			CString csInfo;
			csInfo.Format("%d%d%d%d%d", m_stuInfo.stuABLockInfo.stuDoors[i].nDoor, m_stuInfo.stuABLockInfo.stuDoors[i].anDoor[0]
			, m_stuInfo.stuABLockInfo.stuDoors[i].anDoor[1]
			, m_stuInfo.stuABLockInfo.stuDoors[i].anDoor[2]
			, m_stuInfo.stuABLockInfo.stuDoors[i].anDoor[3]);
			csAllABInfo.Append(csInfo);
			
		}

		if (csAllABInfo == "20100")
			m_checkD1D2.SetCheck(1);
		else if (csAllABInfo == "30120")
			m_checkD1D2D3.SetCheck(1);
		else if (csAllABInfo == "40123")
			m_checkD1D2D3D4.SetCheck(1);
		else if (csAllABInfo == "31230")
			m_checkD2D3D4.SetCheck(1);
		else if (csAllABInfo == "2020021300")
			m_checkD1D3ANDD2D4.SetCheck(1);
		else if (csAllABInfo == "2030021200")
			m_checkD1D4ANDD2D3.SetCheck(1);
		else if (csAllABInfo == "22300")
			m_checkD3D4.SetCheck(1);
	}


}


void CDlgABDoor::OnDestroy() 
{
	CDialog::OnDestroy();


}
void CDlgABDoor::OnBnClickedOk()
{
	// TODO: 在此添加控件通知处理程序代码
	OnOK();
}

void CDlgABDoor::OnBnClickedDlgAbBtnOk()
{
	// TODO: 在此添加控件通知处理程序代码   
	if (!m_hLoginID)
	{
		MessageBox(ConvertString(CString("we haven't login a device yet!"), DLG_VERSION), ConvertString("Prompt"));
		OnCancel();
		return;
	} 
	if(!GetNewDevConfig())
		return;
	m_stuInfo.abABLockInfo = true;
	m_stuInfo.stuABLockInfo.bEnable = (1 == m_chkABDoorEnable.GetCheck()) ? TRUE : FALSE;

	m_stuInfo.stuABLockInfo.nDoors = 0;
	int iDoorIndex = 0;
	if (1 == m_checkD1D2.GetCheck())
	{	 
		m_stuInfo.stuABLockInfo.nDoors++;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].nDoor = 2;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[0] = 0;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[1] = 1;
		iDoorIndex ++ ;
	}else
	if (1 == m_checkD1D2D3.GetCheck())
	{	 
		m_stuInfo.stuABLockInfo.nDoors++;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].nDoor = 3;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[0] = 0;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[1] = 1;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[2] = 2;
		iDoorIndex ++ ;
	}else
	if (1 == m_checkD1D2D3D4.GetCheck())
	{	 
		m_stuInfo.stuABLockInfo.nDoors++;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].nDoor = 4;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[0] = 0;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[1] = 1;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[2] = 2;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[3] = 3;
		iDoorIndex ++ ;
	} else
	if (1 == m_checkD2D3D4.GetCheck())
	{	 
		m_stuInfo.stuABLockInfo.nDoors++;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].nDoor = 3;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[0] = 1;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[1] = 2;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[2] = 3;
		iDoorIndex ++ ;
	}else
	if (1 == m_checkD1D3ANDD2D4.GetCheck())
	{	 
		m_stuInfo.stuABLockInfo.nDoors++;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].nDoor = 2;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[0] = 0;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[1] = 2;
		iDoorIndex ++ ;

		m_stuInfo.stuABLockInfo.nDoors++;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].nDoor = 2;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[0] = 1;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[1] = 3;
		iDoorIndex ++ ; 

	}else
	if (1 == m_checkD1D4ANDD2D3.GetCheck())
	{	 
		m_stuInfo.stuABLockInfo.nDoors++;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].nDoor = 2;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[0] = 0;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[1] = 3;
		iDoorIndex ++ ;

		m_stuInfo.stuABLockInfo.nDoors++;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].nDoor = 2;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[0] = 1;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[1] = 2;
		iDoorIndex ++ ;
	}else
	if (1 == m_checkD3D4.GetCheck())
	{	 
		m_stuInfo.stuABLockInfo.nDoors++;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].nDoor = 2;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[0] = 2;
		m_stuInfo.stuABLockInfo.stuDoors[iDoorIndex].anDoor[1] = 3;
		iDoorIndex ++ ;
	} 
	 


	char szJsonBufSet[1024 * 40] = {0};

	BOOL bRet = CLIENT_PacketData(CFG_CMD_ACCESS_GENERAL, &m_stuInfo, sizeof(m_stuInfo), szJsonBufSet, sizeof(szJsonBufSet));

	if (bRet)
	{
		int nerror = 0;
		int nrestart = 0;
		bRet = CLIENT_SetNewDevConfig(m_hLoginID, CFG_CMD_ACCESS_GENERAL, -1, szJsonBufSet, sizeof(szJsonBufSet), &nerror, &nrestart, SDK_API_WAITTIME);
		if (bRet)
		{

			CString csInfo;
			csInfo.Format("%s  ok", ConvertString("Set AB door", DLG_CFG_ABDOOR));
			MessageBox(csInfo, ConvertString("Prompt"));
		}
		else
		{
			CString csInfo;
			csInfo.Format("%s failed:0x%08x", ConvertString("Set AB door", DLG_CFG_ABDOOR), 
				CLIENT_GetLastError());
			MessageBox(csInfo, ConvertString("Prompt"));
		}			
	}
	else
	{
		//csErr = "Opening door is failure!";
		MessageBox(ConvertString(CString("Packet AB door Config failed!"), DLG_CFG_ABDOOR), ConvertString("Prompt"));
	} 
	 
	OnOK();
}

void CDlgABDoor::OnBnClickedDlgAbBtnCancel()
{
	// TODO: 在此添加控件通知处理程序代码
	OnCancel();
}

void CDlgABDoor::OnBnClickedDlgAbCheckEnable()
{
	// TODO: 在此添加控件通知处理程序代码
	if (1 == m_chkABDoorEnable.GetCheck())
	{
		m_checkD1D2.EnableWindow(1);
		m_checkD1D2D3.EnableWindow(1);
		m_checkD1D2D3D4.EnableWindow(1);
		m_checkD2D3D4.EnableWindow(1);
		m_checkD1D3ANDD2D4.EnableWindow(1);
		m_checkD1D4ANDD2D3.EnableWindow(1);
		m_checkD3D4.EnableWindow(1);

	}else
	{

		m_checkD1D2.EnableWindow(0);
		m_checkD1D2D3.EnableWindow(0);
		m_checkD1D2D3D4.EnableWindow(0);
		m_checkD2D3D4.EnableWindow(0);
		m_checkD1D3ANDD2D4.EnableWindow(0);
		m_checkD1D4ANDD2D3.EnableWindow(0);
		m_checkD3D4.EnableWindow(0);
	}
}

bool CDlgABDoor::GetNewDevConfig()
{
	int nRetLen = 0;

	char szJsonBuf[1024 * 40] = {0};	

	memset(&m_stuInfo,0,sizeof(CFG_ACCESS_GENERAL_INFO));
	memset(szJsonBuf,0,1024 * 40);

	int nerror = 0;
	BOOL bRet =  CLIENT_GetNewDevConfig((LLONG)m_hLoginID, CFG_CMD_ACCESS_GENERAL, -1, szJsonBuf, 1024*40, &nerror, 5000);
	if (bRet)
	{
		DWORD dwRetLen = 0;
		bRet = CLIENT_ParseData(CFG_CMD_ACCESS_GENERAL, szJsonBuf, (void*)&m_stuInfo, sizeof(CFG_ACCESS_GENERAL_INFO), &dwRetLen);
		if (bRet)
		{

			return true;
		}
		else
		{

			MessageBox(ConvertString(CString("Packet AB door Config failed!"), DLG_CFG_ABDOOR), ConvertString("Prompt"));
		}			
	}
	else
	{

		CString csInfo;
		csInfo.Format("%s failed:0x%08x", ConvertString("Get AB door", DLG_CFG_ABDOOR), 
			CLIENT_GetLastError());
		MessageBox(csInfo, ConvertString("Prompt"));
		//csErr = "Opening door is failure!";
	}  
	return false;
}
