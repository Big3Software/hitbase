#if !defined(AFX_MEDIUM_H__61C12BB6_CFE6_11D2_A642_0080AD740CD1__INCLUDED_)
#define AFX_MEDIUM_H__61C12BB6_CFE6_11D2_A642_0080AD740CD1__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// Medium.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CMedium DAO recordset

class CMedium : public CDBQuery
{
public:
	virtual void Copy(const CMedium& theOther);
	CMedium(CDataBase* pdb = NULL);
	DECLARE_DYNAMIC(CMedium)

	// Tabellenname
	static const CString GetTableName() { return L"Medium"; }

// Field/Param Data
	//{{AFX_FIELD(CMedium, CDaoRecordset)
	long	m_dwID;
	CString	m_sMedium;
	short	m_wOrder;
	//}}AFX_FIELD

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CMedium)
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

#endif // !defined(AFX_MEDIUM_H__61C12BB6_CFE6_11D2_A642_0080AD740CD1__INCLUDED_)
