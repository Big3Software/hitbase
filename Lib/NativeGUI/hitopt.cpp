// hitopt.cpp : implementation file
//

#include "stdafx.h"
#include "hitopt.h"
#include "shlobj.h"
#include "OptionsCDArchiveFieldsDlg.h"
#include "OptionsCDArchiveProxyDlg.h"
#include "..\..\Lib\hitMisc\hitmisc.h"
#include "CDArchiveNewDlg.h"

#ifdef _DEBUG
#undef THIS_FILE
static char BASED_CODE THIS_FILE[] = __FILE__;
#endif

#define SYMBOL_INTERPRET   1
#define SYMBOL_TITLE       2
#define SYMBOL_TRACKNR     4
#define SYMBOL_TRACKNAME   8
#define SYMBOL_TRACKTIME   16
#define SYMBOL_TOTALTIME   32
#define SYMBOL_TRACKARTIST 64

/////////////////////////////////////////////////////////////////////////////
// COptCDPlayer property page

IMPLEMENT_DYNCREATE(COptCDPlayer, CPropertyPage)

COptCDPlayer::COptCDPlayer() : CPropertyPage(COptCDPlayer::IDD)
, m_bDisableCDText(FALSE)
{
	//{{AFX_DATA_INIT(COptCDPlayer)
	m_Intro = 0;
	m_OnNewCD = -1;
	m_OnExit = -1;
	m_OnStart = -1;
	m_AutoRepeat = FALSE;
	m_nLapView = -1;
	m_bCenterCurrentTrack = Settings::Current->CenterCurrentTrack;
	m_bDisableCDText = Settings::Current->DisableCDText;
	//}}AFX_DATA_INIT
}

COptCDPlayer::~COptCDPlayer()
{
}

void COptCDPlayer::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(COptCDPlayer)
	DDX_Control(pDX, ID_OPTIONS_STARTINTRO, m_IntroStartCtrl);
	DDX_Control(pDX, ID_OPTIONS_STARTINTROUPDOWN, m_IntroStartUpDown);
	DDX_Control(pDX, ID_OPTIONS_INTROUPDOWN, m_IntroUpDown);
	DDX_Text(pDX, ID_OPTIONS_INTRO, m_Intro);
	DDV_MinMaxInt(pDX, m_Intro, 1, 90);
	DDX_Radio(pDX, IDC_NEWCD, m_OnNewCD);
	DDX_Radio(pDX, IDC_ONEXIT, m_OnExit);
	DDX_Radio(pDX, IDC_RADIO1, m_OnStart);
	DDX_Check(pDX, IDC_AUTOREPEAT, m_AutoRepeat);
	DDX_Radio(pDX, IDC_LAPABS, m_nLapView);
	DDX_Check(pDX, IDC_CENTER_CURRENT_TRACK, m_bCenterCurrentTrack);
	//}}AFX_DATA_MAP
	DDX_Check(pDX, IDC_DISABLE_CDTEXT, m_bDisableCDText);
}


BEGIN_MESSAGE_MAP(COptCDPlayer, CPropertyPage)
	//{{AFX_MSG_MAP(COptCDPlayer)
	ON_WM_VSCROLL()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()


/////////////////////////////////////////////////////////////////////////////
// COptCDPlayer message handlers

BOOL COptCDPlayer::OnInitDialog() 
{
//	char*p=NULL;*p=1;
   m_AutoRepeat = Settings::Current->AutoRepeat;
   m_OnStart = Settings::Current->OnStart;

   m_Intro = Settings::Current->IntroSeconds;

   m_OnNewCD = Settings::Current->OnNewCD;
   m_OnExit = Settings::Current->OnExit;

   m_nLapView = Settings::Current->LapView;

   CPropertyPage::OnInitDialog();
	
   m_IntroStartUpDown.SetRange(0, MAX_INTRO_TIME+1);
   m_IntroStartUpDown.SetPos(Settings::Current->IntroStart+1);

   if (Settings::Current->IntroStart < 0)        // Mitte
      {
      CString rcstr;

      rcstr.LoadString(TEXT_MID);
      m_IntroStartCtrl.SetWindowText(rcstr);
      }
   else
      m_IntroStartCtrl.SetWindowText(CMisc::Long2Time(Settings::Current->IntroStart*1000));

   m_IntroUpDown.SetRange(1, 90);
   m_IntroUpDown.SetPos(Settings::Current->IntroSeconds);

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void COptCDPlayer::OnOK() 
{
	CPropertyPage::OnOK();
	
	CString IntroStart;
	
	if (!LOWORD(m_IntroStartUpDown.GetPos()))
	{
		Settings::Current->IntroStart = -1;
	}
	else
	{
		m_IntroStartCtrl.GetWindowText(IntroStart);
		Settings::Current->IntroStart = time2long(IntroStart)/1000;
	}
	
	Settings::Current->AutoRepeat = m_AutoRepeat;
	Settings::Current->IntroSeconds = m_Intro;
	
	Settings::Current->OnNewCD = m_OnNewCD;
	Settings::Current->OnExit = m_OnExit;
	Settings::Current->OnStart = m_OnStart;
	
	Settings::Current->LapView = m_nLapView;
	Settings::Current->CenterCurrentTrack = m_bCenterCurrentTrack;
	Settings::Current->DisableCDText = m_bDisableCDText;
}

void COptCDPlayer::OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar) 
{
	if (pScrollBar->GetDlgCtrlID() == ID_OPTIONS_STARTINTROUPDOWN)
	{
		int CurPos = LOWORD(m_IntroStartUpDown.GetPos());
		
		if (!CurPos)
		{
			CString rcstr;
			
			rcstr.LoadString(TEXT_MID);
			m_IntroStartCtrl.SetWindowText(rcstr);
		}
		else
			m_IntroStartCtrl.SetWindowText(CMisc::Long2Time((CurPos-1)*1000));
	}
	
	CPropertyPage::OnVScroll(nSBCode, nPos, pScrollBar);
}

/////////////////////////////////////////////////////////////////////////////
// COptDatabase property page

IMPLEMENT_DYNCREATE(COptDatabase, CPropertyPage)

