// hitconf.cpp - Konfigurationsklasse von Hitbase        11-Apr-96

#include "stdafx.h"
#include "hitmisc.h"
#include "HitbaseWinAppBase.h"
#include "config.h"
#include "misc.h"
#include "../../app/hitbase/resource.h"

CConfig::CConfig()
{
	m_sRegBaseKey = REGISTRY_KEY;

	int i;
	
	m_nBackgroundType = 0;
	
	IntroSeconds = 0;
	IntroStart= 0;
	
	CDBusyLight = TRUE;
	CDDevice = 0;                 // Zuletzt benutztes CDROM-Laufwerk
	
	m_bAlwaysOnTop = FALSE;
	
	m_bShowToolBar = TRUE;
	m_bShowStatusBar = TRUE;
	m_bShowGrid = TRUE;
	m_bShowMenu = TRUE;
	
	BackgroundSearch = FALSE;
	ShowSplashScreen = TRUE;

	m_bSmallCDPlayer = FALSE;
	
	AutoNewOpen = FALSE;
	ShowOnlyUsedCodes = FALSE;
	AutoBackup = FALSE;           // Automatische Sicherheitskopie beim Laden
	
	BackgroundSearch = FALSE;
	
	AutoShowCover = FALSE;
	StretchCover = FALSE;
	
	MeasureUnit = 0;              // 0 = Millimeter, 1 = Zoll
	LeftMargin = 0.0;
	RightMargin = 0.0;
	TopMargin = 0.0;
	BottomMargin = 0.0;
	
	for (i=0;i<20;i++)
		SoundEvent[i] = "";
	
	/*for (i=0;i<MAX_TOOLS;i++)
		memset(&Tools, 0, sizeof(tagTOOL));*/
	NumberOfTools = 0;
	
	EqualizerQuality = 0;
	
	OnNewCD = 0;
	OnExit = 0;
	OnStart = 0;
	AutoRepeat = FALSE;

	m_bShowRemainTime = FALSE;
	
	ShowSymbol = 0;
	SymbolInTaskbar = 0;
//	ShowSplashScreen = 0;
	
	MaxShowCDs = 0;
	
	memset(&SmallWindowRect, 0, sizeof(RECT));     // Position des kleinen Bedienfelds
	
	System::Windows::Forms::ProfessionalColorTable t;
    COLORREF color1 = RGB(t.MenuStripGradientBegin.R, t.MenuStripGradientBegin.G, t.MenuStripGradientBegin.B);
    COLORREF color2 = RGB(t.MenuStripGradientEnd.R, t.MenuStripGradientEnd.G, t.MenuStripGradientEnd.B);
		
	m_colorCDPlayer1 = color1;
	m_colorCDPlayer2 = color2;;
	m_colorNumbers = RGB(0, 255, 255);
	m_colorNumbersGray = RGB(128, 128, 128);
	m_colorDisplayBackground = RGB(0, 0, 0);
	m_colorCurrentTrackBackground = RGB(128, 128, 128);
	
	IntroStart = 0;
	IntroSeconds = 5;

	m_nDelay = 2;
	m_nFadeSecs = 5;

	m_bShowGrid = TRUE;
	
	m_bResetCDRomOnStop = TRUE;
	m_bNoDupArchiveNr = TRUE;
	ShowOnlyUsedCodes = TRUE;

	m_bAutoDateToday = TRUE;

	m_bAutoSearch = TRUE;                 // Automatisch Suche beginnen
	
	m_PrintFontName = "Times New Roman";
	m_PrintFontSize = 10;
	
	m_nPrintCDCoverSize = 50;           // 50% der Seitenbreite
	m_bPrintCDCover = TRUE;             // CD-Cover standardmäßig drucken
	m_bPrintTextUnderCDCover = TRUE;
	m_nPrintCDCoverAlign = 0;
	
	m_nCDPlayerAlign = 1;               // Standardmäßig mittig ausrichten
	
	m_bAutoPlay = TRUE;

	m_bLightsAnimation = TRUE;

	LeftMargin = 20;
	TopMargin = 20;
	RightMargin = 20;
	BottomMargin = 20;

	m_bSmallCDPlayer = FALSE;

	m_nFadeSecs = 5;
	m_nDelay = 2;

	m_bAutoPlay = TRUE;

	m_bLightsAnimation = TRUE;

	m_bFirstStart = FALSE;
}

// Daten aus der Registry lesen.
// Wenn bUpdate auf True steht, dann werden Informationen aus einem alten Hitbase-Key
// gelesen. Die Einstellungen werden also übernommen. Das Flag gibt es deswegen, weil
// manche Einstellungen von einer alten Version aus technischen Gründen nicht über-
// nommen werden können.

