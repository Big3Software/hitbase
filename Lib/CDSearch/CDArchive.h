// CDArchive.h: interface for the CCDArchive class.
//
//////////////////////////////////////////////////////////////////////

#pragma once

#include "CDDBSock.h"
#include "WaitAnimatedDlg.h"
#define INTERNET_FILE_VERSION    1          // Aktuelle Fileversion der Internetdatei

// Mögliche Ergebnisse durch eine Upload-Aktion
public enum class CDArchivUploadResults
{
	UPLOAD_ARCHIV_CD_ERROR=-2,        // Fehler beim Uploaden (Fehlende oder fehlerhafte Einträge in den CD-Daten)
	UPLOAD_ARCHIV_ERROR=-1,           // Fehler beim Uploaden
	UPLOAD_ARCHIV_CD_EXISTS=0,        // CD wurde nicht ins Archiv übertragen, da sie schon vorhanden ist.
	UPLOAD_ARCHIV_NEW_CD=1            // CD wurde ins Archiv übertragen (noch nicht vorhanden)
};

public enum class CDArchivUploadOptions
{
	UPLOAD_OPTIONS_VERBOSE=1,        // Während des Uploads mit mir reden
	UPLOAD_OPTIONS_CD_UPDATE=2        // Falls CD schon vorhanden, abfragen ob überschreiben
};

#define UPLOADTYPE_HITBASE 1
#define UPLOADTYPE_CDDB 2

using namespace System;
using namespace Big3::Hitbase::Configuration;

class CUploadDownloadCDsDlg;


// CCDArchiveApp
//

class CCDArchiveApp : public CWinApp
{
public:
	CCDArchiveApp();

// Overrides
public:
	virtual BOOL InitInstance();

	DECLARE_MESSAGE_MAP()
};

