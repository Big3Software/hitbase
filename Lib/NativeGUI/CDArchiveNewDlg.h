#if !defined(AFX_CDARCHIVENEWDLG_H__E9C4D164_E71A_11D1_A417_000000000000__INCLUDED_)
#define AFX_CDARCHIVENEWDLG_H__E9C4D164_E71A_11D1_A417_000000000000__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000
// CDArchiveNewDlg.h : header file
//
/////////////////////////////////////////////////////////////////////////////
// CCDArchiveNewDlg dialog

class CCDArchiveNewDlg : public CDialog
{
// Construction
public:
	CCDArchiveNewDlg(BOOL bEdit, CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CCDArchiveNewDlg)
	enum { IDD = IDD_ADD_CDARCHIVE };

	CComboBox	m_SourceCtrl;
	CComboBox	m_TypeCtrl;
	CButton	m_OKCtrl;
	CButton	m_BrowseCtrl;
	BOOL	m_bAutoSearch;
	CString	m_sSource;
	int		m_nType;
	BOOL	m_bActive;
	BOOL	m_bUpload;
	BOOL	m_bAutoCreateSampler;
	CString	m_sSamplerTrennzeichen;
	CButton	m_UploadCtrl;
	CButton m_CreateSamplerCtrl;
	CEdit	m_SamplerTrennzeichenCtrl;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CCDArchiveNewDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CCDArchiveNewDlg)
	afx_msg void OnBrowse();
	afx_msg void OnSelchangeType();
	virtual BOOL OnInitDialog();
	afx_msg void OnChangeEdit1();
	afx_msg void OnSelchangeCombo1();
	afx_msg void OnCreateSampler();
	afx_msg void OnOK();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
private:
	void FillArchiveList();
	void UpdateWindowState();
	BOOL m_bEdit;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_CDARCHIVENEWDLG_H__E9C4D164_E71A_11D1_A417_000000000000__INCLUDED_)
