// ChooseFieldsDlg.h : header file
//

#ifndef _CLASS_CCHOOSEFIELDSDLG
#define _CLASS_CCHOOSEFIELDSDLG

#define MAX_CF_FIELDS        50

/////////////////////////////////////////////////////////////////////////////
// CChooseFieldsDlg dialog

#include "../HitMisc/FieldList.h"
#include "resource.h"

class CDataBase;

class HITDB_INTERFACE CChooseFieldsDlg : public CDialog
{
// Construction
public:
	CChooseFieldsDlg(CDataBase* pdb, CWnd* pParent = NULL);   // standard constructor

    CFieldList m_FieldList;
	void SetFields(const CFieldList& FieldList);
	void SetDefault(const CFieldList& FieldList);

// Dialog Data
	//{{AFX_DATA(CChooseFieldsDlg)
	enum { IDD = IDD_HITDB_CHOOSE_FIELDS };
	CButton	m_UpCtrl;
	CButton	m_DownCtrl;
	CButton	m_AddCtrl;
	CButton	m_RemoveCtrl;
	CListBox	m_SelectedCtrl;
	CListBox	m_AvailableCtrl;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CChooseFieldsDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CChooseFieldsDlg)
	afx_msg void OnStandard();
	afx_msg void OnUp();
	afx_msg void OnDown();
	afx_msg void OnAdd();
	afx_msg void OnRemove();
	virtual BOOL OnInitDialog();
	afx_msg void OnSelchangeList1();
	afx_msg void OnSelchangeList2();
	afx_msg void OnDblclkList1();
	afx_msg void OnDblclkList2();
	virtual void OnOK();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

private:
	void AddField(UINT uiField);
	void FillListboxes();
	void UpdateWindowState();
	
	CFont hfontwing;
	
	CFieldList m_AllFields;
	CFieldList m_StandardFields;
	DWORD m_dwFlags;

	CDataBase* m_pdb;
};

#endif
