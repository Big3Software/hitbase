#pragma once

// Role.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CRole DAO recordset

class CRole : public CDBQuery
{
public:
	virtual void Copy(const CRole& theOther);
	CRole(CDataBase* pdb = NULL);
	DECLARE_DYNAMIC(CRole)

	// Tabellenname
	static const CString GetTableName() { return L"Role"; }

// Field/Param Data
	//{{AFX_FIELD(CRole, CDaoRecordset)
	long	m_dwID;
	CString	m_sRole;
	//}}AFX_FIELD

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CRole)
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
