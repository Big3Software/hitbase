// Track.cpp: implementation of the CTrack class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "hitdb.h"
#include "Track.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

/////////////////////////////////////////////////////////////////////////////
// CTrack

IMPLEMENT_DYNAMIC(CTrack, CDBQuery)

CTrack::CTrack(CDataBase* pDataBase)
	: CDBQuery(pDataBase)
{
	//{{AFX_FIELD_INIT(CTrack)
	m_dwID = 0;
	m_dwIDCD = 0;
	m_dwIDArtist = 0;
	m_wTrack = 0;
	m_sTitle = "";
	m_dwLength = 0;
	m_wBpm = 0;
	m_sWavePath = "";
	m_sCodes = "";
	m_sComment = "";
	m_sLyrics = "";
	m_lFormat = 0;					 // Format des Liedes (Audio-Track, MP3, WMA, ...)
	m_lBitRate = 0;				 // Bitrate des Liedes (z.b. 128 KBit = 131072)
	m_lSampleRate = 0;				 // Samplerate des Liedes (z.b. 44100 Hz)
	m_lChannels = 0;				 // Anzahl der Kanäle (0 = stereo (default), 1 = mono, 2 = stereo)
	m_lYearRecorded = 0;             // Aufnahme-Jahr (für Sampler)
	m_sCheckSum = "";
	m_lRating = 0;
	m_dwIDCategory = 0;
	m_dwIDComposer = 0;
	m_sLanguage = "";
	m_nFields = 26;
	//}}AFX_FIELD_INIT
	m_nDefaultType = dbOpenDynaset;

	for (int i=0;i<MAX_USER_FIELDS;i++)
		m_sUser[i] = "";

	m_pCD = NULL;
	m_pArtist = NULL;
	m_pComposer = NULL;
	m_pCategory = NULL;
}

CTrack::CTrack(const CTrack& theOther)
{
	m_pCD = NULL;
	m_pArtist = NULL;
	m_pComposer = NULL;
	m_pCategory = NULL;

	*this = theOther;
}

void CTrack::operator =(const CTrack& theOther)
{
	m_dwID = theOther.m_dwID;
	m_dwIDCD = theOther.m_dwIDCD;
	m_dwIDArtist = theOther.m_dwIDArtist;
	m_wTrack = theOther.m_wTrack;
	m_sTitle = theOther.m_sTitle;
	m_dwLength = theOther.m_dwLength;
	m_dwStartPosition = theOther.m_dwStartPosition;
	m_wBpm = theOther.m_wBpm;
	m_sWavePath = theOther.m_sWavePath;
	m_sCodes = theOther.m_sCodes;
	m_sComment = theOther.m_sComment;
	m_sLyrics = theOther.m_sLyrics;

	for (int i=0;i<MAX_USER_FIELDS;i++)
		m_sUser[i] = theOther.m_sUser[i];

	m_lFormat = theOther.m_lFormat;
	m_lBitRate = theOther.m_lBitRate;
	m_lSampleRate = theOther.m_lSampleRate;
	m_lChannels = theOther.m_lChannels;

	// 14.01.2002
	m_lYearRecorded = theOther.m_lYearRecorded;
	//_14.01.2002

	// 20.08.2003
	m_sCheckSum = theOther.m_sCheckSum;
	m_lRating = theOther.m_lRating;
	//_20.08.2003

	// Version 11, 20.12.2005
	m_dwIDCategory = theOther.m_dwIDCategory;
	m_dwIDComposer = theOther.m_dwIDComposer;
	m_sLanguage = theOther.m_sLanguage;

	m_nFields = theOther.m_nFields;

	m_nDefaultType = theOther.m_nDefaultType;

	if (m_pCD)
		delete m_pCD;

	if (m_pArtist)
		delete m_pArtist;

	if (m_pComposer)
		delete m_pComposer;

	m_pCD = NULL;
	m_pArtist = NULL;
	m_pComposer = NULL;
	m_pCategory = NULL;

	m_pdb = theOther.m_pdb;
	m_pDatabase = theOther.m_pDatabase;
}

