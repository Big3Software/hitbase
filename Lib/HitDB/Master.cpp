// Master.cpp : implementation file
//

#include "stdafx.h"
#include "hitdb.h"
#include "Master.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CMaster

IMPLEMENT_DYNAMIC(CMaster, CDaoRecordset)

CMaster::CMaster(CDataBase* pdb)
	: CDBQuery(pdb)
{
	//{{AFX_FIELD_INIT(CMaster)
	m_wVersion = 0;
	m_sNameDate = _T("");
	m_cTypeDate = 0;
	m_nFields = 23;
	//}}AFX_FIELD_INIT
	m_nDefaultType = dbOpenDynaset;

	for (int i=0;i<MAX_USER_FIELDS;i++)
	{
		m_sCDUserField[i] = _T("");
		m_sTrackUserField[i] = _T("");
		m_cTypeCDUserField[i] = 0;
		m_cTypeTrackUserField[i] = 0;
	}
}

CString CMaster::GetDefaultSQL()
{
	return _T("[" + GetTableName() + "]");
}

void CMaster::DoFieldExchange(CDaoFieldExchange* pFX)
{
	//{{AFX_FIELD_MAP(CMaster)
	pFX->SetFieldType(CDaoFieldExchange::outputColumn);
	DFX_Short(pFX, _T("[wVersion]"), m_wVersion);
	DFX_Text(pFX, _T("[szNameDatum]"), m_sNameDate);
	DFX_Byte(pFX, _T("[cTypDatum]"), m_cTypeDate);
	//}}AFX_FIELD_MAP

	for (int i=0;i<MAX_USER_FIELDS;i++)
	{
		CString sField;
		sField.Format(L"szCDbFeld%d", i+1);
		DFX_Text(pFX, sField, m_sCDUserField[i]);

		sField.Format(L"szTrackbFeld%d", i+1);
		DFX_Text(pFX, sField, m_sTrackUserField[i]);

		sField.Format(L"cTypCDbFeld%d", i+1);
		DFX_Byte(pFX, sField, m_cTypeCDUserField[i]);

		sField.Format(L"cTypTrackbFeld%d", i+1);
		DFX_Byte(pFX, sField, m_cTypeTrackUserField[i]);
	}
}

/////////////////////////////////////////////////////////////////////////////
// CMaster diagnostics

#ifdef _DEBUG
void CMaster::AssertValid() const
{
	CDaoRecordset::AssertValid();
}

void CMaster::Dump(CDumpContext& dc) const
{
	CDaoRecordset::Dump(dc);
}
#endif //_DEBUG

void CMaster::operator =(const CMaster& theOther)
{
	m_wVersion = theOther.m_wVersion;
	m_sNameDate = theOther.m_sNameDate;
	m_cTypeDate = theOther.m_cTypeDate;

	for (int i=0;i<MAX_USER_FIELDS;i++)
	{
		m_sCDUserField[i] = theOther.m_sCDUserField[i];
		m_sTrackUserField[i] = theOther.m_sTrackUserField[i];
		m_cTypeCDUserField[i] = theOther.m_cTypeCDUserField[i];
		m_cTypeTrackUserField[i] = theOther.m_cTypeTrackUserField[i];
	}

	m_nFields = theOther.m_nFields;
}

