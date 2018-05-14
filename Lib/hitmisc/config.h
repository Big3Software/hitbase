#ifndef _CLASS_CONFIG
#define _CLASS_CONFIG

int const MAX_TOOLS = 20;      // Maximale Anzahl von Tools
//int const MAX_CATEGORIES = 100;
//int const MAX_CODES = 26;           // Wegen 26 Buchstaben!
//int const MAX_MEDIUM = 100;

int const NUMBER_OF_EVENTS = 12;        // Achtung! Muß geändert werden, wenn sich was ändert!

// Die einzelnen Datums-Formate
#define DATE_TTMMJJJJ 0
#define DATE_MMJJJJ   1
#define DATE_JJJJ     2
#define DATE_NONE     3

struct tagTOOL 
{
    CString Command;
    CString Text;
    CString InitDir;
};

#define ARCHIVFIELD_CATEGORY      1
#define ARCHIVFIELD_MEDIUM        2
#define ARCHIVFIELD_CDCOMMENT     4
#define ARCHIVFIELD_BPM           8
#define ARCHIVFIELD_TRACKCOMMENT  16
#define ARCHIVFIELD_LYRICS        32

#ifdef _HITMISC_DLL
#define HITMISC_INTERFACE __declspec ( dllexport )
#else
#define HITMISC_INTERFACE __declspec ( dllimport )
#endif

#include "FieldList.h"

//own_extern struct tagCONFIG {
class HITMISC_INTERFACE CConfig {
public:
    CConfig();
	
	int ReadRegistry(BOOL bUpdate = FALSE);
	static int ReadGlobalRegistryKeyInt(const CString& szKeyName, int iDefault);
	static CString ReadGlobalRegistryKeyString(const CString& szKeyName, const CString& szDefault);
	static void ReadGlobalRegistryKeyBinary(const CString& szKeyName, void* pszDefault, void *pBuf, int nLen);
	
	static CString RegQueryString(HKEY hKey, const CString& szSubKey, const CString &sDefault);
	static long RegQueryInt(HKEY hKey, const CString& szSubKey, int idefault);
	static void RegQueryBinary(HKEY hKey, const CString& subKey, void* pValue, int nLen);

	static void RegWriteString(HKEY hKey, const CString& szSubKey, const CString &sValue);
	static void RegWriteInt(HKEY hKey, const CString& szSubKey, int iValue);
	static void RegWriteBinary(HKEY hKey, const CString& szSubKey, void* pValue, int size);
	
	static HKEY CreateSubKey(const CString& sSubKey);
	static void CloseKey(HKEY hKey);

	int WriteRegistry();				//Schreibt alle Einstellungen in die Registry
	static void WriteGlobalRegistryKeyInt(const CString& szKeyName, int iValue);
	static void WriteGlobalRegistryKeyString(const CString& szKeyName, const CString& str);
	static void WriteGlobalRegistryKeyBinary(const CString& szKeyName, char* pValue, int nLen);
	
public:
	CString m_sHitbaseServer;
	void SetRegistryKey(const CString& sRegKey);
	CString m_sRegBaseKey;

	BOOL m_bDontSearchDataCDsInCDArchive;
	CString m_sPlugInDirectory;
	BOOL m_bResetCDRomOnStop;
	BOOL m_bLightsAnimation;
	BOOL m_bNoDupArchiveNr;
	BOOL m_bAutoPlay;
	BOOL m_bShowRemainTime;
	BOOL m_bAutoDateToday;
	CString m_CDCoverPath;
	CString m_sRecordPath;
	int m_nCDPlayerAlign;
	int m_nPrintCDCoverAlign;
	BOOL m_bPrintTextUnderCDCover;
	int m_nPrintCDCoverSize;
	BOOL m_bPrintCDCover;
	int m_PrintFontSize;
	CString m_PrintFontName;
	int m_nLapView;
	BOOL m_bAutoSearch;
	COLORREF m_colorCurrentTrackBackground;
	
	CString m_LastDatabase;
	