void CTrack::Copy(CTrack &theOther)
{
	GetArtist()->Copy(*theOther.GetArtist());
	m_wTrack = theOther.m_wTrack;
	m_sTitle = theOther.m_sTitle;
	m_dwLength = theOther.m_dwLength;
	m_dwStartPosition = theOther.m_dwStartPosition;
	m_wBpm = theOther.m_wBpm;
	m_sWavePath = theOther.m_sWavePath;
	m_sCodes = theOther.m_sCodes;
	m_sComment = theOther.m_sComment;
	m_sLyrics = theOther.m_sLyrics;

	for (int i=0;i<MAX_USER_FIELDS;i++)
		m_sUser[i] = theOther.m_sUser[i];

	m_lFormat = theOther.m_lFormat;
	m_lBitRate = theOther.m_lBitRate;
	m_lSampleRate = theOther.m_lSampleRate;
	m_lChannels = theOther.m_lChannels;

	// 14.01.2002
	m_lYearRecorded = theOther.m_lYearRecorded;
	//_14.01.2002

	// 20.08.2003
	m_sCheckSum = theOther.m_sCheckSum;
	m_lRating = theOther.m_lRating;
	//_20.08.2003

	// Version 11, 20.12.2005
	m_dwIDCategory = theOther.m_dwIDCategory;
	GetComposer()->Copy(*theOther.GetComposer());
	m_sLanguage = theOther.m_sLanguage;
}

CTrack::~CTrack()
{
	if (m_pCD)
	{
		delete m_pCD;
		m_pCD = NULL;
	}

	if (m_pArtist)
	{
		delete m_pArtist;
		m_pArtist = NULL;
	}

	if (m_pComposer)
	{
		delete m_pComposer;
		m_pComposer = NULL;
	}
}

CString CTrack::GetDefaultSQL()
{
	return _T("[" + GetTableName() + "]");
}

