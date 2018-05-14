// CDArchive.cpp: implementation of the CCDArchive class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "resource.h"
#include "../hitmisc/hitmisc.h"
#include "../../lib/hitmisc/hitbasewinappbase.h"
#include "UploadDownloadCDsDlg.h"
#include "CDArchive.h"
#include "MultipleCDsFound.h"
#include "CDDBSock.h"
#include "CDDBConnect.h"
#include "../hitmisc/HttpFileAsync.h"
#include "../hitdb/hitdb.h"
#include "shlwapi.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

using namespace System;
using namespace System::Collections::Generic;

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

//
//TODO: If this DLL is dynamically linked against the MFC DLLs,
//		any functions exported from this DLL which call into
//		MFC must have the AFX_MANAGE_STATE macro added at the
//		very beginning of the function.
//
//		For example:
//
//		extern "C" BOOL PASCAL EXPORT ExportedFunction()
//		{
//			AFX_MANAGE_STATE(AfxGetStaticModuleState());
//			// normal function body here
//		}
//
//		It is very important that this macro appear in each
//		function, prior to any calls into MFC.  This means that
//		it must appear as the first statement within the 
//		function, even before any object variable declarations
//		as their constructors may generate calls into the MFC
//		DLL.
//
//		Please see MFC Technical Notes 33 and 58 for additional
//		details.
//

// CNativeGUIApp

BEGIN_MESSAGE_MAP(CCDArchiveApp, CWinApp)
END_MESSAGE_MAP()


// CNativeGUIApp construction

CCDArchiveApp::CCDArchiveApp()
{
	// TODO: add construction code here,
	// Place all significant initialization in InitInstance
}


// The one and only CNativeGUIApp object

CCDArchiveApp theApp;


// CNativeGUIApp initialization

BOOL CCDArchiveApp::InitInstance()
{
	CWinApp::InitInstance();

	AfxSocketInit();
	
	return TRUE;
}

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CCDArchive::CCDArchive()
{
//	MyApp.InitInstance();
//	afxCurrentAppName = L"Hitbase";

//	AfxSocketInit ();
}

// Sucht die angegebene CD in allen aktiven CD-Archiven! Wenn bVerbose TRUE ist, dann
// wird eine Animation angezeigt und alle Fehlermeldungen werden auf dem Bildschirm
// ausgegeben. Wenn Verbose FALSE ist, ist im Fehlerfalle bError = TRUE und die 
// Fehlermeldung kann mit GetLastErrorMessage() ermittelt werden.
BOOL CCDArchive::Search(Big3::Hitbase::DataBaseEngine::CD^ cd, BOOL bAutoSearch, BOOL% bError, BOOL bVerbose/* = TRUE */, IntPtr parent /* = NULL */)
{
	try
	{
		AFX_MANAGE_STATE(AfxGetStaticModuleState());

		if (String::IsNullOrEmpty(cd->Identity))
			return FALSE;

		m_bTransferCanceled = FALSE;

		// Zunächst prüfen, wenn ein AutoSearch gemacht wird, ob dies überhaupt
		// bei einem CD-Archive eingeschaltet ist. Sonst wird nämlich kein Fenster
		// aufgemacht.
		if (bAutoSearch)
		{
			BOOL bOK = FALSE;

			for (int i=0;i<Settings::Current->CDArchives->Count;i++)
				if (Settings::Current->CDArchives[i]->AutoSearch && Settings::Current->CDArchives[i]->Active)
					bOK = TRUE;

			if (!bOK)
				return FALSE;
		}

		if (bVerbose)
		{
			FormWaitAnimated = gcnew Big3::Hitbase::Miscellaneous::FormWaitAnimated();
			FormWaitAnimated->Show();//gcnew Big3::Hitbase::Miscellaneous::NativeWindowWrapper(parent));
			FormWaitAnimated->Text = gcnew String(get_string(TEXT_SEARCH_CDARCHIV));
		}

		// PeekMessage, damit der Dialog auf dem Bildschirm erscheint!!
		MSG msg;
		while (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE))
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}

		m_bVerbose = bVerbose;

		BOOL bRet = FALSE;

		for (int i=0;i<Settings::Current->CDArchives->Count && !bRet && !m_bTransferCanceled;i++)
		{
			if (Settings::Current->CDArchives[i]->Active && (!bAutoSearch || Settings::Current->CDArchives[i]->AutoSearch))
			{
				if (bVerbose)
				{
					CString str;
					str.Format(get_string(TEXT_CONNECT), (CString)Settings::Current->CDArchives[i]->ArchiveName);
					FormWaitAnimated->Status = gcnew String(str);
				}

				switch (Settings::Current->CDArchives[i]->Type)
				{
					case CDArchiveType::CDArchiveLocalCDDB:
						bRet = SearchCDInCDArchiveLocalFileCDDB(Settings::Current->CDArchives[i], cd, m_bTransferCanceled, bError);
						break;
					case CDArchiveType::CDArchiveLocal:
						bRet = SearchCDInCDArchiveLocalFile(Settings::Current->CDArchives[i], cd, m_bTransferCanceled, bError);
						break;
					case CDArchiveType::File:
						bRet = SearchCDInCDArchiveFile(Settings::Current->CDArchives[i], cd, m_bTransferCanceled, bError);
						break;
					case CDArchiveType::BIG3:
						bRet = SearchCDInInternetBig3(Settings::Current->CDArchives[i], cd, m_bTransferCanceled, bError);		
						break;
					case CDArchiveType::CDDBsockets:
					case CDArchiveType::CDDBhttp:
						bRet = SearchCDInCDDB(Settings::Current->CDArchives[i], cd, m_bTransferCanceled, bError);
						break;
				}
			}
		}

		if (bVerbose)
		{
			FormWaitAnimated->Close();
		}

		return bRet;
	}
	catch (Exception^ e)
	{
		Big3::Hitbase::Miscellaneous::FormUnhandledException^ formUnhandledException = gcnew Big3::Hitbase::Miscellaneous::FormUnhandledException(e);
		formUnhandledException->ShowDialog();
		return false;
	}
}

bool CCDArchive::SearchCDInCDArchiveFile(CDArchiveConfig^ Archive, Big3::Hitbase::DataBaseEngine::CD^ cd, BOOL% bCanceled, BOOL% bError)
{
	Big3::Hitbase::DataBaseEngine::DataBase^ db = gcnew Big3::Hitbase::DataBaseEngine::DataBase();

	db->Open(Archive->ArchiveName, true, false);

	Big3::Hitbase::DataBaseEngine::CD^ searchCD = db->GetCDByIdentity(cd->Identity);
	if (searchCD != nullptr)
	{
		searchCD->ID = 0;
	}
	else
	{
		db->Close();
		return false;
	}

	db->Close();

	cd->Artist = searchCD->Artist;
	cd->Title = searchCD->Title;
	cd->Composer = searchCD->Composer;
	cd->Sampler = searchCD->Sampler;

	if (cd->Sampler && Big3::Hitbase::Configuration::Settings::Current->SamplerUseFixedArtist)
	{
		cd->Artist = Big3::Hitbase::Configuration::Settings::Current->SamplerFixedArtistText;
	}

	for (int i=0;i<searchCD->NumberOfTracks;i++)
	{
		if (cd->Sampler)
			cd->Tracks[i]->Artist = searchCD->Tracks[i]->Artist;
		cd->Tracks[i]->Composer = searchCD->Tracks[i]->Composer;
		cd->Tracks[i]->Title = searchCD->Tracks[i]->Title;

		if ((Settings::Current->CDArchiveFields & CDArchiveFields::BPM) == CDArchiveFields::BPM)
			cd->Tracks[i]->Bpm = searchCD->Tracks[i]->Bpm;

		if ((Settings::Current->CDArchiveFields & CDArchiveFields::TrackComment) == CDArchiveFields::TrackComment)
			cd->Tracks[i]->Comment = searchCD->Tracks[i]->Comment;

		if ((Settings::Current->CDArchiveFields & CDArchiveFields::Lyrics) == CDArchiveFields::Lyrics)
			cd->Tracks[i]->Lyrics = searchCD->Tracks[i]->Lyrics;
	}

	if ((Settings::Current->CDArchiveFields & CDArchiveFields::Category) == CDArchiveFields::Category)
		cd->Category = searchCD->Category;

	if ((Settings::Current->CDArchiveFields & CDArchiveFields::Medium) == CDArchiveFields::Medium)
		cd->Medium = searchCD->Medium;

	if ((Settings::Current->CDArchiveFields & CDArchiveFields::Comment) == CDArchiveFields::Comment)
		cd->Comment = searchCD->Comment;

	return true;
}

BOOL CCDArchive::SearchCDInInternetBig3(CDArchiveConfig^ Archive, Big3::Hitbase::DataBaseEngine::CD^ cd, BOOL% bCanceled, BOOL% bError)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	try
	{
		CString str;
		//TODO!!!!!!!!!!!!!!!!!!str.Format(get_string(TEXT_CONNECT), (CString)Archive->ArchiveName);
		//!!!!!!!!!!!!!!!!!CMisc::SetStatusText(str);

		CInternetSession * pInternetSession;

/*		switch (Settings::Current->ProxyType)
		{
		case 1:
			pInternetSession = new CInternetSession(NULL, 1, INTERNET_OPEN_TYPE_PROXY, (CString)Settings::Current->ProxyServerName);
			break;
		default:
			pInternetSession = new CInternetSession();
		}*/

		pInternetSession = new CInternetSession();

		// Maximal x Millisekunden warten (10 sek. default = 10000)
		pInternetSession->SetOption(INTERNET_OPTION_CONNECT_TIMEOUT, Settings::Current->HttpTimeoutInMS);

		CHttpConnection * pHttpConnection;
		try
		{
			pHttpConnection = pInternetSession->GetHttpConnection((CString)Archive->ArchiveName);
		}
		catch (CInternetException* e)
		{
			wchar_t szCause[255];
			e->GetErrorMessage(szCause, 255);
			DisplayErrorMessage(gcnew String(szCause));

			e->Delete();
			pInternetSession->Close();
			delete pInternetSession;

			bError = TRUE;

			return FALSE;
		}

		if (m_bVerbose)
		{
			CString str;
			str.Format(get_string(IDS_CDARCHIV_SEARCH_CD), (CString)Archive->ArchiveName);
			FormWaitAnimated->Status = gcnew String(str);
		}
		
		BOOL ret = DownloadCD(gcnew IntPtr(pHttpConnection), cd, bCanceled, bError, true);

		//CMisc::SetStatusText(L"");

		pHttpConnection->Close();
		delete pHttpConnection;

		pInternetSession->Close();
		delete pInternetSession;

		return ret;
	}
	catch (...)
	{
		return FALSE;
	}
}

// Suche die angegebene CD auf dem Server.
BOOL CCDArchive::DownloadCD(IntPtr^ httpConnection, Big3::Hitbase::DataBaseEngine::CD^ cd, BOOL% bCanceled, BOOL% bError, BOOL bBackground /* = TRUE */)
{
	CWaitCursor wait;
	BOOL bRet;
	CHttpConnection * pHttpConnection = (CHttpConnection *)httpConnection->ToPointer();

	ASSERT(pHttpConnection);
	if (String::IsNullOrEmpty(cd->Identity))
	{
		// Ohne Identity auch keine CD ;-)
		return FALSE;
	}

	CString sInternetFileName;

	sInternetFileName.Format(L"cdquery.asp?User=Hitbase&Password=hitbase2k&ID=%s&UPC=%s", (CString)cd->Identity, (CString)cd->UPC);

	CHttpFileAsync httpFileAsync;
	httpFileAsync.ReadFileFromConnection(pHttpConnection, sInternetFileName, TRUE, bBackground);

	if (bBackground)
	{
		while (httpFileAsync.IsTransferActive())
		{
			// Das hier ist nicht optimal, aber ich lass es erst mal drin.
			MSG msg;
			while (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE))
			{
				TranslateMessage(&msg);
				DispatchMessage(&msg);
			}
		}
	}

	if (!httpFileAsync.WasTransferAborted())
	{
		CString sContent = httpFileAsync.GetResult();

		bRet = ReadCDInfoFromString(sContent, cd, bError);
	}
	else
	{
		bRet = FALSE;
		bCanceled = true;
	}

	return bRet;
}

// Liest alle Daten aus der angegebenen Datei. Dies ist im allgemeinen eine
// Datei, die vom Internet-Server kommt. Später kann hiermit vielleicht
// auch ein Text-Import realisiert werden?

