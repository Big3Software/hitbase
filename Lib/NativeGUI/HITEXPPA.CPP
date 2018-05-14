// HitExportTextPage1.cpp : implementation file
//

#include "stdafx.h"
#include "HitExpSh.h"
#include "HitExpPa.h"
#include "../hitmisc/HitbaseWinAppBase.h"
#include "../../app/hitbase/resource.h"
#include "../hitmisc/newfiledialog.h"

using namespace Big3::Hitbase::DataBaseEngine;

#ifdef _DEBUG
#undef THIS_FILE
static char BASED_CODE THIS_FILE[] = __FILE__;
#endif

IMPLEMENT_DYNCREATE(CHitExportTextPage1, CPropertyPage)
IMPLEMENT_DYNCREATE(CHitExportTextPage2, CPropertyPage)
IMPLEMENT_DYNCREATE(CHitExportTextPage3, CPropertyPage)
IMPLEMENT_DYNCREATE(CHitExportTextPage4, CPropertyPage)
IMPLEMENT_DYNCREATE(CHitExportTextPage5, CPropertyPage)


/////////////////////////////////////////////////////////////////////////////
// CHitExportTextPage1 property page

CHitExportTextPage1::CHitExportTextPage1() : CPropertyPage(CHitExportTextPage1::IDD)
{
	//{{AFX_DATA_INIT(CHitExportTextPage1)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}

CHitExportTextPage1::~CHitExportTextPage1()
{
}

void CHitExportTextPage1::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CHitExportTextPage1)
	DDX_Control(pDX, IDC_FILENAME, m_FileName);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CHitExportTextPage1, CPropertyPage)
	//{{AFX_MSG_MAP(CHitExportTextPage1)
	ON_BN_CLICKED(IDC_SEARCHFILE, OnSearchfile)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()


/////////////////////////////////////////////////////////////////////////////
// CHitExportTextPage2 property page

CHitExportTextPage2::CHitExportTextPage2() : CPropertyPage(CHitExportTextPage2::IDD)
{
	//{{AFX_DATA_INIT(CHitExportTextPage2)
	//}}AFX_DATA_INIT
}

CHitExportTextPage2::~CHitExportTextPage2()
{
}

void CHitExportTextPage2::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CHitExportTextPage2)
	DDX_Control(pDX, IDC_HEADER1, m_Feld1);
	DDX_Control(pDX, IDC_TRENN_SATZ, m_TrennSatz);
	DDX_Control(pDX, IDC_TRENN_FELD, m_TrennFeld);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CHitExportTextPage2, CPropertyPage)
	//{{AFX_MSG_MAP(CHitExportTextPage2)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()


/////////////////////////////////////////////////////////////////////////////
// CHitExportTextPage3 property page

CHitExportTextPage3::CHitExportTextPage3() : CPropertyPage(CHitExportTextPage3::IDD)
{
	//{{AFX_DATA_INIT(CHitExportTextPage3)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}

CHitExportTextPage3::~CHitExportTextPage3()
{
}

void CHitExportTextPage3::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CHitExportTextPage3)
	DDX_Control(pDX, IDC_HINZUFUEGEN, m_Hinzufuegen);
	DDX_Control(pDX, IDC_ENTFERNEN, m_Entfernen);
	DDX_Control(pDX, IDC_AUF, m_Auf);
	DDX_Control(pDX, IDC_ALLE_HINZUFUEGEN, m_AlleHinzufuegen);
	DDX_Control(pDX, IDC_ALLE_ENTFERNEN, m_AlleEntfernen);
	DDX_Control(pDX, IDC_AB, m_Ab);
	DDX_Control(pDX, IDC_LIST_CD_AUSWAHL, m_AuswahlCD);
	DDX_Control(pDX, IDC_LIST_CD_ALLES, m_AlleFelderCD);
	//}}AFX_DATA_MAP
}

/*
 * Enable/Disable Buttons for current dialog
 */
void CHitExportTextPage3::Update_Windows() 
{
    if (m_AuswahlCD.GetCount () > 0)
	   ExportTextSheet->SetWizardButtons (PSWIZB_NEXT|PSWIZB_BACK);
	else
	   ExportTextSheet->SetWizardButtons (PSWIZB_BACK);

    if (m_AuswahlCD.GetSelCount () > 0)
       m_Entfernen.EnableWindow (TRUE);
    else
       m_Entfernen.EnableWindow (FALSE);
    
    if (m_AlleFelderCD.GetSelCount () > 0)
       m_Hinzufuegen.EnableWindow (TRUE);
    else
       m_Hinzufuegen.EnableWindow (FALSE);

    if (m_AuswahlCD.GetCount () > 0)
       m_AlleEntfernen.EnableWindow (TRUE);
    else
       m_AlleEntfernen.EnableWindow (FALSE);

    if (m_AlleFelderCD.GetCount () > 0)
       m_AlleHinzufuegen.EnableWindow (TRUE);
    else
       m_AlleHinzufuegen.EnableWindow (FALSE);
    
    //if (m_AuswahlCD.GetCount () > 0)
    //   m_OK.EnableWindow (TRUE);
    //else
    //   m_OK.EnableWindow (FALSE);

    if (m_AuswahlCD.GetSelCount () == 1)
    {
	   if (m_AuswahlCD.GetCurSel () > 0)
          m_Auf.EnableWindow (TRUE);
	   else
          m_Auf.EnableWindow (FALSE);

	   if (m_AuswahlCD.GetCurSel () + 1 < m_AuswahlCD.GetCount())
          m_Ab.EnableWindow (TRUE);
	   else
          m_Ab.EnableWindow (FALSE);
	}
    else
    {
       m_Auf.EnableWindow (FALSE);
       m_Ab.EnableWindow (FALSE);
	}
}


