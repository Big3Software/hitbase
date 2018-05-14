// CDQuery.cpp: Implementierung der Klasse CCDQuery.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "hitdb.h"
#include "../../app/hitbase/resource.h"
#include "CDQuery.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Konstruktion/Destruktion
//////////////////////////////////////////////////////////////////////

CCDQuery::CCDQuery(CDataBase* pDataBase)
{
	m_pDataBase = pDataBase;
	m_pDBQuery = NULL;
	m_bSearchStopped = FALSE;
}

CCDQuery::~CCDQuery()
{
	if (m_pDBQuery)
	{
		if (m_pDBQuery->IsOpen())
			m_pDBQuery->QueryEnd();
		delete m_pDBQuery;
	}
}

int CCDQuery::ReadRecordset()
{
	int i, RecordsRead=0;
	COleVariant var;

	if (m_pDBQuery->IsEOF() || m_bSearchStopped)
		return 0;

	// Artist-Tabelle (von der CD)
	m_Artist.m_sArtist = GetFieldString("Artist.szArtistName");  // CD-Name (Artistenname)
	m_Artist.m_sSortKey = GetFieldString("Artist.sSortKey");     // SortKey 
	m_Artist.m_nGroup = GetFieldShort("Artist.nGroup");     // Group 
	m_Artist.m_nSex = GetFieldShort("Artist.nSex");     // Sex 
	m_Artist.m_sComment = GetFieldString("Artist.sComment");     // Comment
	m_Artist.m_sURL = GetFieldString("Artist.A_URL");     // URL
	m_Artist.m_sCountry = GetFieldString("Artist.A_Country");     // Country
	
	// Version 11
	m_Artist.m_dtBirthDay = GetFieldDateTime("Artist.A_BirthDay");     
	m_Artist.m_dtDayOfDeath = GetFieldDateTime("Artist.A_DayOfDeath");
	m_Artist.m_sImageFilename = GetFieldString("Artist.A_ImageFilename");

	// Composer (von der CD)
	m_Composer.m_sArtist = GetFieldString("Composer.szArtistName");  // CD-Name (Artistenname)
	m_Composer.m_sSortKey = GetFieldString("Composer.sSortKey");     // SortKey 
	m_Composer.m_nGroup = GetFieldShort("Composer.nGroup");     // Group 
	m_Composer.m_nSex = GetFieldShort("Composer.nSex");     // Sex 
	m_Composer.m_sComment = GetFieldString("Composer.sComment");     // Comment
	m_Composer.m_sURL = GetFieldString("Composer.A_URL");     // URL
	m_Composer.m_sCountry = GetFieldString("Composer.A_Country");     // Country
	
	// Version 11
	m_Composer.m_dtBirthDay = GetFieldDateTime("Composer.A_BirthDay");     
	m_Composer.m_dtDayOfDeath = GetFieldDateTime("Composer.A_DayOfDeath");
	m_Composer.m_sImageFilename = GetFieldString("Composer.A_ImageFilename");

	// CD-Tabelle
	m_dwID = GetFieldLong(m_QueryType == CCDQuery::queryCDOnly ? "IDCD" : "CD.IDCD");						    // Recordnummer (idcd)
	m_dwTotalLength = GetFieldLong("dwGesamtlaenge");		    // Gesamtlänge
	m_wNumberOfTracks = GetFieldLong("cAnzahlLieder");           // Anzahl von Liedern
	m_bSampler = GetFieldBool("bCDSampler");						// Ist diese CD ein Sampler
	m_wCDSetNumber = GetFieldShort("wNummerImSet");				// Die CDSet Nummer falls vorhanden
	m_sCDSetName = GetFieldString("szCDSetName");				// CD-Set-Name
	m_sCategory = GetFieldString(m_QueryType == CCDQuery::queryCDOnly ? "szKategorieName" : "Kategorie.szKategorieName");				// Kategorie
	m_sMedium = GetFieldString("szMedium");						// Medium
	m_sTitle = GetFieldString(m_QueryType == CCDQuery::queryCDOnly ? "szTitel" : "CD.szTitel");						// CD Titel (bzw. Titel 2)
	m_sDate = GetFieldString("szDatum");							// Das Datumsfeld
	m_sArchiveNumber = GetFieldString("szArchivNummer");			// Die ArchivNummer
	m_sBitmapPath = GetFieldString("szPfadBitmap");				// Der Bitmapname der Datei
	m_sCodes = GetFieldString(m_QueryType == CCDQuery::queryCDOnly ? "szKennzeichen" : "CD.szKennzeichen");				// Die Kennzeichen
	m_sComment = GetFieldString(m_QueryType == CCDQuery::queryCDOnly ? "szKommentar" : "CD.szKommentar");				// Kommentar
	m_lYearRecorded = GetFieldLong("C_YearRecorded");			// Year Recorded
	m_sCopyright = GetFieldString("C_Copyright");				// Copyright

	// Version 10
	m_bOriginal = GetFieldBool("C_Original");		            // Original-CD (kann ins CD-Archiv übertragen werden)
	m_sBackCoverFilename = GetFieldString("C_BackCoverBitmap");	// Dateiname des Back-Cover (Grafik)
	m_sCDLabelBitmap = GetFieldString("C_CDLabelBitmap");		// Dateiname des CD-Labels (Grafik)
	m_lRating = GetFieldLong("C_Rating");						// Rating der CD (z.Zt. 1-5 Sterne)
	m_sLabel = GetFieldString("C_Label");						// Label (z.B. Sony, EMI, etc.)
	m_sURL = GetFieldString("C_URL");							// Verknüpfungen (getrennt mit Semikolon)
	m_lPrice = GetFieldLong("C_Price");							// Preis (Wähung * 100, z.B. 29,99€ = 2999)
	m_sUPC = GetFieldString("C_UPC");							// UPC oder EAN Code

	// Version 11
	m_sLanguage = GetFieldString("C_Language");				// Sprache
	m_sLocation = GetFieldString("C_Location");				// Standort

	// Benutzerdefinierte Felder (CD)
	m_sUser[0] = GetFieldString(m_QueryType == CCDQuery::queryCDOnly ? "szFeld1" : "CD.szFeld1");
	m_sUser[1] = GetFieldString(m_QueryType == CCDQuery::queryCDOnly ? "szFeld2" : "CD.szFeld2");
	m_sUser[2] = GetFieldString(m_QueryType == CCDQuery::queryCDOnly ? "szFeld3" : "CD.szFeld3");
	m_sUser[3] = GetFieldString(m_QueryType == CCDQuery::queryCDOnly ? "szFeld4" : "CD.szFeld4");
	m_sUser[4] = GetFieldString(m_QueryType == CCDQuery::queryCDOnly ? "szFeld5" : "CD.szFeld5");

	if (m_QueryType != CCDQuery::queryCDOnly)
	{
		// JUS 01.11.2002: Bei defekten Datenbanken kann der Wert m_wNumberOfTracks von den 
		// tatsächlich gespeicherten Liedern abweichen. Deshalb hier nur noch nach der RecordNumber gehen.
		//   for (i=0; i < m_wNumberOfTracks; i++)
		for (i=0; ; i++)
		{
			// ACHTUNG: Wenn die ID nicht mehr gleich ist, dann abbrechen!
			long lTrackIDCD = GetFieldLong("CD.IDCD");
			if (lTrackIDCD != m_dwID)
				break;

			// Lied-Nummer auf CD
			short iTrack;

			if (m_QueryType == CCDQuery::queryTrack)
				iTrack = 1;
			else
				iTrack = GetFieldShort("wLiedNummer");

			if (iTrack > m_Track.GetCount())
			{
				m_Track.SetSize(iTrack);
				m_ArtistTrack.SetSize(iTrack);
				m_ComposerTrack.SetSize(iTrack);
			}

			iTrack--;    // Fängt bei 1 an.

			m_Track[iTrack].m_wTrack = GetFieldShort("wLiedNummer");		// Liednummer
			m_Track[iTrack].m_dwID = GetFieldLong("IDLied");				// ID des Liedes
			m_Track[iTrack].m_sTitle = GetFieldString("Lied.szTitel");		// Name Lied
			m_Track[iTrack].m_dwLength = GetFieldLong("dwLaenge");			// Länge Lied
			m_Track[iTrack].m_wBpm = GetFieldShort("wBpm");					// Bpm Lied
			m_Track[iTrack].m_sWavePath = GetFieldString("szNameRecDatei");	// Record-Datei Lied
			m_Track[iTrack].m_sCodes = GetFieldString("Lied.szKennzeichen");// Kennzeichen Lied
			m_Track[iTrack].m_sComment = GetFieldString("Lied.szKommentar");// Kommentar Lied
			m_Track[iTrack].m_sLyrics = GetFieldString("szLiedtext");		// Liedtext Lied
			m_Track[iTrack].m_lYearRecorded = GetFieldLong("L_YearRecorded");// Year Recorded
			m_Track[iTrack].m_lFormat = GetFieldLong("L_TrackFormat");			// Format des Liedes (Audio-Track, MP3, WMA, ...)
			m_Track[iTrack].m_lBitRate = GetFieldLong("L_BitRate");			// Bitrate des Liedes (z.b. 128 KBit = 131072)
			m_Track[iTrack].m_lSampleRate = GetFieldLong("L_SampleRate");	// Samplerate des Liedes (z.b. 44100 Hz)
			m_Track[iTrack].m_lChannels = GetFieldLong("L_Channels");		// Anzahl der Kanäle (0 = stereo (default), 1 = mono, 2 = stereo)

			m_Track[iTrack].m_sUser[0] = GetFieldString("Lied.szFeld1");	// Benutzerdefiniertes Feld 1 Lied
			m_Track[iTrack].m_sUser[1] = GetFieldString("Lied.szFeld2");	// Benutzerdefiniertes Feld 2 Lied
			m_Track[iTrack].m_sUser[2] = GetFieldString("Lied.szFeld3");	// Benutzerdefiniertes Feld 3 Lied
			m_Track[iTrack].m_sUser[3] = GetFieldString("Lied.szFeld4");	// Benutzerdefiniertes Feld 4 Lied
			m_Track[iTrack].m_sUser[4] = GetFieldString("Lied.szFeld5");	// Benutzerdefiniertes Feld 5 Lied

			// Version 10
			m_Track[iTrack].m_sCheckSum = GetFieldString("L_Checksum");// Checksum (MD5)
			m_Track[iTrack].m_lRating = GetFieldLong("L_Rating");		// Rating des Liedes

			// Version 11
			m_Track[iTrack].m_sLanguage = GetFieldString("L_Language");		// Sprache
			m_Track[iTrack].m_sCategory = GetFieldString("KategorieTrack.szKategorieName");				// Kategorie

			// Artist des Interpreten
			m_ArtistTrack[iTrack].m_sArtist = GetFieldString("ArtistTrack.szArtistName");   // ArtistenName des Liedes (für Sampler)
			m_ArtistTrack[iTrack].m_sSortKey = GetFieldString("ArtistTrack.sSortKey");     // SortKey 
			m_ArtistTrack[iTrack].m_nGroup = GetFieldShort("ArtistTrack.nGroup");     // Group 
			m_ArtistTrack[iTrack].m_nSex = GetFieldShort("ArtistTrack.nSex");     // Sex 
			m_ArtistTrack[iTrack].m_sComment = GetFieldString("ArtistTrack.sComment");     // Comment
			m_ArtistTrack[iTrack].m_sURL = GetFieldString("ArtistTrack.A_URL");     // URL
			m_ArtistTrack[iTrack].m_sCountry = GetFieldString("ArtistTrack.A_Country");     // Country

			// Version 11
			m_ArtistTrack[iTrack].m_dtBirthDay = GetFieldDateTime("ArtistTrack.A_BirthDay");     
			m_ArtistTrack[iTrack].m_dtDayOfDeath = GetFieldDateTime("ArtistTrack.A_DayOfDeath");
			m_ArtistTrack[iTrack].m_sImageFilename = GetFieldString("ArtistTrack.A_ImageFilename");

			// Composer des Interpreten
			m_ComposerTrack[iTrack].m_sArtist = GetFieldString("ComposerTrack.szArtistName");   // ArtistenName des Liedes (für Sampler)
			m_ComposerTrack[iTrack].m_sSortKey = GetFieldString("ComposerTrack.sSortKey");     // SortKey 
			m_ComposerTrack[iTrack].m_nGroup = GetFieldShort("ComposerTrack.nGroup");     // Group 
			m_ComposerTrack[iTrack].m_nSex = GetFieldShort("ComposerTrack.nSex");     // Sex 
			m_ComposerTrack[iTrack].m_sComment = GetFieldString("ComposerTrack.sComment");     // Comment
			m_ComposerTrack[iTrack].m_sURL = GetFieldString("ComposerTrack.A_URL");     // URL
			m_ComposerTrack[iTrack].m_sCountry = GetFieldString("ComposerTrack.A_Country");     // Country

			// Version 11
			m_ComposerTrack[iTrack].m_dtBirthDay = GetFieldDateTime("ComposerTrack.A_BirthDay");     
			m_ComposerTrack[iTrack].m_dtDayOfDeath = GetFieldDateTime("ComposerTrack.A_DayOfDeath");
			m_ComposerTrack[iTrack].m_sImageFilename = GetFieldString("ComposerTrack.A_ImageFilename");

			RecordsRead++;
			m_pDBQuery->QueryFindNext();
			if (m_pDBQuery->IsEOF() || m_QueryType == CCDQuery::queryTrack)
				return RecordsRead;
		}
	}
	else
	{
		RecordsRead++;
		m_pDBQuery->QueryFindNext();
		if (m_pDBQuery->IsEOF())
			return RecordsRead;
	}

	return RecordsRead;
}

