// DataBaseErrorDlg.cpp : implementation file
//

#include "stdafx.h"
#include "resource.h"
#include "DataBaseErrorDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDataBaseErrorDlg dialog


CDataBaseErrorDlg::CDataBaseErrorDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CDataBaseErrorDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CDataBaseErrorDlg)
	m_bNoMoreErrors = FALSE;
	//}}AFX_DATA_INIT
}


void CDataBaseErrorDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDataBaseErrorDlg)
	DDX_Check(pDX, IDC_HITDB_CHECK1, m_bNoMoreErrors);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CDataBaseErrorDlg, CDialog)
	//{{AFX_MSG_MAP(CDataBaseErrorDlg)
		// NOTE: the ClassWizard will add message map macros here
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDataBaseErrorDlg message handlers