BOOL CCDArchive::ReadCDInfoFromString(const CString& sContent, Big3::Hitbase::DataBaseEngine::CD^ cd, BOOL% bError)
{
	CString str;
	CString sFieldName;
	CString sFieldValue;
	CString sCDInfo = sContent;
	int nVersion, i, iCurPos=0;

	String^ testString = gcnew String(sContent);

	// Zuerst Versionsnummer lesen.
	str = sCDInfo.Tokenize(L"\r\n", iCurPos);
	if (str.Left(8) == "Version=")
		nVersion = _wtoi(str.Mid(8));
	else
	{
		if (str.Left(12) != "CD not found")      // hm....... ist blöd, aber klappt!
			bError = TRUE;
		return FALSE;           // Version muß am Anfang stehen
	}

	if (nVersion < 0 || nVersion > INTERNET_FILE_VERSION)
	{
		bError = TRUE;
		return FALSE;           // Diese Version verstehe ich nicht.
	}

	str = sCDInfo.Tokenize(L"\r\n", iCurPos);
	while (!str.IsEmpty())
	{
		bool bOK = false;

		i = str.Find('=');

		ASSERT(i >= 0);

		if (i >= 0)                // Unter Umständen keine Information vorhanden, wenn der Upload abgebrochen wurde.
		{
			sFieldName = str.Left(i);         // Feldname (z.B. "Artist")
			sFieldValue = str.Mid(i+1);       // Inhalt (z.B. "Adams, Bryan")

			// \n am Schluß noch löschen
			while (sFieldValue.Right(1) == "\r" || sFieldValue.Right(1) == "\n")
				sFieldValue = sFieldValue.Left(sFieldValue.GetLength()-1);

			// Jetzt noch die CR/LF zurückwandeln
			CString strBuf = sFieldValue;
			sFieldValue = "";
			for (int j=0;j<strBuf.GetLength();j++)
			{
				if (strBuf.Mid(j, 4) == "\\r\\n")
				{
					sFieldValue += "\r\n";
					j+=3;
				}
				else
				{
					if (strBuf.Mid(j, 2) == "\\\\")
					{
						sFieldValue += "\\";
						j++;
					}
					else
						sFieldValue += strBuf[j];
				}
			}

			// Den HTML-Tag <BR> noch eliminieren!
			if (!sFieldValue.Right(4).CompareNoCase(L"<BR>"))
			{
				sFieldValue = sFieldValue.Left(sFieldValue.GetLength()-4);
				bOK = true;
			}

			if (!sFieldName.CompareNoCase(L"Date"))
			{
				bOK = true;
				// Brauch ich im Moment nicht. Vielleicht später????
			}

			if (!sFieldName.CompareNoCase(L"ID"))
			{
				ASSERT(sFieldValue == cd->Identity);  // Darf nicht sein!
				bOK = true;
			}

			if (!sFieldName.CompareNoCase(L"Artist"))
			{
				cd->Artist = (gcnew String(sFieldValue))->Trim();
				bOK = true;
			}

			if (!sFieldName.CompareNoCase(L"Title"))
			{
				cd->Title = (gcnew String(sFieldValue))->Trim();
				bOK = true;
			}

			if (!sFieldName.CompareNoCase(L"Sampler"))
			{
				cd->Sampler = _wtoi(sFieldValue);
				bOK = true;
			}

			if (!sFieldName.CompareNoCase(L"TotalLength"))
			{
				cd->TotalLength = _wtoi(sFieldValue);
				bOK = true;
			}

			if (!sFieldName.CompareNoCase(L"NumberOfTracks"))
			{
				cd->NumberOfTracks = _wtoi(sFieldValue);
				cd->InitTracks(cd->NumberOfTracks);
				bOK = true;
			}

			if (!sFieldName.CompareNoCase(L"Category"))
			{
				if ((Settings::Current->CDArchiveFields & CDArchiveFields::Category) == CDArchiveFields::Category)
					cd->Category = gcnew String(sFieldValue);
				bOK = true;
			}

			if (!sFieldName.CompareNoCase(L"Media"))
			{
				if ((Settings::Current->CDArchiveFields & CDArchiveFields::Medium) == CDArchiveFields::Medium)
					cd->Medium = gcnew String(sFieldValue);
				bOK = true;
			}

			if (!sFieldName.CompareNoCase(L"Comment"))
			{
				if ((Settings::Current->CDArchiveFields & CDArchiveFields::Comment) == CDArchiveFields::Comment)
					cd->Comment = (gcnew String(sFieldValue))->Trim();
				bOK = true;
			}

			if (!sFieldName.CompareNoCase(L"YearRecorded"))
			{
				cd->YearRecorded = _wtoi(sFieldValue);
				bOK = true;
			}

			if (!sFieldName.CompareNoCase(L"Copyright"))
			{
				cd->Copyright = (gcnew String(sFieldValue))->Trim();
				bOK = true;
			}

			// 08.02.2004
			if (!sFieldName.CompareNoCase(L"UPC"))
			{
				cd->UPC = (gcnew String(sFieldValue))->Trim();
				bOK = true;
			}

			// 08.02.2004
			if (!sFieldName.CompareNoCase(L"Label"))
			{
				cd->Label = (gcnew String(sFieldValue))->Trim();
				bOK = true;
			}

			if (!sFieldName.CompareNoCase(L"TrackArtist"))
			{
				int nr = _wtoi(sFieldValue)-1;
				int j = sFieldValue.Find(L"\"");
				sFieldValue = sFieldValue.Left(sFieldValue.GetLength()-1);
				cd->Tracks[nr]->Artist = (gcnew String(sFieldValue.Mid(j+1)))->Trim();
				bOK = true;
			}

			if (!sFieldName.CompareNoCase(L"TrackTitle"))
			{
				int nr = _wtoi(sFieldValue)-1;
				int j = sFieldValue.Find(L"\"");
				sFieldValue = sFieldValue.Left(sFieldValue.GetLength()-1);
				cd->Tracks[nr]->Title = (gcnew String(sFieldValue.Mid(j+1)))->Trim();
				bOK = true;
			}

			if (!sFieldName.CompareNoCase(L"TrackLength"))
			{
				int nr = _wtoi(sFieldValue)-1;
				int j = sFieldValue.Find(L" ");
				cd->Tracks[nr]->Length = _wtoi(sFieldValue.Mid(j+1));
				bOK = true;
			}

			if (!sFieldName.CompareNoCase(L"TrackBPM"))
			{
				int nr = _wtoi(sFieldValue)-1;
				int j = sFieldValue.Find(L" ");
				if ((Settings::Current->CDArchiveFields & CDArchiveFields::BPM) == CDArchiveFields::BPM)
					cd->Tracks[nr]->Bpm = _wtoi(sFieldValue.Mid(j+1));
				bOK = true;
			}

			if (!sFieldName.CompareNoCase(L"TrackComment"))
			{
				int nr = _wtoi(sFieldValue)-1;
				int j = sFieldValue.Find(L"\"");
				sFieldValue = sFieldValue.Left(sFieldValue.GetLength()-1);
				if ((Settings::Current->CDArchiveFields & CDArchiveFields::TrackComment) == CDArchiveFields::TrackComment)
					cd->Tracks[nr]->Comment = gcnew String(sFieldValue.Mid(j+1));
				bOK = true;
			}

			if (!sFieldName.CompareNoCase(L"TrackLyrics"))
			{
				int nr = _wtoi(sFieldValue)-1;
				int j = sFieldValue.Find(L"\"");
				sFieldValue = sFieldValue.Left(sFieldValue.GetLength()-1);
				if ((Settings::Current->CDArchiveFields & CDArchiveFields::Lyrics) == CDArchiveFields::Lyrics)
					cd->Tracks[nr]->Lyrics = gcnew String(sFieldValue.Mid(j+1));
				bOK = true;
			}

			if (!sFieldName.CompareNoCase(L"TrackYearRecorded"))
			{
				int nr = _wtoi(sFieldValue)-1;
				int j = sFieldValue.Find(L" ");
				cd->Tracks[nr]->YearRecorded = _wtoi(sFieldValue.Mid(j+1));
				bOK = true;
			}
		}

		str = sCDInfo.Tokenize(L"\r\n", iCurPos);

		if (!bOK)   // Unbekannter Eintrag
			break;
	}

	if (cd->Sampler && Big3::Hitbase::Configuration::Settings::Current->SamplerUseFixedArtist)
	{
		cd->Artist = Big3::Hitbase::Configuration::Settings::Current->SamplerFixedArtistText;
	}

	return TRUE;
}

