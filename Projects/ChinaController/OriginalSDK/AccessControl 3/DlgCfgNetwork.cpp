// DlgCfgNetwork.cpp : implementation file
//

#include "stdafx.h"
#include "AccessControl.h"
#include "DlgCfgNetwork.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDlgCfgNetwork dialog


CDlgCfgNetwork::CDlgCfgNetwork(CWnd* pParent /*=NULL*/, LLONG lLoginID)
	: CDialog(CDlgCfgNetwork::IDD, pParent)
{
	//{{AFX_DATA_INIT(CDlgCfgNetwork)
		// NOTE: the ClassWizard will add member initialization here
	m_lLoginID = lLoginID;
	//}}AFX_DATA_INIT
	memset(&m_stuNetwork, 0, sizeof(m_stuNetwork));
}


void CDlgCfgNetwork::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDlgCfgNetwork)
	DDX_Control(pDX, IDC_NETWORK_MASK, m_ctlMask);
	DDX_Control(pDX, IDC_NETWORK_IP, m_ctlIp);
	DDX_Control(pDX, IDC_NETWORK_GATEWAY, m_ctlGateway);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CDlgCfgNetwork, CDialog)
	//{{AFX_MSG_MAP(CDlgCfgNetwork)
	ON_BN_CLICKED(IDC_NETWORK_BTN_CANCEL, OnBtnCancel)
	ON_BN_CLICKED(IDC_NETWORK_BTN_SET, OnBtnSet)
	ON_MESSAGE(WM_ALARM_INFO, OnNetWorkInfo)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDlgCfgNetwork message handlers
// 事件的信息，作为投递消息的载体，用户管理内存
typedef struct tagNetWorkInfo
{
	long	lCommand;
	char*	pBuf;
	DWORD	dwBufLen;

	tagNetWorkInfo()
	{
		lCommand = 0;
		pBuf = NULL;
		dwBufLen = 0;
	}

	~tagNetWorkInfo()
	{
		if (pBuf)
		{
			delete []pBuf;
		}
	}
}NetWorkInfo;

BOOL CALLBACK NetWorkMessCallBack(LONG lCommand, LLONG lLoginID, char *pBuf,
						   DWORD dwBufLen, char *pchDVRIP, LONG nDVRPort, LDWORD dwUser)
{
	if(!dwUser) 
	{
		return FALSE;
	}
	
	CDlgCfgNetwork* dlg = (CDlgCfgNetwork*)dwUser;
	BOOL bRet = FALSE;
	if (dlg != NULL && dlg->GetSafeHwnd())
	{
		NetWorkInfo* pInfo = new NetWorkInfo;
		if (!pInfo)
		{
			return FALSE;
		}
		pInfo->lCommand = lCommand;
		pInfo->pBuf = new char[dwBufLen];
		if (!pInfo->pBuf)
		{
			delete pInfo;
			return FALSE;
		}
		memcpy(pInfo->pBuf, pBuf, dwBufLen);
		pInfo->dwBufLen = dwBufLen;
		{
			TRACE("NetWorkMessCallBack triggered %08x in ClientDemo...\n", pInfo->lCommand);
		}
		dlg->PostMessage(WM_ALARM_INFO, (WPARAM)pInfo, (LPARAM)0);
	}

	return bRet;	
}

LRESULT CDlgCfgNetwork::OnNetWorkInfo(WPARAM wParam, LPARAM lParam)
{
	NetWorkInfo* pNetWorkInfo = (NetWorkInfo*)wParam;
	if (!pNetWorkInfo || !pNetWorkInfo->pBuf || pNetWorkInfo->dwBufLen <= 0)
	{
		return -1;
	}

	switch (pNetWorkInfo->lCommand)
	{
	case DH_CONFIG_RESULT_EVENT_EX:
		{
			DEV_SET_RESULT* pInfo = (DEV_SET_RESULT*)pNetWorkInfo->pBuf;
			CString csErr;
			// 返回码；0：成功，1：失败，2：数据不合法，3：暂时无法设置，4：没有权限
			if (pInfo->wResultCode != 0)
			{
				csErr.Format("Set NetWork Config error!");
				AfxMessageBox(csErr);
			}

		}
		break;
	default:
		break;
	}

	if (pNetWorkInfo ==  NULL)
	{
		delete[] pNetWorkInfo;
	}

	return 0;
}

