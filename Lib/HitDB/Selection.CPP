// hitsel.cpp : implementation file
//

#include "stdafx.h"
#include "HitDB.h"
#include "Selection.h"
#include "resource.h"
#include "..\..\app\hitbase\resource.h"
#include "..\hitmisc\hitbasewinappbase.h"

/////////////////////////////////////////////////////////////////////////////
// CSelection

CSelection::CSelection()
{
	Reset();

	m_SortKeys.SetSize(3);

	// Standardsortierung
	m_SortKeys[0] = FIELD_CDNAME;
	m_SortKeys[1] = FIELD_CDTITLE;
	m_SortKeys[2] = 0;

	m_bSortKeysDirection[0] = TRUE;
	m_bSortKeysDirection[1] = TRUE;
	m_bSortKeysDirection[2] = TRUE;
}

CSelection::~CSelection()
{
}

// Liefert den Selektions-String zurück
CString CSelection::GetSelectionString()
{
	CString str;
	CString newstr;
	CString sContains;

	if (m_bExactMatch)
		sContains = "=";
	else
		sContains = " " + get_string(IDS_CONTAINS) + " ";
	
	if (m_wCDType == 1)        // Keine Sampler
		str += get_string(IDS_NOSAMPLER) + ", ";
	
	if (m_wCDType == 2)        // Nur Sampler
		str += get_string(IDS_ONLYSAMPLER) + ", ";
	
	if (m_szCDArtist != "")
	{
		newstr.Format(L"%s%s'%s', ", get_string(IDS_ARTIST), sContains, m_szCDArtist);
		str += newstr;
	}
	if (m_szCDTitle != "")
	{
		newstr.Format(L"%s%s'%s', ", get_string(IDS_TITLE), sContains, m_szCDTitle);
		str += newstr;
	}
	
	if (m_dwTotalLengthFrom || m_dwTotalLengthTo)
	{
		CString zwi;
		zwi = CMisc::GetTime(m_dwTotalLengthFrom);
		newstr.Format(L"%s='%s-%s', ", get_string(IDS_TOTALLENGTH), zwi, CMisc::GetTime(m_dwTotalLengthTo));
		str += newstr;
	}
	
	if (m_wNumberOfTracksFrom || m_wNumberOfTracksTo)
	{
		newstr.Format(L"%s='%d-%d', ", get_string(IDS_NUMBEROFTRACKS), m_wNumberOfTracksFrom, m_wNumberOfTracksTo);
		str += newstr;
	}
	
	if (m_szTrackTitle != "")
	{
		newstr.Format(L"%s%s'%s', ", get_string(IDS_NAMEOFTRACK), sContains, m_szTrackTitle);
		str += newstr;
	}
	
	if (m_szDateFrom != "" || m_szDateTo != "")
	{
		newstr.Format(L"%s='%s-%s', ", CString(DataBase->Master->DateName), m_szDateFrom, m_szDateTo);
		str += newstr;
	}
	
	if (m_szCategory != "")
	{
		newstr.Format(L"%s='%s', ", get_string(IDS_CATEGORY), m_szCategory);
		str += newstr;
	}
	
	if (m_szMedium != "")
	{
		newstr.Format(L"%s='%s', ", get_string(IDS_MEDIUM), m_szMedium);
		str += newstr;
	}
	
	if (m_szCodes != "")
	{
		newstr.Format(L"%s='%s', ", get_string(IDS_CODES), m_szCodes);
		str += newstr;
	}
	
	if (m_sArchivNummerFrom != "" || m_sArchivNummerTo != "")
	{
		newstr.Format(L"%s='%s-%s', ", get_string(IDS_ARCHIVNUMMER), m_sArchivNummerFrom, m_sArchivNummerTo);
		str += newstr;
	}
	
	if (m_szCDComment != "")
	{
		newstr.Format(L"%s%s'%s', ", get_string(IDS_COMMENT), sContains, m_szCDComment);
		str += newstr;
	}

	if (m_sCopyright != "")
	{
		newstr.Format(L"%s%s'%s', ", get_string(IDS_COPYRIGHT), sContains, m_sCopyright);
		str += newstr;
	}

	if (m_lYearRecordedFrom || m_lYearRecordedTo)
	{
		newstr.Format(L"%s='%d-%d', ", get_string(IDS_YEAR_RECORDED), m_lYearRecordedFrom, m_lYearRecordedTo);
		str += newstr;
	}
	
	for (int i=0;i<MAX_USER_FIELDS;i++)
	{
		if (m_szCDUser[i] != "")
		{
			newstr.Format(L"%s%s'%s', ", CString(DataBase->Master->UserCDFields[i]->Name), sContains, m_szCDUser[i]);
			str += newstr;
		}
	}
	
	// Version 10
	if (m_iOriginalCD == 1)     // Nur Original-CDs
		str += get_string(IDS_ONLY_ORIGINAL_CD) + ", ";

	if (m_iOriginalCD == 2)     // Nur "Eigenkreationen"
		str += get_string(IDS_ONLY_NO_ORIGINAL_CD) + ", ";

	if (!m_sLabel.IsEmpty())
	{
		newstr.Format(L"%s%s'%s', ", get_string(IDS_LABEL), sContains, m_sLabel);
		str += newstr;
	}

	if (!m_sURL.IsEmpty())
	{
		newstr.Format(L"%s%s'%s', ", get_string(IDS_URL), sContains, m_sURL);
		str += newstr;
	}

	if (!m_sUPC.IsEmpty())
	{
		newstr.Format(L"%s%s'%s', ", get_string(IDS_UPC), sContains, m_sUPC);
		str += newstr;
	}

	if (m_lRatingFrom || m_lRatingTo)
	{
		newstr.Format(L"%s='%d-%d', ", get_string(IDS_RATING), m_lRatingFrom, m_lRatingTo);
		str += newstr;
	}
	
	if (m_lPriceFrom || m_lPriceTo)
	{
		newstr.Format(L"%s='%s-%s', ", get_string(IDS_PRICE), CMisc::FormatCurrencyValue(m_lPriceFrom), CMisc::FormatCurrencyValue(m_lPriceTo));
		str += newstr;
	}

	if (!m_sFrontCover.IsEmpty())
	{
		newstr.Format(L"%s%s'%s', ", get_string(IDS_FRONT_COVER), sContains, m_sFrontCover);
		str += newstr;
	}

	if (!m_sBackCover.IsEmpty())
	{
		newstr.Format(L"%s%s'%s', ", get_string(IDS_BACK_COVER), sContains, m_sBackCover);
		str += newstr;
	}

	if (!m_sCDLabel.IsEmpty())
	{
		newstr.Format(L"%s%s'%s', ", get_string(IDS_CD_LABEL), sContains, m_sCDLabel);
		str += newstr;
	}

	// Version 11
	if (!m_sComposer.IsEmpty())
	{
		newstr.Format(L"%s%s'%s', ", get_string(IDS_COMPOSER), sContains, m_sComposer);
		str += newstr;
	}

	if (!m_sLanguage.IsEmpty())
	{
		newstr.Format(L"%s%s'%s', ", get_string(IDS_LANGUAGE), sContains, m_sLanguage);
		str += newstr;
	}

	if (!m_sLocation.IsEmpty())
	{
		newstr.Format(L"%s%s'%s', ", get_string(IDS_LOCATION), sContains, m_sLocation);
		str += newstr;
	}

	if (!m_sParticipant.IsEmpty())
	{
		newstr.Format(L"%s%s'%s', ", get_string(IDS_PARTICIPANT), sContains, m_sParticipant);
		str += newstr;
	}


	if (m_szTrackArtist != "")
	{
		newstr.Format(L"%s%s'%s', ", get_string(IDS_TRACKARTIST), sContains, m_szTrackArtist);
		str += newstr;
	}
	
	if (m_dwTrackLengthFrom || m_dwTrackLengthTo)
	{
		CString zwi;
		
		zwi = CMisc::GetTime(m_dwTrackLengthFrom);
		newstr.Format(L"%s='%s-%s', ", get_string(IDS_LENGTHOFTRACK), zwi, CMisc::GetTime(m_dwTrackLengthTo));
		str += newstr;
	}
	
	if (m_wTrackBpmFrom || m_wTrackBpmTo)
	{
		newstr.Format(L"%s='%d-%d', ", get_string(IDS_BPM), m_wTrackBpmFrom, m_wTrackBpmTo);
		str += newstr;
	}
	
	if (m_szTrackCodes != "")
	{
		newstr.Format(L"%s='%s', ", get_string(IDS_TRACKCODES), m_szTrackCodes);
		str += newstr;
	}
	
	if (m_szTrackComment != "")
	{
		newstr.Format(L"%s%s'%s', ", get_string(IDS_TRACKCOMMENT), sContains, m_szTrackComment);
		str += newstr;
	}
	
	if (m_szTrackLyrics != "")
	{
		newstr.Format(L"%s%s'%s', ", get_string(IDS_LYRICS), sContains, m_szTrackLyrics);
		str += newstr;
	}

	if (m_lTrackYearRecordedFrom || m_lTrackYearRecordedTo)
	{
		newstr.Format(L"%s='%d-%d', ", get_string(IDS_TRACK_YEAR_RECORDED), m_lTrackYearRecordedFrom, m_lTrackYearRecordedTo);
		str += newstr;
	}
	
	for (int i=0;i<MAX_USER_FIELDS;i++)
	{
		if (m_szTrackUser[i] != "")
		{
			newstr.Format(L"%s%s'%s', ", CString(DataBase->Master->UserTrackFields[i]->Name), sContains, m_szTrackUser[i]);
			str += newstr;
		}
	}
	
	if (m_sTrackSoundFilename != "")
	{
		newstr.Format(L"%s%s'%s', ", get_string(IDS_SOUNDFILENAME), sContains, m_sTrackSoundFilename);
		str += newstr;
	}
	
	if (m_lTrackRatingFrom || m_lTrackRatingTo)
	{
		newstr.Format(L"%s='%d-%d', ", get_string(IDS_TRACK_RATING), m_lTrackRatingFrom, m_lTrackRatingTo);
		str += newstr;
	}
	
	// Version 11
	if (!m_sTrackComposer.IsEmpty())
	{
		newstr.Format(L"%s%s'%s', ", get_string(IDS_TRACK_COMPOSER), sContains, m_sTrackComposer);
		str += newstr;
	}

	if (!m_szTrackLanguage.IsEmpty())
	{
		newstr.Format(L"%s%s'%s', ", get_string(IDS_TRACK_LANGUAGE), sContains, m_szTrackLanguage);
		str += newstr;
	}

	if (!m_szTrackCategory.IsEmpty())
	{
		newstr.Format(L"%s='%s', ", get_string(IDS_TRACK_CATEGORY), m_szTrackCategory);
		str += newstr;
	}

	if (str != "")
		str = str.Left(str.GetLength()-2);
	else
		str.Format(L"<%s>", get_string(IDS_NONE2));
	
	return str;
}

