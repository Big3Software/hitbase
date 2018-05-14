// CDCoverFrame.h : interface of the CDCoverFrame class
//
/////////////////////////////////////////////////////////////////////////////

#if !defined(AFX_MAINFRM_H__910F52E7_FE6C_11D3_A4D5_0080AD834EB5__INCLUDED_)
#define AFX_MAINFRM_H__910F52E7_FE6C_11D3_A4D5_0080AD834EB5__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "CoverLayout.h"
using namespace Big3::Hitbase::DataBaseEngine;

class CDCOVER_INTERFACE CCDCoverFrame : public CFrameWnd
//class /*AFX_EXT_CLASS*/ CCDCoverFrame : public CFrameWnd
{

public: // create from serialization only
	CCDCoverFrame();
	DECLARE_DYNCREATE(CCDCoverFrame)

// Operations
public:

	CCoverLayout m_Layout;
	CCoverLayout m_oldLayout;

	gcroot<CD^> cd;
	gcroot<DataBase^> db;

	void BindDBToLayout(const BOOL bLoadData);
	static CString GetPurePathName(const CString& strFullPathName);
	static CString GetPureFileName(const CString& strPath);
	BOOL LoadFrame();

	private:

		void RedrawAll();

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CCDCoverFrame)
	public:
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
	virtual void ActivateFrame(int nCmdShow = -1);
	//}}AFX_VIRTUAL

// Implementation
public:
	virtual ~CCDCoverFrame();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:  // control bar embedded members
	CStatusBar  m_wndStatusBar;
	CToolBar    m_wndToolBar;
	CReBar      m_wndReBar;
//	CDialogBar      m_wndDlgBar;

// Generated message map functions
protected:
	//{{AFX_MSG(CCDCoverFrame)
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	afx_msg void OnToolbarButton();
	afx_msg void OnFileNew();
	afx_msg void OnFileOpen();
	afx_msg void OnFileSave();
	afx_msg void OnFileSaveAs();
	afx_msg void OnMyQuit();
	afx_msg void MySetRadio(CCmdUI* pCmdUI);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_MAINFRM_H__910F52E7_FE6C_11D3_A4D5_0080AD834EB5__INCLUDED_)