BOOL CCDQuery::QueryStart(CSelection* pSelection, CProgressCtrl* pProgressCtrl /* = NULL */, BOOL bBackground /* = TRUE */, BOOL bQueryTrack /* = FALSE */)
{
	CString sWhere = m_pDataBase->GetWhereString(pSelection, FALSE, TRUE);

	return QueryStartFreeWhere(sWhere, pSelection, pProgressCtrl, bBackground, bQueryTrack);
}

BOOL CCDQuery::QueryStartFreeWhere(const CString& sWhere, CSelection* pSelection, CProgressCtrl* pProgressCtrl /* = NULL */, BOOL bBackground /* = TRUE */, BOOL bQueryTrack /* = FALSE */)
{
	ASSERT(m_pDataBase->IsDataBaseOpened());

	m_bSearchStopped = FALSE;

	CString sSQLString = "SELECT * FROM ((((Artist INNER JOIN (Kategorie RIGHT JOIN " +
		                 "(Medium RIGHT JOIN (CDSet RIGHT JOIN CD ON CDSet.IDCDSet = CD.IDCDSet) " +
						 "ON Medium.IDMedium = CD.IDMedium) ON Kategorie.IDKategorie = CD.IDKategorie) " +
						 "ON Artist.IDArtist = CD.IDArtist) INNER JOIN (Lied INNER JOIN Artist AS ArtistTrack " +
						 "ON Lied.IDArtist = ArtistTrack.IDArtist) ON CD.IDCD = Lied.IDCD) LEFT JOIN Artist AS ComposerTrack " +
						 "ON Lied.L_IDComposer = ComposerTrack.IDArtist) LEFT JOIN Kategorie AS KategorieTrack " +
						 "ON Lied.L_IDCategory = KategorieTrack.IDKategorie) LEFT JOIN Artist AS Composer " +
						 "ON CD.C_IDComposer = Composer.IDArtist";

	CString sOrder;
	if (pSelection)
		sOrder = pSelection->GetOrderString();

	if (!sWhere.IsEmpty())
		sSQLString += " WHERE " + sWhere;

	if (sOrder.IsEmpty())
		sSQLString += " ORDER BY CD.IDCD";
	else
		sSQLString += " ORDER BY " + sOrder + ", CD.IDCD";

	if (m_pDBQuery)
		delete m_pDBQuery;

	if (bQueryTrack)
		m_QueryType = CCDQuery::queryTrack;
	else
		m_QueryType = CCDQuery::queryAll;

	m_pDBQuery = new CDBQuery(m_pDataBase);

	if (pProgressCtrl)
		m_pDBQuery->SetProgressCtrl(pProgressCtrl, bBackground);

	BOOL bRet = m_pDBQuery->QueryStartFreeSQL(sSQLString);

	if (bRet)
	{
		if (m_pDBQuery->IsEOF())
			return FALSE;

		ReadRecordset();
	}

	return bRet;
}

