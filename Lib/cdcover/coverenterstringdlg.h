#if !defined(AFX_COVERENTERSTRINGDLG_H__862EB8A5_098B_11D4_A4D6_0080AD834EB5__INCLUDED_)
#define AFX_COVERENTERSTRINGDLG_H__862EB8A5_098B_11D4_A4D6_0080AD834EB5__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// CoverEnterStringDlg.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CCoverEnterStringDlg dialog

class CCoverEnterStringDlg : public CDialog
{

// Construction
public:
	CCoverEnterStringDlg(CString *pstrText, CWnd* pParent);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CCoverEnterStringDlg)
	enum { IDD = IDD_DIALOG_ENTERSTRING };
	CString	m_strText;
	//}}AFX_DATA


	private:

		CString *m_pString;
		BOOL m_bOnce;


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CCoverEnterStringDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CCoverEnterStringDlg)
	virtual void OnOK();
	virtual BOOL OnInitDialog();
	afx_msg void OnPaint();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_COVERENTERSTRINGDLG_H__862EB8A5_098B_11D4_A4D6_0080AD834EB5__INCLUDED_)