void CTrack::DoFieldExchange(CDaoFieldExchange* pFX)
{
	//{{AFX_FIELD_MAP(CTrack)
	pFX->SetFieldType(CDaoFieldExchange::outputColumn);
	DFX_Long(pFX, _T("[IDLied]"), m_dwID);
	DFX_Long(pFX, _T("[IDCD]"), m_dwIDCD);
	DFX_Long(pFX, _T("[IDArtist]"), m_dwIDArtist);
	DFX_Short(pFX, _T("[wLiedNummer]"), m_wTrack);
	DFX_Text(pFX, _T("[szTitel]"), m_sTitle);
	DFX_Long(pFX, _T("[dwLaenge]"), m_dwLength);
	DFX_Short(pFX, _T("[wBpm]"), m_wBpm);
	DFX_Text(pFX, _T("[szNameRecDatei]"), m_sWavePath);
	DFX_Text(pFX, _T("[szKennzeichen]"), m_sCodes);
	DFX_Text(pFX, _T("[szKommentar]"), m_sComment);
	DFX_Text(pFX, _T("[szLiedtext]"), m_sLyrics);
	//}}AFX_FIELD_MAP

	if (m_pdb->GetVersion() < 801)
	{
		for (int i=0;i<2;i++)
		{
			CString sField;
			sField.Format(L"szFeld%d", i+1);
			DFX_Text(pFX, sField, m_sUser[i]);
		}

		m_nFields = 13;
	}
	else
	{
		for (int i=0;i<MAX_USER_FIELDS;i++)
		{
			CString sField;
			sField.Format(L"szFeld%d", i+1);
			DFX_Text(pFX, sField, m_sUser[i]);
		}

		if (m_pdb->GetVersion() < 901)
			m_nFields = 16;
		else
		{
			DFX_Long(pFX, _T("[L_TrackFormat]"), m_lFormat);
			DFX_Long(pFX, _T("[L_BitRate]"), m_lBitRate);
			DFX_Long(pFX, _T("[L_SampleRate]"), m_lSampleRate);
			DFX_Long(pFX, _T("[L_Channels]"), m_lChannels);
			DFX_Long(pFX, _T("[L_YearRecorded]"), m_lYearRecorded);

			if (m_pdb->GetVersion() < 1001)
			{
				m_nFields = 21;
			}
			else
			{
				DFX_Text(pFX, _T("[L_CheckSum]"), m_sCheckSum);
				DFX_Long(pFX, _T("[L_Rating]"), m_lRating);

				if (m_pdb->GetVersion() < 1101)
				{
					m_nFields = 23;
				}
				else
				{
					DFX_Long(pFX, _T("[L_IDCategory]"), m_dwIDCategory);
					DFX_Long(pFX, _T("[L_IDComposer]"), m_dwIDComposer);
					DFX_Text(pFX, _T("[L_Language]"), m_sLanguage);
				}
			}
		}
	}

	if (m_lFormat == AFX_RFX_LONG_PSEUDO_NULL)
		m_lFormat = 0;

	if (m_lBitRate == AFX_RFX_LONG_PSEUDO_NULL)
		m_lBitRate = 0;

	if (m_lSampleRate == AFX_RFX_LONG_PSEUDO_NULL)
		m_lSampleRate = 0;

	if (m_lChannels == AFX_RFX_LONG_PSEUDO_NULL)
		m_lChannels = 0;

	// Wenn das Feld leer ist, ist das Jahr 0 (unbekannt).
	if (m_lYearRecorded == AFX_RFX_LONG_PSEUDO_NULL)
		m_lYearRecorded = 0;

	// Wenn das Feld leer ist, ist das Rating 0 (unbekannt).
	if (m_lRating == AFX_RFX_LONG_PSEUDO_NULL)
		m_lRating = 0;

	// Wenn das Feld leer ist, nehmen wir als Standardwert 0.
	if (m_dwIDComposer == AFX_RFX_LONG_PSEUDO_NULL)
		m_dwIDComposer = 0;

	// Wenn das Feld leer ist, nehmen wir als Standardwert 0.
	if (m_dwIDCategory == AFX_RFX_LONG_PSEUDO_NULL)
		m_dwIDCategory = 0;
}

void CTrack::FillRecordset(CDaoRecordset *prs)
{
	prs->SetFieldValue(L"IDLied", m_dwID);
	prs->SetFieldValue(L"IDCD", m_dwIDCD);
	prs->SetFieldValue(L"IDArtist", m_dwIDArtist);
	prs->SetFieldValue(L"wLiedNummer", m_wTrack);
	prs->SetFieldValue(L"szTitel", (LPCTSTR)m_sTitle);
	prs->SetFieldValue(L"dwLaenge", m_dwLength);
	prs->SetFieldValue(L"wBpm", m_wBpm);
	prs->SetFieldValue(L"szNameRecDatei", (LPCTSTR)m_sWavePath);
	prs->SetFieldValue(L"szKennzeichen", (LPCTSTR)m_sCodes);
	prs->SetFieldValue(L"szKommentar", (LPCTSTR)m_sComment);
	prs->SetFieldValue(L"szLiedText", (LPCTSTR)m_sLyrics);
	prs->SetFieldValue(L"L_TrackFormat", m_lFormat);
	prs->SetFieldValue(L"L_BitRate", m_lBitRate);
	prs->SetFieldValue(L"L_SampleRate", m_lSampleRate);
	prs->SetFieldValue(L"L_Channels", m_lChannels);
	prs->SetFieldValue(L"L_YearRecorded", m_lYearRecorded);
	prs->SetFieldValue(L"L_CheckSum", (LPCTSTR)m_sCheckSum);
	prs->SetFieldValue(L"L_Rating", m_lRating);
	prs->SetFieldValue(L"L_IDCategory", m_dwIDCategory);
	prs->SetFieldValue(L"L_IDComposer", m_dwIDComposer);
	prs->SetFieldValue(L"L_Language", (LPCTSTR)m_sLanguage);

	if (m_pdb->GetVersion() < 801)
	{
		for (int i=0;i<2;i++)
		{
			CString sField;
			sField.Format(L"szFeld%d", i+1);
			prs->SetFieldValue(sField, (LPCTSTR)m_sUser[i]);
		}

		m_nFields = 17;
	}
	else
	{
		for (int i=0;i<MAX_USER_FIELDS;i++)
		{
			CString sField;
			sField.Format(L"szFeld%d", i+1);
			prs->SetFieldValue(sField, (LPCTSTR)m_sUser[i]);
		}
	}
}

