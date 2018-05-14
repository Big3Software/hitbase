// HTMLTemplate.cpp : implementation file
//

#include "stdafx.h"
#include "HTMLTemplate.h"
#include "../hitmisc/HitbaseWinAppBase.h"
#include "../../app/hitbase/resource.h"
#include "../hitmisc/misc.h"
#include "../hitmisc/newfiledialog.h"

using namespace Big3::Hitbase::DataBaseEngine;

// CHTMLTemplate

CHTMLTemplate::CHTMLTemplate(Big3::Hitbase::DataBaseEngine::DataBase^ db)
{
	DataBase = db;
	m_dwCounter = 0;
}

CHTMLTemplate::~CHTMLTemplate()
{
}

bool CHTMLTemplate::WriteCD2HTML (Big3::Hitbase::DataBaseEngine::CD^ CD)
{
	int nCount;
	// Now write HTML for each CD
	// Laufende Nummer für jede CD
	m_dwCounter++;
	WriteBuffer (m_sCodeHTMLCDHeader, CD);

	// Now check for Trackinfo
	if (m_sCodeHTMLTRACK.GetLength() > 0)
	{
		for (nCount=0; nCount < CD->NumberOfTracks; nCount++)
		{
			// Write each Trackinfo...
			WriteBuffer (m_sCodeHTMLTRACK, CD, nCount);
		}
	}

	if (m_sCodeHTMLCDFooter.GetLength() > 0)
	{
		WriteBuffer (m_sCodeHTMLCDFooter, CD);
	}

	return TRUE;
}

bool CHTMLTemplate::WriteTrack2HTML (Big3::Hitbase::DataBaseEngine::TrackDataView^ Track, int index)
{
	// Now write HTML for each Track
	// Laufende Nummer für jeden Track
	m_dwCounter++;
	WriteBuffer (m_sCodeHTMLTRACK, Track, index);

	return TRUE;
}

bool CHTMLTemplate::WriteBuffer (CString sBuffer, Big3::Hitbase::DataBaseEngine::CD^ CD, int nTrack)
{
	USES_CONVERSION;
	CString sHTMLText;
	
	ReplaceGlobalKeywords (sBuffer);
	ReplaceKeywords (sBuffer, CD, nTrack);

	if (nTrack < 0)
	{
		m_cfHTMLExport.Write(W2A(sBuffer), sBuffer.GetLength());
		return TRUE;
	}
	else
	{
		m_cfHTMLExport.Write(W2A(sBuffer), sBuffer.GetLength());
	}
	return TRUE;
}

bool CHTMLTemplate::WriteBuffer (CString sBuffer, Big3::Hitbase::DataBaseEngine::TrackDataView^ Track, int index)
{
	USES_CONVERSION;
	CString sHTMLText;
	
	ReplaceGlobalKeywords (sBuffer);
	ReplaceKeywords (sBuffer, Track, index);

	m_cfHTMLExport.Write(W2A(sBuffer), sBuffer.GetLength());

	return TRUE;
}

bool CHTMLTemplate::WriteBuffer (CString sBuffer)
{
	USES_CONVERSION;

	CString sHTMLText;
	ReplaceGlobalKeywords (sBuffer);

	m_cfHTMLExport.Write(W2A(sBuffer), sBuffer.GetLength());
	return TRUE;
}

void CHTMLTemplate::ReplaceGlobalKeywords (CString &sBuffer)
{
	CString sBuf;

	sBuf.Format(L"%d", m_dwAnzahlCDs);
	sBuffer.Replace(L"<!INFO_NUMBEROFCDS!>", sBuf);
	sBuf.Format(L"%d", m_dwAnzahlLieder);
	sBuffer.Replace(L"<!INFO_NUMBEROFSONGS!>", sBuf);
	sBuf.Format(L"%d", m_dwAnzahlInterpreten);
	sBuffer.Replace(L"<!INFO_NUMBEROFARTISTS!>", sBuf);
	sBuffer.Replace(L"<!INFO_DATABASEFILENAME!>", Text2HTMLText((CString)System::IO::Path::GetFileName(DataBase->DataBasePath)));
	sBuf.Format(L"%d", m_dwAnzahlLieder);
	sBuffer.Replace(L"<!INFO_DATABASEPATHNAME!>", Text2HTMLText(CString(DataBase->DataBasePath)));
	sBuf.Format(L"%d", m_dwVerliehendeCDs);
	sBuffer.Replace(L"<!INFO_NUMBEROFCDSLOANED!>", sBuf);
	sBuf.Format(L"%d", m_dwCounter);
	sBuffer.Replace(L"<!INFO_COUNTER!>", sBuf);
}