BOOL CCDArchive::SearchCDInCDDB(CDArchiveConfig^ Archive, Big3::Hitbase::DataBaseEngine::CD^ cd, BOOL% bCanceled, BOOL% bError)
{
	CString sBuffer;
	CString sBuffer2;
	int nReturnCode;
	int nTrennPos;
	//CCDDBSock cddbSocket;
	CString SendCmd;
	CString buf;
	CString Kategorie, ReadCDID;
	CString CDDBData;
	CString TitelInterpret;
	CString ArchivName;
	wchar_t *p;
	List<Big3::Hitbase::DataBaseEngine::CD^>^ CDArray = gcnew List<Big3::Hitbase::DataBaseEngine::CD^>();
	CArray <CString, CString&> MultiCD;
	int nReturnCDDB;
	CCDDBConnect cddb;
	BOOL SocketConnect;

	if (Archive->Type == CDArchiveType::CDDBhttp)
	{
		SocketConnect = FALSE;
	}

	// socket Verbindung?
	if (Archive->Type == CDArchiveType::CDDBsockets)
	{
		SocketConnect = TRUE;
	}

	if (cddb.CDDBOpen(Archive) == FALSE)
	{
		DisplayErrorMessage ("Error CDDB Server connect");
		bError = TRUE;
		return FALSE;
	}

	if (m_bVerbose)
	{
		CString str;
		str.Format(get_string(IDS_CDARCHIV_SEARCH_CD), (CString)Archive->ArchiveName);
		FormWaitAnimated->Status = gcnew String(str);
	}

	// Check for answer...

	if (SocketConnect == TRUE && cddb.CDDBReceive (sBuffer) == FALSE)
	{
		// OK - Check for correct return string
		nReturnCode = CDDBGetReturnCode(sBuffer);
		//200	OK, read/write allowed	
		//201	OK, read only
		//432	No connections allowed: permission denied
		//433	No connections allowed: X users allowed, Y currently active
		//434	No connections allowed: system load too high    
		if (nReturnCode == 200 || nReturnCode == 201)
		{
			// Connect OK
		}
		else
		{
			CString s;
			s.Format(L"Error cddb server\n\n%s", sBuffer);
			DisplayErrorMessage (gcnew String(s));

			cddb.CDDBClose();
			bError = TRUE;
			return FALSE;
		}
	}

	if (cddb.CDDBInit () == FALSE)
	{
		// Fehler Send nun noch Close() socket und zurück
		DisplayErrorMessage ("Error cddb server: Send timeout");
		cddb.CDDBClose ();
		bError = TRUE;
		return FALSE;
	}

	// Wait for handshake reply...
	//memset (sockbuf, 0, sizeof(sockbuf));
	if (SocketConnect == TRUE && cddb.CDDBReceive(sBuffer) == TRUE)
	{
		nReturnCode = CDDBGetReturnCode(sBuffer);
		if (nReturnCode == 200)
		{
			// Handshake OK!
			// Go on...
		}
		else
		{
			// Sorry no handshake!
			// Close Connection
			CString s;
			s.Format(L"Error cddb server - handshake\n\n%s", sBuffer);
			DisplayErrorMessage (gcnew String(s));
			cddb.CDDBClose();
			bError = TRUE;
			return FALSE;
		}
	}

	// Versuche Protocol level 3
	// 
	// JUS 001103: Vorher Level 4
	//
	// JUS 010721: Wieder Protokoll 4, da freedb.org mittlerweile auf protokoll 4 ist!!
	// GUN: Und jetzt sind wir auf 5, da sonst das Feld YEAR nicht zurückgeliefert wird.
	//      und das geht NUR bei http connect... Mist freedb! Socket macht nur Level 4
	if (SocketConnect == TRUE)
		cddb.CDDBSetProtocolLevel(4);
	else
		cddb.CDDBSetProtocolLevel(5);

	if (SocketConnect == TRUE)
	{
		if (cddb.CDDBReceive(sBuffer) == TRUE)
		{
			// Look for success
			nReturnCode = CDDBGetReturnCode(sBuffer);

			if (nReturnCode == 201 || nReturnCode == 502)
			{
				// Im Moment nicht benötigt...
				// Wenn Protocol Level nicht größer, dann gib es
				// halt keinen "inexact match" beim "cddb query"
			}
		}
		else
		{
			// Nun unbedingt nur noch Close() socket
			// Alle anderen Operationen sind nicht mehr erlaubt
			CString s;
			s.Format(L"Error cddb server - protocol level\n\n%s", sBuffer);
			DisplayErrorMessage (gcnew String(s));

			cddb.CDDBClose();
			bError = TRUE;
			return FALSE;
		}
	}
	// Test only - REMOVE goto !!!!!!!!!!!!*************
	// goto Rrr;
	// Test only - REMOVE goto !!!!!!!!!!!!*************

	String^ sCDDBQuery;

	if (!cd->NumberOfTracks)
	{
		sCDDBQuery = m_sUploadCDIdentityCDDB;
		// Der zweite String ist die Anzahl der Lieder
		int iPos = m_sUploadCDIdentityCDDB->IndexOf(" ");
		if (iPos >= 0)
		{
			int iNumberOfTracks = Big3::Hitbase::Miscellaneous::Misc::Atoi(m_sUploadCDIdentityCDDB->Substring(iPos));
			if (iNumberOfTracks > 0 && iNumberOfTracks <= 999)
				cd->NumberOfTracks = iNumberOfTracks;
		}
	}
	else
		sCDDBQuery = GetCDDBQueryString(cd);

	if (!SocketConnect)
		sCDDBQuery = sCDDBQuery->Replace(",", "");

	// Nach allen CDDB Einträgen suchen
	SendCmd.Format(L"cddb query %s ", CString(sCDDBQuery));

	CDArchiveLog::AddLog(gcnew System::String(SendCmd));

	// Senden des query commands zum cddb Server
	if (cddb.CDDBTransmit (SendCmd) == FALSE)
	{
		// Fehler Send nun noch Close() socket und zurück
		DisplayErrorMessage ("Error cddb server: Can not send command (server incorrect or down?)");
		cddb.CDDBClose ();
		bError = TRUE;
		return FALSE;
	}

	// Rückgabe String vom Befehl query
	if (cddb.CDDBReceive(sBuffer) == FALSE)
	{
		// Nun unbedingt nur noch Close() socket
		// Alle anderen Operationen sind nicht mehr erlaubt
		CString s;
		s.Format(L"Error cddb server - query\n\n%s", sBuffer);
		DisplayErrorMessage (gcnew String(s));
		cddb.CDDBClose();
		bError = TRUE;
		return FALSE;
	}
	// Lesen return code vom cddb Server
	nReturnCDDB = CDDBGetReturnCode(sBuffer);

	if (nReturnCDDB == 210 || nReturnCDDB == 211)
	{
		// Zur Sicherheit hier noch mal schauen 
		// ob noch mehr im Buffer ist - kann aber nur bei
		// socket Verbindung auftreten
		if (sBuffer.Find(L"\n.") < 0)
		{
			// So, so - da fehlt noch der Punkt also nachlesen
			CString tmpbuf;

			if (cddb.CDDBReceive(tmpbuf, TRUE) == FALSE)
			{
				// Nun unbedingt nur noch Close() socket
				// Alle anderen Operationen sind nicht mehr erlaubt
				DisplayErrorMessage ("Error cddb server");
				cddb.CDDBClose();
				bError = TRUE;
				return FALSE;
			}
			sBuffer = sBuffer + tmpbuf;
		}
	}

	Kategorie.Empty();
	ReadCDID.Empty();

	// Lesen return code vom cddb Server
	nReturnCDDB = CDDBGetReturnCode(sBuffer);

	if (nReturnCDDB == 200)
	{
		// Eindeutige Rückmeldung
		// Kategorie und ID auslesen
		CDDBGetKategorieCDID (sBuffer, Kategorie, ReadCDID);

		if (Kategorie.GetLength() == 0 || ReadCDID.GetLength() == 0)
		{
			cddb.CDDBClose();
			return FALSE;
		}

		SendCmd = "cddb read " + Kategorie + " " + ReadCDID + "\n";
		if (cddb.CDDBTransmit (SendCmd) == FALSE)
		{
			// Fehler Send nun noch Close() socket und zurück
			DisplayErrorMessage ("Error cddb server: Send timeout");
			cddb.CDDBClose ();
			bError = TRUE;
			return FALSE;
		}

		// So - nun kommen (hoffentlich!) die CD Daten
		// Füllen von CDDBData mit Buffer
		CDDBData.Empty();

		// Receive bis zum "."
		if (cddb.CDDBReceive(CDDBData, TRUE) == FALSE)
		{
			// Nun unbedingt nur noch Close() socket
			// Alle anderen Operationen sind nicht mehr erlaubt
			CString s;
			s.Format(L"Error cddb server - read\n\n%s", CDDBData);
			DisplayErrorMessage (gcnew String(s));
			cddb.CDDBClose();
			bError = TRUE;
			return FALSE;
		}

		// !!!!!!!!!!!!!!!!!!
		// return überprüfen
		// Struktur nun mit gelesenen CD Daten füllen
		CDDBFillCDInfo(cd, CDDBData, Archive);

		cddb.CDDBClose();

		return TRUE;
	}

	if (nReturnCDDB == 210 || nReturnCDDB == 211)
	{
		// Rückgabewert nicht eindeutig
		// Zuerst mal alle Rückgabe Zeilen lesen - bis '.'
		// Auswahl einblenden
		// Ausgewählte Zeile Kategorie und ID auslesen
		//if (cddb.CDDBReceive(sBuffer) == FALSE)
		//{
		// Nun unbedingt nur noch Close() socket
		// Alle anderen Operationen sind nicht mehr erlaubt
		//	DisplayErrorMessage ("Error cddb server\n\n%s", sBuffer);
		//	cddb.CDDBClose();
		//	return FALSE;
		//}

		// !!!!!!!!!!!!!! Test only ... remove!!!!!!!!!!!!!!!!
		//Rrr:		
		//sBuffer = "211 Found exact matches, list follows (until terminating `.')";
		//sBuffer = sBuffer + "\n" + "misc b90fec0b id software / Quake 2 CD-ROM";
		//sBuffer = sBuffer + "\n" + "data b90fec0b id software111 / Quake 2111 CD-ROM";
		//sBuffer = sBuffer + "\n" + "rock 6d0b4409 Supertramp / Famous Last Words";
		//sBuffer = sBuffer + "\n" + ".";
		// !!!!!!!!!!!!!! Test only ... remove!!!!!!!!!!!!!!!!

		wchar_t mcdbuf[10000];
		wcscpy (mcdbuf, sBuffer);
		p = wcstok (mcdbuf, L"\r\n");
		Big3::Hitbase::DataBaseEngine::CD^ TempCD = gcnew Big3::Hitbase::DataBaseEngine::CD();

		while (1)
		{
			p = wcstok (NULL, L"\r\n");
			if (p == NULL)
			{
				cddb.CDDBClose();
				return FALSE;
			}

			if (*p == '.')
			{
				// Ende der Liste nun zur Auswahl
				break;
			}

			Kategorie = p;
			Kategorie = Kategorie.Left(Kategorie.Find(' '));

			ReadCDID = p;
			ReadCDID = ReadCDID.Mid(ReadCDID.Find(' ')+1);
			ReadCDID = ReadCDID.Left(ReadCDID.Find(' '));

			if (Kategorie.GetLength() == 0)
			{
				cddb.CDDBClose();
				return FALSE;
			}

			TitelInterpret = p;
			TitelInterpret = TitelInterpret.Mid(TitelInterpret.Find(' ') + 1);
			TitelInterpret = TitelInterpret.Mid(TitelInterpret.Find(' ') + 1);

			// Titel und Interpret teilen
			nTrennPos = TitelInterpret.Find((CString)Archive->SamplerTrennzeichen);

			if (nTrennPos > 0)
			{
				TempCD->Title = gcnew String(TitelInterpret.Mid(nTrennPos + 1));
				TempCD->Artist = gcnew String(TitelInterpret.Left(nTrennPos));
			}
			else
			{
				TempCD->Title = gcnew String(TitelInterpret);
				TempCD->Artist = gcnew String(TitelInterpret);
			}

			TempCD->Title = TempCD->Title->Trim();
			TempCD->Artist = TempCD->Artist->Trim();

			if (TempCD->Artist->Length > 99)
				TempCD->Artist = TempCD->Artist->Substring(0, 99);
			if (TempCD->Title->Length > 99)
				TempCD->Title = TempCD->Title->Substring(0, 99);

			if (TempCD->Artist->Length == 0)
				TempCD->Artist = TempCD->Title;

			RemoveSpecialChars (TempCD->Artist);
			RemoveSpecialChars (TempCD->Title);

			// Struktur nun mit gelesenen CD Daten füllen
			CDDBFillCDInfo(TempCD, CDDBData, Archive);

			CDArray->Add(TempCD);

			// Original Query Zeile merken
			buf = p;
			MultiCD.Add (buf);
		} // while (1) für alle gefundenen CDs bei der query

		CMultipleCDsFound MultipleCDsFound(CDArray);
		if (MultipleCDsFound.DoModal() == IDCANCEL)
		{
			cddb.CDDBClose();
			return FALSE;
		}

		//MultipleCDsFound.m_nSelected;
		//CDInfo = CDInfoArray[MultipleCDsFound.m_nSelected]; 
		buf = MultiCD[MultipleCDsFound.m_nSelected];
		Kategorie = buf;
		Kategorie = Kategorie.Left(Kategorie.Find(' '));

		ReadCDID = buf;
		ReadCDID = ReadCDID.Mid(ReadCDID.Find(' ')+1);
		ReadCDID = ReadCDID.Left(ReadCDID.Find(' '));

		SendCmd = "cddb read " + Kategorie + " " + ReadCDID + "\n";
		if (cddb.CDDBTransmit (SendCmd) == FALSE)
		{
			// Fehler Send nun noch Close() socket und zurück
			DisplayErrorMessage ("Error cddb server: Send timeout");
			cddb.CDDBClose ();
			bError = TRUE;
			return FALSE;
		}

		// So - nun kommen (hoffentlich!) die CD Daten
		// Lesen bis zur Zeile mit '.'
		// Füllen von CDDBData mit Buffer
		CDDBData.Empty();

		// Receive bis zum "."
		if (cddb.CDDBReceive(CDDBData,TRUE) == FALSE)
		{
			// Nun unbedingt nur noch Close() socket
			// Alle anderen Operationen sind nicht mehr erlaubt
			CString s;
			s.Format(L"Error cddb server\n\n%s", CDDBData);
			DisplayErrorMessage (gcnew String(s));
			bError = TRUE;
			cddb.CDDBClose();
			return FALSE;
		}

		// Struktur nun mit gelesenen CD Daten füllen
		// CDInfo.Empty();

		CDDBFillCDInfo(cd, CDDBData, Archive);

		cddb.CDDBClose();

		return TRUE;
	}

	if (nReturnCDDB != 202)
	{
		bError = TRUE;
		CString s;
		s.Format(L"Fehler cddb return:\n\n%s", sBuffer);
		DisplayErrorMessage (gcnew String(s));
	}

	// nReturnCDDB != 200, 210, 211
	// So - das wars. Keine CD gefunden
	// Nun noch aufräumen und zurück

	cddb.CDDBClose();

	return FALSE;
}

String^ CCDArchive::GetCDDBQueryString(Big3::Hitbase::DataBaseEngine::CD^ cd)
{
	unsigned long lCDDBID = cd->GetCDDBDiscID();
	CString sCDDBQuery;
	CString buf;

	// Nach allen CDDB Einträgen suchen
	sCDDBQuery.Format(L"%08x %d ", lCDDBID, cd->NumberOfTracks);

	for (int numtracks = 0; numtracks < cd->NumberOfTracks; numtracks++)
	{
		buf.Format (L"%d ", ((cd->Tracks[numtracks]->StartPosition * 75) / 1000));

		sCDDBQuery += buf;
	}

	// Löschen letztes Leerzeichen
	sCDDBQuery = sCDDBQuery.Left(sCDDBQuery.GetLength()-1);

	// Jetzt noch Länge CD in Sekunden
	//    	buf.Format (L" %d\n", (pCD->m_dwTotalLength / 1000));
	buf.Format (L" %d\n", (cd->GetCDDBDiscLength() / 1000));
	sCDDBQuery += buf;

	return gcnew String(sCDDBQuery);
}

