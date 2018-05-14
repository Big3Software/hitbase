// CDArchiveNewDlg.cpp : implementation file
//

#include "stdafx.h"
#include "../hitmisc/HitbaseWinAppBase.h"
#include "../../app/hitbase/resource.h"
#include "CDArchiveNewDlg.h"
#include "../hitmisc/misc.h"
#include "../hitmisc/newfiledialog.h"
#include "resource.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

/////////////////////////////////////////////////////////////////////////////
// CCDArchiveNewDlg dialog


CCDArchiveNewDlg::CCDArchiveNewDlg(BOOL bEdit, CWnd* pParent /*=NULL*/)
	: CDialog(CCDArchiveNewDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CCDArchiveNewDlg)
	m_bAutoSearch = TRUE;
	m_sSource = _T("");
	m_nType = -1;
	m_bActive = TRUE;
	m_bUpload = TRUE;
	m_bAutoCreateSampler = FALSE;
	m_sSamplerTrennzeichen = _T("");
	//}}AFX_DATA_INIT

	m_bEdit = bEdit;
}


void CCDArchiveNewDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CCDArchiveNewDlg)
	DDX_Control(pDX, IDC_COMBO1, m_SourceCtrl);
	DDX_Control(pDX, IDC_TYPE, m_TypeCtrl);
	DDX_Control(pDX, IDOK, m_OKCtrl);
	DDX_Control(pDX, IDC_BROWSE, m_BrowseCtrl);
	DDX_Check(pDX, IDC_AUTOSEARCH, m_bAutoSearch);
	DDX_CBString(pDX, IDC_COMBO1, m_sSource);
	DDX_CBIndex(pDX, IDC_TYPE, m_nType);
	DDX_Check(pDX, IDC_ACTIVE, m_bActive);
	DDX_Check(pDX, IDC_UPLOAD, m_bUpload);
	DDX_Check(pDX, IDC_CREATE_SAMPLER, m_bAutoCreateSampler);
	DDX_Control(pDX, IDC_UPLOAD, m_UploadCtrl);
	DDX_Text(pDX, IDC_SAMPLER_TRENNZEICHEN, m_sSamplerTrennzeichen);
	DDX_Control(pDX, IDC_CREATE_SAMPLER, m_CreateSamplerCtrl);
	DDX_Control(pDX, IDC_SAMPLER_TRENNZEICHEN, m_SamplerTrennzeichenCtrl);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CCDArchiveNewDlg, CDialog)
	//{{AFX_MSG_MAP(CCDArchiveNewDlg)
	ON_BN_CLICKED(IDC_BROWSE, OnBrowse)
	ON_CBN_SELCHANGE(IDC_TYPE, OnSelchangeType)
	ON_CBN_EDITCHANGE(IDC_COMBO1, OnChangeEdit1)
	ON_CBN_SELCHANGE(IDC_COMBO1, OnSelchangeCombo1)
	ON_BN_CLICKED(IDC_CREATE_SAMPLER, OnCreateSampler)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CCDArchiveNewDlg message handlers

BOOL CCDArchiveNewDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();

	if (m_bEdit)
		SetWindowText(get_string(TEXT_EDIT_CDARCHIV));

	// Reihenfolge nicht ändern!!!
	m_TypeCtrl.AddString(get_string(TEXT_HITBASE_FILE));
	m_TypeCtrl.AddString(get_string(TEXT_HITBASE_INTERNET));
	m_TypeCtrl.AddString(get_string(TEXT_CDDB) + L" (Sockets)");
	m_TypeCtrl.AddString(get_string(TEXT_CDDB) + L" (Http)");
	m_TypeCtrl.AddString(get_string(IDS_CDARCHIVE_LOCAL));
	// TODO!!!!!!!!!!!!!!!!!!!!!!!!!!! 
	// Auch den String für IDS_CDARCHIVE_LOCAL und TEXT_HITBASE_FILE ändern???
	m_TypeCtrl.AddString(L"CDDB Lokale Dateien");

	UpdateData(FALSE);     // Muß ich hier nochmal machen, damit der String in der Combobox ausgewählt wird,

	UpdateWindowState();

	FillArchiveList();

	UpdateData(FALSE);     // Muß ich hier nochmal machen, damit der String in der Combobox ausgewählt wird,

	return TRUE;
}