COptDatabase::COptDatabase() : CPropertyPage(COptDatabase::IDD)
, m_bAutoCompleteArtist(FALSE)
, m_bAutoCompleteCDTitle(FALSE)
, m_bAutoCompleteTrackname(FALSE)
, m_bAutoSaveCDs(FALSE)
, m_bFreeArchiveNumberSearchWithGaps(FALSE)
{
	//{{AFX_DATA_INIT(COptDatabase)
	m_ShowOnlyUsedCodes = FALSE;
	m_AutoBackup = FALSE;
//	m_bAutoSearch = FALSE;
	m_bAutoDateToday = FALSE;
	m_bSamplerVorgabe = Big3::Hitbase::Configuration::Settings::Current->SamplerUseFixedArtist ? TRUE : FALSE;
	m_SamplerVorgabeText = CString(Big3::Hitbase::Configuration::Settings::Current->SamplerFixedArtistText);
	m_bNoDupArchiveNr = Settings::Current->NoDuplicateArchiveNumbers;
	m_sAutoBackupDirectory = Settings::Current->AutoBackupDirectory;
	//}}AFX_DATA_INIT
    m_bAutoSaveCDs = Settings::Current->AutoSaveCDs;
	m_bArchiveNumberNumeric = Big3::Hitbase::Configuration::Settings::Current->SortArchiveNumberNumeric ? TRUE : FALSE;
}

COptDatabase::~COptDatabase()
{
}

void COptDatabase::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(COptDatabase)
	DDX_Control(pDX, IDC_SAMPLER_TEXT_STATIC, m_SamplerVorgabeTextStaticCtrl);
	DDX_Control(pDX, IDC_SAMPLER_TEXT, m_SamplerVorgabeTextCtrl);
	DDX_Check(pDX, IDC_OPTIONS_CODESDISP, m_ShowOnlyUsedCodes);
	DDX_Check(pDX, IDC_OPTIONS_AUTOBACKUP, m_AutoBackup);
	DDX_Check(pDX, IDC_AUTO_CURRENT_DATE, m_bAutoDateToday);
	DDX_Check(pDX, IDC_SAMPLER_VORGABE, m_bSamplerVorgabe);
	DDX_Text(pDX, IDC_SAMPLER_TEXT, m_SamplerVorgabeText);
	DDX_Check(pDX, IDC_NO_DUPLICATE_ARCHIVE_NR, m_bNoDupArchiveNr);
	DDX_Text(pDX, IDC_AUTOBACKUP_DIRECTORY, m_sAutoBackupDirectory);
	//}}AFX_DATA_MAP
	DDX_Control(pDX, IDC_AUTOBACKUP_DIRECTORY, m_edtBackupDirectory);
	DDX_Control(pDX, IDC_CHOOSE_DIRECTORY, m_btnChooseBackupDirectory);
	DDX_Control(pDX, IDC_PREFERED_SPELLING, m_cboPreferedSpelling);
	DDX_Control(pDX, IDC_ARTIST_DISPLAY, m_cboArtistDisplay);
	DDX_Check(pDX, IDC_AUTOCOMPLETE_ARTIST, m_bAutoCompleteArtist);
	DDX_Check(pDX, IDC_AUTOCOMPLETE_CDTITLE, m_bAutoCompleteCDTitle);
	DDX_Check(pDX, IDC_AUTOCOMPLETE_TRACKNAME, m_bAutoCompleteTrackname);
	DDX_Check(pDX, IDC_AUTOSAVE_CDS, m_bAutoSaveCDs);
	DDX_Check(pDX, IDC_ARCHIVENUMBER_NUMERIC, m_bArchiveNumberNumeric);
	DDX_Check(pDX, IDC_FREEARCHIVENUMBERSEARCH_WITH_GAPS, m_bFreeArchiveNumberSearchWithGaps);
}


BEGIN_MESSAGE_MAP(COptDatabase, CPropertyPage)
	//{{AFX_MSG_MAP(COptDatabase)
	ON_BN_CLICKED(IDC_SAMPLER_VORGABE, OnOptionsSamplerVorgabe)
	ON_BN_CLICKED(IDC_OPTIONS_AUTOBACKUP, OnAutobackup)
	//}}AFX_MSG_MAP
	ON_BN_CLICKED(IDC_CHOOSE_DIRECTORY, OnBnClickedChooseDirectory)
END_MESSAGE_MAP()


/////////////////////////////////////////////////////////////////////////////
// COptDatabase message handlers

BOOL COptDatabase::OnInitDialog() 
{
    CString str;
    
//    m_AutoNewOpen = Settings::Current->AutoNewOpen;
    m_AutoBackup = Settings::Current->AutoBackup;
    m_ShowOnlyUsedCodes = Settings::Current->ShowOnlyUsedCodes;
//    m_bAutoSearch = Settings::Current->m_bAutoSearch;
    m_bAutoDateToday = Settings::Current->AutoDateToday;
    m_bAutoCompleteArtist = Settings::Current->AutoCompleteArtist;
    m_bAutoCompleteCDTitle = Settings::Current->AutoCompleteCDTitle;
    m_bAutoCompleteTrackname = Settings::Current->AutoCompleteTrackname;
    
	m_bFreeArchiveNumberSearchWithGaps = Settings::Current->FreeArchiveNumberSearchWithGaps;


    CPropertyPage::OnInitDialog();
    
    m_SamplerVorgabeTextCtrl.EnableWindow(m_bSamplerVorgabe);
    m_SamplerVorgabeTextStaticCtrl.EnableWindow(m_bSamplerVorgabe);

	m_edtBackupDirectory.EnableWindow(m_AutoBackup);	
    m_btnChooseBackupDirectory.EnableWindow(m_AutoBackup);

	// Bevorzugte Schreibweise
	m_cboPreferedSpelling.AddString(get_string(TEXT_NONE));
	m_cboPreferedSpelling.AddString(get_string(IDS_FIRST_CHAR_UPPER));
	m_cboPreferedSpelling.AddString(get_string(IDS_WORD_UPPER));

	m_cboPreferedSpelling.SetCurSel(Settings::Current->AdjustSpelling);

	// Interpreten-Anzeige
	m_cboArtistDisplay.AddString(get_string(TEXT_NAME));
	m_cboArtistDisplay.AddString(get_string(TEXT_SORTKEY));
	m_cboArtistDisplay.SetCurSel(Settings::Current->ArtistDisplay);

    return TRUE;  // return TRUE unless you set the focus to a control
    // EXCEPTION: OCX Property Pages should return FALSE
}