BOOL CCDQuery::QueryStartCDOnly(CSelection* pSelection, CProgressCtrl* pProgressCtrl /* = NULL */, BOOL bBackground /* = TRUE */)
{
	CString sWhere = m_pDataBase->GetWhereStringCD(*pSelection, FALSE, TRUE);

	return QueryStartCDFreeWhere(sWhere, pSelection, pProgressCtrl, bBackground);
}

BOOL CCDQuery::QueryStartTrack(CSelection* pSelection, CProgressCtrl* pProgressCtrl /* = NULL */, BOOL bBackground /* = TRUE */)
{
	return QueryStart(pSelection, pProgressCtrl, bBackground, TRUE);
}

BOOL CCDQuery::QueryStartCDFreeWhere(const CString& sWhere, CSelection* pSelection, CProgressCtrl* pProgressCtrl /* = NULL */, BOOL bBackground /* = TRUE */)
{
	ASSERT(m_pDataBase->IsDataBaseOpened());

	m_bSearchStopped = FALSE;

	CString sSQLString = "SELECT * FROM (Artist INNER JOIN (Kategorie RIGHT JOIN (Medium RIGHT JOIN (CDSet RIGHT JOIN CD ON " +
		"CDSet.IDCDSet = CD.IDCDSet) ON Medium.IDMedium = CD.IDMedium) ON Kategorie.IDKategorie = CD.IDKategorie) ON " +
		"Artist.IDArtist = CD.IDArtist) LEFT JOIN Artist AS Composer ON CD.C_IDComposer = Composer.IDArtist";

	CString sOrder;
	
	if (pSelection)
		sOrder = pSelection->GetOrderString(TRUE);

	if (!sWhere.IsEmpty())
		sSQLString += " WHERE " + sWhere;

	if (!sOrder.IsEmpty())
		sSQLString += " ORDER BY " + sOrder;

	if (m_pDBQuery)
		delete m_pDBQuery;

	m_QueryType = CCDQuery::queryCDOnly;

	m_pDBQuery = new CDBQuery(m_pDataBase);

	if (pProgressCtrl)
		m_pDBQuery->SetProgressCtrl(pProgressCtrl, bBackground);

	BOOL bRet = m_pDBQuery->QueryStartFreeSQL(sSQLString);

	if (bRet)
	{
		if (m_pDBQuery->IsEOF())
			return FALSE;

		ReadRecordset();
	}

	return bRet;
}