void CCDArchiveNewDlg::OnBrowse() 
{
	UpdateData(TRUE);

	if (m_nType == (int)CDArchiveType::CDArchiveLocalCDDB)
	{
		CString sDir;
		// TODO!!!!!!!!!!!!!!!!!!!!
		sDir = CMisc::BrowseForDirectory((CWnd*)this, L"Verzeichnis mit CDDB Daten wählen", L"");
		if (sDir.GetLength() > 3)
		{
			m_sSource = sDir;
			UpdateData(FALSE);
		}
	}

	if (m_nType == (int)CDArchiveType::CDArchiveLocal || m_nType == (int)CDArchiveType::File)
	{
		CNewFileDialog FileDialog (TRUE, m_nType == (int)CDArchiveType::CDArchiveLocal ? L"mdb" : L"hdb", m_sSource, OFN_HIDEREADONLY|OFN_OVERWRITEPROMPT,
				get_string(m_nType == (int)CDArchiveType::CDArchiveLocal ? IDS_CDARCHIVE_LOCALEXT : TEXT_FILESAVEDIALOG_EXT));
		
		CString str = get_string(TEXT_BROWSE_FOR_CD_ARCHIVE);
		FileDialog.m_ofn.lpstrTitle = str;

		if (FileDialog.DoModal(L"CDArchive") == IDOK)
		{
			m_sSource = FileDialog.GetPathName();
			UpdateData(FALSE);
		}
	}
	
	UpdateWindowState();
}

void CCDArchiveNewDlg::OnSelchangeType() 
{
	UpdateData(TRUE);
	if (m_nType != (int)CDArchiveType::BIG3 && 
		m_bUpload && 
		m_nType != (int)CDArchiveType::CDDBsockets && 
		m_nType != (int)CDArchiveType::CDDBhttp)
	{
		m_bUpload = FALSE;
		UpdateData(FALSE);
	}
	else
	{
		if (m_nType == (int)CDArchiveType::CDDBhttp)
			m_bUpload = TRUE;
		if (m_nType == (int)CDArchiveType::CDDBsockets)
			m_bUpload = FALSE;

		UpdateData(FALSE);
	}

	if (m_nType != (int)CDArchiveType::CDDBsockets && 
		m_nType != (int)CDArchiveType::CDDBhttp &&
		m_nType != (int)CDArchiveType::CDArchiveLocalCDDB)
	{
		m_bAutoCreateSampler = FALSE;

		UpdateData(FALSE);
	}
	else
	{
		m_bAutoCreateSampler = TRUE;
		m_sSamplerTrennzeichen = "/";
		UpdateData(FALSE);
	}

	UpdateWindowState();

	FillArchiveList();
}

void CCDArchiveNewDlg::UpdateWindowState()
{
	UpdateData(TRUE);
	
	m_BrowseCtrl.EnableWindow(!m_nType || m_nType == (int)CDArchiveType::CDArchiveLocal || m_nType == (int)CDArchiveType::CDArchiveLocalCDDB);
	m_OKCtrl.EnableWindow(m_nType >= (int)CDArchiveType::File && !m_sSource.IsEmpty());
	m_UploadCtrl.EnableWindow(m_nType == (int)CDArchiveType::BIG3 || m_nType == (int)CDArchiveType::CDDBhttp);
	m_CreateSamplerCtrl.EnableWindow(m_nType == (int)CDArchiveType::CDDBsockets || m_nType == (int)CDArchiveType::CDDBhttp || m_nType == (int)CDArchiveType::CDArchiveLocalCDDB);
	m_SamplerTrennzeichenCtrl.EnableWindow(m_bAutoCreateSampler && (m_nType == (int)CDArchiveType::CDDBsockets || m_nType == (int)CDArchiveType::CDDBhttp || m_nType == (int)CDArchiveType::CDArchiveLocalCDDB));
}

void CCDArchiveNewDlg::OnChangeEdit1() 
{
	UpdateWindowState();
}

void CCDArchiveNewDlg::FillArchiveList()
{
	m_SourceCtrl.ResetContent();

	switch(m_nType)
	{
	case CDArchiveType::BIG3:
		m_SourceCtrl.AddString(L"www.cdarchiv.de");
		break;
	case CDArchiveType::CDDBsockets:
 		m_SourceCtrl.AddString(L"freedb.freedb.org:8880");
		break;
	case CDArchiveType::CDDBhttp:
		m_SourceCtrl.AddString(L"freedb.freedb.org/~cddb/cddb.cgi");
		break;
	}
}

// JUS 16-Sep-98: Damit auch beim Auswählen eines Elementes aus der Liste der OK-Button aktiv wird.
void CCDArchiveNewDlg::OnSelchangeCombo1() 
{
	int sel = m_SourceCtrl.GetCurSel();

	// JUS 990128
	if (sel >= 0)
	{
		m_SourceCtrl.GetLBText(sel, m_sSource);
		UpdateData(FALSE);
	}

	UpdateWindowState();
}

void CCDArchiveNewDlg::OnCreateSampler() 
{
	UpdateWindowState();
}

void CCDArchiveNewDlg::OnOK() 
{
	UpdateData(TRUE);

	if (m_bAutoCreateSampler && m_sSamplerTrennzeichen.IsEmpty())
	{
		CString str;
		str.LoadString(IDS_ENTER_TRENNZEICHEN);
		AfxMessageBox(str, MB_ICONINFORMATION|MB_OK);
		return;
	}

	CDialog::OnOK();
}
