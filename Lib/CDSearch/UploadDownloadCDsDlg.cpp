// UploadDownloadCDsDlg.cpp : implementation file
//

#include "stdafx.h"
//#include "..\..\app\hitbase\hitbase.h"
#include "resource.h"
#include "CDArchive.h"
#include "..\hitmisc\misc.h"
#include "UploadDownloadCDsDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

/////////////////////////////////////////////////////////////////////////////
// CUploadDownloadCDsDlg dialog


CUploadDownloadCDsDlg::CUploadDownloadCDsDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CUploadDownloadCDsDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CUploadDownloadCDsDlg)
	m_sCurrentAction = _T("");
	//}}AFX_DATA_INIT

	m_bModeless = FALSE;
}


void CUploadDownloadCDsDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CUploadDownloadCDsDlg)
	DDX_Control(pDX, IDC_CDSEARCH_PROGRESS1, m_ProgressCtrl);
	DDX_Control(pDX, IDC_CDSEARCH_START, m_StartCtrl);
	DDX_Control(pDX, IDC_CDSEARCH_CANCEL, m_CancelCtrl);
	DDX_Control(pDX, IDC_CDSEARCH_DETAILS, m_DetailsCtrl);
	DDX_Control(pDX, IDC_CDSEARCH_ERRORS, m_ErrorsCtrl);
	DDX_Control(pDX, IDC_CDSEARCH_LIST, m_ListCtrl);
	DDX_Text(pDX, IDC_CDSEARCH_ACTION, m_sCurrentAction);
	DDX_Control(pDX, IDC_CDSEARCH_ACTION, m_stcCurrentAction);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CUploadDownloadCDsDlg, CDialog)
	//{{AFX_MSG_MAP(CUploadDownloadCDsDlg)
	ON_BN_CLICKED(IDC_CDSEARCH_DETAILS, OnDetails)
	ON_BN_CLICKED(IDC_CDSEARCH_START, OnStarted)
	ON_BN_CLICKED(IDC_CDSEARCH_CANCEL, OnCdsearchCancel)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CUploadDownloadCDsDlg message handlers

void CUploadDownloadCDsDlg::OnDetails() 
{
	// Details einblenden
	CRect rect, rect1;
	m_ErrorsCtrl.GetWindowRect(&rect);
	GetWindowRect(&rect1);
	SetWindowPos(NULL, 0, 0, rect1.Width(), rect.bottom+5-rect1.top, SWP_NOZORDER|SWP_NOMOVE);

	m_DetailsCtrl.EnableWindow(FALSE);
}

BOOL CUploadDownloadCDsDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	// Details zunächst ausblenden
	CRect rect, rect1;
	m_ListCtrl.GetWindowRect(&rect);
	GetWindowRect(&rect1);
	SetWindowPos(NULL, 0, 0, rect1.Width(), rect.bottom+10-rect1.top, SWP_NOZORDER|SWP_NOMOVE);

	m_DetailsCtrl.EnableWindow(FALSE);

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

BOOL CUploadDownloadCDsDlg::Create(CWnd* pParentWnd) 
{
	m_bModeless = TRUE;

	return CDialog::Create(IDD, pParentWnd);
}

void CUploadDownloadCDsDlg::OnCdsearchCancel() 
{
	if (m_bModeless)
		DestroyWindow();
	else
		CDialog::OnCancel();
}

void CUploadDownloadCDsDlg::OnStarted() 
{
	m_StartCtrl.EnableWindow(FALSE);

	CDArchives->StartUploadDownload();
}

void CUploadDownloadCDsDlg::AddDetailsString(const CString &sMessage)
{
	CString str;
	m_ErrorsCtrl.GetWindowText(str);
	str += sMessage + "\r\n";
	m_ErrorsCtrl.SetWindowText(str);
	m_DetailsCtrl.EnableWindow(TRUE);
}