int CConfig::ReadRegistry(BOOL bUpdate /* = FALSE */)
{
	// Als allerallererstes wird jetzt geprüft, ob von einer alten Hitbase-Version
	// die Einstellungen übernommen werden sollen. Dies wird nur beim ersten Programm-
	// start einmal ausgeführt.

	if (ReadGlobalRegistryKeyInt(L"FirstStart", 1) == 1)
	{
		WriteGlobalRegistryKeyInt(L"FirstStart", 0);

		m_bFirstStart = TRUE;

		HKEY hHitKeyUpdate;
		if (RegOpenKeyEx(HKEY_CURRENT_USER, L"SOFTWARE\\Big 3\\Hitbase 2003", 0, KEY_ALL_ACCESS, &hHitKeyUpdate) == ERROR_SUCCESS)
		{
			RegCloseKey(hHitKeyUpdate);
			SetRegistryKey(L"SOFTWARE\\Big 3\\Hitbase 2003");
			ReadRegistry(TRUE);
			SetRegistryKey(L"");

			// Alten Werte in den neuen Registry-Pfad schreiben
			WriteRegistry();
		}
		else
		if (RegOpenKeyEx(HKEY_CURRENT_USER, L"SOFTWARE\\Big 3\\Hitbase 2001", 0, KEY_ALL_ACCESS, &hHitKeyUpdate) == ERROR_SUCCESS)
		{
			RegCloseKey(hHitKeyUpdate);
			SetRegistryKey(L"SOFTWARE\\Big 3\\Hitbase 2001");
			ReadRegistry(TRUE);
			SetRegistryKey(L"");

			// Alten Werte in den neuen Registry-Pfad schreiben
			WriteRegistry();
		}
		else
		if (RegOpenKeyEx(HKEY_CURRENT_USER, L"SOFTWARE\\Big 3\\Hitbase 98", 0, KEY_ALL_ACCESS, &hHitKeyUpdate) == ERROR_SUCCESS)
		{
			RegCloseKey(hHitKeyUpdate);
			SetRegistryKey("SOFTWARE\\Big 3\\Hitbase 98");
			ReadRegistry(TRUE);
			SetRegistryKey("");

			// Alten Werte in den neuen Registry-Pfad schreiben
			WriteRegistry();
		}
		else
		if (RegOpenKeyEx(HKEY_CURRENT_USER, L"SOFTWARE\\Big 3\\Hitline 97", 0, KEY_ALL_ACCESS, &hHitKeyUpdate) == ERROR_SUCCESS)
		{
			RegCloseKey(hHitKeyUpdate);
			SetRegistryKey("SOFTWARE\\Big 3\\Hitline 97");
			ReadRegistry(TRUE);
			SetRegistryKey("");

			// Alten Werte in den neuen Registry-Pfad schreiben
			WriteRegistry();
		}
	}

	DWORD ret;
	wchar_t str[200];
	wchar_t contents[100];
	int i=0;
	HKEY hHitKeyGlobTools;
	HKEY hHitKeyTools;
	HKEY hHitKeyGlobal;
	HKEY hHitKeyEvents;
	
	RegCreateKeyEx(HKEY_CURRENT_USER, m_sRegBaseKey, 0,
		L"Hitbase", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL,
		&hHitKeyGlobal, &ret);
	
	RegCreateKeyEx(HKEY_CURRENT_USER, m_sRegBaseKey + L"\\Events", 0,
		L"Hitbase", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL,
		&hHitKeyEvents, &ret);
	
	ret = _MAX_PATH;
	RegQueryValueEx(hHitKeyGlobal, L"BackgroundPattern", 0, NULL, (unsigned char *)m_PatternPath.GetBuffer(_MAX_PATH), &ret);
	m_PatternPath.ReleaseBuffer();

	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"BackgroundType", 0, NULL, (unsigned char *)&m_nBackgroundType, &ret);

	ret = sizeof(COLORREF);
	RegQueryValueEx(hHitKeyGlobal, L"BackgroundColor1", 0, NULL, (unsigned char *)&m_colorCDPlayer1, &ret);
	ret = sizeof(COLORREF);
	RegQueryValueEx(hHitKeyGlobal, L"BackgroundColor2", 0, NULL, (unsigned char *)&m_colorCDPlayer2, &ret);
	
	ret = sizeof(COLORREF);
	RegQueryValueEx(hHitKeyGlobal, L"DisplayBgColor", 0, NULL, (unsigned char *)&m_colorDisplayBackground, &ret);
	
	ret = sizeof(COLORREF);
	RegQueryValueEx(hHitKeyGlobal, L"DisplayLCDOnColor", 0, NULL, (unsigned char *)&m_colorNumbers, &ret);
	
	ret = sizeof(COLORREF);
	RegQueryValueEx(hHitKeyGlobal, L"DisplayLCDOffColor", 0, NULL, (unsigned char *)&m_colorNumbersGray, &ret);
	
	ret = sizeof(COLORREF);
	RegQueryValueEx(hHitKeyGlobal, L"DisplayBgCurrentTrackColor", 0, NULL, (unsigned char *)&m_colorCurrentTrackBackground, &ret);
	
	ret = _MAX_PATH;
	RegQueryValueEx(hHitKeyGlobal, L"LastDatabase", 0, NULL, (unsigned char *)m_LastDatabase.GetBuffer(_MAX_PATH), &ret);
	m_LastDatabase.ReleaseBuffer();
	
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"IntroStart", 0, NULL, (unsigned char *)&IntroStart, &ret);
	
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"IntroSeconds", 0, NULL, (unsigned char *)&IntroSeconds, &ret);

// JUS 010826: Immer TRUE!
//	ret = sizeof(int);
//	RegQueryValueEx(hHitKeyGlobal, L"ResetCDRomOnStop", 0, NULL, (unsigned char *)&m_bResetCDRomOnStop, &ret);
	
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"ShowGrid", 0, NULL, (unsigned char *)&m_bShowGrid, &ret);

	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"ShowToolBar", 0, NULL, (unsigned char *)&m_bShowToolBar, &ret);

	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"ShowStatusBar", 0, NULL, (unsigned char *)&m_bShowStatusBar, &ret);
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"ShowMenu", 0, NULL, (unsigned char *)&m_bShowMenu, &ret);
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"AlwaysOnTop", 0, NULL, (unsigned char *)&m_bAlwaysOnTop, &ret);
	
