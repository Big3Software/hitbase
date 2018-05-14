// HitExportTextPage1.h : header file
//

#ifndef __HITEXPORTTEXTPAGE1_H__
#define __HITEXPORTTEXTPAGE1_H__

class CHitExportTextSheet;
/////////////////////////////////////////////////////////////////////////////
// CHitExportTextPage1 dialog

class CHitExportTextPage1 : public CPropertyPage
{
	DECLARE_DYNCREATE(CHitExportTextPage1)

// Construction
public:
	CHitExportTextSheet *ExportTextSheet;

	CHitExportTextPage1();
	~CHitExportTextPage1();
/*
 * Enable/Disable Buttons for current dialog
 */

// Dialog Data
	//{{AFX_DATA(CHitExportTextPage1)
	enum { IDD = IDD_EXPPROPPAGE1 };
	CEdit	m_FileName;
	//}}AFX_DATA


// Overrides
	// ClassWizard generate virtual function overrides
	//{{AFX_VIRTUAL(CHitExportTextPage1)
	public:
	virtual BOOL OnKillActive();
	virtual BOOL OnSetActive();
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	// Generated message map functions
	//{{AFX_MSG(CHitExportTextPage1)
	afx_msg void OnSearchfile();
	virtual BOOL OnInitDialog();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

};


/////////////////////////////////////////////////////////////////////////////
// CHitExportTextPage2 dialog

class CHitExportTextPage2 : public CPropertyPage
{
	DECLARE_DYNCREATE(CHitExportTextPage2)

// Construction
public:
	CHitExportTextSheet *ExportTextSheet;

	CHitExportTextPage2();
	~CHitExportTextPage2();

// Dialog Data
	//{{AFX_DATA(CHitExportTextPage2)
	enum { IDD = IDD_EXPPROPPAGE2 };
	CButton	m_Feld1;
	CComboBox	m_TrennSatz;
	CComboBox	m_TrennFeld;
	//}}AFX_DATA


// Overrides
	// ClassWizard generate virtual function overrides
	//{{AFX_VIRTUAL(CHitExportTextPage2)
	public:
	virtual BOOL OnKillActive();
	virtual BOOL OnSetActive();
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	// Generated message map functions
	//{{AFX_MSG(CHitExportTextPage2)
	virtual BOOL OnInitDialog();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

};


/////////////////////////////////////////////////////////////////////////////
// CHitExportTextPage3 dialog

class CHitExportTextPage3 : public CPropertyPage
{
	DECLARE_DYNCREATE(CHitExportTextPage3)

// Construction
public:
	CHitExportTextSheet *ExportTextSheet;

	CHitExportTextPage3();
	~CHitExportTextPage3();
    void Update_Windows();

// Dialog Data
	//{{AFX_DATA(CHitExportTextPage3)
	enum { IDD = IDD_EXPPROPPAGE3 };
	CButton	m_Hinzufuegen;
	CButton	m_Entfernen;
	CButton	m_Auf;
	CButton	m_AlleHinzufuegen;
	CButton	m_AlleEntfernen;
	CButton	m_Ab;
	CListBox	m_AuswahlCD;
	CListBox	m_AlleFelderCD;
	//}}AFX_DATA


// Overrides
	// ClassWizard generate virtual function overrides
	//{{AFX_VIRTUAL(CHitExportTextPage3)
	public:
	virtual BOOL OnSetActive();
	virtual BOOL OnKillActive();
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	// Generated message map functions
	//{{AFX_MSG(CHitExportTextPage3)
	virtual BOOL OnInitDialog();
	afx_msg void OnAlleHinzufuegen();
	afx_msg void OnAlleEntfernen();
	afx_msg void OnAuf();
	afx_msg void OnEntfernen();
	afx_msg void OnHinzufuegen();
	afx_msg void OnAb();
	afx_msg void OnSelchangeListCdAlles();
	afx_msg void OnSelchangeListCdAuswahl();
	afx_msg void OnDblclkListCdAlles();
	afx_msg void OnDblclkListCdAuswahl();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

};


/////////////////////////////////////////////////////////////////////////////
// CHitExportTextPage4 dialog

class CHitExportTextPage4 : public CPropertyPage
{
	DECLARE_DYNCREATE(CHitExportTextPage4)

// Construction
public:
	CHitExportTextSheet *ExportTextSheet;

	CHitExportTextPage4();
	~CHitExportTextPage4();
    void Update_Windows();

// Dialog Data
	//{{AFX_DATA(CHitExportTextPage4)
	enum { IDD = IDD_EXPPROPPAGE4 };
	CButton	m_Hinzufuegen;
	CButton	m_Entfernen;
	CButton	m_AlleHinzufuegen;
	CButton	m_AlleEntfernen;
	CButton	m_Ab;
	CButton	m_Auf;
	CListBox	m_AuswahlSONG;
	CListBox	m_AlleFelderSONG;
	//}}AFX_DATA


// Overrides
	// ClassWizard generate virtual function overrides
	//{{AFX_VIRTUAL(CHitExportTextPage4)
	public:
	virtual BOOL OnSetActive();
	virtual BOOL OnKillActive();
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	// Generated message map functions
	//{{AFX_MSG(CHitExportTextPage4)
	virtual BOOL OnInitDialog();
	afx_msg void OnAb();
	afx_msg void OnAlleEntfernen();
	afx_msg void OnAlleHinzufuegen();
	afx_msg void OnAuf();
	afx_msg void OnEntfernen();
	afx_msg void OnHinzufuegen();
	afx_msg void OnSelchangeListSongAlles();
	afx_msg void OnSelchangeListSongAuswahl();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

public:
	afx_msg void OnLbnDblclkListSongAlles();
	afx_msg void OnLbnDblclkListSongAuswahl();
};


/////////////////////////////////////////////////////////////////////////////
// CHitExportTextPage5 dialog

class CHitExportTextPage5 : public CPropertyPage
{
	DECLARE_DYNCREATE(CHitExportTextPage5)

// Construction
public:
	CHitExportTextSheet *ExportTextSheet;
	gcroot<Big3::Hitbase::DataBaseEngine::Condition^> ViewSelection;
	gcroot<Big3::Hitbase::DataBaseEngine::SortFieldCollection^> SortFields;

	CHitExportTextPage5();
	~CHitExportTextPage5();

	void write_header (CStdioFile* file);

// Dialog Data
	//{{AFX_DATA(CHitExportTextPage5)
	enum { IDD = IDD_EXPPROPPAGE5 };
	CStatic	m_AktSort;
	CStatic	m_AktSel;
	//}}AFX_DATA


// Overrides
	// ClassWizard generate virtual function overrides
	//{{AFX_VIRTUAL(CHitExportTextPage5)
	public:
	virtual BOOL OnSetActive();
	virtual BOOL OnWizardFinish();
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	// Generated message map functions
	//{{AFX_MSG(CHitExportTextPage5)
	virtual BOOL OnInitDialog();
	afx_msg void OnSelektion();
	afx_msg void OnSortierung();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

};



#endif // __HITEXPORTTEXTPAGE1_H__