BOOL CCDQuery::GetRecordFromID(long dwID)
{
	CString str;
	str.Format(L"CD.IDCD = %d", dwID);
	return QueryStartFreeWhere(str, NULL);
}

BOOL CCDQuery::QueryFindNext()
{
	if (!ReadRecordset())
		return FALSE;
	else
		return TRUE;
}

BOOL CCDQuery::QueryEnd()
{
	return m_pDBQuery->QueryEnd();
}

long CCDQuery::GetCount()
{
	return m_pDBQuery->GetRecordCount();
}

CString CCDQuery::GetFieldValue(int iField)
{
	CString str;

	switch (iField)
	{
	case FIELD_TOTALLENGTH: str = CMisc::Long2Time(m_dwTotalLength); break;
	case FIELD_NUMBEROFTRACKS:  str.Format(L"%d", m_wNumberOfTracks); break;
	case FIELD_CDSET:  
		if (!m_sCDSetName.IsEmpty())
			str.Format(L"%s (%d)", m_sCDSetName, m_wCDSetNumber); 
		break;
	case FIELD_CDSAMPLER: str.LoadString(m_bSampler == 1 ? TEXT_YES : TEXT_NO); break;
	case FIELD_CDNAME: str = ((CHitbaseWinAppBase*)AfxGetApp())->m_config.m_iArtistDisplay == 0 ? m_Artist.m_sArtist : m_Artist.m_sSortKey; break;
	case FIELD_CDTITLE: str = m_sTitle; break;
	case FIELD_CATEGORY: str = m_sCategory; break;
	case FIELD_DATE: str = m_pDataBase->DateShort2Long(m_sDate); break;
	case FIELD_CODES: str = m_sCodes; break;
	case FIELD_ARCHIVNUMMER: str = m_sArchiveNumber; break;
	case FIELD_MEDIUM: str = m_sMedium; break;
	case FIELD_CDCOMMENT: str = m_sComment; break;
	case FIELD_YEAR_RECORDED: 
		{
			if (m_lYearRecorded > 0)
				str.Format(L"%d", m_lYearRecorded); 
			else
				str = ""; 
			break;
		}
	case FIELD_COPYRIGHT: str = m_sCopyright; break;
	case FIELD_CDCOVER_FILENAME: str = m_sBitmapPath; break;
	case FIELD_CDUSER1: str = m_sUser[0]; break;
	case FIELD_CDUSER2: str = m_sUser[1]; break;
	case FIELD_CDUSER3: str = m_sUser[2]; break;
	case FIELD_CDUSER4: str = m_sUser[3]; break;
	case FIELD_CDUSER5: str = m_sUser[4]; break;

	// Version 10
	case FIELD_CDCOVERBACK_FILENAME: str = m_sBackCoverFilename; break;
	case FIELD_CDCOVERLABEL_FILENAME: str = m_sCDLabelBitmap; break;
	case FIELD_ORIGINAL_CD: str.LoadString(m_bOriginal == 1 ? TEXT_YES : TEXT_NO);  break;
	case FIELD_LABEL: str = m_sLabel; break;
	case FIELD_UPC: str = m_sUPC; break;
	case FIELD_URL: str = m_sURL; break;
	case FIELD_RATING: str.Format(L"%d", m_lRating); break;
	case FIELD_PRICE: str = CMisc::FormatCurrencyValue(m_lPrice); break;

	case FIELD_LANGUAGE: str = m_sLanguage; break;
	case FIELD_LOCATION: str = m_sLocation; break;

	case FIELD_ARTIST_CD_SORTKEY: str = m_Artist.m_sSortKey; break;
	case FIELD_ARTIST_CD_GROUPTYPE: str = CArtist::GetGroupString(m_Artist.m_nGroup); break;
	case FIELD_ARTIST_CD_SEX: str = CArtist::GetSexString(m_Artist.m_nSex); break;
	case FIELD_ARTIST_CD_COMMENT: str = m_Artist.m_sComment; break;
	case FIELD_ARTIST_CD_URL: str = m_Artist.m_sURL; break;
	case FIELD_ARTIST_CD_COUNTRY: str = m_Artist.m_sCountry; break;
	
	case FIELD_ARTIST_CD_BIRTHDAY: str = m_Artist.m_dtBirthDay.Format(VAR_DATEVALUEONLY); break;
	case FIELD_ARTIST_CD_DAYOFDEATH: str = m_Artist.m_dtDayOfDeath.Format(VAR_DATEVALUEONLY); break;
	case FIELD_ARTIST_CD_IMAGEFILENAME: str = m_Artist.m_sImageFilename; break;

	case FIELD_COMPOSER_CD_NAME: str = m_Composer.m_sArtist; break;
	case FIELD_COMPOSER_CD_SORTKEY: str = m_Composer.m_sSortKey; break;
	case FIELD_COMPOSER_CD_GROUPTYPE: str = CArtist::GetGroupString(m_Composer.m_nGroup); break;
	case FIELD_COMPOSER_CD_SEX: str = CArtist::GetSexString(m_Composer.m_nSex); break;
	case FIELD_COMPOSER_CD_COMMENT: str = m_Composer.m_sComment; break;
	case FIELD_COMPOSER_CD_URL: str = m_Composer.m_sURL; break;
	case FIELD_COMPOSER_CD_COUNTRY: str = m_Composer.m_sCountry; break;
	
	case FIELD_COMPOSER_CD_BIRTHDAY: str = m_Composer.m_dtBirthDay.Format(VAR_DATEVALUEONLY); break;
	case FIELD_COMPOSER_CD_DAYOFDEATH: str = m_Composer.m_dtDayOfDeath.Format(VAR_DATEVALUEONLY); break;
	case FIELD_COMPOSER_CD_IMAGEFILENAME: str = m_Composer.m_sImageFilename; break;

	default: ASSERT(false);
	}

	return str;
}

