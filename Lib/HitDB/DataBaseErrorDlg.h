#if !defined(AFX_DATABASEERRORDLG_H__8B2A99A9_2630_11D1_A596_0000C055286C__INCLUDED_)
#define AFX_DATABASEERRORDLG_H__8B2A99A9_2630_11D1_A596_0000C055286C__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000
// DataBaseErrorDlg.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CDataBaseErrorDlg dialog

class CDataBaseErrorDlg : public CDialog
{
// Construction
public:
	CDataBaseErrorDlg(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CDataBaseErrorDlg)
	enum { IDD = IDD_HITDB_POSSIBLE_ERROR_IN_DATABASE };
	BOOL	m_bNoMoreErrors;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDataBaseErrorDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CDataBaseErrorDlg)
		// NOTE: the ClassWizard will add member functions here
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_DATABASEERRORDLG_H__8B2A99A9_2630_11D1_A596_0000C055286C__INCLUDED_)
