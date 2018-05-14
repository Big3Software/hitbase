// HTMLExportDlg.cpp : Implementierungsdatei
//

#include "stdafx.h"
#include "../hitmisc/HitbaseWinAppBase.h"
#include "../../app/hitbase/resource.h"
#include "HTMLExportDlg.h"
#include "../hitmisc/misc.h"
#include "../hitmisc/newfiledialog.h"

using namespace Big3::Hitbase::DataBaseEngine;

// CHTMLExportDlg-Dialogfeld

IMPLEMENT_DYNAMIC(CHTMLExportDlg, CDialog)
CHTMLExportDlg::CHTMLExportDlg(Big3::Hitbase::DataBaseEngine::DataBase^ db, CWnd* pParent /*=NULL*/)
	: CDialog(CHTMLExportDlg::IDD, pParent)
	, m_sHTMLTemplate(_T(""))
	, m_sHTMLOutput(_T(""))
	, m_sSelection(_T("")), m_sSortierung(_T(""))
	, m_bShowFile(FALSE)
{
	DataBase = db;
	Condition = gcnew Big3::Hitbase::DataBaseEngine::Condition();
	SortFields = gcnew Big3::Hitbase::DataBaseEngine::SortFieldCollection();
}

CHTMLExportDlg::~CHTMLExportDlg()
{
}

void CHTMLExportDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Text(pDX, IDC_HTML_TEMPLATE, m_sHTMLTemplate);
	DDX_Text(pDX, IDC_HTML_OUTPUT, m_sHTMLOutput);
	DDX_Text(pDX, IDC_AKTUELLE_SELEKTION, m_sSelection);
	DDX_Text(pDX, IDC_AKTUELLE_SORTIERUNG, m_sSortierung);
	DDX_Check(pDX, IDC_SHOWFILE, m_bShowFile);
	DDX_Control(pDX, IDOK, m_btnOK);
}


BEGIN_MESSAGE_MAP(CHTMLExportDlg, CDialog)
	ON_BN_CLICKED(IDC_HTML_TEMPLATE_BROWSE, OnBnClickedHtmlTemplateBrowse)
	ON_BN_CLICKED(IDC_HTML_OUTPUT_BROWSE, OnBnClickedHtmlOutputBrowse)
	ON_BN_CLICKED(IDC_SELEKTION, OnBnClickedSelektion)
	ON_BN_CLICKED(IDC_SORTIERUNG, OnBnClickedSortierung)
	ON_EN_CHANGE(IDC_HTML_TEMPLATE, OnEnChangeHtmlTemplate)
	ON_EN_CHANGE(IDC_HTML_OUTPUT, OnEnChangeHtmlOutput)
	ON_BN_CLICKED(IDC_EXPORTTITELLISTE, &CHTMLExportDlg::OnBnClickedExportTITELListe)
	ON_BN_CLICKED(IDC_EXPORTCDLISTE, &CHTMLExportDlg::OnBnClickedExportCDListe)
	ON_BN_CLICKED(IDOK, &CHTMLExportDlg::OnBnClickedOk)
	ON_BN_CLICKED(IDC_RESETHTMLFILTER, &CHTMLExportDlg::OnBnClickedResethtmlfilter)
END_MESSAGE_MAP()


// CHTMLExportDlg-Meldungshandler

BOOL CHTMLExportDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	DWORD sorttype;

	UpdateData();	
	DWORD extype = (int)Big3::Hitbase::Configuration::Settings::GetValue(L"HTMLOutputExportType", IDC_EXPORTCDLISTE);
	
	CheckRadioButton(IDC_EXPORTCDLISTE, IDC_EXPORTTITELLISTE, extype);
	m_dwExportType = extype;
	
	if (m_dwExportType == IDC_EXPORTCDLISTE)
	{
		sorttype = (int)Big3::Hitbase::Configuration::Settings::GetValue(L"HTMLOutputExportSortType", IDC_EXPORTCDLISTE);

		//m_pSelection->m_SortKeys[0]=FIELD_CDNAME;
		//m_pSelection->m_SortKeys[1]=FIELD_TRACK_NUMBER;
		//m_pSelection->m_bSortKeysDirection[0] = TRUE;
		//m_pSelection->m_bSortKeysDirection[1] = TRUE;
		Condition->RestoreFromRegistry(gcnew String(L"HTMLExportCD"));
	}
	else
	{
		sorttype = (int)Big3::Hitbase::Configuration::Settings::GetValue(L"HTMLOutputExportSortType", FIELD_TRACK_TITLE);
		CFieldList StandardFields;

		//m_pSelection->m_SortKeys[0]=FIELD_TRACK_ARTIST;
		//m_pSelection->m_SortKeys[1]=FIELD_CDNAME;
		//m_pSelection->m_bSortKeysDirection[0] = TRUE;		
		//m_pSelection->m_bSortKeysDirection[1] = TRUE;
		Condition->RestoreFromRegistry(gcnew String(L"HTMLExportTRACK"));
	}

    m_sSelection = (CString)Condition->GetConditionString(DataBase);
	m_sSortierung = (CString)SortFields->GetSortString(DataBase);

	UpdateData(FALSE);

	return TRUE;  // return TRUE unless you set the focus to a control
	// AUSNAHME: OCX-Eigenschaftenseite muss FALSE zurückgeben.
}

