#if !defined(AFX_OPTIONSPLUGINSPAGE_H__756D71C3_F59C_11D1_A430_000000000000__INCLUDED_)
#define AFX_OPTIONSPLUGINSPAGE_H__756D71C3_F59C_11D1_A430_000000000000__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000
// OptionsPlugInsPage.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// COptionsPlugInsPage dialog

class COptionsPlugInsPage : public CPropertyPage
{
	DECLARE_DYNCREATE(COptionsPlugInsPage)

// Construction
public:
	COptionsPlugInsPage();
	~COptionsPlugInsPage();

// Dialog Data
	//{{AFX_DATA(COptionsPlugInsPage)
	enum { IDD = DLG_OPTIONS_PLUGINS };
	CComboBox	m_ModuleCtrl;
	CComboBox	m_LibraryCtrl;
	CString	m_sPlugInDirectory;
	CString	m_sVersion;
	CString	m_sCopyright;
	CString	m_sURL;
	CString	m_sComment;
	//}}AFX_DATA


// Overrides
	// ClassWizard generate virtual function overrides
	//{{AFX_VIRTUAL(COptionsPlugInsPage)
	public:
	virtual void OnOK();
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	// Generated message map functions
	//{{AFX_MSG(COptionsPlugInsPage)
	afx_msg void OnReloadPlugins();
	virtual BOOL OnInitDialog();
	afx_msg void OnSelchangeLibrary();
	afx_msg void OnSelchangeModule();
	afx_msg void OnOptions();
	afx_msg void OnBrowse();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

private:
	void ShowPlugInInformation(int nPlugInIndex);
	void FillPlugIns();
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_OPTIONSPLUGINSPAGE_H__756D71C3_F59C_11D1_A430_000000000000__INCLUDED_)
