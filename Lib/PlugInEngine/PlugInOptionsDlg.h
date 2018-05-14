#if !defined(AFX_PLUGINOPTIONSDLG_H__3A1B3AF3_F612_11D1_A431_000000000000__INCLUDED_)
#define AFX_PLUGINOPTIONSDLG_H__3A1B3AF3_F612_11D1_A431_000000000000__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000
// PlugInOptionsDlg.h : header file
//

#include "../gridctrl/hitgrid.h"

/////////////////////////////////////////////////////////////////////////////
// CPlugInOptionsDlg dialog

class CPlugIn;

class CPlugInOptionsDlg : public CDialog
{
// Construction
public:
	CPlugInOptionsDlg(CPlugIn* pPlugIn, CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CPlugInOptionsDlg)
	enum { IDD = IDD_PLUGINOPTIONS };
	CGridListCtrl	m_OptionsListCtrl;
	CString	m_sDescription;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CPlugInOptionsDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CPlugInOptionsDlg)
	virtual BOOL OnInitDialog();
	virtual void OnOK();
	afx_msg void OnItemchangedOptionslist(NMHDR* pNMHDR, LRESULT* pResult);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

private:
	CPlugIn* m_pPlugIn;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_PLUGINOPTIONSDLG_H__3A1B3AF3_F612_11D1_A431_000000000000__INCLUDED_)
