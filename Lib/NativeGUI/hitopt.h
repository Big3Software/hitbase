// hitopt.h : header file
//

#ifndef _CLASS_COPTIONS

#define _CLASS_COPTIONS

#define MAX_INTRO_TIME 120

#include "OptionsPlugInsPage.h"
#include "OptionsSoundsPage.h"

/////////////////////////////////////////////////////////////////////////////
// COptCDPlayer dialog

class COptCDPlayer : public CPropertyPage
{
	DECLARE_DYNCREATE(COptCDPlayer)

// Construction
public:
	COptCDPlayer();
	~COptCDPlayer();

// Dialog Data
	//{{AFX_DATA(COptCDPlayer)
	enum { IDD = DLG_OPTIONS_CDPLAYER };
	CEdit	m_IntroStartCtrl;
	CSpinButtonCtrl	m_IntroStartUpDown;
	CSpinButtonCtrl	m_IntroUpDown;
	int		m_Intro;
	int		m_OnNewCD;
	int		m_OnExit;
	int		m_OnStart;
	BOOL	m_AutoRepeat;
	int		m_nLapView;
	BOOL	m_bCenterCurrentTrack;
	//}}AFX_DATA


// Overrides
	// ClassWizard generate virtual function overrides
	//{{AFX_VIRTUAL(COptCDPlayer)
	public:
	virtual void OnOK();
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	// Generated message map functions
	//{{AFX_MSG(COptCDPlayer)
	virtual BOOL OnInitDialog();
	afx_msg void OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

public:
	BOOL m_bDisableCDText;
	BOOL m_bCDOutputDigital;
};
/////////////////////////////////////////////////////////////////////////////
// COptDatabase dialog

class COptDatabase : public CPropertyPage
{
	DECLARE_DYNCREATE(COptDatabase)

// Construction
public:
	COptDatabase();
	~COptDatabase();

// Dialog Data
	//{{AFX_DATA(COptDatabase)
	enum { IDD = DLG_OPTIONS_DATABASE };
	CStatic	m_SamplerVorgabeTextStaticCtrl;
	CEdit	m_SamplerVorgabeTextCtrl;
	CComboBox	m_MaxNumberOfCDsCtrl;
//	BOOL	m_AutoNewOpen;
	BOOL	m_ShowOnlyUsedCodes;
	BOOL	m_AutoBackup;
	BOOL	m_bAutoSearch;
	BOOL	m_bAutoDateToday;
	BOOL	m_bSamplerVorgabe;
	CString	m_SamplerVorgabeText;
	BOOL	m_bNoDupArchiveNr;
	CString	m_sAutoBackupDirectory;
	//}}AFX_DATA


// Overrides
	// ClassWizard generate virtual function overrides
	//{{AFX_VIRTUAL(COptDatabase)
	public:
	virtual void OnOK();
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	// Generated message map functions
	//{{AFX_MSG(COptDatabase)
	virtual BOOL OnInitDialog();
	afx_msg void OnOptionsSamplerVorgabe();
	afx_msg void OnAutobackup();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnBnClickedChooseDirectory();
	CEdit m_edtBackupDirectory;
	CButton m_btnChooseBackupDirectory;
	CComboBox m_cboPreferedSpelling;
	CComboBox m_cboArtistDisplay;
	BOOL m_bAutoCompleteArtist;
	BOOL m_bAutoCompleteCDTitle;
	BOOL m_bAutoCompleteTrackname;
	BOOL m_bAutoSaveCDs;
	BOOL m_bArchiveNumberNumeric;
	BOOL m_bFreeArchiveNumberSearchWithGaps;
};
/////////////////////////////////////////////////////////////////////////////

#include "../../lib/gridctrl/hitgrid.h"
#include "afxwin.h"

/////////////////////////////////////////////////////////////////////////////
// COptionsCDArchivPage dialog

class COptionsCDArchivPage : public CPropertyPage
{
	DECLARE_DYNCREATE(COptionsCDArchivPage)
		
		// Construction
public:
	CGridListCtrl m_ArchivListCtrl;

	void UpdateWindowState();
	COptionsCDArchivPage();
	~COptionsCDArchivPage();
	
