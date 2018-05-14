// CDSet.h: interface for the CCDSet class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_CDSET_H__C6E7CDA0_9A59_11D2_A5C5_0080AD740CD1__INCLUDED_)
#define AFX_CDSET_H__C6E7CDA0_9A59_11D2_A5C5_0080AD740CD1__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "DBQuery.h"

class HITDB_INTERFACE CCDSet : public CDBQuery
{
public:
	virtual BOOL ReadAllCDSets(CStringArray& saCDSets, CUIntArray& uiaCDSetsID);
	virtual void Copy(const CCDSet& theOther);
	CCDSet(CDataBase* pDatabase = NULL);
	~CCDSet();
	DECLARE_DYNAMIC(CCDSet)

	// Tabellenname
	static const CString GetTableName() { return L"CDSet"; }

// Field/Param Data
	//{{AFX_FIELD(CCDSet, CDaoRecordset)
	long	m_dwID;
	CString	m_sCDSetName;
	//}}AFX_FIELD

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CCDSet)
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

#endif // !defined(AFX_CDSET_H__C6E7CDA0_9A59_11D2_A5C5_0080AD740CD1__INCLUDED_)