void COptDatabase::OnOptionsSamplerVorgabe() 
{
	UpdateData(TRUE);

    m_SamplerVorgabeTextCtrl.EnableWindow(m_bSamplerVorgabe);
    m_SamplerVorgabeTextStaticCtrl.EnableWindow(m_bSamplerVorgabe);
}

void COptDatabase::OnBnClickedChooseDirectory()
{
	UpdateData(TRUE);

	CString sAutoBackupDirectory;
	sAutoBackupDirectory = CMisc::BrowseForDirectory(this, get_string(TEXT_CHOOSE_DIRECTORY), m_sAutoBackupDirectory);

	if (!sAutoBackupDirectory.IsEmpty())
		m_sAutoBackupDirectory = sAutoBackupDirectory;

	UpdateData(FALSE);
}

void COptDatabase::OnAutobackup() 
{
	UpdateData(TRUE);

	m_edtBackupDirectory.EnableWindow(m_AutoBackup);	
    m_btnChooseBackupDirectory.EnableWindow(m_AutoBackup);
}

void COptDatabase::OnOK() 
{
    CPropertyPage::OnOK();
    
//    Settings::Current->AutoNewOpen = m_AutoNewOpen;
    Settings::Current->AutoBackup = m_AutoBackup;
    Settings::Current->ShowOnlyUsedCodes = m_ShowOnlyUsedCodes;
//    Settings::Current->m_bAutoSearch = m_bAutoSearch;
    Settings::Current->AutoSaveCDs = m_bAutoSaveCDs;
    Settings::Current->AutoDateToday = m_bAutoDateToday;
	Settings::Current->SamplerUseFixedArtist = m_bSamplerVorgabe ? true : false;
	Settings::Current->SamplerFixedArtistText = gcnew String(m_SamplerVorgabeText);
	Settings::Current->NoDuplicateArchiveNumbers = m_bNoDupArchiveNr;
	Settings::Current->AutoBackupDirectory = gcnew String(m_sAutoBackupDirectory);
	Settings::Current->FreeArchiveNumberSearchWithGaps = m_bFreeArchiveNumberSearchWithGaps;

	Settings::Current->AdjustSpelling = m_cboPreferedSpelling.GetCurSel();

	Settings::Current->ArtistDisplay = m_cboArtistDisplay.GetCurSel();

	Settings::Current->AutoCompleteArtist = m_bAutoCompleteArtist;
    Settings::Current->AutoCompleteCDTitle = m_bAutoCompleteCDTitle;
    Settings::Current->AutoCompleteTrackname = m_bAutoCompleteTrackname;

	Settings::Current->SortArchiveNumberNumeric = m_bArchiveNumberNumeric ? true : false;
}

/////////////////////////////////////////////////////////////////////////////
// COptionsSheet

IMPLEMENT_DYNAMIC(COptionsSheet, CPropertySheet)

COptionsSheet::COptionsSheet(UINT nIDCaption, CWnd* pParentWnd, UINT iSelectPage)
	:CPropertySheet(nIDCaption, pParentWnd, iSelectPage)
{
	m_psh.dwFlags |= PSH_NOAPPLYNOW;
	//AddPage(&m_COptCDPlayer);
	AddPage(&m_COptDatabase);
	AddPage(&m_OptionsCDArchivPage);
	AddPage(&m_COptMisc);
	//AddPage(&m_COptColors);
	//AddPage(&m_COptSymbol);
	//AddPage(&m_OptionsPlugInsPage);
	AddPage(&m_OptionsSoundsPage);
	AddPage(&m_OptionsPlayPage);
}

COptionsSheet::COptionsSheet(LPCTSTR pszCaption, CWnd* pParentWnd, UINT iSelectPage)
	:CPropertySheet(pszCaption, pParentWnd, iSelectPage)
{
}

COptionsSheet::~COptionsSheet()
{
}


BEGIN_MESSAGE_MAP(COptionsSheet, CPropertySheet)
	//{{AFX_MSG_MAP(COptionsSheet)
	ON_WM_CREATE()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()


/////////////////////////////////////////////////////////////////////////////
// COptionsSheet message handlers

int COptionsSheet::OnCreate(LPCREATESTRUCT lpCreateStruct) 
{
	if (CPropertySheet::OnCreate(lpCreateStruct) == -1)
		return -1;
	
	//m_saveconfig = theApp.m_config;
	
	return 0;
}

int COptionsSheet::DoModal() 
{
	int ret;
	
	ret = CPropertySheet::DoModal();

   //if (ret == IDCANCEL)
     // theApp.m_config = m_saveconfig;

   return ret;
}

/////////////////////////////////////////////////////////////////////////////
// COptMisc property page

IMPLEMENT_DYNCREATE(COptMisc, CPropertyPage)

COptMisc::COptMisc() : CPropertyPage(COptMisc::IDD)
, m_bUseMCI(FALSE)
, m_bDisableCDText(FALSE)
, m_bDisableScreenSaver(FALSE)
{
	m_bAutoCheckNewVersion = Settings::Current->AutoCheckNewVersion;
	m_bAutoCheckAnnouncement = Settings::Current->AutoCheckAnnouncement;
	m_sDefaultCDCoverPath = Settings::Current->DefaultCDCoverPath;
	m_ShowWelcomeScreen = Settings::Current->ShowSplashScreen;
	m_bUseMCI = Settings::Current->UseMCI;
	m_bDisableCDText = Settings::Current->DisableCDText;
	m_bDisableScreenSaver = Settings::Current->DisableScreenSaver;
}

COptMisc::~COptMisc()
{
}

void COptMisc::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(COptMisc)
	DDX_Check(pDX, IDC_WELCOMESCREEN, m_ShowWelcomeScreen);
	DDX_Check(pDX, IDC_CHECKVERSION, m_bAutoCheckNewVersion);
	DDX_Check(pDX, IDC_CHECKANNOUNCEMENT, m_bAutoCheckAnnouncement);
	DDX_Text(pDX, IDC_DEFAULT_CDCOVER_PATH, m_sDefaultCDCoverPath);
	//}}AFX_DATA_MAP
	DDX_Check(pDX, IDC_USEMCI, m_bUseMCI);
	DDX_Check(pDX, IDC_DISABLECDTEXT, m_bDisableCDText);
	DDX_Check(pDX, IDC_DISABLESCREENSAVER, m_bDisableScreenSaver);
	DDX_Control(pDX, IDC_TIMEFORMAT, m_cboTimeFormat);
}