// JUS 010826: Immer TRUE!
//	ret = sizeof(int);
//	RegQueryValueEx(hHitKeyGlobal, L"CDBusyLight", 0, NULL, (unsigned char *)&CDBusyLight, &ret);
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"AutoNewOpen", 0, NULL, (unsigned char *)&AutoNewOpen, &ret);
	
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"AutoBackup", 0, NULL, (unsigned char *)&AutoBackup, &ret);
	
	if (!bUpdate)
		m_sAutoBackupDirectory = RegQueryString(hHitKeyGlobal, L"AutoBackupDirectory", ((CHitbaseWinAppBase*)AfxGetApp())->m_sExecutablePath);
	
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"ShowOnlyUsedCodes", 0, NULL, (unsigned char *)&ShowOnlyUsedCodes, &ret);
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"AutoSearch", 0, NULL, (unsigned char *)&m_bAutoSearch, &ret);
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"NoDupArchiveNumber", 0, NULL, (unsigned char *)&m_bNoDupArchiveNr, &ret);
	
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"AutoShowCover", 0, NULL, (unsigned char *)&AutoShowCover, &ret);
	
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"AutoDateToday", 0, NULL, (unsigned char *)&m_bAutoDateToday, &ret);
	
	ret = MAX_PATH;
	RegQueryValueEx(hHitKeyGlobal, L"LastCDCoverPath", 0, NULL, (unsigned char *)m_CDCoverPath.GetBuffer(MAX_PATH), &ret);
	m_CDCoverPath.ReleaseBuffer();
	
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"BackgroundSearch", 0, NULL, (unsigned char *)&BackgroundSearch, &ret);
	
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"OnNewCD", 0, NULL, (unsigned char *)&OnNewCD, &ret);
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"OnExit", 0, NULL, (unsigned char *)&OnExit, &ret);
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"OnStart", 0, NULL, (unsigned char *)&OnStart, &ret);
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"AutoRepeat", 0, NULL, (unsigned char *)&AutoRepeat, &ret);
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"LAPView", 0, NULL, (unsigned char *)&m_nLapView, &ret);
	
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"ShowRemainTime", 0, NULL, (unsigned char *)&m_bShowRemainTime, &ret);
	
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"ShowSymbol", 0, NULL, (unsigned char *)&ShowSymbol, &ret);
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"SymbolInTaskbar", 0, NULL, (unsigned char *)&SymbolInTaskbar, &ret);
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"EqualizerQuality", 0, NULL, (unsigned char *)&EqualizerQuality, &ret);
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"CDDevice", 0, NULL, (unsigned char *)&CDDevice, &ret);
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"VirtualCD", 0, NULL, (unsigned char *)&m_bVirtualCD, &ret);
	if (((CHitbaseWinAppBase*)AfxGetApp())->m_iCommandLineCDDevice >= 0)   // Kommandozeile überschreibt diesen Wert!
		CDDevice = ((CHitbaseWinAppBase*)AfxGetApp())->m_iCommandLineCDDevice;
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"MaxShowCDs", 0, NULL, (unsigned char *)&MaxShowCDs, &ret);
	
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"ShowSplashScreen", 0, NULL, (unsigned char *)&ShowSplashScreen, &ret);
	
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"MeasureUnit", 0, NULL, (unsigned char *)&MeasureUnit, &ret);
	ret = sizeof(double);
	RegQueryValueEx(hHitKeyGlobal, L"LeftMargin", 0, NULL, (unsigned char *)&LeftMargin, &ret);
	ret = sizeof(double);
	RegQueryValueEx(hHitKeyGlobal, L"TopMargin", 0, NULL, (unsigned char *)&TopMargin, &ret);
	ret = sizeof(double);
	RegQueryValueEx(hHitKeyGlobal, L"RightMargin", 0, NULL, (unsigned char *)&RightMargin, &ret);
	ret = sizeof(double);
	RegQueryValueEx(hHitKeyGlobal, L"BottomMargin", 0, NULL, (unsigned char *)&BottomMargin, &ret);
	
	ret = sizeof(RECT);
	RegQueryValueEx(hHitKeyGlobal, L"SmallWindowPos", 0, NULL, (unsigned char *)&SmallWindowRect, &ret);
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"SmallCDPlayer", 0, NULL, (unsigned char *)&m_bSmallCDPlayer, &ret);
	
	ret = 100;
	RegQueryValueEx(hHitKeyGlobal, L"PrintFontName", 0, NULL, (unsigned char *)m_PrintFontName.GetBuffer(100), &ret);
	m_PrintFontName.ReleaseBuffer();
	
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"PrintFontSize", 0, NULL, (unsigned char *)&m_PrintFontSize, &ret);
	
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"PrintCDCoverSize", 0, NULL, (unsigned char *)&m_nPrintCDCoverSize, &ret);
	
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"PrintCDCover", 0, NULL, (unsigned char *)&m_bPrintCDCover, &ret);
	
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"PrintCDCoverAlign", 0, NULL, (unsigned char *)&m_nPrintCDCoverAlign, &ret);
	
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"PrintTextUnderCDCover", 0, NULL, (unsigned char *)&m_bPrintTextUnderCDCover, &ret);
	
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"CDPlayerAlign", 0, NULL, (unsigned char *)&m_nCDPlayerAlign, &ret);
	
	m_bDontSearchDataCDsInCDArchive = RegQueryInt(hHitKeyGlobal, L"DontSearchDataCDsInCDArchive", TRUE);
	
	//CString sSoundDir = ((CHitbaseWinAppBase*)AfxGetApp())->m_sExecutablePath + L"record\\";
	CString sSoundDir = CMisc::GetPersonalHitbaseFolder() + "\\record\\";
	m_sRecordPath = RegQueryString(hHitKeyGlobal, L"DefaultRecordPath", sSoundDir);
	if (m_sRecordPath.IsEmpty())
		m_sRecordPath = sSoundDir;

	// Kann im Programm nicht eingestellt werden, da eigentlich fest!
	m_sHitbaseServer = RegQueryString(hHitKeyGlobal, L"HitbaseServer", "www.hitbase.de");

	if (!bUpdate)
		m_sPlugInDirectory = RegQueryString(hHitKeyGlobal, L"PlugInDirectory", L"");

	if (m_sPlugInDirectory.IsEmpty())
		m_sPlugInDirectory = ((CHitbaseWinAppBase*)AfxGetApp())->m_sExecutablePath + L"plug-ins";

	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"VolumeFadeSpeed", 0, NULL, (unsigned char *)&m_nFadeSecs, &ret);
	
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"VolumeFadeWait", 0, NULL, (unsigned char *)&m_nDelay, &ret);
	
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"AutoPlay", 0, NULL, (unsigned char *)&m_bAutoPlay, &ret);
	
	// JUS 990323
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"DisableInterrupts", 0, NULL, (unsigned char *)&m_bDisableInterrupts, &ret);

	// JUS 990816
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"AutoCheckNewVersion", 0, NULL, (unsigned char *)&m_bAutoCheckNewVersion, &ret);

	// JUS 990816
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"AutoCheckAnnouncement", 0, NULL, (unsigned char *)&m_bAutoCheckAnnouncement, &ret);

	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, L"LightsAnimation", 0, NULL, (unsigned char *)&m_bLightsAnimation, &ret);

	// JUS 000805
	m_iVirtualCDOutputDevice = RegQueryInt(hHitKeyGlobal, L"VirtualCDOutputDevice", 0);   // Output-Device für virtuelle cd (z.Zt. 0 = DirectSound, 1 = Wave output)

	// JUS 000819
	m_bShowCDPlayer = RegQueryInt(hHitKeyGlobal, L"ShowCDPlayer", TRUE);

	// JUS 000917
	m_iVolumeDevice = RegQueryInt(hHitKeyGlobal, L"VolumeDevice", MASTER_VOLUME);

	// JUS 010826
	m_bCenterCurrentTrack = RegQueryInt(hHitKeyGlobal, L"CenterCurrentTrack", TRUE);

	// JUS 011127
	m_bSkinActive = RegQueryInt(hHitKeyGlobal, L"SkinActive", FALSE);
	m_sLastSkin = RegQueryString(hHitKeyGlobal, L"LastSkin", L"");
	if (!bUpdate)
		m_sSkinDirectory = RegQueryString(hHitKeyGlobal, L"SkinDirectory", L"");

	if (m_sSkinDirectory.IsEmpty())
		m_sSkinDirectory = ((CHitbaseWinAppBase*)AfxGetApp())->m_sExecutablePath + L"skins";

	m_bSkinMouseDisableTransparenz  = RegQueryInt(hHitKeyGlobal, L"SkinMouseDisableTransparenz", FALSE);
	m_iTransparenzProzentSichtbar = RegQueryInt(hHitKeyGlobal, L"WindowTransparenz", 100);

	// JUS 020221
	m_iGroupArtistBoundary = RegQueryInt(hHitKeyGlobal, L"GroupArtistBoundary", 1000);

	// JUS 020802: Kopfzeilen beim Ausdruck
	m_sPrintHeaderText0 = CConfig::ReadGlobalRegistryKeyString(L"PrintHeaderText0", get_string(TEXT_CATSUM));
	m_sPrintHeaderText1 = CConfig::ReadGlobalRegistryKeyString(L"PrintHeaderText1", get_string(TEXT_DETAILEDINFO));
	m_sPrintHeaderText2 = CConfig::ReadGlobalRegistryKeyString(L"PrintHeaderText2", get_string(TEXT_DETAILEDINFO));
	m_sPrintHeaderText3 = CConfig::ReadGlobalRegistryKeyString(L"PrintHeaderText3", get_string(TEXT_TRACKLIST));
	m_sPrintHeaderText4 = CConfig::ReadGlobalRegistryKeyString(L"PrintHeaderText4", get_string(IDS_LIST_LOANCDS));
	m_bPrintHeaderShowSort = CConfig::ReadGlobalRegistryKeyInt(L"PrintHeaderShowSort", TRUE);
	m_bPrintHeaderShowFilter = CConfig::ReadGlobalRegistryKeyInt(L"PrintHeaderShowFilter", TRUE);

	// JUS 020810
	m_iVirtualCDBufferSize = RegQueryInt(hHitKeyGlobal, L"VirtualCDBufferSize", 5);

	// JUS 020819
	m_iRecordOptionsJitterReadTracks = RegQueryInt(hHitKeyGlobal, L"RecordOptionsJitterReadTracks", 26);
	m_iRecordOptionsJitterCheck = RegQueryInt(hHitKeyGlobal, L"RecordOptionsJitterCheck", 1);
	m_iRecordOptionsJitterOverlap = RegQueryInt(hHitKeyGlobal, L"RecordOptionsJitterOverlap", 3);

	// JUS 020903
	m_iArtistDisplay = RegQueryInt(hHitKeyGlobal, L"ArtistDisplay", 0);

	// JUS 020922
	m_bAutoInsertQueue = RegQueryInt(hHitKeyGlobal, L"AutoInsertQueue", FALSE);

	// JUS 021224
	m_bDisableCDText = RegQueryInt(hHitKeyGlobal, L"DisableCDText", FALSE);

	// JUS 021230
	m_iReadAlgorithm = RegQueryInt(hHitKeyGlobal, L"ReadAlgorithm", 0);

	// JUS 030324
	m_iCatalogViewMode = RegQueryInt(hHitKeyGlobal, L"CatalogViewMode", 0);

	// JUS 030615
	m_bCDOutputDigital = RegQueryInt(hHitKeyGlobal, L"CDOutputDigital", FALSE);

	// JUS 030628
	m_iCDCoverDisplaySize = RegQueryInt(hHitKeyGlobal, L"CDCoverDisplaySize", 160);
	m_iCDCoverShowWhat = RegQueryInt(hHitKeyGlobal, L"CDCoverDisplayWhat", 0);

	// JUS 030822
	m_bAutoCompleteArtist = RegQueryInt(hHitKeyGlobal, L"AutoCompleteArtist", TRUE);
	m_bAutoCompleteCDTitle = RegQueryInt(hHitKeyGlobal, L"AutoCompleteCDTitle", TRUE);
	m_bAutoCompleteTrackname = RegQueryInt(hHitKeyGlobal, L"AutoCompleteTrackname", TRUE);

	// JUS 031019
	m_bAutoVolumeAdjust = RegQueryInt(hHitKeyGlobal, L"AutoVolumeAdjust", FALSE);
	m_lAutoVolumeAdjustMax = RegQueryInt(hHitKeyGlobal, L"AutoVolumeAdjustMax", 200);

	// JUS 031025
	m_nRecordCopyMode = RegQueryInt(hHitKeyGlobal, L"CopyMode", 0);
	m_nRecordSpinUpDrive = RegQueryInt(hHitKeyGlobal, L"SpinUpDrive", 0);
	m_nRecordSpinUpWaitTimeSec = RegQueryInt(hHitKeyGlobal, L"SpinUpWaitTimeSec", 0);