// Liefert den Selektions-String zurück
CString CSelection::GetSortString(BOOL bTrackAndCD)
{
	CString str;
	int i;

	for (i=0;i<3 && i < m_SortKeys.GetCount();i++)
	{
		if (m_SortKeys[i] > 0)
		{
			str += CString(DataBase->GetNameOfField(FieldHelper::GetNewFieldFromOldField(m_SortKeys[i])));

			if (str.Right(2) != ", ")
				str += ", ";
		}
	}

	if (str.Right(2) == ", ")
		str = str.Left(str.GetLength()-2);

	return str;
}

int CSelection::DoSortDlg(DWORD dwFlags/*=SSD_CD*/)
{
	int ret;
	
	CSearchSortDlg SearchSortDlg(dwFlags);
	SearchSortDlg.DataBase = DataBase;
	
	SearchSortDlg.m_SortKey1 = m_SortKeys[0];
	SearchSortDlg.m_SortKey2 = m_SortKeys[1];
	SearchSortDlg.m_SortKey3 = m_SortKeys[2];
	SearchSortDlg.m_bSortDirection1Desc = !m_bSortKeysDirection[0];
	SearchSortDlg.m_bSortDirection2Desc = !m_bSortKeysDirection[1];
	SearchSortDlg.m_bSortDirection3Desc = !m_bSortKeysDirection[2];

	ret = SearchSortDlg.DoModal();

	if (ret == IDOK)
	{
		m_SortKeys[0] = SearchSortDlg.m_SortKey1;
		m_SortKeys[1] = SearchSortDlg.m_SortKey2;
		m_SortKeys[2] = SearchSortDlg.m_SortKey3;

		m_bSortKeysDirection[0] = !SearchSortDlg.m_bSortDirection1Desc;
		m_bSortKeysDirection[1] = !SearchSortDlg.m_bSortDirection2Desc;
		m_bSortKeysDirection[2] = !SearchSortDlg.m_bSortDirection3Desc;
	}
	
	return ret;
}