BEGIN_MESSAGE_MAP(COptMisc, CPropertyPage)
	//{{AFX_MSG_MAP(COptMisc)
	ON_BN_CLICKED(IDC_CDCOVER_BROWSE, OnCdcoverBrowse)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()


/////////////////////////////////////////////////////////////////////////////
// COptMisc message handlers

BOOL COptMisc::OnInitDialog() 
{
	CPropertyPage::OnInitDialog();

	m_cboTimeFormat.AddString(get_string(TEXT_TIMEFORMAT_HHMMSS));
	m_cboTimeFormat.AddString(get_string(TEXT_TIMEFORMAT_MMSS));
	m_cboTimeFormat.SetCurSel((int)Settings::Current->TimeFormat);

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void COptMisc::OnCdcoverBrowse() 
{
	UpdateData(TRUE);

	CString sDirectory;
	sDirectory = CMisc::BrowseForDirectory(this, get_string(TEXT_CHOOSE_CDCOVER_DIRECTORY), m_sDefaultCDCoverPath);
	if (!sDirectory.IsEmpty())
		m_sDefaultCDCoverPath = sDirectory;

	UpdateData(FALSE);
}

void COptMisc::OnOK() 
{
	UpdateData(TRUE);
	
	Settings::Current->ShowSplashScreen = m_ShowWelcomeScreen;
	Settings::Current->AutoCheckNewVersion = m_bAutoCheckNewVersion;
	Settings::Current->AutoCheckAnnouncement = m_bAutoCheckAnnouncement;
	Settings::Current->DefaultCDCoverPath = gcnew String(m_sDefaultCDCoverPath);
	Settings::Current->UseMCI = m_bUseMCI;
	Settings::Current->DisableCDText = m_bDisableCDText;
	Settings::Current->DisableScreenSaver = m_bDisableScreenSaver;
	Settings::Current->TimeFormat = (TimeFormat)m_cboTimeFormat.GetCurSel();

	CPropertyPage::OnOK();
}

/////////////////////////////////////////////////////////////////////////////
// COptSymbol property page

IMPLEMENT_DYNCREATE(COptSymbol, CPropertyPage)

COptSymbol::COptSymbol() : CPropertyPage(COptSymbol::IDD)
{
	//{{AFX_DATA_INIT(COptSymbol)
	m_ShowInterpret = FALSE;
	m_ShowTitle = FALSE;
	m_ShowTrackNr = FALSE;
	m_ShowTrackName = FALSE;
	m_ShowTrackTime = FALSE;
	m_ShowTotalTime = FALSE;
	m_SymbolInTaskbar = FALSE;
	m_bShowTrackArtist = FALSE;
	//}}AFX_DATA_INIT
}

COptSymbol::~COptSymbol()
{
}

void COptSymbol::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(COptSymbol)
	DDX_Check(pDX, ID_OPTIONS_VIEWARTIST, m_ShowInterpret);
	DDX_Check(pDX, ID_OPTIONS_VIEWTITLE, m_ShowTitle);
	DDX_Check(pDX, ID_OPTIONS_VIEWTRACK, m_ShowTrackNr);
	DDX_Check(pDX, ID_OPTIONS_VIEWTRACKNAME, m_ShowTrackName);
	DDX_Check(pDX, ID_OPTIONS_VIEWTIME, m_ShowTrackTime);
	DDX_Check(pDX, ID_OPTIONS_VIEWTIME2, m_ShowTotalTime);
	DDX_Check(pDX, IDC_SYMBOL_IN_TASKBAR, m_SymbolInTaskbar);
	DDX_Check(pDX, ID_OPTIONS_VIEWTRACKARTIST, m_bShowTrackArtist);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(COptSymbol, CPropertyPage)
	//{{AFX_MSG_MAP(COptSymbol)
	ON_WM_DESTROY()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()


/////////////////////////////////////////////////////////////////////////////
// COptSymbol message handlers

BOOL COptSymbol::OnInitDialog() 
{
   m_ShowInterpret = (Settings::Current->ShowSymbol & SYMBOL_INTERPRET) ? TRUE : FALSE;
   m_ShowTitle     = (Settings::Current->ShowSymbol & SYMBOL_TITLE) ? TRUE : FALSE;
   m_ShowTrackNr   = (Settings::Current->ShowSymbol & SYMBOL_TRACKNR) ? TRUE : FALSE;
   m_bShowTrackArtist = (Settings::Current->ShowSymbol & SYMBOL_TRACKARTIST) ? TRUE : FALSE;
   m_ShowTrackName = (Settings::Current->ShowSymbol & SYMBOL_TRACKNAME) ? TRUE : FALSE;
   m_ShowTrackTime = (Settings::Current->ShowSymbol & SYMBOL_TRACKTIME) ? TRUE : FALSE;
   m_ShowTotalTime = (Settings::Current->ShowSymbol & SYMBOL_TOTALTIME) ? TRUE : FALSE;
   m_SymbolInTaskbar = Settings::Current->SymbolInTaskbar;

	CPropertyPage::OnInitDialog();

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void COptSymbol::OnDestroy() 
{
	CPropertyPage::OnDestroy();

   Settings::Current->SymbolInTaskbar = m_SymbolInTaskbar;
   Settings::Current->ShowSymbol = 0;

   if (m_ShowInterpret) Settings::Current->ShowSymbol |= SYMBOL_INTERPRET;
   if (m_ShowTitle)     Settings::Current->ShowSymbol |= SYMBOL_TITLE;
   if (m_ShowTrackNr)   Settings::Current->ShowSymbol |= SYMBOL_TRACKNR;
   if (m_bShowTrackArtist) Settings::Current->ShowSymbol |= SYMBOL_TRACKARTIST;
   if (m_ShowTrackName) Settings::Current->ShowSymbol |= SYMBOL_TRACKNAME;
   if (m_ShowTrackTime) Settings::Current->ShowSymbol |= SYMBOL_TRACKTIME;
   if (m_ShowTotalTime) Settings::Current->ShowSymbol |= SYMBOL_TOTALTIME;
}


/////////////////////////////////////////////////////////////////////////////
// COptionsCDArchivPage property page

IMPLEMENT_DYNCREATE(COptionsCDArchivPage, CPropertyPage)

COptionsCDArchivPage::COptionsCDArchivPage() : CPropertyPage(COptionsCDArchivPage::IDD)
, m_bAutoInsertQueue(FALSE)
{
	//{{AFX_DATA_INIT(COptionsCDArchivPage)
	m_bDontSearchDataCDs = Settings::Current->DontSearchDataCDsInCDArchive;
	//}}AFX_DATA_INIT

	m_bAutoInsertQueue = Settings::Current->AutoInsertQueue;
	m_bSearchLyrics = Big3::Hitbase::Configuration::Settings::Current->AutoSearchLyrics;
	m_bSearchCDCover = Big3::Hitbase::Configuration::Settings::Current->AutoSearchCover;

	m_FontWing.CreateFont(16, 0, 0, 0, 0, 0, 0, 0, DEFAULT_CHARSET, 0, 0, 0, 0, L"Wingdings");
}

COptionsCDArchivPage::~COptionsCDArchivPage()
{
}

void COptionsCDArchivPage::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(COptionsCDArchivPage)
	DDX_Control(pDX, IDC_DOWN, m_DownCtrl);
	DDX_Control(pDX, IDC_UP, m_UpCtrl);
	DDX_Control(pDX, IDC_INFO, m_InfoCtrl);
	DDX_Control(pDX, IDC_EDIT, m_EditCtrl);
	DDX_Control(pDX, IDC_DELETE, m_DeleteCtrl);
	DDX_Control(pDX, IDC_CD_ARCHIVES, m_ArchivListCtrl);
	DDX_Check(pDX, IDC_DONT_SEARCH_DATA_CDS, m_bDontSearchDataCDs);
	//}}AFX_DATA_MAP
	DDX_Check(pDX, IDC_AUTO_INSERT_QUEUE, m_bAutoInsertQueue);
	DDX_Check(pDX, IDC_SEARCH_CDCOVER, m_bSearchCDCover);
	DDX_Check(pDX, IDC_SEARCH_LYRICS, m_bSearchLyrics);
}


BEGIN_MESSAGE_MAP(COptionsCDArchivPage, CPropertyPage)
	//{{AFX_MSG_MAP(COptionsCDArchivPage)
	ON_BN_CLICKED(IDC_FIELDS, OnFields)
	ON_BN_CLICKED(IDC_ADD, OnAdd)
	ON_BN_CLICKED(IDC_EDIT, OnEdit)
	ON_BN_CLICKED(IDC_DELETE, OnDelete)
	ON_BN_CLICKED(IDC_INFO, OnInfo)
	ON_NOTIFY(LVN_ITEMCHANGED, IDC_CD_ARCHIVES, OnItemchangedCdArchives)
	ON_NOTIFY(NM_DBLCLK, IDC_CD_ARCHIVES, OnDblclkCdArchives)
	ON_BN_CLICKED(IDC_UP, OnUp)
	ON_BN_CLICKED(IDC_DOWN, OnDown)
	ON_BN_CLICKED(IDC_PROXY_SETTINGS, OnProxySettings)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// COptionsCDArchivPage message handlers

BOOL COptionsCDArchivPage::OnInitDialog() 
{
	CPropertyPage::OnInitDialog();

    m_DownCtrl.SetFont(&m_FontWing);
    m_UpCtrl.SetFont(&m_FontWing);

    m_ArchivListCtrl.InsertColumn(0, get_string(TEXT_TYPE), LVCFMT_LEFT, 90);
	m_ArchivListCtrl.InsertColumn(1, get_string(TEXT_SOURCE), LVCFMT_LEFT, 142);
	m_ArchivListCtrl.InsertColumn(2, get_string(TEXT_ACTIVE), LVCFMT_LEFT, 37);
	m_ArchivListCtrl.InsertColumn(3, get_string(TEXT_AUTO), LVCFMT_LEFT, 35);

	FillArchiveList();

	UpdateWindowState();
	
	return TRUE;
}

void COptionsCDArchivPage::OnOK() 
{
	CPropertyPage::OnOK();

	Settings::Current->DontSearchDataCDsInCDArchive = m_bDontSearchDataCDs;
	Settings::Current->AutoInsertQueue = m_bAutoInsertQueue;
	Big3::Hitbase::Configuration::Settings::Current->AutoSearchCover = m_bSearchCDCover;
	Big3::Hitbase::Configuration::Settings::Current->AutoSearchLyrics = m_bSearchLyrics;

//	theApp.m_CDArchives->WriteConfig();

}

void COptionsCDArchivPage::OnFields() 
{
	COptionsCDArchiveFieldsDlg OptionsCDArchiveFieldsDlg;

	OptionsCDArchiveFieldsDlg.m_bCategory = ((Settings::Current->CDArchiveFields & CDArchiveFields::Category) == CDArchiveFields::Category) ? TRUE : FALSE;
	OptionsCDArchiveFieldsDlg.m_bMedium = ((Settings::Current->CDArchiveFields & CDArchiveFields::Medium) == CDArchiveFields::Medium) ? TRUE : FALSE;
	OptionsCDArchiveFieldsDlg.m_bCDComment = ((Settings::Current->CDArchiveFields & CDArchiveFields::Comment) == CDArchiveFields::Comment) ? TRUE : FALSE;
	OptionsCDArchiveFieldsDlg.m_bBPM = ((Settings::Current->CDArchiveFields & CDArchiveFields::BPM) == CDArchiveFields::BPM) ? TRUE : FALSE;
	OptionsCDArchiveFieldsDlg.m_bTrackComment = ((Settings::Current->CDArchiveFields & CDArchiveFields::TrackComment) == CDArchiveFields::TrackComment) ? TRUE : FALSE;
	OptionsCDArchiveFieldsDlg.m_bLyrics = ((Settings::Current->CDArchiveFields & CDArchiveFields::Lyrics) == CDArchiveFields::Lyrics) ? TRUE : FALSE;

	if (OptionsCDArchiveFieldsDlg.DoModal() == IDOK)
	{
		Settings::Current->CDArchiveFields = CDArchiveFields::None;

		if (OptionsCDArchiveFieldsDlg.m_bCategory)
			Settings::Current->CDArchiveFields |= CDArchiveFields::Category;
		if (OptionsCDArchiveFieldsDlg.m_bMedium)
			Settings::Current->CDArchiveFields |= CDArchiveFields::Medium;
		if (OptionsCDArchiveFieldsDlg.m_bCDComment)
			Settings::Current->CDArchiveFields |= CDArchiveFields::Comment;
		if (OptionsCDArchiveFieldsDlg.m_bBPM)
			Settings::Current->CDArchiveFields |= CDArchiveFields::BPM;
		if (OptionsCDArchiveFieldsDlg.m_bTrackComment)
			Settings::Current->CDArchiveFields |= CDArchiveFields::TrackComment;
		if (OptionsCDArchiveFieldsDlg.m_bLyrics)
			Settings::Current->CDArchiveFields |= CDArchiveFields::Lyrics;
	}
}

void COptionsCDArchivPage::UpdateWindowState()
{
	int sel = m_ArchivListCtrl.GetCurSel();

	m_EditCtrl.EnableWindow(sel >= 0);
	m_DeleteCtrl.EnableWindow(sel >= 0);
	m_InfoCtrl.EnableWindow(sel >= 0);
	m_UpCtrl.EnableWindow(sel > 0);
	m_DownCtrl.EnableWindow(sel >= 0 && sel < m_ArchivListCtrl.GetItemCount()-1);
}

void COptionsCDArchivPage::OnAdd() 
{
	CCDArchiveNewDlg CDArchiveNewDlg(FALSE);

	if (CDArchiveNewDlg.DoModal() == IDOK)
	{
		CDArchiveType nTypes;
		switch (CDArchiveNewDlg.m_nType)
		{
		case 0: nTypes = CDArchiveType::File; break;
		case 1: nTypes = CDArchiveType::BIG3; break;
		case 2: nTypes = CDArchiveType::CDDBsockets; break;
		case 3: nTypes = CDArchiveType::CDDBhttp; break;
		case 4: nTypes = CDArchiveType::CDArchiveLocal; break;
		case 5: nTypes = CDArchiveType::CDArchiveLocalCDDB; break;
		default: ASSERT(FALSE);
		}

		if (!CheckCDArchiveFormat(nTypes, gcnew String(CDArchiveNewDlg.m_sSource)))
		{
			AfxMessageBox(get_string(IDS_INVALID_CDARCHIVE), MB_OK|MB_ICONINFORMATION);
			return;
		}

		CDArchiveConfig^ cdArchive =  gcnew CDArchiveConfig(nTypes, gcnew String(CDArchiveNewDlg.m_sSource), TRUE, CDArchiveNewDlg.m_bAutoSearch, CDArchiveNewDlg.m_bUpload, CDArchiveNewDlg.m_bAutoCreateSampler, gcnew String(CDArchiveNewDlg.m_sSamplerTrennzeichen));
		Settings::Current->CDArchives->Add(cdArchive);
		FillArchiveList();
	}
}

void COptionsCDArchivPage::OnEdit() 
{
	int sel = m_ArchivListCtrl.GetCurSel();
	
	ASSERT(sel >= 0);
	if (sel < 0)
		return;

	CCDArchiveNewDlg CDArchiveNewDlg(TRUE);
	CDArchiveNewDlg.m_sSource = (CString)Settings::Current->CDArchives[sel]->ArchiveName;
	CDArchiveNewDlg.m_bActive = Settings::Current->CDArchives[sel]->Active;
	CDArchiveNewDlg.m_bAutoSearch = Settings::Current->CDArchives[sel]->AutoSearch;
	CDArchiveNewDlg.m_bUpload = Settings::Current->CDArchives[sel]->Upload;
	CDArchiveNewDlg.m_bAutoCreateSampler = Settings::Current->CDArchives[sel]->AutoCreateSampler;
	CDArchiveNewDlg.m_sSamplerTrennzeichen = (CString)Settings::Current->CDArchives[sel]->SamplerTrennzeichen;

	switch (Settings::Current->CDArchives[sel]->Type)
	{
	case CDArchiveType::File: CDArchiveNewDlg.m_nType = 0; break;
	case CDArchiveType::BIG3: CDArchiveNewDlg.m_nType = 1; break;
	case CDArchiveType::CDDBsockets: CDArchiveNewDlg.m_nType = 2; break;
	case CDArchiveType::CDDBhttp: CDArchiveNewDlg.m_nType = 3; break;
	case CDArchiveType::CDArchiveLocal: CDArchiveNewDlg.m_nType = 4; break;
	case CDArchiveType::CDArchiveLocalCDDB: CDArchiveNewDlg.m_nType = 5; break;
	default: ASSERT(FALSE);
	}

	if (CDArchiveNewDlg.DoModal() == IDOK)
	{
		CDArchiveType nTypes;
		switch (CDArchiveNewDlg.m_nType)
		{
		case 0: nTypes = CDArchiveType::File; break;
		case 1: nTypes = CDArchiveType::BIG3; break;
		case 2: nTypes = CDArchiveType::CDDBsockets; break;
		case 3: nTypes = CDArchiveType::CDDBhttp; break;
		case 4: nTypes = CDArchiveType::CDArchiveLocal; break;
		case 5: nTypes = CDArchiveType::CDArchiveLocalCDDB; break;
		default: ASSERT(FALSE);
		}

		if (!CheckCDArchiveFormat(nTypes, gcnew String(CDArchiveNewDlg.m_sSource)))
		{
			AfxMessageBox(get_string(IDS_INVALID_CDARCHIVE), MB_OK|MB_ICONINFORMATION);
			return;
		}

		Settings::Current->CDArchives[sel] = gcnew CDArchiveConfig(nTypes, gcnew String(CDArchiveNewDlg.m_sSource), CDArchiveNewDlg.m_bActive, CDArchiveNewDlg.m_bAutoSearch, CDArchiveNewDlg.m_bUpload, CDArchiveNewDlg.m_bAutoCreateSampler, gcnew String(CDArchiveNewDlg.m_sSamplerTrennzeichen));
		FillArchiveList();

		// JUS 981111: Das fehlte hier! Nach dem Editieren war kein Element markiert.
		UpdateWindowState();
	}
}

BOOL COptionsCDArchivPage::CheckCDArchiveFormat(CDArchiveType nType, String^ sSource)
{
	// Prüfen, ob die ausgewählte Datei den richtigen Typ hat.
/*TODO!!!!!!!!!!!!!!!!!!!!!!!!!!!	if (nType == CConfigArchive::enumArchiveTypes::archiveCDArchiveLocal)
	{
		// Wenn die Datenbank eine "Master"-Tabelle hat, ist es eine normale Hitbase-Datenbank
		CDaoDatabase db;
		db.Open(CString(sSource), TRUE);
		try
		{
			CDaoTableDef def(&db);
			def.Open(L"Master");
			def.Close();
			return FALSE;
			// Master-Tabelle darf nicht vorhanden sein!
		}
		catch (CException* e)
		{
			e->Delete();
		}
		db.Close();
		return TRUE;
	}

	// Prüfen, ob die ausgewählte Datei den richtigen Typ hat.
	if (nType == CConfigArchive::enumArchiveTypes::archiveFile)
	{
		try
		{
			Big3::Hitbase::DataBaseEngine::DataBase^ db = gcnew Big3::Hitbase::DataBaseEngine::DataBase();
			db->Open(sSource);
			db->Close();
		}
		catch (CException* e)
		{
			e->Delete();
			return FALSE;
		}

		return TRUE;
	}*/

	return TRUE;
}

void COptionsCDArchivPage::OnDelete() 
{
	int sel = m_ArchivListCtrl.GetCurSel();
	
	ASSERT(sel >= 0);

	if (MessageResBox(TEXT_DELETE_CDARCHIV, MB_YESNO|MB_ICONQUESTION, (CString)Settings::Current->CDArchives[sel]->ArchiveName) == IDYES)
	{
		Settings::Current->CDArchives->RemoveAt(sel);

		FillArchiveList();

		UpdateWindowState();
	}
}

void COptionsCDArchivPage::OnInfo() 
{
	String^ sDate;
	int nNumberOfCDs;
	int sel = m_ArchivListCtrl.GetCurSel();
	
	ASSERT(sel >= 0);
	CCDArchive^ cdarchive = gcnew CCDArchive();
	if (cdarchive->GetStatistics(sel, nNumberOfCDs, sDate))
	{
		MessageResBox(TEXT_SERVER_INFO, MB_OK|MB_ICONINFORMATION, (CString)Settings::Current->CDArchives[sel]->ArchiveName, nNumberOfCDs, (CString)sDate);
	}
	else
	{
		ErrorResBox(this, TEXT_SERVER_NOT_FOUND, (CString)Settings::Current->CDArchives[sel]->ArchiveName);
	}
}

void COptionsCDArchivPage::FillArchiveList()
{
	CString str;

	m_ArchivListCtrl.DeleteAllItems();

	for (int i=0;i<Settings::Current->CDArchives->Count;i++)
	{
		switch (Settings::Current->CDArchives[i]->Type)
		{
		case CDArchiveType::File:
			str = get_string(TEXT_HITBASE_FILE);
			break;
		case CDArchiveType::BIG3:
			str = get_string(TEXT_HITBASE_INTERNET);
			break;
		case CDArchiveType::CDDBsockets:
			str = get_string(TEXT_CDDB) + L" (Sockets)";
			break;
		case CDArchiveType::CDDBhttp:
			str = get_string(TEXT_CDDB) + L" (Http)";
			break;
		case CDArchiveType::CDArchiveLocal:
			str = get_string(IDS_CDARCHIVE_LOCAL);
			break;
		case CDArchiveType::CDArchiveLocalCDDB:
			str = get_string(IDS_CDARCHIVE_LOCAL_CDDB);
			break;
		}

		m_ArchivListCtrl.InsertItem(i, str);
		m_ArchivListCtrl.SetItem(i, 1, LVIF_TEXT, (CString)Settings::Current->CDArchives[i]->ArchiveName, 0, 0, 0, 0);

		if (Settings::Current->CDArchives[i]->Active)
			str = get_string(TEXT_YES);
		else
			str = get_string(TEXT_NO);
		m_ArchivListCtrl.SetItem(i, 2, LVIF_TEXT, str, 0, 0, 0, 0);

		if (Settings::Current->CDArchives[i]->AutoSearch)
			str = get_string(TEXT_YES);
		else
			str = get_string(TEXT_NO);
		m_ArchivListCtrl.SetItem(i, 3, LVIF_TEXT, str, 0, 0, 0, 0);
	}
}

void COptionsCDArchivPage::OnItemchangedCdArchives(NMHDR* pNMHDR, LRESULT* pResult) 
{
	NM_LISTVIEW* pNMListView = (NM_LISTVIEW*)pNMHDR;

	UpdateWindowState();

	*pResult = 0;
}

void COptionsCDArchivPage::OnDblclkCdArchives(NMHDR* pNMHDR, LRESULT* pResult) 
{
	OnEdit();
	
	*pResult = 0;
}

void COptionsCDArchivPage::OnUp() 
{
	int sel = m_ArchivListCtrl.GetCurSel();
	
	ASSERT(sel > 0);
	
	CDArchiveConfig^ save = Settings::Current->CDArchives[sel-1];
	
	Settings::Current->CDArchives[sel-1] = Settings::Current->CDArchives[sel];
	Settings::Current->CDArchives[sel] = save;
	
	FillArchiveList();
	
	m_ArchivListCtrl.SetItemState(sel-1, LVIS_SELECTED, LVIS_SELECTED);
	
	UpdateWindowState();
}

void COptionsCDArchivPage::OnDown() 
{
	int sel = m_ArchivListCtrl.GetCurSel();
	
	ASSERT(sel >= 0 && sel < m_ArchivListCtrl.GetItemCount()-1);
	
	CDArchiveConfig^ save = Settings::Current->CDArchives[sel+1];
	
	Settings::Current->CDArchives[sel+1] = Settings::Current->CDArchives[sel];
	Settings::Current->CDArchives[sel] = save;
	
	FillArchiveList();

	m_ArchivListCtrl.SetItemState(sel+1, LVIS_SELECTED, LVIS_SELECTED);
	
	UpdateWindowState();
}

void COptionsCDArchivPage::OnProxySettings() 
{
	COptionsCDArchiveProxyDlg OptionsCDArchiveProxyDlg;
	
	if (OptionsCDArchiveProxyDlg.DoModal() == IDOK)
	{
		Settings::Current->ProxyType = OptionsCDArchiveProxyDlg.m_nProxyType;
		Settings::Current->ProxyServerName = String::Format(L"{0}:{1}", gcnew String(OptionsCDArchiveProxyDlg.m_sProxyServerName), OptionsCDArchiveProxyDlg.m_iPort);
	}
}

/////////////////////////////////////////////////////////////////////////////
// COptionsPlayPage property page

IMPLEMENT_DYNCREATE(COptionsPlayPage, CPropertyPage)

COptionsPlayPage::COptionsPlayPage() : CPropertyPage(COptionsPlayPage::IDD)
, m_sPassword(_T(""))
, m_bEnablePasswordProtection(FALSE)
, m_bScrobbleActive(FALSE)
, m_bAllowPlayNext(FALSE)
, m_bAllowPlayLast(FALSE)
{
	//{{AFX_DATA_INIT(CVirtualCDTrackOptionsDlg)
	//}}AFX_DATA_INIT
}

void COptionsPlayPage::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPage::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_ENABLEPASSWORDPROTECTION, m_chkEnablePasswordProtection);
	DDX_Control(pDX, IDC_PASSWORD, m_edtPassword);
	DDX_Text(pDX, IDC_PASSWORD, m_sPassword);
	DDX_Check(pDX, IDC_ENABLEPASSWORDPROTECTION, m_bEnablePasswordProtection);
	DDX_Control(pDX, IDC_ACTIVATE_SCROBBLE, m_cboScrobbleActive);
	DDX_Control(pDX, IDC_BUTTON_AUTH_SCROBBLE, m_btnAuthScrobble);
	DDX_Check(pDX, IDC_ACTIVATE_SCROBBLE, m_bScrobbleActive);
	DDX_Control(pDX, IDC_STATIC_AUTH_SCROBBLE, m_stcAuthScrobble);
	DDX_Control(pDX, IDC_LASTFM, m_iconLastFm);
	DDX_Check(pDX, IDC_ALLOWPLAYNEXT, m_bAllowPlayNext);
	DDX_Check(pDX, IDC_ALLOWPLAYLAST, m_bAllowPlayLast);
	DDX_Control(pDX, IDC_ALLOWPLAYNEXT, m_chkAllowPlayNext);
	DDX_Control(pDX, IDC_ALLOWPLAYLAST, m_chkAllowPlayLast);
}