void CHTMLExportDlg::OnBnClickedHtmlTemplateBrowse()
{
	UpdateData();

	CNewFileDialog dlg(TRUE, L"html", m_sHTMLTemplate, OFN_HIDEREADONLY|OFN_OVERWRITEPROMPT,
		get_string(TEXT_FILEDIALOG_HTML));

	if (dlg.DoModal(L"HTMLOutputTemplate") == IDOK)
	{
		m_sHTMLTemplate = dlg.GetPathName();
		UpdateData(FALSE);
	}

	UpdateWindowState();
}

void CHTMLExportDlg::OnBnClickedHtmlOutputBrowse()
{
	UpdateData();

	CNewFileDialog dlg(FALSE, L"html", m_sHTMLOutput, OFN_HIDEREADONLY|OFN_OVERWRITEPROMPT,
		get_string(TEXT_FILEDIALOG_HTML));

	if (dlg.DoModal(L"HTMLOutputFile") == IDOK)
	{
		m_sHTMLOutput = dlg.GetPathName();
		UpdateData(FALSE);
	}
	
	UpdateWindowState();
}

void CHTMLExportDlg::OnBnClickedSelektion()
{
	Big3::Hitbase::CDUtilities::FormSearch formSearch(DataBase, Condition, false, true);

	if (formSearch.ShowDialog() == System::Windows::Forms::DialogResult::OK)
	{
		Condition = formSearch.GetCondition();
	}

	UpdateData();
    m_sSelection = (CString)Condition->GetConditionString(DataBase);
	UpdateData(FALSE);
}

void CHTMLExportDlg::OnBnClickedSortierung()
{
	if (m_dwExportType == IDC_EXPORTCDLISTE)
	{
		Big3::Hitbase::CDUtilities::FormSort formSort(DataBase, FieldType::CD, SortFields);

		if (formSort.ShowDialog() == System::Windows::Forms::DialogResult::OK)
		{
			SortFields = formSort.SortFields;
		}
	}

	if (m_dwExportType == IDC_EXPORTTITELLISTE)
	{
		Big3::Hitbase::CDUtilities::FormSort formSort(DataBase, FieldType::Track, SortFields);

		if (formSort.ShowDialog() == System::Windows::Forms::DialogResult::OK)
		{
			SortFields = formSort.SortFields;
		}
	}

	UpdateData();
    m_sSortierung = (CString)SortFields->GetSortString(DataBase);
	UpdateData(FALSE);
}

void CHTMLExportDlg::UpdateWindowState(void)
{
	UpdateData(TRUE);

	m_btnOK.EnableWindow(!m_sHTMLOutput.IsEmpty() && !m_sHTMLTemplate.IsEmpty());
}

void CHTMLExportDlg::OnEnChangeHtmlTemplate()
{
	UpdateWindowState();
}

void CHTMLExportDlg::OnEnChangeHtmlOutput()
{
	UpdateWindowState();
}

void CHTMLExportDlg::OnBnClickedExportTITELListe()
{
	Condition->RestoreFromRegistry(L"HTMLExportTRACK");
	m_dwExportType = IDC_EXPORTTITELLISTE;
	UpdateData();
    m_sSelection = Condition->GetConditionString(DataBase);
	m_sSortierung = SortFields->GetSortString(DataBase);
	UpdateData(FALSE);
}

void CHTMLExportDlg::OnBnClickedExportCDListe()
{
	Condition->RestoreFromRegistry(L"HTMLExportCD");
	UpdateData();
	m_sSelection = Condition->GetConditionString(DataBase);
	m_sSortierung = SortFields->GetSortString(DataBase);
	m_dwExportType = IDC_EXPORTCDLISTE;
	UpdateData(FALSE);
}

void CHTMLExportDlg::OnBnClickedOk()
{
	// TODO: Fügen Sie hier Ihren Kontrollbehandlungscode für die Benachrichtigung ein.
	int check = GetCheckedRadioButton(IDC_EXPORTCDLISTE, IDC_EXPORTTITELLISTE);
	CConfig::WriteGlobalRegistryKeyInt(L"HTMLOutputExportType", check);

	if (m_dwExportType == IDC_EXPORTCDLISTE)
		Condition->SaveToRegistry(L"HTMLExportCD");

	if (m_dwExportType == IDC_EXPORTTITELLISTE)
		Condition->SaveToRegistry(L"HTMLExportTRACK");

	OnOK();
}

void CHTMLExportDlg::OnBnClickedResethtmlfilter()
{
	if (m_dwExportType == IDC_EXPORTCDLISTE)
	{
		Condition->Clear();
	}


	if (m_dwExportType == IDC_EXPORTTITELLISTE)
	{
		Condition->Clear();
	}
	UpdateData();
    m_sSelection = Condition->GetConditionString(DataBase);
	m_sSortierung = SortFields->GetSortString(DataBase);

	UpdateData(FALSE);
}