void CSelection::SetSortKey(int iSortKey, BOOL bAscending)
{
	m_SortKeys[0] = iSortKey;
	m_SortKeys[1] = 0;
	m_SortKeys[2] = 0;

	m_bSortKeysDirection[0] = bAscending;
	m_bSortKeysDirection[1] = TRUE;
	m_bSortKeysDirection[2] = TRUE;
}

void CSelection::Reset()
{
	ResetCD();
	ResetTrack();

	m_bMatchCase = FALSE;

	m_bExactMatch = FALSE;

	m_Filter.RemoveAll();
}

void CSelection::ResetCD()
{
	m_dwTotalLengthFrom = 0;
	m_dwTotalLengthTo = 0;
	m_wNumberOfTracksFrom = 0;
	m_wNumberOfTracksTo = 0;
	m_wCDSetNumberFrom = 0;
	m_wCDSetNumberTo = 0;
	m_szCDSetName = "";
	m_wCDType = 0;
	m_szCDTitle = "";
	m_szCategory = "";
	m_szMedium = "";
	m_szDateFrom = "";
	m_szDateTo = "";
	m_szCodes = "";
	m_sArchivNummerFrom = "";
	m_szCDComment = "";
	m_sCopyright = "";
	m_lYearRecordedFrom = 0;
	m_lYearRecordedTo = 0;

	m_iOriginalCD = 0;
	m_sArchivNummerTo = "";
	m_sLabel = "";
	m_sURL = "";
	m_sUPC = "";
	m_lRatingFrom = 0;
	m_lRatingTo = 0;
	m_lPriceFrom = 0;
	m_lPriceTo = 0;
	m_sFrontCover = "";
	m_sBackCover = "";
	m_sCDLabel = "";

	m_sComposer = "";
	m_sLanguage = "";
	m_sLocation = "";
	m_sParticipant = "";

	for (int i=0;i<MAX_USER_FIELDS;i++)
	{
		m_szCDUser[i] = "";
	}

	ResetCDArtist();
}

void CSelection::ResetCDArtist()
{
	m_szCDArtist = "";
}

void CSelection::ResetTrack()
{
	m_szTrackTitle = "";
	m_dwTrackLengthFrom = 0;
	m_dwTrackLengthTo = 0;
	m_wTrackBpmFrom = 0;
	m_wTrackBpmTo = 0;
	m_szTrackCodes = "";
	m_szTrackComment = "";
	m_szTrackLyrics = "";
	m_sTrackSoundFilename = "";
	m_lTrackYearRecordedFrom = 0;
	m_lTrackYearRecordedTo = 0;

	m_lTrackRatingFrom = 0;
	m_lTrackRatingTo = 0;

	for (int i=0;i<MAX_USER_FIELDS;i++)
	{
		m_szTrackUser[i] = "";
	}

	m_sTrackComposer = "";
	m_szTrackLanguage = "";
	m_szTrackCategory = "";

	ResetTrackArtist();
}

void CSelection::ResetTrackArtist()
{
	m_szTrackArtist = "";
}

