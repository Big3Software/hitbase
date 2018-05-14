#if !defined(AFX_INDEX_H__B0CAB3E2_EA92_11D2_A67C_0080AD740CD1__INCLUDED_)
#define AFX_INDEX_H__B0CAB3E2_EA92_11D2_A67C_0080AD740CD1__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// Index.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CIndex DAO recordset

class HITDB_INTERFACE CIndex : public CDBQuery
{
public:
	CIndex(CDataBase* pdb = NULL);
	CIndex(const CIndex& theOther);
	CIndex& operator =(const CIndex& theOther);

	DECLARE_DYNAMIC(CIndex)

	// Tabellenname
	static const CString GetTableName() { return L"Index"; }

// Field/Param Data
	//{{AFX_FIELD(CIndex, CDaoRecordset)
	long	m_dwIDCD;
	CString	m_sIndexName;
	long	m_dwPosition;
	//}}AFX_FIELD

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CIndex)
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
	virtual void Copy(const CIndex& theOther);
	BOOL Add();
};

// Index-Liste für CD
class HITDB_INTERFACE CIndexList : public CArray<CIndex, CIndex&>
{
public:
	virtual void Copy(const CIndexList& theOther);
	CIndexList(CDataBase* pdb, long dwIDCD) { m_pdb = pdb; m_dwIDCD = dwIDCD; }

	long m_dwIDCD;                  // ID, zu der die Lieder gehören

	virtual BOOL Save();
	virtual BOOL Delete(int iIndex);
	virtual BOOL AddNew(const CString& sName, DWORD dwPosition);

protected:
	CDataBase* m_pdb;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_INDEX_H__B0CAB3E2_EA92_11D2_A67C_0080AD740CD1__INCLUDED_)
