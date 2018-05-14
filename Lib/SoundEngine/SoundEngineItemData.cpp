#include "stdafx.h"
#include "SoundEngineItemData.h"
#include <shlwapi.h>
#include "../hitmisc/hitmisc.h"

CSoundEngineItemData::CSoundEngineItemData() 
{ 
	m_dwLength = 0; 
	m_dwStartTime = 0; 
	m_dwVorlaufZeitSeconds = -1;	// -1 = Standardwert benutzen
	m_dwStopTimeSeconds = -1;		// -1 = Standardwert benutzen
	m_iChannels = 0;
	m_iBitRate = 0;
	m_iSampleRate = 0;
	m_iTrack = 0;
	m_iNumberOfTracks = 0;
	m_bPlaying = FALSE;        // Um spielendes Lied wiederzufinden, wenn Liste geändert wurde (z.b. Shuffle)
	m_iBitsPerSample = 0;
}

CSoundEngineItemData::CSoundEngineItemData(const CSoundEngineItemData& theOther)
{
	*this = theOther;
}

CSoundEngineItemData& CSoundEngineItemData::operator =(const CSoundEngineItemData &theOther)
{
	m_dwLength = theOther.m_dwLength;
	m_dwStartTime = theOther.m_dwStartTime; 
	m_dwVorlaufZeitSeconds = theOther.m_dwVorlaufZeitSeconds; 
	m_dwStopTimeSeconds = theOther.m_dwStopTimeSeconds; 
	m_sFilename = theOther.m_sFilename;
	m_sArtist = theOther.m_sArtist;
	m_sTitle = theOther.m_sTitle;
	m_sComment = theOther.m_sComment;
	m_iChannels = theOther.m_iChannels;
	m_iBitRate = theOther.m_iBitRate;
	m_iSampleRate = theOther.m_iSampleRate;
	m_iBitsPerSample = theOther.m_iBitsPerSample;
	m_bPlaying = theOther.m_bPlaying;
	m_sYear = theOther.m_sYear;
	m_iTrack = theOther.m_iTrack;
	m_iNumberOfTracks = theOther.m_iNumberOfTracks;
	m_sAlbum = theOther.m_sAlbum;
	m_sChecksum = theOther.m_sChecksum;
	m_sFrontCover = theOther.m_sFrontCover;

	return *this;
}

void CSoundEngineItemData::Clear()
{
	m_dwLength = 0;
	m_dwStartTime = 0; 
	m_dwVorlaufZeitSeconds = -1;	// -1 = Standardwert benutzen
	m_dwStopTimeSeconds = -1;		// -1 = Standardwert benutzen
	m_sFilename = "";
	m_sArtist = "";
	m_sTitle = "";
	m_sComment = "";
	m_iChannels = 0;
	m_iBitRate = 0;
	m_iBitsPerSample = 0;
	m_iSampleRate = 0;
	m_bPlaying = FALSE;
	m_sYear = "";
	m_iTrack = 0;
	m_iNumberOfTracks = 0;
	m_sAlbum = "";
	m_sChecksum = "";
	m_sFrontCover = "";
}

// Liefert einen String zurück, mit dem man die Klasse in eine
// Text-Datei speichern kann.
CString CSoundEngineItemData::GetString(const CString& sRelativePath)
{
	USES_CONVERSION;
	CString str;

	CString sRelFilename;
	// Die Pfade werden nun relativ zur m3u-Datei gespeichert.
	BOOL bRet = PathRelativePathTo(sRelFilename.GetBuffer(_MAX_PATH), sRelativePath, 0, m_sFilename, 0);
	sRelFilename.ReleaseBuffer();
	if (sRelFilename.Left(2) == ".\\")
		sRelFilename = sRelFilename.Mid(2);

	if (!bRet)
		sRelFilename = m_sFilename;

	str.Format(L"PARAMS=%d, %d\nLENGTH=%d\nFILENAME=%s\nARTIST=%s\nTITLE=%s\n", m_dwVorlaufZeitSeconds, m_dwStopTimeSeconds, m_dwLength, sRelFilename, m_sArtist, m_sTitle);

	return str;
}

BOOL CSoundEngineItemData::GetValue(CStdioFile* pFile, const CString& sParameter, CString& sValue)
{
	CString str, sParam;

	if (!pFile->ReadString(str))
		return FALSE;

	// Parameter suchen
	if (str.Find('=') > 0)
	{
		sParam = str.Left(str.Find('='));
		sValue = str.Mid(str.Find('=')+1);
	}

	// Richtiger Parameter?
	if (sParam == sParameter)
		return TRUE;
	else
		return FALSE;
}

BOOL CSoundEngineItemData::Read(CStdioFile *pFile, const CString& sRelativePath)
{
	CString sLine, sValue;

	if (!GetValue(pFile, L"PARAMS", sValue))
		return FALSE;
	m_dwVorlaufZeitSeconds = _wtoi(sValue);
	m_dwStopTimeSeconds = _wtoi(sValue.Mid(sValue.Find(',')+1));

	if (!GetValue(pFile, L"LENGTH", sValue))
		return FALSE;
	m_dwLength = _wtoi(sValue);

	if (!GetValue(pFile, L"FILENAME", sValue))
		return FALSE;

	CString sAbsFilename;
	if (PathIsRelative(sValue))
	{
		CString sPath = CMisc::CombinePathWithFileName(GetPathFromFileName(sRelativePath), sValue);
		PathCanonicalize(sAbsFilename.GetBuffer(_MAX_PATH), sPath);
		sAbsFilename.ReleaseBuffer();
	}
	else
		sAbsFilename = sValue;

	m_sFilename = sAbsFilename;

	if (!GetValue(pFile, L"ARTIST", sValue))
		return FALSE;
	m_sArtist = sValue;

	if (!GetValue(pFile, L"TITLE", sValue))
		return FALSE;
	m_sTitle = sValue;

	return TRUE;
}

