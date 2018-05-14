// Category.cpp : implementation file
//

#include "stdafx.h"
#include "hitdb.h"
#include "Category.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CCategory

IMPLEMENT_DYNAMIC(CCategory, CDaoRecordset)

CCategory::CCategory(CDataBase* pdb)
	: CDBQuery(pdb)
{
	//{{AFX_FIELD_INIT(CCategory)
	m_dwID = 0;
	m_sCategory = _T("");
	m_wOrder = 0;
	m_nFields = 3;
	//}}AFX_FIELD_INIT
	m_nDefaultType = dbOpenDynaset;
}

void CCategory::Copy(const CCategory &theOther)
{
	m_sCategory = theOther.m_sCategory;
}

CString CCategory::GetDefaultSQL()
{
	return _T("[" + GetTableName() +"]");
}

void CCategory::DoFieldExchange(CDaoFieldExchange* pFX)
{
	//{{AFX_FIELD_MAP(CCategory)
	pFX->SetFieldType(CDaoFieldExchange::outputColumn);
	DFX_Long(pFX, _T("[IDKategorie]"), m_dwID);
	DFX_Text(pFX, _T("[szKategorieName]"), m_sCategory);
	DFX_Short(pFX, _T("[wOrder]"), m_wOrder);
	//}}AFX_FIELD_MAP
}

/////////////////////////////////////////////////////////////////////////////
// CCategory diagnostics

#ifdef _DEBUG
void CCategory::AssertValid() const
{
	CDaoRecordset::AssertValid();
}

void CCategory::Dump(CDumpContext& dc) const
{
	CDaoRecordset::Dump(dc);
}
#endif //_DEBUG