BEGIN_MESSAGE_MAP(COptionsPlayPage, CPropertyPage)
	//{{AFX_MSG_MAP(COptCDPlayer)
	//}}AFX_MSG_MAP
	ON_BN_CLICKED(IDC_ENABLEPASSWORDPROTECTION, &COptionsPlayPage::OnBnClickedEnablepasswordprotection)
	ON_BN_CLICKED(IDC_BUTTON_AUTH_SCROBBLE, &COptionsPlayPage::OnBnClickedButtonAuthScrobble)
	ON_BN_CLICKED(IDC_ACTIVATE_SCROBBLE, &COptionsPlayPage::OnBnClickedActivateScrobble)
	ON_STN_CLICKED(IDC_LASTFM, &COptionsPlayPage::OnStnClickedLastfm)
	ON_WM_SETCURSOR()
END_MESSAGE_MAP()


/////////////////////////////////////////////////////////////////////////////
// COptionsPlayPage message handlers

BOOL COptionsPlayPage::OnInitDialog() 
{
	m_bEnablePasswordProtection = Big3::Hitbase::Configuration::Settings::Current->PartyModusEnablePassword;
	m_sPassword = Big3::Hitbase::Configuration::Settings::Current->PartyModusPassword;
	m_bScrobbleActive = Big3::Hitbase::Configuration::Settings::Current->ScrobbleActive;

	m_bAllowPlayNext = Big3::Hitbase::Configuration::Settings::Current->PartyModeAllowPlayNext;
	m_bAllowPlayLast = Big3::Hitbase::Configuration::Settings::Current->PartyModeAllowPlayLast;

	CPropertyPage::OnInitDialog();
	
	m_iconLastFm.SetCursor(::LoadCursor(NULL, IDC_WAIT));

	UpdateWindowState();

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void COptionsPlayPage::OnOK() 
{
	CPropertyPage::OnOK();

	Big3::Hitbase::Configuration::Settings::Current->ScrobbleActive = m_bScrobbleActive;
	Big3::Hitbase::Configuration::Settings::Current->PartyModusEnablePassword = m_bEnablePasswordProtection;
	Big3::Hitbase::Configuration::Settings::Current->PartyModusPassword = gcnew String(m_sPassword);

	Big3::Hitbase::Configuration::Settings::Current->PartyModeAllowPlayNext = m_bAllowPlayNext;
	Big3::Hitbase::Configuration::Settings::Current->PartyModeAllowPlayLast = m_bAllowPlayLast;
}


void COptionsPlayPage::OnBnClickedEnablepasswordprotection()
{
	UpdateWindowState();
}

void COptionsPlayPage::UpdateWindowState()
{
    UpdateData(TRUE);

	m_edtPassword.EnableWindow(m_bEnablePasswordProtection);
	m_chkAllowPlayNext.EnableWindow(m_bEnablePasswordProtection);
	m_chkAllowPlayLast.EnableWindow(m_bEnablePasswordProtection);

	m_edtPassword.EnableWindow(m_bEnablePasswordProtection);
	m_btnAuthScrobble.EnableWindow(m_bScrobbleActive);
	m_stcAuthScrobble.EnableWindow(m_bScrobbleActive);
}



void COptionsPlayPage::OnBnClickedButtonAuthScrobble()
{
	Big3::Hitbase::SoundEngine::Last::fm::ScrobblerClass::Authorize();
}


void COptionsPlayPage::OnBnClickedActivateScrobble()
{
	UpdateWindowState();
}

void COptionsPlayPage::OnStnClickedLastfm()
{
	CMisc::OpenURL(AfxGetMainWnd(), L"www.lastfm.de");
}




