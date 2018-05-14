// Index.cpp : implementation file
//

#include "stdafx.h"
#include "hitdb.h"
#include "Index.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CIndex

IMPLEMENT_DYNAMIC(CIndex, CDaoRecordset)

CIndex::CIndex(CDataBase* pdb)
	: CDBQuery(pdb)
{
	//{{AFX_FIELD_INIT(CIndex)
	m_dwIDCD = 0;
	m_sIndexName = _T("");
	m_dwPosition = 0;
	m_nFields = 3;
	//}}AFX_FIELD_INIT
	m_nDefaultType = dbOpenDynaset;
}

CString CIndex::GetDefaultSQL()
{
	return _T("[" + GetTableName() + "]");
}

void CIndex::DoFieldExchange(CDaoFieldExchange* pFX)
{
	//{{AFX_FIELD_MAP(CIndex)
	pFX->SetFieldType(CDaoFieldExchange::outputColumn);
	DFX_Long(pFX, _T("[IDCD]"), m_dwIDCD);
	DFX_Text(pFX, _T("[szIndexName]"), m_sIndexName);
	DFX_Long(pFX, _T("[dwPosition]"), m_dwPosition);
	//}}AFX_FIELD_MAP
}

/////////////////////////////////////////////////////////////////////////////
// CIndex diagnostics

#ifdef _DEBUG
void CIndex::AssertValid() const
{
	CDaoRecordset::AssertValid();
}

void CIndex::Dump(CDumpContext& dc) const
{
	CDaoRecordset::Dump(dc);
}
#endif //_DEBUG

CIndex::CIndex(const CIndex& theOther)
{
	*this = theOther;
}

CIndex& CIndex::operator =(const CIndex& theOther)
{
	m_dwIDCD = theOther.m_dwIDCD;
	m_sIndexName = theOther.m_sIndexName;
	m_dwPosition = theOther.m_dwPosition;

	m_nFields = theOther.m_nFields;

	m_nDefaultType = theOther.m_nDefaultType;

	m_pdb = theOther.m_pdb;
	m_pDatabase = theOther.m_pDatabase;

	return *this;
}

BOOL CIndexList::AddNew(const CString &sName, DWORD dwPosition)
{
	CIndex index(m_pdb);
	index.m_dwIDCD = m_dwIDCD;
	index.m_sIndexName = sName;
	index.m_dwPosition = dwPosition;

	Add(index);

	return TRUE;
}

BOOL CIndexList::Delete(int iIndex)
{
	RemoveAt(iIndex);

	return TRUE;
}

// Alle Indizes speichern
BOOL CIndexList::Save()
{
	// Damit das Speichern einfacher geht, werden zuerst mal alle Programme
	// der aktuellen CD gelöscht.

	m_pdb->BeginTransaction();
	CString sSQL;
	sSQL.Format(L"DELETE * FROM %s Where IDCD = %d", CIndex::GetTableName(), m_dwIDCD);
	if (!m_pdb->ExecuteSQL(sSQL))
	{
		m_pdb->RollbackTransaction();
		return FALSE;
	}

	for (int i=0;i<GetSize();i++)
	{
		(*this)[i].Add();
	}

	m_pdb->CommitTransaction();

	return TRUE;
}

// Den Index in die Datenbank hinzufügen
BOOL CIndex::Add()
{
    CIndex IndexSave = *this;

	if (!OpenTable())
		return FALSE;

	AddNew();

	*this = IndexSave;

	Update();

	QueryEnd();

	*this = IndexSave;

	return TRUE;	
}

void CIndexList::Copy(const CIndexList &theOther)
{

}

void CIndex::Copy(const CIndex &theOther)
{

}
