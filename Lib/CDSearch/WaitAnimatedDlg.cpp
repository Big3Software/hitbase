// WaitAnimatedDlg.cpp : implementation file
//

#include "stdafx.h"
#include "../../app/hitbase/resource.h"
#include "WaitAnimatedDlg.h"
#include "../hitmisc/httpfileasync.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

/////////////////////////////////////////////////////////////////////////////
// CWaitAnimatedDlg dialog


CWaitAnimatedDlg::CWaitAnimatedDlg(const CString& text, CWnd* pParent /*=NULL*/)
	: CDialog(CWaitAnimatedDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CWaitAnimatedDlg)
	m_Text = text;
	//}}AFX_DATA_INIT
}


void CWaitAnimatedDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CWaitAnimatedDlg)
	DDX_Control(pDX, IDC_ANIMATE1, m_AnimateCtrl);
	DDX_Text(pDX, IDC_TEXT, m_Text);
	DDX_Text(pDX, IDC_STATUS, m_sStatus);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CWaitAnimatedDlg, CDialog)
	//{{AFX_MSG_MAP(CWaitAnimatedDlg)
	ON_BN_CLICKED(IDCANCEL, OnCancel)
	ON_WM_DESTROY()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CWaitAnimatedDlg message handlers

BOOL CWaitAnimatedDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	m_AnimateCtrl.Open(IDA_SEARCHANI);
	m_AnimateCtrl.Play(0, (UINT)-1, (UINT)-1);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CWaitAnimatedDlg::OnDestroy() 
{
	m_AnimateCtrl.Close();

	CDialog::OnDestroy();
}

void CWaitAnimatedDlg::SetText(const CString& sText)
{
	m_Text = sText;
	UpdateData(FALSE);
}

void CWaitAnimatedDlg::SetStatus(const CString& sStatus)
{
	m_sStatus = sStatus;
	UpdateData(FALSE);
}

void CWaitAnimatedDlg::SetHttpFileAsync(CHttpFileAsync* pHttpFileAsync)
{
	m_pHttpFileAsync = pHttpFileAsync;
}

void CWaitAnimatedDlg::OnCancel() 
{
	//m_pHttpFileAsync->AbortTransfer();
}

