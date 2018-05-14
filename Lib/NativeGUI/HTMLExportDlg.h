#pragma once
#include "afxwin.h"
#include "HTMLTemplate.h"


// CHTMLExportDlg-Dialogfeld

class CSelection;
class CDataBase;

class CHTMLExportDlg : public CDialog
{
	DECLARE_DYNAMIC(CHTMLExportDlg)

public:
	CHTMLExportDlg(Big3::Hitbase::DataBaseEngine::DataBase^ db, CWnd* pParent = NULL);   // Standardkonstruktor
	virtual ~CHTMLExportDlg();

// Dialogfelddaten
	enum { IDD = IDD_HTML_EXPORT };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV-Unterstützung

	DECLARE_MESSAGE_MAP()
public:
	CString m_sHTMLTemplate;
	CString m_sHTMLOutput;
	afx_msg void OnBnClickedHtmlTemplateBrowse();
	afx_msg void OnBnClickedHtmlOutputBrowse();
	afx_msg void OnBnClickedSelektion();
	afx_msg void OnBnClickedSortierung();
	CString m_sSelection;
	CString m_sSortierung;
	DWORD m_dwExportType;

	gcroot<Big3::Hitbase::DataBaseEngine::Condition^> Condition;
	gcroot<Big3::Hitbase::DataBaseEngine::SortFieldCollection^> SortFields;

private:
	gcroot<Big3::Hitbase::DataBaseEngine::DataBase^> DataBase;

public:
	virtual BOOL OnInitDialog();
	BOOL m_bShowFile;
	void UpdateWindowState(void);
	CButton m_btnOK;
	afx_msg void OnEnChangeHtmlTemplate();
	afx_msg void OnEnChangeHtmlOutput();
	afx_msg void OnBnClickedExportTITELListe();
	afx_msg void OnBnClickedExportCDListe();
	afx_msg void OnBnClickedOk();
	afx_msg void OnBnClickedButton5();
	afx_msg void OnBnClickedButton7();
	afx_msg void OnBnClickedButton8();
	afx_msg void OnBnClickedResethtmlfilter();
	afx_msg void OnBnClickedResethtmlsort();
};
namespace Big3
{
	namespace Hitbase
	{
		namespace NativeGUI
		{
			public ref class HTMLExportDialog
			{
			public:
				HTMLExportDialog::HTMLExportDialog()
				{
				}

			public:
				void Show(Big3::Hitbase::DataBaseEngine::DataBase^ db)
				{
					AFX_MANAGE_STATE(AfxGetStaticModuleState());

					CHTMLExportDlg dlgExport(db);

					dlgExport.m_sHTMLTemplate = CConfig::ReadGlobalRegistryKeyString(L"HTMLTemplateFilename", (CString)Big3::Hitbase::Miscellaneous::Misc::GetApplicationPath() + L"\\Templates\\cd short list.html");
					dlgExport.m_sHTMLOutput = CConfig::ReadGlobalRegistryKeyString(L"HTMLOutputFilename", L"");
					dlgExport.m_bShowFile = CConfig::ReadGlobalRegistryKeyInt(L"HTMLOutputShowFile", TRUE);

					if (dlgExport.DoModal() == IDOK)
					{
						CConfig::WriteGlobalRegistryKeyString(L"HTMLTemplateFilename", dlgExport.m_sHTMLTemplate);
						CConfig::WriteGlobalRegistryKeyString(L"HTMLOutputFilename", dlgExport.m_sHTMLOutput);
						CConfig::WriteGlobalRegistryKeyInt(L"HTMLOutputShowFile", dlgExport.m_bShowFile);

						::CHTMLTemplate ht(db);
						if (ht.DoExport(dlgExport.Condition, dlgExport.SortFields, dlgExport.m_sHTMLTemplate, dlgExport.m_sHTMLOutput))
							ShellExecute(*AfxGetMainWnd(), NULL, dlgExport.m_sHTMLOutput, NULL, L"", SW_SHOWNORMAL);
					}
				};
			};
		}
	}
}