	// Dialog Data
	//{{AFX_DATA(COptionsCDArchivPage)
	enum { IDD = DLG_OPTIONS_CDARCHIV };
	CButton	m_DownCtrl;
	CButton	m_UpCtrl;
	CButton	m_InfoCtrl;
	CButton	m_EditCtrl;
	CButton	m_DeleteCtrl;
	BOOL	m_bDontSearchDataCDs;
	//}}AFX_DATA
	
	
	// Overrides
	// ClassWizard generate virtual function overrides
	//{{AFX_VIRTUAL(COptionsCDArchivPage)
public:
	virtual void OnOK();
protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL
	
	// Implementation
protected:
	// Generated message map functions
	//{{AFX_MSG(COptionsCDArchivPage)
	afx_msg void OnLocalActive();
	afx_msg void OnInternetActive();
	virtual BOOL OnInitDialog();
	afx_msg void OnInternetServerInfo();
	afx_msg void OnChangeInternetServer();
	afx_msg void OnChangeLocalCdarchiv();
	afx_msg void OnFields();
	afx_msg void OnAdd();
	afx_msg void OnEdit();
	afx_msg void OnDelete();
	afx_msg void OnInfo();
	afx_msg void OnItemchangedCdArchives(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnDblclkCdArchives(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnUp();
	afx_msg void OnDown();
	afx_msg void OnProxySettings();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
		
private:
	void FillArchiveList();
	BOOL CheckCDArchiveFormat(Big3::Hitbase::Configuration::CDArchiveType nType, String^ sSource);

	CFont m_FontWing;
public:
	BOOL m_bAutoInsertQueue;
	BOOL m_bSearchCDCover;
	BOOL m_bSearchLyrics;
};

/////////////////////////////////////////////////////////////////////////////
// COptColors dialog

/*class COptColors : public CPropertyPage
{
// Construction
public:
	CString m_PatternPath;
	COLORREF m_rgbPlayerBackground1;
	COLORREF m_rgbPlayerBackground2;
	COptColors();   // standard constructor

// Dialog Data
	//{{AFX_DATA(COptColors)
	enum { IDD = DLG_OPTIONS_COLORS };
	CButton	m_BackgroundCtrl;
	CButton	m_LCDOffCtrl;
	CButton	m_LCDOnCtrl;
	CStatic	m_PreviewCtrl;
	int		m_nStruktur;
	int		m_nCDPlayerPos;
    CButton  m_Color1Ctrl;
    CButton  m_Color2Ctrl;
    CButton  m_PatternCtrl;
	CString	m_sSkinDirectory;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(COptColors)
	public:
	virtual void OnOK();
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(COptColors)
	afx_msg void OnBgcolor1();
	afx_msg void OnBgcolor2();
	afx_msg void OnMuster();
	afx_msg void OnGlatt();
	afx_msg void OnStruktur();
	virtual BOOL OnInitDialog();
	afx_msg void OnBackground();
	afx_msg void OnFont();
	afx_msg void OnLcdoff();
	afx_msg void OnPaint();
	afx_msg void OnBkcolor();
	afx_msg void OnChooseSkinDirectory();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
private:
	COLORREF m_rgbCurrentTrackBackground;
	COLORREF m_rgbNumbersGray;
	COLORREF m_rgbNumbers;
	COLORREF m_rgbBackground;
	void DrawPreview(CDC *pDC);
	void UpdateWindowState();
};*/
/////////////////////////////////////////////////////////////////////////////
// COptMisc dialog

class COptMisc : public CPropertyPage
{
	DECLARE_DYNCREATE(COptMisc)

// Construction
public:
	COptMisc();
	~COptMisc();

// Dialog Data
	//{{AFX_DATA(COptMisc)
	enum { IDD = DLG_OPTIONS_MISC };
	BOOL	m_ShowWelcomeScreen;
	BOOL	m_bAutoCheckNewVersion;
	BOOL	m_bAutoCheckAnnouncement;
	CString	m_sDefaultCDCoverPath;
	//}}AFX_DATA


// Overrides
	// ClassWizard generate virtual function overrides
	//{{AFX_VIRTUAL(COptMisc)
	public:
	virtual void OnOK();
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	// Generated message map functions
	//{{AFX_MSG(COptMisc)
	virtual BOOL OnInitDialog();
	afx_msg void OnCdcoverBrowse();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

public:
	BOOL m_bUseMCI;
	BOOL m_bDisableCDText;
	BOOL m_bDisableScreenSaver;
	CComboBox m_cboTimeFormat;
};
/////////////////////////////////////////////////////////////////////////////
// COptSymbol dialog

class COptSymbol : public CPropertyPage
{
	DECLARE_DYNCREATE(COptSymbol)

// Construction
public:
	COptSymbol();
	~COptSymbol();

// Dialog Data
	//{{AFX_DATA(COptSymbol)
	enum { IDD = DLG_OPTIONS_SYMBOL };
	BOOL	m_ShowInterpret;
	BOOL	m_ShowTitle;
	BOOL	m_ShowTrackNr;
	BOOL	m_ShowTrackName;
	BOOL	m_ShowTrackTime;
	BOOL	m_ShowTotalTime;
	BOOL	m_SymbolInTaskbar;
	BOOL	m_bShowTrackArtist;
	//}}AFX_DATA


// Overrides
	// ClassWizard generate virtual function overrides
	//{{AFX_VIRTUAL(COptSymbol)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	// Generated message map functions
	//{{AFX_MSG(COptSymbol)
	virtual BOOL OnInitDialog();
	afx_msg void OnDestroy();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

class COptionsPlayPage : public CPropertyPage
{
	DECLARE_DYNCREATE(COptionsPlayPage)

	// Construction
public:
	COptionsPlayPage();   // standard constructor

// Dialog Data
	//{{AFX_DATA(COptionsPlayPage)
	enum { IDD = DLG_OPTIONS_PLAY };
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(COptionsPlayPage)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	virtual void OnOK();
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(COptionsPlayPage)
	virtual BOOL OnInitDialog();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
public:
	CButton m_chkEnablePasswordProtection;
	afx_msg void OnBnClickedEnablepasswordprotection();

private:
	void UpdateWindowState();
public:
	CEdit m_edtPassword;
	CString m_sPassword;
	BOOL m_bEnablePasswordProtection;
	afx_msg void OnBnClickedButtonAuthScrobble();
	afx_msg void OnBnClickedActivateScrobble();
	CButton m_cboScrobbleActive;
	CButton m_btnAuthScrobble;
	BOOL m_bScrobbleActive;
	CStatic m_stcAuthScrobble;
	CStatic m_iconLastFm;
	afx_msg void OnStnClickedLastfm();
	BOOL m_bAllowPlayNext;
	BOOL m_bAllowPlayLast;
	CButton m_chkAllowPlayNext;
	CButton m_chkAllowPlayLast;
};

/////////////////////////////////////////////////////////////////////////////
// COptionsSheet

class COptionsSheet : public CPropertySheet
{
	DECLARE_DYNAMIC(COptionsSheet)

// Construction
public:
	COptionsSheet(UINT nIDCaption, CWnd* pParentWnd = NULL, UINT iSelectPage = 0);
	COptionsSheet(LPCTSTR pszCaption, CWnd* pParentWnd = NULL, UINT iSelectPage = 0);

// Attributes
public:
    COptCDPlayer m_COptCDPlayer;
	COptDatabase m_COptDatabase;
	COptionsCDArchivPage m_OptionsCDArchivPage;
	COptionsPlugInsPage m_OptionsPlugInsPage;
//	COptColors   m_COptColors;
	COptMisc     m_COptMisc;
	COptSymbol   m_COptSymbol;
	COptionsSoundsPage m_OptionsSoundsPage;
	COptionsPlayPage m_OptionsPlayPage;

// Operations
public:

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(COptionsSheet)
	public:
	virtual int DoModal();
	//}}AFX_VIRTUAL

// Implementation
public:
	virtual ~COptionsSheet();

	// Generated message map functions
protected:
	//{{AFX_MSG(COptionsSheet)
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

namespace Big3
{
	namespace Hitbase
	{
		namespace NativeGUI
		{
			public ref class OptionsDialog
			{
			public:
				OptionsDialog::OptionsDialog()
				{
				}

			public:
				void Show(int currentPage)
				{
					AFX_MANAGE_STATE(AfxGetStaticModuleState());
					::COptionsSheet optionsSheet(TEXT_OPTIONS, NULL, currentPage);
					optionsSheet.DoModal();
				}
			};
		}
	}
}

#endif
