// Participant.cpp : implementation file
//

#include "stdafx.h"
#include "hitdb.h"
#include "Participant.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CRole

IMPLEMENT_DYNAMIC(CParticipant, CDaoRecordset)

CParticipant::CParticipant(CDataBase* pdb)
	: CDBQuery(pdb)
{
	//{{AFX_FIELD_INIT(CParticipant)
	m_dwIDCD = 0;
	m_dwIDRole = 0;
	m_dwIDArtist = 0;
	m_dwTrackNumber = 0;
	m_sComment = L"";
	m_nFields = 5;
	//}}AFX_FIELD_INIT
	m_nDefaultType = dbOpenDynaset;
}

CParticipant::CParticipant(const CParticipant& theOther)
{
	*this = theOther;
}

void CParticipant::Copy(const CParticipant &theOther)
{
	m_dwIDRole = theOther.m_dwIDRole;
	m_dwIDArtist = theOther.m_dwIDArtist;
	m_dwTrackNumber = theOther.m_dwTrackNumber;
	m_sComment = theOther.m_sComment;
}

CParticipant& CParticipant::operator =(const CParticipant &theOther)
{
	Copy(theOther);
	m_dwIDCD = theOther.m_dwIDCD;

	m_pdb = theOther.m_pdb;
	m_pDatabase = theOther.m_pDatabase;

	return *this;
}

CString CParticipant::GetDefaultSQL()
{
	return _T("[" + GetTableName() + "]");
}

void CParticipant::DoFieldExchange(CDaoFieldExchange* pFX)
{
	//{{AFX_FIELD_MAP(CParticipant)
	pFX->SetFieldType(CDaoFieldExchange::outputColumn);
	DFX_Long(pFX, _T("[P_IDCD]"), m_dwIDCD);
	DFX_Long(pFX, _T("[P_IDRole]"), m_dwIDRole);
	DFX_Long(pFX, _T("[P_IDArtist]"), m_dwIDArtist);
	DFX_Long(pFX, _T("[P_TrackNumber]"), m_dwTrackNumber);
	DFX_Text(pFX, _T("[P_Comment]"), m_sComment);
	//}}AFX_FIELD_MAP
}

/////////////////////////////////////////////////////////////////////////////
// CRole diagnostics

#ifdef _DEBUG
void CParticipant::AssertValid() const
{
	CDaoRecordset::AssertValid();
}

void CParticipant::Dump(CDumpContext& dc) const
{
	CDaoRecordset::Dump(dc);
}
#endif //_DEBUG

// Den Index in die Datenbank hinzufügen
BOOL CParticipant::Add()
{
    CParticipant participantSave;

	participantSave = *this;

	if (!OpenTable())
		return FALSE;

	AddNew();

	*this = participantSave;

	Update();

	QueryEnd();

	*this = participantSave;

	return TRUE;	
}

////////////////////////////////////////////////////////////////////////////////////////////////////////
CParticipantList::CParticipantList(CDataBase* pdb, long dwIDCD) 
{ 
	m_pdb = pdb; 
	m_dwIDCD = dwIDCD; 
}

BOOL CParticipantList::AddNew(long dwIDRole, long dwIDArtist, long dwTrackNumber, const CString& sComment)
{
	CParticipant participant(m_pdb);
	participant.m_dwIDCD = m_dwIDCD;
	participant.m_dwIDRole = dwIDRole;
	participant.m_dwIDArtist = dwIDArtist;
	participant.m_dwTrackNumber = dwTrackNumber;
	participant.m_sComment = sComment;

	Add(participant);

	return TRUE;
}

BOOL CParticipantList::Delete(int iIndex)
{
	RemoveAt(iIndex);

	return TRUE;
}

// Alle Mitwirkenden speichern
BOOL CParticipantList::AddAll()
{
	// Damit das Speichern einfacher geht, werden zuerst mal alle Mitwirkenden
	// der aktuellen CD gelöscht.

	m_pdb->BeginTransaction();
	CString sSQL;
	sSQL.Format(L"DELETE * FROM %s Where P_IDCD = %d", CParticipant::GetTableName(), m_dwIDCD);
	if (!m_pdb->ExecuteSQL(sSQL))
	{
		m_pdb->RollbackTransaction();
		return FALSE;
	}

	for (int i=0;i<GetSize();i++)
	{
		(*this)[i].m_dwIDCD = m_dwIDCD;
		(*this)[i].Add();
	}

	m_pdb->CommitTransaction();

	return TRUE;
}


