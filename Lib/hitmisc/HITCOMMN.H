// hitcommon.h : header file
//

#pragma once

/////////////////////////////////////////////////////////////////////////////
// CCommonCheckDlg dialog

class HITMISC_INTERFACE CCommonCheckDlg : public CDialog
{
// Construction
public:
	CString m_Message;
	CCommonCheckDlg(UINT nIDTemplate, const CString& message, CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CCommonCheckDlg)
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CCommonCheckDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	virtual BOOL OnCommand(WPARAM wParam, LPARAM lParam);
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CCommonCheckDlg)
	virtual BOOL OnInitDialog();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

public:
   int m_Check1;
};