//	m_bRecordForceASPI = RegQueryInt(hHitKeyGlobal, L"RecordForceASPI", FALSE);

	// JUS 031129
	m_bAutoSaveCDs = RegQueryInt(hHitKeyGlobal, L"AutoSaveCDs", TRUE);

	// JUS 040412
	m_bCrossFade = RegQueryInt(hHitKeyGlobal, L"CrossFade", FALSE);
	m_iCrossFadeDefaultSeconds = RegQueryInt(hHitKeyGlobal, L"CrossFadeDefaultSeconds", 10);

	// JUS 040508
	m_bPlaylistVisible = RegQueryInt(hHitKeyGlobal, L"PlaylistVisible", FALSE);

	// JUS 041005
	m_bGroupDisplay = RegQueryInt(hHitKeyGlobal, L"GroupDisplay", FALSE);

	// JUS 041103
	m_bDisableAutoPlay = RegQueryInt(hHitKeyGlobal, L"DisableAutoPlay", TRUE);

	// JUS 051226
	/*CString defaultImageDir = CMisc::GetPersonalHitbaseFolder() + "\\Images";
	m_sDefaultImageDirectory = RegQueryString(hHitKeyGlobal, L"DefaultImageDirectory", defaultImageDir);
	CMisc::CreateDirectory(m_sDefaultImageDirectory);		// Verzeichnis anlegen, muss immer da sein.
*/

	// JUS 060129
	m_iSearchBarSearchWhere = RegQueryInt(hHitKeyGlobal, L"SearchBarSearchWhere", 0);

	// JUS 060610
	m_iStopTimeBeforeEnd = RegQueryInt(hHitKeyGlobal, L"StopTimeBeforeEnd", 0);

	// JUS 061205
	m_iSkinSignalDisplayMode = RegQueryInt(hHitKeyGlobal, L"SkinSignalDisplayMode", 0); // 0 = DISPLAYMODE_SIGNAL;

	for (i=0;i<NUMBER_OF_EVENTS;i++)
	{
		ret = _MAX_PATH;
		swprintf(str, L"%d", i);
		contents[0] = 0;
		RegQueryValueEx(hHitKeyEvents, str, 0, NULL, (BYTE*)contents, &ret);
		SoundEvent[i] = contents;
	}

	swprintf(str, m_sRegBaseKey + L"\\Tools");
	RegCreateKeyEx(HKEY_CURRENT_USER, str, 0,
		L"Hitbase", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL,
		&hHitKeyGlobTools, &ret);
	
	for (i=0;i<MAX_TOOLS;i++)
	{
		swprintf(str, L"%d", i+1);
		ret = RegOpenKeyEx(hHitKeyGlobTools, str, 0, KEY_ALL_ACCESS, &hHitKeyTools);
		
		if (ret != ERROR_SUCCESS)
		{
			RegCloseKey(hHitKeyTools);
			break;
		}
		
		Tools[i].Command = RegQueryString(hHitKeyTools, L"Command", "");
		
		if (Tools[i].Command.IsEmpty())
		{
			RegCloseKey(hHitKeyTools);
			break;
		}
		
		Tools[i].Text = RegQueryString(hHitKeyTools, L"MenuText", "");
		Tools[i].InitDir = RegQueryString(hHitKeyTools, L"InitDir", "");
		
		RegCloseKey(hHitKeyTools);
		
		NumberOfTools ++;
	}
	
	RegCloseKey(hHitKeyGlobTools);
	