BEGIN_MESSAGE_MAP(CHitExportTextPage3, CPropertyPage)
	//{{AFX_MSG_MAP(CHitExportTextPage3)
	ON_BN_CLICKED(IDC_ALLE_HINZUFUEGEN, OnAlleHinzufuegen)
	ON_BN_CLICKED(IDC_ALLE_ENTFERNEN, OnAlleEntfernen)
	ON_BN_CLICKED(IDC_AUF, OnAuf)
	ON_BN_CLICKED(IDC_ENTFERNEN, OnEntfernen)
	ON_BN_CLICKED(IDC_HINZUFUEGEN, OnHinzufuegen)
	ON_BN_CLICKED(IDC_AB, OnAb)
	ON_LBN_SELCHANGE(IDC_LIST_CD_ALLES, OnSelchangeListCdAlles)
	ON_LBN_SELCHANGE(IDC_LIST_CD_AUSWAHL, OnSelchangeListCdAuswahl)
	ON_LBN_DBLCLK(IDC_LIST_CD_ALLES, OnDblclkListCdAlles)
	ON_LBN_DBLCLK(IDC_LIST_CD_AUSWAHL, OnDblclkListCdAuswahl)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()


/////////////////////////////////////////////////////////////////////////////
// CHitExportTextPage4 property page

CHitExportTextPage4::CHitExportTextPage4() : CPropertyPage(CHitExportTextPage4::IDD)
{
	//{{AFX_DATA_INIT(CHitExportTextPage4)
	//}}AFX_DATA_INIT
}

CHitExportTextPage4::~CHitExportTextPage4()
{
}

void CHitExportTextPage4::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CHitExportTextPage4)
	DDX_Control(pDX, IDC_HINZUFUEGEN, m_Hinzufuegen);
	DDX_Control(pDX, IDC_ENTFERNEN, m_Entfernen);
	DDX_Control(pDX, IDC_ALLE_HINZUFUEGEN, m_AlleHinzufuegen);
	DDX_Control(pDX, IDC_ALLE_ENTFERNEN, m_AlleEntfernen);
	DDX_Control(pDX, IDC_AB, m_Ab);
	DDX_Control(pDX, IDC_AUF, m_Auf);
	DDX_Control(pDX, IDC_LIST_SONG_AUSWAHL, m_AuswahlSONG);
	DDX_Control(pDX, IDC_LIST_SONG_ALLES, m_AlleFelderSONG);
	//}}AFX_DATA_MAP
}

/*
 * Enable/Disable Buttons for current dialog
 */
void CHitExportTextPage4::Update_Windows() 
{
    if (m_AuswahlSONG.GetCount () > 0)
	   ExportTextSheet->SetWizardButtons (PSWIZB_NEXT|PSWIZB_BACK);
	else
	   ExportTextSheet->SetWizardButtons (PSWIZB_BACK);

    if (m_AuswahlSONG.GetSelCount () > 0)
       m_Entfernen.EnableWindow (TRUE);
    else
       m_Entfernen.EnableWindow (FALSE);
    
    if (m_AlleFelderSONG.GetSelCount () > 0)
       m_Hinzufuegen.EnableWindow (TRUE);
    else
       m_Hinzufuegen.EnableWindow (FALSE);

    if (m_AuswahlSONG.GetCount () > 0)
       m_AlleEntfernen.EnableWindow (TRUE);
    else
       m_AlleEntfernen.EnableWindow (FALSE);

    if (m_AlleFelderSONG.GetCount () > 0)
       m_AlleHinzufuegen.EnableWindow (TRUE);
    else
       m_AlleHinzufuegen.EnableWindow (FALSE);
    
    //if (m_AuswahlSONG.GetCount () > 0)
    //   m_OK.EnableWindow (TRUE);
    //else
    //   m_OK.EnableWindow (FALSE);
 
    if (m_AuswahlSONG.GetSelCount () == 1)
       {
	   if (m_AuswahlSONG.GetCurSel () > 0)
          m_Auf.EnableWindow (TRUE);
	   else
          m_Auf.EnableWindow (FALSE);

	   if (m_AuswahlSONG.GetCurSel () + 1 < m_AuswahlSONG.GetCount())
          m_Ab.EnableWindow (TRUE);
	   else
          m_Ab.EnableWindow (FALSE);
	   }
    else
       {
       m_Auf.EnableWindow (FALSE);
       m_Ab.EnableWindow (FALSE);
	   }
 }


BEGIN_MESSAGE_MAP(CHitExportTextPage4, CPropertyPage)
	//{{AFX_MSG_MAP(CHitExportTextPage4)
	ON_BN_CLICKED(IDC_AB, OnAb)
	ON_BN_CLICKED(IDC_ALLE_ENTFERNEN, OnAlleEntfernen)
	ON_BN_CLICKED(IDC_ALLE_HINZUFUEGEN, OnAlleHinzufuegen)
	ON_BN_CLICKED(IDC_AUF, OnAuf)
	ON_BN_CLICKED(IDC_ENTFERNEN, OnEntfernen)
	ON_BN_CLICKED(IDC_HINZUFUEGEN, OnHinzufuegen)
	ON_LBN_SELCHANGE(IDC_LIST_SONG_ALLES, OnSelchangeListSongAlles)
	ON_LBN_SELCHANGE(IDC_LIST_SONG_AUSWAHL, OnSelchangeListSongAuswahl)
	//}}AFX_MSG_MAP
	ON_LBN_DBLCLK(IDC_LIST_SONG_ALLES, OnLbnDblclkListSongAlles)
	ON_LBN_DBLCLK(IDC_LIST_SONG_AUSWAHL, OnLbnDblclkListSongAuswahl)