// Füllen der CDInfo Struktur mit XMCD Daten
// ------------------------------------------------------
BOOL CCDArchive::CDDBFillCDInfo(Big3::Hitbase::DataBaseEngine::CD^ cd, CString& CDDBData, CDArchiveConfig^ Archive)
{
	int nBufPos, nTrennPos, nLineEnd;
	CString TitelInterpret;
	CString Line;
	CString Titel;
	CString ExtTitel;
	int nTitelNum;
	int nLinePos;
	int nExtTitelNum;
	wchar_t buf[1000];
	int pos;

	nBufPos = 0;

	// Alle vorherigen Daten zuerst mal raus
	TitelInterpret.Empty();
	cd->Comment = "";

	for (pos = 0; pos < cd->NumberOfTracks; pos++)
	{
		cd->Tracks[pos]->Title = "";
		cd->Tracks[pos]->Artist = "";
		cd->Tracks[pos]->Comment = "";
	}

	while (1)
	{
		nLineEnd = CDDBData.Find('\n');

		if (nLineEnd < 0)
			break;

		Line = CDDBData.Left(nLineEnd);
		while (Line.Mid(Line.GetLength()-1) == '\n' || Line.Mid(Line.GetLength()-1) == '\r')
			Line = Line.Left(Line.GetLength() - 1);

		nLineEnd++;
		if (nLineEnd > CDDBData.GetLength())
			break;

		CDDBData = CDDBData.Mid(nLineEnd);

		if (Line.Left(7).CompareNoCase(L"DTITLE=") == 0)
		{
			// Können auch mehrere Zeilen sein...
			TitelInterpret = TitelInterpret + Line.Mid(7);
			continue;
		}
		if (Line.Left(6).CompareNoCase(L"DYEAR=") == 0)
		{
			cd->YearRecorded = _wtoi(Line.Mid(6));

			if (cd->YearRecorded < 1000 || cd->YearRecorded > 9999)
				cd->YearRecorded = 0;

			continue;
		}
		if (Line.Left(6).CompareNoCase (L"TTITLE") == 0)
		{
			Titel.Empty();

			Titel = Line.Mid(6, 1);

			wcscpy (buf,Line.Mid(7,1));
			if (isdigit (buf[0]))
			{
				Titel += Line.Mid(7,1);

				wcscpy (buf,Line.Mid(8,1));
				if (isdigit (buf[0]))
					Titel += Line.Mid(8,1);
			}
			nTitelNum = _wtoi (Titel);

			if (nTitelNum > cd->NumberOfTracks)
			{
				// Track zu hoch - überspringen des Tracks
				continue;
			}

			if (nTitelNum < 0)
			{
				// Track zu hoch - überspringen des Tracks
				continue;
			}

			nLinePos = 8;
			if (nTitelNum > 9)
				nLinePos++;

			if (nTitelNum > 99)
				nLinePos++;

			cd->Tracks[nTitelNum]->TrackNumber = nTitelNum + 1;

			if (cd->Tracks[nTitelNum]->Title->Length == 0)
			{
				cd->Tracks[nTitelNum]->Title = gcnew String(Line.Mid(nLinePos));
			}
			else
			{
				cd->Tracks[nTitelNum]->Title += gcnew String(Line.Mid(nLinePos));
			}

			if (cd->Tracks[nTitelNum]->Title->Length > 99)
				cd->Tracks[nTitelNum]->Title = cd->Tracks[nTitelNum]->Title->Substring(0, 99);

			RemoveSpecialChars (cd->Tracks[nTitelNum]->Title);
			continue;
		}
		if (Line.Left(4).CompareNoCase(L"EXTD") == 0)
		{
			if (cd->Comment->Length == 0)
				cd->Comment = gcnew String(Line.Mid(5));
			else
				cd->Comment += gcnew String(Line.Mid(5));

			if (cd->Comment->Length > 255)
				cd->Comment = cd->Comment->Substring(0, 255);

			RemoveSpecialChars (cd->Comment);

			continue;
		}
		if (Line.Left(4).CompareNoCase(L"EXTT") == 0)
		{
			nExtTitelNum = _wtoi(Line.Mid(4));

			/*JUS 98.08.11: Das geht doch viel einfacher!? Siehe oben!
			ExtTitel.Empty();
			ExtTitel.Mid(0) = Line.Mid(4);

			strcpy (buf, Line.Mid(5));
			if (isdigit (buf[0]))
			ExtTitel.Mid(1) = Line.Mid(5);

			strcpy (buf, Line.Mid(6));
			if (isdigit (buf[0]))
			ExtTitel.Mid(2) = Line.Mid(6);

			nExtTitelNum = atoi (ExtTitel);
			JUS_*/

			if (nExtTitelNum > cd->NumberOfTracks)
			{
				// Track zu hoch - überspringen des Tracks
				continue;
			}

			if (nExtTitelNum < 0)
			{
				// Track zu hoch - überspringen des Tracks
				continue;
			}

			nLinePos = 6;
			if (nExtTitelNum > 9)
				nLinePos++;
			if (nExtTitelNum > 99)
				nLinePos++;

			if (cd->Tracks[nExtTitelNum]->Comment->Length == 0)
				cd->Tracks[nExtTitelNum]->Comment = gcnew String(Line.Mid(nLinePos));
			else
				cd->Tracks[nExtTitelNum]->Comment += gcnew String(Line.Mid(nLinePos));

			if (cd->Tracks[nExtTitelNum]->Comment->Length > 255)
				cd->Tracks[nExtTitelNum]->Comment = cd->Tracks[nTitelNum]->Comment->Substring(0, 255);

			RemoveSpecialChars (cd->Tracks[nExtTitelNum]->Comment);

			continue;
		}
		// Lesen und wegschmeissen
		if (Line.Left(9).CompareNoCase(L"PLAYORDER") == 0)
		{
			// und tschüss
			break;
		}
	}

	if (TitelInterpret.GetLength() < 1)
		return FALSE;

	// Titel und Interpret noch teilen
	nTrennPos = TitelInterpret.Find((CString)Archive->SamplerTrennzeichen);


	if (nTrennPos > 0)
	{
		cd->Title = gcnew String(TitelInterpret.Mid(nTrennPos + 1));
		cd->Artist = gcnew String(TitelInterpret.Left(nTrennPos));
	}
	else
	{
		cd->Title = gcnew String(TitelInterpret);
		cd->Artist = gcnew String(TitelInterpret);
	}

	cd->Title = cd->Title->Trim();
	cd->Artist = cd->Artist->Trim();

	if (cd->Artist->Length > 99)
		cd->Artist = cd->Artist->Substring(0, 99);
	if (cd->Title->Length > 99)
		cd->Title = cd->Title->Substring(0, 99);

	if (cd->Artist->Length == 0)
		cd->Artist = cd->Title;

	RemoveSpecialChars (cd->Artist);
	RemoveSpecialChars (cd->Title);
	CString r;
	r = cd->Artist;

	for (int i=0;i<cd->NumberOfTracks;i++)
	{
		cd->Tracks[i]->Artist = cd->Artist;
	}

	// JUS 24.01.2001: Automatisch CD-Sampler erzeugen
	if (Archive->AutoCreateSampler)
	{
		BOOL bNotFound = FALSE;

		// Zuerst mal prüfen, ob in jedem Lied das Trennzeichen vorkommt
		for (int i=0;i<cd->NumberOfTracks;i++)
		{
			if (cd->Tracks[i]->Title->IndexOf(Archive->SamplerTrennzeichen) < 0)
			{
				bNotFound = TRUE;
				break;
			}
		}

		if (!bNotFound)
		{		// Dann ist es wohl ein Sampler. Feld nun trennen.
			String^ sArtist;
			String^ sTitle;

			for (int i=0;i<cd->NumberOfTracks;i++)
			{
				int iPos = cd->Tracks[i]->Title->IndexOf(Archive->SamplerTrennzeichen);

				sArtist = cd->Tracks[i]->Title->Substring(0, iPos)->Trim();
				sTitle = cd->Tracks[i]->Title->Substring(iPos+1)->Trim();

				cd->Tracks[i]->Artist = sArtist;
				cd->Tracks[i]->Title = sTitle;
			}

			cd->Sampler = true;

			if (Big3::Hitbase::Configuration::Settings::Current->SamplerUseFixedArtist)
			{
				cd->Artist = Big3::Hitbase::Configuration::Settings::Current->SamplerFixedArtistText;
			}
		}
	}

	return TRUE;
}

BOOL CCDArchive::GetStatistics(int nIndex, int% nNumberOfCDs, String^% sDate)
{
	CWaitCursor wait;

	switch (Settings::Current->CDArchives[nIndex]->Type)
	{
	case CDArchiveType::File:
		return GetHitbaseFileStatistics(Settings::Current->CDArchives[nIndex]->ArchiveName, nNumberOfCDs, sDate);
	case CDArchiveType::CDArchiveLocal:
		return GetCDArchivLocalStatistics(Settings::Current->CDArchives[nIndex]->ArchiveName, nNumberOfCDs, sDate);
	case CDArchiveType::BIG3:
		return GetHitbaseInternetStatistics(Settings::Current->CDArchives[nIndex]->ArchiveName, nNumberOfCDs, sDate);
	case CDArchiveType::CDDBsockets:
	case CDArchiveType::CDDBhttp:
		return GetCDDBStatistics(Settings::Current->CDArchives[nIndex], nNumberOfCDs, sDate);
	}

	return TRUE;
}

BOOL CCDArchive::GetHitbaseInternetStatistics(String^ sInternetServer, int% nNumberOfCDs, String^% sDate)
{
	CString str;
	str.Format(get_string(TEXT_CONNECT), (CString)sInternetServer);
//!!!!!!!!!!!!!!!	CMisc::SetStatusText("");

	CInternetSession * pInternetSession;

	switch (Settings::Current->ProxyType)
	{
	case 0:
		pInternetSession = new CInternetSession;
		break;
	case 1:
		pInternetSession = new CInternetSession(NULL, 1, INTERNET_OPEN_TYPE_PROXY, (CString)Settings::Current->ProxyServerName);
		break;
	default:
		ASSERT(FALSE);
	}

	ASSERT(pInternetSession);

	if (!pInternetSession)
		return FALSE;

	CHttpConnection * pHttpConnection = 
		pInternetSession->GetHttpConnection(
		(CString)sInternetServer);

	BOOL bRet = FALSE;

	ASSERT(pHttpConnection);

	if (!pHttpConnection)
	{
		pInternetSession->Close();
		delete pInternetSession;

		return FALSE;
	}

//!!!!!!!!!!!!!!!	CMisc::SetStatusText(get_string(TEXT_READ_INFORMATION) + L"...");

	CString sInternetFileName = "/stat.asp";

	const TCHAR szHeaders[] =
		_T("Accept: text/*\r\nUser-Agent: Hitbase_2012\r\n");

	DWORD dwHttpRequestFlags =
		INTERNET_FLAG_EXISTING_CONNECT | INTERNET_FLAG_NO_AUTO_REDIRECT | INTERNET_FLAG_DONT_CACHE;

	CHttpFile * pFile = pHttpConnection->OpenRequest(CHttpConnection::HTTP_VERB_GET,
		sInternetFileName, NULL, 1, NULL, NULL, dwHttpRequestFlags);
	
	if (pFile)
	{
		pFile->AddRequestHeaders(szHeaders);
		pFile->SendRequest();

		bRet = ReadStatisticsFile(*pFile, nNumberOfCDs, sDate);

		pFile->Close();
		delete pFile;
	}

//!!!!!!!!!!!!!!!!!!	CMisc::SetStatusText("");

	pHttpConnection->Close();
	delete pHttpConnection;

	pInternetSession->Close();
	delete pInternetSession;

	return bRet;
}

BOOL CCDArchive::ReadStatisticsFile(CHttpFile & file, int% nNumberOfCDs, String^% sLastModified)
{
	USES_CONVERSION;
	nNumberOfCDs = 0;
	sLastModified = "";

	char str[10000];
	while (file.ReadString((LPTSTR)str, sizeof(str)))
	{
		CString sLine = CString(A2W(str));
		if (sLine.IsEmpty())
			continue;

		if (sLine.Left(12) == "NumberOfCDs=")
		{
			nNumberOfCDs = _wtoi(sLine.Mid(12));
			continue;
		}

		if (sLine.Left(13) == "LastModified=")
		{
			sLastModified = gcnew String(sLine.Mid(13));
			continue;
		}

		CString str;
		str.Format(L"Unknown tag found : %s", sLine);
		DisplayErrorMessage(gcnew String(str));

		// JUS 990607: Das ist jetzt ein schwerer Fehler!
		file.Close();
		return FALSE;
	}

	file.Close();

	return TRUE;
}

BOOL CCDArchive::GetHitbaseFileStatistics(String^ sInternetServer, int% nNumberOfCDs, String^% sDate)
{
	try
	{
		Big3::Hitbase::DataBaseEngine::DataBase^ db = gcnew Big3::Hitbase::DataBaseEngine::DataBase();

		db->Open(sInternetServer, true, false);
		nNumberOfCDs = (int)db->ExecuteScalar("SELECT count(*) from CD");

		db->Close();

		CFileStatus stat;
		CFile::GetStatus((CString)sInternetServer, stat);
		sDate = gcnew String(stat.m_mtime.Format("%c"));

		return TRUE;
	}
	catch (Exception^)
	{
		return false;
	}
}

BOOL CCDArchive::GetCDArchivLocalStatistics(String^ sFilename, int% nNumberOfCDs, String^% sDate)
{
	CDaoDatabase db;

	try
	{
		db.Open((CString)sFilename);

		CDaoRecordset RecordSet(&db);
		COleVariant var;

		RecordSet.Open(dbOpenDynaset, L"SELECT count(*) from CD", dbSeeChanges);

		var = RecordSet.GetFieldValue(0);

		RecordSet.Close();

		nNumberOfCDs = var.lVal;

		db.Close();

		CFileStatus stat;
		CFile::GetStatus((CString)sFilename, stat);
		sDate = gcnew String(stat.m_mtime.Format("%c"));
	}
	catch (CException* e)
	{
		e->Delete();
		return FALSE;
	}

	return TRUE;
}