bool CHTMLTemplate::ReplaceKeywords (CString &sBuffer, Big3::Hitbase::DataBaseEngine::CD^ CD, int nTrack)
{
	CString sBuf;

	sBuffer.Replace(L"<!CD_TITLE!>", Text2HTMLText(CD->Title));
	sBuffer.Replace(L"<!CD_ARTIST!>", Text2HTMLText(CD->Artist));
	sBuffer.Replace(L"<!CD_CATEGORY!>", Text2HTMLText(CD->Category));
	sBuffer.Replace(L"<!CD_CODES!>", Text2HTMLText(CD->Codes));
	sBuffer.Replace(L"<!CD_SAMPLER!>", Text2HTMLText(CD->Sampler.ToString()));
	sBuffer.Replace(L"<!CD_COMMENT!>", Text2HTMLText(CD->Comment));
	sBuf.Format(L"%d", CD->NumberOfTracks);
	sBuffer.Replace(L"<!CD_NUMBEROFTRACKS!>", sBuf);
	sBuf.Format (L"%s", (CString)Big3::Hitbase::Miscellaneous::Misc::GetShortTimeString(CD->TotalLength));
	sBuffer.Replace(L"<!CD_TOTALLENGTH!>", sBuf);
	sBuffer.Replace(L"<!CD_SETNAME!>", Text2HTMLText(CD->CDSetName));
	sBuf.Format(L"%d", CD->CDSetNumber);
	sBuffer.Replace(L"<!CD_NUMBEROFSETS!>", sBuf);
	sBuffer.Replace(L"<!CD_BITMAPNAME!>", Text2HTMLText(CD->CDCoverFrontFilename));
	sBuf.Format(L"%d", CD->ID);
	sBuffer.Replace(L"<!CD_RECORDNUMBER!>", sBuf);
	sBuffer.Replace(L"<!CD_NAMEDATEFIELD!>", Text2HTMLText(DataBase->FormatDate(CD->Date)));
	sBuffer.Replace(L"<!CD_ARCHIVENUMBER!>", Text2HTMLText(CD->ArchiveNumber));
	sBuf.Format(L"%d", CD->YearRecorded);
	sBuffer.Replace(L"<!CD_YEARRECORDED!>", sBuf);
	sBuffer.Replace(L"<!CD_COPYRIGHT!>", Text2HTMLText(CD->Copyright));
	sBuffer.Replace(L"<!CD_MEDIUM!>", Text2HTMLText(CD->Medium));
	sBuffer.Replace(L"<!CD_USER1!>", Text2HTMLText(CD->UserField1));
	sBuffer.Replace(L"<!CD_USER2!>", Text2HTMLText(CD->UserField2));
	sBuffer.Replace(L"<!CD_USER3!>", Text2HTMLText(CD->UserField3));
	sBuffer.Replace(L"<!CD_USER4!>", Text2HTMLText(CD->UserField4));
	sBuffer.Replace(L"<!CD_USER5!>", Text2HTMLText(CD->UserField5));
	sBuffer.Replace(L"<!CD_USERFIELDNAME1!>", CString(DataBase->Master->UserCDFields[0]->Name));
	sBuffer.Replace(L"<!CD_USERFIELDNAME2!>", CString(DataBase->Master->UserCDFields[1]->Name));
	sBuffer.Replace(L"<!CD_USERFIELDNAME3!>", CString(DataBase->Master->UserCDFields[2]->Name));
	sBuffer.Replace(L"<!CD_USERFIELDNAME4!>", CString(DataBase->Master->UserCDFields[3]->Name));
	sBuffer.Replace(L"<!CD_USERFIELDNAME5!>", CString(DataBase->Master->UserCDFields[4]->Name));

	sBuffer.Replace(L"<!CD_CDCOVERBACK!>", Text2HTMLText(CD->CDCoverBackFilename));
	sBuffer.Replace(L"<!CD_CDCOVERLABEL!>", Text2HTMLText(CD->CDCoverLabelFilename));
	sBuf.Format(L"%d", CD->Original);
	sBuffer.Replace(L"<!CD_ORIGINAL_CD!>", Text2HTMLText(sBuf));
	sBuffer.Replace(L"<!CD_LABEL!>", Text2HTMLText(CD->Label));
	sBuffer.Replace(L"<!CD_UPC!>", Text2HTMLText(CD->UPC));
	sBuffer.Replace(L"<!CD_URL!>", Text2HTMLText(CD->URL));
	sBuf.Format(L"%d", CD->Rating);
	sBuffer.Replace(L"<!CD_RATING!>", Text2HTMLText(sBuf));
	sBuffer.Replace(L"<!CD_PRICE!>", Text2HTMLText((CString)Big3::Hitbase::Miscellaneous::Misc::FormatCurrencyValue(CD->Price)));
	// NEW
	sBuffer.Replace(L"<!CD_LANGUAGE!>", Text2HTMLText(CD->Language));
	sBuffer.Replace(L"<!CD_LOCATION!>", Text2HTMLText(CD->Location));
	sBuffer.Replace(L"<!CD_COMPOSER_CD_NAME!>", Text2HTMLText(CD->Composer));
	if (nTrack >= 0)
	{
		//array<Big3::Hitbase::DataBaseEngine::CDQueryDataSet::TrackRow^>^ tracks = CD->GetTrackRows();
		ReplaceKeywords(sBuffer, CD->Tracks[nTrack]);
	}

	return TRUE;
}

