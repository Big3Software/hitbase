// hitdb.cpp : implementation file
//

#include "stdafx.h"
#include "HitDB.h"
#include "DataBase.h"
#include "DupRecordDlg.h"
#include "DBQuery.h"
#include "ChooseFieldsDlg.h"
#include "../hitmisc/HitbaseWinAppBase.h"
#include "../../app/hitbase/resource.h"
#include "../gridctrl/hitgrid.h"
#include ".\database.h"

#ifdef _DEBUG
#undef THIS_FILE
static char BASED_CODE THIS_FILE[] = __FILE__;
#endif

using namespace System;
using namespace System::Windows::Forms;

/////////////////////////////////////////////////////////////////////////////
// CDataBase

CDataBase::CDataBase()
{
	m_ProgressCtrl = NULL;
	m_ProgressUpdateStopped = FALSE;
	m_StopSearchProcess = FALSE;
	m_bBackground = FALSE;
	m_ProgressedRecords = 0;
	
	m_nQueryType = -1;

	m_CurrentSelection = NULL;
	m_nNumberOfCategories = -1;
	m_nNumberOfMedia = -1;
	m_bReadOnly = FALSE;
	m_iDatabaseVersion = 0;
}

CDataBase::~CDataBase()
{
   Close();
}

/*
 * Eröffnet die angegebene Datenbank.
 */

BOOL CDataBase::Open(const CString szDBFileName, BOOL bReadOnly /* = FALSE */, BOOL bDisplayMessages /* = FALSE */)
{
	BOOL bOpenReadOnly = FALSE;
	BOOL bTabelleVorhanden = TRUE;
	CString sTabelle = "VerlieheneCDs";

	CString sDBFilename = szDBFileName;

	if (m_db.IsOpen())
		m_db.Close();

	if (szDBFileName.IsEmpty())
	{
		sDBFilename = m_sDBFilename;
		ASSERT(!sDBFilename.IsEmpty());
	}

	try
	{
		// Wenn ich die Datenbank read-only öffne, so muß sie auch exklusiv geöffnet
		// werden, da die Jet-Engine sie sonst versucht, zu locken!! (Warum???)
		m_db.Open(sDBFilename, bReadOnly, bReadOnly);
//		m_db.Open("", bReadOnly, bReadOnly);     // Für ODBC!!
		m_bReadOnly = bReadOnly;
	}
	catch(CDaoException* e)
	{
		if (e->m_pErrorInfo)
		{

//			if (e->m_pErrorInfo->m_lErrorCode == E_DAO_FileAccessDenied)   // Datei kann nicht exklusiv geöffnet werden
			if (e->m_scode == E_DAO_FileAccessDenied)   // Datei kann nicht exklusiv geöffnet werden
			{
				if (bDisplayMessages)
				{
					CString str;
					str.Format(get_string(IDS_OPEN_READONLY), sDBFilename); 
					AfxMessageBox(str, MB_OK|MB_ICONINFORMATION);
				}
				bOpenReadOnly = TRUE;
			}
			else
			{
				ErrorBox(e->m_pErrorInfo->m_strDescription);
				e->Delete();
				return FALSE;
			}
		}
		else
		{
			ErrorBox(get_string(IDS_OPEN_FAILED), sDBFilename, e->m_nAfxDaoError);

			e->Delete();
			return FALSE;
		}
	}

	if (bOpenReadOnly)
	{
		try
		{
			m_db.Open(sDBFilename, TRUE, TRUE);
		}
		catch (CDaoException* e)
		{
			if (e->m_pErrorInfo)
				ErrorBox(e->m_pErrorInfo->m_strDescription);
			else
				ErrorBox(get_string(IDS_OPEN_FAILED), sDBFilename, e->m_nAfxDaoError);

			e->Delete();
			return FALSE;
		}

		m_bReadOnly = TRUE;
	}

	if (!m_bReadOnly)
	{
		// Erst mal schauen, ob die Tabelle da ist.
		// Zu beachten ist, daß diese Art von Tabellenmanipulation nur über DAO
		// möglich ist. ODBC furzt da voll in die Pulle.

		m_bTabelleVerliehenVorh = TRUE;

		CDaoTableDef table(&m_db);
		try
		{
			table.Open(sTabelle);
		}

		catch(CDaoException* e)
		{
			// Ei verbischt. Die Tabelle is jo gar nich do
			bTabelleVorhanden = FALSE;
			m_bTabelleVerliehenVorh = FALSE;

			e->Delete();
		}

		if (bTabelleVorhanden == FALSE)
		{
			table.Close();
			try
			{
				CDaoIndexInfo indexinfo;
				CDaoIndexFieldInfo IndexFieldInfo;
				CDaoFieldInfo fieldInfo;

				// Jetzt erst mal die nackte Tabelle anlegen
				table.Create(sTabelle);

				// Und nun das Feld IDCD anlegen
				fieldInfo.m_strName = "IDCD";
				fieldInfo.m_nType = dbLong;
				fieldInfo.m_nOrdinalPosition = 1;
				fieldInfo.m_bRequired = TRUE;
				fieldInfo.m_bAllowZeroLength = FALSE;
				fieldInfo.m_lAttributes = 0;

				table.CreateField(fieldInfo);

				fieldInfo.m_strName = "Kommentar";
				fieldInfo.m_nType = dbText;
				fieldInfo.m_lSize = 255;
				fieldInfo.m_nOrdinalPosition = 5;
				fieldInfo.m_bRequired = FALSE;
				fieldInfo.m_bAllowZeroLength = TRUE;
				fieldInfo.m_lAttributes = 0;
				table.CreateField(fieldInfo);
				
				// Jetzt wird der Index für das Feld IDCD angelegt
				IndexFieldInfo.m_strName = "IDCD";
				IndexFieldInfo.m_bDescending = FALSE;
				indexinfo.m_strName = "IDCD";
				indexinfo.m_pFieldInfos = &IndexFieldInfo;
				indexinfo.m_nFields = 1;
				indexinfo.m_bPrimary = TRUE;
				indexinfo.m_bUnique = TRUE;
				indexinfo.m_bRequired = TRUE;
				indexinfo.m_bIgnoreNulls = FALSE;
				indexinfo.m_lDistinctCount = 1;
				table.CreateIndex(indexinfo);

				// Die restlichen Felder auch noch anlegen
				table.CreateField(L"VerliehenAm", dbDate, 0);
				table.CreateField(L"VerliehenAn", dbText, 30);
				table.CreateField(L"RueckgabeTermin", dbDate, 0);

				table.Append();
				m_bTabelleVerliehenVorh = TRUE;
			}

			catch(CException* e)
			{
				CString str;
				str.LoadString(IDS_ERROR_CREATE_TABLE);
				AfxMessageBox(str + " " + sTabelle);
				e->Delete();
			}
		}
	}
	
	m_ProgressUpdateStopped = FALSE;
	m_StopSearchProcess = FALSE;

	// JUS 000820: Die Versionsnummer der Datenbank jetzt immer laden!!
	ReadDatabaseVersion();

	return TRUE;
}

/*
 * Schließt die Datenbank.
 */

BOOL CDataBase::Close()
{
	m_StopSearchProcess = FALSE;
	
	if (!IsDataBaseOpened())
		return FALSE;
	
	m_db.Close();

	m_ProgressCtrl = NULL;
	m_ProgressUpdateStopped = FALSE;
	m_StopSearchProcess = FALSE;
	
	m_CurrentSelection = NULL;
	m_bReadOnly = TRUE;
	
	return TRUE;
}

/*
 * Legt einen neuen Katalog an.
 * Achtung: Ein vorhandener wird ohne Warnung überschrieben!
 */

BOOL CDataBase::Create(const CString & pszFileName, const CString & sTemplate)
{
   return CopyFile(sTemplate, pszFileName, FALSE);
}

/*
 * Definiert eine Select-Abfrage. Die nächste FindFirst, FindNext-Kombination
 * benutzt dann diese Selektion und Sortierung.
 */

int CDataBase::Select(CSelection *Selection)
{
   m_CurrentSelection = Selection;

   return TRUE;
}

/*
 * Setzt ein Progress-Control, der bei FindFirst und FindNext benutzt werden soll.
 */

int CDataBase::SetProgressCtrl(CProgressCtrl *ProgressCtrl, BOOL bBackground/*=FALSE*/)
{
	m_ProgressCtrl = ProgressCtrl;
	m_bBackground = bBackground;      // Hintergrund-Suche JA/NEIN
	
	return TRUE;
}

/*
 * Liefert den aktuellen Progress-Control zurück, NULL = keine ProgressCtrl.
 */

CProgressCtrl *CDataBase::GetProgressCtrl()
{
	return m_ProgressCtrl;
}

/*
 * Progress-Bar beim Lesen weiter updaten oder kurz anhalten.
 * Benutzen, um z.B. kurz in einem anderen Key zu lesen.
 */

void CDataBase::SetProgressUpdate(BOOL update)
{
	m_ProgressUpdateStopped = !update;
}

// Diese Funktion ist für multi-threading nötig, um eine laufende Suche
// "vernünftig" abzubrechen.
void CDataBase::StopSearchProcess()
{
	m_StopSearchProcess = TRUE;
}

// Für Hintergrund-Check
void CDataBase::CheckAbortProc()
{
	if (m_bBackground)
	{
		MSG msg;
		
		while (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE))
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
	}
}

BOOL CDataBase::IsDataBaseOpened()
{
	return m_db.IsOpen();
}

BOOL CDataBase::ReadAllCategories()
{
	int i=0;
	CDaoRecordset RecordSet(&m_db);
	COleVariant var;
	
	if (!IsDataBaseOpened())
		return FALSE;
	
	RecordSet.Open(dbOpenDynaset, L"SELECT * FROM Kategorie ORDER BY wOrder", dbSeeChanges);
	
	if (RecordSet.IsEOF())
	{
		m_nNumberOfCategories = 0;
		return TRUE;
	}
	
	RecordSet.MoveFirst();
	
	while (!RecordSet.IsEOF())
	{
		var = RecordSet.GetFieldValue(0);
		m_IDCategories[i] = var.lVal;
		
		var = RecordSet.GetFieldValue(1);
		m_Categories[i++] = var.bstrVal;
		
		RecordSet.MoveNext();
	}
	
	m_nNumberOfCategories = i;
	return TRUE;
}

BOOL CDataBase::ReadAllCodes()
{
	int index;
	CDaoRecordset RecordSet(&m_db);
	COleVariant var;
	CWaitCursor wait;
	
	if (!IsDataBaseOpened())
		return FALSE;
	
	RecordSet.Open(dbOpenDynaset, L"SELECT * FROM Kennzeichen", dbSeeChanges);
	
	if (RecordSet.IsEOF())
	{
		return TRUE;
	}
	
	RecordSet.MoveFirst();
	
	while (!RecordSet.IsEOF())
	{
		var = RecordSet.GetFieldValue(0);
		index = var.pbVal[0]-65;
		
		var = RecordSet.GetFieldValue(1);
		m_Codes[index] = var.bstrVal;
		
		RecordSet.MoveNext();
	}
	
	return TRUE;
}

BOOL CDataBase::WriteAllCodes()
{
	int i;
	CString sqlStr;
	CWaitCursor wait;
	
	if (m_bReadOnly)
		return FALSE;
	
	m_db.m_pWorkspace->BeginTrans();
	
	if (!ExecuteSQL("DELETE * FROM Kennzeichen"))
	{
		m_db.m_pWorkspace->Rollback();
		return FALSE;
	}
	
	for (i=0;i<MAX_CODES;i++)
	{
		if (m_Codes[i] != "")
		{
			CString sCode = m_Codes[i];
			CMisc::SqlPrepare(sCode);

			sqlStr.Format(L"INSERT INTO Kennzeichen (cBuchstabe, szBedeutung) VALUES (\"%c\", \"%s\")", 'A'+i, sCode);
			if (!ExecuteSQL(sqlStr))
			{
				m_db.m_pWorkspace->Rollback();
				return FALSE;
			}
		}
	}
	
	m_db.m_pWorkspace->CommitTrans();
	
	return TRUE;
}

BOOL CDataBase::ReadAllMedia()
{
   CDaoRecordset RecordSet(&m_db);
   COleVariant var;
   int nCount = 0;

   if (!IsDataBaseOpened())
      return FALSE;

   RecordSet.Open(dbOpenDynaset, L"SELECT * FROM Medium ORDER BY wOrder", dbSeeChanges);

   if (RecordSet.IsEOF())
   {
      m_nNumberOfMedia = 0;
      return TRUE;
   }

   RecordSet.MoveFirst();

   while (!RecordSet.IsEOF())
   {
      var = RecordSet.GetFieldValue(0);
      m_IDMedium[nCount] = var.lVal;
      var = RecordSet.GetFieldValue(1);
      m_Medium[nCount++] = var.bstrVal;
      RecordSet.MoveNext();
   }

   RecordSet.Close();

   m_nNumberOfMedia = nCount;

   return TRUE;
}

// szArtist-> Der komplette Name eines Artisten (z.B. Bon Jovi)
// Returnwert: long (die ID aus der Tabelle Artist)

long CDataBase::GetIDFromArtist(const CString& szArtist, BOOL* bAdded /* = NULL */, BOOL bDoNotAdd /* = FALSE */)
{
	CString SqlFindArtist;
	CDaoRecordset RecordSet(&m_db);
	COleVariant var;
	CString sArtistSQL = szArtist; 

	if (bAdded != NULL)
		*bAdded = FALSE;

	if (!IsDataBaseOpened())
		return 0;

	// jus 07.05.97: Auch leere Artisten eintragen!
	//   if (szArtist.IsEmpty())    // Keinen Interpreten eingegeben!
	//      return 0L;              

//	sArtistSQL = CMisc::SqlPrepareString(szArtist);

//	SqlFindArtist = "SELECT * from Artist where Artist.szArtistName = '" + sArtistSQL + "'";

	CMisc::SqlPrepare(sArtistSQL);
	SqlFindArtist = "SELECT * from Artist where Artist.szArtistName = \"" + sArtistSQL + "\"";


	try
	{
		RecordSet.Open(dbOpenDynaset, SqlFindArtist, dbSeeChanges);
	}
	catch (CException* e)
	{
		e->ReportError();
 	    AfxMessageBox(SqlFindArtist);

		e->Delete();
		return 0;
	}

	if (RecordSet.IsEOF())      // Jetzt muß der Artist hinzugefügt werden
	{
		if (!bDoNotAdd)
		{
			RecordSet.AddNew();

			var.SetString(szArtist, VT_BSTRT);
			RecordSet.SetFieldValue(L"szArtistName", var);
			// JUS 990102: Der Sortierschlüssel ist zunächst gleich!
			RecordSet.SetFieldValue(L"sSortKey", var);
			RecordSet.GetFieldValue(L"IDArtist", var);

			RecordSet.Update();
			if (bAdded != NULL)
				*bAdded = TRUE;
		}
		else
		{
			var.lVal = 0;
		}
	}
	else
		RecordSet.GetFieldValue(L"IDArtist", var);

	RecordSet.Close();

	return var.lVal;
}


