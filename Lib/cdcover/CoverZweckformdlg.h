#if !defined(AFX_CoverZweckformdlg_H__79A1C13E_8F6B_4DB5_A9CE_ACA54EFD41AC__INCLUDED_)
#define AFX_CoverZweckformdlg_H__79A1C13E_8F6B_4DB5_A9CE_ACA54EFD41AC__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// CoverZweckformdlg.h : header file
//


#include "coverlayout.h"

/////////////////////////////////////////////////////////////////////////////
// CCoverZweckformDlg dialog

class CCoverZweckformDlg : public CDialog
{
// Construction

	public:
		
		CCoverZweckformDlg(CCoverLayout *pLayout, CWnd* pParent = NULL);   // standard constructor

	public:

		CCoverLayout *m_pLayout;

		int m_nFrontX;
		int m_nFrontY;
		int m_nBackX;
		int m_nBackY;
		int m_nLabelX;
		int m_nLabelY;


	public:

		void InitEditBoxes();
		int TransformValue(const CString& strValue);
		void ValidateValues();
		void SetValues();


// Dialog Data
	//{{AFX_DATA(CCoverZweckformDlg)
	enum { IDD = IDD_DIALOG_COVERZWECKFORM };
	CEdit	m_editRandOben;
	CEdit	m_editRandLinks;
	CComboBox	m_comboNummer;
	CComboBox	m_comboMarke;
	CEdit	m_editLabelY;
	CEdit	m_editLabelX;
	CEdit	m_editFrontY;
	CEdit	m_editFrontX;
	CEdit	m_editBackY;
	CEdit	m_editBackX;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CCoverZweckformDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CCoverZweckformDlg)
	virtual BOOL OnInitDialog();
	virtual void OnOK();
	afx_msg void OnSelchangeComboMarke();
	afx_msg void OnSelchangeComboNummer();
	afx_msg void OnKillfocusEdit();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_CoverZweckformdlg_H__79A1C13E_8F6B_4DB5_A9CE_ACA54EFD41AC__INCLUDED_)