	int IntroSeconds;
	int IntroStart;
	
	int CDBusyLight;
	int CDDevice;                 // Zuletzt benutztes CDROM-Laufwerk
	BOOL m_bVirtualCD;            // = TRUE, wenn virtuelle CD aktiv war.
	
	BOOL m_bShowGrid;
	BOOL m_bShowToolBar;
	BOOL m_bShowStatusBar;
	BOOL m_bShowMenu;
	
	BOOL m_bAlwaysOnTop;
	
	BOOL AutoNewOpen;
	BOOL ShowOnlyUsedCodes;
	BOOL AutoBackup;              // Automatische Sicherheitskopie beim Laden
	CString m_sAutoBackupDirectory;    // und das Verzeichnis dafür
	
	BOOL BackgroundSearch;
	
	BOOL AutoShowCover;
	BOOL StretchCover;
	
	int MeasureUnit;              // 0 = Millimeter, 1 = Zoll
	double LeftMargin;
	double RightMargin;
	double TopMargin;
	double BottomMargin;
	
	CString SoundEvent[20];
	
	struct tagTOOL Tools[MAX_TOOLS];
	
	int NumberOfTools;
	
//	CFieldList m_ShowFieldsCD;
//	CFieldList m_ShowFieldsVirtualCD;    // Lied-Felder, die bei virtueller CD angezeigt werden sollen
	
	int EqualizerQuality;
	
	int OnNewCD;
	int OnExit;
	int OnStart;
	BOOL AutoRepeat;
	
	int ShowSymbol;
	int SymbolInTaskbar;
	int ShowSplashScreen;
	
	int MaxShowCDs;
	
	RECT SmallWindowRect;             // Position des kleinen Bedienfelds
	BOOL m_bSmallCDPlayer;
	
	COLORREF m_colorCDPlayer1;           // Farbe 1 des CDSpieler-Gehäuses
	COLORREF m_colorCDPlayer2;           // Farbe 2 des CDSpieler-Gehäuses
	COLORREF m_colorNumbers;            // Farbe der Zahlen im Display
	COLORREF m_colorNumbersGray;        // Farbe des grauen Anteils der Zahlen im Display
	COLORREF m_colorDisplayBackground;  // Farbe des Anzeigen-Hintergrunds
	CString m_PatternPath;            // Bitmap für CDSpieler-Gehäuse
	int m_nBackgroundType;      // = 0 : Glatt, = 1 : Strukturiert
	
	int m_nDelay;                  // Verzögerung für Volume-Fader
	int m_nFadeSecs;               // Dauer des "Fadens"

	// JUS 990323
	BOOL m_bDisableInterrupts;     // Bei Fullscreen Plugs-Ins kein Update der Anzeige!

	// JUS 990816
	BOOL m_bAutoCheckNewVersion;   // Auf neue Version im Internet prüfen
	BOOL m_bAutoCheckAnnouncement; // Auf wichtige Nachricht im Internet prüfen

	// JUS 000805
	int m_iVirtualCDOutputDevice; // Output-Device für virtuelle cd (z.Zt. 0 = DirectSound, 1 = Wave output)

	// JUS 000819
	BOOL m_bShowCDPlayer;         // CD-Spieler anzeigen oder nicht

	// JUS 000917
	int m_iVolumeDevice;          // Volume-Device für die Lautstärkeregelung auf dem CD-Player

	// JUS 010826
	BOOL m_bCenterCurrentTrack;   // Aktuell spielendes Lied in der Lieder-Liste zentrieren (also immer sichtbar)

	// JUS 011127
	BOOL m_bSkinActive;           // Skin ist aktiv.
	CString m_sLastSkin;          // Das zuletzt aktivierte Skin.
	CString m_sSkinDirectory;     // Verzeichnis, in dem die Skins liegen.
	BOOL m_bSkinMouseDisableTransparenz; // Transparenz des Skins wird deaktiviert, wenn die Maus im Skin ist.

