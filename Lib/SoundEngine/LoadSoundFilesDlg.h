#if !defined(AFX_LOADSOUNDFILESDLG_H__8162D8C1_09F6_11D3_A6B3_0080AD740CD1__INCLUDED_)
#define AFX_LOADSOUNDFILESDLG_H__8162D8C1_09F6_11D3_A6B3_0080AD740CD1__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// LoadSoundFilesDlg.h : header file
//

#include "resource.h"

/////////////////////////////////////////////////////////////////////////////
// CLoadSoundFilesDlg dialog

class SOUNDENGINE_INTERFACE CLoadSoundFilesDlg : public CDialog
{
// Construction
public:
	CLoadSoundFilesDlg(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CLoadSoundFilesDlg)
	enum { IDD = IDD_LOAD_WAVES };
	CStatic	m_FilenameCtrl;
	CProgressCtrl	m_ProgressCtrl;
	CString	m_sFileName;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CLoadSoundFilesDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CLoadSoundFilesDlg)
	virtual void OnCancel();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

public:
	BOOL Create(CWnd* pParentWnd = NULL) 
	{
		return CDialog::Create(IDD, pParentWnd) ;
	}
	BOOL m_bCanceled;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_LOADSOUNDFILESDLG_H__8162D8C1_09F6_11D3_A6B3_0080AD740CD1__INCLUDED_)
