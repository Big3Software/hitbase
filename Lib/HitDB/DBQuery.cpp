// DBQuery.cpp: implementation of the CDBQuery class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "hitdb.h"
#include "DBQuery.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

IMPLEMENT_DYNAMIC(CDBQuery, CDaoRecordset)

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CDBQuery::CDBQuery(CDataBase* pDataBase) 
	: CDaoRecordset(&pDataBase->m_db ? &pDataBase->m_db : NULL)
{
	m_pdb = pDataBase;
	m_iProgressedRecords = 0;
	m_pProgressCtrl = NULL;
	m_bBackgroundSearch = FALSE;
	m_bStopBackgroundSearch = FALSE;
}

void CDBQuery::SetDataBase(CDataBase* pDataBase)
{
	m_pdb = pDataBase;
	m_pDatabase = &pDataBase->m_db;
}

long CDBQuery::GetCount()
{
	try 
	{
		MoveLast();

		long lCount = GetRecordCount();

		MoveFirst();

		return lCount;
	}
	catch (CDaoException* e)
	{               // Die Exception ignorieren und 0 zurückliefern!
		e->Delete();
		return 0;
	}
}

BOOL CDBQuery::GetRecordFromID(long dwID)
{
	// Tabellennamen ermitteln, um ihn für die Erstellung der ID zu benutzen
	CString sTablename = GetDefaultSQL().Mid(1);
	sTablename = sTablename.Left(sTablename.GetLength()-1);

	// Die ID heißt in unseren Tabellen immer ID[Tabellenname]
	CString sWhere;
	sWhere.Format(L"ID%s = %ld", sTablename, dwID);
	if (!QueryStart(sWhere))
		return FALSE;

	BOOL bFound = QueryFindFirst();
	
	QueryEnd();

	return bFound;	
}

BOOL CDBQuery::WriteRecord()
{
	// Tabellennamen ermitteln, um ihn für die Erstellung der ID zu benutzen
	CString sTablename = GetDefaultSQL().Mid(1);
	sTablename = sTablename.Left(sTablename.GetLength()-1);

	// !!!!!!!!!!!!! TODO: Fehler abfragen!
	Edit();
	Update();
	
	return TRUE;	
}

BOOL CDBQuery::QueryStart(const CString &sWhere, const CString& sOrder)
{
	ASSERT(m_pdb->IsDataBaseOpened());

	CString sTable = GetDefaultSQL();
	CString sSQLString = "SELECT * FROM " + sTable;

	if (!sWhere.IsEmpty())
	{
		sSQLString += " WHERE " + sWhere;
	}

	if (!sOrder.IsEmpty())
	{
		sSQLString += " ORDER BY " + sOrder;
	}

	return QueryStartFreeSQL(sSQLString);
}

BOOL CDBQuery::QueryStartFreeSQL(const CString &sSQL)
{
	ASSERT(m_pdb->IsDataBaseOpened());

	m_pdb->m_StopSearchProcess = FALSE;

	try
	{
		Open(dbOpenDynaset, sSQL, dbSeeChanges);
	}
	catch (CDaoException* e)
	{
		e->ReportError();
		e->Delete();
		return FALSE;
	}

    if (GetProgressCtrl())
        GetProgressCtrl()->SetRange32(0, GetCount()/10);

	return TRUE;
}

// Beendet die zuletzt ausgeführte Abfrage an die Datenbank.
BOOL CDBQuery::QueryEnd()
{
	ASSERT(this);

	Close();

	return TRUE;
}

// Liest die erste Zeile des Ergebnisses.
// 
// Return: TRUE = Ok, Ergebnis in pTable 
//         FALSE = Keinen Satz (mehr) gefunden
BOOL CDBQuery::QueryFindFirst()
{
	ASSERT(this);

	if (IsEOF())
		return FALSE;

	try 
	{
		MoveFirst();
	}
	catch (CDaoException* e)
	{
		e->Delete();
		return FALSE;
	}

    m_iProgressedRecords = 0;
	m_bStopBackgroundSearch = FALSE;

    if (GetProgressCtrl())
        GetProgressCtrl()->SetPos(0);
   
	return TRUE;
}

