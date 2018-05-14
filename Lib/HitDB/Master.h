#pragma once

// Master.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CMaster DAO recordset

#include "DBQuery.h"

class CMaster : public CDBQuery
{
public:
	CMaster(CDataBase* pDatabase = NULL);
	DECLARE_DYNAMIC(CMaster)

	// Tabellenname
	static const CString GetTableName() { return L"Master"; }

// Field/Param Data
	//{{AFX_FIELD(CMaster, CDaoRecordset)
	short	m_wVersion;
	CString	m_sNameDate;
	BYTE	m_cTypeDate;
	CString	m_sCDUserField[MAX_USER_FIELDS];
	CString	m_sTrackUserField[MAX_USER_FIELDS];
	BYTE	m_cTypeCDUserField[MAX_USER_FIELDS];
	BYTE	m_cTypeTrackUserField[MAX_USER_FIELDS];
	//}}AFX_FIELD

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CMaster)
	public:
	virtual CString GetDefaultSQL();		// Default SQL for Recordset
	virtual void DoFieldExchange(CDaoFieldExchange* pFX);  // RFX support
	//}}AFX_VIRTUAL

// Implementation
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

public:
	void operator =(const CMaster& theOther);
};