void CHTMLTemplate::ReplaceKeywords(CString& sBuffer, Big3::Hitbase::DataBaseEngine::Track^ track)
{
	CString sBuf;

	// JUS 07.10.2003
	sBuf.Format (L"%d", track->TrackNumber);
	sBuffer.Replace(L"<!TRACK_NR!>", sBuf);

	sBuffer.Replace(L"<!TRACK_TITLE!>", Text2HTMLText(track->Title));
	sBuffer.Replace(L"<!TRACK_ARTIST!>", Text2HTMLText(track->Artist));
	sBuf.Format (L"%s", (CString)Big3::Hitbase::Miscellaneous::Misc::GetShortTimeString (track->Length));
	sBuffer.Replace(L"<!TRACK_LENGTH!>", sBuf);
	sBuf.Format (L"%d", (int)track->Bpm);
	sBuffer.Replace(L"<!TRACK_BPM!>", sBuf);
	sBuffer.Replace(L"<!TRACK_CODES!>", Text2HTMLText(track->Codes));
	sBuf.Format (L"%d", track->YearRecorded);
	sBuffer.Replace(L"<!TRACK_YEARRECORDED!>", sBuf);
	sBuffer.Replace(L"<!TRACK_COMMENT!>", Text2HTMLText(track->Comment));
	sBuffer.Replace(L"<!TRACK_LYRICS!>", Text2HTMLText(track->Lyrics));
	sBuffer.Replace(L"<!TRACK_USER1!>", Text2HTMLText(track->UserField1));
	sBuffer.Replace(L"<!TRACK_USER2!>", Text2HTMLText(track->UserField2));
	sBuffer.Replace(L"<!TRACK_USER3!>", Text2HTMLText(track->UserField3));
	sBuffer.Replace(L"<!TRACK_USER4!>", Text2HTMLText(track->UserField4));
	sBuffer.Replace(L"<!TRACK_USER5!>", Text2HTMLText(track->UserField5));
	sBuffer.Replace(L"<!TRACK_USERFIELDNAME1!>", CString(DataBase->Master->UserTrackFields[0]->Name));
	sBuffer.Replace(L"<!TRACK_USERFIELDNAME2!>", CString(DataBase->Master->UserTrackFields[1]->Name));
	sBuffer.Replace(L"<!TRACK_USERFIELDNAME3!>", CString(DataBase->Master->UserTrackFields[2]->Name));
	sBuffer.Replace(L"<!TRACK_USERFIELDNAME4!>", CString(DataBase->Master->UserTrackFields[3]->Name));
	sBuffer.Replace(L"<!TRACK_USERFIELDNAME5!>", CString(DataBase->Master->UserTrackFields[4]->Name));

	sBuf.Format(L"%d", track->Rating);
	sBuffer.Replace(L"<!TRACK_RATING!>", Text2HTMLText(sBuf));
	sBuffer.Replace(L"<!TRACK_CHECKSUM!>", Text2HTMLText(track->CheckSum));
	// NEW
	sBuffer.Replace(L"<!TRACK_SOUNDFILE!>", Text2HTMLText(track->Soundfile));

	sBuffer.Replace(L"<!TRACK_COMPOSER_TRACK_NAME!>", Text2HTMLText(track->Composer));
}


