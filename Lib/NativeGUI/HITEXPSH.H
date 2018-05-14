// HitExportTextSheet.h : header file
//
// This class defines custom modal property sheet 
// CHitExportTextSheet.
 
#pragma once

#include "HitExpPa.h"

/////////////////////////////////////////////////////////////////////////////
// CHitExportTextSheet

class CHitbaseDoc;

class CHitExportTextSheet : public CPropertySheet
{
	DECLARE_DYNAMIC(CHitExportTextSheet)

// Construction
public:
	CHitExportTextSheet(Big3::Hitbase::DataBaseEngine::DataBase^ db, CWnd* pParentWnd = NULL);

// Attributes
public:
	CHitExportTextPage1 m_Page1;
	CHitExportTextPage2 m_Page2;
	CHitExportTextPage3 m_Page3;
	CHitExportTextPage4 m_Page4;
	CHitExportTextPage5 m_Page5;

// Operations
public:

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CHitExportTextSheet)
	//}}AFX_VIRTUAL

// Implementation
public:
	virtual ~CHitExportTextSheet();

// Generated message map functions
protected:
	//{{AFX_MSG(CHitExportTextSheet)
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

public:
	gcroot<Big3::Hitbase::DataBaseEngine::DataBase^> DataBase;
};

/////////////////////////////////////////////////////////////////////////////

namespace Big3
{
	namespace Hitbase
	{
		namespace NativeGUI
		{
			public ref class TXTExportDialog
			{
			public:
				TXTExportDialog::TXTExportDialog()
				{
				}

			public:
				void Show(Big3::Hitbase::DataBaseEngine::DataBase^ db)
				{
					AFX_MANAGE_STATE(AfxGetStaticModuleState());

					::CHitExportTextSheet ExportSheet(db, NULL);
	
					ExportSheet.DoModal();
				};
			};
		}
	}
}



