#if !defined(AFX_COVERFREEVALUEDLG_H__40CF5ED2_706D_454B_9493_9E84E4332027__INCLUDED_)
#define AFX_COVERFREEVALUEDLG_H__40CF5ED2_706D_454B_9493_9E84E4332027__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// CoverFreeValueDlg.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CCoverFreeValueDlg dialog

class CCoverFreeValueDlg : public CDialog
{
// Construction
public:
	CCoverFreeValueDlg(int *pValue, CWnd* pParent);   // standard constructor

	int *m_pValue;

// Dialog Data
	//{{AFX_DATA(CCoverFreeValueDlg)
	enum { IDD = IDD_DIALOG_FREEVALUE };
	int		m_nValue;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CCoverFreeValueDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CCoverFreeValueDlg)
	virtual void OnOK();
	virtual BOOL OnInitDialog();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_COVERFREEVALUEDLG_H__40CF5ED2_706D_454B_9493_9E84E4332027__INCLUDED_)
