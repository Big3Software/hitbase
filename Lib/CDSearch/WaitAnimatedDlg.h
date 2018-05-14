#pragma once

// WaitAnimatedDlg.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CWaitAnimatedDlg dialog

class CHttpFileAsync;

#include "../../app/hitbase/resource.h"

class CWaitAnimatedDlg : public CDialog
{
// Construction
public:
	CWaitAnimatedDlg(const CString& text = L"", CWnd* pParent=NULL);

// Dialog Data
	//{{AFX_DATA(CWaitAnimatedDlg)
	enum { IDD = IDD_WAIT_ANIMATED };
	CAnimateCtrl	m_AnimateCtrl;
	CString	m_Text;
	CString	m_sStatus;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CWaitAnimatedDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CWaitAnimatedDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnDestroy();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

public:
	void SetText(const CString& sText);
	void SetHttpFileAsync(CHttpFileAsync* pHttpFileAsync);
	void SetStatus(const CString& sStatus);

protected:
	virtual void OnCancel();

private:
	CHttpFileAsync* m_pHttpFileAsync;
};

//{{AFX_INSERT_LOCATION}}
