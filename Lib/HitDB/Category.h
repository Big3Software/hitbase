#if !defined(AFX_CATEGORY_H__61C12BB5_CFE6_11D2_A642_0080AD740CD1__INCLUDED_)
#define AFX_CATEGORY_H__61C12BB5_CFE6_11D2_A642_0080AD740CD1__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// Category.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CCategory DAO recordset

class CCategory : public CDBQuery
{
public:
	virtual void Copy(const CCategory& theOther);
	CCategory(CDataBase* pDatabase = NULL);
	DECLARE_DYNAMIC(CCategory)

	// Tabellenname
	static const CString GetTableName() { return L"Kategorie"; }

// Field/Param Data
	//{{AFX_FIELD(CCategory, CDaoRecordset)
	long	m_dwID;
	CString	m_sCategory;
	short	m_wOrder;
	//}}AFX_FIELD

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CCategory)
	public:
	virtual CString GetDefaultSQL();		// Default SQL for Recordset
	virtual void DoFieldExchange(CDaoFieldExchange* pFX);  // RFX support
	//}}AFX_VIRTUAL

// Implementation
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_CATEGORY_H__61C12BB5_CFE6_11D2_A642_0080AD740CD1__INCLUDED_)