END_MESSAGE_MAP()


/////////////////////////////////////////////////////////////////////////////
// CHitExportTextPage5 property page

CHitExportTextPage5::CHitExportTextPage5() : CPropertyPage(CHitExportTextPage5::IDD)
{
	//{{AFX_DATA_INIT(CHitExportTextPage5)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}

CHitExportTextPage5::~CHitExportTextPage5()
{
}

void CHitExportTextPage5::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CHitExportTextPage5)
	DDX_Control(pDX, IDC_AKTUELLE_SORTIERUNG, m_AktSort);
	DDX_Control(pDX, IDC_AKTUELLE_SELEKTION, m_AktSel);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CHitExportTextPage5, CPropertyPage)
	//{{AFX_MSG_MAP(CHitExportTextPage5)
	ON_BN_CLICKED(IDC_SELEKTION, OnSelektion)
	ON_BN_CLICKED(IDC_SORTIERUNG, OnSortierung)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()



BOOL CHitExportTextPage2::OnInitDialog() 
{
    HKEY HitKeyExportText;
	DWORD size;
	wchar_t str[_MAX_PATH];
    CString exportfeld;
	DWORD check;

	CPropertyPage::OnInitDialog();
    
    size = 0L;
    RegCreateKeyEx(HKEY_CURRENT_USER, (CString)REGISTRY_KEY + L"\\ExportText", 0,
                   L"Hitbase", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL,
                   &HitKeyExportText, &size);

    size = sizeof(DWORD);
	if (RegQueryValueEx(HitKeyExportText, L"ExportTextExportType", 0, NULL,
	                    (unsigned char *)&check, &size) == ERROR_SUCCESS)
	{
		CheckRadioButton(IDC_HEADER1, IDC_HEADER2, check);
	}
	else
	{
		CheckRadioButton(IDC_HEADER1, IDC_HEADER2, IDC_HEADER1);
	}

    exportfeld.LoadString (TEXT_EXPORT_CRNL);
    m_TrennSatz.AddString (exportfeld);
    m_TrennSatz.AddString (L"TAB");
    m_TrennSatz.AddString (L";");
    m_TrennSatz.AddString (L",");
    m_TrennSatz.AddString (L"|");
    m_TrennSatz.AddString (L"_");
    m_TrennSatz.AddString (L"-");
    m_TrennSatz.AddString (L":");
    m_TrennSatz.AddString (L".");
    m_TrennSatz.AddString (L"#");
    m_TrennSatz.AddString (L"+");
    m_TrennSatz.AddString (L"=");
    m_TrennSatz.AddString (L"*");
    m_TrennSatz.AddString (L"~");
    m_TrennSatz.AddString (L"§");
    m_TrennSatz.AddString (L"\\");
    m_TrennSatz.AddString (L"/");
    m_TrennSatz.AddString (L"!");
    m_TrennSatz.AddString (L"$");
    m_TrennSatz.AddString (L"%");
    m_TrennSatz.AddString (L"&");
    m_TrennSatz.AddString (L"?");
    m_TrennSatz.AddString (L"'");
    m_TrennSatz.AddString (L"^");

    m_TrennSatz.SetCurSel (0);

    m_TrennFeld.AddString (L"TAB");
    exportfeld.LoadString (TEXT_EXPORT_CRNL);
    m_TrennFeld.AddString (exportfeld);
    m_TrennFeld.AddString (L";");
    m_TrennFeld.AddString (L",");
    m_TrennFeld.AddString (L"|");
    m_TrennFeld.AddString (L"_");
    m_TrennFeld.AddString (L"-");
    m_TrennFeld.AddString (L":");
    m_TrennFeld.AddString (L".");
    m_TrennFeld.AddString (L"#");
    m_TrennFeld.AddString (L"+");
    m_TrennFeld.AddString (L"=");
    m_TrennFeld.AddString (L"*");
    m_TrennFeld.AddString (L"~");
    m_TrennFeld.AddString (L"§");
    m_TrennFeld.AddString (L"\\");
    m_TrennFeld.AddString (L"/");
    m_TrennFeld.AddString (L"!");
    m_TrennFeld.AddString (L"$");
    m_TrennFeld.AddString (L"%");
    m_TrennFeld.AddString (L"&");
    m_TrennFeld.AddString (L"?");
    m_TrennFeld.AddString (L"'");
    m_TrennFeld.AddString (L"^");

    m_TrennFeld.SetCurSel (0);

    size = _MAX_PATH;
	if (RegQueryValueEx(HitKeyExportText, L"ExportTextTrennFeld", 0, NULL,
	                    (unsigned char *)str, &size) == ERROR_SUCCESS)
	{
        m_TrennFeld.SetWindowText(str);
	}

    size = _MAX_PATH;
	if (RegQueryValueEx(HitKeyExportText, L"ExportTextTrennSatz", 0, NULL,
	                    (unsigned char *)str, &size) == ERROR_SUCCESS)
	{
        m_TrennSatz.SetWindowText(str);
	}
	
	RegCloseKey(HitKeyExportText);

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

BOOL CHitExportTextPage3::OnInitDialog() 
{
	HKEY HitKeyExportText;

	CPropertyPage::OnInitDialog();

	HitKeyExportText = CConfig::CreateSubKey(L"ExportText");

	CFieldList fl = CFieldList::GetAllFields(FLF_CD|FLF_CD_NO_ARTIST);

	for (int i=0;i<fl.GetSize();i++)
	{
		int iDummy;
		int iIndex = m_AlleFelderCD.AddString((CString)ExportTextSheet->DataBase->GetNameOfField(Big3::Hitbase::DataBaseEngine::FieldHelper::GetNewFieldFromOldField(fl[i])));
		m_AlleFelderCD.SetItemData(iIndex, fl[i]);
	}

	CFieldList flSelected;
	flSelected.Load(L"ExportTextFieldsCD", HitKeyExportText);

	for (int i=0;i<flSelected.GetSize();i++)
	{
		int iItem = -1;
		for (int j=0;j<m_AlleFelderCD.GetCount();j++)
		{
			if (m_AlleFelderCD.GetItemData(j) == flSelected[i])
			{
				iItem = j;
				break;
			}
		}

		ASSERT(iItem >= 0);

		if (iItem >= 0)
		{
			CString sBuf;
			m_AlleFelderCD.GetText (iItem, sBuf);

			int iIndex = m_AuswahlCD.InsertString (-1, sBuf);
			m_AuswahlCD.SetItemData(iIndex, m_AlleFelderCD.GetItemData(iItem));
			m_AlleFelderCD.DeleteString (iItem);
		}
	}

	CConfig::CloseKey(HitKeyExportText);

	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

BOOL CHitExportTextPage4::OnInitDialog() 
{
	CPropertyPage::OnInitDialog();
	
	HKEY HitKeyExportText = CConfig::CreateSubKey(L"ExportText");

	CFieldList fl = CFieldList::GetAllFields(FLF_TRACK|FLF_TRACK_NO_ARTIST);

	for (int i=0;i<fl.GetSize();i++)
	{
		int iDummy;
		int iIndex = m_AlleFelderSONG.AddString((CString)ExportTextSheet->DataBase->GetNameOfField(Big3::Hitbase::DataBaseEngine::FieldHelper::GetNewFieldFromOldField(fl[i])));
		m_AlleFelderSONG.SetItemData(iIndex, fl[i]);
	}

	CFieldList flSelected;
	flSelected.Load(L"ExportTextFieldsSONG", HitKeyExportText);

	for (int i=0;i<flSelected.GetSize();i++)
	{
		int iItem = -1;
		for (int j=0;j<m_AlleFelderSONG.GetCount();j++)
		{
			if (m_AlleFelderSONG.GetItemData(j) == flSelected[i])
			{
				iItem = j;
				break;
			}
		}

		ASSERT(iItem >= 0);

		if (iItem >= 0)
		{
			CString sBuf;
			m_AlleFelderSONG.GetText (iItem, sBuf);

			int iIndex = m_AuswahlSONG.InsertString (-1, sBuf);
			m_AuswahlSONG.SetItemData(iIndex, m_AlleFelderSONG.GetItemData(iItem));
			m_AlleFelderSONG.DeleteString (iItem);
		}
	}

	CConfig::CloseKey(HitKeyExportText);

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CHitExportTextPage3::OnAlleHinzufuegen() 
{
	int sc;
    CString buf;

	for (sc = 0; sc < m_AlleFelderCD.GetCount(); sc++)
	{
		m_AlleFelderCD.GetText (sc, buf);

		int iIndex = m_AuswahlCD.AddString (buf);
		m_AuswahlCD.SetItemData(iIndex, m_AlleFelderCD.GetItemData(sc));
	}

	Update_Windows();

    m_AlleFelderCD.ResetContent ();
}

void CHitExportTextPage3::OnAlleEntfernen() 
{
	int sc;
    CString buf;

	for (sc = 0; sc < m_AuswahlCD.GetCount(); sc++)
	{
		m_AuswahlCD.GetText (sc, buf);

		int iIndex = m_AlleFelderCD.AddString (buf);
		m_AlleFelderCD.SetItemData(iIndex, m_AuswahlCD.GetItemData(sc));
	}

    m_AuswahlCD.ResetContent ();

	Update_Windows ();
	
}

void CHitExportTextPage3::OnSelchangeListCdAlles() 
{
	Update_Windows();	
}

void CHitExportTextPage3::OnSelchangeListCdAuswahl() 
{
	Update_Windows();
}

void CHitExportTextPage3::OnAuf() 
{
    CString buf;
	int pos;

	pos = m_AuswahlCD.GetCurSel ();
    m_AuswahlCD.GetText (pos, buf);
	int iData = m_AuswahlCD.GetItemData(pos);
    m_AuswahlCD.DeleteString (pos);

    int iIndex = m_AuswahlCD.InsertString (pos - 1, buf);
	m_AuswahlCD.SetItemData(iIndex, iData);

	m_AuswahlCD.SetSel (pos - 1, TRUE);

	Update_Windows ();
}

void CHitExportTextPage3::OnAb() 
{
    CString buf;
	int pos;

	pos = m_AuswahlCD.GetCurSel ();
    m_AuswahlCD.GetText (pos, buf);
	int iData = m_AuswahlCD.GetItemData(pos);

    int iIndex = m_AuswahlCD.InsertString (pos + 2, buf);
	m_AuswahlCD.SetItemData(iIndex, iData);

    m_AuswahlCD.DeleteString (pos);

	m_AuswahlCD.SetSel (pos + 1, TRUE);

	Update_Windows();
}

void CHitExportTextPage3::OnEntfernen() 
{
	int sc;
    CString buf;
    int cur_sel[30];
    int count;

	m_AuswahlCD.GetSelItems (30, (LPINT)&cur_sel);
	count = m_AuswahlCD.GetSelCount();
	for (sc = 0; sc < count; sc++)
	{
		m_AuswahlCD.GetText (cur_sel[sc], buf);

		int iIndex = m_AlleFelderCD.AddString (buf);
		m_AlleFelderCD.SetItemData(iIndex, m_AuswahlCD.GetItemData(cur_sel[sc]));
	}
	for (sc = count; sc > 0; sc--)
		m_AuswahlCD.DeleteString (cur_sel[sc - 1]);

	m_AuswahlCD.SetSel (-1, FALSE);

	Update_Windows();
	
}

void CHitExportTextPage3::OnHinzufuegen() 
{
	int sc;
    CString buf;
    int cur_sel[30];
    int count;

    m_AlleFelderCD.GetSelItems (30, (LPINT)&cur_sel);
    count = m_AlleFelderCD.GetSelCount();
	for (sc = 0; sc < count; sc++)
	{
		m_AlleFelderCD.GetText (cur_sel[sc], buf);

		int iIndex = m_AuswahlCD.InsertString (-1, buf);
		m_AuswahlCD.SetItemData(iIndex, m_AlleFelderCD.GetItemData(cur_sel[sc]));
	}
    for (sc = count; sc > 0; sc--)
        m_AlleFelderCD.DeleteString (cur_sel[sc - 1]);

    m_AlleFelderCD.SetSel (-1, FALSE);

	Update_Windows ();
}


void CHitExportTextPage3::OnDblclkListCdAlles() 
{
	OnHinzufuegen();	
}

void CHitExportTextPage3::OnDblclkListCdAuswahl() 
{
	OnEntfernen();	
}

void CHitExportTextPage4::OnAb() 
{
    CString buf;
	int pos;

	pos = m_AuswahlSONG.GetCurSel ();
    m_AuswahlSONG.GetText (pos, buf);
	int iData = m_AuswahlSONG.GetItemData(pos);

    int iIndex = m_AuswahlSONG.InsertString (pos + 2, buf);
	m_AuswahlSONG.SetItemData(iIndex, iData);

    m_AuswahlSONG.DeleteString (pos);

	m_AuswahlSONG.SetSel (pos + 1, TRUE);

	Update_Windows();
}

void CHitExportTextPage4::OnLbnDblclkListSongAlles()
{
    OnHinzufuegen();	
}

void CHitExportTextPage4::OnLbnDblclkListSongAuswahl()
{
	OnEntfernen();
}

void CHitExportTextPage4::OnAlleEntfernen() 
{
	// TODO: Add your control notification handler code here
	int sc;
    CString buf;

	for (sc = 0; sc < m_AuswahlSONG.GetCount(); sc++)
	{
		m_AuswahlSONG.GetText (sc, buf);

		int iIndex = m_AlleFelderSONG.AddString (buf);
		m_AlleFelderSONG.SetItemData(iIndex, m_AuswahlSONG.GetItemData(sc));
	}

    m_AuswahlSONG.ResetContent ();

	Update_Windows ();
}

void CHitExportTextPage4::OnAlleHinzufuegen() 
{
	int sc;
    CString buf;
    
	for (sc = 0; sc < m_AlleFelderSONG.GetCount(); sc++)
	{
		m_AlleFelderSONG.GetText (sc, buf);

		int iIndex = m_AuswahlSONG.AddString (buf);
		m_AuswahlSONG.SetItemData(iIndex, m_AlleFelderSONG.GetItemData(sc));
	}

	Update_Windows();

    m_AlleFelderSONG.ResetContent ();
}

void CHitExportTextPage4::OnAuf() 
{
	// TODO: Add your control notification handler code here
    CString buf;
	int pos;

	pos = m_AuswahlSONG.GetCurSel ();
    m_AuswahlSONG.GetText (pos, buf);
	int iData = m_AuswahlSONG.GetItemData(pos);
    m_AuswahlSONG.DeleteString (pos);

    int iIndex = m_AuswahlSONG.InsertString (pos - 1, buf);
	m_AuswahlSONG.SetItemData(iIndex, iData);

	m_AuswahlSONG.SetSel (pos - 1, TRUE);

	Update_Windows();
}

void CHitExportTextPage4::OnEntfernen() 
{
	int sc;
    CString buf;
    int cur_sel[30];
    int count;

    m_AuswahlSONG.GetSelItems (30, (LPINT)&cur_sel);
    count = m_AuswahlSONG.GetSelCount();
	for (sc = 0; sc < count; sc++)
	{
		m_AuswahlSONG.GetText (cur_sel[sc], buf);

		int iIndex = m_AlleFelderSONG.AddString (buf);
		m_AlleFelderSONG.SetItemData(iIndex, m_AuswahlSONG.GetItemData(cur_sel[sc]));
	}
    for (sc = count; sc > 0; sc--)
        m_AuswahlSONG.DeleteString (cur_sel[sc - 1]);

    m_AuswahlSONG.SetSel (-1, FALSE);

	Update_Windows();
}

void CHitExportTextPage4::OnHinzufuegen() 
{
	int sc;
    CString buf;
    int cur_sel[30];
    int count;

    m_AlleFelderSONG.GetSelItems (30, (LPINT)&cur_sel);
    count = m_AlleFelderSONG.GetSelCount();
	for (sc = 0; sc < count; sc++)
	{
		m_AlleFelderSONG.GetText (cur_sel[sc], buf);

		int iIndex = m_AuswahlSONG.InsertString (-1, buf);
		m_AuswahlSONG.SetItemData(iIndex, m_AlleFelderSONG.GetItemData(cur_sel[sc]));
	}

	for (sc = count; sc > 0; sc--)
        m_AlleFelderSONG.DeleteString (cur_sel[sc - 1]);

    m_AlleFelderSONG.SetSel (-1, FALSE);

	Update_Windows ();
}

void CHitExportTextPage4::OnSelchangeListSongAlles() 
{
	Update_Windows();
}

void CHitExportTextPage4::OnSelchangeListSongAuswahl() 
{
	Update_Windows();
}

void CHitExportTextPage1::OnSearchfile() 
{
	CString str = get_string(TEXT_EXPORT_EXT);

    CNewFileDialog SaveDialog (FALSE, L"txt", L"export", OFN_HIDEREADONLY|OFN_OVERWRITEPROMPT,
                str, GetParent());

	SaveDialog.DoModal(L"ExportText");

    m_FileName.SetWindowText(SaveDialog.GetPathName());
}

BOOL CHitExportTextPage1::OnInitDialog() 
{
    HKEY HitKeyExportText;
	CString str;

	CPropertyPage::OnInitDialog();
	
    RegCreateKeyEx(HKEY_CURRENT_USER, (CString)REGISTRY_KEY + L"\\ExportText", 0,
                   L"Hitbase", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL,
                   &HitKeyExportText, NULL);

	str = CConfig::RegQueryString(HitKeyExportText, L"ExportTextFileName", L"");
	m_FileName.SetWindowText(str);
	
	RegCloseKey(HitKeyExportText);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}


BOOL CHitExportTextPage1::OnKillActive() 
{
    HKEY HitKeyExportText;
	CString filename;
    
    RegCreateKeyEx(HKEY_CURRENT_USER, (CString)REGISTRY_KEY + L"\\ExportText", 0,
                   L"Hitbase", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL,
                   &HitKeyExportText, NULL);

	m_FileName.GetWindowText(filename);
	CConfig::RegWriteString(HitKeyExportText, L"ExportTextFileName", filename);

	RegCloseKey(HitKeyExportText);
	
	return CPropertyPage::OnKillActive();
}

BOOL CHitExportTextPage2::OnKillActive() 
{
    HKEY HitKeyExportText;
	DWORD size;
	CString filename;
	DWORD check;
	CString trennsatz, trennfeld;

    size = 0L;
    RegCreateKeyEx(HKEY_CURRENT_USER, (CString)REGISTRY_KEY + L"\\ExportText", 0,
                   L"Hitbase", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL,
                   &HitKeyExportText, &size);

	check = GetCheckedRadioButton(IDC_HEADER1, IDC_HEADER2);

	CConfig::RegWriteInt(HitKeyExportText, L"ExportTextExportType", check);

	m_TrennFeld.GetWindowText (trennfeld);
	
	CConfig::RegWriteString(HitKeyExportText, L"ExportTextTrennFeld", trennfeld);
	
	m_TrennSatz.GetWindowText (trennsatz);
    CConfig::RegWriteString(HitKeyExportText, L"ExportTextTrennSatz", trennsatz);

	RegCloseKey(HitKeyExportText);
	
	return CPropertyPage::OnKillActive();
}

BOOL CHitExportTextPage5::OnInitDialog() 
{
	CPropertyPage::OnInitDialog();
	
	ViewSelection = gcnew Condition();
	SortFields = gcnew SortFieldCollection();

    m_AktSort.SetWindowText((CString)ViewSelection->GetConditionString(ExportTextSheet->DataBase));
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

BOOL CHitExportTextPage1::OnSetActive() 
{
	ExportTextSheet->SetWizardButtons (PSWIZB_NEXT);
	
	return CPropertyPage::OnSetActive();
}

BOOL CHitExportTextPage2::OnSetActive() 
{
	ExportTextSheet->SetWizardButtons (PSWIZB_NEXT|PSWIZB_BACK);
	
	return CPropertyPage::OnSetActive();
}

BOOL CHitExportTextPage3::OnSetActive() 
{
    ExportTextSheet->SetWizardButtons (PSWIZB_NEXT|PSWIZB_BACK);

	return CPropertyPage::OnSetActive();
}

BOOL CHitExportTextPage4::OnSetActive() 
{
    if (m_AuswahlSONG.GetCount () > 0 || ExportTextSheet->m_Page3.m_AuswahlCD.GetCount () > 0)
	   ExportTextSheet->SetWizardButtons (PSWIZB_NEXT|PSWIZB_BACK);
    else
	   ExportTextSheet->SetWizardButtons (PSWIZB_BACK);

	return CPropertyPage::OnSetActive();
}

BOOL CHitExportTextPage5::OnSetActive() 
{
    CString finishtext;

	ExportTextSheet->SetWizardButtons (PSWIZB_BACK|PSWIZB_FINISH);

	ExportTextSheet->SetDlgItemText( PSWIZB_FINISH, L"Export");

	return CPropertyPage::OnSetActive();
}


void CHitExportTextPage5::OnSelektion() 
{
//	CSearchSheet SearchSheet(TEXT_SEARCH, this, 0);
	
//	SearchSheet.OpenDialog(&ViewSelection, ExportTextSheet->m_HitbaseDoc->DataBase);

	Big3::Hitbase::CDUtilities::FormSearch formSearch(ExportTextSheet->DataBase, ViewSelection, false, true);

	if (formSearch.ShowDialog() == System::Windows::Forms::DialogResult::OK)
	{
		ViewSelection = formSearch.GetCondition();

		m_AktSel.SetWindowText((CString)ViewSelection->GetConditionString(ExportTextSheet->DataBase));
	}
}

void CHitExportTextPage5::OnSortierung() 
{
	Big3::Hitbase::CDUtilities::FormSort formSort(ExportTextSheet->DataBase, Big3::Hitbase::DataBaseEngine::FieldType::CD, SortFields);
	if (formSort.ShowDialog() == Windows::Forms::DialogResult::OK)
	{
		SortFields = formSort.SortFields;
	}

   m_AktSort.SetWindowText((CString)SortFields->GetSortString(this->ExportTextSheet->DataBase));
}

BOOL CHitExportTextPage3::OnKillActive() 
{
    HKEY HitKeyExportText;
    int count, sc;
	CFieldList fl;

	HitKeyExportText = CConfig::CreateSubKey(L"ExportText");

	// now build registry key - now right listbox
	count = m_AuswahlCD.GetCount();
	for (sc = 0; sc < count; sc++)
	{
		fl.Add(m_AuswahlCD.GetItemData(sc));
	}

	fl.Save(L"ExportTextFieldsCD", HitKeyExportText);

	CConfig::CloseKey(HitKeyExportText);
	
	return CPropertyPage::OnKillActive();
}

BOOL CHitExportTextPage4::OnKillActive() 
{
    HKEY HitKeyExportText;
    int count, sc;
	CFieldList fl;

	HitKeyExportText = CConfig::CreateSubKey(L"ExportText");

	// now build registry key - now right listbox
	count = m_AuswahlSONG.GetCount();
	for (sc = 0; sc < count; sc++)
	{
		fl.Add(m_AuswahlSONG.GetItemData(sc));
	}

	fl.Save(L"ExportTextFieldsSONG", HitKeyExportText);

	CConfig::CloseKey(HitKeyExportText);
	

	return CPropertyPage::OnKillActive();
}

BOOL CHitExportTextPage5::OnWizardFinish() 
{
	CString import;
	int iAddRecord=0;
	CString sBuf;
	// nun - jetzt tun wir mal was... export...
	CDQueryDataSet^ query = ExportTextSheet->DataBase->ExecuteTrackQuery();
    CDDataView^ CDView = gcnew CDDataView(ExportTextSheet->DataBase, query, ViewSelection, SortFields);

	int found;
	int count, sc, sc1;
	int check_export_type;
	CStdioFile file;
	wchar_t buf[_MAX_PATH];
	CString message, message1, error;
	int max_tracks;
	CUIntArray export_cd_fields;
	CUIntArray export_song_fields;

	wchar_t trennsatz[_MAX_PATH];
	wchar_t trennfeld[_MAX_PATH];
	CString str;

	//CDataBaseProgressDlg DataBaseProgressDlg(this);
	CWaitCursor wait;

	ExportTextSheet->m_Page2.m_TrennSatz.GetWindowText(trennsatz, _MAX_PATH);
	ExportTextSheet->m_Page2.m_TrennFeld.GetWindowText(trennfeld, _MAX_PATH);

	str.LoadString (TEXT_EXPORT_CRNL);
	if (wcscmp (str, trennsatz) == 0)
	{
		wcscpy_s (trennsatz, _countof(trennsatz), L"\r\n");
	}
	if (wcscmp (str, trennfeld) == 0)
	{
		wcscpy_s (trennfeld, _countof(trennsatz), L"\r\n");
	}

	if (wcscmp (trennsatz, L"TAB") == 0)
	{
		wcscpy_s (trennsatz, _countof(trennsatz), L"\t");
	}

	if (wcscmp (trennfeld, L"TAB") == 0)
	{
		wcscpy_s (trennfeld, _countof(trennsatz), L"\t");
	}

	//CDataBaseProgressDlg DataBaseProgressDlg;
	//DataBaseProgressDlg.Create(IDD_HITDB_DATABASE_PROGRESS, this);
	//DataBaseProgressDlg.SetWindowText(get_string(TEXT_BUILDING_EXPORTFILE));

	ExportTextSheet->m_Page1.m_FileName.GetWindowText (buf, _MAX_PATH);

	if (!file.Open(buf, CFile::modeReadWrite|CFile::modeCreate))
	{
		message1.LoadString (TEXT_EXPORT_ERROR_FILE_CREATE);
		error.LoadString (TEXT_ERROR);
		message.Format (message1, buf);
		MessageBox (message, error, MB_OK);
		return CPropertyPage::OnWizardFinish();
	}

	max_tracks = 0;

	if (CDView->Rows->Count > 0)
	{
		// check for generation of export-header
		//ExportTextSheet->m_Page1.m_FileName.GetWindowText (buf, _MAX_PATH);
		check_export_type = ExportTextSheet->m_Page2.GetCheckedRadioButton(IDC_HEADER1, IDC_HEADER2);
		if (check_export_type == IDC_HEADER2)
		{
			// generate header
			write_header (&file);
		}
		else
		{
			// now first get max. songs
			for (int i=0;i<CDView->Rows->Count;i++)
			{
				int numberOfTracks = (int)CDView->GetRowRawValue(i, Field::NumberOfTracks);
				
				if (numberOfTracks > max_tracks)
				{
					max_tracks = numberOfTracks;
				}
			}
		}
	}
	else
	{
		file.Close();

		return CPropertyPage::OnWizardFinish();
	}

	// now get all entry-numbers in selected listbox - cd-entries
	count = ExportTextSheet->m_Page3.m_AuswahlCD.GetCount();

	for (sc = 0; sc < count; sc++)
		export_cd_fields.Add(ExportTextSheet->m_Page3.m_AuswahlCD.GetItemData(sc));

	// now get all entry-numbers in selected listbox - song-entries
	count = ExportTextSheet->m_Page4.m_AuswahlSONG.GetCount();

	for (sc = 0; sc < count; sc++)
		export_song_fields.Add(ExportTextSheet->m_Page4.m_AuswahlSONG.GetItemData(sc));

	// now export cd-data

	//DataBaseProgressDlg.m_ProgressCtrl.SetRange32(0, CDView->Rows->Count);

	for (int i=0;i<CDView->Rows->Count;i++)
	{
		BOOL bFirst = TRUE;
		Big3::Hitbase::DataBaseEngine::CD^ CD = ExportTextSheet->DataBase->GetCDById(CDView->GetCDID(i));

		for (sc=0;sc<export_cd_fields.GetSize();sc++)
		{
			CString sValue = CD->GetStringByField(ExportTextSheet->DataBase, FieldHelper::GetNewFieldFromOldField(export_cd_fields[sc]));
			if (!bFirst)
				file.WriteString(trennfeld);
			file.WriteString(sValue);
			bFirst = FALSE;
		}

		// now process track info
		if (max_tracks <= 0)
			max_tracks = 99;

		for (sc = 0; sc < max_tracks; sc++)
		{
			sc1 = 0;
			for (sc1=0;sc1<export_song_fields.GetSize();sc1++)
			{
				if (sc < CD->NumberOfTracks)
				{
					CString sValue = CD->GetTrackStringByField(sc, FieldHelper::GetNewFieldFromOldField(export_song_fields[sc1]));
					if (!bFirst)
						file.WriteString(trennfeld);
					file.WriteString(sValue);
					bFirst = FALSE;
				}
			}
		}

		file.WriteString(trennsatz);
		
		//DataBaseProgressDlg.m_ProgressCtrl.SetPos(i);

		System::Windows::Forms::Application::DoEvents();
	}

	file.Close();

	//DataBaseProgressDlg.DestroyWindow();

	return CPropertyPage::OnWizardFinish();
}

// write header to export file - only if selected
void CHitExportTextPage5::write_header(CStdioFile* file)
{
	int count;
	int sc, num_song;
	wchar_t *p;
	wchar_t buf[_MAX_PATH];
	wchar_t trennsatz[_MAX_PATH];
	wchar_t trennfeld[_MAX_PATH];
	wchar_t count_str[_MAX_PATH];
	CString str;

    ExportTextSheet->m_Page2.m_TrennSatz.GetWindowText(trennsatz, _MAX_PATH);
    ExportTextSheet->m_Page2.m_TrennFeld.GetWindowText(trennfeld, _MAX_PATH);

	str.LoadString (TEXT_EXPORT_CRNL);
	if (wcscmp (str, trennsatz) == 0)
	{
		wcscpy_s (trennsatz, _countof(trennsatz), L"\r\n");
	}
	if (wcscmp (str, trennfeld) == 0)
	{
		wcscpy_s (trennfeld, _countof(trennsatz), L"\r\n");
	}

	if (wcscmp (trennsatz, L"TAB") == 0)
	{
		wcscpy_s (trennsatz, _countof(trennsatz), L"\t");
	}

	if (wcscmp (trennfeld, L"TAB") == 0)
	{
		wcscpy_s (trennfeld, _countof(trennsatz), L"\t");
	}

	// now get all entries in selected listbox - cd-entries
    count = ExportTextSheet->m_Page3.m_AuswahlCD.GetCount();
    
	for (sc = 0; sc < count; sc++)
    {
        ExportTextSheet->m_Page3.m_AuswahlCD.GetText (sc, buf);
		p = wcstok (buf, L"\t");

		file->WriteString(p);

		//if (sc < count - 1)
		file->WriteString(trennfeld);
    }
	// now get all entries in selected listbox - song-entries
    count = ExportTextSheet->m_Page4.m_AuswahlSONG.GetCount();
    
	for (num_song = 1; num_song < 100; num_song++)
	{
		for (sc = 0; sc < count; sc++)
        {
            ExportTextSheet->m_Page4.m_AuswahlSONG.GetText (sc, buf);
		    p = wcstok (buf, L"\t");

		    file->WriteString(p);
			wsprintf (count_str, L" %d", num_song);
			file->WriteString(count_str);

	        if (num_song != 99 || sc < count - 1)
				file->WriteString(trennfeld);
        }
	}

	file->WriteString(trennsatz);
}