/////////////////////////////////////////////////////////////////////////////
// CArtist diagnostics

#ifdef _DEBUG
void CTrack::AssertValid() const
{
	CDaoRecordset::AssertValid();
}

void CTrack::Dump(CDumpContext& dc) const
{
	CDaoRecordset::Dump(dc);
}
#endif //_DEBUG


CCD* CTrack::GetCD()
{
	// Wurde der Interpret schon mal gelesen?
	if (!m_pCD || m_dwIDCD != m_pCD->m_dwID)
	{
		if (m_pCD)
		{
			delete m_pCD;
			m_pCD = NULL;
		}

		// OK, jetzt lesen!
		m_pCD = new CCD(m_pdb);

		// Interpreten jetzt lesen!
		CString sWhere;
		sWhere.Format(L"IDCD=%d", m_dwIDCD);
		m_pCD->QueryStart(sWhere);

		BOOL bFound = m_pCD->QueryFindFirst();
		ASSERT(bFound);

		m_pCD->QueryEnd();
	}

	ASSERT (m_pCD);

	return m_pCD;
}

CArtist* CTrack::GetArtist()
{
	// Wurde der Interpret schon mal gelesen?
	if (!m_pArtist || m_dwIDArtist != m_pArtist->m_dwID)
	{
		if (m_pArtist)
		{
			delete m_pArtist;
			m_pArtist = NULL;
		}

		// OK, jetzt lesen!
		m_pArtist = new CArtist(m_pdb);

		if (m_dwIDArtist)
		{
			// Interpreten jetzt lesen!
			CString sWhere;
			sWhere.Format(L"IDArtist=%d", m_dwIDArtist);
			m_pArtist->QueryStart(sWhere);

			BOOL bFound = m_pArtist->QueryFindFirst();
			ASSERT(bFound);

			m_pArtist->QueryEnd();
		}
	}

	ASSERT (m_pArtist);

	return m_pArtist;
}

CArtist* CTrack::GetComposer()
{
	// Wurde der Interpret schon mal gelesen?
	if (!m_pComposer || m_dwIDComposer != m_pComposer->m_dwID)
	{
		if (m_pComposer)
		{
			delete m_pComposer;
			m_pComposer = NULL;
		}

		// OK, jetzt lesen!
		m_pComposer = new CArtist(m_pdb);

		if (m_dwIDComposer)
		{
			// Komponisten jetzt lesen!
			CString sWhere;
			sWhere.Format(L"IDArtist=%d", m_dwIDComposer);
			m_pComposer->QueryStart(sWhere);

			BOOL bFound = m_pComposer->QueryFindFirst();
			ASSERT(bFound);

			m_pComposer->QueryEnd();
		}
	}

	ASSERT (m_pComposer);

	return m_pComposer;
}

CCategory* CTrack::GetCategory()
{
	// Wurde die Identity schon mal gelesen?
	if (!m_pCategory || m_pCategory->m_dwID != m_dwIDCategory)
	{
		if (m_pCategory)
		{
			delete m_pCategory;
			m_pCategory = NULL;
		}

		// OK, jetzt lesen!
		m_pCategory = new CCategory(m_pdb);

		if (m_dwIDCategory)
		{
			// Interpreten jetzt lesen!
			CString sWhere;
			sWhere.Format(L"IDKategorie=%d", m_dwIDCategory);
			m_pCategory->QueryStart(sWhere);

			BOOL bFound = m_pCategory->QueryFindFirst();
			// Es muß keins gefunden werden!

			m_pCategory->QueryEnd();
		}
	}

	ASSERT (m_pCategory);

	return m_pCategory;
}


