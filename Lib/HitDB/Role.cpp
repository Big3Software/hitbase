// Medium.cpp : implementation file
//

#include "stdafx.h"
#include "hitdb.h"
#include "Role.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CRole

IMPLEMENT_DYNAMIC(CRole, CDaoRecordset)

CRole::CRole(CDataBase* pdb)
	: CDBQuery(pdb)
{
	//{{AFX_FIELD_INIT(CRole)
	m_dwID = 0;
	m_sRole = _T("");
	m_nFields = 2;
	//}}AFX_FIELD_INIT
	m_nDefaultType = dbOpenDynaset;
}

void CRole::Copy(const CRole &theOther)
{
	m_sRole = theOther.m_sRole;
}

CString CRole::GetDefaultSQL()
{
	return _T("[" + GetTableName() + "]");
}

void CRole::DoFieldExchange(CDaoFieldExchange* pFX)
{
	//{{AFX_FIELD_MAP(CRole)
	pFX->SetFieldType(CDaoFieldExchange::outputColumn);
	DFX_Long(pFX, _T("[R_ID]"), m_dwID);
	DFX_Text(pFX, _T("[R_Role]"), m_sRole);
	//}}AFX_FIELD_MAP
}

/////////////////////////////////////////////////////////////////////////////
// CRole diagnostics

#ifdef _DEBUG
void CRole::AssertValid() const
{
	CDaoRecordset::AssertValid();
}

void CRole::Dump(CDumpContext& dc) const
{
	CDaoRecordset::Dump(dc);
}
#endif //_DEBUG
