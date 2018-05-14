#ifndef __DUPRECORDDLG_H__
#define __DUPRECORDDLG_H__

#define DUP_RECORD_UPDATE		1
#define DUP_RECORD_UPDATEALL	2
#define DUP_RECORD_NOUPDATE		3
#define DUP_RECORD_NEVERUPDATE	4
#define DUP_RECORD_CANCEL		5
// DupRecordDlg.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CDupRecordDlg dialog

class CDupRecordDlg : public CDialog
{
// Construction
public:
	int m_AddRecord;
	CDupRecordDlg(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CDupRecordDlg)
	enum { IDD = IDD_HITDB_DUP_RECORD };
	CString	m_Artist;
	CString	m_Title;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDupRecordDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CDupRecordDlg)
	afx_msg void OnDupYes();
	afx_msg void OnDupYesall();
	afx_msg void OnDupNo();
	afx_msg void OnDupNoall();
	virtual void OnCancel();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // __DUPRECORDDLG_H__