// Das aktuelle Lied speichern!

BOOL CTrack::Write()
{
	if (GetCD()->m_bSampler)
	{
		m_dwIDArtist = m_pdb->GetIDFromArtist(GetArtist()->m_sArtist);
	}
	else
	{
		m_dwIDArtist = GetCD()->m_dwIDArtist;
	}

	if (!GetComposer()->m_sArtist.IsEmpty())
		m_dwIDComposer = m_pdb->GetIDFromArtist(GetComposer()->m_sArtist);
	else
		m_dwIDComposer = 0;

	if (!GetCategory()->m_sCategory.IsEmpty())
	    m_dwIDCategory = m_pdb->GetIDFromCategory(GetCategory()->m_sCategory);
	else
		m_dwIDCategory = 0;

    CTrack Track = *this;

	// Tabellennamen ermitteln, um ihn für die Erstellung der ID zu benutzen
	CString sTablename = GetDefaultSQL().Mid(1);
	sTablename = sTablename.Left(sTablename.GetLength()-1);

	BOOL bAddNew = (m_dwID <= 0);

	// Die ID heißt in unseren Tabellen immer ID[Tabellenname]
	CString sWhere;
	sWhere.Format(L"ID%s = %ld", sTablename, m_dwID);
	if (!QueryStart(sWhere))
		return FALSE;

	//!!!!!!!!! Noch Fehlerbehandlung!!
	if (IsEOF())
		AddNew();
	else
		Edit();

	DWORD dwID = GetFieldValue(L"IDLied").lVal;
	*this = Track;
	m_dwID = dwID;

	Update();
	
	QueryEnd();

	*this = Track;
	m_dwID = dwID;

	return TRUE;
}

BOOL CTrack::Add()
{
	if (GetCD()->m_bSampler)
	{
		m_dwIDArtist = m_pdb->GetIDFromArtist(GetArtist()->m_sArtist);
		m_dwIDComposer = m_pdb->GetIDFromArtist(GetComposer()->m_sArtist);
	}
	else
	{
		m_dwIDArtist = GetCD()->m_dwIDArtist;
		m_dwIDComposer = GetCD()->m_dwIDComposer;
	}

	if (!GetCategory()->m_sCategory.IsEmpty())
	    m_dwIDCategory = m_pdb->GetIDFromCategory(GetCategory()->m_sCategory);
	else
		m_dwIDCategory = 0;

    CTrack Track = *this;

	if (!QueryStart())
		return FALSE;

	AddNew();

	DWORD dwID = GetFieldValue(L"IDLied").lVal;
	*this = Track;
	m_dwID = dwID;

	Update();
	
	QueryEnd();

	*this = Track;
	m_dwID = dwID;

	return TRUE;
}

// Alle Lieder auf einmal hinzufügen. Geht so viel schneller, als jeden einzelnen Track
// wegzuschreiben.
BOOL CTrackList::AddAll(CCD* pCD)
{
	CDataBase* pdb = (*this)[0].GetDataBase();
	CDaoRecordset rs(&pdb->m_db);

	try
	{
		rs.Open(dbOpenTable, L"Lied");
	}
	catch (CDaoException* e)
	{
		e->ReportError();
		e->Delete();
		return FALSE;
	}


	for (int i=0;i<pCD->m_wNumberOfTracks;i++)
	{
		if (pCD->m_bSampler)
		{
			(*this)[i].m_dwIDArtist = pdb->GetIDFromArtist((*this)[i].GetArtist()->m_sArtist);
		}
		else
		{
			(*this)[i].m_dwIDArtist = pCD->m_dwIDArtist;
		}

		if (!(*this)[i].GetComposer()->m_sArtist.IsEmpty())
			(*this)[i].m_dwIDComposer = pdb->GetIDFromArtist((*this)[i].GetComposer()->m_sArtist);
		else
			(*this)[i].m_dwIDComposer = 0;

		if (!(*this)[i].GetCategory()->m_sCategory.IsEmpty())
			(*this)[i].m_dwIDCategory = pdb->GetIDFromCategory((*this)[i].GetCategory()->m_sCategory);
		else
			(*this)[i].m_dwIDCategory = 0;

		rs.AddNew();

		(*this)[i].m_dwID = rs.GetFieldValue(L"IDLied").lVal;
		GetAt(i).FillRecordset(&rs);

		rs.Update();
	}

	rs.Close();

	return TRUE;
}

