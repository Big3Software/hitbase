#if !defined(AFX_MULTIPLECDSFOUND_H__E9C4D163_E71A_11D1_A417_000000000000__INCLUDED_)
#define AFX_MULTIPLECDSFOUND_H__E9C4D163_E71A_11D1_A417_000000000000__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000
// MultipleCDsFound.h : header file
//

#include "../GridCtrl/hitgrid.h"

/////////////////////////////////////////////////////////////////////////////
// CMultipleCDsFound dialog

using namespace System::Collections::Generic;

class CMultipleCDsFound : public CDialog
{
// Construction
public:
	CMultipleCDsFound(List<Big3::Hitbase::DataBaseEngine::CD^>^ cdarray, CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CMultipleCDsFound)
	enum { IDD = IDD_MULTIPLE_CDS_FOUND };
	CButton	m_OKCtrl;
	CGridListCtrl	m_ListCtrl;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CMultipleCDsFound)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CMultipleCDsFound)
	virtual BOOL OnInitDialog();
	virtual void OnOK();
	afx_msg void OnItemchangedList(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnDblclkList(NMHDR* pNMHDR, LRESULT* pResult);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

private:
	void UpdateWindowState();
	gcroot<List<Big3::Hitbase::DataBaseEngine::CD^>^> CDArray;

public:
	int m_nSelected;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_MULTIPLECDSFOUND_H__E9C4D163_E71A_11D1_A417_000000000000__INCLUDED_)
