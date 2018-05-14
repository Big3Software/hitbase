// OptionsCDArchiveFieldsDlg.cpp : implementation file
//

#include "stdafx.h"
#include "OptionsCDArchiveFieldsDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// COptionsCDArchiveFieldsDlg dialog


COptionsCDArchiveFieldsDlg::COptionsCDArchiveFieldsDlg(CWnd* pParent /*=NULL*/)
	: CDialog(COptionsCDArchiveFieldsDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(COptionsCDArchiveFieldsDlg)
	m_bCategory = FALSE;
	m_bMedium = FALSE;
	m_bCDComment = FALSE;
	m_bBPM = FALSE;
	m_bTrackComment = FALSE;
	m_bLyrics = FALSE;
	//}}AFX_DATA_INIT
}


void COptionsCDArchiveFieldsDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(COptionsCDArchiveFieldsDlg)
	DDX_Check(pDX, IDC_CATEGORY, m_bCategory);
	DDX_Check(pDX, IDC_MEDIUM, m_bMedium);
	DDX_Check(pDX, IDC_CDCOMMENT, m_bCDComment);
	DDX_Check(pDX, IDC_BPM, m_bBPM);
	DDX_Check(pDX, IDC_TRACKCOMMENT, m_bTrackComment);
	DDX_Check(pDX, IDC_LYRICS, m_bLyrics);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(COptionsCDArchiveFieldsDlg, CDialog)
	//{{AFX_MSG_MAP(COptionsCDArchiveFieldsDlg)
		// NOTE: the ClassWizard will add message map macros here
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// COptionsCDArchiveFieldsDlg message handlers