BOOL CDataBase::FindArtist(CString szArtistPrefix, CString &szArtistName)
{
   CDaoRecordset RecordSet(&m_db);
   COleVariant var;

   if (!IsDataBaseOpened())
      return FALSE;

   if (szArtistPrefix.FindOneOf(L"*?#[]!") >= 0)
      return FALSE;

   CMisc::SqlPrepare(szArtistPrefix);

   RecordSet.Open(dbOpenDynaset, "SELECT TOP 1 szArtistName FROM Artist WHERE "
                                 "szArtistName like \"" + szArtistPrefix + "*\" order by szArtistName", dbSeeChanges);

   if (!RecordSet.IsEOF())
   {
      var = RecordSet.GetFieldValue(0);
      szArtistName = var.bstrVal;
      RecordSet.Close();

      return TRUE;
   }

   RecordSet.Close();

   return FALSE;
}

BOOL CDataBase::FindCDTitle(CString sPrefix, CString &sCDTitle)
{
   CDaoRecordset RecordSet(&m_db);
   COleVariant var;

   if (!IsDataBaseOpened())
      return FALSE;

   if (sPrefix.FindOneOf(L"*?#[]!") >= 0)
      return FALSE;

   CMisc::SqlPrepare(sPrefix);

   RecordSet.Open(dbOpenDynaset, "SELECT TOP 1 szTitel FROM CD WHERE "
                                 "szTitel like \"" + sPrefix + "*\" order by szTitel", dbSeeChanges);

   if (!RecordSet.IsEOF())
   {
      var = RecordSet.GetFieldValue(0);
      sCDTitle = var.bstrVal;
      RecordSet.Close();

      return TRUE;
   }

   RecordSet.Close();

   return FALSE;
}

BOOL CDataBase::FindTrackname(CString sPrefix, CString &sTrackname)
{
   CDaoRecordset RecordSet(&m_db);
   COleVariant var;

   if (!IsDataBaseOpened())
      return FALSE;

   if (sPrefix.FindOneOf(L"*?#[]!") >= 0)
      return FALSE;

   CMisc::SqlPrepare(sPrefix);

   RecordSet.Open(dbOpenDynaset, "SELECT TOP 1 szTitel FROM Lied WHERE "
                                 "szTitel like \"" + sPrefix + "*\" order by szTitel", dbSeeChanges);

   if (!RecordSet.IsEOF())
   {
      var = RecordSet.GetFieldValue(0);
      sTrackname = var.bstrVal;
      RecordSet.Close();

      return TRUE;
   }

   RecordSet.Close();

   return FALSE;
}

long CDataBase::GetIDFromCDSet(CString szCDSetName, BOOL *bAdded /* = NULL */)
{
	CDaoRecordset RecordSet(&m_db);
	COleVariant var;
	CString szSQLGetID;
	CString sSQLCDSet = szCDSetName;

	// JUS 981227: Hatte ich vergessen!!
	CMisc::SqlPrepare(sSQLCDSet);

	if (bAdded != NULL)
		*bAdded = FALSE;
	
	if (!IsDataBaseOpened())
		return FALSE;
	
	if (szCDSetName.IsEmpty())
		return 0L;
	
	szSQLGetID = "SELECT * FROM CDSet WHERE szCDSetName = \"" + sSQLCDSet + "\"";
	
	RecordSet.Open(dbOpenDynaset, szSQLGetID, dbSeeChanges);
	
	if (RecordSet.IsEOF())
	{
		RecordSet.AddNew();
		
		var.SetString(szCDSetName, VT_BSTRT);
		RecordSet.SetFieldValue(L"szCDSetName", var);
		RecordSet.GetFieldValue(L"IDCDSet", var);
		
		RecordSet.Update();
		if (bAdded != NULL)
			*bAdded = TRUE;
	}
	else
		RecordSet.GetFieldValue(L"IDCDSet", var);
	
	RecordSet.Close();
	return var.lVal;
}

// Wenn bSimple = TRUE ist, dann wird bei jedem String nach *xyz* gesucht, dass
// heißt, die Sternchen werden automatisch davor und dahinter gesetzt!
CString CDataBase::GetWhereString(const CSelection *Selection, BOOL bSimple /* = FALSE */, BOOL bNoFirstAnd /* = FALSE */)
{
	CString str, buf;
	int IDKategorie, IDMedium;
	CString sWildcard;
	CString str1;

	// Wenn ein erweiterter Filter definiert ist, dann nehmen wir den.
	if (!Selection->IsFilterEmpty())
	{
		str = GetFilterWhereString(*Selection);
		return str;
	}

	if (bSimple || !Selection->m_bExactMatch)
		sWildcard = "*";

	ASSERT(m_nNumberOfCategories >= 0);

	if (!Selection)
		return "";

	if (Selection->m_wCDType == 1)     // Nur CDs mit einem Interpreten
		str += "AND CD.bCDSampler = FALSE ";

	if (Selection->m_wCDType == 2)     // Nur CDs mit mehreren Interpreten
		str += "AND CD.bCDSampler = TRUE ";

	if (Selection->m_szCDArtist != "")
		str += "AND Artist.szArtistName like \"" + sWildcard + CMisc::GetSqlString(Selection->m_szCDArtist) + sWildcard + "\" ";

	if (Selection->m_szCDTitle != "")
		str += "AND CD.szTitel like \"" + sWildcard + CMisc::GetSqlString(Selection->m_szCDTitle) + sWildcard + "\" ";

	if (Selection->m_dwTotalLengthFrom > 0)
	{
		buf.Format(L"%ld", Selection->m_dwTotalLengthFrom);
		str += "AND CD.dwGesamtLaenge >= " + buf + " ";
	}

	if (Selection->m_dwTotalLengthTo > 0)
	{                           // Achtung: + 999, wegen voller Sekunde!
		buf.Format(L"%ld", Selection->m_dwTotalLengthTo+999);
		str += "AND CD.dwGesamtLaenge <= " + buf + " ";
	}

	if (Selection->m_wNumberOfTracksFrom > 0)
	{
		buf.Format(L"%d", Selection->m_wNumberOfTracksFrom);
		str += "AND CD.cAnzahlLieder >= " + buf + " ";
	}

	if (Selection->m_wNumberOfTracksTo > 0)
	{
		buf.Format(L"%d", Selection->m_wNumberOfTracksTo);
		str += "AND CD.cAnzahlLieder <= " + buf + " ";
	}

	IDKategorie = 0;
	if (Selection->m_szCategory != "")
		IDKategorie = GetIDFromCategory(Selection->m_szCategory);

	if (IDKategorie > 0)
	{
		buf.Format(L"%d", IDKategorie);
		str += "AND CD.IDKategorie = " + buf + " ";
	}

	IDMedium = 0;
	if (Selection->m_szMedium != "")
		IDMedium = GetIDFromMedium(Selection->m_szMedium);

	if (IDMedium > 0)
	{
		buf.Format(L"%d", IDMedium);
		str += "AND CD.IDMedium = " + buf + " ";
	}

	if (Big3::Hitbase::Configuration::Settings::Current->SortArchiveNumberNumeric)
	{
		if (_wtoi(Selection->m_sArchivNummerFrom) > 0)
		{
			buf.Format(L"%d", _wtoi(Selection->m_sArchivNummerFrom));
			str += "AND IIF(IsNumeric(CD.szArchivnummer),CLng(CD.szArchivnummer),0) >= " + buf + " ";
		}
		if (_wtoi(Selection->m_sArchivNummerTo) > 0)
		{
			buf.Format(L"%d", _wtoi(Selection->m_sArchivNummerTo));
			str += "AND IIF(IsNumeric(CD.szArchivnummer),CLng(CD.szArchivnummer),0) <= " + buf + " ";
		}
	}
	else
	{
		if (!Selection->m_sArchivNummerFrom.IsEmpty())
		{
			str += "AND CD.szArchivNummer >= \"" + Selection->m_sArchivNummerFrom + "\" ";
		}
		if (!Selection->m_sArchivNummerTo.IsEmpty())
		{
			str += "AND CD.szArchivNummer <= \"" + Selection->m_sArchivNummerTo + "\" ";
		}
	}

	if (Selection->m_szCodes != "")
		str += "AND (" + GetWhereCodesString(Selection->m_szCodes, FALSE) + ") ";

	if (Selection->m_szCDComment != "")
		str += "AND CD.szKommentar like \"" + sWildcard + CMisc::GetSqlString(Selection->m_szCDComment) + sWildcard + "\" ";

	if (Selection->m_sCopyright != "")
		str += "AND CD.C_Copyright like \"" + sWildcard + CMisc::GetSqlString(Selection->m_sCopyright) + sWildcard + "\" ";

	if (Selection->m_lYearRecordedFrom > 0)
	{
		buf.Format(L"%d", Selection->m_lYearRecordedFrom);
		str += "AND CD.C_YearRecorded >= " + buf + " ";
	}

	if (Selection->m_lYearRecordedTo > 0)
	{
		buf.Format(L"%d", Selection->m_lYearRecordedTo);
		str += "AND CD.C_YearRecorded <= " + buf + " ";
	}

	// Version 10
	if (Selection->m_iOriginalCD == 1)     // Nur Original-CDs
		str += "AND CD.C_Original = TRUE ";

	if (Selection->m_iOriginalCD == 2)     // Nur "Eigenkreationen"
		str += "AND CD.C_Original = FALSE ";

	if (!Selection->m_sLabel.IsEmpty())
		str += "AND CD.C_Label like \"" + sWildcard + CMisc::GetSqlString(Selection->m_sLabel) + sWildcard + "\" ";

	if (!Selection->m_sURL.IsEmpty())
		str += "AND CD.C_URL like \"" + sWildcard + CMisc::GetSqlString(Selection->m_sURL) + sWildcard + "\" ";

	if (!Selection->m_sUPC.IsEmpty())
		str += "AND CD.C_UPC like \"" + sWildcard + CMisc::GetSqlString(Selection->m_sUPC) + sWildcard + "\" ";

	if (Selection->m_lRatingFrom > 0)
	{
		buf.Format(L"%d", Selection->m_lRatingFrom);
		str += "AND CD.C_Rating >= " + buf + " ";
	}

	if (Selection->m_lRatingTo > 0)
	{
		buf.Format(L"%d", Selection->m_lRatingTo);
		str += "AND CD.C_Rating <= " + buf + " ";
	}

	if (Selection->m_lPriceFrom > 0)
	{
		buf.Format(L"%d", Selection->m_lPriceFrom);
		str += "AND CD.C_Price >= " + buf + " ";
	}

	if (Selection->m_lPriceTo > 0)
	{
		buf.Format(L"%d", Selection->m_lPriceTo);
		str += "AND CD.C_Price <= " + buf + " ";
	}

	if (!Selection->m_sFrontCover.IsEmpty())
		str += "AND CD.szPfadBitmap like \"" + sWildcard + CMisc::GetSqlString(Selection->m_sFrontCover) + sWildcard + "\" ";

	if (!Selection->m_sBackCover.IsEmpty())
		str += "AND CD.C_BackCoverBitmap like \"" + sWildcard + CMisc::GetSqlString(Selection->m_sBackCover) + sWildcard + "\" ";

	if (!Selection->m_sCDLabel.IsEmpty())
		str += "AND CD.C_CDLabelBitmap like \"" + sWildcard + CMisc::GetSqlString(Selection->m_sCDLabel) + sWildcard + "\" ";

	for (int i=0;i<MAX_USER_FIELDS;i++)
	{
		if (Selection->m_szCDUser[i] != "")
		{
			str1.Format(L"AND CD.szFeld%d like \"%s%s%s\" ", i+1, sWildcard, CMisc::GetSqlString(Selection->m_szCDUser[i]), sWildcard);
			str += str1;
		}
	}

	// Version 11
	if (!Selection->m_sLanguage.IsEmpty())
		str += "AND CD.C_Language like \"" + sWildcard + CMisc::GetSqlString(Selection->m_sLanguage) + sWildcard + "\" ";

	if (!Selection->m_sLocation.IsEmpty())
		str += "AND CD.C_Location like \"" + sWildcard + CMisc::GetSqlString(Selection->m_sLocation) + sWildcard + "\" ";

	if (!Selection->m_sComposer.IsEmpty())
		str += "AND Composer.szArtistName like \"" + sWildcard + CMisc::GetSqlString(Selection->m_sComposer) + sWildcard + "\" ";

	if (!Selection->m_sParticipant.IsEmpty())
		str += "AND (CD.IDCD in (SELECT P_IDCD FROM Participant INNER JOIN Artist ON Artist.IDArtist = Participant.P_IDArtist WHERE P_IDCD=CD.IDCD and Artist.szArtistName like \"" + sWildcard + CMisc::GetSqlString(Selection->m_sParticipant) + sWildcard + "\")) ";

	if (Selection->m_szTrackTitle != "")
		str += "AND Lied.szTitel like \"" + sWildcard + CMisc::GetSqlString(Selection->m_szTrackTitle) + sWildcard + "\" ";

	if (Selection->m_szTrackLyrics != "")
		str += "AND Lied.szLiedtext like \"" + sWildcard + CMisc::GetSqlString(Selection->m_szTrackLyrics) + sWildcard + "\" ";

	if (Selection->m_szDateFrom != "")
		str += "AND CD.szDatum >= \"" + DateLong2Short(Selection->m_szDateFrom) + "\" ";

	if (Selection->m_szDateTo != "")
		str += "AND CD.szDatum <= \"" + DateLong2Short(Selection->m_szDateTo) + "\" ";

	if (Selection->m_szTrackArtist != "")
		// JUS 23.02.2003      str += "AND szArtistName like \"" + sWildcard + Selection->m_szTrackArtist + sWildcard + "\" ";
		str += "AND ArtistTrack.szArtistName like \"" + sWildcard + CMisc::GetSqlString(Selection->m_szTrackArtist) + sWildcard + "\" ";

	if (Selection->m_dwTrackLengthFrom > 0)
	{
		buf.Format(L"%d", Selection->m_dwTrackLengthFrom);
		str += "AND Lied.dwLaenge >= " + buf + " ";
	}

	if (Selection->m_dwTrackLengthTo > 0)
	{
		buf.Format(L"%d", Selection->m_dwTrackLengthTo + 999);
		str += "AND Lied.dwLaenge <= " + buf + " ";
	}

	if (Selection->m_wTrackBpmFrom > 0)
	{
		buf.Format(L"%d", Selection->m_wTrackBpmFrom);
		str += "AND Lied.wBpm >= " + buf + " ";
	}

	if (Selection->m_wTrackBpmTo > 0)
	{
		buf.Format(L"%d", Selection->m_wTrackBpmTo);
		str += "AND Lied.wBpm <= " + buf + " ";
	}

	if (Selection->m_szTrackCodes != "")
		str += "AND (" + GetWhereCodesString(Selection->m_szTrackCodes, TRUE) + ") ";

	if (Selection->m_szTrackComment != "")
		str += "AND Lied.szKommentar like \"" + sWildcard + CMisc::GetSqlString(Selection->m_szTrackComment) + sWildcard + "\" ";

	if (Selection->m_lTrackYearRecordedFrom > 0)
	{
		buf.Format(L"%d", Selection->m_lTrackYearRecordedFrom);
		str += "AND Lied.L_YearRecorded >= " + buf + " ";
	}

	if (Selection->m_lTrackYearRecordedTo > 0)
	{
		buf.Format(L"%d", Selection->m_lTrackYearRecordedTo);
		str += "AND Lied.L_YearRecorded <= " + buf + " ";
	}

	for (int i=0;i<MAX_USER_FIELDS;i++)
	{
		if (Selection->m_szTrackUser[i] != "")
		{
			str1.Format(L"AND Lied.szFeld%d like \"%s%s%s\" ", i+1, sWildcard, CMisc::GetSqlString(Selection->m_szTrackUser[i]), sWildcard);
			str += str1;
		}
	}

	if (Selection->m_sTrackSoundFilename != "")
		str += "AND Lied.szNameRecDatei like \"" + sWildcard + CMisc::GetSqlString(Selection->m_sTrackSoundFilename) + sWildcard + "\" ";

	// Version 10
	if (Selection->m_lTrackRatingFrom > 0)
	{
		buf.Format(L"%d", Selection->m_lTrackRatingFrom);
		str += "AND Lied.L_Rating >= " + buf + " ";
	}

	if (Selection->m_lTrackRatingTo > 0)
	{
		buf.Format(L"%d", Selection->m_lTrackRatingTo);
		str += "AND Lied.L_Rating <= " + buf + " ";
	}

	// Version 11
	if (Selection->m_szTrackLanguage != "")
		str += "AND Lied.L_Language like \"" + sWildcard + CMisc::GetSqlString(Selection->m_szTrackLanguage) + sWildcard + "\" ";

	int IDTrackKategorie = 0;
	if (Selection->m_szTrackCategory != "")
		IDTrackKategorie = GetIDFromCategory(Selection->m_szTrackCategory);

	if (IDTrackKategorie > 0)
	{
		buf.Format(L"%d", IDTrackKategorie);
		str += "AND Lied.L_IDCategory = " + buf + " ";
	}

	if (!Selection->m_sTrackComposer.IsEmpty())
		str += "AND ComposerTrack.szArtistName like \"" + sWildcard + CMisc::GetSqlString(Selection->m_sTrackComposer) + sWildcard + "\" ";


	if (bNoFirstAnd)        // Das erste AND entfernen
		str = str.Mid(4);

	return str;
}

