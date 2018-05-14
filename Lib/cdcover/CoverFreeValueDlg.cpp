// CoverFreeValueDlg.cpp : implementation file
//

#include "stdafx.h"
#include "cdcover.h"
#include "CoverFreeValueDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CCoverFreeValueDlg dialog


CCoverFreeValueDlg::CCoverFreeValueDlg(int *pValue, CWnd* pParent /*=NULL*/)
	: CDialog(CCoverFreeValueDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CCoverFreeValueDlg)
	m_nValue = 0;
	//}}AFX_DATA_INIT

	m_pValue = pValue;
}


void CCoverFreeValueDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CCoverFreeValueDlg)
	DDX_Text(pDX, IDC_EDIT_FREEVALUE, m_nValue);
	DDV_MinMaxInt(pDX, m_nValue, 0, 1200);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CCoverFreeValueDlg, CDialog)
	//{{AFX_MSG_MAP(CCoverFreeValueDlg)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CCoverFreeValueDlg message handlers

void CCoverFreeValueDlg::OnOK() 
{
	// TODO: Add extra validation here

	UpdateData(TRUE);
	*m_pValue = m_nValue;
	
	CDialog::OnOK();
}

BOOL CCoverFreeValueDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();

	m_nValue = *m_pValue;

	UpdateData(FALSE);
		
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}
