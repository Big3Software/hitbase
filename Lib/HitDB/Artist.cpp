// Artist.cpp: implementation of the CArtist class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "hitdb.h"
#include "Artist.h"
#include "..\..\app\hitbase\resource.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

/////////////////////////////////////////////////////////////////////////////
// CArtist

IMPLEMENT_DYNAMIC(CArtist, CDBQuery)

CArtist::CArtist(CDataBase* pDataBase)
	: CDBQuery(pDataBase)
{
	//{{AFX_FIELD_INIT(CArtist)
	m_dwID = 0;
	m_sArtist = "";
	m_sSortKey = "";
	m_nGroup = 0;
	m_nSex = 0;
	m_nFields = 11;
	m_sComment = "";
	m_sCountry = "";
	m_sURL = "";
	m_dtBirthDay.SetStatus(COleDateTime::null);
	m_dtDayOfDeath.SetStatus(COleDateTime::null);
	m_sImageFilename = "";
	//}}AFX_FIELD_INIT
	m_nDefaultType = dbOpenDynaset;
}

CArtist::CArtist(const CArtist &theOther)
{
	m_dwID = theOther.m_dwID;
	this->Copy(theOther);
}

void CArtist::Copy(const CArtist &theOther)
{
	m_sArtist = theOther.m_sArtist;
	m_sSortKey = theOther.m_sSortKey;
	m_nGroup = theOther.m_nGroup;
	m_nSex = theOther.m_nSex;
	m_sComment = theOther.m_sComment;
	m_sURL = theOther.m_sURL;
	m_sCountry = theOther.m_sCountry;
	m_dtBirthDay = theOther.m_dtBirthDay;
	m_dtDayOfDeath = theOther.m_dtDayOfDeath;
	m_sImageFilename = theOther.m_sImageFilename;
}

CString CArtist::GetDefaultSQL()
{
	return _T("[" + GetTableName() + "]");
}

void CArtist::DoFieldExchange(CDaoFieldExchange* pFX)
{
	//{{AFX_FIELD_MAP(CArtist)
	pFX->SetFieldType(CDaoFieldExchange::outputColumn);
	DFX_Long(pFX, _T("[IDArtist]"), m_dwID);
	DFX_Text(pFX, _T("[szArtistName]"), m_sArtist);
	//}}AFX_FIELD_MAP

	ASSERT(m_pdb->GetVersion() >= 601);

	if (m_pdb->GetVersion() < 801)
	{
		m_nFields = 2;
	}
	else
	{
		DFX_Text(pFX, _T("[sSortKey]"), m_sSortKey);
		DFX_Short(pFX, _T("[nGroup]"), m_nGroup);
		DFX_Short(pFX, _T("[nSex]"), m_nSex);
		DFX_Text(pFX, _T("[sComment]"), m_sComment);

		if (m_nGroup == AFX_RFX_SHORT_PSEUDO_NULL)
			m_nGroup = 0;
		if (m_nSex == AFX_RFX_SHORT_PSEUDO_NULL)
			m_nSex = 0;

		if (m_pdb->GetVersion() < 1001)
			m_nFields = 6;
		else
		{
			DFX_Text(pFX, _T("[A_URL]"), m_sURL);
			DFX_Text(pFX, _T("[A_Country]"), m_sCountry);

			if (m_pdb->GetVersion() < 1101)
				m_nFields = 8;
			else
			{
				DFX_DateTime(pFX, _T("[A_BirthDay]"), m_dtBirthDay);
				DFX_DateTime(pFX, _T("[A_DayOfDeath]"), m_dtDayOfDeath);
				DFX_Text(pFX, _T("[A_ImageFilename]"), m_sImageFilename);
			}
		}
	}
}

/////////////////////////////////////////////////////////////////////////////
// CArtist diagnostics

#ifdef _DEBUG
void CArtist::AssertValid() const
{
	CDaoRecordset::AssertValid();
}

void CArtist::Dump(CDumpContext& dc) const
{
	CDaoRecordset::Dump(dc);
}
#endif //_DEBUG

CString CArtist::GetGroupString()
{
	return GetGroupString(m_nGroup);
}

CString CArtist::GetGroupString(short nGroup)
{
	CString str;

	switch (nGroup)
	{
	case 0:
		str.LoadString(IDS_UNKNOWN);
		break;
	case 1:
		str.LoadString(IDS_GROUP_MULTI);
		break;
	case 2:
		str.LoadString(IDS_GROUP_SINGLE);
		break;
	case 3:
		str.LoadString(IDS_GROUP_ORCHESTER);
		break;
	case 4:
		str.LoadString(IDS_GROUP_DUO);
		break;
	default:
		ASSERT(FALSE);
		str.LoadString(IDS_UNKNOWN);
	}

	return str;
}

CString CArtist::GetSexString()
{
	return GetSexString(m_nSex);
}

CString CArtist::GetSexString(short nSex)
{
	CString str;

	switch (nSex)
	{
	case 0:
		str.LoadString(IDS_UNKNOWN);
		break;
	case 1:
		str.LoadString(IDS_SEX_FEMININ);
		break;
	case 2:
		str.LoadString(IDS_SEX_MASKULIN);
		break;
	case 3:
		str.LoadString(IDS_SEX_GEMISCHT);
		break;
	default:
		ASSERT(FALSE);
		str.LoadString(IDS_UNKNOWN);
	}

	return str;
}

DWORD CArtist::GetIDFromName(BOOL* bAdded /* = NULL */, BOOL bDoNotAdd /* = FALSE */)
{
	CString sSqlFindArtist;
	CString sArtistSQL = m_sArtist;
	CArtist SaveArtist;
	
	if (m_sArtist.IsEmpty())
		return 0;

	SaveArtist.Copy(*this);
	
	if (bAdded != NULL)
		*bAdded = FALSE;
	
	// jus 07.05.97: Auch leere Artisten eintragen!
	//   if (szArtist.IsEmpty())    // Keinen Interpreten eingegeben!
	//      return 0L;              
	
	CMisc::SqlPrepare(sArtistSQL);
	
	sSqlFindArtist = "szArtistName = \"" + sArtistSQL + "\"";
	
	if (!QueryStart(sSqlFindArtist))
		return FALSE;

	BOOL bFound = QueryFindFirst();
	
	if (!bFound)      // Jetzt muß der Artist hinzugefügt werden
	{
		if (!bDoNotAdd)
		{
			AddNew();

			// Der Sortierschlüssel wird standardmäßig mit dem Interpreten gefüllt!
			if (SaveArtist.m_sSortKey.IsEmpty())
				SaveArtist.m_sSortKey = SaveArtist.m_sArtist;

			Copy(SaveArtist);

			// Die ID muss gelesen werden!
			DWORD dwID;
			m_dwID = dwID = GetFieldValue(L"IDArtist").lVal;

			Update();

			Copy(SaveArtist);

			m_dwID = dwID;

			if (bAdded != NULL)
				*bAdded = TRUE;
		}
	}
/*	else    // Artist schon vorhanden, dann Inhalt updaten.
// JUS 09.03.2003: Wieder rausgenommen
	{
		Edit();

		Copy(SaveArtist);

		Update();
	}*/
	
	QueryEnd();

	return m_dwID;
}
