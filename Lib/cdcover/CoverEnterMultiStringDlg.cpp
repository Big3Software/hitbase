// CoverEnterMultiStringDlg.cpp : implementation file
//

#include "stdafx.h"
#include "cdcover.h"
#include "CoverEnterMultiStringDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CCoverEnterMultiStringDlg dialog


CCoverEnterMultiStringDlg::CCoverEnterMultiStringDlg(CString *pstrText, CWnd* pParent /*=NULL*/)
	: CDialog(CCoverEnterMultiStringDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CCoverEnterMultiStringDlg)
	m_strText = _T("");
	//}}AFX_DATA_INIT

	m_pString = pstrText;
	m_bOnce = TRUE;
}


void CCoverEnterMultiStringDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CCoverEnterMultiStringDlg)
	DDX_Text(pDX, IDC_COVEREDIT_MULTISTRING, m_strText);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CCoverEnterMultiStringDlg, CDialog)
	//{{AFX_MSG_MAP(CCoverEnterMultiStringDlg)
	ON_WM_PAINT()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CCoverEnterMultiStringDlg message handlers

BOOL CCoverEnterMultiStringDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	UpdateData(TRUE);
	m_strText = *m_pString;
	UpdateData(FALSE);

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CCoverEnterMultiStringDlg::OnPaint() 
{
	CPaintDC dc(this); // device context for painting

	if (m_bOnce)
	{
		m_bOnce = FALSE;
		CEdit *pEdit = (CEdit *)GetDlgItem(IDC_COVEREDIT_MULTISTRING);
		pEdit->SetFocus();
	}

	return;

}

void CCoverEnterMultiStringDlg::OnOK() 
{
	UpdateData(TRUE);
	*m_pString = m_strText;
	CDialog::OnOK();

	return;
}