CString CCDQuery::GetTrackFieldValue(int iTrack, int iField)
{
	CString str;

	if (CFieldList::IsCDField(iField))
		return GetFieldValue(iField);

	switch (iField)
	{
	case FIELD_TRACK_ARTIST: str = m_ArtistTrack[iTrack].m_sArtist; break;
	case FIELD_TRACK_TITLE: str = m_Track[iTrack].m_sTitle; break;
	case FIELD_TRACK_LENGTH: str = CMisc::Long2Time(m_Track[iTrack].m_dwLength); break;
	case FIELD_TRACK_NUMBER: str.Format(L"%d", m_Track[iTrack].m_wTrack); break;
	case FIELD_TRACK_BPM: str.Format(L"%d", m_Track[iTrack].m_wBpm); break;
	case FIELD_TRACK_CODES: str = m_Track[iTrack].m_sCodes; break;
	case FIELD_TRACK_COMMENT: str = m_Track[iTrack].m_sComment; break;
	case FIELD_TRACK_YEAR_RECORDED: 
		{
			if (m_Track[iTrack].m_lYearRecorded > 0)
				str.Format(L"%d", m_Track[iTrack].m_lYearRecorded); 
			else
				str = "";
			break;
		}
	case FIELD_TRACK_USER1: str = m_Track[iTrack].m_sUser[0]; break;
	case FIELD_TRACK_USER2: str = m_Track[iTrack].m_sUser[1]; break;
	case FIELD_TRACK_USER3: str = m_Track[iTrack].m_sUser[2]; break;
	case FIELD_TRACK_USER4: str = m_Track[iTrack].m_sUser[3]; break;
	case FIELD_TRACK_USER5: str = m_Track[iTrack].m_sUser[4]; break;
	case FIELD_TRACK_LYRICS: str = m_Track[iTrack].m_sLyrics; break;   // JUS 29.08.2002
	case FIELD_TRACK_SOUNDFILE: str = m_Track[iTrack].m_sWavePath; break;   // JUS 29.08.2002
	case FIELD_TRACK_RATING: str.Format(L"%d", m_Track[iTrack].m_lRating); break;   // JUS 29.08.2002

	// Version 11: 21.05.2007
	case FIELD_TRACK_LANGUAGE: str = m_Track[iTrack].m_sLanguage; break;
	case FIELD_TRACK_CATEGORY: str = m_Track[iTrack].m_sCategory; break;

	case FIELD_ARTIST_TRACK_NAME: str = m_ArtistTrack[iTrack].m_sArtist; break;
	case FIELD_ARTIST_TRACK_SORTKEY: str = m_ArtistTrack[iTrack].m_sSortKey; break;
	case FIELD_ARTIST_TRACK_GROUPTYPE: str = CArtist::GetGroupString(m_ArtistTrack[iTrack].m_nGroup); break;
	case FIELD_ARTIST_TRACK_SEX: str = CArtist::GetSexString(m_ArtistTrack[iTrack].m_nSex); break;
	case FIELD_ARTIST_TRACK_COMMENT: str = m_ArtistTrack[iTrack].m_sComment; break;
	case FIELD_ARTIST_TRACK_URL: str = m_ArtistTrack[iTrack].m_sURL; break;
	case FIELD_ARTIST_TRACK_COUNTRY: str = m_ArtistTrack[iTrack].m_sCountry; break;

	case FIELD_ARTIST_TRACK_BIRTHDAY: str = m_ArtistTrack[iTrack].m_dtBirthDay.Format(VAR_DATEVALUEONLY); break;
	case FIELD_ARTIST_TRACK_DAYOFDEATH: str = m_ArtistTrack[iTrack].m_dtDayOfDeath.Format(VAR_DATEVALUEONLY); break;
	case FIELD_ARTIST_TRACK_IMAGEFILENAME: str = m_ArtistTrack[iTrack].m_sImageFilename; break;

	case FIELD_COMPOSER_TRACK_NAME: str = m_ComposerTrack[iTrack].m_sArtist; break;
	case FIELD_COMPOSER_TRACK_SORTKEY: str = m_ComposerTrack[iTrack].m_sSortKey; break;
	case FIELD_COMPOSER_TRACK_GROUPTYPE: str = CArtist::GetGroupString(m_ComposerTrack[iTrack].m_nGroup); break;
	case FIELD_COMPOSER_TRACK_SEX: str = CArtist::GetSexString(m_ComposerTrack[iTrack].m_nSex); break;
	case FIELD_COMPOSER_TRACK_COMMENT: str = m_ComposerTrack[iTrack].m_sComment; break;
	case FIELD_COMPOSER_TRACK_URL: str = m_ComposerTrack[iTrack].m_sURL; break;
	case FIELD_COMPOSER_TRACK_COUNTRY: str = m_ComposerTrack[iTrack].m_sCountry; break;

	case FIELD_COMPOSER_TRACK_BIRTHDAY: str = m_ComposerTrack[iTrack].m_dtBirthDay.Format(VAR_DATEVALUEONLY); break;
	case FIELD_COMPOSER_TRACK_DAYOFDEATH: str = m_ComposerTrack[iTrack].m_dtDayOfDeath.Format(VAR_DATEVALUEONLY); break;
	case FIELD_COMPOSER_TRACK_IMAGEFILENAME: str = m_ComposerTrack[iTrack].m_sImageFilename; break;

	default: ASSERT(false);
	}

	return str;
}