COleVariant CTrack::GetField(UINT uiField)
{
	COleVariant var;

	switch (uiField)
	{
	case FIELD_TRACK_NUMBER:
		var = m_wTrack;
		break;
	case FIELD_TRACK_ARTIST:
		var = GetArtist()->m_sArtist;
		break;
	case FIELD_TRACK_TITLE:
		var = m_sTitle;
		break;
	case FIELD_TRACK_LENGTH:
		var = m_dwLength;
		break;
	case FIELD_TRACK_BPM:
		var = m_wBpm;
		break;
	case FIELD_TRACK_CODES:
		var = m_sCodes;
		break;
	case FIELD_TRACK_COMMENT:
		var = m_sComment;
		break;
	case FIELD_TRACK_YEAR_RECORDED:
		var = m_lYearRecorded;
		break;
	case FIELD_TRACK_USER1:
		var = m_sUser[0];
		break;
	case FIELD_TRACK_USER2:
		var = m_sUser[1];
		break;
	case FIELD_TRACK_USER3:
		var = m_sUser[2];
		break;
	case FIELD_TRACK_USER4:
		var = m_sUser[3];
		break;
	case FIELD_TRACK_USER5:
		var = m_sUser[4];
		break;
	case FIELD_TRACK_LYRICS:
		var = m_sLyrics;
		break;
	case FIELD_TRACK_SOUNDFILE:
		var = m_sWavePath;
		break;
	case FIELD_TRACK_RATING:
		var = m_lRating;
		break;
	case FIELD_TRACK_CHECKSUM:
		var = m_sCheckSum;
		break;
	case FIELD_TRACK_CATEGORY:
		//TODO!!!!!!!!!!!!!!!!!!!!
		break;
	case FIELD_TRACK_LANGUAGE:
		var = m_sLanguage;
		break;
	case FIELD_ARTIST_TRACK_SORTKEY:
		var = GetArtist()->m_sSortKey;
		break;
	case FIELD_ARTIST_TRACK_GROUPTYPE:
		var = GetArtist()->GetGroupString();
		break;
	case FIELD_ARTIST_TRACK_SEX:
		var = GetArtist()->GetSexString();
		break;
	case FIELD_ARTIST_TRACK_COMMENT:
		var = GetArtist()->m_sComment;
		break;
	case FIELD_ARTIST_TRACK_URL:
		var = GetArtist()->m_sURL;
		break;
	case FIELD_ARTIST_TRACK_COUNTRY:
		var = GetArtist()->m_sCountry;
		break;
	case FIELD_ARTIST_TRACK_BIRTHDAY:
		var = GetArtist()->m_dtBirthDay;
		break;
	case FIELD_ARTIST_TRACK_DAYOFDEATH:
		var = GetArtist()->m_dtDayOfDeath;
		break;
	case FIELD_ARTIST_TRACK_IMAGEFILENAME:
		var = GetArtist()->m_sImageFilename;
		break;

	case FIELD_COMPOSER_TRACK_NAME:
		var = GetComposer()->m_sArtist;
		break;
	case FIELD_COMPOSER_TRACK_SORTKEY:
		var = GetComposer()->m_sSortKey;
		break;
	case FIELD_COMPOSER_TRACK_GROUPTYPE:
		var = GetComposer()->GetGroupString();
		break;
	case FIELD_COMPOSER_TRACK_SEX:
		var = GetComposer()->GetSexString();
		break;
	case FIELD_COMPOSER_TRACK_COMMENT:
		var = GetComposer()->m_sComment;
		break;
	case FIELD_COMPOSER_TRACK_URL:
		var = GetComposer()->m_sURL;
		break;
	case FIELD_COMPOSER_TRACK_COUNTRY:
		var = GetComposer()->m_sCountry;
		break;
	case FIELD_COMPOSER_TRACK_BIRTHDAY:
		var = GetComposer()->m_dtBirthDay;
		break;
	case FIELD_COMPOSER_TRACK_DAYOFDEATH:
		var = GetComposer()->m_dtDayOfDeath;
		break;
	case FIELD_COMPOSER_TRACK_IMAGEFILENAME:
		var = GetComposer()->m_sImageFilename;
		break;
	default:
		ASSERT(false);
	}

	return var;
}