// Speichert die Selection in der Registry
void CSelection::Save(const CString & sSaveKey)
{
	HKEY hHitKey;
	DWORD ret;

	RegCreateKeyEx(HKEY_CURRENT_USER, sSaveKey, 0,
		L"Hitbase", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL,
		&hHitKey, &ret);

	CConfig::RegWriteInt(hHitKey, "TotalLengthFrom", m_dwTotalLengthFrom);
	CConfig::RegWriteInt(hHitKey, "TotalLengthTo", m_dwTotalLengthTo);
	CConfig::RegWriteInt(hHitKey, "NumberOfTracksFrom", m_wNumberOfTracksFrom);
	CConfig::RegWriteInt(hHitKey, "NumberOfTracksTo", m_wNumberOfTracksTo);

	CConfig::RegWriteString(hHitKey, "CDName", m_szCDArtist);
	CConfig::RegWriteString(hHitKey, "CDTitle", m_szCDTitle);
	CConfig::RegWriteString(hHitKey, "Category", m_szCategory);
	CConfig::RegWriteString(hHitKey, "Medium", m_szMedium);
	CConfig::RegWriteString(hHitKey, "DateFrom", m_szDateFrom);
	CConfig::RegWriteString(hHitKey, "DateTo", m_szDateTo);
	CConfig::RegWriteString(hHitKey, "Codes", m_szCodes);
	CConfig::RegWriteString(hHitKey, "ArchivNummer", m_sArchivNummerFrom);
	CConfig::RegWriteString(hHitKey, "CDComment", m_szCDComment);
	CConfig::RegWriteString(hHitKey, "Copyright", m_sCopyright);
	CConfig::RegWriteInt(hHitKey, "YearRecordedFrom", m_lYearRecordedFrom);
	CConfig::RegWriteInt(hHitKey, "YearRecordedTo", m_lYearRecordedTo);

	CConfig::RegWriteInt(hHitKey, "OriginalCD", m_iOriginalCD);
	CConfig::RegWriteString(hHitKey, "ArchivNummerTo", m_sArchivNummerTo);
	CConfig::RegWriteString(hHitKey, "Label", m_sLabel);
	CConfig::RegWriteString(hHitKey, "URL", m_sURL);
	CConfig::RegWriteString(hHitKey, "UPC", m_sUPC);
	CConfig::RegWriteInt(hHitKey, "RatingFrom", m_lRatingFrom);
	CConfig::RegWriteInt(hHitKey, "RatingTo", m_lRatingTo);
	CConfig::RegWriteInt(hHitKey, "PriceFrom", m_lPriceFrom);
	CConfig::RegWriteInt(hHitKey, "PriceTo", m_lPriceTo);
	CConfig::RegWriteString(hHitKey, "FrontCover", m_sFrontCover);
	CConfig::RegWriteString(hHitKey, "BackCover", m_sBackCover);
	CConfig::RegWriteString(hHitKey, "CDLabel", m_sCDLabel);

	// Version 11
	CConfig::RegWriteString(hHitKey, "Composer", m_sComposer);
	CConfig::RegWriteString(hHitKey, "Language", m_sLanguage);
	CConfig::RegWriteString(hHitKey, "Location", m_sLocation);
	CConfig::RegWriteString(hHitKey, "Participant", m_sParticipant);

	for (int i=0;i<MAX_USER_FIELDS;i++)
	{
		CString sFieldName;
		sFieldName.Format(L"CDUser%d", i+1);
		CConfig::RegWriteString(hHitKey, sFieldName, m_szCDUser[i]);
		sFieldName.Format(L"TrackUser%d", i+1);
		CConfig::RegWriteString(hHitKey, sFieldName, m_szTrackUser[i]);
	}

	CConfig::RegWriteString(hHitKey, "TrackArtist", m_szTrackArtist);
	CConfig::RegWriteString(hHitKey, "TrackName", m_szTrackTitle);
	CConfig::RegWriteInt(hHitKey, "TrackLengthFrom", m_dwTrackLengthFrom);
	CConfig::RegWriteInt(hHitKey, "TrackLengthTo", m_dwTrackLengthTo);
	CConfig::RegWriteInt(hHitKey, "TrackBPMFrom", m_wTrackBpmFrom);
	CConfig::RegWriteInt(hHitKey, "TrackBPMTo", m_wTrackBpmTo);
	CConfig::RegWriteString(hHitKey, "TrackCodes", m_szTrackCodes);
	CConfig::RegWriteString(hHitKey, "TrackComment", m_szTrackComment);
	CConfig::RegWriteString(hHitKey, "TrackLyrics", m_szTrackLyrics);
	CConfig::RegWriteString(hHitKey, "TrackSoundFilename", m_sTrackSoundFilename);
	CConfig::RegWriteInt(hHitKey, "TrackYearRecordedFrom", m_lTrackYearRecordedFrom);
	CConfig::RegWriteInt(hHitKey, "TrackYearRecordedTo", m_lTrackYearRecordedTo);

	CConfig::RegWriteInt(hHitKey, "TrackRatingFrom", m_lTrackRatingFrom);
	CConfig::RegWriteInt(hHitKey, "TrackRatingTo", m_lTrackRatingTo);

	// Version 11
	CConfig::RegWriteString(hHitKey, "TrackComposer", m_sTrackComposer);
	CConfig::RegWriteString(hHitKey, "TrackLanguage", m_szTrackLanguage);
	CConfig::RegWriteString(hHitKey, "TrackCategory", m_szTrackCategory);

	CConfig::RegWriteInt(hHitKey, "SortKey1", m_SortKeys[0]);
	CConfig::RegWriteInt(hHitKey, "SortKey2", m_SortKeys[1]);
	CConfig::RegWriteInt(hHitKey, "SortKey3", m_SortKeys[2]);

	CConfig::RegWriteInt(hHitKey, "SortKeyDirection1", m_bSortKeysDirection[0]);
	CConfig::RegWriteInt(hHitKey, "SortKeyDirection2", m_bSortKeysDirection[1]);
	CConfig::RegWriteInt(hHitKey, "SortKeyDirection3", m_bSortKeysDirection[2]);

	CConfig::RegWriteInt(hHitKey, "MatchCase", m_bMatchCase);

	CConfig::RegWriteInt(hHitKey, "ExactMatch", m_bExactMatch);

	RegCloseKey(hHitKey);
}