BOOL CCDArchive::GetCDDBStatistics(CDArchiveConfig^ CDArchive, int% nNumberOfCDs, String^% sDate)
{
	// Connect und Stat Befehl senden
	int nReturnCode;
	CCDDBConnect cddb;
	BOOL SocketConnect;
	CString SendCmd;
	CString sBuffer;
	int nPos;

	if (CDArchive->Type == CDArchiveType::CDDBhttp)
	{
		SocketConnect = FALSE;
	}

	// socket Verbindung?
	if (CDArchive->Type == CDArchiveType::CDDBsockets)
	{
		SocketConnect = TRUE;
	}

	if (cddb.CDDBOpen(CDArchive) == FALSE)
	{
		DisplayErrorMessage ("Error CDDB Server connect");
		return FALSE;
	}

	// Check for answer...
	if (SocketConnect == TRUE && cddb.CDDBReceive (sBuffer) == FALSE)
	{
		// OK - Check for correct return string
		nReturnCode = CDDBGetReturnCode(sBuffer);
		//200	OK, read/write allowed	
		//201	OK, read only
		//432	No connections allowed: permission denied
		//433	No connections allowed: X users allowed, Y currently active
		//434	No connections allowed: system load too high    
		if (nReturnCode == 200 || nReturnCode == 201)
		{
			// Connect OK
		}
		else
		{
			CString s;
			s.Format(L"Error cddb server\n\n%s", sBuffer);
			DisplayErrorMessage (gcnew String(s));

			cddb.CDDBClose();
			return FALSE;
		}
	}

	if (cddb.CDDBInit () == FALSE)
	{
		// Fehler Send nun noch Close() socket und zurück
		DisplayErrorMessage ("Error cddb server: Send timeout");
		cddb.CDDBClose ();
		return FALSE;
	}

	// Wait for handshake reply...
	//memset (sockbuf, 0, sizeof(sockbuf));
	if (SocketConnect == TRUE && cddb.CDDBReceive(sBuffer) == TRUE)
	{
		nReturnCode = CDDBGetReturnCode(sBuffer);
		if (nReturnCode == 200)
		{
			// Handshake OK!
			// Go on...
		}
		else
		{
			// Sorry no handshake!
			// Close Connection
			CString s;
			s.Format(L"Error cddb server\n\n%s", sBuffer);
			DisplayErrorMessage (gcnew String(s));
			cddb.CDDBClose();
			return FALSE;
		}
	}

	SendCmd = "stat\n";
	if (cddb.CDDBTransmit (SendCmd) == FALSE)
	{
		// Fehler Send nun noch Close() socket und zurück
		DisplayErrorMessage ("Error cddb server: Send timeout");
		cddb.CDDBClose ();
		return FALSE;
	}

	// Nun Daten bis zum Punkt lesen...
	if (cddb.CDDBReceive(sBuffer, TRUE) == TRUE)
	{
		nReturnCode = CDDBGetReturnCode(sBuffer);
		if (nReturnCode != 210)
		{
			// Close Connection
			CString s;
			s.Format(L"Error cddb server\n\n%s", sBuffer);
			DisplayErrorMessage (gcnew String(s));
			cddb.CDDBClose();
			return FALSE;
		}
	}
	if ((nPos = sBuffer.Find(L"Database entries:")) > 0)
	{
		CString NumEntries;

		NumEntries = sBuffer.Mid(nPos);
		NumEntries = NumEntries.Mid(18);

		nNumberOfCDs = _wtol(NumEntries);

		sDate = gcnew String(GetRealDate());

		cddb.CDDBClose();
		return TRUE;
	}

	cddb.CDDBClose();

	return FALSE;
}

// Entfernen von Sonderzeichen 
void CCDArchive::RemoveSpecialChars (String^% line)
{
//	int pos;

	line = line->Replace("\\n", "; ");
	line = line->Replace("\\r", "; ");
	line = line->Replace("\\t", " ");
	line = line->Replace(";;", ";");

/*	while ((pos = line->Find(L"\\n")) != -1)
	{
		//CString str;
		*line = line->Left(pos) + "; " + line->Mid(pos + 2);
		//*line = str;
	}
	while ((pos = line->Find(L"\\r")) != -1)
	{
		*line = line->Left(pos) + "; " + line->Mid(pos + 2);
	}
	while ((pos = line->Find(L"\\t")) != -1)
	{
		*line = line->Left(pos) + " " + line->Mid(pos + 2);
	}
	while ((pos = line->Find(L";;")) != -1)
	{
		*line = line->Left(pos) + ";" + line->Mid(pos + 2);
	}*/
}

// Lese das erste Wort vom String
// return: Integer des ersten Wortes
int CCDArchive::CDDBGetReturnCode(CString ReturnString)
{
	return _wtol(ReturnString.Left(ReturnString.Find(' ')));
}

// Lese das erste Wort vom String
// return: Integer des ersten Wortes
int CCDArchive::CDDBGetKategorieCDID(CString ReturnString, CString & Kategorie, CString & CDID)
{
	Kategorie = ReturnString.Mid(ReturnString.Find(' ')+1);
	ReturnString = Kategorie;
	Kategorie = Kategorie.Left(Kategorie.Find(' '));

	CDID = ReturnString.Mid(ReturnString.Find(' ')+1);
	CDID = CDID.Left(CDID.Find(' '));

	return TRUE;
}

// Liefert TRUE zurück, wenn in der Liste der Archive ein Archiv vorhanden ist, in 
// dem man manuell Suchen kann. Zur Zeit ist die nur das Hitbase (Datei) Archiv.
BOOL CCDArchive::IsSearchableArchivAvailable()
{
	for (int i=0;i<Settings::Current->CDArchives->Count;i++)
		if (Settings::Current->CDArchives[i]->Active && (Settings::Current->CDArchives[i]->Type == CDArchiveType::File || Settings::Current->CDArchives[i]->Type == CDArchiveType::BIG3 || Settings::Current->CDArchives[i]->Type == CDArchiveType::CDArchiveLocal))
			return TRUE;

	return FALSE;
}

// Liefert den Typ des ersten zu durchsuchenden Archives zurück.
CDArchiveType CCDArchive::GetSearchableArchivType()
{
	for (int i=0;i<Settings::Current->CDArchives->Count;i++)
		if (Settings::Current->CDArchives[i]->Active && (Settings::Current->CDArchives[i]->Type == CDArchiveType::File || Settings::Current->CDArchives[i]->Type == CDArchiveType::BIG3 || Settings::Current->CDArchives[i]->Type == CDArchiveType::CDArchiveLocal))
			return Settings::Current->CDArchives[i]->Type;

	ASSERT(false);

	return (CDArchiveType)-1;
}

// Liefert True zurück, wenn ein durchsuchbares Archiv automatisch beim Einlegen einer CD durchsucht wird.
BOOL CCDArchive::IsSearchableArchivAutoSearch()
{
	for (int i=0;i<Settings::Current->CDArchives->Count;i++)
		if (Settings::Current->CDArchives[i]->Active && Settings::Current->CDArchives[i]->AutoSearch && (Settings::Current->CDArchives[i]->Type == CDArchiveType::File || Settings::Current->CDArchives[i]->Type == CDArchiveType::BIG3 || Settings::Current->CDArchives[i]->Type == CDArchiveType::CDArchiveLocal))
			return TRUE;

	return FALSE;
}

// Liefert TRUE zurück, wenn in der Liste der Archive ein aktives Archiv vorhanden ist.
BOOL CCDArchive::IsArchiveActive()
{
	for (int i=0;i<Settings::Current->CDArchives->Count;i++)
		if (Settings::Current->CDArchives[i]->Active)
			return TRUE;

	return FALSE;
}

// Liefert TRUE zurück, wenn in der Liste der Archive ein aktives Archiv vorhanden ist.
BOOL CCDArchive::IsUploadableArchivAvailable()
{
	for (int i=0;i<Settings::Current->CDArchives->Count;i++)
		if (Settings::Current->CDArchives[i]->Upload && Settings::Current->CDArchives[i]->Active)
			return TRUE;

	return FALSE;
}

// Liefert den Namen des ersten durchsuchbaren Archivs zurück. Dies sollte vielleicht
// später noch so erweitert werden, daß man auch in mehreren Archiven gleichzeitig
// suchen kann.
String^ CCDArchive::GetSearchableArchivName()
{
	for (int i=0;i<Settings::Current->CDArchives->Count;i++)
		if (Settings::Current->CDArchives[i]->Active && (Settings::Current->CDArchives[i]->Type == CDArchiveType::File || Settings::Current->CDArchives[i]->Type == CDArchiveType::BIG3 || Settings::Current->CDArchives[i]->Type == CDArchiveType::CDArchiveLocal))
			return Settings::Current->CDArchives[i]->ArchiveName;

	return "";
}

// Liefert den Namen des ersten durchsuchbaren Archivs zurück. Dies sollte vielleicht
// später noch so erweitert werden, daß man auch in mehreren Archiven gleichzeitig
// suchen kann.
String^ CCDArchive::GetBrowseableOnlineArchiv()
{
	for (int i=0;i<Settings::Current->CDArchives->Count;i++)
		if (Settings::Current->CDArchives[i]->Active && (Settings::Current->CDArchives[i]->Type == CDArchiveType::BIG3))
			return Settings::Current->CDArchives[i]->ArchiveName;

	return "";
}

bool CCDArchive::IsArchiveBrowseable(CDArchiveConfig^ archive)
{
	if (archive->Type == CDArchiveType::BIG3 || archive->Type == CDArchiveType::CDArchiveLocal)
		return true;

	return false;
}

BOOL CCDArchive::Upload(Big3::Hitbase::DataBaseEngine::CD^ cd, int% iResult, IntPtr^ parent, DWORD dwUploadOptions)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	m_bVerbose = (dwUploadOptions & (int)CDArchivUploadOptions::UPLOAD_OPTIONS_VERBOSE) ? TRUE : FALSE;
	m_bCDUpdate = (dwUploadOptions & (int)CDArchivUploadOptions::UPLOAD_OPTIONS_CD_UPDATE) ? TRUE : FALSE;

	if (m_bVerbose)
	{
		CWnd* pParentWnd = (CWnd*)parent->ToPointer();
		m_pUploadCDsDlg = new CUploadDownloadCDsDlg(pParentWnd);
		m_pUploadCDsDlg->CDArchives = this;
		m_pUploadCDsDlg->Create(pParentWnd);
		m_pUploadCDsDlg->m_StartCtrl.ShowWindow(SW_HIDE);
		m_pUploadCDsDlg->SetWindowPos(NULL, 0, 0, 0, 0, SWP_NOZORDER|SWP_NOMOVE|SWP_NOSIZE|SWP_SHOWWINDOW);

		CRect rect;
		m_pUploadCDsDlg->m_ListCtrl.GetClientRect(&rect);
		m_pUploadCDsDlg->m_ListCtrl.InsertColumn(0, get_string(TEXT_SERVER), LVCFMT_LEFT, rect.right/4*3);
		m_pUploadCDsDlg->m_ListCtrl.InsertColumn(1, get_string(TEXT_STATUS), LVCFMT_LEFT, rect.right/4);

		CString str;
		str.Format(get_string(TEXT_SEND_CDS), 1, 1);
		m_pUploadCDsDlg->m_sCurrentAction = str;
		m_pUploadCDsDlg->UpdateData(FALSE);
	}

	// PeekMessage, damit der Dialog auf dem Bildschirm erscheint!!
	MSG msg;
	while (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE))
	{
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}

	CString sErrorMessage;

	// Zuerst Plausi-Prüfungen!!
	if (!CheckCDForUpload(cd, sErrorMessage))
	{
		if (m_bVerbose)
		{
			m_pUploadCDsDlg->m_ListCtrl.InsertItem(0, L"");
			m_pUploadCDsDlg->m_ListCtrl.SetItem(0, 1, LVIF_TEXT, get_string(TEXT_FAILED), 0, 0, 0, 0);

			m_pUploadCDsDlg->m_ErrorsCtrl.SetWindowText(sErrorMessage);
			m_pUploadCDsDlg->m_DetailsCtrl.EnableWindow(TRUE);
			m_pUploadCDsDlg->m_CancelCtrl.SetWindowText(get_string(TEXT_CLOSE));
		}
		else
		{
			CString s;
			s.Format(get_string(TEXT_UPLOAD_ERROR_DETAIL), (CString)cd->Artist, (CString)cd->Title, sErrorMessage);
			m_sLastDetailMessage = gcnew String(s);
		}

		iResult = (int)CDArchivUploadResults::UPLOAD_ARCHIV_CD_ERROR;

		return FALSE;
	}

	BOOL bRet = TRUE, bCanceled = FALSE;

	int nCount=0;
	for (int i=0;i<Settings::Current->CDArchives->Count && !bCanceled;i++)
	{
		if (Settings::Current->CDArchives[i]->Active && Settings::Current->CDArchives[i]->Upload)
		{
			if (m_bVerbose)
			{
				m_pUploadCDsDlg->m_ListCtrl.InsertItem(nCount, (CString)Settings::Current->CDArchives[i]->ArchiveName);
			}

			m_nCurrentLine = nCount;

			if (m_bVerbose)
				m_pUploadCDsDlg->m_ListCtrl.SetItem(m_nCurrentLine, 1, LVIF_TEXT, get_string(TEXT_CONNECTING) + L"...", 0, 0, 0, 0);

			// PeekMessage, damit der Dialog auf dem Bildschirm erscheint!!
			MSG msg;
			while (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE))
			{
				TranslateMessage(&msg);
				DispatchMessage(&msg);
			}

			switch (Settings::Current->CDArchives[i]->Type)
			{
			case CDArchiveType::File:
				bRet = UploadCDInCDArchiveFile(Settings::Current->CDArchives[i], cd, bCanceled);
				break;
			case CDArchiveType::BIG3:
				bRet = UploadCDInInternetBig3(Settings::Current->CDArchives[i], cd, bCanceled, iResult);		
				break;
			case CDArchiveType::CDDBsockets:
			case CDArchiveType::CDDBhttp:
				bRet = UploadCDInCDDB(Settings::Current->CDArchives[i], cd, bCanceled, iResult);
				break;
			}

			if (!bRet && m_bVerbose)
				m_pUploadCDsDlg->m_ListCtrl.SetItem(m_nCurrentLine, 1, LVIF_TEXT, get_string(TEXT_FAILED), 0, 0, 0, 0);

			nCount++;
		}
	}

	if (bRet && m_bVerbose)
		delete m_pUploadCDsDlg;

	return bRet;
}

