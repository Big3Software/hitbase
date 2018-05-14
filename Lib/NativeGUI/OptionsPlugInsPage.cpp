// OptionsPlugInsPage.cpp : implementation file
//

#include "stdafx.h"
#include "resource.h"
#include "OptionsPlugInsPage.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// COptionsPlugInsPage property page

IMPLEMENT_DYNCREATE(COptionsPlugInsPage, CPropertyPage)

COptionsPlugInsPage::COptionsPlugInsPage() : CPropertyPage(COptionsPlugInsPage::IDD)
{
	//{{AFX_DATA_INIT(COptionsPlugInsPage)
/*TODO!!!!!!!!!!!!!!!	m_sPlugInDirectory = ((CHitbaseWinAppBase*)AfxGetApp())->m_config.m_sPlugInDirectory;*/
	m_sVersion = _T("");
	m_sCopyright = _T("");
	m_sURL = _T("");
	m_sComment = _T("");
	//}}AFX_DATA_INIT
}

COptionsPlugInsPage::~COptionsPlugInsPage()
{
}

void COptionsPlugInsPage::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(COptionsPlugInsPage)
	DDX_Control(pDX, IDC_MODULE, m_ModuleCtrl);
	DDX_Control(pDX, IDC_LIBRARY, m_LibraryCtrl);
	DDX_Text(pDX, IDC_PLUGIN_DIRECTORY, m_sPlugInDirectory);
	DDX_Text(pDX, IDC_VERSION, m_sVersion);
	DDX_Text(pDX, IDC_COPYRIGHT, m_sCopyright);
	DDX_Text(pDX, IDC_URL, m_sURL);
	DDX_Text(pDX, IDC_COMMENT, m_sComment);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(COptionsPlugInsPage, CPropertyPage)
	//{{AFX_MSG_MAP(COptionsPlugInsPage)
	ON_BN_CLICKED(IDC_RELOAD_PLUGINS, OnReloadPlugins)
	ON_CBN_SELCHANGE(IDC_LIBRARY, OnSelchangeLibrary)
	ON_CBN_SELCHANGE(IDC_MODULE, OnSelchangeModule)
	ON_BN_CLICKED(IDC_OPTIONS, OnOptions)
	ON_BN_CLICKED(IDC_BROWSE, OnBrowse)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// COptionsPlugInsPage message handlers

void COptionsPlugInsPage::OnReloadPlugins() 
{
	UpdateData(TRUE);

	//Achtung,wenn Plug-Ins offen sind!
/*TODO!!!!!!!!!!!!!!!!!!!!!		((CHitbaseWinAppBase*)AfxGetApp())->m_pPlugInManager->ReadPlugIns(m_sPlugInDirectory);

	if (HitbaseFrame)
		HitbaseFrame->AddPlugInBitmaps();*/

	FillPlugIns();
}

BOOL COptionsPlugInsPage::OnInitDialog() 
{
	CPropertyPage::OnInitDialog();
	
	FillPlugIns();
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void COptionsPlugInsPage::FillPlugIns()
{
//TODO!!!!!!!!!!!!!!!!!!!!!	((CHitbaseWinAppBase*)AfxGetApp())->m_pPlugInManager->FillLibrariesInComboBox(&m_LibraryCtrl);
}

void COptionsPlugInsPage::OnSelchangeLibrary() 
{
	/*TODO!!!!!!!!!!!!!!!!!!!!!!! int nSel = m_LibraryCtrl.GetCurSel();

	if (nSel >= 0)
	{
		int nBibIndex = m_LibraryCtrl.GetItemData(nSel);

		// JUS 990513: Die Liste der Module zuerst löschen
		m_ModuleCtrl.ResetContent();

		((CHitbaseWinAppBase*)AfxGetApp())->m_pPlugInManager->FillModulesInComboBox(&m_ModuleCtrl, nBibIndex);
	}*/
}

void COptionsPlugInsPage::OnSelchangeModule() 
{
	int nSel = m_ModuleCtrl.GetCurSel();

	if (nSel >= 0)
	{
		ShowPlugInInformation(m_ModuleCtrl.GetItemData(nSel));
	}
}

void COptionsPlugInsPage::ShowPlugInInformation(int nPlugInIndex)
{
/*TODO!!!!!!!!!!!!!!!!!!!!	HPI_INFO* phpiInit;

	phpiInit = ((CHitbaseWinAppBase*)AfxGetApp())->m_pPlugInManager->GetPlugInInformation(nPlugInIndex);

	m_sVersion = phpiInit->szhpiVersion;
	m_sCopyright = phpiInit->szCopyright;
	m_sURL = phpiInit->szURL;
	m_sComment = phpiInit->szComment;

	UpdateData(FALSE);*/
}

void COptionsPlugInsPage::OnOK() 
{
	CPropertyPage::OnOK();

//TODO!!!!!!!!!!!!!!!!!!!!!!!!!	((CHitbaseWinAppBase*)AfxGetApp())->m_config.m_sPlugInDirectory = m_sPlugInDirectory;
}

void COptionsPlugInsPage::OnOptions() 
{
	int nSel = m_ModuleCtrl.GetCurSel();

	if (nSel >= 0)
	{
		// Zeigt einen Dialog, in dem alle Optionen des Plug-Ins eingestellt werden
		// können.
		//TODO!!!!!!!!!!!!!!!!!!!!!((CHitbaseWinAppBase*)AfxGetApp())->m_pPlugInManager->ChangeOptions(m_ModuleCtrl.GetItemData(nSel));
	}
}

void COptionsPlugInsPage::OnBrowse()
{
	UpdateData(TRUE);

	/*TODO!!!!!!!!!!!!!!!!!!!CString sDir;
	sDir = CMisc::BrowseForDirectory(this, get_string(TEXT_CHOOSE_PLUGINS_DIRECTORY), m_sPlugInDirectory);
	if (!sDir.IsEmpty())
		m_sPlugInDirectory = sDir;*/

	UpdateData(FALSE);
}