CString CTrack::GetFieldAsString(UINT uiField)
{
	CString sValue;

	switch (uiField)
	{
	case FIELD_TRACK_NUMBER:
		sValue.Format(L"%d", m_wTrack);
		break;
	case FIELD_TRACK_ARTIST:
		sValue = GetArtist()->m_sArtist;
		break;
	case FIELD_TRACK_TITLE:
		sValue = m_sTitle;
		break;
	case FIELD_TRACK_LENGTH:
		sValue = CMisc::Long2Time(m_dwLength);
		break;
	case FIELD_TRACK_BPM:
		sValue.Format(L"%d", m_wBpm);
		break;
	case FIELD_TRACK_CODES:
		sValue = m_sCodes;
		break;
	case FIELD_TRACK_COMMENT:
		sValue = m_sComment;
		break;
	case FIELD_TRACK_YEAR_RECORDED:
		sValue.Format(L"%d",  m_lYearRecorded);
		break;
	case FIELD_TRACK_USER1:
		sValue = m_sUser[0];
		break;
	case FIELD_TRACK_USER2:
		sValue = m_sUser[1];
		break;
	case FIELD_TRACK_USER3:
		sValue = m_sUser[2];
		break;
	case FIELD_TRACK_USER4:
		sValue = m_sUser[3];
		break;
	case FIELD_TRACK_USER5:
		sValue = m_sUser[4];
		break;
	case FIELD_TRACK_LYRICS:
		sValue = m_sLyrics;
		break;
	case FIELD_TRACK_SOUNDFILE:
		sValue = m_sWavePath;
		break;
	case FIELD_TRACK_RATING:
		sValue.Format(L"%d", m_lRating);
		break;
	case FIELD_TRACK_CHECKSUM:
		sValue = m_sCheckSum;
		break;
	case FIELD_TRACK_CATEGORY:
		//TODO!!!!!!!!!!!!!!!!!!!!!!!!!
		break;
	case FIELD_TRACK_LANGUAGE:
		sValue = m_sLanguage;
		break;
	case FIELD_ARTIST_TRACK_SORTKEY:
		sValue = GetArtist()->m_sSortKey;
		break;
	case FIELD_ARTIST_TRACK_GROUPTYPE:
		sValue = GetArtist()->GetGroupString();
		break;
	case FIELD_ARTIST_TRACK_SEX:
		sValue = GetArtist()->GetSexString();
		break;
	case FIELD_ARTIST_TRACK_COMMENT:
		sValue = GetArtist()->m_sComment;
		break;
	case FIELD_ARTIST_TRACK_URL:
		sValue = GetArtist()->m_sURL;
		break;
	case FIELD_ARTIST_TRACK_COUNTRY:
		sValue = GetArtist()->m_sCountry;
		break;
	case FIELD_ARTIST_TRACK_BIRTHDAY:
		sValue = GetArtist()->m_dtBirthDay.Format(VAR_DATEVALUEONLY);
		break;
	case FIELD_ARTIST_TRACK_DAYOFDEATH:
		sValue = GetArtist()->m_dtDayOfDeath.Format(VAR_DATEVALUEONLY);
		break;
	case FIELD_ARTIST_TRACK_IMAGEFILENAME:
		sValue = GetArtist()->m_sImageFilename;
		break;

	case FIELD_COMPOSER_TRACK_NAME:
		sValue = GetComposer()->m_sArtist;
		break;
	case FIELD_COMPOSER_TRACK_SORTKEY:
		sValue = GetComposer()->m_sSortKey;
		break;
	case FIELD_COMPOSER_TRACK_GROUPTYPE:
		sValue = GetComposer()->GetGroupString();
		break;
	case FIELD_COMPOSER_TRACK_SEX:
		sValue = GetComposer()->GetSexString();
		break;
	case FIELD_COMPOSER_TRACK_COMMENT:
		sValue = GetComposer()->m_sComment;
		break;
	case FIELD_COMPOSER_TRACK_URL:
		sValue = GetComposer()->m_sURL;
		break;
	case FIELD_COMPOSER_TRACK_COUNTRY:
		sValue = GetComposer()->m_sCountry;
		break;
	case FIELD_COMPOSER_TRACK_BIRTHDAY:
		sValue = GetComposer()->m_dtBirthDay.Format(VAR_DATEVALUEONLY);
		break;
	case FIELD_COMPOSER_TRACK_DAYOFDEATH:
		sValue = GetComposer()->m_dtDayOfDeath.Format(VAR_DATEVALUEONLY);
		break;
	case FIELD_COMPOSER_TRACK_IMAGEFILENAME:
		sValue = GetComposer()->m_sImageFilename;
		break;
	default:
		ASSERT(false);
	}

	return sValue;
}

