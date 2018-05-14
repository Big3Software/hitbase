// Track.h: interface for the CTrack class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_TRACK_H__150CFA50_910C_11D2_A5B2_0080AD740CD1__INCLUDED_)
#define AFX_TRACK_H__150CFA50_910C_11D2_A5B2_0080AD740CD1__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "DBQuery.h"

class CArtist;
class CCD;

class CTrackData {
public:
	long m_dwID;					 // Eindeutige ID (aus der Datenbank)
	long m_dwIDCD;					 // Eindeutige ID der CD (aus der Datenbank)
	long m_dwIDArtist;				 // Eindeutige ID des Interpreten (aus der Datenbank)
	short m_wTrack;					 // Lied-Nummer
	CString m_sTitle;				 // Titel des Liedes
	long m_dwLength;				 // Länge des Liedes in ms
	short m_wBpm;					 // Beat per minute
	CString m_sWavePath;			 // Pfad zur aufgenommenen WAV oder MP3-Datei
	CString m_sCodes;				 // Kennzeichen
	CString m_sComment;				 // Kommentar
	CString m_sLyrics;				 // Liedtext (Lyrics)
	CString m_sUser[MAX_USER_FIELDS];// Benutzerdefinierte Felder
	long m_lFormat;					 // Format des Liedes (Audio-Track, MP3, WMA, ...)
	long m_lBitRate;				 // Bitrate des Liedes (z.b. 128 KBit = 131072)
	long m_lSampleRate;				 // Samplerate des Liedes (z.b. 44100 Hz)
	long m_lChannels;				 // Anzahl der Kanäle (0 = stereo (default), 1 = mono, 2 = stereo)
	long m_lYearRecorded;            // Aufnahme-Jahr (für Sampler) (0 = unbekannt)

	// Version 10
	CString m_sCheckSum;             // MD5 CheckSum (für MP3s)
	long m_lRating;                  // Rating (z.Zt. 1-5 Sterne)

	// Version 11
	long m_dwIDCategory;			 // Kategorie des Tracks
	long m_dwIDComposer;			 // Komponist
	CString m_sLanguage;		     // Sprache
	CString m_sCategory;			 // Kategory (als Text)
};

class HITDB_INTERFACE CTrack : public CDBQuery, public CTrackData  
{
public:
	CTrack(CDataBase* pDatabase = NULL);
	CTrack(const CTrack& theOther);
	~CTrack();
	DECLARE_DYNAMIC(CTrack)

	void CTrack::operator =(const CTrack &theOther);

public:
	void FillRecordset(CDaoRecordset* prs);
	BOOL Add();
	virtual void Copy(CTrack& theOther);
	BOOL Write();
	CCD* GetCD();
	CArtist* GetArtist();
	CArtist* GetComposer();
	CCategory* GetCategory();

	// Tabellenname
	static const CString GetTableName() { return L"Lied"; }

	long m_dwStartPosition;			 // Startposition des Liedes auf der CD in ms

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CTrack)
	public:
	virtual CString GetDefaultSQL();		// Default SQL for Recordset
	virtual void DoFieldExchange(CDaoFieldExchange* pFX);  // RFX support
	//}}AFX_VIRTUAL

// Implementation
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

	COleVariant GetField(UINT uiField);
	CString GetFieldAsString(UINT uiField);
	void SetField(UINT uiField, const COleVariant& var);

protected:
	CCD* m_pCD;
	CArtist* m_pArtist;				 // Interpret des Liedes
	CArtist* m_pComposer;			 // Komponist des Liedes
	CCategory* m_pCategory;			 // Kategorie des Liedes
};

// Lieder-Liste für CD
class HITDB_INTERFACE CTrackList : public CArray<CTrack, CTrack&>
{
public:
	BOOL AddAll(CCD* pCD);
	CTrackList(long dwIDCD) { m_dwIDCD = dwIDCD; }

/*	CTrack  operator[](int nIndex) 
	{
		if (nIndex >= GetSize())
			SetSize(nIndex+1);

		return GetAt(nIndex);
	}
*/
	long m_dwIDCD;                  // ID, zu der die Lieder gehören
};

#endif // !defined(AFX_TRACK_H__150CFA50_910C_11D2_A5B2_0080AD740CD1__INCLUDED_)