BOOL CDlgCfgNetwork::OnInitDialog() 
{
	CDialog::OnInitDialog();
	g_SetWndStaticText(this, DLG_CFG_NETWORK);
	
	// TODO: Add extra initialization here
	m_ctlIp.SetAddress(0, 0, 0, 0);
	m_ctlMask.SetAddress(0, 0, 0, 0);
	m_ctlGateway.SetAddress(0, 0, 0, 0);

	// 设置回调接口
	CLIENT_SetDVRMessCallBack(NetWorkMessCallBack, (LDWORD)this);

	GetNetworkPara();
	UpdateData(FALSE);
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CDlgCfgNetwork::OnBtnCancel() 
{
	// TODO: Add your control notification handler code here
	CDialog::OnCancel();
}

void CDlgCfgNetwork::OnBtnSet() 
{
	// TODO: Add your control notification handler code here
	UpdateData(TRUE);

	if (0 == m_lLoginID)
	{
		AfxMessageBox("we haven't login a device yet!");
		return;
	}

	CString csIp, csMask, csGateway;
	m_ctlIp.GetWindowText(csIp);
	m_ctlMask.GetWindowText(csMask);
	m_ctlGateway.GetWindowText(csGateway);

//	CFG_NETWORK_INFO stuNetworkPara = {0};
	CFG_NETWORK_INFO& stuNetworkPara = m_stuNetwork;
	stuNetworkPara.nInterfaceNum = 1;
	strncpy(stuNetworkPara.stuInterfaces[0].szIP, csIp, MAX_ADDRESS_LEN-1);
	strncpy(stuNetworkPara.stuInterfaces[0].szSubnetMask, csMask, MAX_ADDRESS_LEN-1);
	strncpy(stuNetworkPara.stuInterfaces[0].szDefGateway, csGateway, MAX_ADDRESS_LEN-1);

	char szJsonBuf[1024] = {0};
	BOOL nRet = CLIENT_PacketData(CFG_CMD_NETWORK, &stuNetworkPara, sizeof(CFG_NETWORK_INFO), szJsonBuf, sizeof(szJsonBuf));
	if (!nRet)
	{
		AfxMessageBox("Packet network json buffer failed.");
		return;
	}

	int nErr = 0, nRestart = 0;
	nRet = CLIENT_SetNewDevConfig(m_lLoginID, CFG_CMD_NETWORK,
		-1, szJsonBuf, strlen(szJsonBuf), &nErr, &nRestart, 0);
	if (!nRet)
	{
		CString csErr;
		csErr.Format("SetupConfig=>%s failed: %08x...", CFG_CMD_NETWORK, CLIENT_GetLastError());
		AfxMessageBox(csErr);
		return ;
	}

 	AfxMessageBox("send network parameters successfully.");

	return;
}

void CDlgCfgNetwork::GetNetworkPara()
{
	if (0 == m_lLoginID)
	{
		AfxMessageBox("we haven't login a device yet!");
		return;
	}
	char szJsonBuf[1024] = {0};
	int nErr = 0;
	BOOL bRet = CLIENT_GetNewDevConfig(m_lLoginID, CFG_CMD_NETWORK,
		-1, szJsonBuf, sizeof(szJsonBuf), &nErr, 0);
	if (!bRet)
	{
		CString csErr;
		csErr.Format("GetConfig=>%s failed: %08x...", CFG_CMD_NETWORK, CLIENT_GetLastError());
		AfxMessageBox(csErr);
		return;
	}

	CFG_NETWORK_INFO stNetworkInfo = {0};
	DWORD dwRetLen = 0;
	bRet = CLIENT_ParseData(CFG_CMD_NETWORK, szJsonBuf, &stNetworkInfo, sizeof(stNetworkInfo), &dwRetLen);
	if (!bRet)
	{
		AfxMessageBox("Parse network jason failed.");
		return;
	}

	if (0 == stNetworkInfo.stuInterfaces[0].szIP[0] 
		|| 0 == stNetworkInfo.stuInterfaces[0].szSubnetMask[0]
		|| 0 == stNetworkInfo.stuInterfaces[0].szDefGateway[0])
	{
		AfxMessageBox("Invalidate ip parameters.");
		return;
	}

	memcpy(&m_stuNetwork, &stNetworkInfo, sizeof(CFG_NETWORK_INFO));

	m_ctlIp.SetWindowText(stNetworkInfo.stuInterfaces[0].szIP);
	m_ctlMask.SetWindowText(stNetworkInfo.stuInterfaces[0].szSubnetMask);
	m_ctlGateway.SetWindowText(stNetworkInfo.stuInterfaces[0].szDefGateway);

#if 0
	DWORD dwIp = ntohl(inet_addr(stNetworkInfo.stuInterfaces[0].szIP));
	m_ctlIp.SetAddress(dwIp);

	DWORD dwMask = ntohl(inet_addr(stNetworkInfo.stuInterfaces[0].szSubnetMask));
	m_ctlMask.SetAddress(dwMask);

	DWORD dwGateway = ntohl(inet_addr(stNetworkInfo.stuInterfaces[0].szDefGateway));
	m_ctlGateway.SetAddress(dwGateway);
#endif
}