// Liest die nächste Zeile des Ergebnisses.
// 
// Return: TRUE = Ok, Ergebnis in pTable 
//         FALSE = Keinen Satz (mehr) gefunden
BOOL CDBQuery::QueryFindNext()
{
	ASSERT(this);

	TRY
	{
		MoveNext();
	}
	CATCH(CException, e)
	{
		return FALSE;
	}
	END_CATCH

	if (IsEOF())
		return FALSE;

    if (GetProgressCtrl())
	{
		m_iProgressedRecords++;
		GetProgressCtrl()->SetPos(m_iProgressedRecords/10);
	}

	if (m_bBackgroundSearch)
	{
		if (!BackgroundProcessing())
			return FALSE;
	}

	return TRUE;
}

// Löscht den Datensatz aus der Tabelle mit der angegebenen ID.
BOOL CDBQuery::DeleteFromID(long dwID)
{
	// Tabellennamen ermitteln, um ihn für die Erstellung der ID zu benutzen
	CString sTablename = GetDefaultSQL().Mid(1);
	sTablename = sTablename.Left(sTablename.GetLength()-1);

	// Die ID heißt in unseren Tabellen immer ID[Tabellenname]
	CString sWhere;
	sWhere.Format(L"ID%s = %ld", sTablename, dwID);

	if (!QueryStart(sWhere))
		return FALSE;

	BOOL bFound = QueryFindFirst();

	if (bFound)
		Delete();

	QueryEnd();

	return bFound;	
}

// Löscht ALLE Datensätze aus der Tabelle!
BOOL CDBQuery::DeleteAll()
{
	CString sSQL;
	sSQL.Format(L"DELETE * FROM %s", GetDefaultSQL());

	BOOL bRet = m_pdb->ExecuteSQL(sSQL);

	return bRet;	
}

// Öffnet die gesamte Tabelle (Der Tabellenname wird in der abgeleiteten Klasse definiert!)
BOOL CDBQuery::OpenTable()
{
	ASSERT(m_pdb->IsDataBaseOpened());

	CString sTable = GetDefaultSQL();

	// TODO!!!!!!!!!!!! Error Handling!
	Open(dbOpenTable, sTable, dbSeeChanges);

	return TRUE;
}

long CDBQuery::GetTotalCount()
{
	// Tabellennamen ermitteln, um ihn für die Erstellung der ID zu benutzen
	CString sTablename = GetDefaultSQL().Mid(1);
	sTablename = sTablename.Left(sTablename.GetLength()-1);

	CString sQuery;
	sQuery.Format(L"SELECT count(*) from %s", sTablename);

	return m_pdb->Abfrage(sQuery).lVal;
}

// Liefert die Datenbank zurück, der die Query zugeordnet ist!
CDataBase* CDBQuery::GetDataBase() 
{
	return m_pdb;
}

// Setzt ein Progress-Control, der bei QueryFindFirst und QueryFindNext benutzt werden soll.
void CDBQuery::SetProgressCtrl(CProgressCtrl* pProgressCtrl, BOOL bBackground /* = TRUE */)
{
	m_pProgressCtrl = pProgressCtrl;
	m_bBackgroundSearch = bBackground;      // Hintergrund-Suche JA/NEIN
}

// Setzt den Progress-Dialog, der den Fortschritt der Suche anzeigen soll.
void CDBQuery::SetProgressDlg(CDataBaseProgressDlg* pProgressDlg, BOOL bBackground /* = TRUE */)
{
	pProgressDlg->m_pQuery = this;
	
	SetProgressCtrl(&pProgressDlg->m_ProgressCtrl);
}

// Liefert den aktuellen Progress-Control zurück, NULL = keine ProgressCtrl.
CProgressCtrl* CDBQuery::GetProgressCtrl()
{
	return m_pProgressCtrl;
}

void CDBQuery::StopBackgroundSearch()
{
	m_bStopBackgroundSearch = TRUE;
}

BOOL CDBQuery::BackgroundProcessing()
{
	MSG msg;
	while ( ::PeekMessage( &msg, NULL, 0, 0, PM_NOREMOVE ) && !m_bStopBackgroundSearch) 
	{ 
		if ( !AfxGetApp()->PumpMessage( ) ) 
		{ 
			::PostQuitMessage(0); 
			return FALSE; 
		} 
	} 
	// let MFC do its idle processing
	LONG lIdle = 0;
	while ( AfxGetApp()->OnIdle(lIdle++ ) && !m_bStopBackgroundSearch)
		;  

	if (m_bStopBackgroundSearch)
		return FALSE;

	return TRUE;
}