// Liefert den Where-String für die Kennzeichen zurück.
// bTrack = TRUE: für Lied-Abfrage
CString CDataBase::GetWhereCodesString(const CString& Codes, BOOL bTrack)
{
   CString WhereString, CodesString = Codes;
   CString OrString, AndString;
   
   OrString.LoadString(IDS_OR);
   AndString.LoadString(IDS_AND);

   while (CodesString != "")
   {
      if (bTrack)
         WhereString += "Lied.szKennzeichen LIKE \"*" + CodesString.Left(1) + "*\"";     // Kennzeichen
      else
         WhereString += "CD.szKennzeichen LIKE \"*" + CodesString.Left(1) + "*\"";     // Kennzeichen

	  if (CodesString.GetLength() < 2)
		  break;

      CodesString = CodesString.Mid(2);
      // Jetzt "oder" oder "und" umsetzen
      if (CodesString == "")
         break;

      if (CodesString.Left(OrString.GetLength()) == OrString)
      {
         WhereString += " OR ";
         CodesString = CodesString.Mid(OrString.GetLength()+1);
      }

      if (CodesString.Left(AndString.GetLength()) == AndString)
      {
         WhereString += " AND ";
         CodesString = CodesString.Mid(AndString.GetLength()+1);
      }
   }

   return WhereString;
}

// Füllt alle vorhandenen CDSets in die angegebene ComboBox und liefert
// die Anzahl zurück.

int CDataBase::AddCDSetsToComboBox(CComboBox *cb)
{
   CDaoRecordset RecordSet(&m_db);
   COleVariant var;
   CString CDSetName;
   int nCount = 0;

   RecordSet.Open(dbOpenDynaset, L"SELECT szCDSetName FROM CDSet", dbSeeChanges);

   while (!RecordSet.IsEOF())
   {
      var = RecordSet.GetFieldValue(0);
      CDSetName = var.bstrVal;
      cb->AddString(CDSetName);
      nCount ++;
      RecordSet.MoveNext();
   }

   RecordSet.Close();

   return nCount;
}

int CDataBase::AddMediaToComboBox(CComboBox *cb, BOOL bResetContents /* = TRUE */)
{
   int i;

   if (bResetContents)
      cb->ResetContent();

   for (i=0;i<m_nNumberOfMedia;i++)
      cb->AddString(m_Medium[i]);

   return i;
}


BOOL CDataBase::UpdateCategory(LPCTSTR pszOldCategory, LPCTSTR pszNewCategory)
{
	long id;
	CString sqlStr;
	CWaitCursor wait;
	
	id = GetIDFromCategory(pszOldCategory);
	
	m_db.m_pWorkspace->BeginTrans();

	CString sNewCategory = pszNewCategory;
	CMisc::SqlPrepare(sNewCategory);
	
	sqlStr.Format(L"UPDATE Kategorie SET szKategorieName=\"%s\" WHERE IDKategorie=%ld", sNewCategory, id);
	if (!ExecuteSQL(sqlStr))
	{
		m_db.m_pWorkspace->Rollback();
		return FALSE;
	}
	
	m_db.m_pWorkspace->CommitTrans();
	
	return TRUE;
}

BOOL CDataBase::DeleteCategory(LPCTSTR pszCategory, LPCTSTR pszNewCategory, int nCategory)
{
   long id, idnew;
   CString sqlStr;
   CWaitCursor wait;

   id = GetIDFromCategory(pszCategory);

   if (pszNewCategory)
      idnew = GetIDFromCategory(pszNewCategory);
   else
      idnew = 0;

   m_db.m_pWorkspace->BeginTrans();

   sqlStr.Format(L"DELETE FROM Kategorie WHERE IDKategorie=%ld", id);
   if (!ExecuteSQL(sqlStr))
   {
      m_db.m_pWorkspace->Rollback();
      return FALSE;
   }

   sqlStr.Format(L"UPDATE CD SET IDKategorie=%ld WHERE IDKategorie=%ld", idnew, id);
   if (!ExecuteSQL(sqlStr))
   {
      m_db.m_pWorkspace->Rollback();
      return FALSE;
   }

   sqlStr.Format(L"Update Kategorie SET wOrder=wOrder-1 WHERE wOrder > %d", nCategory);
   if (!ExecuteSQL(sqlStr))
   {
      m_db.m_pWorkspace->Rollback();
      return FALSE;
   }

   m_db.m_pWorkspace->CommitTrans();

   return TRUE;
}

BOOL CDataBase::AddCategory(LPCTSTR pszCategory)
{
	CString sqlStr;
	CWaitCursor wait;
	CDaoRecordset RecordSet(&m_db);
	COleVariant var;
	CString sCategory = pszCategory;
	
	CMisc::SqlPrepare(sCategory);
	if (m_bReadOnly)
		return FALSE;
	
	ASSERT(m_nNumberOfCategories >= 0);
	
	m_db.m_pWorkspace->BeginTrans();
	
	sqlStr.Format(L"INSERT INTO Kategorie (szKategorieName, wOrder) VALUES (\"%s\", %d)", sCategory, m_nNumberOfCategories+1);
	if (!ExecuteSQL(sqlStr))
	{
		m_db.m_pWorkspace->Rollback();
		return FALSE;
	}
	
	m_Categories[m_nNumberOfCategories] = pszCategory;
	
	sqlStr.Format(L"SELECT IDKategorie FROM Kategorie WHERE szKategorieName = \"%s\"", sCategory);
	
	RecordSet.Open(dbOpenDynaset, sqlStr, dbSeeChanges);
	
	if (RecordSet.IsEOF())
	{
		m_db.m_pWorkspace->Rollback();
		ErrorBox("CDataBase.AddCategory: Can not insert new category.");
		return FALSE;
	}
	
	RecordSet.MoveFirst();
	
	var = RecordSet.GetFieldValue(0);
	m_IDCategories[m_nNumberOfCategories] = var.lVal;
	
	RecordSet.Close();
	
	m_db.m_pWorkspace->CommitTrans();
	
	m_nNumberOfCategories++;
	
	return TRUE;
}

// Tauscht die Kategorie nID mit der Kategorie nID+1 und verändert die Order
// in der Datenbank.

BOOL CDataBase::SwapCategories(int nCategory)
{
   CString sqlStr;
   CString swapCat;
   long swapID;

   ASSERT(m_nNumberOfCategories >= 0);

   sqlStr.Format(L"UPDATE Kategorie Set wOrder = %d WHERE IDKategorie = %d", nCategory+2, m_IDCategories[nCategory]);
   if (!ExecuteSQL(sqlStr))
      return FALSE;

   sqlStr.Format(L"UPDATE Kategorie Set wOrder = %d WHERE IDKategorie = %d", nCategory+1, m_IDCategories[nCategory+1]);
   if (!ExecuteSQL(sqlStr))
      return FALSE;

   swapCat = m_Categories[nCategory];
   m_Categories[nCategory] = m_Categories[nCategory+1];
   m_Categories[nCategory+1] = swapCat;

   swapID = m_IDCategories[nCategory];
   m_IDCategories[nCategory] = m_IDCategories[nCategory+1];
   m_IDCategories[nCategory+1] = swapID;

   return TRUE;   
}

// Löscht alle Kategorien. Da die betroffenen Sätze nicht geupdated werden,
// sollte diese Funktion nur aufgerufen werden, wenn kein Satz in der Datenbank
// vorhanden ist (wenn man also einen LEEREN Katalog ohne Kategorien haben will)
BOOL CDataBase::DeleteAllCategories()
{
   CWaitCursor wait;

   m_db.m_pWorkspace->BeginTrans();

   if (!ExecuteSQL("DELETE FROM Kategorie"))
   {
      m_db.m_pWorkspace->Rollback();
      return FALSE;
   }

   m_db.m_pWorkspace->CommitTrans();

   return TRUE;
}

BOOL CDataBase::SwapMedium(int nMedium)
{
   CString sqlStr;
   CString swapMedium;
   long swapID;

   ASSERT(m_nNumberOfMedia >= 0);

   sqlStr.Format(L"UPDATE Medium Set wOrder = %d WHERE IDMedium = %d", nMedium+2, m_IDMedium[nMedium]);
   if (!ExecuteSQL(sqlStr))
      return FALSE;

   sqlStr.Format(L"UPDATE Medium Set wOrder = %d WHERE IDMedium = %d", nMedium+1, m_IDMedium[nMedium+1]);
   if (!ExecuteSQL(sqlStr))
      return FALSE;

   swapMedium = m_Medium[nMedium];
   m_Medium[nMedium] = m_Medium[nMedium+1];
   m_Medium[nMedium+1] = swapMedium;

   swapID = m_IDMedium[nMedium];
   m_IDMedium[nMedium] = m_IDMedium[nMedium+1];
   m_IDMedium[nMedium+1] = swapID;

   return TRUE;   
}

BOOL CDataBase::AddMedium(LPCTSTR pszMedium)
{
	CString sqlStr;
	CWaitCursor wait;
	CDaoRecordset RecordSet(&m_db);
	COleVariant var;
	CString sMedium = pszMedium;
	
	CMisc::SqlPrepare(sMedium);
	
	if (m_bReadOnly)
		return FALSE;
	
	m_db.m_pWorkspace->BeginTrans();
	
	sqlStr.Format(L"INSERT INTO Medium (szMedium, wOrder) VALUES (\"%s\", %d)", sMedium, m_nNumberOfMedia+1);
	if (!ExecuteSQL(sqlStr))
	{
		m_db.m_pWorkspace->Rollback();
		return FALSE;
	}
	
	m_Medium[m_nNumberOfMedia] = pszMedium;
	
	sqlStr.Format(L"SELECT IDMedium FROM Medium WHERE szMedium = \"%s\"", sMedium);
	
	RecordSet.Open(dbOpenDynaset, sqlStr, dbSeeChanges);
	
	if (RecordSet.IsEOF())
	{
		m_db.m_pWorkspace->Rollback();
		ErrorBox("CDataBase.AddMedium: Can not insert new media.");
		return FALSE;
	}
	
	RecordSet.MoveFirst();
	
	var = RecordSet.GetFieldValue(0);
	m_IDMedium[m_nNumberOfMedia] = var.lVal;
	
	RecordSet.Close();
	
	m_db.m_pWorkspace->CommitTrans();
	
	m_nNumberOfMedia++;
	
	return TRUE;
}

BOOL CDataBase::DeleteMedium(LPCTSTR pszMedium, LPCTSTR pszNewMedium /*=NULL*/)
{
   long id, idnew;
   CString sqlStr;
   CWaitCursor wait;
   int i,j;

   if (m_bReadOnly)
	   return FALSE;

   id = GetIDFromMedium(pszMedium);

   if (pszNewMedium)
      idnew = GetIDFromMedium(pszNewMedium);
   else
      idnew = 0;

   m_db.m_pWorkspace->BeginTrans();

   sqlStr.Format(L"DELETE FROM Medium WHERE IDMedium=%ld", id);
   if (!ExecuteSQL(sqlStr))
   {
      m_db.m_pWorkspace->Rollback();
      return FALSE;
   }

   sqlStr.Format(L"UPDATE CD SET IDMedium=%ld WHERE IDMedium=%ld", idnew, id);
   if (!ExecuteSQL(sqlStr))
   {
      m_db.m_pWorkspace->Rollback();
      return FALSE;
   }

   for (i=0;i<m_nNumberOfMedia;i++)
   {
      if (m_IDMedium[i] == id)
      {
         for (j=i;j<m_nNumberOfMedia-1;j++)
         {
            m_Medium[j] = m_Medium[j+1];
            m_IDMedium[j] = m_IDMedium[j+1];
         }

         m_Medium[j] = "";
         m_IDMedium[j] = 0;
         break;
      }
   }

   m_nNumberOfMedia--;

   m_db.m_pWorkspace->CommitTrans();

   return TRUE;
}

BOOL CDataBase::EditMedium(LPCTSTR pszOldMedium, LPCTSTR pszNewMedium)
{
	long id;
	CString sqlStr;
	CWaitCursor wait;
	int i;
	
	if (m_bReadOnly)
		return FALSE;
	
	id = GetIDFromMedium(pszOldMedium);
	
	m_db.m_pWorkspace->BeginTrans();
	
	CString sNewMedium = pszNewMedium;
	CMisc::SqlPrepare(sNewMedium);

	sqlStr.Format(L"UPDATE Medium SET szMedium=\"%s\" WHERE IDMedium=%ld", sNewMedium, id);
	if (!ExecuteSQL(sqlStr))
	{
		m_db.m_pWorkspace->Rollback();
		return FALSE;
	}
	
	m_db.m_pWorkspace->CommitTrans();
	
	for (i=0;i<m_nNumberOfMedia;i++)
	{
		if (m_IDMedium[i] == id)
		{
			m_Medium[i] = pszNewMedium;
			break;
		}
	}
	
	return TRUE;
}


BOOL CDataBase::ExecuteSQL(CString sqlStr)
{
   try
   {
      m_db.Execute(sqlStr);
   }
   catch(CDaoException* e)
   {
      ErrorBox(e->m_pErrorInfo->m_strDescription);
      e->Delete();
      return FALSE;
   }

   return TRUE;
}