// Liest die gespeicherten Informationen aus der Registry.
void CSelection::Restore(const CString & sKey)
{
	HKEY hHitKey;
	DWORD ret;
	
	RegCreateKeyEx(HKEY_CURRENT_USER, sKey, 0,
		L"Hitbase", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL,
		&hHitKey, &ret);
	
	m_dwTotalLengthFrom = CConfig::RegQueryInt(hHitKey, L"TotalLengthFrom", 0);
	m_dwTotalLengthTo = CConfig::RegQueryInt(hHitKey, L"TotalLengthTo", 0);
	m_wNumberOfTracksFrom = (short)CConfig::RegQueryInt(hHitKey, L"NumberOfTracksFrom", 0);
	m_wNumberOfTracksTo = (short)CConfig::RegQueryInt(hHitKey, L"NumberOfTracksTo", 0);
	
	m_szCDArtist = CConfig::RegQueryString(hHitKey, L"CDName", L"");
	m_szCDTitle = CConfig::RegQueryString(hHitKey, L"CDTitle", L"");
	m_szCategory = CConfig::RegQueryString(hHitKey, L"Category", L"");
	m_szMedium = CConfig::RegQueryString(hHitKey, L"Medium", L"");
	m_szDateFrom = CConfig::RegQueryString(hHitKey, L"DateFrom", L"");
	m_szDateTo = CConfig::RegQueryString(hHitKey, L"DateTo", L"");
	m_szCodes = CConfig::RegQueryString(hHitKey, L"Codes", L"");
	m_szCDComment = CConfig::RegQueryString(hHitKey, L"CDComment", L"");
	m_sArchivNummerFrom = CConfig::RegQueryString(hHitKey, L"ArchivNummer", L"");
	m_sCopyright = CConfig::RegQueryString(hHitKey, L"Copyright", L"");
	m_lYearRecordedFrom = CConfig::RegQueryInt(hHitKey, L"YearRecordedFrom", 0);
	m_lYearRecordedTo = CConfig::RegQueryInt(hHitKey, L"YearRecordedTo", 0);

	m_iOriginalCD = CConfig::RegQueryInt(hHitKey, L"OriginalCD", 0);
	m_sArchivNummerTo = CConfig::RegQueryString(hHitKey, L"ArchivNummerTo", L"");
	m_sLabel = CConfig::RegQueryString(hHitKey, L"Label", L"");
	m_sURL = CConfig::RegQueryString(hHitKey, L"URL", L"");
	m_sUPC = CConfig::RegQueryString(hHitKey, L"UPC", L"");
	m_lRatingFrom = CConfig::RegQueryInt(hHitKey, L"RatingFrom", 0);
	m_lRatingTo = CConfig::RegQueryInt(hHitKey, L"RatingTo", 0);
	m_lPriceFrom = CConfig::RegQueryInt(hHitKey, L"PriceFrom", 0);
	m_lPriceTo = CConfig::RegQueryInt(hHitKey, L"PriceTo", 0);
	m_sFrontCover = CConfig::RegQueryString(hHitKey, L"FrontCover", L"");
	m_sBackCover = CConfig::RegQueryString(hHitKey, L"BackCover", L"");
	m_sCDLabel = CConfig::RegQueryString(hHitKey, L"CDLabel", L"");

	m_sComposer = CConfig::RegQueryString(hHitKey, L"Composer", L"");
	m_sLanguage = CConfig::RegQueryString(hHitKey, L"Language", L"");
	m_sLocation = CConfig::RegQueryString(hHitKey, L"Location", L"");
	m_sParticipant = CConfig::RegQueryString(hHitKey, L"Participant", L"");

	for (int i=0;i<MAX_USER_FIELDS;i++)
	{
		CString sFieldname;
		sFieldname.Format(L"CDUser%d", i+1);
		m_szCDUser[i] = CConfig::RegQueryString(hHitKey, sFieldname, L"");
		sFieldname.Format(L"TrackUser%d", i+1);
		m_szTrackUser[i] = CConfig::RegQueryString(hHitKey, sFieldname, L"");
	}
	
	m_szTrackArtist = CConfig::RegQueryString(hHitKey, L"TrackArtist", L"");
	m_szTrackTitle = CConfig::RegQueryString(hHitKey, L"TrackName", L"");
	m_dwTrackLengthFrom = CConfig::RegQueryInt(hHitKey, L"TrackLengthFrom", 0);
	m_dwTrackLengthTo = CConfig::RegQueryInt(hHitKey, L"TrackLengthTo", 0);
	m_wTrackBpmFrom = (short)CConfig::RegQueryInt(hHitKey, L"TrackBPMFrom", 0);
	m_wTrackBpmTo = (short)CConfig::RegQueryInt(hHitKey, L"TrackBPMTo", 0);
	m_szTrackCodes = CConfig::RegQueryString(hHitKey, L"TrackCodes", L"");
	m_szTrackComment = CConfig::RegQueryString(hHitKey, L"TrackComment", L"");
	m_szTrackLyrics = CConfig::RegQueryString(hHitKey, L"TrackLyrics", L"");
	m_sTrackSoundFilename = CConfig::RegQueryString(hHitKey, L"TrackSoundFilename", L"");

	m_lTrackYearRecordedFrom = CConfig::RegQueryInt(hHitKey, L"TrackYearRecordedFrom", 0);
	m_lTrackYearRecordedTo = CConfig::RegQueryInt(hHitKey, L"TrackYearRecordedTo", 0);

	m_lTrackRatingFrom = CConfig::RegQueryInt(hHitKey, L"TrackRatingFrom", 0);
	m_lTrackRatingTo = CConfig::RegQueryInt(hHitKey, L"TrackRatingTo", 0);

	m_sTrackComposer = CConfig::RegQueryString(hHitKey, L"TrackComposer", L"");
	m_szTrackLanguage = CConfig::RegQueryString(hHitKey, L"TrackLanguage", L"");
	m_szTrackCategory = CConfig::RegQueryString(hHitKey, L"TrackCategory", L"");

	m_SortKeys[0] = CConfig::RegQueryInt(hHitKey, L"SortKey1", FIELD_CDNAME);
	m_SortKeys[1] = CConfig::RegQueryInt(hHitKey, L"SortKey2", FIELD_CDTITLE);
	m_SortKeys[2] = CConfig::RegQueryInt(hHitKey, L"SortKey3", 0);
	
	m_bSortKeysDirection[0] = CConfig::RegQueryInt(hHitKey, L"SortKeyDirection1", 1);
	m_bSortKeysDirection[1] = CConfig::RegQueryInt(hHitKey, L"SortKeyDirection2", 1);
	m_bSortKeysDirection[2] = CConfig::RegQueryInt(hHitKey, L"SortKeyDirection3", 1);
	
	m_bMatchCase = CConfig::RegQueryInt(hHitKey, L"MatchCase", 0);
	m_bExactMatch = CConfig::RegQueryInt(hHitKey, L"ExactMatch", 0);
	
	RegCloseKey(hHitKey);
}

// Liefert TRUE, wenn alle Selektionsfelder leer sind.
BOOL CSelection::IsEmpty()
{
   return (IsEmptyCD() && IsEmptyTrack());
}

// Liefert TRUE, wenn alle Selektionsfelder der Lieder leer sind.
BOOL CSelection::IsEmptyTrack()
{
	BOOL bUserFieldsEmpty = TRUE;
	
	for (int i=0;i<MAX_USER_FIELDS;i++)
	{
		if (!m_szTrackUser[i].IsEmpty())
		{
			bUserFieldsEmpty = FALSE;
			break;
		}
	}

	if (m_szTrackArtist.IsEmpty() && 
		m_szTrackTitle.IsEmpty() &&
		!m_dwTrackLengthFrom &&
		!m_dwTrackLengthTo &&
		!m_wTrackBpmFrom &&
		!m_wTrackBpmTo &&
		m_szTrackCodes.IsEmpty() &&
		m_szTrackComment.IsEmpty() &&
		m_szTrackLyrics.IsEmpty() &&
		!m_lTrackYearRecordedFrom &&
		!m_lTrackYearRecordedTo &&
		bUserFieldsEmpty &&
		m_sTrackSoundFilename.IsEmpty() &&
		!m_lTrackRatingFrom &&
		!m_lTrackRatingTo &&
		m_sTrackComposer.IsEmpty() && 
		m_szTrackLanguage.IsEmpty() &&
		m_szTrackCategory.IsEmpty())
		return TRUE;
	
   return FALSE;
}

