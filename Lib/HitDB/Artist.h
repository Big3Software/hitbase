// Artist.h: interface for the CArtist class.
//
//////////////////////////////////////////////////////////////////////

#pragma once

#include "DBQuery.h"

class CDataBase;

class CArtistData
{
public:
	long m_dwID;        // Eindeutiger Schlüssel

	CString m_sArtist;     // Name des Interpreten/Gruppe...
	CString m_sSortKey;    // Sortierschlüssel
	
	short m_nGroup;        // 0 = Unbekannt, 1 = Einzelinterpret, 2 = Gruppe, 3 = Orchester
	short m_nSex;          // 0 = Unbekannt, 1 = Männlich, 2 = Weiblich

	CString m_sComment;    // Kommentar

	// Version 10.0
	CString m_sURL;        // URLs
	CString m_sCountry;    // Herkunftsland

	// Version 11
	COleDateTime m_dtBirthDay;		 // Geburtsdatum oder Gründungsdatum
	COleDateTime m_dtDayOfDeath;	 // Todesdatum oder Auflösungsdatum
	CString m_sImageFilename;		 // Bild des Interpreten
};

class HITDB_INTERFACE CArtist : public CDBQuery, public CArtistData
{
public:
	CArtist(CDataBase* pDatabase = NULL);
	CArtist(const CArtist &theOther);
	void Copy(const CArtist &theOther);

	DECLARE_DYNAMIC(CArtist)

public:
	virtual DWORD GetIDFromName(BOOL* bAdded = NULL, BOOL bDoNotAdd = FALSE);
	CString GetSexString();
	CString GetGroupString();
	static CString GetGroupString(short nGroup);
	static CString GetSexString(short nSex);

	// Tabellenname
	static const CString GetTableName() { return L"Artist"; }

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CArtist)
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

