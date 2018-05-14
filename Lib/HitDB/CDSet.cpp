// CDSet.cpp : implementation file
//

#include "stdafx.h"
#include "hitdb.h"
#include "CDSet.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CCDSet

IMPLEMENT_DYNAMIC(CCDSet, CDaoRecordset)

CCDSet::CCDSet(CDataBase* pdb)
	: CDBQuery(pdb)
{
	//{{AFX_FIELD_INIT(CCDSet)
	m_dwID = 0;
	m_sCDSetName = _T("");
	m_nFields = 2;
	//}}AFX_FIELD_INIT
	m_nDefaultType = dbOpenDynaset;
}

void CCDSet::Copy(const CCDSet &theOther)
{
	m_sCDSetName = theOther.m_sCDSetName;
}

CCDSet::~CCDSet()
{
}

CString CCDSet::GetDefaultSQL()
{
	return _T("[" + GetTableName() + "]");
}

void CCDSet::DoFieldExchange(CDaoFieldExchange* pFX)
{
	//{{AFX_FIELD_MAP(CCDSet)
	pFX->SetFieldType(CDaoFieldExchange::outputColumn);
	DFX_Long(pFX, _T("[IDCDSet]"), m_dwID);
	DFX_Text(pFX, _T("[szCDSetName]"), m_sCDSetName);
	//}}AFX_FIELD_MAP
}

/////////////////////////////////////////////////////////////////////////////
// CCDSet diagnostics

#ifdef _DEBUG
void CCDSet::AssertValid() const
{
	CDaoRecordset::AssertValid();
}

void CCDSet::Dump(CDumpContext& dc) const
{
	CDaoRecordset::Dump(dc);
}
#endif //_DEBUG

// Liest alle CDSets.
BOOL CCDSet::ReadAllCDSets(CStringArray &saCDSets, CUIntArray &uiaCDSetsID)
{
	if (!QueryStart())
		return FALSE;

	BOOL bFound = QueryFindFirst();

	while (bFound)
	{
		saCDSets.Add(m_sCDSetName);
		uiaCDSetsID.Add(m_dwID);
		bFound = QueryFindNext();
	}

	QueryEnd();

	return TRUE;
}