/*	CFieldList StandardFields;
	StandardFields.Add(FIELD_TRACK_NUMBER, 50);
	StandardFields.Add(FIELD_TRACK_ARTIST, 150);
	StandardFields.Add(FIELD_TRACK_TITLE, 150);
	StandardFields.Add(FIELD_TRACK_LENGTH, 50);*/
//	m_ShowFieldsCD.SetType(FLF_TRACK);
//	m_ShowFieldsCD.SetStandardFields(StandardFields);
//	m_ShowFieldsCD.Load(L"ShowFieldsCD");

//	m_ShowFieldsVirtualCD.SetType(FLF_TRACK);
//	m_ShowFieldsVirtualCD.SetStandardFields(StandardFields);
//	m_ShowFieldsVirtualCD.Load(L"ShowFieldsVirtualCD");
	
	RegCloseKey(hHitKeyGlobal);
	RegCloseKey(hHitKeyEvents);
	
	return TRUE;
}

int CConfig::ReadGlobalRegistryKeyInt(const CString& szKeyName, int iDefault)
{
	DWORD ret;
	int value;
	HKEY hHitKeyGlobal;
	
	RegCreateKeyEx(HKEY_CURRENT_USER, REGISTRY_KEY, 0,
		L"Hitbase", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL,
		&hHitKeyGlobal, &ret);
	
	value = iDefault;
	
	ret = sizeof(int);
	RegQueryValueEx(hHitKeyGlobal, szKeyName, 0, NULL, (unsigned char *)&value, &ret);
	
	RegCloseKey(hHitKeyGlobal);
	
	return value;
}

CString CConfig::ReadGlobalRegistryKeyString(const CString& szKeyName, const CString & szDefault)
{
	DWORD ret;
	wchar_t str[200];
	HKEY hHitKeyGlobal;
	
	RegCreateKeyEx(HKEY_CURRENT_USER, REGISTRY_KEY, 0,
		L"Hitbase", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL,
		&hHitKeyGlobal, &ret);
	
	wcscpy(str, szDefault);
	
	ret = sizeof(str);
	RegQueryValueEx(hHitKeyGlobal, szKeyName, 0, NULL, (unsigned char *)&str, &ret);
	
	RegCloseKey(hHitKeyGlobal);
	
	return str;
}

void CConfig::ReadGlobalRegistryKeyBinary(const CString& szKeyName, void* pszDefault, void *pBuf, int nLen)
{
	DWORD ret;
	HKEY hHitKeyGlobal;
	
	RegCreateKeyEx(HKEY_CURRENT_USER, REGISTRY_KEY, 0,
		L"Hitbase", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL,
		&hHitKeyGlobal, &ret);
	
	memcpy(pBuf, pszDefault, nLen);
	
	ret = nLen;
	RegQueryValueEx(hHitKeyGlobal, szKeyName, 0, NULL, (unsigned char *)pBuf, &ret);
	
	RegCloseKey(hHitKeyGlobal);
}

/*
 * Daten in die Registry schreiben.
 */