// Liest die Informationen aus der Master-Tabelle
BOOL CDataBase::ReadDBProperties()
{
	m_MasterTable.m_pDatabase = &m_db;
	
	if (!IsDataBaseOpened())
		return FALSE;

	try 
	{
		m_MasterTable.Open(dbOpenTable, CMaster::GetTableName(), dbSeeChanges);
		
		if (m_MasterTable.IsEOF())
		{
			m_MasterTable.Close();
			ErrorBox("CDataBase.ReadDBProperties: Master table not found.");
			return FALSE;
		}
		
		m_MasterTable.MoveFirst();

		m_MasterTable.Close();
	}
	catch (CException* e)
	{
		e->Delete();
	}
	
	return TRUE;
}

// Schreibt die Informationen in die Master-Tabelle
BOOL CDataBase::WriteDBProperties()
{
	CWaitCursor wait;
	CString sqlStr, s;
	
	if (m_bReadOnly)
		return FALSE;
	
	if (!IsDataBaseOpened())
		return FALSE;
	
	try
	{
		m_db.m_pWorkspace->BeginTrans();

		// Zuerst sichern, da ja durch das Open die Klasse neu gefüllt wird!
		CMaster MasterSave;
		
		MasterSave = m_MasterTable;
		
		m_MasterTable.Open(dbOpenTable, CMaster::GetTableName(), dbSeeChanges);

		m_MasterTable.Edit();
		m_MasterTable = MasterSave;
		m_MasterTable.Update();

		m_MasterTable.Close();

		m_db.m_pWorkspace->m_pDAOWorkspace->CommitTrans(dbForceOSFlush );
	}
	catch(CDaoException* e)
	{
		ErrorBox(e->m_pErrorInfo->m_strDescription);
		e->Delete();
		return FALSE;
	}

	return TRUE;
}

long CDataBase::GetIDFromCategory(LPCTSTR pszCategory)
{
   int i;

   if (!pszCategory[0])
      return 0;

   ASSERT(m_nNumberOfCategories >= 0);

   for (i=0;i<m_nNumberOfCategories && 
         _wcsicmp(pszCategory, m_Categories[i]);i++);

   if (!_wcsicmp(pszCategory, m_Categories[i]))
      return m_IDCategories[i];
   else
      return 0;
}

long CDataBase::GetIDFromMedium(LPCTSTR pszMedium)
{
   int i;

   if (!pszMedium[0])
      return 0;

   for (i=0;i<m_nNumberOfMedia && 
         _wcsicmp(pszMedium, m_Medium[i]);i++);

   if (!_wcsicmp(pszMedium, m_Medium[i]))
      return m_IDMedium[i];
   else
      return 0;
}


void CDataBase::AddCodesToComboBox(CComboBox *cb, BOOL bShowEmpty /* = TRUE */)
{
	CString rcstr;
	
	rcstr.LoadString(IDS_NONE);
	cb->AddString((CString)"<" + rcstr + (CString)">");
	
	for (int i=0;i<MAX_CODES;i++)
	{
		if (bShowEmpty || m_Codes[i].GetLength())
		{
			CString str;
			str.Format(L"%c: %s", 'A' + i, m_Codes[i]);
			cb->AddString(str);
		}
	}
}


// Liefert das aktuelle Datum als String in der eingestellten Form zurück.
CString CDataBase::GetDate()
{
	CTime now = CTime::GetCurrentTime();
	CString str;
	
	switch (m_MasterTable.m_cTypeDate)
	{
#if GERMAN
	case DATE_TTMMJJJJ:
		str.Format(L"%02d.%02d.%04d", now.GetDay(), now.GetMonth(), now.GetYear());
		break;
	case DATE_MMJJJJ:
		str.Format(L"%02d.%04d", now.GetMonth(), now.GetYear());
		break;
	case DATE_JJJJ:
		str.Format(L"%04d", now.GetYear());
		break;
#else
	case DATE_TTMMJJJJ:
		str.Format("%02d/%02d/%04d", now.GetMonth(), now.GetDay(), now.GetYear());
		break;
	case DATE_MMJJJJ:
		str.Format("%02d/%04d", now.GetMonth(), now.GetYear());
		break;
	case DATE_JJJJ:
		str.Format("%04d", now.GetYear());
		break;
#endif
	default:
		str = "";
	}
	
	return str;
}

/*
 * Converts a short date (YYYYMMDD) into a real date.
 * returns a pointer to converted string.
 * German: DD.MM.JJJJ
 * English: MM/DD/JJJJ
 */

CString CDataBase::DateShort2Long(CString sDate)
{
   CString datestr;

   if (sDate.IsEmpty())
      {
      datestr = "";
      return datestr;
      }

   switch (m_MasterTable.m_cTypeDate)
   {
   case DATE_TTMMJJJJ:
#if ENGLISH
      datestr = sDate.Mid(4,2)+"/"+sDate.Mid(6,2)+"/"+sDate.Mid(0,4);
#else
      datestr = sDate.Mid(6,2)+"."+sDate.Mid(4,2)+"."+sDate.Mid(0,4);
#endif
      break;

   case DATE_MMJJJJ:
#if ENGLISH
      datestr = sDate.Mid(4,2)+"/"+sDate.Mid(0,4);
#else
      datestr = sDate.Mid(4,2)+"."+sDate.Mid(0,4);
#endif
      break;

   case DATE_JJJJ:
      datestr = sDate.Mid(0,4);
      break;

   default:
      datestr = sDate;
      break;
   }

   return datestr;
}

/*
 * Converts a long date into a short date format.
 * returns a pointer to converted string.
 */

CString CDataBase::DateLong2Short(CString lDate)
{
	CString datestr;
	CString check;
	int pos;
	int day, month, year;
	
	if (!DateCheckFormat(lDate) || lDate.IsEmpty())
	{
		datestr = "";
		return datestr;
	}
	
	switch (m_MasterTable.m_cTypeDate)
	{
	case DATE_TTMMJJJJ:
		check.Format(L"%-2.2s", lDate);
#if ENGLISH
		month = _wtoi(check);
#else
		day = _wtoi(check);
#endif
        pos = lDate.FindOneOf(L"./-");
		
		check = lDate.Mid(pos+1,2);
#if ENGLISH
		day = _wtoi(check);
#else
		month = _wtoi(check);
#endif
		pos = pos+1+lDate.Mid(pos+1).FindOneOf(L"./-");
		
		check = lDate.Mid(pos+1,4);
		year = _wtoi(check);
		
		datestr.Format(L"%04d%02d%02d", year, month, day);
		break;
		
	case DATE_MMJJJJ:
		datestr = lDate.Mid(3, 4) + lDate.Mid(0,2) + "01";
		break;
		
	case DATE_JJJJ:
		datestr = lDate.Mid(0, 4) + "0101";
		break;
		
	default:
		datestr = lDate;
		break;
	}
	
	return datestr;
}

/*
 * Checks, if the given date is in correct form (e.g. DD.MM.JJJJ or MM/DD/JJJJ)
 * Returns TRUE if it is, else FALSE
 */
 
int CDataBase::DateCheckFormat(CString date)
{
   CString check;
   int cdate, cmonth, cyear;
   int pos;

   if (date.GetLength() == 0)
		return TRUE;

   switch (m_MasterTable.m_cTypeDate)
   {
   case DATE_TTMMJJJJ:
      check.Format(L"%-2.2s", date);
#if ENGLISH
      cmonth = _wtoi(check);
#else
      cdate = _wtoi(check);
#endif
      pos = date.FindOneOf(L"./-");

      check = date.Mid(pos+1,2);
#if ENGLISH
      cdate = _wtoi(check);
#else
      cmonth = _wtoi(check);
#endif
      pos = pos+1+date.Mid(pos+1).FindOneOf(L"./-");

      check = date.Mid(pos+1,4);
      cyear = _wtoi(check);
   
      break;

   case DATE_MMJJJJ:
      if (date.GetLength() != 7)
         return FALSE;
      if (date[2] != '/' && date[2] != '.')
         return FALSE;
      cdate = 1;
      check = date.Mid(0,2);
      cmonth = _wtoi(check);

      check = date.Mid(3,4);
      cyear = _wtoi(check);

      break;

   case DATE_JJJJ:
      if (date.GetLength() != 4)
         return FALSE;
      cdate = 1;
      cmonth = 1;
      cyear = _wtoi(date);
      break;
   default:
      return TRUE;
   }

   if (cdate < 1 || cdate > 31)
      return FALSE;
   if (cmonth < 1 || cmonth > 12)
      return FALSE;

   if ((cmonth == 4 || cmonth == 6 || cmonth == 9 || cmonth == 11) && cdate > 30)
      return FALSE;

   if (cmonth == 2 && (cdate > 29 || cdate > 28 && ((cyear % 4) || !(cyear % 200))))
      return FALSE;

   if (cyear < 1000 || cyear > 9999)
      return FALSE;
   
   return TRUE;
}

// Liefert das Format des Datums zurück (z.B. "TT.MM.JJJJ")
CString CDataBase::GetDateFormatString(void)
{
#if ENGLISH
   switch (m_MasterTable.m_cTypeDate)
   {
      case DATE_TTMMJJJJ:
         return "MM/DD/YYYY";
      case DATE_MMJJJJ:
         return "MM/YYYY";
      case DATE_JJJJ:
         return "YYYY";
      default:
         return "";
   }
#else
   switch (m_MasterTable.m_cTypeDate)
   {
      case DATE_TTMMJJJJ:
         return "TT.MM.JJJJ";
      case DATE_MMJJJJ:
         return "MM.JJJJ";
      case DATE_JJJJ:
         return "JJJJ";
      default:
         return "";
   }
#endif
}

