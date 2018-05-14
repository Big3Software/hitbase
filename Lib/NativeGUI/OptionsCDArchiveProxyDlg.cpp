// OptionsCDArchiveProxyDlg.cpp: Implementierungsdatei
//

#include "stdafx.h"
#include "OptionsCDArchiveProxyDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// Dialogfeld COptionsCDArchiveProxyDlg 


COptionsCDArchiveProxyDlg::COptionsCDArchiveProxyDlg(CWnd* pParent /*=NULL*/)
	: CDialog(COptionsCDArchiveProxyDlg::IDD, pParent)
{
	m_nProxyType = Settings::Current->ProxyType;

	int iPos = Settings::Current->ProxyServerName->LastIndexOf(':');
	if (iPos >= 0)
	{
		m_sProxyServerName = Settings::Current->ProxyServerName->Substring(0, iPos);
		m_iPort = Big3::Hitbase::Miscellaneous::Misc::Atoi(Settings::Current->ProxyServerName->Substring(iPos+1));
	}
	else
	{
		m_sProxyServerName = Settings::Current->ProxyServerName;
		m_iPort = 0;
	}
}


void COptionsCDArchiveProxyDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(COptionsCDArchiveProxyDlg)
	DDX_Control(pDX, IDC_PROXY_SERVER_PORT, m_ProxyServerPortCtrl);
	DDX_Control(pDX, IDC_PROXY_SERVER_NAME, m_ProxyServerNameCtrl);
	DDX_Control(pDX, IDC_NO_PROXY, m_NoProxyCtrl);
	DDX_Control(pDX, IDC_PROXY, m_ProxyCtrl);
	DDX_Radio(pDX, IDC_NO_PROXY, m_nProxyType);
	DDX_Text(pDX, IDC_PROXY_SERVER_NAME, m_sProxyServerName);
	DDX_Text(pDX, IDC_PROXY_SERVER_PORT, m_iPort);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(COptionsCDArchiveProxyDlg, CDialog)
	//{{AFX_MSG_MAP(COptionsCDArchiveProxyDlg)
	ON_BN_CLICKED(IDC_PROXY, OnProxy)
	ON_BN_CLICKED(IDC_NO_PROXY, OnNoProxy)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// Behandlungsroutinen für Nachrichten COptionsCDArchiveProxyDlg

BOOL COptionsCDArchiveProxyDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	UpdateWindowState();
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX-Eigenschaftenseiten sollten FALSE zurückgeben
}

void COptionsCDArchiveProxyDlg::OnProxy() 
{
	UpdateWindowState();
}

void COptionsCDArchiveProxyDlg::OnNoProxy() 
{
	UpdateWindowState();
}

//Die Controls des Fensters (de)aktivieren
void COptionsCDArchiveProxyDlg::UpdateWindowState()
{
	UpdateData(TRUE);

	m_ProxyServerNameCtrl.EnableWindow(m_nProxyType == 1);
	m_ProxyServerPortCtrl.EnableWindow(m_nProxyType == 1);
}