void CHTMLTemplate::ReplaceKeywords(CString& sBuffer, Big3::Hitbase::DataBaseEngine::TrackDataView^ track, int index)
{
	CString sBuf;

	sBuffer.Replace(L"<!CD_TITLE!>", Text2HTMLText(track->GetRowStringValue(index, Field::Title)));
	sBuffer.Replace(L"<!CD_ARTIST!>", Text2HTMLText(track->GetRowStringValue(index, Field::ArtistCDName)));
	sBuffer.Replace(L"<!CD_CATEGORY!>", Text2HTMLText(track->GetRowStringValue(index, Field::Category)));
	sBuffer.Replace(L"<!CD_CODES!>", Text2HTMLText(track->GetRowStringValue(index, Field::Codes)));
	sBuffer.Replace(L"<!CD_SAMPLER!>", Text2HTMLText(track->GetRowStringValue(index, Field::Sampler)));
	sBuffer.Replace(L"<!CD_COMMENT!>", Text2HTMLText(track->GetRowStringValue(index, Field::Comment)));
	sBuf.Format(L"%d", (int)track->GetRowRawValue(index, Field::NumberOfTracks));
	sBuffer.Replace(L"<!CD_NUMBEROFTRACKS!>", sBuf);
	sBuf.Format (L"%s", (CString)Big3::Hitbase::Miscellaneous::Misc::GetShortTimeString ((int)track->GetRowRawValue(index, Field::TotalLength)));
	sBuffer.Replace(L"<!CD_TOTALLENGTH!>", sBuf);
	sBuffer.Replace(L"<!CD_SETNAME!>", Text2HTMLText(track->GetRowStringValue(index, Field::CDSet)));
	sBuf.Format(L"%d", (int)track->GetRowRawValue(index, Field::DiscNumberInCDSet));
	sBuffer.Replace(L"<!CD_NUMBEROFSETS!>", sBuf);
	sBuffer.Replace(L"<!CD_BITMAPNAME!>", Text2HTMLText(track->GetRowStringValue(index, Field::CDCoverFront)));
//	sBuf.Format(L"%d", track->GetGetRowStringValue(index, Field::ID));
//	sBuffer.Replace(L"<!CD_RECORDNUMBER!>", sBuf);
	sBuffer.Replace(L"<!CD_NAMEDATEFIELD!>", Text2HTMLText(DataBase->FormatDate(track->GetRowStringValue(index, Field::Date))));
	sBuffer.Replace(L"<!CD_ARCHIVENUMBER!>", Text2HTMLText(track->GetRowStringValue(index, Field::ArchiveNumber)));
	sBuf.Format(L"%d", (int)track->GetRowRawValue(index, Field::YearRecorded));
	sBuffer.Replace(L"<!CD_YEARRECORDED!>", sBuf);
	sBuffer.Replace(L"<!CD_COPYRIGHT!>", Text2HTMLText(track->GetRowStringValue(index, Field::Copyright)));
	sBuffer.Replace(L"<!CD_MEDIUM!>", Text2HTMLText(track->GetRowStringValue(index, Field::Medium)));
	sBuffer.Replace(L"<!CD_USER1!>", Text2HTMLText(track->GetRowStringValue(index, Field::User1)));
	sBuffer.Replace(L"<!CD_USER2!>", Text2HTMLText(track->GetRowStringValue(index, Field::User2)));
	sBuffer.Replace(L"<!CD_USER3!>", Text2HTMLText(track->GetRowStringValue(index, Field::User3)));
	sBuffer.Replace(L"<!CD_USER4!>", Text2HTMLText(track->GetRowStringValue(index, Field::User4)));
	sBuffer.Replace(L"<!CD_USER5!>", Text2HTMLText(track->GetRowStringValue(index, Field::User5)));
	sBuffer.Replace(L"<!CD_USERFIELDNAME1!>", CString(DataBase->Master->UserCDFields[0]->Name));
	sBuffer.Replace(L"<!CD_USERFIELDNAME2!>", CString(DataBase->Master->UserCDFields[1]->Name));
	sBuffer.Replace(L"<!CD_USERFIELDNAME3!>", CString(DataBase->Master->UserCDFields[2]->Name));
	sBuffer.Replace(L"<!CD_USERFIELDNAME4!>", CString(DataBase->Master->UserCDFields[3]->Name));
	sBuffer.Replace(L"<!CD_USERFIELDNAME5!>", CString(DataBase->Master->UserCDFields[4]->Name));

	sBuffer.Replace(L"<!CD_CDCOVERBACK!>", Text2HTMLText(track->GetRowStringValue(index, Field::CDCoverBack)));
	sBuffer.Replace(L"<!CD_CDCOVERLABEL!>", Text2HTMLText(track->GetRowStringValue(index, Field::CDCoverLabel)));
	sBuffer.Replace(L"<!CD_ORIGINAL_CD!>", Text2HTMLText(track->GetRowStringValue(index, Field::OriginalCD)));
	sBuffer.Replace(L"<!CD_LABEL!>", Text2HTMLText(track->GetRowStringValue(index, Field::Label)));
	sBuffer.Replace(L"<!CD_UPC!>", Text2HTMLText(track->GetRowStringValue(index, Field::UPC)));
	sBuffer.Replace(L"<!CD_URL!>", Text2HTMLText(track->GetRowStringValue(index, Field::Homepage)));
	sBuf.Format(L"%d", (int)track->GetRowRawValue(index, Field::Rating));
	sBuffer.Replace(L"<!CD_RATING!>", Text2HTMLText(sBuf));
	sBuffer.Replace(L"<!CD_PRICE!>", Text2HTMLText((CString)Big3::Hitbase::Miscellaneous::Misc::FormatCurrencyValue((int)track->GetRowRawValue(index, Field::Price))));
	// NEW
	sBuffer.Replace(L"<!CD_LANGUAGE!>", Text2HTMLText(track->GetRowStringValue(index, Field::Language)));
	sBuffer.Replace(L"<!CD_LOCATION!>", Text2HTMLText(track->GetRowStringValue(index, Field::Location)));
	sBuffer.Replace(L"<!CD_COMPOSER_CD_NAME!>", Text2HTMLText(track->GetRowStringValue(index, Field::ComposerCDName)));


	// JUS 07.10.2003
	sBuffer.Replace(L"<!TRACK_NR!>", (CString)track->GetRowStringValue(index, Field::TrackNumber));

	sBuffer.Replace(L"<!TRACK_TITLE!>", Text2HTMLText(track->GetRowStringValue(index, Field::TrackTitle)));
	sBuffer.Replace(L"<!TRACK_ARTIST!>", Text2HTMLText(track->GetRowStringValue(index, Field::ArtistTrackName)));
	sBuf.Format (L"%s", (CString)Big3::Hitbase::Miscellaneous::Misc::GetShortTimeString ((int)track->GetRowRawValue(index, Field::TrackLength)));
	sBuffer.Replace(L"<!TRACK_LENGTH!>", sBuf);
	sBuf.Format (L"%d", (int)track->GetRowRawValue(index, Field::TrackBpm));
	sBuffer.Replace(L"<!TRACK_BPM!>", sBuf);
	sBuffer.Replace(L"<!TRACK_CODES!>", Text2HTMLText(track->GetRowStringValue(index, Field::TrackCodes)));
	sBuf.Format (L"%d", (int)track->GetRowRawValue(index, Field::TrackYearRecorded));
	sBuffer.Replace(L"<!TRACK_YEARRECORDED!>", sBuf);
	sBuffer.Replace(L"<!TRACK_COMMENT!>", Text2HTMLText(track->GetRowStringValue(index, Field::TrackComment)));
	sBuffer.Replace(L"<!TRACK_LYRICS!>", Text2HTMLText(track->GetRowStringValue(index, Field::TrackLyrics)));
	sBuffer.Replace(L"<!TRACK_USER1!>", Text2HTMLText(track->GetRowStringValue(index, Field::TrackUser1)));
	sBuffer.Replace(L"<!TRACK_USER2!>", Text2HTMLText(track->GetRowStringValue(index, Field::TrackUser2)));
	sBuffer.Replace(L"<!TRACK_USER3!>", Text2HTMLText(track->GetRowStringValue(index, Field::TrackUser3)));
	sBuffer.Replace(L"<!TRACK_USER4!>", Text2HTMLText(track->GetRowStringValue(index, Field::TrackUser4)));
	sBuffer.Replace(L"<!TRACK_USER5!>", Text2HTMLText(track->GetRowStringValue(index, Field::TrackUser5)));
	sBuffer.Replace(L"<!TRACK_USERFIELDNAME1!>", CString(DataBase->Master->UserTrackFields[0]->Name));
	sBuffer.Replace(L"<!TRACK_USERFIELDNAME2!>", CString(DataBase->Master->UserTrackFields[1]->Name));
	sBuffer.Replace(L"<!TRACK_USERFIELDNAME3!>", CString(DataBase->Master->UserTrackFields[2]->Name));
	sBuffer.Replace(L"<!TRACK_USERFIELDNAME4!>", CString(DataBase->Master->UserTrackFields[3]->Name));
	sBuffer.Replace(L"<!TRACK_USERFIELDNAME5!>", CString(DataBase->Master->UserTrackFields[4]->Name));

	sBuffer.Replace(L"<!TRACK_RATING!>", Text2HTMLText(track->GetRowStringValue(index, Field::TrackRating)));
	sBuffer.Replace(L"<!TRACK_CHECKSUM!>", Text2HTMLText(track->GetRowStringValue(index, Field::TrackChecksum)));
	// NEW
	sBuffer.Replace(L"<!TRACK_SOUNDFILE!>", Text2HTMLText(track->GetRowStringValue(index, Field::TrackSoundFile)));

	sBuffer.Replace(L"<!TRACK_COMPOSER_TRACK_NAME!>", Text2HTMLText(track->GetRowStringValue(index, Field::ComposerTrackName)));
}