// Liefert den Namen des Feldes zurück.
// Ist bCDAndTrack auf TRUE, bedeutet das, dass in einer Tabelle CD- und
// Lied-Felder angezeigt werden. In diesem Falle werden zweideutige Felder
// entsprechend gekennzeichnet.
// Aus "Interpret" wird z.B. "Interpret des Liedes", damit man es unterscheiden kann.
CString CDataBase::GetFieldName(UINT uiField, int& iDefaultSize, BOOL bCDAndTrack /* = FALSE */, BOOL bUseDefaultUserFieldName /* = FALSE */)
{
	iDefaultSize = 0;
	CString str;

	switch (uiField)
	{
	case FIELD_TOTALLENGTH:
		str = get_string(IDS_TOTALLENGTH);
		iDefaultSize = 50;
		break;
	case FIELD_NUMBEROFTRACKS:
		str = get_string(IDS_NUMBEROFTRACKS);
		iDefaultSize = 50;
		break;
	case FIELD_CDSET:
		str = get_string(IDS_CDSET);
		iDefaultSize = 200;
		break;
	case FIELD_CDSAMPLER:
		str = get_string(IDS_CDSAMPLER);
		iDefaultSize = 50;
		break;
	case FIELD_CDNAME:
		str = get_string(IDS_ARTIST);
		iDefaultSize = 200;
		break;
	case FIELD_CDTITLE:
		str = get_string(IDS_TITLE);
		iDefaultSize = 200;
		break;
	case FIELD_CATEGORY:
		str = get_string(IDS_CATEGORY);
		iDefaultSize = 100;
		break;
	case FIELD_DATE:
		if (!m_MasterTable.m_sNameDate.IsEmpty())
			str = m_MasterTable.m_sNameDate;
		else
			str = get_string(IDS_DATE);
		iDefaultSize = 100;
		break;
	case FIELD_CODES:
		str = get_string(IDS_CODES);
		iDefaultSize = 60;
		break;
	case FIELD_CDCOMMENT:
		str = get_string(IDS_COMMENT);
		iDefaultSize = 200;
		break;
	case FIELD_ARCHIVNUMMER:
		str = get_string(IDS_ARCHIVNUMMER);
		iDefaultSize = 50;
		break;
	case FIELD_MEDIUM:
		str = get_string(IDS_MEDIUM);
		iDefaultSize = 100;
		break;
	case FIELD_YEAR_RECORDED:
		str = get_string(IDS_YEAR_RECORDED);
		iDefaultSize = 50;
		break;
	case FIELD_COPYRIGHT:
		str = get_string(IDS_COPYRIGHT);
		iDefaultSize = 200;
		break;
	case FIELD_CDCOVER_FILENAME:
		str = get_string(IDS_CDCOVER_FILENAME);
		iDefaultSize = 200;
		break;
	case FIELD_CDCOVERBACK_FILENAME:
		str = get_string(IDS_CDCOVERBACK_FILENAME);
		iDefaultSize = 200;
		break;
	case FIELD_CDCOVERLABEL_FILENAME:
		str = get_string(IDS_CDCOVERLABEL_FILENAME);
		iDefaultSize = 200;
		break;
	case FIELD_ORIGINAL_CD:
		str = get_string(IDS_ORIGINAL_CD);
		iDefaultSize = 50;
		break;
	case FIELD_LABEL:
		str = get_string(IDS_LABEL);
		iDefaultSize = 100;
		break;
	case FIELD_UPC:
		str = get_string(IDS_UPC);
		iDefaultSize = 100;
		break;
	case FIELD_URL:
		str = get_string(IDS_URL);
		iDefaultSize = 200;
		break;
	case FIELD_RATING:
		str = get_string(IDS_RATING);
		iDefaultSize = 50;
		break;
	case FIELD_PRICE:
		str = get_string(IDS_PRICE);
		iDefaultSize = 100;
		break;
	case FIELD_LANGUAGE:
		str = get_string(IDS_LANGUAGE);
		iDefaultSize = 100;
		break;
	case FIELD_LOCATION:
		str = get_string(IDS_LOCATION);
		iDefaultSize = 100;
		break;
	case FIELD_CDUSER1:
	case FIELD_CDUSER2:
	case FIELD_CDUSER3:
	case FIELD_CDUSER4:
	case FIELD_CDUSER5:
		if (!m_MasterTable.m_sCDUserField[uiField-FIELD_CDUSER1].IsEmpty() && !bUseDefaultUserFieldName)
		{
			str = m_MasterTable.m_sCDUserField[uiField-FIELD_CDUSER1];
		}
		else
		{
			str.Format(L"%s %d (%s)", get_string(IDS_USER), uiField-FIELD_CDUSER1+1, get_string(IDS_CD));
		}
		iDefaultSize = 200;
		break;
		
	case FIELD_TRACK_NUMBER:
		str = get_string(IDS_NUMBER);
		iDefaultSize = 50;
		break;
	case FIELD_TRACK_ARTIST:
		if (bCDAndTrack)
			str = get_string(IDS_TRACKARTIST);
		else
			str = get_string(IDS_ARTIST);
		iDefaultSize = 200;
		break;
	case FIELD_TRACK_TITLE:
		str = get_string(IDS_NAMEOFTRACK);
		iDefaultSize = 200;
		break;
	case FIELD_TRACK_LENGTH:
		str = get_string(IDS_LENGTH);
		iDefaultSize = 50;
		break;
	case FIELD_TRACK_BPM:
		str = get_string(IDS_BPM);
		iDefaultSize = 50;
		break;
	case FIELD_TRACK_CODES:
		if (bCDAndTrack)
			str = get_string(IDS_TRACKCODES);
		else
			str = get_string(IDS_CODES);
		iDefaultSize = 60;
		break;
	case FIELD_TRACK_COMMENT:
		if (bCDAndTrack)
			str = get_string(IDS_TRACKCOMMENT);
		else
			str = get_string(IDS_COMMENT);
		iDefaultSize = 200;
		break;
	case FIELD_TRACK_YEAR_RECORDED:
		if (bCDAndTrack)
			str = get_string(IDS_TRACK_YEAR_RECORDED);
		else
			str = get_string(IDS_YEAR_RECORDED);
		iDefaultSize = 50;
		break;
	case FIELD_TRACK_USER1:
	case FIELD_TRACK_USER2:
	case FIELD_TRACK_USER3:
	case FIELD_TRACK_USER4:
	case FIELD_TRACK_USER5:
		if (!m_MasterTable.m_sTrackUserField[uiField-FIELD_TRACK_USER1].IsEmpty() && !bUseDefaultUserFieldName)
		{
			str = m_MasterTable.m_sTrackUserField[uiField-FIELD_TRACK_USER1];
		}
		else
		{
			str.Format(L"%s %d (%s)", get_string(IDS_USER), uiField-FIELD_TRACK_USER1+1, get_string(IDS_TRACK));
		}
		
		iDefaultSize = 200;
		break;
	case FIELD_TRACK_LYRICS:
		{
			str = get_string(IDS_LYRICS);
			iDefaultSize = 20;
			break;
		}
	case FIELD_TRACK_SOUNDFILE:
		{
			str = get_string(IDS_SOUNDFILE);
			iDefaultSize = 200;
			break;
		}
/*	case FIELD_TRACK_FILENAME:
		{
			str = get_string(IDS_FILENAME);
			iDefaultSize = 200;
			break;
		}*/
	case FIELD_TRACK_RATING:
		{
			if (bCDAndTrack)
				str = get_string(IDS_TRACK_RATING);
			else
				str = get_string(IDS_RATING);
			iDefaultSize = 50;
			break;
		}
	case FIELD_TRACK_CATEGORY:
		str = get_string(IDS_TRACK_CATEGORY);
		iDefaultSize = 100;
		break;

	case FIELD_TRACK_LANGUAGE:
		str = get_string(IDS_TRACK_LANGUAGE);
		iDefaultSize = 100;
		break;

	case FIELD_ARTIST_CD_SORTKEY:
		{
			str = get_string(TEXT_SORTKEY);
			iDefaultSize = 200;
			break;
		}
	case FIELD_ARTIST_CD_GROUPTYPE:
		{
			str = get_string(TEXT_GROUP);
			iDefaultSize = 50;
			break;
		}
	case FIELD_ARTIST_CD_SEX:
		{
			str = get_string(TEXT_SEX);
			iDefaultSize = 50;
			break;
		}
	case FIELD_ARTIST_CD_COMMENT:
		{
			str = get_string(TEXT_COMMENT);
			iDefaultSize = 200;
			break;
		}
	case FIELD_ARTIST_CD_URL:
		{
			str = get_string(IDS_URL);
			iDefaultSize = 200;
			break;
		}
	case FIELD_ARTIST_CD_COUNTRY:
		{
			str = get_string(IDS_COUNTRY);
			iDefaultSize = 100;
			break;
		}
	case FIELD_ARTIST_CD_BIRTHDAY:
		{
			str = get_string(IDS_ARTIST_BIRTHDAY);
			iDefaultSize = 100;
			break;
		}
	case FIELD_ARTIST_CD_DAYOFDEATH:
		{
			str = get_string(IDS_ARTIST_DAYOFDEATH);
			iDefaultSize = 100;
			break;
		}
	case FIELD_ARTIST_CD_IMAGEFILENAME:
		{
			str = get_string(IDS_ARTIST_IMAGE);
			iDefaultSize = 100;
			break;
		}
	case FIELD_ARTIST_TRACK_NAME:
		{
			str = get_string(TEXT_TRACKARTIST);
			iDefaultSize = 200;
			break;
		}
	case FIELD_ARTIST_TRACK_SORTKEY:
		{
			str = get_string(TEXT_SORTKEY);
			iDefaultSize = 200;
			break;
		}
	case FIELD_ARTIST_TRACK_GROUPTYPE:
		{
			str = get_string(TEXT_GROUP);
			iDefaultSize = 50;
			break;
		}
	case FIELD_ARTIST_TRACK_SEX:
		{
			str = get_string(TEXT_SEX);
			iDefaultSize = 50;
			break;
		}
	case FIELD_ARTIST_TRACK_COMMENT:
		{
			if (bCDAndTrack)
				str = get_string(TEXT_TRACKCOMMENT);
			else
				str = get_string(TEXT_COMMENT);
			iDefaultSize = 50;
			break;
		}
	case FIELD_ARTIST_TRACK_URL:
		{
			str = get_string(IDS_URL);
			iDefaultSize = 200;
			break;
		}
	case FIELD_ARTIST_TRACK_COUNTRY:
		{
			str = get_string(IDS_COUNTRY);
			iDefaultSize = 100;
			break;
		}
	case FIELD_ARTIST_TRACK_BIRTHDAY:
		{
			str = get_string(IDS_ARTIST_BIRTHDAY);
			iDefaultSize = 100;
			break;
		}
	case FIELD_ARTIST_TRACK_DAYOFDEATH:
		{
			str = get_string(IDS_ARTIST_DAYOFDEATH);
			iDefaultSize = 100;
			break;
		}
	case FIELD_ARTIST_TRACK_IMAGEFILENAME:
		{
			str = get_string(IDS_ARTIST_IMAGE);
			iDefaultSize = 100;
			break;
		}

	case FIELD_COMPOSER_CD_NAME:
		{
			str = get_string(IDS_COMPOSER_NAME) + " (" + get_string(IDS_CD) + ")";
			iDefaultSize = 200;
			break;
		}
	case FIELD_COMPOSER_CD_SORTKEY:
		{
			str = get_string(IDS_COMPOSER_SORTKEY) + " (" + get_string(IDS_CD) + ")";
			iDefaultSize = 200;
			break;
		}
	case FIELD_COMPOSER_CD_GROUPTYPE:
		{
			str = get_string(IDS_COMPOSER_GROUP) + " (" + get_string(IDS_CD) + ")";
			iDefaultSize = 50;
			break;
		}
	case FIELD_COMPOSER_CD_SEX:
		{
			str = get_string(IDS_COMPOSER_SEX) + " (" + get_string(IDS_CD) + ")";
			iDefaultSize = 50;
			break;
		}
	case FIELD_COMPOSER_CD_COMMENT:
		{
			str = get_string(IDS_COMPOSER_COMMENT) + " (" + get_string(IDS_CD) + ")";
			iDefaultSize = 200;
			break;
		}
	case FIELD_COMPOSER_CD_URL:
		{
			str = get_string(IDS_COMPOSER_URL) + " (" + get_string(IDS_CD) + ")";
			iDefaultSize = 200;
			break;
		}
	case FIELD_COMPOSER_CD_COUNTRY:
		{
			str = get_string(IDS_COMPOSER_COUNTRY) + " (" + get_string(IDS_CD) + ")";
			iDefaultSize = 100;
			break;
		}
	case FIELD_COMPOSER_CD_BIRTHDAY:
		{
			str = get_string(IDS_COMPOSER_BIRTHDAY) + " (" + get_string(IDS_CD) + ")";
			iDefaultSize = 100;
			break;
		}
	case FIELD_COMPOSER_CD_DAYOFDEATH:
		{
			str = get_string(IDS_COMPOSER_DAYOFDEATH) + " (" + get_string(IDS_CD) + ")";
			iDefaultSize = 100;
			break;
		}
	case FIELD_COMPOSER_CD_IMAGEFILENAME:
		{
			str = get_string(IDS_COMPOSER_IMAGE) + " (" + get_string(IDS_CD) + ")";
			iDefaultSize = 100;
			break;
		}

	case FIELD_COMPOSER_TRACK_NAME:
		{
			str = get_string(IDS_COMPOSER_NAME) + " (" + get_string(IDS_TRACK) + ")";
			iDefaultSize = 200;
			break;
		}
	case FIELD_COMPOSER_TRACK_SORTKEY:
		{
			str = get_string(IDS_COMPOSER_SORTKEY) + " (" + get_string(IDS_TRACK) + ")";
			iDefaultSize = 200;
			break;
		}
	case FIELD_COMPOSER_TRACK_GROUPTYPE:
		{
			str = get_string(IDS_COMPOSER_GROUP) + " (" + get_string(IDS_TRACK) + ")";
			iDefaultSize = 50;
			break;
		}
	case FIELD_COMPOSER_TRACK_SEX:
		{
			str = get_string(IDS_COMPOSER_SEX) + " (" + get_string(IDS_TRACK) + ")";
			iDefaultSize = 50;
			break;
		}
	case FIELD_COMPOSER_TRACK_COMMENT:
		{
			str = get_string(IDS_COMPOSER_COMMENT) + " (" + get_string(IDS_TRACK) + ")";
			iDefaultSize = 50;
			break;
		}
	case FIELD_COMPOSER_TRACK_URL:
		{
			str = get_string(IDS_COMPOSER_URL) + " (" + get_string(IDS_TRACK) + ")";
			iDefaultSize = 200;
			break;
		}
	case FIELD_COMPOSER_TRACK_COUNTRY:
		{
			str = get_string(IDS_COMPOSER_COUNTRY) + " (" + get_string(IDS_TRACK) + ")";
			iDefaultSize = 100;
			break;
		}
	case FIELD_COMPOSER_TRACK_BIRTHDAY:
		{
			str = get_string(IDS_COMPOSER_BIRTHDAY) + " (" + get_string(IDS_TRACK) + ")";
			iDefaultSize = 100;
			break;
		}
	case FIELD_COMPOSER_TRACK_DAYOFDEATH:
		{
			str = get_string(IDS_COMPOSER_DAYOFDEATH) + " (" + get_string(IDS_TRACK) + ")";
			iDefaultSize = 100;
			break;
		}
	case FIELD_COMPOSER_TRACK_IMAGEFILENAME:
		{
			str = get_string(IDS_COMPOSER_IMAGE) + " (" + get_string(IDS_TRACK) + ")";
			iDefaultSize = 100;
			break;
		}
	default:
		ASSERT(uiField == 0);
	}

	return str;
}

void CDataBase::SetDateText(CWnd *pWnd)
{
	CString str;
	
	if (!m_MasterTable.m_sNameDate.IsEmpty())
	{
		str.Format(L"%s:", m_MasterTable.m_sNameDate);
		pWnd->SetWindowText(str);
	}
	else
	{
		str.LoadString(IDS_UNDEFINED);
		pWnd->SetWindowText("<" + str + ">");
	}
}

void CDataBase::AddCategoriesToComboBox(CComboBox *cb, BOOL bResetContents/* = TRUE */)
{
   if (bResetContents)
      cb->ResetContent();

   ASSERT(m_nNumberOfCategories >= 0);

   for (int i=0;i<m_nNumberOfCategories;i++)
       cb->AddString(m_Categories[i]);
}


COleVariant CDataBase::Abfrage(const CString& SQLString, BOOL* bFound /* = NULL */)
{
	CDaoRecordset RecordSet(&m_db);
	COleVariant var;
	
	RecordSet.Open(dbOpenDynaset, SQLString, dbSeeChanges);
	
	if (!RecordSet.IsEOF())
	{
		RecordSet.MoveFirst();
		
		var = RecordSet.GetFieldValue(0);
		
		if (bFound)
			*bFound = TRUE;
	}
	else
	{
		if (bFound)
			*bFound = FALSE;

		var.Clear();
		// Achtung: Hier var undefiniert!
	}
	   
	RecordSet.Close();
	
	return var;
}

int CDataBase::AbfrageInt(const CString &sSQLString, BOOL *bFound)
{
	COleVariant var;

	var = Abfrage(sSQLString, bFound);

	return var.lVal;
}

BOOL CDataBase::WriteIdentity(const CString & strIdentity, DWORD dwRecNr)
{
   CDaoRecordset RecordSet(&m_db);
   BOOL bAddNew;
   COleVariant var;

   if (strIdentity.IsEmpty())
	   return FALSE;

   try
   {
	   RecordSet.Open(dbOpenDynaset, "select * from identity where szIdentity = \"" + strIdentity + "\"", dbSeeChanges);
	   
	   bAddNew = RecordSet.IsEOF();
	   
	   if (bAddNew)
	   {
		   RecordSet.AddNew();
		   var.SetString(strIdentity, VT_BSTRT);
		   RecordSet.SetFieldValue(L"szIdentity", var);
		   
		   var = (long)dwRecNr;
		   RecordSet.SetFieldValue(L"IDCD", var);
		   RecordSet.Update();
	   }

	   RecordSet.Close();
   }
   catch(CDaoException* e)
   {
	   ErrorBox(e->m_pErrorInfo->m_strDescription);
	   e->Delete();
	   return FALSE;
   }

   return TRUE;
}

BOOL CDataBase::GetIDList(DWORD dwRecNr, CStringArray & IDList)
{
	CDaoRecordset RecordSet(&m_db);
	COleVariant var;
	
	try
	{
		CString sSQL;
		sSQL.Format(L"select * from identity where IDCD = %ld", dwRecNr);
		RecordSet.Open(dbOpenDynaset, sSQL, dbSeeChanges);
		
		while (!RecordSet.IsEOF())
		{
			var = RecordSet.GetFieldValue(L"szIdentity");
			IDList.Add(var.bstrVal);

			RecordSet.MoveNext();
		}

		RecordSet.Close();
	}
	catch(CDaoException* e)
	{
		ErrorBox(e->m_pErrorInfo->m_strDescription);
		e->Delete();
		return FALSE;
	}
	
	return TRUE;
}

BOOL CDataBase::WriteIDList(DWORD dwRecNr, const CStringArray & IDList)
{
	BOOL bRet = TRUE;

	for (int i=0;i<IDList.GetSize() && bRet;i++)
	{
		bRet = WriteIdentity(IDList[i], dwRecNr);
	}

	return bRet;
}

// Prüft, ob die angegebene ArchivNummer vorhanden ist.
DWORD CDataBase::CheckArchiveNumber(const CString & ArchiveNumber)
{
	CDaoRecordset RecordSet(&m_db);
	COleVariant var;
	CString SQLString = "select IDCD from cd where szArchivNummer = \"" + ArchiveNumber + "\"";
	DWORD dwRecordNumber = 0;
	
	if (ArchiveNumber.IsEmpty())
		return 0;
	
	RecordSet.Open(dbOpenDynaset, SQLString, dbSeeChanges);
	
	if (!RecordSet.IsEOF())
	{
		RecordSet.MoveFirst();
		var = RecordSet.GetFieldValue(0);
		dwRecordNumber = var.lVal;
	}
	
	RecordSet.Close();
	
	return dwRecordNumber;
}

// Alle globalen Tabellen lesen
BOOL CDataBase::ReadAllTables()
{
	if (!ReadAllCategories())
		return FALSE;

	if (!ReadAllCodes())
		return FALSE;
	
	if (!ReadAllMedia())
		return FALSE;

	if (!ReadDBProperties())         // Master-Tabelle
		return FALSE;

	return TRUE;
}

BOOL CDataBase::IsReadOnly()
{
	return m_bReadOnly;
}