void CTrack::SetField(UINT uiField, const COleVariant& var)
{
	switch (uiField)
	{
	case FIELD_TRACK_NUMBER:
		m_wTrack = var.iVal;
		break;
	case FIELD_TRACK_ARTIST:
		GetArtist()->m_sArtist = var.bstrVal;
		break;
	case FIELD_TRACK_TITLE:
		m_sTitle = var.bstrVal;
		break;
	case FIELD_TRACK_LENGTH:
		m_dwLength = var.lVal;
		break;
	case FIELD_TRACK_BPM:
		m_wBpm = var.iVal;
		break;
	case FIELD_TRACK_CODES:
		m_sCodes = var.bstrVal;
		break;
	case FIELD_TRACK_COMMENT:
		m_sComment = var.bstrVal;
		break;
	case FIELD_TRACK_YEAR_RECORDED:
		m_lYearRecorded = var.lVal;
		break;
	case FIELD_TRACK_USER1:
		m_sUser[0] = var.bstrVal;
		break;
	case FIELD_TRACK_USER2:
		m_sUser[1] = var.bstrVal;
		break;
	case FIELD_TRACK_USER3:
		m_sUser[2] = var.bstrVal;
		break;
	case FIELD_TRACK_USER4:
		m_sUser[3] = var.bstrVal;
		break;
	case FIELD_TRACK_USER5:
		m_sUser[4] = var.bstrVal;
		break;
	case FIELD_TRACK_LYRICS:
		m_sLyrics = var.bstrVal;
		break;
	case FIELD_TRACK_SOUNDFILE:
		m_sWavePath = var.bstrVal;
		break;
	case FIELD_TRACK_RATING:
		m_lRating = var.lVal;
		break;
	case FIELD_TRACK_CHECKSUM:
		m_sCheckSum = var.bstrVal;
		break;
	case FIELD_TRACK_CATEGORY:
		//TODO!!!!!!!!!!!!!!!!!!!!!!!!!
		break;
	case FIELD_TRACK_LANGUAGE:
		m_sLanguage = var.bstrVal;
		break;
	case FIELD_COMPOSER_TRACK_NAME:
		GetComposer()->m_sArtist = var.bstrVal;
		break;
	default:
		ASSERT(false);
	}
}
