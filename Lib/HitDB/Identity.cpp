// Identity.cpp : implementation file
//

#include "stdafx.h"
#include "hitdb.h"
#include "Identity.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CIdentity

IMPLEMENT_DYNAMIC(CIdentity, CDaoRecordset)

CIdentity::CIdentity(CDataBase* pdb)
	: CDBQuery(pdb)
{
	//{{AFX_FIELD_INIT(CIdentity)
	m_sIdentity = _T("");
	m_dwIDCD = 0;
	m_nFields = 2;
	//}}AFX_FIELD_INIT
	m_nDefaultType = dbOpenDynaset;
}

CIdentity::CIdentity(const CIdentity& theOther)
{
	*this = theOther;
}

void CIdentity::Copy(const CIdentity &theOther)
{
	m_sIdentity = theOther.m_sIdentity;
}

CIdentity::~CIdentity()
{
}

CString CIdentity::GetDefaultSQL()
{
	return _T("[" + GetTableName() + "]");
}

void CIdentity::DoFieldExchange(CDaoFieldExchange* pFX)
{
	//{{AFX_FIELD_MAP(CIdentity)
	pFX->SetFieldType(CDaoFieldExchange::outputColumn);
	DFX_Text(pFX, _T("[szIdentity]"), m_sIdentity);
	DFX_Long(pFX, _T("[IDCD]"), m_dwIDCD);
	//}}AFX_FIELD_MAP
}

/////////////////////////////////////////////////////////////////////////////
// CIdentity diagnostics

#ifdef _DEBUG
void CIdentity::AssertValid() const
{
	CDaoRecordset::AssertValid();
}

void CIdentity::Dump(CDumpContext& dc) const
{
	CDaoRecordset::Dump(dc);
}
#endif //_DEBUG

void CIdentity::operator =(const CIdentity& theOther)
{
	m_dwIDCD = theOther.m_dwIDCD;
	m_sIdentity = theOther.m_sIdentity;
}

// Schreibt die Identity in die Datenbank, wenn Sie dort noch nicht vorhanden ist.
BOOL CIdentity::Write()
{
	if (m_sIdentity.IsEmpty())    // Wo nix is, kann auch nix geschrieben werden!
		return TRUE;

	// Die ID heißt in unseren Tabellen immer ID[Tabellenname]
	CString sWhere;
	sWhere.Format(L"szIdentity = \"%s\"", m_sIdentity);

	// Sichern!
	CIdentity IDSave = *this;

	if (!QueryStart(sWhere))
		return FALSE;

	// Nur hinzufügen, wenn noch nicht vorhanden!
	if (IsEOF())
	{
		AddNew();

		*this = IDSave;
	
		Update();
	}
	else
	{
		// JUS 29.12.2002: Wenn die CD zugewiesen wurde, hat sich die ID der CD geändert
		if (IDSave.m_dwIDCD != m_dwIDCD)
		{
			Edit();

			*this = IDSave;

			Update();
		}
	}

	QueryEnd();

	*this = IDSave;

	return TRUE;	
}
