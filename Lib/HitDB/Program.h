#if !defined(AFX_PROGRAM_H__B0CAB3E1_EA92_11D2_A67C_0080AD740CD1__INCLUDED_)
#define AFX_PROGRAM_H__B0CAB3E1_EA92_11D2_A67C_0080AD740CD1__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// Program.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CProgram DAO recordset

class CProgram : public CDBQuery
{
public:
	virtual void Copy(const CProgram& theOther);
	virtual BOOL Add();
	CProgram(CDataBase* pDatabase = NULL);
	CProgram(const CProgram& theOther);
	CProgram& operator =(const CProgram& theOther);
	DECLARE_DYNAMIC(CProgram)

	// Tabellenname
	static const CString GetTableName() { return L"Programme"; }

// Field/Param Data
	//{{AFX_FIELD(CProgram, CDaoRecordset)
	long	m_dwIDCD;
	CString	m_sName;
	CString	m_sLieder;
	BOOL	m_bStandard;
	//}}AFX_FIELD

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CProgram)
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

// Program-Liste für CD
class HITDB_INTERFACE CProgramList : public CArray<CProgram, CProgram&>
{
public:
	virtual void Copy(const CProgramList& theOther);
	virtual BOOL Save();
	virtual BOOL AddNew(const CString& sName, const CString& sTracks);
	virtual BOOL Delete(int iIndex);
	virtual int GetDefault();
	virtual void SetDefault(int iIndex);
	CProgramList(CDataBase* pdb, long dwIDCD) { m_pdb = pdb; m_dwIDCD = dwIDCD; }

	long m_dwIDCD;                  // ID, zu der die Programme gehören

protected:
	CDataBase* m_pdb;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_PROGRAM_H__B0CAB3E1_EA92_11D2_A67C_0080AD740CD1__INCLUDED_)
