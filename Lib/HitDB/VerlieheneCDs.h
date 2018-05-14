// VerlieheneCDs.h: Schnittstelle f�r die Klasse CVerlieheneCDs.
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

// �berschreibungen
	// Vom Klassen-Assistenten generierte virtuelle Funktions�berschreibungen
	//{{AFX_VIRTUAL(CVerlieheneCDs)
	public:
	virtual CString GetDefaultSQL();		// Standard-SQL f�r Satzgruppe
	virtual void DoFieldExchange(CDaoFieldExchange* pFX);  // RFX-Unterst�tzung
	//}}AFX_VIRTUAL

// Implementierung
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif
};