public ref class CCDArchive  
{
public:
	CCDArchive();

protected:
	void DisplayErrorMessage(String^ message);

	bool SearchCDInCDArchiveFile(CDArchiveConfig^ Archive, Big3::Hitbase::DataBaseEngine::CD^ cd, BOOL% bCanceled, BOOL% bError);
	BOOL SearchCDInCDDB(CDArchiveConfig^ Archive, Big3::Hitbase::DataBaseEngine::CD^ cd, BOOL% bCanceled, BOOL% bError);
	BOOL SearchCDInCDArchiveLocalFileCDDB(CDArchiveConfig^ Archive, Big3::Hitbase::DataBaseEngine::CD^ cd, BOOL% bCanceled, BOOL% bError);
	int SearchCDInLocalCDDB(String^ sPath, String^ sFileName, String^% sCDDBData);

	CUploadDownloadCDsDlg* m_pUploadCDsDlg;
	int m_nCurrentLine;
	bool m_bVerbose;
	bool m_bCDUpdate;
	String^ m_sLastErrorMessage;
	Big3::Hitbase::DataBaseEngine::DataBase^ UploadDownloadDataBase;
	String^ m_sUploadCDIdentityCDDB;

private:
	BOOL UploadCDInCDArchiveFile(CDArchiveConfig^ Archive, Big3::Hitbase::DataBaseEngine::CD^ cd, BOOL% bCanceled);
	BOOL UploadCDInInternetBig3(CDArchiveConfig^ Archive, Big3::Hitbase::DataBaseEngine::CD^ cd, BOOL% bCanceled, int% iResult);
	BOOL UploadCDInCDDB(CDArchiveConfig^ Archive, Big3::Hitbase::DataBaseEngine::CD^ cd, BOOL% bCanceled, int% iResult);

	BOOL UploadCD(CHttpConnection * pHttpConnection, Big3::Hitbase::DataBaseEngine::CD^ cd, int nType, int% iResult);
	BOOL ReadCDInfoFromString(const CString& sContent, Big3::Hitbase::DataBaseEngine::CD^ cd, BOOL% bError);
	BOOL ReadStatisticsFile(CHttpFile & file, int% nNumberOfCDs, String^% sLastModified);
	BOOL GetHitbaseInternetStatistics(String^ sInternetServer, int% sNumberOfCDs, String^% sDate);
	BOOL GetHitbaseFileStatistics(String^ sInternetServer, int% nNumberOfCDs, String^% sDate);
	BOOL GetCDDBStatistics(CDArchiveConfig^ CDArchive, int% nNumberOfCDs, String^% sDate);
	BOOL GetCDArchivLocalStatistics(String^ sFilename, int% nNumberOfCDs, String^% sDate);
	BOOL CDDBFillCDInfo(Big3::Hitbase::DataBaseEngine::CD^ cd, CString& CDDBData, CDArchiveConfig^ Archive);
	void RemoveSpecialChars (String^% line);
	int CDDBGetReturnCode(CString ReturnString);
	BOOL CDDBGetKategorieCDID(CString ReturnString, CString & Kategorie, CString & CDID);
	BOOL CheckCDForUpload(Big3::Hitbase::DataBaseEngine::CD^ cd, CString& sErrorMessage);

	CString GetCDInfoAsString(Big3::Hitbase::DataBaseEngine::CD^ cd);
	CString DoUploadCDToCDArchive(CHttpConnection* pHttpConnection, Big3::Hitbase::DataBaseEngine::CD^ cd, int nType, BOOL bForceUpdate);

	Big3::Hitbase::Miscellaneous::FormWaitAnimated^ FormWaitAnimated;

public:
	virtual void UploadDownload(Big3::Hitbase::DataBaseEngine::DataBase^ db);
	virtual void StartUploadDownload();
	virtual CString GetLastErrorMessage();
	BOOL Search(Big3::Hitbase::DataBaseEngine::CD^ cd, BOOL bAutoSearch, BOOL% bError, BOOL bVerbose/* = TRUE */, IntPtr pParent /* = NULL */);
	BOOL Upload(Big3::Hitbase::DataBaseEngine::CD^ cd, int% iResult, IntPtr^ pParentWnd, DWORD dwUploadOptions);

	BOOL SearchCDInInternetBig3(CDArchiveConfig^ Archive, Big3::Hitbase::DataBaseEngine::CD^ cd, BOOL% bCanceled, BOOL% bError);
	BOOL DownloadCD(IntPtr^ pHttpConnection, Big3::Hitbase::DataBaseEngine::CD^ cd, BOOL% bCanceled, BOOL% bError, BOOL bBackground/*= TRUE*/);
	BOOL SearchCDInCDArchiveLocalFile(CDArchiveConfig^ Archive, Big3::Hitbase::DataBaseEngine::CD^ cd, BOOL% bCanceled, BOOL% bError);

	static String^ GetCDDBQueryString(Big3::Hitbase::DataBaseEngine::CD^ cd);

	String^ GetBrowseableOnlineArchiv();
	String^ GetSearchableArchivName();
	BOOL IsUploadableArchivAvailable();
	BOOL IsSearchableArchivAvailable();
	BOOL IsArchiveActive();
	CDArchiveType GetSearchableArchivType();
	BOOL IsSearchableArchivAutoSearch();
	bool IsArchiveBrowseable(CDArchiveConfig^ archive);

	BOOL GetStatistics(int nIndex, int% nNumberOfCDs, String^% sDate);

	String^ m_sLastDetailMessage;      // Letzte (Fehler-)Meldung

	BOOL m_bTransferCanceled;          // Down- oder Upload abgebrochen
};

//#include "../hitmisc/config.h"

ref class CDArchiveLog
{
public:
	static void AddLog(System::String^ str)
	{
		try
		{
			/*int bWriteLog = CConfig::ReadGlobalRegistryKeyInt(L"Log", 0);
			if (bWriteLog > 0)
				System::IO::File::AppendAllText(L"c:\\cdarchive.log", str + "\r\n");*/
		}
		catch (System::Exception^ e)
		{
			e;
		}
	}
};