BOOL CSelection::IsEmptyCD()
{
	BOOL bUserFieldsEmpty = TRUE;
	
	for (int i=0;i<MAX_USER_FIELDS;i++)
	{
		if (!m_szCDUser[i].IsEmpty())
		{
			bUserFieldsEmpty = FALSE;
			break;
		}
	}

   if (!m_dwTotalLengthFrom &&
       !m_dwTotalLengthTo &&
       !m_wNumberOfTracksFrom &&
       !m_wNumberOfTracksTo &&
       !m_wCDSetNumberFrom &&
       !m_wCDSetNumberTo &&
       m_szCDSetName.IsEmpty() &&
       !m_wCDType &&
       m_szCDArtist.IsEmpty() &&
       m_szCDTitle.IsEmpty() &&
       m_szCategory.IsEmpty() &&
       m_szMedium.IsEmpty() &&
       m_szDateFrom.IsEmpty() &&
       m_szDateTo.IsEmpty() &&
       m_szCodes.IsEmpty() &&
       m_sArchivNummerFrom.IsEmpty() &&
       m_szCDComment.IsEmpty() &&
	   m_sCopyright.IsEmpty() &&
	   !m_lYearRecordedFrom &&
	   !m_lYearRecordedTo &&
	   !m_iOriginalCD &&
	   m_sArchivNummerTo.IsEmpty() &&
	   m_sLabel.IsEmpty() &&
	   m_sURL.IsEmpty() &&
	   m_sUPC.IsEmpty() &&
	   !m_lRatingFrom &&
	   !m_lRatingTo &&
	   !m_lPriceFrom &&
	   !m_lPriceTo &&
	   m_sFrontCover.IsEmpty() &&
	   m_sBackCover.IsEmpty() &&
	   m_sCDLabel.IsEmpty() &&
	   m_sComposer.IsEmpty() &&
	   m_sLanguage.IsEmpty() &&
	   m_sLocation.IsEmpty() &&
	   m_sParticipant.IsEmpty() &&
       bUserFieldsEmpty)
      return TRUE;

   return FALSE;
}

// Liefert TRUE zurück, wenn nach einem CD-Feld sortiert wird.
BOOL CSelection::IsSortedByCDField()
{
	for (int i=0;i<m_SortKeys.GetSize();i++)
		if (m_SortKeys[i] >= 1 && m_SortKeys[i] <= FIELD_LAST_CD_FIELD)
			return TRUE;

	return FALSE;
}

// Liefert den Selektions-String für den neuen Filter zurück 
CString CSelection::GetFilterString(enumFilterTable filterTable) const
{
	CString newstr;
	
	CString str = "";
	
	for (int i=0;i<m_Filter.GetSize();i++)
	{
		bool bUseField = false;

		// Prüfen, welche Tabelle betroffen ist
		switch (filterTable)
		{
		case filterTableArtist:
			if (m_Filter[i].m_iField == FIELD_CDNAME)
				bUseField = true;
			break;
		case filterTableCD:
			if (m_Filter[i].m_iField <= FIELD_LAST_CD_FIELD &&
				m_Filter[i].m_iField != FIELD_CDNAME)
				bUseField = true;
			break;
		case filterTableCDArtist:
			if (m_Filter[i].m_iField <= FIELD_LAST_CD_FIELD)
				bUseField = true;
			break;
		case filterTableTrack:
			if (m_Filter[i].m_iField >= FIELD_TRACK_ARTIST &&
				m_Filter[i].m_iField <= FIELD_LAST_TRACK_FIELD)
				bUseField = true;
			break;
		case filterTableAll:    // OK, alle Felder
			bUseField = true;
			break;
		}

		if (bUseField)
		{
			CString sLog;

			if (i > 0)
			{
				switch (m_Filter[i-1].m_log)
				{
				case CFilterElement::opAnd:
					sLog = " AND ";
					break;
				case CFilterElement::opOr:
					sLog = " OR ";
					break;
				}
			}

			CString sFieldname = CString(DataBase->GetDataBaseColumnOfField(FieldHelper::GetNewFieldFromOldField(m_Filter[i].m_iField)));

			System::Type^ fieldType;
			fieldType = DataBase->GetTypeByField(FieldHelper::GetNewFieldFromOldField(m_Filter[i].m_iField));

			if (Big3::Hitbase::Configuration::Settings::Current->SortArchiveNumberNumeric &&
				m_Filter[i].m_iField == FIELD_ARCHIVNUMMER)
			{
				sFieldname = "IIF(IsNumeric(CD.szArchivnummer),CLng(CD.szArchivnummer),0)";
				fieldType = System::Int32::typeid;
			}

			CString sOperator;

			switch (m_Filter[i].m_op)
			{
			case CFilterElement::opLess:
				sOperator = "<";
				break;
			case CFilterElement::opLessEqual:
				sOperator = "<=";
				break;
			case CFilterElement::opEqual:
				if (fieldType == System::String::typeid)
					sOperator = "like";
				else
					sOperator = "=";
				break;
			case CFilterElement::opGreaterEqual:
				sOperator = ">=";
				break;
			case CFilterElement::opGreater:
				sOperator = ">";
				break;
			case CFilterElement::opNotEqual:
				sOperator = "<>";
				break;
			default:
				ASSERT(FALSE);
			}

			CString sValue;

			if (fieldType == System::String::typeid)
			{
				if (m_Filter[i].m_op == CFilterElement::opEqual)
					sValue = "'*" + m_Filter[i].m_sValue + "*'";
				else
					sValue = "'" + m_Filter[i].m_sValue + "'";
			}
			else
			{
				sValue.Format(L"%d", _wtoi(m_Filter[i].m_sValue));
			}

			CString sWherePart;

			if (m_Filter[i].m_bNot)
				sWherePart.Format(L"%s NOT (%s %s %s)", sLog, sFieldname, sOperator, sValue);
			else
				sWherePart.Format(L"%s %s %s %s", sLog, sFieldname, sOperator, sValue);

			str += sWherePart;
		}
	}
	
	return str;
}