int CConfig::WriteRegistry()
{
	int i;
	wchar_t str[200];
	HKEY hHitKeyTools;
	DWORD ret;
	HKEY hHitKeyGlobal;
	HKEY hHitKeyEvents;
	
	RegCreateKeyEx(HKEY_CURRENT_USER, m_sRegBaseKey, 0,
		L"Hitbase", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL,
		&hHitKeyGlobal, &ret);
	
	RegCreateKeyEx(HKEY_CURRENT_USER, m_sRegBaseKey + "\\Events", 0,
		L"Hitbase", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL,
		&hHitKeyEvents, &ret);
	
	RegWriteString(hHitKeyGlobal, L"BackgroundPattern", m_PatternPath);
	RegSetValueEx(hHitKeyGlobal, L"BackgroundType", 0, REG_DWORD, (unsigned char *)&m_nBackgroundType, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"BackgroundColor1", 0, REG_DWORD, (unsigned char *)&m_colorCDPlayer1, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"BackgroundColor2", 0, REG_DWORD, (unsigned char *)&m_colorCDPlayer2, sizeof(int));
	
	RegSetValueEx(hHitKeyGlobal, L"DisplayBgColor", 0, REG_DWORD, (unsigned char *)&m_colorDisplayBackground, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"DisplayLCDOnColor", 0, REG_DWORD, (unsigned char *)&m_colorNumbers, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"DisplayLCDOffColor", 0, REG_DWORD, (unsigned char *)&m_colorNumbersGray, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"DisplayBgCurrentTrackColor", 0, REG_DWORD, (unsigned char *)&m_colorCurrentTrackBackground, sizeof(int));
	
	RegWriteString(hHitKeyGlobal, L"LastDatabase", m_LastDatabase);
	RegSetValueEx(hHitKeyGlobal, L"IntroStart", 0, REG_DWORD, (unsigned char *)&IntroStart, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"IntroSeconds", 0, REG_DWORD, (unsigned char *)&IntroSeconds, sizeof(int));
// JUS 010826: Immer TRUE!
	//RegSetValueEx(hHitKeyGlobal, L"ResetCDRomOnStop", 0, REG_DWORD, (unsigned char *)&m_bResetCDRomOnStop, sizeof(int));
	
	RegSetValueEx(hHitKeyGlobal, L"ShowGrid", 0, REG_DWORD, (unsigned char *)&m_bShowGrid, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"ShowToolBar", 0, REG_DWORD, (unsigned char *)&m_bShowToolBar, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"ShowStatusBar", 0, REG_DWORD, (unsigned char *)&m_bShowStatusBar, sizeof(int));
	
	RegSetValueEx(hHitKeyGlobal, L"ShowMenuBar", 0, REG_DWORD, (unsigned char *)&m_bShowMenu, sizeof(int));
	
	RegSetValueEx(hHitKeyGlobal, L"AlwaysOnTop", 0, REG_DWORD, (unsigned char *)&m_bAlwaysOnTop, sizeof(int));
	
// JUS 010826: Immer TRUE!
//	RegSetValueEx(hHitKeyGlobal, L"CDBusyLight", 0, REG_DWORD, (unsigned char *)&CDBusyLight, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"AutoNewOpen", 0, REG_DWORD, (unsigned char *)&AutoNewOpen, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"ShowOnlyUsedCodes", 0, REG_DWORD, (unsigned char *)&ShowOnlyUsedCodes, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"AutoShowCover", 0, REG_DWORD, (unsigned char *)&AutoShowCover, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"AutoSearch", 0, REG_DWORD, (unsigned char *)&m_bAutoSearch, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"NoDupArchiveNumber", 0, REG_DWORD, (unsigned char *)&m_bNoDupArchiveNr, sizeof(int));
	
	RegSetValueEx(hHitKeyGlobal, L"CDDevice", 0, REG_DWORD, (unsigned char *)&CDDevice, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"VirtualCD", 0, REG_DWORD, (unsigned char *)&m_bVirtualCD, sizeof(int));
	
	RegWriteString(hHitKeyGlobal, L"LastCDCoverPath", m_CDCoverPath);
	
	RegWriteString(hHitKeyGlobal, L"HitbaseServer", m_sHitbaseServer);
	
	RegSetValueEx(hHitKeyGlobal, L"OnNewCD", 0, REG_DWORD, (unsigned char *)&OnNewCD, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"OnExit", 0, REG_DWORD, (unsigned char *)&OnExit, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"OnStart", 0, REG_DWORD, (unsigned char *)&OnStart, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"AutoRepeat", 0, REG_DWORD, (unsigned char *)&AutoRepeat, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"LAPView", 0, REG_DWORD, (unsigned char *)&m_nLapView, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"ShowRemainTime", 0, REG_DWORD, (unsigned char *)&m_bShowRemainTime, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"AutoBackup", 0, REG_DWORD, (unsigned char *)&AutoBackup, sizeof(int));
	RegWriteString(hHitKeyGlobal, L"AutoBackupDirectory", m_sAutoBackupDirectory);
	RegSetValueEx(hHitKeyGlobal, L"AutoDateToday", 0, REG_DWORD, (unsigned char *)&m_bAutoDateToday, sizeof(int));
	
	RegSetValueEx(hHitKeyGlobal, L"ShowSymbol", 0, REG_DWORD, (unsigned char *)&ShowSymbol, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"SymbolInTaskbar", 0, REG_DWORD, (unsigned char *)&SymbolInTaskbar, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"BackgroundSearch", 0, REG_DWORD, (unsigned char *)&BackgroundSearch, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"MaxShowCDs", 0, REG_DWORD, (unsigned char *)&MaxShowCDs, sizeof(int));
	
	RegSetValueEx(hHitKeyGlobal, L"ShowSplashScreen", 0, REG_DWORD, (unsigned char *)&ShowSplashScreen, sizeof(int));
	
	RegSetValueEx(hHitKeyGlobal, L"EqualizerQuality", 0, REG_DWORD, (unsigned char *)&EqualizerQuality, sizeof(int));
	
	RegSetValueEx(hHitKeyGlobal, L"MeasureUnit", 0, REG_DWORD, (unsigned char *)&MeasureUnit, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"LeftMargin", 0, REG_BINARY, (unsigned char *)&LeftMargin, sizeof(double));
	RegSetValueEx(hHitKeyGlobal, L"TopMargin", 0, REG_BINARY, (unsigned char *)&TopMargin, sizeof(double));
	RegSetValueEx(hHitKeyGlobal, L"RightMargin", 0, REG_BINARY, (unsigned char *)&RightMargin, sizeof(double));
	RegSetValueEx(hHitKeyGlobal, L"BottomMargin", 0, REG_BINARY, (unsigned char *)&BottomMargin, sizeof(double));
	
	RegSetValueEx(hHitKeyGlobal, L"SmallWindowPos", 0, REG_BINARY, (unsigned char *)&SmallWindowRect, sizeof(RECT));
	RegSetValueEx(hHitKeyGlobal, L"SmallCDPlayer", 0, REG_DWORD, (unsigned char *)&m_bSmallCDPlayer, sizeof(int));
	
	RegWriteString(hHitKeyGlobal, L"PrintFontName", m_PrintFontName);
	RegSetValueEx(hHitKeyGlobal, L"PrintFontSize", 0, REG_DWORD, (unsigned char *)&m_PrintFontSize, sizeof(int));
	
	RegSetValueEx(hHitKeyGlobal, L"PrintCDCoverSize", 0, REG_DWORD, (unsigned char *)&m_nPrintCDCoverSize, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"PrintCDCover", 0, REG_DWORD, (unsigned char *)&m_bPrintCDCover, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"PrintCDCoverAlign", 0, REG_DWORD, (unsigned char *)&m_nPrintCDCoverAlign, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"PrintTextUnderCDCover", 0, REG_DWORD, (unsigned char *)&m_bPrintTextUnderCDCover, sizeof(int));
	
	RegSetValueEx(hHitKeyGlobal, L"CDPlayerAlign", 0, REG_DWORD, (unsigned char *)&m_nCDPlayerAlign, sizeof(int));
	
	RegSetValueEx(hHitKeyGlobal, L"DontSearchDataCDsInCDArchive", 0, REG_DWORD, (unsigned char *)&m_bDontSearchDataCDsInCDArchive, sizeof(int));
	
	RegWriteString(hHitKeyGlobal, L"DefaultRecordPath", m_sRecordPath);
	RegWriteString(hHitKeyGlobal, L"PlugInDirectory", m_sPlugInDirectory);
	
	RegSetValueEx(hHitKeyGlobal, L"VolumeFadeSpeed", 0, REG_DWORD, (unsigned char *)&m_nFadeSecs, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"VolumeFadeWait", 0, REG_DWORD, (unsigned char *)&m_nDelay, sizeof(int));
	
	RegSetValueEx(hHitKeyGlobal, L"AutoPlay", 0, REG_DWORD, (unsigned char *)&m_bAutoPlay, sizeof(int));
	
	// JUS 990816
	RegSetValueEx(hHitKeyGlobal, L"AutoCheckNewVersion", 0, REG_DWORD, (unsigned char *)&m_bAutoCheckNewVersion, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"AutoCheckAnnouncement", 0, REG_DWORD, (unsigned char *)&m_bAutoCheckAnnouncement, sizeof(int));
	
	// JUS 000805
	RegSetValueEx(hHitKeyGlobal, L"VirtualCDOutputDevice", 0, REG_DWORD, (unsigned char *)&m_iVirtualCDOutputDevice, sizeof(int));

	// JUS 000819
	RegSetValueEx(hHitKeyGlobal, L"ShowCDPlayer", 0, REG_DWORD, (unsigned char *)&m_bShowCDPlayer, sizeof(int));

	// JUS 000917
	RegSetValueEx(hHitKeyGlobal, L"VolumeDevice", 0, REG_DWORD, (unsigned char *)&m_iVolumeDevice, sizeof(int));

	// JUS 010825
	RegSetValueEx(hHitKeyGlobal, L"CenterCurrentTrack", 0, REG_DWORD, (unsigned char *)&m_bCenterCurrentTrack, sizeof(int));

	// JUS 011127
	RegSetValueEx(hHitKeyGlobal, L"SkinActive", 0, REG_DWORD, (unsigned char *)&m_bSkinActive, sizeof(int));
	RegWriteString(hHitKeyGlobal, L"LastSkin", m_sLastSkin);
	RegWriteString(hHitKeyGlobal, L"SkinDirectory", m_sSkinDirectory);
	RegSetValueEx(hHitKeyGlobal, L"SkinMouseDisableTransparenz", 0, REG_DWORD, (unsigned char *)&m_bSkinMouseDisableTransparenz, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"WindowTransparenz", 0, REG_DWORD, (unsigned char *)&m_iTransparenzProzentSichtbar, sizeof(int));

	// JUS 020221
	RegSetValueEx(hHitKeyGlobal, L"GroupArtistBoundary", 0, REG_DWORD, (unsigned char *)&m_iGroupArtistBoundary, sizeof(int));

	// JUS 020802: Kopfzeilen beim Ausdruck
	RegWriteString(hHitKeyGlobal, L"PrintHeaderText0", m_sPrintHeaderText0);
	RegWriteString(hHitKeyGlobal, L"PrintHeaderText1", m_sPrintHeaderText1);
	RegWriteString(hHitKeyGlobal, L"PrintHeaderText2", m_sPrintHeaderText2);
	RegWriteString(hHitKeyGlobal, L"PrintHeaderText3", m_sPrintHeaderText3);
	RegWriteString(hHitKeyGlobal, L"PrintHeaderText4", m_sPrintHeaderText4);
	RegSetValueEx(hHitKeyGlobal, L"PrintHeaderShowSort", 0, REG_DWORD, (unsigned char *)&m_bPrintHeaderShowSort, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"PrintHeaderShowFilter", 0, REG_DWORD, (unsigned char *)&m_bPrintHeaderShowFilter, sizeof(int));

	// JUS 020810
	RegSetValueEx(hHitKeyGlobal, L"VirtualCDBufferSize", 0, REG_DWORD, (unsigned char *)&m_iVirtualCDBufferSize, sizeof(int));

	// JUS 020819
	RegSetValueEx(hHitKeyGlobal, L"RecordOptionsJitterReadTracks", 0, REG_DWORD, (unsigned char *)&m_iRecordOptionsJitterReadTracks, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"RecordOptionsJitterCheck", 0, REG_DWORD, (unsigned char *)&m_iRecordOptionsJitterCheck, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"RecordOptionsJitterOverlap", 0, REG_DWORD, (unsigned char *)&m_iRecordOptionsJitterOverlap, sizeof(int));

	// JUS 020903
	RegSetValueEx(hHitKeyGlobal, L"ArtistDisplay", 0, REG_DWORD, (unsigned char *)&m_iArtistDisplay, sizeof(int));

	// JUS 020922
	RegSetValueEx(hHitKeyGlobal, L"AutoInsertQueue", 0, REG_DWORD, (unsigned char *)&m_bAutoInsertQueue, sizeof(int));

	// JUS 021224
	RegSetValueEx(hHitKeyGlobal, L"DisableCDText", 0, REG_DWORD, (unsigned char *)&m_bDisableCDText, sizeof(int));

	// JUS 021230
	RegSetValueEx(hHitKeyGlobal, L"ReadAlgorithm", 0, REG_DWORD, (unsigned char *)&m_iReadAlgorithm, sizeof(int));

	// JUS 030324
	RegSetValueEx(hHitKeyGlobal, L"CatalogViewMode", 0, REG_DWORD, (unsigned char *)&m_iCatalogViewMode, sizeof(int));

	// JUS 030615
	RegSetValueEx(hHitKeyGlobal, L"CDOutputDigital", 0, REG_DWORD, (unsigned char *)&m_bCDOutputDigital, sizeof(int));

	// JUS 030628
	RegSetValueEx(hHitKeyGlobal, L"CDCoverDisplaySize", 0, REG_DWORD, (unsigned char *)&m_iCDCoverDisplaySize, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"CDCoverDisplayWhat", 0, REG_DWORD, (unsigned char *)&m_iCDCoverShowWhat, sizeof(int));

	// JUS 030822
	RegSetValueEx(hHitKeyGlobal, L"AutoCompleteArtist", 0, REG_DWORD, (unsigned char *)&m_bAutoCompleteArtist, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"AutoCompleteCDTitle", 0, REG_DWORD, (unsigned char *)&m_bAutoCompleteCDTitle, sizeof(int));
	RegSetValueEx(hHitKeyGlobal, L"AutoCompleteTrackname", 0, REG_DWORD, (unsigned char *)&m_bAutoCompleteTrackname, sizeof(int));

	// JUS 031019
	RegWriteInt(hHitKeyGlobal, L"AutoVolumeAdjust", m_bAutoVolumeAdjust);
	RegWriteInt(hHitKeyGlobal, L"AutoVolumeAdjustMax", m_lAutoVolumeAdjustMax);

	// JUS 031025
	RegWriteInt(hHitKeyGlobal, L"CopyMode", m_nRecordCopyMode);
	RegWriteInt(hHitKeyGlobal, L"SpinUpDrive", m_nRecordSpinUpDrive);
	RegWriteInt(hHitKeyGlobal, L"SpinUpWaitTimeSec", m_nRecordSpinUpWaitTimeSec);
//	RegWriteInt(hHitKeyGlobal, L"RecordForceASPI", m_bRecordForceASPI);

	// JUS 031129
	RegWriteInt(hHitKeyGlobal, L"AutoSaveCDs", m_bAutoSaveCDs);

	// JUS 040412
	RegWriteInt(hHitKeyGlobal, L"CrossFade", m_bCrossFade);
	RegWriteInt(hHitKeyGlobal, L"CrossFadeDefaultSeconds", m_iCrossFadeDefaultSeconds);

	// JUS 040508
	RegWriteInt(hHitKeyGlobal, L"PlaylistVisible", m_bPlaylistVisible);

	// JUS 041005
	RegWriteInt(hHitKeyGlobal, L"GroupDisplay", m_bGroupDisplay);

	// JUS 041103
	RegWriteInt(hHitKeyGlobal, L"DisableAutoPlay", m_bDisableAutoPlay);

	// JUS 051225
	//RegWriteString(hHitKeyGlobal, L"DefaultImageDirectory", m_sDefaultImageDirectory);

	// JUS 060129
	RegWriteInt(hHitKeyGlobal, L"SearchBarSearchWhere", m_iSearchBarSearchWhere);

	// JUS 060610
	RegWriteInt(hHitKeyGlobal, L"StopTimeBeforeEnd", m_iStopTimeBeforeEnd);

	// JUS 061205
	RegWriteInt(hHitKeyGlobal, L"SkinSignalDisplayMode", m_iSkinSignalDisplayMode);

	for (i=0;i<NUMBER_OF_EVENTS;i++)
	{
		swprintf(str, L"%d", i);
		RegWriteString(hHitKeyEvents, str, SoundEvent[i]);
	}
	
	for (i=0;i<MAX_TOOLS;i++)
	{
		swprintf(str, m_sRegBaseKey + L"\\Tools\\%d", i+1);
		RegCreateKeyEx(HKEY_CURRENT_USER, str, 0,
			L"Hitbase", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL,
			&hHitKeyTools, &ret);
		
		if (i >= NumberOfTools)
		{
			swprintf(str, L"Tools\\%d", i+1);
			RegDeleteKey(hHitKeyGlobal, str);
		}
		else
		{
			RegWriteString(hHitKeyTools, L"Command", Tools[i].Command);
			RegWriteString(hHitKeyTools, L"MenuText", Tools[i].Text);
			RegWriteString(hHitKeyTools, L"InitDir", Tools[i].InitDir);
		}
		
		RegCloseKey(hHitKeyTools);
	}
	
	//m_ShowFieldsCD.Save(L"ShowFieldsCD");
	//m_ShowFieldsVirtualCD.Save(L"ShowFieldsVirtualCD");
	
	RegCloseKey(hHitKeyGlobal);
	RegCloseKey(hHitKeyEvents);
	
	return TRUE;
}

