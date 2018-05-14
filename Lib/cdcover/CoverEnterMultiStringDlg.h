#if !defined(AFX_COVERENTERMULTISTRINGDLG_H__96FAA310_ADFF_47DE_B1F9_BA8ACF60AA0A__INCLUDED_)
#define AFX_COVERENTERMULTISTRINGDLG_H__96FAA310_ADFF_47DE_B1F9_BA8ACF60AA0A__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// CoverEnterMultiStringDlg.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CCoverEnterMultiStringDlg dialog

class CCoverEnterMultiStringDlg : public CDialog
{
// Construction
public:
	CCoverEnterMultiStringDlg(CString *pstrText, CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CCoverEnterMultiStringDlg)
	enum { IDD = IDD_DIALOG_ENTERMULTISTRING };
	CString	m_strText;
	//}}AFX_DATA

	private:

		CString *m_pString;
		BOOL m_bOnce;

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CCoverEnterMultiStringDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CCoverEnterMultiStringDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnPaint();
	virtual void OnOK();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_COVERENTERMULTISTRINGDLG_H__96FAA310_ADFF_47DE_B1F9_BA8ACF60AA0A__INCLUDED_)
