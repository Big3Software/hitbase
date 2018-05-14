// HitExportTextSheet.cpp : implementation file
//

#include "stdafx.h"
#include "HitExpSh.h"
#include "../hitmisc/HitbaseWinAppBase.h"
#include "../../app/hitbase/resource.h"

#ifdef _DEBUG
#undef THIS_FILE
static char BASED_CODE THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CHitExportTextSheet

IMPLEMENT_DYNAMIC(CHitExportTextSheet, CPropertySheet)

CHitExportTextSheet::CHitExportTextSheet(Big3::Hitbase::DataBaseEngine::DataBase^ db, CWnd* pWndParent)
	 : CPropertySheet(IDS_PROPSHT_CAPTION, pWndParent)
{
	// Add all of the property pages here.  Note that
	// the order that they appear in here will be
	// the order they appear in on screen.  By default,
	// the first page of the set is the active one.
	// One way to make a different property page the 
	// active one is to call SetActivePage().

    this->DataBase = db;

	AddPage(&m_Page1);
	AddPage(&m_Page2);
	AddPage(&m_Page3);
	AddPage(&m_Page4);
	AddPage(&m_Page5);

	m_Page1.ExportTextSheet = this;
	m_Page2.ExportTextSheet = this;
	m_Page3.ExportTextSheet = this;
	m_Page4.ExportTextSheet = this;
	m_Page5.ExportTextSheet = this;

	SetWizardMode();
}

CHitExportTextSheet::~CHitExportTextSheet()
{
}


BEGIN_MESSAGE_MAP(CHitExportTextSheet, CPropertySheet)
	//{{AFX_MSG_MAP(CHitExportTextSheet)
	ON_WM_CREATE()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()


/////////////////////////////////////////////////////////////////////////////
// CHitExportTextSheet message handlers



int CHitExportTextSheet::OnCreate(LPCREATESTRUCT lpCreateStruct) 
{
	if (CPropertySheet::OnCreate(lpCreateStruct) == -1)
		return -1;
	
	// TODO: Add your specialized creation code here
	SetWizardButtons (PSWIZB_NEXT|PSWIZB_FINISH);
	
	return 0;
}
