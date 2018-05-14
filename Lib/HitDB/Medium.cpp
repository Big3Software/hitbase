// Medium.cpp : implementation file
//

#include "stdafx.h"
#include "hitdb.h"
#include "Medium.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CMedium

IMPLEMENT_DYNAMIC(CMedium, CDaoRecordset)

CMedium::CMedium(CDataBase* pdb)
	: CDBQuery(pdb)
{
	//{{AFX_FIELD_INIT(CMedium)
	m_dwID = 0;
	m_sMedium = _T("");
	m_wOrder = 0;
	m_nFields = 3;
	//}}AFX_FIELD_INIT
	m_nDefaultType = dbOpenDynaset;
}

void CMedium::Copy(const CMedium &theOther)
{
	m_sMedium = theOther.m_sMedium;
}

CString CMedium::GetDefaultSQL()
{
	return _T("[" + GetTableName() + "]");
}

void CMedium::DoFieldExchange(CDaoFieldExchange* pFX)
{
	//{{AFX_FIELD_MAP(CMedium)
	pFX->SetFieldType(CDaoFieldExchange::outputColumn);
	DFX_Long(pFX, _T("[IDMedium]"), m_dwID);
	DFX_Text(pFX, _T("[szMedium]"), m_sMedium);
	DFX_Short(pFX, _T("[wOrder]"), m_wOrder);
	//}}AFX_FIELD_MAP
}

/////////////////////////////////////////////////////////////////////////////
// CMedium diagnostics

#ifdef _DEBUG
void CMedium::AssertValid() const
{
	CDaoRecordset::AssertValid();
}

void CMedium::Dump(CDumpContext& dc) const
{
	CDaoRecordset::Dump(dc);
}
#endif //_DEBUG
