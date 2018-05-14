// CoverEnterStringDlg.cpp : implementation file
//

#include "stdafx.h"
#include "cdcover.h"
#include "CoverEnterStringDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CCoverEnterStringDlg dialog


CCoverEnterStringDlg::CCoverEnterStringDlg(CString *pstrText, CWnd* pParent /*=NULL*/)
	: CDialog(CCoverEnterStringDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CCoverEnterStringDlg)
	m_strText = _T("");
	//}}AFX_DATA_INIT

	m_pString = pstrText;
	m_bOnce = TRUE;
}


void CCoverEnterStringDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CCoverEnterStringDlg)
	DDX_Text(pDX, IDC_COVEREDIT_STRING, m_strText);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CCoverEnterStringDlg, CDialog)
	//{{AFX_MSG_MAP(CCoverEnterStringDlg)
	ON_WM_PAINT()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CCoverEnterStringDlg message handlers

void CCoverEnterStringDlg::OnOK() 
{
	UpdateData(TRUE);
	*m_pString = m_strText;
	CDialog::OnOK();

	return;
}

BOOL CCoverEnterStringDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();

	UpdateData(TRUE);
	m_strText = *m_pString;
	UpdateData(FALSE);

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CCoverEnterStringDlg::OnPaint() 
{
	CPaintDC dc(this); // device context for painting

	if (m_bOnce)
	{
		m_bOnce = FALSE;
		CEdit *pEdit = (CEdit *)GetDlgItem(IDC_COVEREDIT_STRING);
		pEdit->SetFocus();
	}

	return;
}