// Angegebene CD auf den Hitbase CD-Server uploaden.
BOOL CCDArchive::UploadCDInCDArchiveFile(CDArchiveConfig^ Archive, Big3::Hitbase::DataBaseEngine::CD^ cd, BOOL% bCanceled)
{
	return FALSE;
}

BOOL CCDArchive::UploadCDInInternetBig3(CDArchiveConfig^ Archive, Big3::Hitbase::DataBaseEngine::CD^ cd, BOOL% bCanceled, int% iResult)
{
	CInternetSession * pInternetSession;

	switch (Settings::Current->ProxyType)
	{
	case 0:
		pInternetSession = new CInternetSession;
		break;
	case 1:
		pInternetSession = new CInternetSession(NULL, 1, INTERNET_OPEN_TYPE_PROXY, (CString)Settings::Current->ProxyServerName);
		break;
	default:
		ASSERT(FALSE);
	}

	ASSERT(pInternetSession);

	CHttpConnection * pHttpConnection;
	try
	{
		pHttpConnection = pInternetSession->GetHttpConnection((CString)Archive->ArchiveName);
	}
	catch (CInternetException* e)
	{
		wchar_t szCause[255];
		e->GetErrorMessage(szCause, 255);
		DisplayErrorMessage(gcnew String(szCause));

		e->Delete();
		pInternetSession->Close();
		delete pInternetSession;

		iResult = (int)CDArchivUploadResults::UPLOAD_ARCHIV_ERROR;

		return FALSE;
	}

	BOOL ret = UploadCD(pHttpConnection, cd, UPLOADTYPE_HITBASE, iResult);

	pHttpConnection->Close();
	delete pHttpConnection;

	pInternetSession->Close();
	delete pInternetSession;

	return ret;
}

BOOL CCDArchive::UploadCDInCDDB(CDArchiveConfig^ Archive, Big3::Hitbase::DataBaseEngine::CD^ cd, BOOL% bCanceled, int% iResult)
{
	// Hier geschieht ein Wunder
	CInternetSession * pInternetSession;

	switch (Settings::Current->ProxyType)
	{
	case 0:
		pInternetSession = new CInternetSession;
		break;
	case 1:
		pInternetSession = new CInternetSession(NULL, 1, INTERNET_OPEN_TYPE_PROXY, (CString)Settings::Current->ProxyServerName);
		break;
	default:
		ASSERT(FALSE);
	}

	ASSERT(pInternetSession);

	CString sNew;
	sNew = CString(Archive->ArchiveName);
	//sNew = sNew.Left(sNew.Find(L"cddb.cgi")) + L"submit.cgi";
	sNew = sNew.Left(sNew.Find(L"/"));

	CHttpConnection * pHttpConnection;
	try
	{
		pHttpConnection = pInternetSession->GetHttpConnection(sNew);
	}
	catch (CInternetException* e)
	{
		wchar_t szCause[255];
		e->GetErrorMessage(szCause, 255);
		DisplayErrorMessage(gcnew String(szCause));

		e->Delete();
		pInternetSession->Close();
		delete pInternetSession;

		iResult = (int)CDArchivUploadResults::UPLOAD_ARCHIV_ERROR;

		return FALSE;
	}

	BOOL ret = UploadCD(pHttpConnection, cd, UPLOADTYPE_CDDB, iResult);

	pHttpConnection->Close();
	delete pHttpConnection;

	pInternetSession->Close();
	delete pInternetSession;

	return ret;
}

// Eine CD uploaden!
BOOL CCDArchive::UploadCD(CHttpConnection * pHttpConnection, Big3::Hitbase::DataBaseEngine::CD^ cd, int nType, int% iResult)
{
	CWaitCursor wait;

	ASSERT(pHttpConnection);
	if (String::IsNullOrEmpty(cd->Identity))
	{
		iResult = (int)CDArchivUploadResults::UPLOAD_ARCHIV_CD_ERROR;

		ASSERT(FALSE);
		return FALSE;
	}

	// PeekMessage, damit der Dialog auf dem Bildschirm erscheint!!
	MSG msg;
	while (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE))
	{
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}

	CString sResult = DoUploadCDToCDArchive(pHttpConnection, cd, nType, FALSE);

	BOOL bError = FALSE;

	int uploadResult = 0;

	if (nType == UPLOADTYPE_CDDB)
	{
		if (sResult.Left(3) == "200")		// 200 OK, submission has been sent.	
			uploadResult = 1;
	}
	else
	{
		uploadResult = _wtoi(sResult);
	}

	// Antwort überprüfen!
	switch (uploadResult)
	{
	case 1:
		iResult = (int)CDArchivUploadResults::UPLOAD_ARCHIV_NEW_CD;
		break;
	case 2:
		{
			if (m_bCDUpdate)     // Fragen, ob CD im CD-Archiv aktualisiert werden soll.
			{
				CString str;
				str.Format(get_string(IDS_UPDATE_CD), (CString)cd->Artist, (CString)cd->Title);
				if (m_pUploadCDsDlg->MessageBox(str, AfxGetAppName(), MB_ICONQUESTION|MB_YESNO) == IDYES)
				{
					sResult = DoUploadCDToCDArchive(pHttpConnection, cd, nType, TRUE);

					// Antwort überprüfen!
					switch (_wtoi(sResult))
					{
					case 1:
						iResult = (int)CDArchivUploadResults::UPLOAD_ARCHIV_NEW_CD;
						break;
					default:
						iResult = (int)CDArchivUploadResults::UPLOAD_ARCHIV_ERROR;
						bError = TRUE;
						if (m_bVerbose)
						{
							m_pUploadCDsDlg->m_ErrorsCtrl.SetWindowText(sResult);
							m_pUploadCDsDlg->m_DetailsCtrl.EnableWindow(TRUE);
						}
						break;
					}
				}
				else
				{
					iResult = (int)CDArchivUploadResults::UPLOAD_ARCHIV_CD_EXISTS;
				}
			}
			else
			{
				iResult = (int)CDArchivUploadResults::UPLOAD_ARCHIV_CD_EXISTS;
			}
			break;
		}
	default:
		iResult = (int)CDArchivUploadResults::UPLOAD_ARCHIV_ERROR;
		bError = TRUE;
		if (m_bVerbose)
		{
			m_pUploadCDsDlg->m_ErrorsCtrl.SetWindowText(sResult);
			m_pUploadCDsDlg->m_DetailsCtrl.EnableWindow(TRUE);
		}
		else
		{
			CString s;
			s.Format(get_string(TEXT_UPLOAD_ERROR_DETAIL), (CString)cd->Artist, (CString)cd->Title, get_string(TEXT_UPLOAD_INTERNAL_ERROR));
			m_sLastDetailMessage = gcnew String(s);
		}
	}

	if (m_bVerbose)
	{
		if (bError)
			m_pUploadCDsDlg->m_ListCtrl.SetItem(m_nCurrentLine, 1, LVIF_TEXT, get_string(TEXT_FAILED), 0, 0, 0, 0);
		else
			m_pUploadCDsDlg->m_ListCtrl.SetItem(m_nCurrentLine, 1, LVIF_TEXT, get_string(TEXT_SUCCESS), 0, 0, 0, 0);

		m_pUploadCDsDlg->m_CancelCtrl.SetWindowText(get_string(TEXT_CLOSE));
	}

	return !bError;
}

