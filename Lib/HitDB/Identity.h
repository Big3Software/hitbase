#if !defined(AFX_IDENTITY_H__61C12BB4_CFE6_11D2_A642_0080AD740CD1__INCLUDED_)
#define AFX_IDENTITY_H__61C12BB4_CFE6_11D2_A642_0080AD740CD1__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// Identity.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CIdentity DAO recordset

class CIdentity : public CDBQuery
{
public:
	virtual void Copy(const CIdentity& theOther);
	virtual BOOL Write();
	CIdentity(CDataBase* pDatabase = NULL);
	CIdentity(const CIdentity& theOther);
	~CIdentity();
	void operator =(const CIdentity& theOther);
	DECLARE_DYNAMIC(CIdentity)

	// Tabellenname
	static const CString GetTableName() { return L"Identity"; }

// Field/Param Data
	//{{AFX_FIELD(CIdentity, CDaoRecordset)
	CString	m_sIdentity;
	long	m_dwIDCD;
	//}}AFX_FIELD

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CIdentity)
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

#endif // !defined(AFX_IDENTITY_H__61C12BB4_CFE6_11D2_A642_0080AD740CD1__INCLUDED_)
