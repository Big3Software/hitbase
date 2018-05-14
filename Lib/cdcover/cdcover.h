// cdcover.h : main header file for the CDCOVER application
//



#if !defined(AFX_CDCOVER_H__910F52E3_FE6C_11D3_A4D5_0080AD834EB5__INCLUDED_)
#define AFX_CDCOVER_H__910F52E3_FE6C_11D3_A4D5_0080AD834EB5__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"       // main symbols

/////////////////////////////////////////////////////////////////////////////
// CCdcoverApp:
// See cdcover.cpp for the implementation of this class
//

class CCdcoverApp : public CWinApp
{
public:
	CCdcoverApp();
	gcroot<Big3::Hitbase::DataBaseEngine::CD^> m_ccd;
	gcroot<Big3::Hitbase::DataBaseEngine::DataBase^> m_db;

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CCdcoverApp)
	public:
	virtual BOOL InitInstance();
	virtual int ExitInstance();
	//}}AFX_VIRTUAL

// Implementation
	//{{AFX_MSG(CCdcoverApp)
	afx_msg void OnAppAbout();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_CDCOVER_H__910F52E3_FE6C_11D3_A4D5_0080AD834EB5__INCLUDED_)