// Liefert einen formatierten String zurück, in dem alle Informationen
// der angegebenen CD im Klartext stehen.
CString CCDArchive::DoUploadCDToCDArchive(CHttpConnection* pHttpConnection, Big3::Hitbase::DataBaseEngine::CD^ cd, int nType, BOOL bForceUpdate)
{
	USES_CONVERSION;
	CString sUploadURL = "upload_cd.asp";
	CString sCDData;
	CString header;

	if (nType == UPLOADTYPE_CDDB)
	{
		sUploadURL = "/~cddb/submit.cgi";
	}

	if (nType == UPLOADTYPE_HITBASE)
	{
		sUploadURL = "upload_cd.asp";
		sCDData.Format(L"User=Hitbase&Password=hitbase2k&Update=%d&%s", bForceUpdate ? 1 : 0, GetCDInfoAsString(cd));
	}

	CString strHeaders = _T("Content-Type: application/x-www-form-urlencoded");

	CHttpFile * pFile;
	try
	{
		pFile = pHttpConnection->OpenRequest(CHttpConnection::HTTP_VERB_POST, sUploadURL);

		//if (header.GetLength() > 0)
		//	pFile->AddRequestHeaders(header);
		if (nType == UPLOADTYPE_CDDB)
		{
			// TODO category
			// valid categories: blues, classical, country, data, folk, jazz, misc, newage, reggae, rock, soundtrack

			int cddbid = cd->GetCDDBDiscID();
			//CDDBFillCDInfo(pCD, CDDBData, Archive); DTITLE="

			//TODO!!!!!!!! category setzen
			CString sBuf2;

			pFile->AddRequestHeaders(L"Category: rock");
			sBuf2.Format(L"Discid: %x", cddbid);
			pFile->AddRequestHeaders(sBuf2);
			pFile->AddRequestHeaders(L"User-Email: autoupload@basehit1.de");
			pFile->AddRequestHeaders(L"Submit-Mode: test");
			pFile->AddRequestHeaders(L"Charset: ISO-8859-1");
			pFile->AddRequestHeaders(L"X-Cddbd-Note: Hitbase");

			strHeaders = "";

			sCDData = 	L"# xmcd\r\n"
				L"# \r\n"
				L"# Track frame offsets:\r\n";

			CString sBuf;

			// pCD->m_dwTotalLength
			int nSumAllTracks=0;
			for (int nPos = 0; nPos < cd->NumberOfTracks; nPos++)
				nSumAllTracks += cd->Tracks[nPos]->Length;

			for (int nPos = 0; nPos < cd->NumberOfTracks; nPos++)
			{
				// Frames Start der Tracks ermitteln.
				sBuf.Format(L"# %d\r\n", (cd->Tracks[nPos]->StartPosition * 75) / 1000); 
				sCDData += sBuf;
			}

			/* 
			sArtist, sTitle, pCD->m_dwTotalLength, pCD->m_bSampler, 
			pCD->m_wNumberOfTracks, sCategory, sMedium, sComment, pCD->m_lYearRecorded, 
			sCopyright, sUPC, sLabel);
			CString sArtist = CMisc::ConvertToHTTP(pCD->GetArtist()->m_sArtist);
			CString sTitle = CMisc::ConvertToHTTP(pCD->m_sTitle);
			CString sCategory = CMisc::ConvertToHTTP(pCD->GetCategory()->m_sCategory);
			CString sMedium = CMisc::ConvertToHTTP(pCD->GetMedium()->m_sMedium);
			CString sComment = CMisc::ConvertToHTTP(pCD->m_sComment);
			CString sCopyright = CMisc::ConvertToHTTP(pCD->m_sCopyright);

			*/

			sCDData += L"#\r\n";

			// TODO - genaue Berechnung Disc-Length???
			sBuf.Format(L"# Disc length: %d seconds\r\n#\r\n", cd->TotalLength / 1000);
			sCDData += sBuf;

			sCDData += L"#\r\n";
			sCDData += L"# Revision: 0\r\n";
			sCDData += L"# Processed by: cddbd v1.4PL0 Copyright (c) Steve Scherf et al.\r\n";
			sCDData += L"# Submitted via: Hitbase\r\n";
			sCDData += L"#\r\n";
			sBuf.Format(L"DISCID=%x\r\n", cddbid);
			sCDData += sBuf;

			sBuf.Format(L"DTITLE=%s / %s\r\n", (CString)cd->Artist, (CString)cd->Title);
			sCDData += sBuf;
			sBuf.Format(L"DYEAR=%d\r\n", cd->YearRecorded);
			sCDData += sBuf;
			// TODO - Übersetzungsliste für category->Genre?
			sBuf.Format(L"DGENRE=rock\r\n", cd->Category);
			sCDData += sBuf;

			for (int nPos = 0; nPos < cd->NumberOfTracks; nPos++)
			{
				sBuf.Format(L"TTITLE%d=%s\r\n", nPos, (CString)cd->Tracks[nPos]->Title);
				sCDData += sBuf;
			}

			sBuf.Format(L"EXTD=%s\r\n", (CString)cd->Comment);
			sCDData += sBuf;

			for (int nPos = 0; nPos < cd->NumberOfTracks; nPos++)
			{
				if (cd->Sampler)
				{
					sBuf.Format(L"EXTT%d=%s\r\n", nPos, (CString)cd->Tracks[nPos]->Artist);
					sCDData += sBuf;
				}
				else
				{
					// TODO - wollen wir das exportieren?
					sBuf.Format(L"EXTT%d=%s\r\n", nPos, (CString)cd->Tracks[nPos]->Comment);
					sCDData += sBuf;
				}
			}

			sCDData += L"PLAYORDER=\r\n";
			//AfxMessageBox(sCDData);
			//sBuf = CMisc::ConvertToHTTP(sCDData);
			//RemoveSpecialChars(&sBuf);
		}

		// JUS 01.03.2003
		if (!pFile)
		{
			AfxMessageBox(get_string(TEXT_UPLOAD_INTERNAL_ERROR), MB_OK|MB_ICONEXCLAMATION);
			if (m_bVerbose)
			{
				UINT uINT = GetLastError();
				m_pUploadCDsDlg->m_ErrorsCtrl.SetWindowText(get_string(TEXT_UPLOAD_INTERNAL_ERROR));
				m_pUploadCDsDlg->m_DetailsCtrl.EnableWindow(TRUE);
			}
			return "";
		}

		pFile->SendRequest(strHeaders, (LPVOID)(LPCTSTR)W2A(sCDData), sCDData.GetLength()); 
	}
	catch (CException *e)
	{
		wchar_t szCause[255];
		e->GetErrorMessage(szCause, 255);
		if (m_bVerbose)
		{
			m_pUploadCDsDlg->m_ErrorsCtrl.SetWindowText(szCause);
			m_pUploadCDsDlg->m_DetailsCtrl.EnableWindow(TRUE);
		}

		e->Delete();
		pFile->Close();
		delete pFile;

//!!!!!!!!!!!!!!!!!		CMisc::SetStatusText("");

		return "";
	}

	CString sResult;

	char str1[10000];
	while (pFile->ReadString((LPTSTR)str1, sizeof(str1)))
		sResult += CString(A2W(str1));


	pFile->Close();
	delete pFile;

	return sResult;
}

// Liefert einen formatierten String zurück, in dem alle Informationen
// der angegebenen CD im Klartext stehen.
CString CCDArchive::GetCDInfoAsString(Big3::Hitbase::DataBaseEngine::CD^ cd)
{
	CString sData;
	CString str;

	// Zuerst die CD-Daten.
	CString sArtist = CMisc::ConvertToHTTP(cd->Artist);
	CString sTitle = CMisc::ConvertToHTTP(cd->Title);
	CString sCategory = CMisc::ConvertToHTTP(cd->Category);
	CString sMedium = CMisc::ConvertToHTTP(cd->Medium);
	CString sComment = CMisc::ConvertToHTTP(cd->Comment);
	CString sCopyright = CMisc::ConvertToHTTP(cd->Copyright);
	// 08.02.2004
	CString sUPC = CMisc::ConvertToHTTP(cd->UPC);
	CString sLabel = CMisc::ConvertToHTTP(cd->Label);

	str.Format(L"Identity=%s&Artist=%s&Title=%s&Totallength=%d&Sampler=%d&Tracks=%d&Category=%s&Medium=%s&Comment=%s&YearRecorded=%d&Copyright=%s&UPC=%s&Label=%s", 
		(CString)cd->Identity, sArtist, sTitle, cd->TotalLength, cd->Sampler ? 1 : 0, 
		cd->NumberOfTracks, sCategory, sMedium, sComment, cd->YearRecorded, 
		sCopyright, sUPC, sLabel);
	sData += str;

	// Jetzt noch alle Lieder
	for (int i=0;i<cd->NumberOfTracks;i++)
	{
		long lYearRecorded;

		CString sTitle = CMisc::ConvertToHTTP(cd->Tracks[i]->Title);
		CString sComment = CMisc::ConvertToHTTP(cd->Tracks[i]->Comment);

		if (cd->Tracks[i]->YearRecorded)
			lYearRecorded = cd->Tracks[i]->YearRecorded;
		else
			lYearRecorded = cd->YearRecorded;

		sData += "&";
		if (cd->Sampler)
		{
			CString sArtist = CMisc::ConvertToHTTP(cd->Tracks[i]->Artist);
			str.Format(L"Artist_T%d=%s&Title_T%d=%s&Length_T%d=%d&BPM_T%d=%d&Comment_T%d=%s&YearRecorded_T%d=%d", 
				i, sArtist, i, sTitle, i, cd->Tracks[i]->Length, i, cd->Tracks[i]->Bpm, i, sComment, i, lYearRecorded);
		}
		else
			str.Format(L"Title_T%d=%s&Length_T%d=%d&BPM_T%d=%d&Comment_T%d=%s&YearRecorded_T%d=%d", 
			i, sTitle, i, cd->Tracks[i]->Length, i, cd->Tracks[i]->Bpm, i, sComment, i, lYearRecorded);

		sData += str;
	}

	return sData;
}

// Hier Plausi-Prüfungen!!
BOOL CCDArchive::CheckCDForUpload(Big3::Hitbase::DataBaseEngine::CD^ cd, CString& sErrorMessage)
{
	// Identity muss gesetzt sein!
	if (String::IsNullOrEmpty(cd->Identity))
	{
		sErrorMessage.Format(get_string(TEXT_UPLOAD_VERIFY_ERROR), get_string(TEXT_EXPORT_CDIDENTIFIKATION));
		return FALSE;
	}

	// Nur Audio-CDs uploaden
	if (cd->Type != AlbumType::AudioCD)
	{
		sErrorMessage.Format(get_string(TEXT_CANNOT_UPLOAD_DATA_CDS));
		return FALSE;
	}

	// Interpret muss gefüllt sein!
	if (String::IsNullOrEmpty(cd->Artist))
	{
		sErrorMessage.Format(get_string(TEXT_UPLOAD_VERIFY_ERROR), get_string(TEXT_ARTIST));
		return FALSE;
	}

	// Titel muss gefüllt sein!
	// 20.06.2001: Bisschen viel cut&paste gemacht! ;o)
	//	if (pCD->GetArtist()->m_sArtist.IsEmpty())
	if (String::IsNullOrEmpty(cd->Title))
	{
		sErrorMessage.Format(get_string(TEXT_UPLOAD_VERIFY_ERROR), get_string(TEXT_TITLE));
		return FALSE;
	}

	// CD muss mindestens 10 Sekunden lang sein!
	if (cd->TotalLength < 10000)
	{
		sErrorMessage.Format(get_string(TEXT_UPLOAD_VERIFY_ERROR), get_string(TEXT_TOTALLENGTH));
		return FALSE;
	}

	if (cd->Tracks->Count != cd->NumberOfTracks)
	{
		sErrorMessage.Format(get_string(TEXT_TRACKS_NOT_EXISTS));
		return FALSE;
	}

	// Jedes Lied muss gefüllt sein!
	for (int i=0;i<cd->Tracks->Count;i++)
	{
		if (String::IsNullOrEmpty(cd->Tracks[i]->Title))
		{
			CString str;
			str.Format(L"%s %d", get_string(TEXT_TRACK), i+1);
			sErrorMessage.Format(get_string(TEXT_UPLOAD_VERIFY_ERROR), str);
			return FALSE;
		}
	}

	return TRUE;
}

void CCDArchive::DisplayErrorMessage(String^ message)
{
	CString str;

	str = Big3::Hitbase::SharedResources::StringTable::Error;
	//str.LoadString(TEXT_ERROR);
	
	if (m_bVerbose)
		AfxGetMainWnd()->MessageBox((CString)message, str, MB_OK|MB_ICONSTOP);
	else
		m_sLastErrorMessage = message;
}

CString CCDArchive::GetLastErrorMessage()
{
	return m_sLastErrorMessage;
}

// Alle ausstehenden Aktionen (Upload/Download) ausführen!
void CCDArchive::UploadDownload(Big3::Hitbase::DataBaseEngine::DataBase^ db)
{
	// Erst mal prüfen, ob überhaupt was in der Warteschlange steht.
/*!!!!!!!!!!!!!!!	CString sQuery;
	CQueue queue(pdb);

	// Zuerst mal die Anzahl der Aktionen abfragen.
	sQuery.Format(L"Q_lAction = %d OR Q_lAction = %d", QUEUE_ACTION_DOWNLOAD, QUEUE_ACTION_UPLOAD);
	long lTotalCount = 0;
	if (queue.QueryStart(sQuery))
		lTotalCount = queue.GetCount();

	queue.QueryEnd();
	queue.Close();

	if (!lTotalCount)
	{
		AfxMessageBox(get_string(TEXT_EMPTY_QUEUE));
		return;
	}

	m_pUploadDownloadDB = pdb;

	m_pUploadCDsDlg = new CUploadDownloadCDsDlg(this);
	if (lTotalCount == 1)
		m_pUploadCDsDlg->m_sCurrentAction = get_string(TEXT_QUEUE_CONTENT_SINGULAR);
	else
		m_pUploadCDsDlg->m_sCurrentAction.Format(get_string(TEXT_QUEUE_CONTENT_PLURAL), lTotalCount);
	m_pUploadCDsDlg->DoModal();
*/
}

