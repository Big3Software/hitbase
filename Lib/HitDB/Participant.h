#pragma once

// Participant.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CParticipant DAO recordset

class HITDB_INTERFACE CParticipant : public CDBQuery
{
public:
	CParticipant(CDataBase* pdb = NULL);
	CParticipant(const CParticipant& theOther);
	virtual void Copy(const CParticipant& theOther);
	DECLARE_DYNAMIC(CParticipant)

	CParticipant & operator =(const CParticipant &theOther);

	// Tabellenname
	static const CString GetTableName() { return L"Participant"; }

// Field/Param Data
	//{{AFX_FIELD(CParticipant, CDaoRecordset)
	long	m_dwIDCD;
	long 	m_dwIDRole;
	long	m_dwIDArtist;
	long	m_dwTrackNumber;
	CString m_sComment;
	//}}AFX_FIELD

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CParticipant)
	public:
	virtual CString GetDefaultSQL();		// Default SQL for Recordset
	virtual void DoFieldExchange(CDaoFieldExchange* pFX);  // RFX support
	//}}AFX_VIRTUAL

// Implementation
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

	BOOL Add();
};

// Mitwirkende-Liste für CD
class HITDB_INTERFACE CParticipantList : public CArray<CParticipant, CParticipant&>
{
public:
//	virtual void Copy(const CParticipantList& theOther);
	CParticipantList(CDataBase* pdb, long dwIDCD);

	long m_dwIDCD;                  // ID, zu der die Mitwirkenden gehören

	virtual BOOL AddAll();
	virtual BOOL Delete(int iIndex);
	virtual BOOL AddNew(long dwIDRole, long dwIDArtist, long dwTrackNumber, const CString& sComment);

protected:
	CDataBase* m_pdb;
};


//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.
