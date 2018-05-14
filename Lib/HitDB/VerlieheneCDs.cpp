// VerlieheneCDs.cpp: Implementierungsdatei
//

#include "stdafx.h"
#include "hitdb.h"
#include "VerlieheneCDs.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CVerlieheneCDs

IMPLEMENT_DYNAMIC(CVerlieheneCDs, CDaoRecordset)

CVerlieheneCDs::CVerlieheneCDs(CDataBase* pdb)
	: CDBQuery(pdb)
{
	//{{AFX_FIELD_INIT(CVerlieheneCDs)
	m_dwID = 0;
	m_dtVerliehenAm = (DATE)0;
	m_sVerliehenAn = _T("");
	m_dtRueckgabeTermin = (DATE)0;
	m_sKommentar = _T("");
	m_nFields = 5;
	//}}AFX_FIELD_INIT
	m_nDefaultType = dbOpenDynaset;
}

CString CVerlieheneCDs::GetDefaultSQL()
{
	return _T("[" + GetTableName() + "]");
}

void CVerlieheneCDs::DoFieldExchange(CDaoFieldExchange* pFX)
{
	//{{AFX_FIELD_MAP(CVerlieheneCDs)
	pFX->SetFieldType(CDaoFieldExchange::outputColumn);
	DFX_Long(pFX, _T("[IDCD]"), m_dwID);
	DFX_DateTime(pFX, _T("[VerliehenAm]"), m_dtVerliehenAm);
	DFX_Text(pFX, _T("[VerliehenAn]"), m_sVerliehenAn);
	DFX_DateTime(pFX, _T("[RueckgabeTermin]"), m_dtRueckgabeTermin);
	DFX_Text(pFX, _T("[Kommentar]"), m_sKommentar);
	//}}AFX_FIELD_MAP
}

/////////////////////////////////////////////////////////////////////////////
// Diagnose CVerlieheneCDs

#ifdef _DEBUG
void CVerlieheneCDs::AssertValid() const
{
	CDaoRecordset::AssertValid();
}

void CVerlieheneCDs::Dump(CDumpContext& dc) const
{
	CDaoRecordset::Dump(dc);
}
#endif //_DEBUG
