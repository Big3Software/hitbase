// Program.cpp : implementation file
//

#include "stdafx.h"
#include "hitdb.h"
#include "Program.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CProgram

IMPLEMENT_DYNAMIC(CProgram, CDaoRecordset)

CProgram::CProgram(CDataBase* pdb)
	: CDBQuery(pdb)
{
	//{{AFX_FIELD_INIT(CProgram)
	m_dwIDCD = 0;
	m_sName = _T("");
	m_sLieder = _T("");
	m_bStandard = FALSE;
	m_nFields = 4;
	//}}AFX_FIELD_INIT
	m_nDefaultType = dbOpenDynaset;
}

CString CProgram::GetDefaultSQL()
{
	return _T("[" + GetTableName() + "]");
}

void CProgram::DoFieldExchange(CDaoFieldExchange* pFX)
{
	//{{AFX_FIELD_MAP(CProgram)
	pFX->SetFieldType(CDaoFieldExchange::outputColumn);
	DFX_Long(pFX, _T("[IDCD]"), m_dwIDCD);
	DFX_Text(pFX, _T("[szName]"), m_sName);
	DFX_Text(pFX, _T("[szLieder]"), m_sLieder);
	DFX_Bool(pFX, _T("[bStandard]"), m_bStandard);
	//}}AFX_FIELD_MAP
}

/////////////////////////////////////////////////////////////////////////////
// CProgram diagnostics

#ifdef _DEBUG
void CProgram::AssertValid() const
{
	CDaoRecordset::AssertValid();
}

void CProgram::Dump(CDumpContext& dc) const
{
	CDaoRecordset::Dump(dc);
}
#endif //_DEBUG

CProgram::CProgram(const CProgram& theOther)
{
	*this = theOther;
}

CProgram& CProgram::operator =(const CProgram& theOther)
{
	m_dwIDCD = theOther.m_dwIDCD;
	m_sName = theOther.m_sName;
	m_sLieder = theOther.m_sLieder;
	m_bStandard = theOther.m_bStandard;
	m_nFields = theOther.m_nFields;

	m_nDefaultType = theOther.m_nDefaultType;

	m_pdb = theOther.m_pdb;
	m_pDatabase = theOther.m_pDatabase;

	return *this;
}

// Setzt das Default-Programm
void CProgramList::SetDefault(int iIndex)
{
	for (int i=0;i<GetSize();i++)
		(*this)[i].m_bStandard = FALSE;

	if (iIndex >= 0)
		(*this)[iIndex].m_bStandard = TRUE;
}

int CProgramList::GetDefault()
{
	for (int i=0;i<GetSize();i++)
		if (GetAt(i).m_bStandard)
			return i;

	return -1;
}

BOOL CProgramList::Delete(int iIndex)
{
	RemoveAt(iIndex);

	return TRUE;
}

BOOL CProgramList::AddNew(const CString &sName, const CString &sTracks)
{
	CProgram prog(m_pdb);
	prog.m_dwIDCD = m_dwIDCD;
	prog.m_sName = sName;
	prog.m_sLieder = sTracks;

	Add(prog);

	return TRUE;
}

// Alle Programm speichern
BOOL CProgramList::Save()
{
	// Damit das Speichern einfacher geht, werden zuerst mal alle Programme
	// der aktuellen CD gelöscht.

	m_pdb->BeginTransaction();
	CString sSQL;
	sSQL.Format(L"DELETE * FROM %s Where IDCD = %d", CProgram::GetTableName(), m_dwIDCD);
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

// Das Programm in die Datenbank hinzufügen
BOOL CProgram::Add()
{
    CProgram ProgramSave = *this;

	if (!OpenTable())
		return FALSE;

	AddNew();

	*this = ProgramSave;

	Update();

	QueryEnd();

	*this = ProgramSave;

	return TRUE;	
}

void CProgramList::Copy(const CProgramList &theOther)
{

}

void CProgram::Copy(const CProgram &theOther)
{

}