	// JUS 020221
	BOOL m_iGroupArtistBoundary;  // Grenze, ab der die CDs in der Anzeige gruppiert werden.

	// jUS 020802
	CString m_sPrintHeaderText0;
	CString m_sPrintHeaderText1;
	CString m_sPrintHeaderText2;
	CString m_sPrintHeaderText3;
	CString m_sPrintHeaderText4;
	BOOL m_bPrintHeaderShowSort;
	BOOL m_bPrintHeaderShowFilter;

	// JUS 020810
	int m_iVirtualCDBufferSize;   // Buffer-Größe der MP3-Engine (FMOD)

	// JUS 020819
	int m_iRecordOptionsJitterReadTracks;
	int m_iRecordOptionsJitterCheck;
	int m_iRecordOptionsJitterOverlap;

	// JUS 020903
	int m_iArtistDisplay;     // Bei Interpreten "Name" (=0) oder "Speichern unter" (=1) anzeigen

	// JUS 020922
	BOOL m_bAutoInsertQueue;    // Suche nach eingelegten CDs nicht direkt ausführen (in Warteschlange einfügen)

	// JUS 021002         // Window-Transparent (Designmodus)
	int m_iTransparenzProzentSichtbar;

	// JUS 021224
	BOOL m_bDisableCDText;   // CD-Text Funktionen deaktivieren

	// JUS 021230
	int m_iReadAlgorithm;    // Lese-Algorithmus für DAE (AKRIP)

	// JUS 030324
	int m_iCatalogViewMode;  // Ansicht in Funktion "Katalog->Bearbeiten"

	// JUS 030615
	BOOL m_bCDOutputDigital;            // Digitale CD-Wiedergabe

	// JUS 0306328
	int m_iCDCoverDisplaySize;          // Größe des CD-Covers im Hauptfenster
	int m_iCDCoverShowWhat;             // Welche CD-Cover im Hauptfenster zeigen (0=keins,1=Front,2=Back,3=Beide)

	// JUS 030822
	BOOL m_bAutoCompleteArtist;
	BOOL m_bAutoCompleteCDTitle;
	BOOL m_bAutoCompleteTrackname;

	// JUS 031019
	BOOL m_bAutoVolumeAdjust;           // Automatische Lautstärkeregelung

	// JUD 040505
	long m_lAutoVolumeAdjustMax;	// Maximale Lautstärkeanpassung (in prozent)

	// JUS 031025
	int m_nRecordCopyMode;
	int m_nRecordSpinUpDrive;
	int m_nRecordSpinUpWaitTimeSec;
//	BOOL m_bRecordForceASPI;             JUS 29.10.2003: Zuerst mal wieder raus, da ich von der akrip.dll aus nicht auf Klassen zugreifen kann (nur C!)

	// JUS 031129
	BOOL m_bAutoSaveCDs;

	// JUS 031226
	BOOL m_bFirstStart;					// TRUE, wenn Hitbase nach der Installation zum ersten mal gestartet wird.

	// JUS 040412
	BOOL m_bCrossFade;					// TRUE, wenn Überblenden von Musikstücken aktiv ist.
	int m_iCrossFadeDefaultSeconds;		// Standardwert für die Überblendung in Sekunden

	// JUS 040508
	BOOL m_bPlaylistVisible;				// Ist die Playlist sichtbar?

	// JUS 041005
	BOOL m_bGroupDisplay;					// Gruppieren in der Interpreten-Übersicht

	// JUS 041103
	BOOL m_bDisableAutoPlay;				// Autoplay deaktivieren

	// JUS 051225
	//CString m_sDefaultImageDirectory;		// Standardverzeichnis für Bilder

	// JUS 060129
	int m_iSearchBarSearchWhere;			// Suchleiste im Katalog (wo suchen?)

	// JUS 060610
	int m_iStopTimeBeforeEnd;		        // x Sekunden vor Ende des Liedes stoppen

	// JUS 061205
	int m_iSkinSignalDisplayMode;			// Der Display-Mode für das Signal im Skin
};

#endif