// Datenbank-Pfad speichern. Wird vom Open() benutzt, wenn kein Name für
// die Datei angegeben wird.
void CDataBase::SetDataBasePath(const CString &sFilename)
{
	m_sDBFilename = sFilename;
}

CString CDataBase::GetDataBasePath()
{
	return m_sDBFilename;
}

// Alle CDs dieses Interpreten löschen!
BOOL CDataBase::DeleteAllCDsFromArtist(long dwID)
{
	CString sSQL;

	sSQL.Format(L"DELETE * FROM %s WHERE IDArtist=%d", CCD::GetTableName(), dwID);

	try
	{
		m_db.Execute(sSQL, dbSeeChanges|dbFailOnError);
	}
	catch (CDaoException* e)
	{
		e->ReportError();
		e->Delete();
		return FALSE;
	}

	return TRUE;
}

BOOL CDataBase::BeginTransaction()
{
	m_db.m_pWorkspace->BeginTrans();

	return TRUE;
}

BOOL CDataBase::CommitTransaction()
{
	m_db.m_pWorkspace->CommitTrans();

	return TRUE;
}

BOOL CDataBase::RollbackTransaction()
{
	m_db.m_pWorkspace->Rollback();

	return TRUE;
}

// Liefert den Where-String für Abfragen von Artisten.
CString CDataBase::GetWhereStringArtist(const CSelection &sel, BOOL bSimple, BOOL bNoFirstAnd)
{
	CString sWhere;
	CString sWildcard;
	
	if (bSimple)
		sWildcard = "*";
	
	if (sel.m_szCDArtist != "")
		sWhere += "AND Artist.szArtistName like \"" + sWildcard + sel.m_szCDArtist + sWildcard + "\" ";
	
    if (bNoFirstAnd)        // Das erste AND entfernen
		sWhere = sWhere.Mid(4);

	return sWhere;
}

// Liefert den Where-String für Abfragen von CDs.
CString CDataBase::GetWhereStringCD(const CSelection &sel, BOOL bSimple, BOOL bNoFirstAnd, BOOL bNoArtist)
{
	CString sWhere, buf;
	int IDKategorie, IDMedium;
	CString sWildcard;
	CString str1;
	
	// Wenn ein erweiterter Filter definiert ist, dann nehmen wir den.
	if (!sel.IsFilterEmpty())
	{
		sWhere = GetFilterWhereString(sel);
		return sWhere;
	}

	if (bSimple || !sel.m_bExactMatch)
		sWildcard = "*";
	
	ASSERT(m_nNumberOfCategories >= 0);
	
	if (sel.m_wCDType == 1)     // Nur CDs mit einem Interpreten
		sWhere += "AND CD.bCDSampler = FALSE ";
	
	if (sel.m_wCDType == 2)     // Nur CDs mit mehreren Interpreten
		sWhere += "AND CD.bCDSampler = TRUE ";
	
	if (sel.m_szCDArtist != "" && !bNoArtist)
		sWhere += "AND Artist.szArtistName like \"" + sWildcard + CMisc::GetSqlString(sel.m_szCDArtist) + sWildcard + "\" ";
	
	if (sel.m_szCDTitle != "")
		sWhere += "AND CD.szTitel like \"" + sWildcard + CMisc::GetSqlString(sel.m_szCDTitle) + sWildcard + "\" ";
	
	if (sel.m_dwTotalLengthFrom > 0)
	{
		buf.Format(L"%ld", sel.m_dwTotalLengthFrom);
		sWhere += "AND CD.dwGesamtLaenge >= " + buf + " ";
	}
	
	if (sel.m_dwTotalLengthTo > 0)
	{                           // Achtung: + 999, wegen voller Sekunde!
		buf.Format(L"%ld", sel.m_dwTotalLengthTo+999);
		sWhere += "AND CD.dwGesamtLaenge <= " + buf + " ";
	}
	
	if (sel.m_wNumberOfTracksFrom > 0)
	{
		buf.Format(L"%d", sel.m_wNumberOfTracksFrom);
		sWhere += "AND CD.cAnzahlLieder >= " + buf + " ";
	}
	
	if (sel.m_wNumberOfTracksTo > 0)
	{
		buf.Format(L"%d", sel.m_wNumberOfTracksTo);
		sWhere += "AND CD.cAnzahlLieder <= " + buf + " ";
	}
	
	IDKategorie = 0;
	if (sel.m_szCategory != "")
	{
		IDKategorie = GetIDFromCategory(sel.m_szCategory);
	
		buf.Format(L"%d", IDKategorie);
		sWhere += "AND CD.IDKategorie = " + buf + " ";
	}
	
	IDMedium = 0;
	if (sel.m_szMedium != "")
		IDMedium = GetIDFromMedium(sel.m_szMedium);
	
	if (IDMedium > 0)
	{
		buf.Format(L"%d", IDMedium);
		sWhere += "AND CD.IDMedium = " + buf + " ";
	}
	
	if (Big3::Hitbase::Configuration::Settings::Current->SortArchiveNumberNumeric)
	{
		if (_wtoi(sel.m_sArchivNummerFrom) > 0)
		{
			buf.Format(L"%d", _wtoi(sel.m_sArchivNummerFrom));
			sWhere += "AND IIF(IsNumeric(CD.szArchivnummer),CLng(CD.szArchivnummer),0) >= " + buf + " ";
		}
		if (_wtoi(sel.m_sArchivNummerTo) > 0)
		{
			buf.Format(L"%d", _wtoi(sel.m_sArchivNummerTo));
			sWhere += "AND IIF(IsNumeric(CD.szArchivnummer),CLng(CD.szArchivnummer),0) <= " + buf + " ";
		}
	}
	else
	{
		if (!sel.m_sArchivNummerFrom.IsEmpty())
		{
			sWhere += "AND CD.szArchivNummer >= \"" + sel.m_sArchivNummerFrom + "\" ";
		}
		if (!sel.m_sArchivNummerTo.IsEmpty())
		{
			sWhere += "AND CD.szArchivNummer <= \"" + sel.m_sArchivNummerTo + "\" ";
		}
	}

	if (sel.m_szCodes != "")
		sWhere += "AND (" + GetWhereCodesString(sel.m_szCodes, FALSE) + ") ";
	
	if (sel.m_szCDComment != "")
		sWhere += "AND CD.szKommentar like \"" + sWildcard + CMisc::GetSqlString(sel.m_szCDComment) + sWildcard + "\" ";
	
	if (sel.m_szDateFrom != "")
		sWhere += "AND CD.szDatum >= \"" + DateLong2Short(sel.m_szDateFrom) + "\" ";
	
	if (sel.m_szDateTo != "")
		sWhere += "AND CD.szDatum <= \"" + DateLong2Short(sel.m_szDateTo) + "\" ";

	if (sel.m_sCopyright != "")
		sWhere += "AND CD.C_Copyright like \"" + sWildcard + CMisc::GetSqlString(sel.m_sCopyright) + sWildcard + "\" ";

	if (sel.m_lYearRecordedFrom > 0)
	{
		buf.Format(L"%d", sel.m_lYearRecordedFrom);
		sWhere += "AND CD.C_YearRecorded >= " + buf + " ";
	}

	if (sel.m_lYearRecordedTo > 0)
	{
		buf.Format(L"%d", sel.m_lYearRecordedTo);
		sWhere += "AND CD.C_YearRecorded <= " + buf + " ";
	}

	for (int i=0;i<MAX_USER_FIELDS;i++)
	{
		if (sel.m_szCDUser[i] != "")
		{
			str1.Format(L"AND CD.szFeld%d like \"%s%s%s\" ", i+1, sWildcard, CMisc::GetSqlString(sel.m_szCDUser[i]), sWildcard);
			sWhere += str1;
		}
	}

	// Version 10
	if (sel.m_iOriginalCD == 1)     // Nur Original-CDs
		sWhere += "AND CD.C_Original = TRUE ";

	if (sel.m_iOriginalCD == 2)     // Nur "Eigenkreationen"
		sWhere += "AND CD.C_Original = FALSE ";

	if (!sel.m_sLabel.IsEmpty())
		sWhere += "AND CD.C_Label like \"" + sWildcard + CMisc::GetSqlString(sel.m_sLabel) + sWildcard + "\" ";

	if (!sel.m_sURL.IsEmpty())
		sWhere += "AND CD.C_URL like \"" + sWildcard + CMisc::GetSqlString(sel.m_sURL) + sWildcard + "\" ";

	if (!sel.m_sUPC.IsEmpty())
		sWhere += "AND CD.C_UPC like \"" + sWildcard + CMisc::GetSqlString(sel.m_sUPC) + sWildcard + "\" ";

	if (sel.m_lRatingFrom > 0)
	{
		buf.Format(L"%d", sel.m_lRatingFrom);
		sWhere += "AND CD.C_Rating >= " + buf + " ";
	}

	if (sel.m_lRatingTo > 0)
	{
		buf.Format(L"%d", sel.m_lRatingTo);
		sWhere += "AND CD.C_Rating <= " + buf + " ";
	}

	if (sel.m_lPriceFrom > 0)
	{
		buf.Format(L"%d", sel.m_lPriceFrom);
		sWhere += "AND CD.C_Price >= " + buf + " ";
	}

	if (sel.m_lPriceTo > 0)
	{
		buf.Format(L"%d", sel.m_lPriceTo);
		sWhere += "AND CD.C_Price <= " + buf + " ";
	}

	if (!sel.m_sFrontCover.IsEmpty())
		sWhere += "AND CD.szPfadBitmap like \"" + sWildcard + CMisc::GetSqlString(sel.m_sFrontCover) + sWildcard + "\" ";

	if (!sel.m_sBackCover.IsEmpty())
		sWhere += "AND CD.C_BackCoverBitmap like \"" + sWildcard + CMisc::GetSqlString(sel.m_sBackCover) + sWildcard + "\" ";

	if (!sel.m_sCDLabel.IsEmpty())
		sWhere += "AND CD.C_CDLabelBitmap like \"" + sWildcard + CMisc::GetSqlString(sel.m_sCDLabel) + sWildcard + "\" ";

	// Version 11
	if (!sel.m_sLanguage.IsEmpty())
		sWhere += "AND CD.C_Language like \"" + sWildcard + CMisc::GetSqlString(sel.m_sLanguage) + sWildcard + "\" ";

	if (!sel.m_sLocation.IsEmpty())
		sWhere += "AND CD.C_Location like \"" + sWildcard + CMisc::GetSqlString(sel.m_sLocation) + sWildcard + "\" ";

	if (!sel.m_sComposer.IsEmpty())
		sWhere += "AND Composer.szArtistName like \"" + sWildcard + CMisc::GetSqlString(sel.m_sComposer) + sWildcard + "\" ";

	if (!sel.m_sParticipant.IsEmpty())
		sWhere += "AND (CD.IDCD in (SELECT P_IDCD FROM Participant INNER JOIN Artist ON Artist.IDArtist = Participant.P_IDArtist WHERE P_IDCD=CD.IDCD and Artist.szArtistName like \"" + sWildcard + CMisc::GetSqlString(sel.m_sParticipant) + sWildcard + "\")) ";

	if (bNoFirstAnd && !sWhere.IsEmpty())        // Das erste AND entfernen
		sWhere = sWhere.Mid(4);
	
	return sWhere;
}

// Liefert den Where-String für Abfragen von Liedern.
CString CDataBase::GetWhereStringTrack(const CSelection &sel, BOOL bSimple, BOOL bNoFirstAnd, BOOL bNoArtist)
{
	CString sWhere, buf;
	CString sWildcard;
	CString str1;
	
	if (bSimple || !sel.m_bExactMatch)
		sWildcard = "*";
	
	ASSERT(m_nNumberOfCategories >= 0);
	
	if (sel.m_szTrackTitle != "")
		sWhere += "AND Lied.szTitel like \"" + sWildcard + CMisc::GetSqlString(sel.m_szTrackTitle) + sWildcard + "\" ";
	
	if (sel.m_szTrackLyrics != "")
		sWhere += "AND Lied.szLiedtext like \"" + sWildcard + CMisc::GetSqlString(sel.m_szTrackLyrics) + sWildcard + "\" ";
	
	if (sel.m_szTrackArtist != "" && !bNoArtist)
		sWhere += "AND szArtistName like \"" + sWildcard + CMisc::GetSqlString(sel.m_szTrackArtist) + sWildcard + "\" ";
	
	if (sel.m_dwTrackLengthFrom > 0)
	{
		buf.Format(L"%d", sel.m_dwTrackLengthFrom);
		sWhere += "AND Lied.dwLaenge >= " + buf + " ";
	}
	
	if (sel.m_dwTrackLengthTo > 0)
	{
		buf.Format(L"%d", sel.m_dwTrackLengthTo + 999);
		sWhere += "AND Lied.dwLaenge <= " + buf + " ";
	}
	
	if (sel.m_wTrackBpmFrom > 0)
	{
		buf.Format(L"%d", sel.m_wTrackBpmFrom);
		sWhere += "AND Lied.wBpm >= " + buf + " ";
	}
	
	if (sel.m_wTrackBpmTo > 0)
	{
		buf.Format(L"%d", sel.m_wTrackBpmTo);
		sWhere += "AND Lied.wBpm <= " + buf + " ";
	}
	
	if (sel.m_szTrackCodes != "")
		sWhere += "AND (" + GetWhereCodesString(sel.m_szTrackCodes, TRUE) + ") ";
	
	if (sel.m_szTrackComment != "")
		sWhere += "AND Lied.szKommentar like \"" + sWildcard + CMisc::GetSqlString(sel.m_szTrackComment) + sWildcard + "\" ";
	
	for (int i=0;i<MAX_USER_FIELDS;i++)
	{
		if (sel.m_szTrackUser[i] != "")
		{
			str1.Format(L"AND Lied.szFeld%d like \"%s%s%s\" ", i+1, sWildcard, CMisc::GetSqlString(sel.m_szTrackUser[i]), sWildcard);
			sWhere += str1;
		}
	}

	if (sel.m_sTrackSoundFilename != "")
		sWhere += "AND Lied.szNameRecDatei like \"" + sWildcard + CMisc::GetSqlString(sel.m_sTrackSoundFilename) + sWildcard + "\" ";

	if (sel.m_lTrackYearRecordedFrom > 0)
	{
		buf.Format(L"%d", sel.m_lTrackYearRecordedFrom);
		sWhere += "AND Lied.L_YearRecorded >= " + buf + " ";
	}

	if (sel.m_lTrackYearRecordedTo > 0)
	{
		buf.Format(L"%d", sel.m_lTrackYearRecordedTo);
		sWhere += "AND Lied.L_YearRecorded <= " + buf + " ";
	}

	// Version 10
	if (sel.m_lTrackRatingFrom > 0)
	{
		buf.Format(L"%d", sel.m_lTrackRatingFrom);
		sWhere += "AND Lied.L_Rating >= " + buf + " ";
	}

	if (sel.m_lTrackRatingTo > 0)
	{
		buf.Format(L"%d", sel.m_lTrackRatingTo);
		sWhere += "AND Lied.L_Rating <= " + buf + " ";
	}
	
	// Version 11
	if (sel.m_szTrackLanguage != "")
		sWhere += "AND Lied.L_Language like \"" + sWildcard + CMisc::GetSqlString(sel.m_szTrackLanguage) + sWildcard + "\" ";

	int IDTrackKategorie = 0;
	if (sel.m_szTrackCategory != "")
	{
		IDTrackKategorie = GetIDFromCategory(sel.m_szTrackCategory);
	
		buf.Format(L"%d", IDTrackKategorie);
		sWhere += "AND Lied.L_IDCategory = " + buf + " ";
	}

	if (!sel.m_sTrackComposer.IsEmpty())
		sWhere += "AND ComposerTrack.szArtistName like \"" + sWildcard + CMisc::GetSqlString(sel.m_sTrackComposer) + sWildcard + "\" ";

	if (bNoFirstAnd && !sWhere.IsEmpty())        // Das erste AND entfernen
		sWhere = sWhere.Mid(4);
	
	return sWhere;
}