// Liefert den Selektions-String für den neuen Filter zurück 
CString CSelection::GetOrderString(BOOL bCDOnly) const
{
	CString sOrder;
	CString sOrderPart;
	
	for (int i=0;i<3;i++)
	{
		if (m_SortKeys[i] > 0 && (!bCDOnly || CFieldList::IsCDField(m_SortKeys[i])))
		{
			CString sFieldname = CString(DataBase->GetDataBaseColumnOfField(FieldHelper::GetNewFieldFromOldField(m_SortKeys[i])));

			// Wenn die Archivnummer nummerisch ist, dann beim Sortieren in Long umwandeln.
			if (Big3::Hitbase::Configuration::Settings::Current->SortArchiveNumberNumeric && m_SortKeys[i] == FIELD_ARCHIVNUMMER)
				sFieldname = "IIF(IsNumeric(szArchivnummer),CLng(szArchivnummer),0)";

			// CD-Sets nach CD-Set und CD-Set-Nummer sortieren
			if (m_SortKeys[i] == FIELD_CDSET)
				sFieldname += "&CStr(wNummerImSet)";

			// Numerische Benutzerdefinierte Felder anders sortieren.
			if (m_SortKeys[i] >= FIELD_CDUSER1 && m_SortKeys[i] <= FIELD_CDUSER5)
			{
				if (DataBase->Master->UserCDFields[m_SortKeys[i]-FIELD_CDUSER1]->Type == UserFieldType::Number)
				{
					CString sUserFieldSort;
					sUserFieldSort.Format(L"IIF(IsNumeric(%s),CLng(%s),0)", sFieldname, sFieldname);
					sFieldname = sUserFieldSort;
				}
			}

			// Numerische Benutzerdefinierte Felder anders sortieren.
			if (m_SortKeys[i] >= FIELD_TRACK_USER1 && m_SortKeys[i] <= FIELD_TRACK_USER5)
			{
				if (DataBase->Master->UserTrackFields[m_SortKeys[i]-FIELD_TRACK_USER1]->Type == UserFieldType::Number)
				{
					CString sUserFieldSort;
					sUserFieldSort.Format(L"IIF(IsNumeric(%s),CLng(%s),0)", sFieldname, sFieldname);
					sFieldname = sUserFieldSort;
				}
			}

			if (!m_bSortKeysDirection[i])
				sOrderPart.Format(L"%s DESC, ", sFieldname);
			else
				sOrderPart.Format(L"%s, ", sFieldname);

			sOrder += sOrderPart;
		}
	}
	
	if (!sOrder.IsEmpty())     // Das letzte ", " löschen
		sOrder = sOrder.Left(sOrder.GetLength()-2);

	return sOrder;
}

// Liefert den Selektions-String als Beschreibung für den neuen Filter zurück 
CString CSelection::GetFilterText(enumFilterTable filterTable) const
{
	CString newstr;
	
	CString str = "";
	
	for (int i=0;i<m_Filter.GetSize();i++)
	{
		bool bUseField = false;

		// Prüfen, welche Tabelle betroffen ist
		switch (filterTable)
		{
		case filterTableArtist:
			if (m_Filter[i].m_iField == FIELD_CDNAME)
				bUseField = true;
			break;
		case filterTableCD:
			if (m_Filter[i].m_iField <= FIELD_LAST_CD_FIELD &&
				m_Filter[i].m_iField != FIELD_CDNAME)
				bUseField = true;
			break;
		case filterTableCDArtist:
			if (m_Filter[i].m_iField <= FIELD_LAST_CD_FIELD)
				bUseField = true;
			break;
		case filterTableTrack:
			if (m_Filter[i].m_iField >= FIELD_TRACK_ARTIST &&
				m_Filter[i].m_iField <= FIELD_LAST_TRACK_FIELD)
				bUseField = true;
			break;
		case filterTableAll:    // OK, alle Felder
			bUseField = true;
			break;
		}

		if (bUseField)
		{
			CString sLog;

			if (i > 0)
			{
				switch (m_Filter[i-1].m_log)
				{
				case CFilterElement::opAnd:
					sLog = " " + get_string(IDS_AND);
					break;
				case CFilterElement::opOr:
					sLog = " " + get_string(IDS_OR);
					break;
				}
			}

//			int iDummy;
			CString sFieldname = CString(DataBase->GetNameOfField(FieldHelper::GetNewFieldFromOldField(m_Filter[i].m_iField)));

			System::Type^ fieldType;
			fieldType = DataBase->GetTypeByField(FieldHelper::GetNewFieldFromOldField(m_Filter[i].m_iField));

			CString sOperator;

			switch (m_Filter[i].m_op)
			{
			case CFilterElement::opLess:
				sOperator = "<";
				break;
			case CFilterElement::opLessEqual:
				sOperator = "<=";
				break;
			case CFilterElement::opEqual:
				if (fieldType == System::String::typeid)
					sOperator = get_string(IDS_LIKE);
				else
					sOperator = "=";
				break;
			case CFilterElement::opGreaterEqual:
				sOperator = ">=";
				break;
			case CFilterElement::opGreater:
				sOperator = ">";
				break;
			case CFilterElement::opNotEqual:
				sOperator = "<>";
				break;
			default:
				ASSERT(FALSE);
			}

			CString sValue;

			if (fieldType == System::String::typeid)
			{
				sValue = "'" + m_Filter[i].m_sValue + "'";
			}
			else
			{
				sValue.Format(L"%d", _wtoi(m_Filter[i].m_sValue));
			}

			CString sWherePart;

			if (m_Filter[i].m_bNot)
				sWherePart.Format(L"%s %s (%s %s %s)", sLog, get_string(IDS_NOT), sFieldname, sOperator, sValue);
			else
				sWherePart.Format(L"%s %s %s %s", sLog, sFieldname, sOperator, sValue);

			str += sWherePart;
		}
	}
	
	return str;
}