bool CHTMLTemplate::Close()
{
	// So - jetzt noch den Rest in die HTML Datei...
	WriteBuffer(m_sCodeHTMLFooter);

	m_cfHTMLExport.Close();
	m_cfTemplate.Close();

	return TRUE;
}

bool CHTMLTemplate::Open(CString sTemplate, CString sHTMLExportFile)
{
	USES_CONVERSION;

	CFileException e;
	CString sBuf, sCodeHTMLCD;
	int nFileSize;

	m_sTemplate = sTemplate;

	if (!m_cfTemplate.Open(sTemplate, CFile::modeRead|CFile::shareDenyNone|CFile::typeBinary, &e))
	{
		MessageResBox(IDS_ERROR_HTML_OPEN, MB_OK|MB_ICONINFORMATION, sTemplate);
		return FALSE;
	}

	if (!m_cfHTMLExport.Open(sHTMLExportFile, CFile::modeCreate|CFile::modeReadWrite|CFile::shareDenyWrite|CFile::typeBinary, &e))
	{
		MessageResBox(TEXT_EXPORT_ERROR_FILE_CREATE, MB_OK|MB_ICONINFORMATION, sHTMLExportFile);
		m_cfTemplate.Close();
		return FALSE;
	}

	// So jetzt lesen wir erst mal die ganze Datei ein und splitten die in die Komponenten
	// Fast egal wie groß... :-)
	// Das soll mal ein Z80 Programmierer sehen... :-o
	if (m_cfTemplate.GetLength() > 1000000)
	{
		return FALSE;
	}

	nFileSize = (int)m_cfTemplate.GetLength();
	char* pBuffer = new char[nFileSize+1];
	m_cfTemplate.Read(pBuffer, nFileSize);
	pBuffer[nFileSize] = '\0';

	sBuf = A2W(pBuffer);

	delete pBuffer;

	int nPosStart, nPosEnd;
	
	DWORD extype = (int)Big3::Hitbase::Configuration::Settings::GetValue(L"HTMLOutputExportType", IDC_EXPORTCDLISTE);

	if (extype == IDC_EXPORTCDLISTE)	
	{
		nPosStart = sBuf.Find(L"<!CD_START!>", 0);
		nPosEnd = sBuf.Find(L"<!CD_END!>", 0);

		if ((nPosStart < 0 || nPosEnd < 0) || nPosStart > nPosEnd)
		{
			return FALSE;
		}

		// 
		m_sCodeHTMLHeader = sBuf.Left(nPosStart);
		m_sCodeHTMLFooter = sBuf.Mid(nPosEnd + 10);
		sCodeHTMLCD = sBuf.Mid(nPosStart + 12, nPosEnd - (nPosStart + 12));

		nPosStart = sBuf.Find(L"<!TRACK_START!>", 0);
		nPosEnd = sBuf.Find(L"<!TRACK_END!>", 0);
		
		if (nPosStart > nPosEnd)
		{
			return FALSE;
		}

		if (nPosStart < 0 || nPosEnd < 0)
		{
			m_sCodeHTMLTRACK.Empty();
		}
		else
		{
			m_sCodeHTMLTRACK = sBuf.Mid(nPosStart+15, nPosEnd - (nPosStart + 15));
		}

		// Jetzt splitten wir noch sCodeHTMLCD
		nPosStart = sCodeHTMLCD.Find(L"<!TRACK_START!>", 0);
		nPosEnd = sCodeHTMLCD.Find(L"<!TRACK_END!>", 0);
		
		if (nPosStart < 0 || nPosEnd < 0)
		{
			m_sCodeHTMLCDHeader = sCodeHTMLCD;
			m_sCodeHTMLCDFooter.Empty();
		}
		else
		{
			m_sCodeHTMLCDHeader = sCodeHTMLCD.Left (nPosStart);
			m_sCodeHTMLCDFooter = sCodeHTMLCD.Mid (nPosEnd +  13);
		}
	}
	// 

	if (extype == IDC_EXPORTTITELLISTE)	
	{
		nPosStart = sBuf.Find(L"<!TITLE_START!>", 0);
		nPosEnd = sBuf.Find(L"<!TITLE_END!>", 0);

		if ((nPosStart < 0 || nPosEnd < 0) || nPosStart > nPosEnd)
		{
			return FALSE;
		}
		m_sCodeHTMLHeader = sBuf.Left(nPosStart);
		//AfxMessageBox(m_sCodeHTMLHeader);
		m_sCodeHTMLFooter = sBuf.Mid(nPosEnd + 13);
		//AfxMessageBox(m_sCodeHTMLFooter);
		m_sCodeHTMLTRACK = sBuf.Mid(nPosStart + 15, nPosEnd - (nPosStart + 15));
		//AfxMessageBox(m_sCodeHTMLTRACK);
	}

	// Jetzt noch ein paar SQL Abfragen für die Ersetzung in allen Teilen des Templates...
	m_dwAnzahlLieder = (int)DataBase->ExecuteScalar(L"SELECT COUNT(*) FROM Track");
	m_dwVerliehendeCDs = (int)DataBase->ExecuteScalar(L"SELECT count (*) FROM LoanedCD");
	m_dwAnzahlCDs = (int)DataBase->ExecuteScalar(L"SELECT count(*) FROM cd");
	m_dwAnzahlInterpreten = (int)DataBase->ExecuteScalar(L"SELECT count(*) FROM PersonGroup");
	
	// Dann schreiben wir doch schon mal was!
	// Später mehr...
	WriteBuffer(m_sCodeHTMLHeader);

	return TRUE;
}

