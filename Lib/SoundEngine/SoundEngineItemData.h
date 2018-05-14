#pragma once

#include "SoundEngineIntern.h"

class SOUNDENGINE_INTERFACE CSoundEngineItemData
{
	// Konstruktoren
public:
	CSoundEngineItemData();

	CSoundEngineItemData(const CSoundEngineItemData& theOther);

	CSoundEngineItemData& operator =(const CSoundEngineItemData &theOther);

	// Operatoren
public:
	virtual BOOL Read (CStdioFile* pFile, const CString& sRelativePath);
	virtual CString GetString(const CString& sRelativePath);

	virtual void Clear();

	// Attribute
public:
	CString m_sFilename;

	CString m_sArtist;
	CString m_sTitle;

	CString m_sComment;

	CString m_sYear;
	int m_iTrack;					// Liednummer
	int m_iNumberOfTracks;			// Anzahl Lieder auf der CD (MP3 tag)
	CString m_sAlbum;				// Name des Albumgs (MP3 tag)
	CString m_sGenre;				// Genre
	CString m_sCopyright;
	CString m_sComposer;

	CString m_sFrontCover;			// Cover der CD

	DWORD m_dwLength;
	DWORD m_dwStartTime;

	DWORD m_dwVorlaufZeitSeconds;         // Vorlaufzeit in Sekunden
	DWORD m_dwStopTimeSeconds;            // x Sekunden vor Liedende stoppen

	int m_iChannels;
	int m_iBitRate;
	int m_iSampleRate;
	int m_iBitsPerSample;			// 0 = 16 Bit, 1 = 8 Bit

	BOOL m_bPlaying;
	
	CString m_sChecksum;

private:
	virtual BOOL GetValue(CStdioFile* pFile, const CString& sParameter, CString& sValue);
};

