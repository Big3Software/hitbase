// ChangeCodesProgressDlg.cpp : implementation file
//

#include "stdafx.h"
#include "HitDB.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDataBaseProgressDlg dialog


CDataBaseProgressDlg::CDataBaseProgressDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CDataBaseProgressDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CDataBaseProgressDlg)
	m_ProgressText = _T("");
	//}}AFX_DATA_INIT

	m_pQuery = NULL;
	m_pCDQuery = NULL;
}

void CDataBaseProgressDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDataBaseProgressDlg)
	DDX_Control(pDX, IDC_HITDB_ANIMATE1, m_Animate1Ctrl);
	DDX_Control(pDX, IDC_HITDB_PROGRESS, m_ProgressCtrl);
	DDX_Text(pDX, IDC_HITDB_PROCESSTEXT, m_ProgressText);
	//}}AFX_DATA_MAP
	DDX_Control(pDX, IDC_HITDB_PROCESSTEXT, m_stcStatus);
}


BEGIN_MESSAGE_MAP(CDataBaseProgressDlg, CDialog)
	//{{AFX_MSG_MAP(CDataBaseProgressDlg)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDataBaseProgressDlg message handlers

void CDataBaseProgressDlg::OnCancel() 
{
	CDialog::OnCancel();

	if (m_pQuery)
		m_pQuery->StopBackgroundSearch();

	if (m_pCDQuery)
		m_pCDQuery->StopSearch();
}

BOOL CDataBaseProgressDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	m_Animate1Ctrl.Open(IDA_HITDB_SEARCHANI);
	m_Animate1Ctrl.Play(0, (UINT)-1, (UINT)-1);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}
