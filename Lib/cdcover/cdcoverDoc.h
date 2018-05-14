// cdcoverDoc.h : interface of the CCdcoverDoc class
//
/////////////////////////////////////////////////////////////////////////////

#if !defined(AFX_CDCOVERDOC_H__910F52E9_FE6C_11D3_A4D5_0080AD834EB5__INCLUDED_)
#define AFX_CDCOVERDOC_H__910F52E9_FE6C_11D3_A4D5_0080AD834EB5__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

class CDCOVER_INTERFACE CCdcoverDoc : public CDocument
{
protected: // create from serialization only
	CCdcoverDoc();
	DECLARE_DYNCREATE(CCdcoverDoc)

// Operations
public:


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CCdcoverDoc)
	public:
	virtual void Serialize(CArchive& ar);
	virtual BOOL OnNewDocument();
	//}}AFX_VIRTUAL

// Implementation
public:
	virtual ~CCdcoverDoc();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:

// Generated message map functions
protected:
	//{{AFX_MSG(CCdcoverDoc)
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_CDCOVERDOC_H__910F52E9_FE6C_11D3_A4D5_0080AD834EB5__INCLUDED_)
