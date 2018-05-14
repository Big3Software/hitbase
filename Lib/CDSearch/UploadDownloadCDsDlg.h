#if !defined(AFX_UPLOADDOWNLOADCDSDLG_H__72463F2E_0EF0_421A_9865_F6F5C9C7006D__INCLUDED_)
#define AFX_UPLOADDOWNLOADCDSDLG_H__72463F2E_0EF0_421A_9865_F6F5C9C7006D__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// UploadDownloadCDsDlg.h : header file
//

#include "..\GridCtrl\hitGrid.h"
#include "cdarchive.h"

/////////////////////////////////////////////////////////////////////////////
// CUploadDownloadCDsDlg dialog

class CUploadDownloadCDsDlg : public CDialog
{
// Construction
public:
	CUploadDownloadCDsDlg(CWnd* pParent = NULL);   // standard constructor
	virtual BOOL Create(CWnd* pParentWnd = NULL);

// Dialog Data
	//{{AFX_DATA(CUploadDownloadCDsDlg)
	enum { IDD = IDD_UPLOAD_DOWNLOAD_CDS };
	CProgressCtrl	m_ProgressCtrl;
	CButton	m_CancelCtrl;
	CButton	m_StartCtrl;
	CButton	m_DetailsCtrl;
	CEdit	m_ErrorsCtrl;
	CGridListCtrl	m_ListCtrl;
	CString	m_sCurrentAction;
	CStatic m_stcCurrentAction;
	//}}AFX_DATA

	gcroot<CCDArchive^> CDArchives;

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CUploadDownloadCDsDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CUploadDownloadCDsDlg)
	afx_msg void OnDetails();
	virtual BOOL OnInitDialog();
	afx_msg void OnCdsearchCancel();
	afx_msg void OnStarted();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

public:
	virtual void AddDetailsString(const CString& sMessage);

private:
	BOOL m_bModeless;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_UPLOADDOWNLOADCDSDLG_H__72463F2E_0EF0_421A_9865_F6F5C9C7006D__INCLUDED_)
