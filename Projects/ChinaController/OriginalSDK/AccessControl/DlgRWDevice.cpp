// DlgRWDevice.cpp : 实现文件
//

#include "stdafx.h"
#include "AccessControl.h"
#include "DlgRWDevice.h"


// CDlgRWDevice 对话框

IMPLEMENT_DYNAMIC(CDlgRWDevice, CDialog)

CDlgRWDevice::CDlgRWDevice(CWnd* pParent /*=NULL*/, LLONG hLoginID , NET_DEVICE_TYPE emDevType)
	: CDialog(CDlgRWDevice::IDD, pParent)
{

	m_hLoginID = hLoginID;
	m_emDevType = emDevType;
}

CDlgRWDevice::~CDlgRWDevice()
{
}

void CDlgRWDevice::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX); 
	DDX_Control(pDX, IDC_DLG_RW_EDT_DATA, m_deviceData);
}


BEGIN_MESSAGE_MAP(CDlgRWDevice, CDialog)
	ON_BN_CLICKED(IDC_DLG_RW_BTN_READ, &CDlgRWDevice::OnBnClickedDlgRwBtnRead)
	ON_BN_CLICKED(IDC_DLG_RW_BTN_WRITE, &CDlgRWDevice::OnBnClickedDlgRwBtnWrite)
	ON_EN_CHANGE(IDC_DLG_RW_EDT_DATA, &CDlgRWDevice::OnEnChangeDlgRwEdtData)
END_MESSAGE_MAP()


// CDlgRWDevice 消息处理程序


BOOL CDlgRWDevice::OnInitDialog() 
{
	CDialog::OnInitDialog();

	// TODO: Add extra initialization here
	g_SetWndStaticText(this, DLG_CFG_RWDATA);

	InitDlg();
	//OnQuerylogBtnTotalCount();
	// TODO: Add your control notification handler code here
	

	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}


void CDlgRWDevice::InitDlg()
{ 
	m_deviceData.Clear();
	GetDlgItem(IDC_DLG_RW_BTN_WRITE)->EnableWindow(FALSE); 
}
void CDlgRWDevice::OnBnClickedDlgRwBtnRead()
{
	// TODO: 在此添加控件通知处理程序代码
	if (!m_hLoginID)
	{
		MessageBox(ConvertString(CString("we haven't login a device yet!"), DLG_VERSION), ConvertString("Prompt"));
		return;

	}

	memset(&m_stuInfo,0,sizeof(CFG_CLIENT_CUSTOM_INFO));  
	CString csReadPos;
	GetDlgItemText(IDC_DLG_RW_EDT_STARTPOS, csReadPos);

	 
	char szJsonBuf[1024 * 40] = {0};
	int nerror = 0;
	BOOL bRet = CLIENT_GetNewDevConfig(m_hLoginID, CFG_CMD_CLIENT_CUSTOM_DATA, -1, szJsonBuf, 1024 *40, &nerror, 5000);

	if (bRet)
	{
		GetDlgItem(IDC_DLG_RW_BTN_WRITE)->EnableWindow(TRUE); 
		DWORD dwRetLen = 0;
		bRet = CLIENT_ParseData(CFG_CMD_CLIENT_CUSTOM_DATA, szJsonBuf, (void*)&m_stuInfo, sizeof(m_stuInfo), &dwRetLen);
		if (!bRet)
		{
			MessageBox(ConvertString(CString("parse AccessControl error..."), DLG_CFG_ACCESS_CONTROL), ConvertString("Prompt"));
			return  ;
		}else
		{
			if (m_stuInfo.abBinary)
			{
 
				CString csBinaryValue; 
				for (int i = 0; i < m_stuInfo.nBinaryNum; i++)
				{
					CString cstValue;
					cstValue.Format("%d ", m_stuInfo.dwBinary[i]) ;
					csBinaryValue += cstValue ;
				}
				SetDlgItemText(IDC_DLG_RW_EDT_DATA, csBinaryValue);  
				//UpdateWindow();
			}

		}
	}
	else
	{			
		CString csErr;
		csErr.Format("%s 0x%08x...\r\n\r\n%s", ConvertString("QueryConfig AccessControl error:", DLG_CFG_ACCESS_CONTROL),
			CLIENT_GetLastError(), szJsonBuf);
		MessageBox(csErr, ConvertString("Prompt"));
		return  ;
	}
	return  ;
}

void CDlgRWDevice::OnBnClickedDlgRwBtnWrite()
{
	// TODO: 在此添加控件通知处理程序代码 
	if (!m_hLoginID)
	{
		MessageBox(ConvertString(CString("we haven't login a device yet!"), DLG_VERSION), ConvertString("Prompt"));

	} 
		int nerror = 0;
		int nChannel = -1;

		CString csBinaryValue; 
		GetDlgItemText(IDC_DLG_RW_EDT_DATA, csBinaryValue); 
		int iPos = 0;
		CString csIntValue;
		int iNumber = 0;
		csIntValue = csBinaryValue.Tokenize(" ", iPos);
		while (csIntValue != "")
		{ 
			int IntValue = _tstoi(csIntValue);
			m_stuInfo.dwBinary[iNumber] = IntValue;
			iNumber ++;
			csIntValue = csBinaryValue.Tokenize(" ", iPos);
			if (iNumber > 127)
			{
				csIntValue = "";
				iNumber = 128;
				break;
			}
		}
		// 修改参数，在这里
		m_stuInfo.abBinary = true;
		m_stuInfo.nBinaryNum = iNumber; 

		char szJsonBufSet[1024 * 40] = {0};            

		BOOL bRet = CLIENT_PacketData(CFG_CMD_CLIENT_CUSTOM_DATA, &m_stuInfo, sizeof(m_stuInfo), szJsonBufSet, sizeof(szJsonBufSet));
		if (!bRet)
		{
			MessageBox(ConvertString(CString("Packet ClientCustomData Config failed!"), DLG_CFG_ACCESS_CONTROL), ConvertString("Prompt"));
		} 
		else
		{ 
			 
			int nerror = 0;
			int nrestart = 0;
			bRet = CLIENT_SetNewDevConfig(m_hLoginID, CFG_CMD_CLIENT_CUSTOM_DATA, nChannel, szJsonBufSet, sizeof(szJsonBufSet), &nerror, &nrestart, SDK_API_WAITTIME);
			if (!bRet)
			{
				//printf("Set ClientCustomData Config failed...0x%08x\n", CLIENT_GetLastError());
				MessageBox(ConvertString(CString("Set ClientCustomData Config failed\n"), DLG_CFG_ACCESS_CONTROL), ConvertString("Prompt"));
			}
			else
			{
				MessageBox(ConvertString(CString("Set ClientCustomData Config ok!\n"), DLG_CFG_ACCESS_CONTROL), ConvertString("Prompt"));
			}
		} 

}


void CDlgRWDevice::OnDestroy() 
{
	CDialog::OnDestroy();
 
 
}
void CDlgRWDevice::OnEnChangeDlgRwEdtData()
{
	// TODO:  如果该控件是 RICHEDIT 控件，则它将不会
	// 发送该通知，除非重写 CDialog::OnInitDialog()
	// 函数并调用 CRichEditCtrl().SetEventMask()，
	// 同时将 ENM_CHANGE 标志“或”运算到掩码中。

	// TODO:  在此添加控件通知处理程序代码

	//CString csBinaryValue; 
	//GetDlgItemText(IDC_DLG_RW_EDT_DATA, csBinaryValue); 

}