bool CSelection::IsFilterEmpty(enumFilterTable filterTable) const
{
	if (filterTable == filterTableAll)
		return (m_Filter.GetSize() == 0);
	else
	{
		for (int i=0;i<m_Filter.GetSize();i++)
		{
			bool bUseField = false;

			// Prüfen, welche Tabelle betroffen ist
			switch (filterTable)
			{
			case filterTableArtist:
				if (m_Filter[i].m_iField == FIELD_CDNAME)
					bUseField = true;
				break;
			case filterTableCD:
				if (m_Filter[i].m_iField <= FIELD_LAST_CD_FIELD &&
					m_Filter[i].m_iField != FIELD_CDNAME)
					bUseField = true;
				break;
			case filterTableCDArtist:
				if (m_Filter[i].m_iField <= FIELD_LAST_CD_FIELD)
					bUseField = true;
				break;
			case filterTableTrack:
				if (m_Filter[i].m_iField >= FIELD_TRACK_ARTIST &&
					m_Filter[i].m_iField <= FIELD_LAST_TRACK_FIELD)
					bUseField = true;
				break;
			case filterTableAll:    // OK, alle Felder
				bUseField = true;
				break;
			}

			if (bUseField)
				return false;
		}
	}

	return true;
}

/////////////////////////////////////////////////////////////////////////////
// CSearchSortDlg dialog


CSearchSortDlg::CSearchSortDlg(DWORD dwFlags/*=0*/,CWnd* pParent /*=NULL*/)
	: CDialog(CSearchSortDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CSearchSortDlg)
	m_SortKey1 = -1;
	m_SortKey2 = -1;
	m_SortKey3 = -1;
	//}}AFX_DATA_INIT

    m_dwFlags = dwFlags;

	m_bSortDirection1Desc = -1;
	m_bSortDirection2Desc = -1;
	m_bSortDirection3Desc = -1;
}


void CSearchSortDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CSearchSortDlg)
	DDX_Control(pDX, IDC_SORTKEY3, m_SortKey3Ctrl);
	DDX_Control(pDX, IDC_SORTKEY2, m_SortKey2Ctrl);
	DDX_Control(pDX, IDC_SORTKEY1, m_SortKey1Ctrl);
	DDX_CBIndex(pDX, IDC_SORTKEY1, m_SortKey1);
	DDX_CBIndex(pDX, IDC_SORTKEY2, m_SortKey2);
	DDX_CBIndex(pDX, IDC_SORTKEY3, m_SortKey3);
	//}}AFX_DATA_MAP
	DDX_Radio(pDX, IDC_SELECTION1_SORTASC, m_bSortDirection1Desc);
	DDX_Radio(pDX, IDC_SELECTION2_SORTASC, m_bSortDirection2Desc);
	DDX_Radio(pDX, IDC_SELECTION3_SORTASC, m_bSortDirection3Desc);
}


BEGIN_MESSAGE_MAP(CSearchSortDlg, CDialog)
	//{{AFX_MSG_MAP(CSearchSortDlg)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()


/////////////////////////////////////////////////////////////////////////////
// CSearchSortDlg message handlers

BOOL CSearchSortDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	AddStringToSortList((CString)"<" + (CString)get_string(IDS_NONE2) + (CString)">", 0);

	switch (m_dwFlags)
	{
	case SSD_CD:
		{
			CFieldList fl = CFieldList::GetAllFields(FLF_CD);

			for (int i=0;i<fl.GetCount();i++)
			{
//				int iDummy;
				CString sName = CString(DataBase->GetNameOfField(FieldHelper::GetNewFieldFromOldField(fl[i])));
				AddStringToSortList(sName, fl[i]);
			}
			break;
		}
	case SSD_TRACK:
		{
			CFieldList fl = CFieldList::GetAllFields(FLF_ALL);

			for (int i=0;i<fl.GetCount();i++)
			{
//				int iDummy;
				CString sName = CString(DataBase->GetNameOfField(FieldHelper::GetNewFieldFromOldField(fl[i])));
				AddStringToSortList(sName, fl[i]);
			}

			break;
		}
	default:
		ASSERT(FALSE);
	}
	
	SelectItemData(m_SortKey1Ctrl, m_SortKey1);
	SelectItemData(m_SortKey2Ctrl, m_SortKey2);
	SelectItemData(m_SortKey3Ctrl, m_SortKey3);

	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

void CSearchSortDlg::AddStringToSortList(const CString& str, int iValue)
{
	int iIndex;

	iIndex = m_SortKey1Ctrl.AddString(str);
	m_SortKey1Ctrl.SetItemData(iIndex, iValue);
	iIndex = m_SortKey2Ctrl.AddString(str);
	m_SortKey2Ctrl.SetItemData(iIndex, iValue);
	iIndex = m_SortKey3Ctrl.AddString(str);
	m_SortKey3Ctrl.SetItemData(iIndex, iValue);
}

void CSearchSortDlg::OnOK() 
{
	int sort1, sort2, sort3;
	
	sort1 = m_SortKey1Ctrl.GetCurSel();
	sort2 = m_SortKey2Ctrl.GetCurSel();
	sort3 = m_SortKey3Ctrl.GetCurSel();
	
	if (sort1 == sort2 && sort1 > 0 || sort1 == sort3 && sort1 > 0 || sort2 == sort3 && sort2 > 0)
	{
		AfxMessageBox(get_string(IDS_WRONGSORT), MB_OK|MB_ICONINFORMATION); 
		return;
	}
	
	CDialog::OnOK();

	if (sort1 > 0) m_SortKey1 = m_SortKey1Ctrl.GetItemData(sort1);
	if (sort2 > 0) m_SortKey2 = m_SortKey2Ctrl.GetItemData(sort2);
	if (sort3 > 0) m_SortKey3 = m_SortKey3Ctrl.GetItemData(sort3);
}

// Selektiert das Element, welches als ItemData den Wert iData hat.
void CSearchSortDlg::SelectItemData(CComboBox& cboSort, int iData)
{
	for (int i=0;i<cboSort.GetCount();i++)
	{
		if (cboSort.GetItemData(i) == (DWORD)iData)
		{
			cboSort.SetCurSel(i);
			return;
		}
	}
}