void CDataBase::SearchFirstPrefix(CEdit *pEditCtrl, int iType)
{
	CString sText, sRet;
	BOOL bRet;
	
	pEditCtrl->GetWindowText(sText);
	if (sText.GetLength() > 0)
	{
		switch (iType)
		{
		case 1:
			if (((CHitbaseWinAppBase*)AfxGetApp())->m_config.m_bAutoCompleteArtist)
				bRet = FindArtist(sText, sRet);
			else
				bRet = FALSE;
			break;
		case 2:
			if (((CHitbaseWinAppBase*)AfxGetApp())->m_config.m_bAutoCompleteCDTitle)
				bRet = FindCDTitle(sText, sRet);
			else
				bRet = FALSE;
			break;
		case 3:
			if (((CHitbaseWinAppBase*)AfxGetApp())->m_config.m_bAutoCompleteTrackname)
				bRet = FindTrackname(sText, sRet);
			else
				bRet = FALSE;
			break;
		default:
			ASSERT(false);
		}

		if (bRet == TRUE && sText != sRet)
		{
			pEditCtrl->SetWindowText(sRet);
			pEditCtrl->SetSel(sText.GetLength(), -1);
		}
	}
}

// Liefert die Kategorie der angegebenen ID zurück.
CString CDataBase::GetCategoryFromID(int iID)
{
	int i;
	
	if (!iID)
		return "";
	
	ASSERT(m_nNumberOfCategories >= 0);
	
	for (i=0;i<m_nNumberOfCategories && 
		m_IDCategories[i] != iID;i++);
	
	if (m_IDCategories[i] == iID)
		return m_Categories[i];
	else
		return "";
}

// Liefert das Medium der angegebenen ID zurück.
CString CDataBase::GetMediumFromID(int iID)
{
	int i;
	
	if (!iID)
		return "";
	
	ASSERT(m_nNumberOfMedia >= 0);
	
	for (i=0;i<m_nNumberOfMedia && 
		m_IDMedium[i] != iID;i++);
	
	if (m_IDMedium[i] == iID)
		return m_Medium[i];
	else
		return "";
}

BOOL CDataBase::ChooseFields(CFieldList& FieldList, CWnd* pParent)
{
	CChooseFieldsDlg ChooseFieldsDlg(this, pParent);

	ChooseFieldsDlg.SetFields(FieldList);
	ChooseFieldsDlg.SetDefault(FieldList.GetStandardFields());

	if (ChooseFieldsDlg.DoModal() == IDOK)
	{
		FieldList = ChooseFieldsDlg.m_FieldList;

		return TRUE;
	}

	return FALSE;
}

// Liest die Version der Hitbase-Datenbank aus.
BOOL CDataBase::ReadDatabaseVersion()
{
/*	CDBQuery DBQuery(this);

	if (!DBQuery.QueryStartFreeSQL("SELECT wVersion FROM [Master]"))
		return FALSE;

	if (DBQuery.QueryFindFirst())
		m_iDatabaseVersion = DBQuery.GetFieldValue("wVersion").iVal;

	DBQuery.QueryEnd();

	return m_iDatabaseVersion != 0;*/

	CDaoRecordset rs(&m_db);
	rs.Open(dbOpenDynaset, L"SELECT wVersion FROM [Master]", 0);
	if (!rs.IsEOF())
		m_iDatabaseVersion = rs.GetFieldValue(L"wVersion").iVal;
	rs.Close();

	return TRUE;
}

// Liefert die Version der Hitbase-Datenbank zurück.
int CDataBase::GetVersion()
{
	return m_iDatabaseVersion;
}

// Legt die angegebene Kategorie an, wenn sie noch nicht vorhanden ist.
BOOL CDataBase::CreateCategory(const CString &sCategory, BOOL bAsk)
{
	if (!sCategory.IsEmpty() && !GetIDFromCategory(sCategory))
	{
		CString str;
		str.Format(get_string(IDS_CREATE_CATEGORY), sCategory);
		
		BOOL bRet = AfxMessageBox(str, MB_YESNOCANCEL|MB_ICONQUESTION);
		if (bRet == IDCANCEL)
			return FALSE;
		if (bRet == IDYES)
			AddCategory(sCategory);
	}

	return TRUE;
}

// Legt das angegebene Medium an, wenn es noch nicht vorhanden ist.
BOOL CDataBase::CreateMedium(const CString &sMedium, BOOL bAsk)
{
	if (!sMedium.IsEmpty() && !GetIDFromMedium(sMedium))
	{
		CString str;
		str.Format(get_string(IDS_CREATE_MEDIUM), sMedium);
		
		BOOL bRet = AfxMessageBox(str, MB_YESNOCANCEL|MB_ICONQUESTION);
		if (bRet == IDCANCEL)
			return FALSE;
		if (bRet == IDYES)
			AddMedium(sMedium);
	}

	return TRUE;
}

// Importiert einen CDArchiv-Snapshot in die aktuelle Datenbank
BOOL CDataBase::ImportCDArchivSnapshot(const CString &sFilename, CProgressCtrl& ctlProgress, CStatic& stcStatus)
{
	CDaoDatabase dbCDArchiv;

	m_StopSearchProcess = FALSE;

	try
	{
		dbCDArchiv.Open(sFilename);
	}
	catch (CDaoException* e)
	{
		e->ReportError();
		e->Delete();
		return FALSE;
	}

	CDaoRecordset rs(&dbCDArchiv);
	CString sSQL;

	// Zuerst die Anzahl der CDs ermitteln.
	sSQL = "SELECT count(*) from CD";
	rs.Open(dbOpenDynaset, sSQL);

	long lCDCount;
	lCDCount = rs.GetFieldValue(0).lVal;

	rs.Close();
	
	sSQL = "SELECT CD.*, Track.*, Artist.*, ArtistTrack.* FROM ((CD INNER JOIN Track ON CD.ID = Track.IDCD) INNER JOIN Artist ON CD.IDArtist = Artist.ID) INNER JOIN Artist AS ArtistTrack ON Track.IDArtist = ArtistTrack.ID";

	rs.Open(dbOpenDynaset, sSQL);

	ctlProgress.SetRange32(0, lCDCount);

	int iCount=0;
	while (!rs.IsEOF() && !m_StopSearchProcess)
	{
		DWORD dwCDID;

		CCD CD(this);

		dwCDID = rs.GetFieldValue(L"CD.ID").lVal;

		CD.GetIdentity()->m_sIdentity = rs.GetFieldValue(L"sIdentity").bstrVal;
		CD.GetArtist()->m_sArtist = rs.GetFieldValue(L"Artist.sArtist").bstrVal;
		CD.m_sTitle = rs.GetFieldValue(L"CD.sTitle").bstrVal;
		CD.m_dwTotalLength = rs.GetFieldValue(L"dwTotalLength").lVal;
		CD.m_bSampler = rs.GetFieldValue(L"bCDSampler").cVal;
		CD.GetCategory()->m_sCategory = rs.GetFieldValue(L"sCategory").bstrVal;
		CD.GetMedium()->m_sMedium = rs.GetFieldValue(L"sMedium").bstrVal;
		CD.m_sComment = rs.GetFieldValue(L"CD.sComment").bstrVal;
		// Zuerst die Anzahl der Lieder ermitteln und soviele Einträge lesen
		int iNumberOfTracks = rs.GetFieldValue(L"cNumberOfTracks").cVal;
		CD.m_wNumberOfTracks = iNumberOfTracks;
		for (int i=0;i<iNumberOfTracks;i++)
		{
			int iTrackNumber = rs.GetFieldValue(L"wTrackNumber").lVal-1;

			CD.GetTrack(iTrackNumber)->GetArtist()->m_sArtist = rs.GetFieldValue(L"ArtistTrack.sArtist").bstrVal;
			CD.GetTrack(iTrackNumber)->m_sTitle = rs.GetFieldValue(L"Track.sTitle").bstrVal;
			CD.GetTrack(iTrackNumber)->m_dwLength = rs.GetFieldValue(L"dwLength").lVal;
			CD.GetTrack(iTrackNumber)->m_wBpm = rs.GetFieldValue(L"wBpm").iVal;
			CD.GetTrack(iTrackNumber)->m_sComment = rs.GetFieldValue(L"Track.sComment").bstrVal;

			rs.MoveNext();

			// Sicherheitsabfrage! Nicht alle Tracks in der Datenbank -> Neue CD
			if (rs.IsEOF() || dwCDID != (DWORD)rs.GetFieldValue(L"CD.ID").lVal)
				break;
		}

		CD.Add();

		CString str;
		str.Format(L"%d von %d", iCount+1, lCDCount);
		stcStatus.SetWindowText(str);
		ctlProgress.SetPos(iCount++);

		MSG msg;
		while (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE))
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
	}

	rs.Close();

	dbCDArchiv.Close();

	return TRUE;
}

// Liefert den Namen des Datenbankfeldes zurück.
CString CDataBase::GetDatabaseFieldName(UINT uiField)
{
	CString str;

	switch (uiField)
	{
	case FIELD_TOTALLENGTH:
		str = "CD.dwGesamtLaenge";
		break;
	case FIELD_NUMBEROFTRACKS:
		str = "CD.cAnzahlLieder";
		break;
	case FIELD_CDSET:
		str = "szCDSetName";
		break;
	case FIELD_CDSAMPLER:
		str = "CD.bCDSampler";
		break;
	case FIELD_CDNAME:			//!!!!!!!!!!!!!!!!!hm..... Sortierung nach Namen?? Muß dann hier nicht der Name hin?
		str = "Artist.sSortKey";
		break;
	case FIELD_CDTITLE:
		str = "CD.szTitel";
		break;
	case FIELD_CATEGORY:
		str = "Kategorie.szKategorieName";
		break;
	case FIELD_DATE:
		str = "CD.szDatum";
		break;
	case FIELD_CODES:
		str = "CD.szKennzeichen";
		break;
	case FIELD_CDCOMMENT:
		str = "CD.szKommentar";
		break;
	case FIELD_ARCHIVNUMMER:
		str = "CD.szArchivNummer";
		break;
	case FIELD_MEDIUM:
		str = "Medium.szMedium";
		break;
	case FIELD_YEAR_RECORDED:
		str = "CD.C_YearRecorded";
		break;
	case FIELD_COPYRIGHT:
		str = "CD.C_Copyright";
		break;
	case FIELD_CDCOVER_FILENAME:
		str = "CD.szPfadBitmap";
		break;
	case FIELD_CDCOVERBACK_FILENAME:
		str = "C_BackCoverBitmap";
		break;
	case FIELD_CDCOVERLABEL_FILENAME:
		str = "C_CDLabelBitmap";
		break;
	case FIELD_ORIGINAL_CD:
		str = "C_Original";
		break;
	case FIELD_LABEL:
		str = "C_Label";
		break;
	case FIELD_UPC:
		str = "C_UPC";
		break;
	case FIELD_URL:
		str = "C_URL";
		break;
	case FIELD_RATING:
		str = "C_Rating";
		break;
	case FIELD_PRICE:
		str = "C_Price";
		break;
	case FIELD_LANGUAGE:
		str = "C_Language";
		break;
	case FIELD_LOCATION:
		str = "C_Location";
		break;

	case FIELD_CDUSER1:
		str = "CD.szFeld1";
		break;
	case FIELD_CDUSER2:
		str = "CD.szFeld2";
		break;
	case FIELD_CDUSER3:
		str = "CD.szFeld3";
		break;
	case FIELD_CDUSER4:
		str = "CD.szFeld4";
		break;
	case FIELD_CDUSER5:
		str = "CD.szFeld5";
		break;
	case FIELD_TRACK_NUMBER:
		str = "lied.wLiedNummer";
		break;
	case FIELD_TRACK_ARTIST:
		str = "ArtistTrack.sSortKey";
		break;
	case FIELD_TRACK_TITLE:
		str = "lied.szTitel";
		break;
	case FIELD_TRACK_LENGTH:
		str = "lied.dwLaenge";
		break;
	case FIELD_TRACK_BPM:
		str = "lied.wBpm";
		break;
	case FIELD_TRACK_CODES:
		str = "lied.szKennzeichen";
		break;
	case FIELD_TRACK_COMMENT:
		str = "lied.szKommentar";
		break;
	case FIELD_TRACK_YEAR_RECORDED:
		str = "lied.L_YearRecorded";
		break;
	case FIELD_TRACK_USER1:
		str = "Lied.szFeld1";
		break;
	case FIELD_TRACK_USER2:
		str = "Lied.szFeld2";
		break;
	case FIELD_TRACK_USER3:
		str = "Lied.szFeld3";
		break;
	case FIELD_TRACK_USER4:
		str = "Lied.szFeld4";
		break;
	case FIELD_TRACK_USER5:
		str = "Lied.szFeld5";
		break;
	case FIELD_TRACK_LYRICS:
		str = "lied.szLiedtext";
		break;
	case FIELD_TRACK_SOUNDFILE:
		str = "lied.szNameRecDatei";
		break;
	case FIELD_TRACK_RATING:
		str = "lied.L_Rating";
		break;
	case FIELD_TRACK_CHECKSUM:
		str = "lied.L_CheckSum";
		break;
	case FIELD_TRACK_LANGUAGE:
		str = "lied.L_Language";
		break;
	case FIELD_TRACK_CATEGORY:
		str = "KategorieTrack.szKategorieName";
		break;

	case FIELD_ARTIST_CD_SORTKEY:
		str = "Artist.sSortKey";
		break;
	case FIELD_ARTIST_CD_GROUPTYPE:
		str = "Artist.nGroup";
		break;
	case FIELD_ARTIST_CD_SEX:
		str = "Artist.nSex";
		break;
	case FIELD_ARTIST_CD_COMMENT:
		str = "Artist.sComment";
		break;
	case FIELD_ARTIST_CD_URL:
		str = "Artist.A_URL";
		break;
	case FIELD_ARTIST_CD_COUNTRY:
		str = "Artist.A_Country";
		break;
	case FIELD_ARTIST_CD_BIRTHDAY:
		str = "Artist.A_BirthDay";
		break;
	case FIELD_ARTIST_CD_DAYOFDEATH:
		str = "Artist.A_DayOfDeath";
		break;
	case FIELD_ARTIST_CD_IMAGEFILENAME:
		str = "Artist.A_ImageFilename";
		break;

	case FIELD_ARTIST_TRACK_NAME:
		str = "ArtistTrack.szArtistName";
		break;
	case FIELD_ARTIST_TRACK_SORTKEY:
		str = "ArtistTrack.sSortKey";
		break;
	case FIELD_ARTIST_TRACK_GROUPTYPE:
		str = "ArtistTrack.nGroup";
		break;
	case FIELD_ARTIST_TRACK_SEX:
		str = "ArtistTrack.nSex";
		break;
	case FIELD_ARTIST_TRACK_COMMENT:
		str = "ArtistTrack.sComment";
		break;
	case FIELD_ARTIST_TRACK_URL:
		str = "ArtistTrack.A_URL";
		break;
	case FIELD_ARTIST_TRACK_COUNTRY:
		str = "ArtistTrack.A_Country";
		break;
	case FIELD_ARTIST_TRACK_BIRTHDAY:
		str = "ArtistTrack.A_BirthDay";
		break;
	case FIELD_ARTIST_TRACK_DAYOFDEATH:
		str = "ArtistTrack.A_DayOfDeath";
		break;
	case FIELD_ARTIST_TRACK_IMAGEFILENAME:
		str = "ArtistTrack.A_ImageFilename";
		break;

	case FIELD_COMPOSER_CD_NAME:
		str = "Composer.szArtistName";
		break;
	case FIELD_COMPOSER_CD_SORTKEY:
		str = "Composer.sSortKey";
		break;
	case FIELD_COMPOSER_CD_GROUPTYPE:
		str = "Composer.nGroup";
		break;
	case FIELD_COMPOSER_CD_SEX:
		str = "Composer.nSex";
		break;
	case FIELD_COMPOSER_CD_COMMENT:
		str = "Composer.sComment";
		break;
	case FIELD_COMPOSER_CD_URL:
		str = "Composer.A_URL";
		break;
	case FIELD_COMPOSER_CD_COUNTRY:
		str = "Composer.A_Country";
		break;
	case FIELD_COMPOSER_CD_BIRTHDAY:
		str = "Composer.A_BirthDay";
		break;
	case FIELD_COMPOSER_CD_DAYOFDEATH:
		str = "Composer.A_DayOfDeath";
		break;
	case FIELD_COMPOSER_CD_IMAGEFILENAME:
		str = "Composer.A_ImageFilename";
		break;

	case FIELD_COMPOSER_TRACK_NAME:
		str = "ComposerTrack.szArtistName";
		break;
	case FIELD_COMPOSER_TRACK_SORTKEY:
		str = "ComposerTrack.sSortKey";
		break;
	case FIELD_COMPOSER_TRACK_GROUPTYPE:
		str = "ComposerTrack.nGroup";
		break;
	case FIELD_COMPOSER_TRACK_SEX:
		str = "ComposerTrack.nSex";
		break;
	case FIELD_COMPOSER_TRACK_COMMENT:
		str = "ComposerTrack.sComment";
		break;
	case FIELD_COMPOSER_TRACK_URL:
		str = "ComposerTrack.A_URL";
		break;
	case FIELD_COMPOSER_TRACK_COUNTRY:
		str = "ComposerTrack.A_Country";
		break;
	case FIELD_COMPOSER_TRACK_BIRTHDAY:
		str = "ComposerTrack.A_BirthDay";
		break;
	case FIELD_COMPOSER_TRACK_DAYOFDEATH:
		str = "ComposerTrack.A_DayOfDeath";
		break;
	case FIELD_COMPOSER_TRACK_IMAGEFILENAME:
		str = "ComposerTrack.A_ImageFilename";
		break;

	default:
		ASSERT(false);
		break;
	}

	return str;
}