CString CCDQuery::GetArtistText()
{
	CString str, title;
	
	if (m_bSampler)
	{
		title.LoadString(IDS_TITLE);
		str.Format(L"%s 1", title);
	}
	else
		str.LoadString(IDS_ARTIST);
	
	return str;
}

CString CCDQuery::GetTitleText()
{
	CString str, title;
	
	title.LoadString(IDS_TITLE);
	if (m_bSampler)
	{
		str.Format(L"%s 2", title);
	}
	else
		str = title;
	
	return str;
}

CString CCDQuery::GetFieldString(const CString& sFieldName)
{
	CString sValue;
	COleVariant var;

    var = m_pDBQuery->GetFieldValue(sFieldName);
	if (var.bstrVal)
		sValue = var.bstrVal;
	else
		sValue = "";

	return sValue;
}

short CCDQuery::GetFieldShort(const CString& sFieldName, BOOL bNoPseudoNull /* = FALSE */)
{
	short iValue;
	COleVariant var;

    var = m_pDBQuery->GetFieldValue(sFieldName);

	iValue = var.iVal;

	if (!bNoPseudoNull && var.lVal == AFX_RFX_SHORT_PSEUDO_NULL)
		iValue = 0;

	return iValue;
}

long CCDQuery::GetFieldLong(const CString& sFieldName, BOOL bNoPseudoNull /* = FALSE */)
{
	long lValue;
	COleVariant var;

    var = m_pDBQuery->GetFieldValue(sFieldName);

	lValue = var.lVal;

	if (!bNoPseudoNull && var.lVal == AFX_RFX_LONG_PSEUDO_NULL)
		lValue = 0;

	return lValue;
}

BOOL CCDQuery::GetFieldBool(const CString& sFieldName)
{
	BOOL bValue;
	COleVariant var;

    var = m_pDBQuery->GetFieldValue(sFieldName);

	bValue = var.boolVal ? TRUE : FALSE;

	return bValue;
}

COleDateTime CCDQuery::GetFieldDateTime(const CString& sFieldName)
{
	COleDateTime dtValue;
	COleVariant var;

    var = m_pDBQuery->GetFieldValue(sFieldName);

	dtValue = var.date;

	return dtValue;
}

COleVariant CCDQuery::GetField(const CString& sFieldName)
{
	COleVariant var;

    var = m_pDBQuery->GetFieldValue(sFieldName);

	return var;
}

void CCDQuery::StopSearch()
{
	m_bSearchStopped = TRUE;
}