BOOL CHTMLTemplate::DoExport(Big3::Hitbase::DataBaseEngine::Condition^ condition, Big3::Hitbase::DataBaseEngine::SortFieldCollection^ sortFields, const CString& sHTMLTemplate, const CString& sHTMLOutput)
{
	try
	{
		//CDataBaseProgressDlg DataBaseProgressDlg;
		//DataBaseProgressDlg.DataBase = DataBase;
		//DataBaseProgressDlg.Create(IDD_HITDB_DATABASE_PROGRESS);

		if (Open(sHTMLTemplate, sHTMLOutput))
		{
			CDQueryDataSet^ CDQuery;
			
			BOOL bFound=false;
			
			DWORD extype = (int)Big3::Hitbase::Configuration::Settings::GetValue(L"HTMLOutputExportType", IDC_EXPORTCDLISTE);

			if (extype == IDC_EXPORTCDLISTE)
			{
				CDQuery = DataBase->ExecuteTrackQuery();
				Big3::Hitbase::DataBaseEngine::CDDataView^ CDView = gcnew CDDataView(DataBase, CDQuery, condition, sortFields);

				//DataBaseProgressDlg.m_ProgressCtrl.SetRange32(0, CDView->Rows->Count);

				for (int i=0;i<CDView->Rows->Count;i++)
				{
					Big3::Hitbase::DataBaseEngine::CD^ cd = DataBase->GetCDById(CDView->GetCDID(i));
					WriteCD2HTML(cd);

					//DataBaseProgressDlg.m_ProgressCtrl.SetPos(i);
					System::Windows::Forms::Application::DoEvents();
				}
			}
			else
			{
				CDQuery = DataBase->ExecuteTrackQuery();
				Big3::Hitbase::DataBaseEngine::TrackDataView^ TrackView = gcnew TrackDataView(DataBase, CDQuery, condition, sortFields);

				//DataBaseProgressDlg.m_ProgressCtrl.SetRange32(0, TrackView->Rows->Count);

				for (int i=0;i<TrackView->Rows->Count;i++)
				{
					WriteTrack2HTML(TrackView, i);

					//DataBaseProgressDlg.m_ProgressCtrl.SetPos(i);
					System::Windows::Forms::Application::DoEvents();
				}
			}

			// Close and write Footer
			Close();
			return TRUE;
		}
		else
		{
			Close();
			return FALSE;
		}
	}
	catch (Exception^ e)
	{
		Big3::Hitbase::Miscellaneous::FormUnhandledException formUnhandledException(e);

		formUnhandledException.ShowDialog();
		return FALSE;
	}
}

// So - jetzt mal alle Zeichen in HTML Code wandeln
// IE kann zwar die meisten auch ohne aber machen wir mal ordentliches HTML :-)
CString CHTMLTemplate::Text2HTMLText(CString sText)
{
	int sPos;
	CString sBuf;
	CString sHTMLText;

	sPos = 0;
	while ((unsigned char)sText[sPos])
	{
		switch (sText[sPos])
		{
		case 10: // \n
			sHTMLText += L"<br>";
			break;
		case 13: // \r
			break;
		case 34: // "
			sHTMLText += L"&#34;";
			break;
		case 38: // &
			sHTMLText += L"&#38;";
			break;
		case 60: // <
			sHTMLText += L"&#60;";
			break;
		case 62: // >
			sHTMLText += L"&#62;";
			break;
		default:
			if ((unsigned int)sText[sPos] >= 160)
			{
				sBuf.Format(L"&#%d;", (unsigned int)sText[sPos]);
				sHTMLText = sHTMLText + sBuf;
			}
			else
				sHTMLText = sHTMLText + sText[sPos];
			break;
		}
		sPos++;
	}

	return sHTMLText;
}