void CCDArchive::StartUploadDownload()
{
/*!!!!!!!!!!!!!!!	CRect rect;
	m_pUploadCDsDlg->m_ListCtrl.GetClientRect(&rect);
	m_pUploadCDsDlg->m_ListCtrl.InsertColumn(0, get_string(TEXT_SERVER), LVCFMT_LEFT, rect.right/4*3);
	m_pUploadCDsDlg->m_ListCtrl.InsertColumn(1, get_string(TEXT_STATUS), LVCFMT_LEFT, rect.right/4);

	CString sQuery;
	CQueue queue(m_pUploadDownloadDB);

	// Zuerst mal die Anzahl der Aktionen abfragen.
	sQuery.Format(L"Q_lAction = %d OR Q_lAction = %d", QUEUE_ACTION_DOWNLOAD, QUEUE_ACTION_UPLOAD);
	queue.QueryStart(sQuery);

	long lTotalCount = queue.GetCount();
	BOOL bErrorsOccured = FALSE;

	queue.QueryEnd();
	queue.Close();

	// Jetzt den ProgressCtrl mit der Anzahl setzen
	m_pUploadCDsDlg->m_ProgressCtrl.SetRange32(0, lTotalCount);
	m_pUploadCDsDlg->m_ProgressCtrl.SetStep(1);

	// Downloads
	sQuery.Format(L"Q_lAction = %d", QUEUE_ACTION_DOWNLOAD);
	queue.QueryStart(sQuery);

	if (!queue.IsEOF())
	{
		long lRecordCount = queue.GetCount();
		long lCount = 0;
		long lCDsFound = 0;
		m_pUploadCDsDlg->m_ListCtrl.InsertItem(0, get_string(TEXT_DOWNLOAD_CDS));

		BOOL bRet = queue.QueryFindFirst();
		while (bRet)
		{
			CString str;
			str.Format(get_string(TEXT_SEARCH_CDS), lCount+1, lRecordCount);
			m_pUploadCDsDlg->m_sCurrentAction = str;
			m_pUploadCDsDlg->UpdateData(FALSE);
			m_pUploadCDsDlg->m_ProgressCtrl.StepIt();

			CCD CD(m_pUploadDownloadDB);
			CD.GetIdentity()->m_sIdentity = queue.m_sIdentity;

			m_sUploadCDIdentityCDDB = queue.m_sIdentityCDDB;

			str.Format(get_string(TEXT_X_OF_X), lCount+1, lRecordCount);
			m_pUploadCDsDlg->m_ListCtrl.SetItem(0, 1, LVIF_TEXT, str, 0, 0, 0, 0);

			BOOL bError = FALSE;
			BOOL bCDFound = FALSE;//!!!!!!!!!!!!!!!!!!!!!!Search(&CD, FALSE, bError, FALSE);

			// Wenn ein Fehler aufgetreten ist, dann in der Queue lassen!
			if (!bError)
			{
				if (bCDFound)       // CD gefunden?
				{
					CD.Add();
					lCDsFound++;
				}

				queue.Delete();     // Diesen Eintrag jetzt löschen!
			}
			else
			{
				m_pUploadCDsDlg->AddDetailsString(GetLastErrorMessage());
				bErrorsOccured = TRUE;
			}

			bRet = queue.QueryFindNext();

			lCount++;
		}
	}

	queue.QueryEnd();
	queue.Close();

	// Uploads
	sQuery.Format(L"Q_lAction = %d", QUEUE_ACTION_UPLOAD);
	queue.QueryStart(sQuery);

	if (!queue.IsEOF())
	{
		long lRecordCount = queue.GetCount();

		long lCount = 0;
		long lCDsFound = 0;
		int iItemIndex = m_pUploadCDsDlg->m_ListCtrl.InsertItem(m_pUploadCDsDlg->m_ListCtrl.GetItemCount(), get_string(TEXT_UPLOAD_CDS));

		BOOL bRet = queue.QueryFindFirst();
		while (bRet)
		{
			CCD CD(m_pUploadDownloadDB);

			CString str;
			str.Format(get_string(TEXT_X_OF_X), lCount+1, lRecordCount);
			m_pUploadCDsDlg->m_ListCtrl.SetItem(iItemIndex, 1, LVIF_TEXT, str, 0, 0, 0, 0);

			str.Format(get_string(TEXT_SEND_CDS), lCount+1, lRecordCount);
			m_pUploadCDsDlg->m_sCurrentAction = str;
			m_pUploadCDsDlg->UpdateData(FALSE);
			m_pUploadCDsDlg->m_ProgressCtrl.StepIt();

			CD.GetRecordFromID(queue.m_dwIDCD);
			BOOL bError = FALSE;
			BOOL bSuccess = Upload(bError, NULL, 0);

			if (bSuccess)
				queue.Delete();
			else
			{
				m_pUploadCDsDlg->AddDetailsString(GetLastErrorMessage());
				bErrorsOccured = TRUE;
			}

			bRet = queue.QueryFindNext();

			lCount++;
		}
	}

	queue.QueryEnd();
	queue.Close();

	if (bErrorsOccured)
	{
		if (AfxMessageBox(get_string(TEXT_UPDOWNLOAD_ERRORS), MB_YESNO) == IDYES)
		{
			// Alle Datensätze aus der Warteschlange löschen!
			queue.DeleteAll();
		}
	}

	if (m_pUploadCDsDlg)
	{
		m_pUploadCDsDlg->m_CancelCtrl.SetWindowText(get_string(TEXT_CLOSE));
		m_pUploadCDsDlg->m_stcCurrentAction.SetWindowText(get_string(IDS_READY));
	}*/
}

// In der lokalen Kopie der CDArchiv-Datenbank suchen (CDArchive Snapshot)
BOOL CCDArchive::SearchCDInCDArchiveLocalFile(CDArchiveConfig^ Archive, Big3::Hitbase::DataBaseEngine::CD^ cd, BOOL% bCanceled, BOOL% bError)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	return Big3::Hitbase::DataBaseEngine::CDArchiveLocalFile::SearchCDInCDArchiveLocalFile(Archive->ArchiveName, cd);
}

// In der lokalen Kopie der CDDB-Datenbank suchen
BOOL CCDArchive::SearchCDInCDArchiveLocalFileCDDB(CDArchiveConfig^ Archive, Big3::Hitbase::DataBaseEngine::CD^ cd, BOOL% bCanceled, BOOL% bError)
{
	CString sFindString;
	unsigned long ulCDDBID;
	int found=0, iIndex;

	ulCDDBID = cd->GetCDDBDiscID();
//	int r=cd->NumberOfTracks;
	sFindString.Format(L"%x", ulCDDBID);

	// Nun suchen in Archive.m_sArchiveName 
	CString sCDDBData, sBuf;
	List <Big3::Hitbase::DataBaseEngine::CD^>^ CDArray = gcnew List <Big3::Hitbase::DataBaseEngine::CD^>();
	CArray <CString, CString&> MultiCD;
	Big3::Hitbase::DataBaseEngine::CD^ TempCD = gcnew Big3::Hitbase::DataBaseEngine::CD();
	TempCD->NumberOfTracks = cd->NumberOfTracks;
	//TempCD = *pCD;
	static char *Genre[] = {
		"rock", "classical", "misc", "soundtrack", "blues", "country", "data" , "folk", "jazz", "misc", "newage",
		"reggae",  "-"
	};

	int nPos=0;
	while (Genre[nPos][0] != '-')
	{
		sBuf = (CString)Archive->ArchiveName + "\\" + Genre[nPos];

		String^ cddbData = "";
		if (SearchCDInLocalCDDB(gcnew String(sBuf), gcnew String(sFindString), cddbData) == 1)
		{
			sCDDBData = cddbData;
			sBuf=sCDDBData;
			MultiCD.Add(sBuf);
			CDDBFillCDInfo(TempCD, sCDDBData, Archive);
			CDArray->Add(TempCD);
		}

		nPos++;
	}

	sCDDBData.Empty();

	if (iIndex > 0)
	{
		CMultipleCDsFound MultipleCDsFound(CDArray);

		if (MultipleCDsFound.DoModal() == IDCANCEL)
		{
			return FALSE;
		}

		sCDDBData = MultiCD.GetAt(MultipleCDsFound.m_nSelected);
	}
	else
	{
		if (iIndex == 0)
		{
			sCDDBData = MultiCD.GetAt(0);
		}
	}

	if (!sCDDBData.IsEmpty())
	{
		CDDBFillCDInfo(cd, sCDDBData, Archive);
		return TRUE;
	}

	return FALSE;
}

// Suchen in einer lokalen CDDB Kopie
// wegen fehlender "Links"-Unterstützung in den Entpackprogrammen - besser Windows Format
// Unix sollte grundsätzlich aber auch gehen
int CCDArchive::SearchCDInLocalCDDB(String^ sPath, String^ fileName, String^% sCDDBData)
{
	WIN32_FIND_DATA FindFileData;
	HANDLE hFind;
	CString sFindFile;
	CString sBuf;
	CString sLine;
	CStringA sLeftHex, sRightHex, sSearchHex;
	int nPos, nRet=0;
	unsigned long nIDStart, nIDEnd, nIDSearch;
	char *pLeft, *pRight, *pEnd, *pSearch;
	CString sCDDBFile;
	CString sFileName = fileName;

	sBuf.Empty();
	sFindFile = sPath + L"\\" + sFileName;
	//printf ("Target file is %s.\n", Archive.m_sArchiveName);
	hFind = FindFirstFile(sFindFile, &FindFileData);
	if (hFind != INVALID_HANDLE_VALUE) 
	{
		FindClose(hFind);
		sBuf = FindFileData.cFileName;
	}

	// UNIX Format - Datei vorhanden?
	if (sFileName.CompareNoCase(sBuf) == 0)
	{
		// Datei da... jetzt einlesen
		CString sRealFileName, sBuf2;

		CStdioFile cfCDDB;
		CString sFile2;
		sFile2 = sPath + "\\" + sBuf;
		if (!cfCDDB.Open(sFile2, CFile::modeRead | CFile::typeText))
		{
			return -2;
		}
		while (cfCDDB.ReadString(sLine)!=NULL)
		{
			nRet=1;
			// ID Found!
			// Jetzt ein paar Zeilen lesen...
			while(cfCDDB.ReadString(sLine) != NULL)
			{
				sCDDBData = sCDDBData + gcnew String(sLine) + "\n";
				if (sCDDBData->Length > 20000)
					break;
			}
		}

		cfCDDB.Close();

		return nRet;
	}

	// Nix gefunden - nun mal nach Windows Archiv-Format suchen
	sFindFile = sPath + L"\\" + (CString)sSearchHex + L"*";

	hFind = FindFirstFile(sFindFile, &FindFileData);

	if (hFind == INVALID_HANDLE_VALUE) 
	{
		//printf ("Invalid File Handle. GetLastError reports %d\n", 
		//   GetLastError ());
		return (-4);
	}

	nRet = 0;
	while (FindNextFile(hFind, &FindFileData))
	{
		sBuf = FindFileData.cFileName;

		if (sBuf.Compare(L".") == 0 || sBuf.Compare(L"..") == 0)
			continue;
		if ((nPos = sBuf.Find(L"to")) > 0)
		{
			// Windows Format... Suchen ob Hexwert im "Range" vorhanden
			sSearchHex = sFileName.Left(nPos);
			pSearch = (char *)sSearchHex.GetBuffer();
			nIDSearch = strtol(pSearch, &pEnd, 16);

			sLeftHex = sBuf.Left(nPos);
			sRightHex = sBuf.Mid(nPos + 2);
			pLeft = (char *)sLeftHex.GetBuffer();
			pRight = (char *)sRightHex.GetBuffer();
			nIDStart = strtol(pLeft, &pEnd, 16);
			nIDEnd = strtol(pRight, &pEnd, 16);

			if (nIDStart > nIDSearch && nIDEnd > nIDSearch)
				break;
			//if (nIDEnd > nIDSearch)
			//	break;

			// Hex-Wert im Range?
			if (nIDSearch >= nIDStart  && nIDSearch <= nIDEnd)
			{
				// Gefunden - jetzt die Datei einlesen
				HANDLE hReadCDDB;

				CString sRealFileName, sBuf2;
				//CStdioFile cfCDDB;
				CStringA sFile2;
				CString sComplete;
				sFile2 = sPath + "\\" + sBuf;


				hReadCDDB = CreateFileA(sFile2,    // file to open
					GENERIC_READ,          // open for reading
					FILE_SHARE_READ,       // share for reading
					NULL,                  // default security
					OPEN_EXISTING,         // existing file only
					FILE_ATTRIBUTE_NORMAL, // normal file
					NULL);                 // no attr. template

				if (hReadCDDB == INVALID_HANDLE_VALUE) 
				{ 
					FindClose(hFind);

					return nRet;
				}

				// Fette Datei lesen... egal wie groß
				// Speicher ist durch nix zu ersetzen - außer durch Speicher! :-)
				char *inBuffer;
				DWORD dwFileSize, nSize, nBytesRead;
				BOOL bResult;
				nSize = GetFileSize(hReadCDDB, &dwFileSize);
				inBuffer = (char *)malloc(nSize + 1000);
				if (inBuffer == NULL)
					return 0;

				bResult = ReadFile(hReadCDDB, inBuffer, nSize, &nBytesRead, NULL);

				if (bResult &&  nBytesRead > 0)
				{
					CStringA sName;

					sName = sFileName;
					char *pfind = StrStrIA(inBuffer, sName);
					if (pfind != NULL)
					{
						char *pfind2;
						strcat (inBuffer, "#FILENAME=");
						pfind2 = StrStrIA(pfind, "#FILENAME=");
						if (pfind2 != NULL)
						{
							int nlen = pfind2 - pfind;
							while (StrCmpNIA(pfind, "#FILENAME=", 10) != 0)
								pfind--;
							nRet = 1;
							CStringA sBuf3;
							memcpy (sBuf3.GetBuffer(pfind2 - pfind + 1), pfind, pfind2 - pfind);
							nlen = sBuf3.GetLength();
							//AfxMessageBox ((CString)sBuf3);
							sCDDBData = gcnew String((CString)sBuf3);
						}
					}
					//AfxMessageBox(sComplete.Left(8000));
					//int r = sComplete.GetLength();
					// CDDBID in Datei vorhanden?
					//if ((nPos = sComplete.Find(sFileName)) > 0)
					{
						CString sCDfound;

						//sComplete += L"#FILENAME=";
						//int nPosEnd = sComplete.Find(L"#FILENAME=", nPos);
						//sCDfound = sLine.Mid(nPos,nPosEnd - nPos); 	
					}
				}
				CloseHandle(hReadCDDB);
			}
		}
	}

	FindClose(hFind);

	return nRet;
}