void CConfig::WriteGlobalRegistryKeyInt(const CString& szKeyName, int iValue)
{
   HKEY hHitKeyGlobal;
   DWORD ret;

   RegCreateKeyEx(HKEY_CURRENT_USER, REGISTRY_KEY, 0,
                  L"Hitbase", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL,
                  &hHitKeyGlobal, &ret);

   RegSetValueEx(hHitKeyGlobal, szKeyName, 0, REG_DWORD, (unsigned char *)&iValue, sizeof(int));

   RegCloseKey(hHitKeyGlobal);
}

void CConfig::WriteGlobalRegistryKeyString(const CString& szKeyName, const CString& str)
{
   HKEY hHitKeyGlobal;
   DWORD ret;

   RegCreateKeyEx(HKEY_CURRENT_USER, REGISTRY_KEY, 0,
                  L"Hitbase", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL,
                  &hHitKeyGlobal, &ret);

   RegSetValueEx(hHitKeyGlobal, szKeyName, 0, REG_SZ, (BYTE *)(LPCTSTR)str, (wcslen(str)+1)*sizeof(wchar_t));

   RegCloseKey(hHitKeyGlobal);
}

void CConfig::WriteGlobalRegistryKeyBinary(const CString& szKeyName, char* pValue, int nLen)
{
   HKEY hHitKeyGlobal;
   DWORD ret;

   RegCreateKeyEx(HKEY_CURRENT_USER, REGISTRY_KEY, 0,
                  L"Hitbase", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL,
                  &hHitKeyGlobal, &ret);

   RegSetValueEx(hHitKeyGlobal, szKeyName, 0, REG_BINARY, (unsigned char *)pValue, nLen);

   RegCloseKey(hHitKeyGlobal);
}

