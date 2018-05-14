// CD.h: interface for the CCD class.
//
//////////////////////////////////////////////////////////////////////

#pragma once

#include "DBQuery.h"

class CArtist;
class CCDSet;
class CIdentity;
class CCategory;
class CMedium;
class CTrackList;
class CProgramList;
class CIndexList;
class CTrack;
class CProgram;
class CIndex;
class CCDExtra;
class CParticipantList;

using namespace Big3::Hitbase::DataBaseEngine;

// CD-Typen (Formate)
#define CDTYPE_AUDIOCD       0          // Normale Audio-CD (Redbook)
#define CDTYPE_MUSIC_DATACD  1          // Musik Daten-CD (mit MP3/WMA/...-Dateien)
#define CDTYPE_SOUND_FILES   2			// Sound-Dateien von Festplatte

class CCDData
{
public:
	long  m_dwID;                 // Interne Datensatz-Nummer (ID)

	long  m_dwTotalLength;        // Länger der CD in ms
	long  m_wNumberOfTracks;      // Anzahl der Lieder (ACHTUNG: künstlich auf long vergrößert, da wir mehr als 255 Lieder für virtuelle CDs unterstützen!)
	
	BOOL  m_bSampler;             // 0 = Normale CD, 1 = Sampler
	
	short m_wCDSetNumber;         // Nummer der CD im CD-Set
	long  m_dwIDCDSet;            // ID des CD-Sets in der CD-Sets Tabelle
	long  m_dwIDArtist;           // ID des Interpreten in der Artist Tabelle
	long  m_dwIDCategory;         // ID der Kategorie
	long  m_dwIDMedium;           // ID des Mediums
	
	CString m_sTitle;             // Titel der CD
	
	CString m_sDate;              // Datum-Feld (Bedeutung frei definierbar)
	CString m_sArchiveNumber;     // Archivnummer
	CString m_sBitmapPath;        // Pfad für Bitmap-Datei (Cover)

	CString m_sCodes;             // Kennzeichen
	CString m_sComment;           // Kommentar

	CString m_sUser[MAX_USER_FIELDS];   // Benutzerdefinierte Felder

	long	m_lType;			  // Typ der CD (Audio-CD, Musik-Daten-CD)

	long	m_lYearRecorded;      // Aufnahme-Jahr
	CString m_sCopyright;         // Copyright-Informationen

	// Version 10
	BOOL m_bOriginal;             // Original-CD (kann ins CD-Archiv übertragen werden)
	CString m_sBackCoverFilename; // Dateiname des Back-Cover (Grafik)
	CString m_sCDLabelBitmap;     // Dateiname des CD-Labels (Grafik)
	long m_lRating;				  // Rating der CD (z.Zt. 1-5 Sterne)
	CString m_sLabel;             // Label (z.B. Sony, EMI, etc.)
	CString m_sURL;               // Verknüpfungen (getrennt mit Semikolon)
	long m_lPrice;                // Preis (Wähung * 100, z.B. 29,99€ = 2999)
	CString m_sUPC;               // UPC oder EAN Code

	// Version 11
	long m_dwIDComposer;		  // ID des Komponisten
	CString m_sLanguage;		  // Sprache der CD
	CString m_sLocation;		  // Standort der CD
};

class HITDB_INTERFACE CCD : public CDBQuery, public CCDData
{
public:
	CCD(CDataBase* pDatabase = NULL);
	CCD(CCD& theOther);
	CCD& operator= (CCD& theOther);
	~CCD();

	DECLARE_DYNAMIC(CCD)

public:
	virtual void Copy(CCD& theOther);
	virtual CIndex* GetIndex(int iIndex);
	virtual CProgram* GetProgram(int iProgram);
	virtual void Clear();
	virtual BOOL Add();
	BOOL Write();
	BOOL Find(const CString& sIdentity);
	BOOL FindUPC(const CString& sUPC);
	unsigned long GetCDDBDiscID();
	unsigned long GetCDDBDiscLength();
	CString GetArtistText();
	CString GetTitleText();
	int ReadOldBuffer(CDataBase* pdb, char *buff);

	int Find(CArray <long, long&> * pRecordNumberArray);
	DWORD IsCDInDataBase(CArray <long, long&> * pRecordNumberArray = NULL);
	DWORD CheckForIdentity();
	DWORD CheckForIdentity(const CString& sIdentity);

	CTrack* GetTrack(int iTrack);
	CIdentity* GetIdentity();
	CArtist* GetArtist();
	CArtist* GetComposer();
	CCDSet* GetCDSet();
	CCategory* GetCategory();
	CMedium* GetMedium();
	CTrackList* GetTracks();
	CProgramList* GetPrograms();
	CIndexList* GetIndexes();
	CParticipantList* GetParticipants();

	// Tabellenname
	static const CString GetTableName() { return L"CD"; }

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CCD)
	public:
	virtual CString GetDefaultSQL();		// Default SQL for Recordset
	virtual void DoFieldExchange(CDaoFieldExchange* pFX);  // RFX support
	//}}AFX_VIRTUAL

// Implementation
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:
	CArtist* m_pArtist;
	CArtist* m_pComposer;
	CCDSet* m_pCDSet;
	CIdentity* m_pIdentity;
	CCategory* m_pCategory;
	CMedium* m_pMedium;
	CTrackList* m_pTracks;
	CProgramList* m_pPrograms;
	CIndexList* m_pIndexes;
	CParticipantList* m_pParticipants;

private:
	int cddb_sum(int n);

	int CopyOldBuff(CString &str1, char *buff, int n, int maxlen);
	int CopyOldBuff(short &str, char *buff);
	int CopyOldBuff(long &str, char *buff);
	int CopyOldBuff(BOOL &str, char *buff);

	DWORD CheckForOldIdentity(CArray <long, long&> *pRecordNumberArray = NULL);
	DWORD CheckForXMCDIdentity();
public:
	bool AdjustSpelling(int iAdjustSpelling);
private:
	void AdjustString(CString& sString, int iAdjustSpelling);
public:
	COleVariant GetField(UINT uiField);
	void SetField(UINT uiField, const COleVariant& var);
	CString GetFieldAsString(UINT uiField);
	void UpdateTotalTime(void);
	unsigned long GetRoundedTime(long t);
	void CopyToNewFormat(gcroot<Big3::Hitbase::DataBaseEngine::CD^> newCDData);
	void CopyFromNewFormat(gcroot<Big3::Hitbase::DataBaseEngine::CD^> newCDData);
};

