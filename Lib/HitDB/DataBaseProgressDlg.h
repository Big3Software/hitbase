// ChangeCodesProgressDlg.h : header file
//

#ifndef _CLASS_DATABASEPROGRESSDLG
#define _CLASS_DATABASEPROGRESSDLG
#include "afxwin.h"

class CDBQuery;

using namespace Big3::Hitbase::DataBaseEngine;

/////////////////////////////////////////////////////////////////////////////
// CDataBaseProgressDlg dialog

class HITDB_INTERFACE CDataBaseProgressDlg : public CDialog
{
// Construction
public:
	CDataBaseProgressDlg(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CDataBaseProgressDlg)
	enum { IDD = IDD_HITDB_DATABASE_PROGRESS };
	CAnimateCtrl	m_Animate1Ctrl;
	CProgressCtrl	m_ProgressCtrl;
	CString	m_ProgressText;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDataBaseProgressDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CDataBaseProgressDlg)
	virtual void OnCancel();
	virtual BOOL OnInitDialog();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

public:
	gcroot<DataBase^> DataBase;

	CDBQuery* m_pQuery;
	CCDQuery* m_pCDQuery;

	CStatic m_stcStatus;
};

#endif