CString CConfig::RegQueryString(HKEY hKey, const CString& szSubKey, const CString &sDefault)
{
   wchar_t buff[200];
   DWORD ret;

   wcscpy(buff, sDefault);
   ret = sizeof(buff);
   RegQueryValueEx(hKey, szSubKey, 0, NULL, (unsigned char *)buff, &ret);

   return buff;
}

long CConfig::RegQueryInt(HKEY hKey, const CString& szSubKey, int idefault)
{
	DWORD ret;
	long value;

	value = idefault;
	ret = sizeof(int);
	RegQueryValueEx(hKey, szSubKey, 0, NULL, (unsigned char *)&value, &ret);

	return value;
}

void CConfig::RegQueryBinary(HKEY hKey, const CString& szKeyName, void *pBuf, int nLen)
{
    DWORD ret;
	
	ret = nLen;
	RegQueryValueEx(hKey, szKeyName, 0, NULL, (unsigned char *)pBuf, &ret);
}

// Standardmäßig siehe REGISTRY_KEY. Ist sRegBaseKey == "", so wird dieses Define
// benutzt.

void CConfig::SetRegistryKey(const CString& sRegBaseKey)
{
	if (sRegBaseKey.IsEmpty())
		m_sRegBaseKey = REGISTRY_KEY;
	else
		m_sRegBaseKey = sRegBaseKey;
}

// Legt einen neuen Unterkey (Unterorder) an.
// Z.b. "Export" für "Hitbase\Export"
HKEY CConfig::CreateSubKey(const CString& sSubKey)
{
   HKEY hKey;
   DWORD ret;

   RegCreateKeyEx(HKEY_CURRENT_USER, (CString)REGISTRY_KEY+"\\"+sSubKey, 0,
                  L"Hitbase", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL,
                  &hKey, &ret);

   return hKey;
}

void CConfig::CloseKey(HKEY hKey)
{
	RegCloseKey(hKey);
}

void CConfig::RegWriteString(HKEY hKey, const CString& szSubKey, const CString &sValue)
{
	RegSetValueEx(hKey, szSubKey, 0, REG_SZ, (BYTE *)(LPCTSTR)sValue, (wcslen(sValue)+1)*sizeof(wchar_t));
}

void CConfig::RegWriteInt(HKEY hKey, const CString& szSubKey, int iValue)
{
	RegSetValueEx(hKey, szSubKey, 0, REG_DWORD, (unsigned char *)&iValue, sizeof(int));
}

void CConfig::RegWriteBinary(HKEY hKey, const CString& szSubKey, void* pValue, int size)
{
    RegSetValueEx(hKey, szSubKey, 0, REG_BINARY, (unsigned char *)pValue, size);
}
