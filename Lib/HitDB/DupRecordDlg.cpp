// DupRecordDlg.cpp : implementation file
//

#include "stdafx.h"
#include "HitDB.h"
#include "DupRecordDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDupRecordDlg dialog


CDupRecordDlg::CDupRecordDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CDupRecordDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CDupRecordDlg)
	m_Artist = _T("");
	m_Title = _T("");
	//}}AFX_DATA_INIT
}


void CDupRecordDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDupRecordDlg)
	DDX_Text(pDX, IDC_HITDB_ARTIST, m_Artist);
	DDX_Text(pDX, IDC_HITDB_TITLE, m_Title);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CDupRecordDlg, CDialog)
	//{{AFX_MSG_MAP(CDupRecordDlg)
	ON_BN_CLICKED(IDC_HITDB_DUP_YES, OnDupYes)
	ON_BN_CLICKED(IDC_HITDB_DUP_YESALL, OnDupYesall)
	ON_BN_CLICKED(IDC_HITDB_DUP_NO, OnDupNo)
	ON_BN_CLICKED(IDC_HITDB_DUP_NOALL, OnDupNoall)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDupRecordDlg message handlers

void CDupRecordDlg::OnDupYes() 
{
	m_AddRecord = DUP_RECORD_UPDATE;
	CDialog::OnCancel();
}

void CDupRecordDlg::OnDupYesall() 
{
	m_AddRecord = DUP_RECORD_UPDATEALL;
	CDialog::OnCancel();
}

void CDupRecordDlg::OnDupNo() 
{
	m_AddRecord = DUP_RECORD_NOUPDATE;
	CDialog::OnCancel();
}

void CDupRecordDlg::OnDupNoall() 
{
	m_AddRecord = DUP_RECORD_NEVERUPDATE;
	CDialog::OnCancel();
}

void CDupRecordDlg::OnCancel() 
{
	m_AddRecord = DUP_RECORD_CANCEL;
	CDialog::OnCancel();
}