// Liefert den Typ des Datenbankfeldes zurück.
CDataBase::enumFieldType CDataBase::GetDatabaseFieldType(UINT uiField)
{
	enumFieldType fieldType;

	switch (uiField)
	{
	case FIELD_TOTALLENGTH:
		fieldType = fieldTypeLong;
		break;
	case FIELD_NUMBEROFTRACKS:
		fieldType = fieldTypeByte;
		break;
	case FIELD_CDSET:
		fieldType = fieldTypeString;
		break;
	case FIELD_CDSAMPLER:
		fieldType = fieldTypeBoolean;
		break;
	case FIELD_CDNAME:
		fieldType = fieldTypeString;
		break;
	case FIELD_COMPOSER_CD_NAME:
		fieldType = fieldTypeString;
		break;
	case FIELD_CDTITLE:
		fieldType = fieldTypeString;
		break;
	case FIELD_CATEGORY:
		fieldType = fieldTypeString;
		break;
	case FIELD_DATE:
		fieldType = fieldTypeString;
		break;
	case FIELD_CODES:
		fieldType = fieldTypeString;
		break;
	case FIELD_CDCOMMENT:
		fieldType = fieldTypeString;
		break;
	case FIELD_ARCHIVNUMMER:
		fieldType = fieldTypeString;
		break;
	case FIELD_MEDIUM:
		fieldType = fieldTypeString;
		break;
	case FIELD_YEAR_RECORDED:
		fieldType = fieldTypeLong;
		break;
	case FIELD_COPYRIGHT:
		fieldType = fieldTypeString;
		break;
	case FIELD_CDCOVER_FILENAME:
		fieldType = fieldTypeString;
		break;
	case FIELD_CDCOVERBACK_FILENAME:
		fieldType = fieldTypeString;
		break;
	case FIELD_CDCOVERLABEL_FILENAME:
		fieldType = fieldTypeString;
		break;
	case FIELD_ORIGINAL_CD:
		fieldType = fieldTypeBoolean;
		break;
	case FIELD_LABEL:
		fieldType = fieldTypeString;
		break;
	case FIELD_UPC:
		fieldType = fieldTypeString;
		break;
	case FIELD_URL:
		fieldType = fieldTypeString;
		break;
	case FIELD_RATING:
		fieldType = fieldTypeLong;
		break;
	case FIELD_PRICE:
		fieldType = fieldTypeLong;
		break;
	case FIELD_LANGUAGE:
		fieldType = fieldTypeString;
		break;
	case FIELD_LOCATION:
		fieldType = fieldTypeString;
		break;
	case FIELD_CDUSER1:
		fieldType = fieldTypeString;
		break;
	case FIELD_CDUSER2:
		fieldType = fieldTypeString;
		break;
	case FIELD_CDUSER3:
		fieldType = fieldTypeString;
		break;
	case FIELD_CDUSER4:
		fieldType = fieldTypeString;
		break;
	case FIELD_CDUSER5:
		fieldType = fieldTypeString;
		break;
	case FIELD_TRACK_NUMBER:
		fieldType = fieldTypeShort;
		break;
	case FIELD_TRACK_ARTIST:
		fieldType = fieldTypeString;
		break;
	case FIELD_TRACK_TITLE:
		fieldType = fieldTypeString;
		break;
	case FIELD_TRACK_LENGTH:
		fieldType = fieldTypeLong;
		break;
	case FIELD_TRACK_BPM:
		fieldType = fieldTypeShort;
		break;
	case FIELD_TRACK_CODES:
		fieldType = fieldTypeString;
		break;
	case FIELD_TRACK_COMMENT:
		fieldType = fieldTypeString;
		break;
	case FIELD_TRACK_YEAR_RECORDED:
		fieldType = fieldTypeLong;
		break;
	case FIELD_TRACK_RATING:
		fieldType = fieldTypeLong;
		break;
	case FIELD_TRACK_CHECKSUM:
		fieldType = fieldTypeString;
		break;
	case FIELD_TRACK_USER1:
		fieldType = fieldTypeString;
		break;
	case FIELD_TRACK_USER2:
		fieldType = fieldTypeString;
		break;
	case FIELD_TRACK_USER3:
		fieldType = fieldTypeString;
		break;
	case FIELD_TRACK_USER4:
		fieldType = fieldTypeString;
		break;
	case FIELD_TRACK_USER5:
		fieldType = fieldTypeString;
		break;
	case FIELD_TRACK_LYRICS:
		fieldType = fieldTypeString;
		break;
	case FIELD_TRACK_SOUNDFILE:
		fieldType = fieldTypeString;
		break;
	case FIELD_COMPOSER_TRACK_NAME:
		fieldType = fieldTypeString;
		break;
	default:
		ASSERT(false);
	}

	return fieldType;
}

CString CDataBase::GetFilterWhereString(const CSelection& sel)
{
	CString sWhere = sel.GetFilterString(CSelection::filterTableAll);

	return sWhere;
}

CString CDataBase::GetFilterWhereStringTable(const CSelection& sel, CSelection::enumFilterTable filterTable)
{
	CString sWhere = sel.GetFilterString(filterTable);

	return sWhere;
}

// Feldübertragung: Den Inhalt eines Feldes in ein anderes kopieren oder verschieben
// Liefert die Anzahl der aufgetretenen Fehler zurück, oder -1, wenn ein allgemeiner Fehler aufgetreten ist.
int CDataBase::TransferFields(int iSourceField, int iTargetField, bool bMoveFields, CProgressCtrl* pProgressCtrl, CStatic* pstcText, CSelection* pSelection)
{
	bool bCDField = CFieldList::IsCDField(iSourceField);
	CString sTableName;
	CString sSourceField = GetDatabaseFieldName(iSourceField);
	CString sTargetField = GetDatabaseFieldName(iTargetField);
	int iErrorCount=0;
	int iTotalCount=0;
	int iCount=0;

	if (bCDField)
	{
		CCDQuery CDQuery(this);

		// Nur Selectionen der CD
		pSelection->ResetTrack();
		pSelection->ResetCDArtist();

		BOOL bFound = CDQuery.QueryStartCDOnly(pSelection, pProgressCtrl, FALSE);

		iTotalCount = CDQuery.GetCount();

		while (bFound && !m_StopSearchProcess)
		{
			bool bError = false;

			CCD CD(this);
			CD.GetRecordFromID(CDQuery.m_dwID);
			
			COleVariant var = CD.GetField(iSourceField);
			// Konvertierung durchführen
			COleVariant newVar;
			try
			{
				newVar.ChangeType(CD.GetField(iTargetField).vt, &var);
			}
			catch (CException *e)
			{
				// Nur merken, dass ein Fehler aufgetreten ist und am Schluß melden, 
				// da sonst viel zu viele Fehlermeldungen kommen.
				e->Delete();
				bError = true;
				iErrorCount++;
			}

			if (!bError)
			{
				CD.SetField(iTargetField, newVar);
				if (bMoveFields)
				{
					COleVariant varEmpty;
					CD.SetField(iSourceField, varEmpty);
				}
			}

			CD.Write();

			bFound = CDQuery.QueryFindNext();

			iCount++;
			if (!(iCount % 32))
			{
				CString sText;
				sText.Format(get_string(IDS_CONVERTTEXT), iCount, iTotalCount);
				pstcText->SetWindowText(sText);
			}
		}

		CDQuery.QueryEnd();
	}
	else
	{
		CCDQuery TrackQuery(this);

		// Nur Selectionen der CD
		pSelection->ResetCD();
		pSelection->ResetTrackArtist();

		BOOL bFound = TrackQuery.QueryStartTrack(pSelection, pProgressCtrl, FALSE);

		iTotalCount = TrackQuery.GetCount();

		while (bFound && !m_StopSearchProcess)
		{
			bool bError = false;

			CTrack Track(this);
			Track.GetRecordFromID(TrackQuery.m_Track[0].m_dwID);

			COleVariant var = Track.GetField(iSourceField);
			// Konvertierung durchführen
			COleVariant newVar;
			try
			{
				newVar.ChangeType(Track.GetField(iTargetField).vt, &var);
			}
			catch (CException *e)
			{
				// Nur merken, dass ein Fehler aufgetreten ist und am Schluß melden, 
				// da sonst viel zu viele Fehlermeldungen kommen.
				e->Delete();
				bError = true;
				iErrorCount++;
			}

			if (!bError)
			{
				Track.SetField(iTargetField, newVar);
				if (bMoveFields)
				{
					COleVariant varEmpty;
					Track.SetField(iSourceField, varEmpty);
				}
			}

			Track.Write();

			bFound = TrackQuery.QueryFindNext();

			iCount++;
			if (!(iCount % 32))
			{
				CString sText;
				sText.Format(get_string(IDS_CONVERTTRACKTEXT), iCount, iTotalCount);
				pstcText->SetWindowText(sText);
			}
		}

		TrackQuery.QueryEnd();
	}

	return iErrorCount;
}

void CDataBase::FillUserFieldListCtrl(CGridListCtrl* pListCtrl, CCD* pCD, int iTrack)
{
	CRect rect;
	pListCtrl->GetClientRect(&rect);
	pListCtrl->InsertColumn(0, get_string(TEXT_FIELD), LVCFMT_LEFT, 100);
	pListCtrl->InsertColumn(1, get_string(TEXT_VALUE), LVCFMT_LEFT, rect.right-100);

	pListCtrl->SetColumnFlags(0, GLCCF_GRAYFIELDS);
	pListCtrl->SetColumnFlags(1, GLCCF_EDIT);

	for (int i=0;i<MAX_USER_FIELDS;i++)
	{
		if (iTrack >= 0)
		{
			if (!m_MasterTable.m_sTrackUserField[i].IsEmpty())
			{
				int iIndex = pListCtrl->InsertItem(i, m_MasterTable.m_sTrackUserField[i]);
				pListCtrl->SetItem(iIndex, 1, LVIF_TEXT, pCD->GetTrack(iTrack)->m_sUser[i], 0, 0, 0, 0);
				pListCtrl->SetCellFormat(iIndex, 1, m_MasterTable.m_cTypeTrackUserField[i]);
			}
		}
		else
		{
			if (!m_MasterTable.m_sCDUserField[i].IsEmpty())
			{
				int iIndex = pListCtrl->InsertItem(i, m_MasterTable.m_sCDUserField[i]);
				pListCtrl->SetItem(iIndex, 1, LVIF_TEXT, pCD->m_sUser[i], 0, 0, 0, 0);
				pListCtrl->SetCellFormat(iIndex, 1, m_MasterTable.m_cTypeCDUserField[i]);
			}
		}
	}
}

void CDataBase::FillFieldsInListbox(CFieldList* pFieldList, CListBox* pListbox)
{
	CString sFieldName;
	int iDummy;

	for (int i=0;i<pFieldList->GetCount();i++)
	{
		sFieldName = GetFieldName(pFieldList->GetAt(i), iDummy);
		int iIndex = pListbox->AddString(sFieldName);
		pListbox->SetItemData(iIndex, pFieldList->GetAt(i));
	}
}

CString CDataBase::GetCoverFolder(void)
{
    CString path = Big3::Hitbase::Configuration::Settings::Current->DefaultCDCoverPath;

    if (path.IsEmpty())		// Wenn nichts definiert ist, nehmen wir das Images-Unterverzeichnis des Katalogs
	{
		path = GetPathFromFileName(GetDataBasePath());
		path = CMisc::CombinePathWithFileName(path, "Images");
	}

	CMisc::CreateDirectory(path);

    return path;
}

bool CDataBase::Compress()
{
	try
	{
		Close();
		CString sFilename = this->GetDataBasePath();
		CString sTempFileName = CMisc::GetTempFilename();

		// Da die Datei direkt angelegt wird.... wollen wir hier aber nicht
		::DeleteFile(sTempFileName);

		CDaoWorkspace::CompactDatabase(sFilename, sTempFileName);

		Open(sFilename);

		return true;
	}
	catch (CException* e)
	{
		e->ReportError();
		e->Delete();
		return false;
	}
}

