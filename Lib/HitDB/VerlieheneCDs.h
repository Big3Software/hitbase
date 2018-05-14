// VerlieheneCDs.h: Schnittstelle für die Klasse CVerlieheneCDs.
//
//////////////////////////////////////////////////////////////////////

#pragma once

/////////////////////////////////////////////////////////////////////////////
// DAO Satzgruppe CVerlieheneCDs 

#include "DBQuery.h"

class HITDB_INTERFACE CVerlieheneCDs : public CDBQuery
{
public:
	CVerlieheneCDs(CDataBase* pdb = NULL);
	DECLARE_DYNAMIC(CVerlieheneCDs)

	// Tabellenname
	static const CString GetTableName() { return L"VerlieheneCDs"; }

// Feld-/Parameterdaten
	//{{AFX_FIELD(CVerlieheneCDs, CDaoRecordset)
	long	m_dwID;
	COleDateTime	m_dtVerliehenAm;
	CString	m_sVerliehenAn;
	COleDateTime	m_dtRueckgabeTermin;
	CString	m_sKommentar;
	//}}AFX_FIELD

// Überschreibungen
	// Vom Klassen-Assistenten generierte virtuelle Funktionsüberschreibungen
	//{{AFX_VIRTUAL(CVerlieheneCDs)
	public:
	virtual CString GetDefaultSQL();		// Standard-SQL für Satzgruppe
	virtual void DoFieldExchange(CDaoFieldExchange* pFX);  // RFX-Unterstützung
	//}}AFX_VIRTUAL

// Implementierung
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif
};

