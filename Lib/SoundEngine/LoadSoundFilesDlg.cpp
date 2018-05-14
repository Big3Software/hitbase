// LoadSoundFilesDlg.cpp : implementation file
//

#include "stdafx.h"
#include "SoundEngineIntern.h"
#include "LoadSoundFilesDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CLoadSoundFilesDlg dialog


CLoadSoundFilesDlg::CLoadSoundFilesDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CLoadSoundFilesDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CLoadSoundFilesDlg)
	m_sFileName = _T("");
	//}}AFX_DATA_INIT
	
	m_bCanceled = FALSE;
}


void CLoadSoundFilesDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CLoadSoundFilesDlg)
	DDX_Control(pDX, IDC_SOUNDENGINE_FILENAME, m_FilenameCtrl);
	DDX_Control(pDX, IDC_SOUNDENGINE_PROGRESS, m_ProgressCtrl);
	DDX_Text(pDX, IDC_SOUNDENGINE_FILENAME, m_sFileName);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CLoadSoundFilesDlg, CDialog)
	//{{AFX_MSG_MAP(CLoadSoundFilesDlg)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CLoadSoundFilesDlg message handlers

void CLoadSoundFilesDlg::OnCancel() 
{
	m_bCanceled = TRUE;
	
	CDialog::OnCancel();
}
