using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Drawing;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.Configuration
{
    public class Settings 
    {
        public const string HitbaseRegistryKey = "SOFTWARE\\Big 3\\Hitbase 2012";

        public static Settings Current = new Settings();

        public Settings()
        {
            Load();
        }

        /// <summary>
        /// Die zuletzt geöffnete Datenbank
        /// </summary>
        public string LastDataBase { get; set; }

        #region CDPlayer
        /// <summary>
        /// Beim Spielen den aktuell spielenden Track in der Liste zentrieren.
        /// </summary>
        public bool CenterCurrentTrack { get; set; }

        /// <summary>
        /// CD-Text-Funktionen abschalten.
        /// </summary>
        public bool DisableCDText { get; set; }

        public bool AutoRepeat { get; set; }
        public int OnStart { get; set; }
        public int IntroStart { get; set; }
        public int IntroSeconds { get; set; }
        public int OnNewCD { get; set; }
        public int OnExit { get; set; }
        public int LapView { get; set; }

        #endregion

        #region DataBase

        public bool AutoNewOpen { get; set; }
        public bool AutoBackup { get; set; }
        public bool ShowOnlyUsedCodes { get; set; }
        public bool AutoDateToday { get; set; }
        public bool AutoCompleteArtist { get; set; }
        public bool AutoCompleteCDTitle { get; set; }
        public bool AutoCompleteTrackname { get; set; }
        public bool AutoSaveCDs { get; set; }
        public string AutoBackupDirectory { get; set; }
        public int ArtistDisplay { get; set; }
        
        #endregion

        #region Misc
        public bool AutoCheckNewVersion { get; set; }
        public bool AutoCheckAnnouncement { get; set; }
        public bool AutoShowCover { get; set; }
        public int EqualizerQuality { get; set; }
        public bool ShowSplashScreen { get; set; }
        #endregion

        public bool SymbolInTaskbar { get; set; }
        public int ShowSymbol { get; set; }

        #region CDArchive
        public bool DontSearchDataCDsInCDArchive { get; set; }
        public bool AutoInsertQueue { get; set; }
        #endregion

        #region Play
        public bool CrossFadeActive { get; set; }
        public int CrossFadeDefaultSeconds { get; set; }
        public int AutoVolumeAdjustMax { get; set; }
        public int StopTimeBeforeEnd { get; set; }

        public int VirtualCDOutputDevice { get; set; }
        public int VirtualCDBufferSize { get; set; }
        #endregion

        private bool showGridLines = true;
        /// <summary>
        /// true, wenn in allen Tabellen Gitternetzlinien angezeigt werden sollen.
        /// </summary>
        public bool ShowGridLines
        {
            get { return showGridLines; }
            set { showGridLines = value; }
        }

        private bool noDuplicateArchiveNumbers = true;
        /// <summary>
        /// true, wenn keine doppelten Archivnummern erlaubt sind.
        /// </summary>
        public bool NoDuplicateArchiveNumbers
        {
            get { return noDuplicateArchiveNumbers; }
            set { noDuplicateArchiveNumbers = value; }
        }

        private bool sortArchiveNumberNumeric = true;
        /// <summary>
        /// true, wenn keine die Archivnummer numerisch sortiert werden soll.
        /// </summary>
        public bool SortArchiveNumberNumeric
        {
            get { return sortArchiveNumberNumeric; }
            set { sortArchiveNumberNumeric = value; }
        }

        private List<string> loanedToRecentList = new List<string>();

        public List<string> LoanedToRecentList
        {
            get { return loanedToRecentList; }
            set { loanedToRecentList = value; }
        }

        private List<string> recentFileList = new List<string>();
        /// <summary>
        /// Zuletzt geöffnete Kataloge.
        /// </summary>
        public List<string> RecentFileList
        {
            get { return recentFileList; }
            set { recentFileList = value; }
        }

        private string loanedDefaultTimeSpan;
        /// <summary>
        /// Enthält die Standardeinstellung für den Zeitraum, wenn eine CD
        /// zurückgegeben werden muss (Z.B. "2 Wochen")
        /// </summary>
        public string LoanedDefaultTimeSpan
        {
            get { return loanedDefaultTimeSpan; }
            set { loanedDefaultTimeSpan = value; }
        }

        private int personGroupListType;
        /// <summary>
        /// Die Art der Personen/Gruppen Ansicht 
        /// (0 = Detailansicht mit Bild, 1 = Tabellenansicht)
        /// </summary>
        public int PersonGroupListType
        {
            get { return personGroupListType; }
            set { personGroupListType = value; }
        }

        /// <summary>
        /// Schreibweise anpassen (Standard: Erster Buchstabe jedes Wortes groß)
        /// </summary>
        public int AdjustSpelling { get; set; }

        /// <summary>
        /// Die Schriftart für den Detailsbereich eines Ausdrucks.
        /// </summary>
        public Font PrintDetailsFont { get; set; }

        /// <summary>
        /// Beim Ausdruck in den Kopfzeilen die Sortierung ausdrucken.
        /// </summary>
        public bool PrintHeaderSort { get; set; }

        /// <summary>
        /// Beim Ausdruck in den Kopfzeilen das aktuelle Tagesdatum ausdrucken.
        /// </summary>
        public bool PrintHeaderDate { get; set; }

        /// <summary>
        /// Beim Ausdruck in den Kopfzeilen den Filter ausdrucken.
        /// </summary>
        public bool PrintHeaderFilter { get; set; }

        /// <summary>
        /// Beim Ausdruck der langen CD-Liste das Cover mit ausdrucken
        /// </summary>
        public bool PrintCDCover { get; set; }

        /// <summary>
        /// Beim Ausdruck den Text erst unter dem CD-Cover beginnen lassen
        /// </summary>
        public bool PrintTextUnderCDCover { get; set; }

        /// <summary>
        /// Die Größe des CD-Covers beim Ausdruck
        /// </summary>
        public int PrintCDCoverSize { get; set; }

        /// <summary>
        /// Die horizontale Anordnung des CD-Covers
        /// </summary>
        public int PrintCDCoverAlign { get; set; }

        /// <summary>
        /// Die globalen Druckereinstellungen
        /// Werden aktuell noch nicht gespeichert (also nur Runtime)
        /// </summary>
        public System.Drawing.Printing.PrinterSettings GlobalPrinterSettings { get; set; }

        /// <summary>
        /// Der Standardpfad für die Ablage von CD-Covern (Pictures)
        /// </summary>
        public string DefaultCDCoverPath { get; set; }

        /// <summary>
        /// true, wenn beim Erfassen eines Samplers, ein fester Text für das Interpreten-Feld
        /// vorgegeben werden soll (z.B. "Diverse")
        /// </summary>
        public bool SamplerUseFixedArtist { get; set; }

        /// <summary>
        /// Der Text, der bei Samplern für das Interpreten-Feld fest vorgegeben werden soll.
        /// (Nur wenn SamplerUseFixedArtist == true)
        /// </summary>
        public string SamplerFixedArtistText { get; set; }

        /// <summary>
        /// Die Liste, die im Katalog zuletzt angezeigt wurde.
        /// </summary>
        public int LastCatalogListType { get; set; }

        /// <summary>
        /// Die Soundkarte, die für die Musikwiedergabe benutzt werden soll.
        /// </summary>
        public int OutputDevice { get; set; }

        /// <summary>
        /// Die Soundkarte, die für Vorhören/Prelisten benutzt werden soll.
        /// </summary>
        public int OutputDevicePreListen { get; set; }

        /// <summary>
        /// Welche ID3-Version soll gelesen/erzeugt werden (0 = ID3v1, 1 = ID3v2, 2 = beide)
        /// </summary>
        public int UseID3Version { get; set; }

        /// <summary>
        /// Mindestgröße der Musikdateien, die untersucht werden sollen (in KB).
        /// </summary>
        public int ManageSoundFilesMinSize { get; set; }

        /// <summary>
        /// Letzter Ordner, der nach Musikdateien durchsucht wurde.
        /// </summary>
        public string ManageSoundFilesLastFolder { get; set; }

        /// <summary>
        /// Automatisch Albuminformationen generieren, falls nicht vorhanden.
        /// </summary>
        public bool ManageSoundFilesCreateAlbumInfo { get; set; }

        /// <summary>
        /// Alle Musikdateien in einer Sammlung speichern.
        /// </summary>
        public bool ManageSoundFilesCreateOneCollection { get; set; }

        /// <summary>
        /// Alle Musikdateien in einer Sammlung speichern  (Titel 1).
        /// </summary>
        public string ManageSoundFilesCreateOneCollectionTitle1 { get; set; }

        /// <summary>
        /// Alle Musikdateien in einer Sammlung speichern (Titel 2).
        /// </summary>
        public string ManageSoundFilesCreateOneCollectionTitle2 { get; set; }

        /// <summary>
        /// true, wenn Interpret und Titel aus dem Dateinamen erkannt werden sollen
        /// </summary>
        public bool DetectTrackInfoFromFilename { get; set; }

        /// <summary>
        /// Trennzeichen dafür
        /// </summary>
        public string DetectTrackInfoDelimiter { get; set; }

        /// <summary>
        /// Führende Ziffern bei der Dateinamenerkennung entfernen
        /// </summary>
        public bool DeleteLeadingNumbers { get; set; }

        /// <summary>
        /// Wenn die Vorhören (Pre-Listen) Funktion das erste mal aufgerufen wird.
        /// </summary>
        public bool PreListenVirgin { get; set; }

        /// <summary>
        /// Im Party-Modus einzelne Funktionen mit Passwort schützen.
        /// </summary>
        public bool PartyModusEnablePassword { get; set; }

        /// <summary>
        /// Das Passwort, um im Party-Modus einzelne Funktionen zu schützen.
        /// </summary>
        public string PartyModusPassword { get; set; }

        /// <summary>
        /// Das Datum der letzten Ankündigung.
        /// </summary>
        public string LastAnnouncementDate { get; set; }

        /// <summary>
        /// Beim Einlegen einer CD automatisch nach Covern suchen (Amazon)
        /// </summary>
        public bool AutoSearchCover { get; set; }

        /// <summary>
        /// Beim Einlegen einer CD automatisch nach den Lyrics aller Tracks suchen.
        /// </summary>
        public bool AutoSearchLyrics { get; set; }

        /// <summary>
        /// Der aktuelle Farbstil (z.B. Blau, Silber, etc.)
        /// </summary>
        public ColorStyle CurrentColorStyle { get; set; }

        /// <summary>
        /// Die Breite des TreeViews im CD-Katalog (Interpreten-Übersicht)
        /// </summary>
        public int CatalogTreeViewWidth { get; set; }

        /// <summary>
        /// Internet-Verbindung vorhanden (bzw. Laden der CD-Daten aktivieren)
        /// </summary>
        public int InternetConnection { get; set; }

        /// <summary>
        /// Internet-Verbindung nur über einen Proxy.
        /// </summary>
        public int ProxyType { get; set; }

        /// <summary>
        /// Name des Proxy-Servers
        /// </summary>
        public string ProxyServerName { get; set; }

        /// <summary>
        /// HTTP-Timeout in Millisekunden
        /// </summary>
        public int HttpTimeoutInMS { get; set; }

        /// <summary>
        /// Letzter Benutzername für my.Hitbase Upload
        /// </summary>
        public string MyHitbaseLastUserName { get; set; }

        /// <summary>
        /// Letztes Password für my.Hitbase Upload (Verschlüsselt)
        /// </summary>
        public string MyHitbaseLastPassword { get; set; }

        /// <summary>
        /// Bilder nicht übertragen
        /// </summary>
        public bool MyHitbaseDontSendImages { get; set; }

        /// <summary>
        /// Bei den Mitwirkenden die Bilder-Ansicht aktivieren
        /// </summary>
        public bool ShowParticipantPictures { get; set; }

        #region Normalize
        /// <summary>
        /// Normalisieren aktivieren?
        /// </summary>
        public bool NormalizeActive { get; set; }
        /// <summary>
        /// Normalize to percent
        /// </summary>
        public int NormalizePercent { get; set; }
        /// <summary>
        /// Normalize only when Min reached
        /// </summary>
        public int NormalizePercentMin { get; set; }
        /// <summary>
        /// Normalize only when Max reached
        /// </summary>
        public int NormalizePercentMax { get; set; }

        #endregion

        /// <summary>
        /// Die Navigationsleiste anzeigen
        /// </summary>
        public bool ShowNavigationBar { get; set; }

        /// <summary>
        /// Die Wiedergabeliste anzeigen
        /// </summary>
        public bool ShowPlaylist { get; set; }

        /// <summary>
        /// Den Player anzeigen
        /// </summary>
        public bool ShowPlayer { get; set; }

        /// <summary>
        /// Der View-Type für die Anzeige "Meine Musik".
        /// </summary>
        public int MyMusicViewType { get; set; }

        /// <summary>
        /// Der View-Type für die Anzeige "Album"
        /// </summary>
        public int AlbumViewType { get; set; }

        /// <summary>
        /// Der View-Type für die Anzeige "Verzeichnis"
        /// </summary>
        public int DirectoryViewType { get; set; }

        /// <summary>
        /// Scrobblen aktivieren.
        /// </summary>
        public bool ScrobbleActive { get; set; }

        /// <summary>
        /// Kann im Programm nicht eingestellt werden. Nur für den Notfall über die Registry.
        /// </summary>
        public string HitbaseServer { get; set; }

        /// <summary>
        /// Bei "nächster freier Archivnummer suchen" auch Lücken beachten
        /// </summary>
        public bool FreeArchiveNumberSearchWithGaps { get; set; }

        /// <summary>
        /// Frequenzband anzeigen
        /// </summary>
        public bool ShowFrequencyBand { get; set; }

        /// <summary>
        /// Lyrics anzeigen
        /// </summary>
        public bool ShowLyrics { get; set; }

        /// <summary>
        /// Im Partymodus "als nächstes spielen" zulassen
        /// </summary>
        public bool PartyModeAllowPlayNext { get; set; }

        /// <summary>
        /// Im Partymodus "als letztes spielen" zulassen
        /// </summary>
        public bool PartyModeAllowPlayLast { get; set; }

        /// <summary>
        /// Die Breite der Wiedergabeliste im Hauptfenster in Pixel.
        /// </summary>
        public int PlaylistWidth { get; set; }

        /// <summary>
        /// Die Breite der Treeansicht im Hauptfenster in Pixel.
        /// </summary>
        public int MainTreeWidth { get; set; }

        /// <summary>
        /// Ist im Party-Modus die Playlist angeheftet?
        /// </summary>
        public bool PartyModePlaylistPinned { get; set; }

        /// <summary>
        /// Ist im Party-Modus die Wishlist angeheftet?
        /// </summary>
        public bool PartyModeWishlistPinned { get; set; }

        /// <summary>
        /// Ist die Ribbon minimiert?
        /// </summary>
        public bool RibbonIsMinimized { get; set; }

        /// <summary>
        /// Wird die QAT über oder unter dem Ribbon angezeigt?
        /// </summary>
        public bool ShowQuickAccessToolBarOnTop { get; set; }

        /// <summary>
        /// Die Buttons in der QAT.
        /// </summary>
        public string RibbonQuickAccessToolBar { get; set; }

        /// <summary>
        /// Zur Ansteuerung des CD-Laufwerks MCI benutzen.
        /// </summary>
        public bool UseMCI { get; set; }

        /// <summary>
        /// Erste Schritte nicht mehr automatisch anzeigen.
        /// </summary>
        public bool DontShowFirstSteps { get; set; }

        /// <summary>
        /// ScreenSaver und Stand-By deaktivieren.
        /// </summary>
        public bool DisableScreenSaver { get; set; }
        
        /// <summary>
        /// Genres automatisch alphabetisch sortieren.
        /// </summary>
        public bool AutoSortGenres { get; set; }

        /// <summary>
        /// Genres automatisch alphabetisch sortieren.
        /// </summary>
        public bool AutoSortMediums { get; set; }

        /// <summary>
        /// Zeitformat (HH:MM:SS oder MM:SS).
        /// </summary>
        public TimeFormat TimeFormat { get; set; }


        #region Record
        /// <summary>
        /// Letztes ausgewähltes CD-Laufwerk für Aufnahme.
        /// </summary>
        public string RecordMediumLastDriveLetter { get; set; }

        /// <summary>
        /// Letzter Pfad für Aufnahme.
        /// </summary>
        public string RecordSelectedPath { get; set; }

        /// <summary>
        /// Aktuell ausgewähltes Dateinamen Format.
        /// </summary>
        public string RecordFormatFile { get; set; }

        /// <summary>
        /// Verfügbare Aufnahme Formatierungen Dateiname
        /// </summary>
        public string[] RecordFilenameFormat { get; set; }

        /// <summary>
        /// Aufnahme und Auswurf nach Fertigstellung
        /// </summary>
        public bool RecordAutoEject { get; set; }
        
        /// <summary>
        /// Aufnahme Kopierbereich (Alles, Track, A-B)
        /// </summary>
        public int RecordCopyRegion { get; set; }

        /// <summary>
        /// Aufnahme ausgewählte Tracks
        /// </summary>
        public string RecordSelectedTracks { get; set; }

        /// <summary>
        /// Aufnahme Format (MP3, OGG, WAV, ...)
        /// </summary>
        public int RecordFormat { get; set; }

        /// <summary>
        /// Aufnahme Dateinamen Bildung
        /// </summary>
        public int RecordFileNamesType { get; set; }

        /// <summary>
        /// Aufnahme ID3 Tags schreiben
        /// </summary>
        public int RecordWriteID3Tags { get; set; }

        /// <summary>
        /// Aufnahme Nummerierung mit Präfix
        /// </summary>
        public string RecordFileNamesPrefix { get; set; }

        /// <summary>
        /// Aufnahme Dateiname Backslash Zeichen ändern
        /// </summary>
        public string RecordFileNameCharBackslash { get; set; }

        /// <summary>
        /// Aufnahme Dateiname Backslash Zeichen ändern
        /// </summary>
        public string RecordFileNameCharSlash { get; set; }

        /// <summary>
        /// Aufnahme Dateiname Backslash Zeichen ändern
        /// </summary>
        public string RecordFileNameCharDoppelpunkt { get; set; }

        /// <summary>
        /// Aufnahme Dateiname Backslash Zeichen ändern
        /// </summary>
        public string RecordFileNameCharStern { get; set; }

        /// <summary>
        /// Aufnahme Dateiname Backslash Zeichen ändern
        /// </summary>
        public string RecordFileNameCharFragezeichen { get; set; }

        /// <summary>
        /// Aufnahme Dateiname Backslash Zeichen ändern
        /// </summary>
        public string RecordFileNameCharAnfuehrung { get; set; }

        /// <summary>
        /// Aufnahme Dateiname Backslash Zeichen ändern
        /// </summary>
        public string RecordFileNameCharKleiner { get; set; }

        /// <summary>
        /// Aufnahme Dateiname Backslash Zeichen ändern
        /// </summary>
        public string RecordFileNameCharGroesser { get; set; }

        /// <summary>
        /// Aufnahme Dateiname Backslash Zeichen ändern
        /// </summary>
        public string RecordFileNameCharPipe { get; set; }

        /// <summary>
        /// Aufnahme Dateiname original Zeichen Benutzer 1
        /// </summary>
        public string RecordFileNameCharUserOrg1 { get; set; }

        /// <summary>
        /// Aufnahme Dateiname original Zeichen Benutzer 2
        /// </summary>
        public string RecordFileNameCharUserOrg2 { get; set; }

        /// <summary>
        /// Aufnahme Dateiname original Zeichen Benutzer 3
        /// </summary>
        public string RecordFileNameCharUserOrg3 { get; set; }

        /// <summary>
        /// Aufnahme Dateiname original Zeichen Benutzer 4
        /// </summary>
        public string RecordFileNameCharUserOrg4 { get; set; }

        /// <summary>
        /// Aufnahme Dateiname original Zeichen Benutzer 5
        /// </summary>
        public string RecordFileNameCharUserOrg5 { get; set; }

        /// <summary>
        /// Aufnahme Dateiname original Zeichen Benutzer 6
        /// </summary>
        public string RecordFileNameCharUserOrg6 { get; set; }

        /// <summary>
        /// Aufnahme Dateiname neues Zeichen Benutzer 1
        /// </summary>
        public string RecordFileNameCharUserNew1 { get; set; }

        /// <summary>
        /// Aufnahme Dateiname neues Zeichen Benutzer 2
        /// </summary>
        public string RecordFileNameCharUserNew2 { get; set; }

        /// <summary>
        /// Aufnahme Dateiname neues Zeichen Benutzer 3
        /// </summary>
        public string RecordFileNameCharUserNew3 { get; set; }

        /// <summary>
        /// Aufnahme Dateiname neues Zeichen Benutzer 4
        /// </summary>
        public string RecordFileNameCharUserNew4 { get; set; }

        /// <summary>
        /// Aufnahme Dateiname neues Zeichen Benutzer 5
        /// </summary>
        public string RecordFileNameCharUserNew5 { get; set; }

        /// <summary>
        /// Aufnahme Dateiname neues Zeichen Benutzer 6
        /// </summary>
        public string RecordFileNameCharUserNew6 { get; set; }

        /// <summary>
        /// Aufnahme Parameter für die lame.exe
        /// </summary>
        public string RecordLameExeParameter { get; set; }

        /// <summary>
        /// Aufnahme Parameter für die flac.exe
        /// </summary>
        public string RecordFlacExeParameter { get; set; }

        /// <summary>
        /// Aufnahme Parameter für die oggenc.exe
        /// </summary>
        public string RecordOggExeParameter { get; set; }

        /// <summary>
        /// Aufnahme Parameter für das benutzerdefinierte Programm 1
        /// </summary>
        public string RecordUser1ExeParameter { get; set; }

        /// <summary>
        /// Aufnahme Parameter für das benutzerdefinierte Programm 2
        /// </summary>
        public string RecordUser2ExeParameter { get; set; }

        /// <summary>
        /// Das zuletzt ausgewählte Aufnahmeformat
        /// </summary>
        public int RecordLastSelectedFormat { get; set; }

        /// <summary>
        /// Das zuletzt ausgewählte Aufnahmeformat in der Schnellauswahl
        /// </summary>
        public int RecordLastQuickSelectedFormat { get; set; }
        
        /// <summary>
        /// CD automatisch kopieren ein/aus
        /// </summary>
        public bool RecordAutoCDCopy { get; set; }

        /// <summary>
        /// Von CD RIP automatisch eine M3U generieren
        /// </summary>
        public bool RecordAutoCreateM3U { get; set; }

        /// <summary>
        /// CD Cover automatisch ins RIP Verzeichnis kopieren
        /// </summary>
        public bool RecordSaveCDCover { get; set; }

        /// <summary>
        /// Custom CBR Settings
        /// </summary>
        public bool RecordMP3CustomCBR { get; set; }

        /// <summary>
        /// Custom Settings
        /// </summary>
        public bool RecordMP3CustomCopyright { get; set; }
        /// <summary>
        /// Custom Settings
        /// </summary>
        public bool RecordMP3CustomCRC { get; set; }
        /// <summary>
        /// Custom Settings
        /// </summary>
        public bool RecordMP3CustomPrivate { get; set; }
        /// <summary>
        /// Custom Settings
        /// </summary>
        public bool RecordMP3CustomOriginal { get; set; }
        /// <summary>
        /// Custom CBR Settings
        /// </summary>
        public int RecordMP3CustomCBRBitRates { get; set; }

        /// <summary>
        /// Custom Channels
        /// </summary>
        public int RecordMP3CustomChannels { get; set; }

        /// <summary>
        /// Custom VBR Settings
        /// </summary>
        public bool RecordMP3CustomVBR { get; set; }

        /// <summary>
        /// Custom VBR Settings
        /// </summary>
        public int RecordMP3CustomVBRMethod { get; set; }

        /// <summary>
        /// Custom VBR Settings
        /// </summary>
        public int RecordMP3CustomVBRMin { get; set; }
        
        /// <summary>
        /// Custom VBR Settings
        /// </summary>
        public int RecordMP3CustomVBRMax { get; set; }
        
        /// <summary>
        /// Custom VBR Settings
        /// </summary>
        public int RecordMP3CustomVBRAverage { get; set; }
        
        /// <summary>
        /// Custom VBR Settings
        /// </summary>
        public int RecordMP3CustomVBRQuality { get; set; }

        /// <summary>
        /// Die zuletzt ausgewählte MP3 Qualität in der Schnellauswahl
        /// </summary>
        public int RecordLastQuickSelectedMP3Quality { get; set; }
        
        /// <summary>
        /// Die zuletzt ausgewählte WMA Qualität in der Schnellauswahl
        /// </summary>
        public int RecordLastQuickSelectedWMAQuality { get; set; }
        
        /// <summary>
        /// Die zuletzt ausgewählte OGG Qualität in der Schnellauswahl
        /// </summary>
        public int RecordLastQuickSelectedOGGQuality { get; set; }
        
        /// <summary>
        /// Die zuletzt ausgewählte FLAC Qualität in der Schnellauswahl
        /// </summary>
        public int RecordLastQuickSelectedFLACQuality { get; set; }

        /// <summary>
        /// Geschwindigkeit fürs Rippen
        /// </summary>
        public int RecordSpeed { get; set; }

        #endregion Record

        #region Burn
        /// <summary>
        /// Burn dataGridSelectDirectoryFiles Colums
        /// </summary>
        public int BurnSelectFilesColumn1 { get; set; }
        /// <summary>
        /// Burn dataGridSelectDirectoryFiles Colums
        /// </summary>
        public int BurnSelectFilesColumn2 { get; set; }
        /// <summary>
        /// Burn dataGridSelectDirectoryFiles Colums
        /// </summary>
        public int BurnSelectFilesColumn3 { get; set; }
        /// <summary>
        /// Burn dataGridSelectDirectoryFiles Colums
        /// </summary>
        public int BurnSelectFilesColumn4 { get; set; }

        /// <summary>
        /// Burn dataGridBurnFileList Colums
        /// </summary>
        public int BurnFilesColumn1 { get; set; }
        /// <summary>
        /// Burn dataGridBurnFileList Colums
        /// </summary>
        public int BurnFilesColumn2 { get; set; }
        /// <summary>
        /// Burn dataGridBurnFileList Colums
        /// </summary>
        public int BurnFilesColumn3 { get; set; }
        /// <summary>
        /// Burn dataGridBurnFileList Colums
        /// </summary>
        public int BurnFilesColumn4 { get; set; }

        /// <summary>
        /// Burn dataGridBurnFileList Colums
        /// </summary>
        public int BurnFilesAudioColumn1 { get; set; }
        /// <summary>
        /// Burn dataGridBurnFileList Colums
        /// </summary>
        public int BurnFilesAudioColumn2 { get; set; }
        /// <summary>
        /// Burn dataGridBurnFileList Colums
        /// </summary>
        public int BurnFilesAudioColumn3 { get; set; }
        /// <summary>
        /// Burn dataGridBurnFileList Colums
        /// </summary>
        public int BurnFilesAudioColumn4 { get; set; }

        /// <summary>
        /// Burn dataGridBurnFileList Colums
        /// </summary>
        public int BurnDialogSizeX { get; set; }
        /// <summary>
        /// Burn dataGridBurnFileList Colums
        /// </summary>
        public int BurnDialogSizeY { get; set; }

        /// <summary>
        /// Burn dataGridBurnFileList Colums
        /// </summary>
        public int BurnGridDirsSize { get; set; }
        /// <summary>
        /// Burn dataGridBurnFileList Colums
        /// </summary>
        public int SelectGridDirsSize { get; set; }
        
        /// <summary>
        /// Burn comboBurner
        /// </summary>
        public int BurnSelectedBurner { get; set; }
        /// <summary>
        /// Burn comboBoxType
        /// </summary>
        public int BurnTypeMedium { get; set; }
        /// <summary>
        /// Burn comboBoxVerify
        /// </summary>
        public int BurnVerifyType { get; set; }
        /// <summary>
        /// Burn buttonToggleEject
        /// </summary>
        public bool BurnEjectMedia { get; set; }
        /// <summary>
        /// Burn buttonToggleGAP
        /// </summary>
        public bool BurnGapTracks { get; set; }
        /// <summary>
        /// BurnDialogTop
        /// </summary>
        public int BurnDialogTop { get; set; }
        /// <summary>
        /// BurnDialogTop
        /// </summary>
        public int BurnDialogLeft { get; set; }
        /// <summary>
        /// BurnEndSoundFile - Wenn brennen fertig, dann diesen sound spielen.
        /// </summary>
        public string BurnEndSoundFile { get; set; }
                /// <summary>
        /// BurnEndPlaySound - Wenn brennen fertig,soudn spielen?
        /// </summary>
        public bool BurnEndPlaySound { get; set; }
        
        #endregion

        /// <summary>
        /// Einstellungen aus der Registry laden 
        /// </summary>
        public void Load()
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(HitbaseRegistryKey);

            if (regKey == null)
                regKey = Registry.CurrentUser.CreateSubKey(HitbaseRegistryKey);

            if (regKey == null)     // Sollte eigentlich nicht sein
                return;

            LastDataBase = (string)regKey.GetValue("LastDataBase", "");

            string[] recentList = (string[])regKey.GetValue("LoanedToRecentList", null);
            if (recentList != null)
            {
                loanedToRecentList.AddRange(recentList); 
            }

            string[] recentFileList = (string[])regKey.GetValue("RecentFileList", null);
            if (recentFileList != null)
            {
                RecentFileList.AddRange(recentFileList);
            }

            CenterCurrentTrack = (int)regKey.GetValue("CenterCurrentTrack", 0) > 0 ? true : false;
            DisableCDText = (int)regKey.GetValue("DisableCDText", 0) > 0 ? true : false;

            AutoRepeat = (int)regKey.GetValue("AutoRepeat", 0) > 0 ? true : false;
            OnStart = (int)regKey.GetValue("OnStart", 0);
            IntroStart = (int)regKey.GetValue("IntroStart", 0);
            IntroSeconds = (int)regKey.GetValue("IntroSeconds", 0);
            OnNewCD = (int)regKey.GetValue("OnNewCD", 0);
            OnExit = (int)regKey.GetValue("OnExit", 0);
            LapView = (int)regKey.GetValue("LapView", 0);
            
          


            AutoNewOpen = (int)regKey.GetValue("AutoNewOpen", 0) > 0 ? true : false;
            AutoBackup = (int)regKey.GetValue("AutoBackup", 0) > 0 ? true : false;
            ShowOnlyUsedCodes = (int)regKey.GetValue("ShowOnlyUsedCodes", 0) > 0 ? true : false;
            AutoDateToday = (int)regKey.GetValue("AutoDateToday", 0) > 0 ? true : false;
            AutoCompleteArtist = (int)regKey.GetValue("AutoCompleteArtist", 1) > 0 ? true : false;
            AutoCompleteCDTitle = (int)regKey.GetValue("AutoCompleteCDTitle", 1) > 0 ? true : false;
            AutoCompleteTrackname = (int)regKey.GetValue("AutoCompleteTrackname", 1) > 0 ? true : false;
            AutoSaveCDs = (int)regKey.GetValue("AutoSaveCDs", 1) > 0 ? true : false;
            AutoBackupDirectory = (string)regKey.GetValue("AutoBackupDirectory", "");
            ArtistDisplay = (int)regKey.GetValue("ArtistDisplay", 0);


            AutoCheckNewVersion = (int)regKey.GetValue("AutoCheckNewVersion", 1) > 0 ? true : false;
            AutoCheckAnnouncement = (int)regKey.GetValue("AutoCheckAnnouncement", 0) > 0 ? true : false;

            AutoShowCover = (int)regKey.GetValue("AutoShowCover", 0) > 0 ? true : false;
            EqualizerQuality = (int)regKey.GetValue("EqualizerQuality", 0);
            ShowSplashScreen = (int)regKey.GetValue("ShowSplashScreen", 1) > 0 ? true : false;


            SymbolInTaskbar = (int)regKey.GetValue("SymbolInTaskbar", 0) > 0 ? true : false;
            ShowSymbol = (int)regKey.GetValue("ShowSymbol", 0);


            DontSearchDataCDsInCDArchive = (int)regKey.GetValue("DontSearchDataCDsInCDArchive", 0) > 0 ? true : false;
            AutoInsertQueue = (int)regKey.GetValue("AutoInsertQueue", 0) > 0 ? true : false;

            CrossFadeDefaultSeconds = (int)regKey.GetValue("CrossFadeDefaultSeconds", 10);
            CrossFadeActive = (int)regKey.GetValue("CrossFadeActive", 0) > 0 ? true : false;
            AutoVolumeAdjustMax = (int)regKey.GetValue("AutoVolumeAdjustMax", 200);
            StopTimeBeforeEnd = (int)regKey.GetValue("StopTimeBeforeEnd", 0);
            VirtualCDOutputDevice = (int)regKey.GetValue("VirtualCDOutputDevice", 0);
            VirtualCDBufferSize = (int)regKey.GetValue("VirtualCDBufferSize", 3);


            LoanedDefaultTimeSpan = (string)regKey.GetValue("LoanedDefaultTimeSpan", "");
            PersonGroupListType = (int)regKey.GetValue("PersonGroupListType", 0);
            PrintDetailsFont = GetFontValue(regKey, "PrintDetailsFont", new Font("Arial", 8));
            PrintHeaderFilter = (int)regKey.GetValue("PrintHeaderShowFilter", 1) > 0 ? true : false;
            PrintHeaderDate = (int)regKey.GetValue("PrintHeaderShowDate", 1) > 0 ? true : false;
            PrintHeaderSort = (int)regKey.GetValue("PrintHeaderShowSort", 1) > 0 ? true : false;
            
            PrintCDCover = (int)regKey.GetValue("PrintCDCover", 1) > 0 ? true : false;
            PrintTextUnderCDCover = (int)regKey.GetValue("PrintTextUnderCDCover", 0) > 0 ? true : false;
            PrintCDCoverSize = (int)regKey.GetValue("PrintCDCoverSize", 30);
            PrintCDCoverAlign = (int)regKey.GetValue("PrintCDCoverAlign", 2);
           
            DefaultCDCoverPath = (string)regKey.GetValue("DefaultCDCoverPath", "");

        	AdjustSpelling = (int)regKey.GetValue("AdjustSpelling", 2);

            NoDuplicateArchiveNumbers = (int)regKey.GetValue("NoDuplicateArchiveNumbers", 1) > 0 ? true : false;
            SortArchiveNumberNumeric = (int)regKey.GetValue("ArchiveNumberNumeric", 1) > 0 ? true : false;

            SamplerUseFixedArtist = (int)regKey.GetValue("SamplerUseText", 1) > 0 ? true : false;
            SamplerFixedArtistText = (string)regKey.GetValue("SamplerText", StringTable.Diverse);

            LastCatalogListType = (int)regKey.GetValue("LastCatalogListType", 2);
            OutputDevice = (int)regKey.GetValue("OutputDevice", 0);
            OutputDevicePreListen = (int)regKey.GetValue("OutputDevicePreListen", 0);
            UseID3Version = (int)regKey.GetValue("UseID3Version", 2);

            ManageSoundFilesMinSize = (int)regKey.GetValue("ManageSoundFilesMinSize", 10);
            ManageSoundFilesLastFolder = (string)regKey.GetValue("ManageSoundFilesLastFolder", "");

            ManageSoundFilesCreateAlbumInfo = (int)regKey.GetValue("ManageSoundFilesCreateAlbumInfo", 1) > 0 ? true : false;
            ManageSoundFilesCreateOneCollection = (int)regKey.GetValue("ManageSoundFilesCreateOneCollection", 0) > 0 ? true : false;

            ManageSoundFilesCreateOneCollectionTitle1 = (string)regKey.GetValue("ManageSoundFilesCreateOneCollectionTitle1", StringTable.Diverse);
            ManageSoundFilesCreateOneCollectionTitle2 = (string)regKey.GetValue("ManageSoundFilesCreateOneCollectionTitle2", StringTable.SoundCollection);

            DetectTrackInfoFromFilename = (int)regKey.GetValue("DetectTrackInfoFromFilename", 1) > 0 ? true : false;
            DetectTrackInfoDelimiter = (string)regKey.GetValue("DetectTrackInfoDelimiter", " - ");
            DeleteLeadingNumbers = (int)regKey.GetValue("DeleteLeadingNumbers", 1) > 0 ? true : false;

            PreListenVirgin = (int)regKey.GetValue("PreListenVirgin", 1) > 0 ? true : false;

            PartyModusEnablePassword = (int)regKey.GetValue("PartyModusEnablePassword", 0) > 0 ? true : false;
            PartyModusPassword = (string)regKey.GetValue("PartyModusPassword", "");

            LastAnnouncementDate = (string)regKey.GetValue("LastAnnouncement", "");

            AutoSearchCover = (int)regKey.GetValue("AutoSearchCover", 1) > 0 ? true : false;
            AutoSearchLyrics = (int)regKey.GetValue("AutoSearchLyrics", 0) > 0 ? true : false;

            CurrentColorStyle = (ColorStyle)regKey.GetValue("CurrentColorStyle", 0);

            CatalogTreeViewWidth = (int)regKey.GetValue("CatalogTreeViewWidth", 300);

            InternetConnection = (int)regKey.GetValue("InternetConnection", -1);
            ProxyType = (int)regKey.GetValue("ProxyType", 0);
            ProxyServerName = (string)regKey.GetValue("ProxyServerName", "");
            HttpTimeoutInMS = (int)regKey.GetValue("HttpTimeout", 10000);     // 10 sek default

            MyHitbaseLastUserName = (string)regKey.GetValue("MyHitbaseLastUserName", "");
            MyHitbaseLastPassword = (string)regKey.GetValue("MyHitbaseLastPassword", "");
            MyHitbaseDontSendImages = (int)regKey.GetValue("MyHitbaseDontSendImages", 0) > 0 ? true : false;

            ShowParticipantPictures = (int)regKey.GetValue("ShowParticipantPictures", 0) > 0 ? true : false;

            RecordFilenameFormat = (string[])regKey.GetValue("RecordFilenameFormat", null);
            // Record Medium Values
            if (RecordFilenameFormat == null)
            {
                RecordFilenameFormat = new String[4];

                RecordFilenameFormat[0] = "[Interpret, Name]\\[Album]\\[Track - Titel]"; 
                RecordFilenameFormat[1] = "[Interpret, Name]\\[Album]\\[Track - Nr.] - [Track - Titel]";
                RecordFilenameFormat[2] = "[Interpret, Name]\\[Album]\\[Track - Nr.] - [Track - Interpret, Name] - [Track - Titel]";
                RecordFilenameFormat[3] = "[Interpret, Name]\\[Genre]\\[Album]\\[Track - Nr.] - [Track - Interpret, Name] - [Track - Titel]";
            }

            RecordFormatFile = (string)regKey.GetValue("RecordFormatFile", "[Interpret, Name]\\[Album]\\[Track - Nr.] - [Track - Titel]");
            RecordMediumLastDriveLetter = (string)regKey.GetValue("RecordMediumLastDriveLetter", "");
            RecordSelectedPath = (string)regKey.GetValue("RecordSelectedPath", "");
            //RecordMethod = (int)regKey.GetValue("RecordMethod", 0);
            RecordAutoEject = (int)regKey.GetValue("RecordAutoEjectFinish", 0) == 1 ? true : false;
            //RecordAnalogFormat = (string)regKey.GetValue("RecordAnalogFormat", "");
            //RecordDirectSpeed = (string)regKey.GetValue("RecordDirectSpeed", "");
            RecordCopyRegion = (int)regKey.GetValue("RecordCopyRegion", 0);
            RecordSelectedTracks = (string)regKey.GetValue("RecordSelectedTracks", "");
            //RecordStartPositionPerTrack = (string)regKey.GetValue("RecordStartPositionPerTrack", "");
            //RecordLengthPerTrack = (string)regKey.GetValue("RecordLengthPerTrack", "");
            //RecordStartPosition = (int)regKey.GetValue("RecordStartPosition", 0);
            //RecordLengthPosition = (int)regKey.GetValue("RecordLengthPosition", 0);
            RecordFormat = (int)regKey.GetValue("RecordFormat", 0);
            RecordFileNamesType = (int)regKey.GetValue("RecordFileNamesType", 0);
            RecordWriteID3Tags = (int)regKey.GetValue("RecordWriteID3Tags", 1);
            RecordFileNamesPrefix = (string)regKey.GetValue("RecordFileNamesPrefix", "");

            RecordFileNameCharBackslash = (string)regKey.GetValue("RecordFileNameCharBackslash", "");
            RecordFileNameCharSlash = (string)regKey.GetValue("RecordFileNameCharSlash", "");
            RecordFileNameCharDoppelpunkt = (string)regKey.GetValue("RecordFileNameCharDoppelpunkt", "");
            RecordFileNameCharStern = (string)regKey.GetValue("RecordFileNameCharStern", "");
            RecordFileNameCharFragezeichen = (string)regKey.GetValue("RecordFileNameFragezeichen", "");
            RecordFileNameCharAnfuehrung = (string)regKey.GetValue("RecordFileNameCharAnfuehrung", "");
            RecordFileNameCharKleiner = (string)regKey.GetValue("RecordFileNameCharKleiner", "");
            RecordFileNameCharGroesser = (string)regKey.GetValue("RecordFileNameCharGroesser", "");
            RecordFileNameCharPipe = (string)regKey.GetValue("RecordFileNameCharPipe", "");

            RecordFileNameCharUserOrg1 = (string)regKey.GetValue("RecordFileNameCharUserOrg1", "");
            RecordFileNameCharUserOrg2 = (string)regKey.GetValue("RecordFileNameCharUserOrg2", "");
            RecordFileNameCharUserOrg3 = (string)regKey.GetValue("RecordFileNameCharUserOrg3", "");
            RecordFileNameCharUserOrg4 = (string)regKey.GetValue("RecordFileNameCharUserOrg4", "");
            RecordFileNameCharUserOrg5 = (string)regKey.GetValue("RecordFileNameCharUserOrg5", "");
            RecordFileNameCharUserOrg6 = (string)regKey.GetValue("RecordFileNameCharUserOrg6", "");

            RecordFileNameCharUserNew1 = (string)regKey.GetValue("RecordFileNameCharUserNew1", "");
            RecordFileNameCharUserNew2 = (string)regKey.GetValue("RecordFileNameCharUserNew2", "");
            RecordFileNameCharUserNew3 = (string)regKey.GetValue("RecordFileNameCharUserNew3", "");
            RecordFileNameCharUserNew4 = (string)regKey.GetValue("RecordFileNameCharUserNew4", "");
            RecordFileNameCharUserNew5 = (string)regKey.GetValue("RecordFileNameCharUserNew5", "");
            RecordFileNameCharUserNew6 = (string)regKey.GetValue("RecordFileNameCharUserNew6", "");

            RecordLameExeParameter = (string)regKey.GetValue("RecordLameExeParameter", "-V 2 --vbr-new %1 %2");
            RecordOggExeParameter = (string)regKey.GetValue("RecordOggExeParameter", "-q 6 %1 %2");
            RecordFlacExeParameter = (string)regKey.GetValue("RecordFlacExeParameter", "-5 %1 %2");
            RecordUser1ExeParameter = (string)regKey.GetValue("RecordUser1ExeParameter", "");
            RecordUser2ExeParameter = (string)regKey.GetValue("RecordUser2ExeParameter", "");
            
            RecordLastSelectedFormat = (int)regKey.GetValue("RecordLastSelectedFormat", 0);
            RecordLastQuickSelectedFormat = (int)regKey.GetValue("RecordLastQuickSelectedFormat", 0);
            RecordLastQuickSelectedMP3Quality = (int)regKey.GetValue("RecordLastQuickSelectedMP3Quality", 4);
            RecordLastQuickSelectedWMAQuality = (int)regKey.GetValue("RecordLastQuickSelectedWMAQuality", 4);
            RecordLastQuickSelectedOGGQuality = (int)regKey.GetValue("RecordLastQuickSelectedOGGQuality", 3);
            RecordLastQuickSelectedFLACQuality = (int)regKey.GetValue("RecordLastQuickSelectedFLACQuality", 1);
            RecordAutoCDCopy = (int)regKey.GetValue("RecordAutoCDCopy", 0) == 1 ? true : false;
            RecordAutoCreateM3U = (int)regKey.GetValue("RecordAutoCreateM3U", 0) == 1 ? true : false;
            RecordSaveCDCover = (int)regKey.GetValue("RecordSaveCDCover", 0) == 1 ? true : false;

            RecordMP3CustomCBR = (int)regKey.GetValue("RecordMP3CustomCBR", 0) == 1 ? true : false;
            RecordMP3CustomCopyright = (int)regKey.GetValue("RecordMP3CustomCBRCopyright", 0) == 1 ? true : false;

            RecordMP3CustomCRC = (int)regKey.GetValue("RecordMP3CustomCBRCRC", 1) == 1 ? true : false;
            RecordMP3CustomPrivate = (int)regKey.GetValue("RecordMP3CustomCBRPrivate", 0) == 1 ? true : false;
            RecordMP3CustomOriginal = (int)regKey.GetValue("RecordMP3CustomCBROriginal", 0) == 1 ? true : false;
            RecordMP3CustomCBRBitRates = (int)regKey.GetValue("RecordMP3CustomCBRBitRates", 192);
            RecordMP3CustomChannels = (int)regKey.GetValue("RecordMP3CustomChannels", 0);

            RecordMP3CustomVBR = (int)regKey.GetValue("RecordMP3CustomVBR", 1) == 1 ? true : false;
            RecordMP3CustomVBRMethod = (int)regKey.GetValue("RecordMP3CustomVBRMethod", 0);
            RecordMP3CustomVBRMin = (int)regKey.GetValue("RecordMP3CustomVBRMin", 128);
            RecordMP3CustomVBRMax = (int)regKey.GetValue("RecordMP3CustomVBRMax", 320);
            RecordMP3CustomVBRAverage = (int)regKey.GetValue("RecordMP3CustomVBRAverage", 192);
            RecordMP3CustomVBRQuality = (int)regKey.GetValue("RecordMP3CustomVBRQuality", 7);

            RecordSpeed = (int)regKey.GetValue("RecordSpeed", 0);

            BurnSelectFilesColumn1 = (int)regKey.GetValue("BurnSelectFilesColumn1", 200);
            BurnSelectFilesColumn2 = (int)regKey.GetValue("BurnSelectFilesColumn2", 100);
            BurnSelectFilesColumn3 = (int)regKey.GetValue("BurnSelectFilesColumn3", 60);
            BurnSelectFilesColumn4 = (int)regKey.GetValue("BurnSelectFilesColumn4", 30);

            BurnFilesColumn1 = (int)regKey.GetValue("BurnFilesColumn1", 100);
            BurnFilesColumn2 = (int)regKey.GetValue("BurnFilesColumn2", 50);
            BurnFilesColumn3 = (int)regKey.GetValue("BurnFilesColumn3", 30);
            BurnFilesColumn4 = (int)regKey.GetValue("BurnFilesColumn4", 15);

            BurnFilesAudioColumn1 = (int)regKey.GetValue("BurnFilesAudioColumn1", 300);
            BurnFilesAudioColumn2 = (int)regKey.GetValue("BurnFilesAudioColumn2", 150);
            BurnFilesAudioColumn3 = (int)regKey.GetValue("BurnFilesAudioColumn3", 90);
            BurnFilesAudioColumn4 = (int)regKey.GetValue("BurnFilesAudioColumn4", 70);

            BurnDialogSizeX = (int)regKey.GetValue("BurnDialogSizeX", 800);
            BurnDialogSizeY = (int)regKey.GetValue("BurnDialogSizeY", 700);

            BurnGridDirsSize = (int)regKey.GetValue("BurnGridDirsSize", 300);
            
            BurnSelectedBurner = (int)regKey.GetValue("BurnSelectedBurner", 0);
            BurnTypeMedium = (int)regKey.GetValue("BurnTypeMedium", 0);
            BurnVerifyType = (int)regKey.GetValue("BurnVerifyType", 0);
            BurnEjectMedia = (int)regKey.GetValue("BurnEjectMedia", 0) > 0 ? true : false;
            BurnGapTracks = (int)regKey.GetValue("BurnGapTracks", 0) > 0 ? true : false;
                       
            BurnDialogTop = (int)regKey.GetValue("BurnDialogTop", 0);
            BurnDialogLeft = (int)regKey.GetValue("BurnDialogLeft", 0);
            BurnEndSoundFile = (string)regKey.GetValue("BurnEndSoundFile", "");
            BurnEndPlaySound = (int)regKey.GetValue("BurnEndPlaySound", 0) > 0 ? true : false;
            SelectGridDirsSize = (int)regKey.GetValue("SelectGridDirsSize", 300);

            NormalizeActive = (int)regKey.GetValue("NormalizeActive", 0) > 0 ? true : false;
            NormalizePercent = (int)regKey.GetValue("NormalizePercent", 98);
            NormalizePercentMin = (int)regKey.GetValue("NormalizePercentMin", 85);
            NormalizePercentMax = (int)regKey.GetValue("NormalizePercentMax", 99);

            ShowNavigationBar = (int)regKey.GetValue("ShowNavigationBar", 1) > 0 ? true : false;
            ShowPlaylist = (int)regKey.GetValue("ShowPlaylist", 1) > 0 ? true : false;
            ShowPlayer = (int)regKey.GetValue("ShowPlayer", 1) > 0 ? true : false;

            MyMusicViewType = (int)regKey.GetValue("MyMusicViewType", 0);
            AlbumViewType = (int)regKey.GetValue("AlbumViewType", 0);
            DirectoryViewType = (int)regKey.GetValue("DirectoryViewType", 0);

            ScrobbleActive = (int)regKey.GetValue("ScrobbleActive", 0) > 0 ? true : false;

            HitbaseServer = (string)regKey.GetValue("HitbaseServer", "www.hitbase.de");

            FreeArchiveNumberSearchWithGaps = (int)regKey.GetValue("FreeArchiveNumberSearchWithGaps", 0) > 0 ? true : false;

            ShowFrequencyBand = (int)regKey.GetValue("ShowFrequencyBand", 1) > 0 ? true : false;
            ShowLyrics = (int)regKey.GetValue("ShowLyrics", 0) > 0 ? true : false;

            PartyModeAllowPlayNext = (int)regKey.GetValue("PartyModeAllowPlayNext", 1) > 0 ? true : false;
            PartyModeAllowPlayLast = (int)regKey.GetValue("PartyModeAllowPlayLast", 1) > 0 ? true : false;

            PlaylistWidth = (int)regKey.GetValue("PlaylistWidth", 200);
            MainTreeWidth = (int)regKey.GetValue("MainTreeWidth", 200);

            PartyModePlaylistPinned = (int)regKey.GetValue("PartyModePlaylistPinned", 1) > 0 ? true : false;
            PartyModeWishlistPinned = (int)regKey.GetValue("PartyModeWishlistPinned", 1) > 0 ? true : false;

            RibbonIsMinimized = (int)regKey.GetValue("RibbonIsMinimized", 0) > 0 ? true : false;
            ShowQuickAccessToolBarOnTop = (int)regKey.GetValue("ShowQuickAccessToolBarOnTop", 1) > 0 ? true : false;
            RibbonQuickAccessToolBar = (string)regKey.GetValue("RibbonQuickAccessToolBar", "");

            UseMCI = (int)regKey.GetValue("UseMCI", 0) > 0 ? true : false;

            DontShowFirstSteps = (int)regKey.GetValue("DontShowFirstSteps", 0) > 0 ? true : false;

            DisableScreenSaver = (int)regKey.GetValue("DisableScreenSaver", 1) > 0 ? true : false;

            AutoSortGenres = (int)regKey.GetValue("AutoSortGenres", 1) > 0 ? true : false;
            AutoSortMediums = (int)regKey.GetValue("AutoSortMediums", 1) > 0 ? true : false;
            TimeFormat = (TimeFormat)(int)regKey.GetValue("TimeFormat", (int)TimeFormat.HHMMSS);

            regKey.Close();

            ReadCDArchiveConfig();
        }

        private Font GetFontValue(RegistryKey regKey, string key, Font defaultFont)
        {
            string fontFamily = defaultFont.FontFamily.Name;
            float fontSize = defaultFont.Size;
            int fontStyle = (int)defaultFont.Style;

            fontFamily = (string)regKey.GetValue(key + "Family", fontFamily);
            fontSize = (float)(int)regKey.GetValue(key + "Size", (int)fontSize);
            fontStyle = (int)regKey.GetValue(key + "Style", fontStyle);

            Font font = new Font(fontFamily, fontSize, (FontStyle)fontStyle);
            return font;
        }

        public List<CDArchiveConfig> CDArchives = new List<CDArchiveConfig>();
        public CDArchiveFields CDArchiveFields { get; set; }

        void ReadCDArchiveConfig()
        {
            RegistryKey regKey = Registry.CurrentUser.CreateSubKey(HitbaseRegistryKey + "\\CDArchives");

	        // Im Start-Assistenten von Hitbase kann angegeben werden, ob man eine
	        // Internet-Verbindung hat. Wenn "nein" gewählt wird, werden standardmäßig
	        // alle CD-Archive deaktiviert.
	        bool active = true;
	        if (InternetConnection == 0)     // 0 = alle deaktivieren.
		        active = false;
            
	        InternetConnection = -1;

	        // Das sind die Standard-Archive, die von Hitbase vorgegeben werden.
	        CDArchives.Add(new CDArchiveConfig(CDArchiveType.BIG3, "www.cdarchiv.de", active, true, true, false, ""));
	        CDArchives.Add(new CDArchiveConfig(CDArchiveType.CDDBsockets, "freedb.freedb.org:8880", active, true, false, true, "/"));

            CDArchiveFields = (CDArchiveFields)regKey.GetValue("ArchiveFields", (int)(CDArchiveFields.Category | CDArchiveFields.Medium | CDArchiveFields.BPM));
	        int nCount = (int)regKey.GetValue("NumberOfArchives", (int)-1);

	        if (nCount >= 0)
		        CDArchives.Clear();

	        // Jetzt alle CD-Archive aus Sub-Keys lesen
	        for (int i=0;i<nCount;i++)
	        {
		        RegistryKey regArchiveKey;
		        String strArchiveKey;

                strArchiveKey = string.Format("{0}\\CDArchives\\{1}", HitbaseRegistryKey, i);

                regArchiveKey = Registry.CurrentUser.CreateSubKey(strArchiveKey);

		        CDArchiveType type = (CDArchiveType)regArchiveKey.GetValue("Type", -1);
		        string archiveName = (string)regArchiveKey.GetValue("ArchiveName", "");
		        bool archiveActive = (int)regArchiveKey.GetValue("Active", 0) != 0 ? true : false;
		        bool autoSearch = (int)regArchiveKey.GetValue("AutoSearch", 0) != 0 ? true : false;
		        bool upload = (int)regArchiveKey.GetValue("Upload", 0) != 0 ? true : false;
		        bool autoCreateSampler = (int)regArchiveKey.GetValue("AutoCreateSampler", 0) != 0 ? true : false;
		        String samplerTrennzeichen = (string)regArchiveKey.GetValue("SamplerSeperator", "");

		        CDArchives.Add(new CDArchiveConfig(type, archiveName, archiveActive, autoSearch, upload, autoCreateSampler, samplerTrennzeichen));

		        regArchiveKey.Close();
	        }

            regKey.Close();
        }

        void WriteCDArchiveConfig()
        {
            string saveKey = HitbaseRegistryKey + "\\CDArchives";
            RegistryKey regKey = Registry.CurrentUser.CreateSubKey(saveKey);

            regKey.SetValue("ArchiveFields", (int)this.CDArchiveFields);
            regKey.SetValue("NumberOfArchives", this.CDArchives.Count);

	        // Jetzt alle CD-Archive in Sub-Keys schreiben
            for (int i = 0; i < this.CDArchives.Count; i++)
            {
		        string archiveKey;

		        archiveKey = string.Format("{0}\\{1}", saveKey, i);

		        RegistryKey regKeyArchive = Registry.CurrentUser.CreateSubKey(archiveKey);

		        regKeyArchive.SetValue("Type", (int)CDArchives[i].Type);
		        regKeyArchive.SetValue("ArchiveName", CDArchives[i].ArchiveName);
		        regKeyArchive.SetValue("Active", CDArchives[i].Active ? 1 : 0);
		        regKeyArchive.SetValue("AutoSearch", CDArchives[i].AutoSearch ? 1 : 0);
		        regKeyArchive.SetValue("Upload", CDArchives[i].Upload ? 1 : 0);
		        regKeyArchive.SetValue("AutoCreateSampler", CDArchives[i].AutoCreateSampler ? 1 : 0);
		        regKeyArchive.SetValue("SamplerSeperator", CDArchives[i].SamplerTrennzeichen);

		        regKeyArchive.Close();
	        }

            regKey.Close();
        }


        private void SetFontValue(RegistryKey regKey, string key, Font font)
        {
            regKey.SetValue(key + "Family", font.FontFamily.Name);
            regKey.SetValue(key + "Size", (int)font.Size);
            regKey.SetValue(key + "Style", (int)font.Style);
        }

        /// <summary>
        /// Einstellungen in die Registry speichern
        /// </summary>
        public void Save()
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(HitbaseRegistryKey, true);

            regKey.SetValue("LastDataBase", LastDataBase);

            regKey.SetValue("CenterCurrentTrack", CenterCurrentTrack ? 1 : 0);
            regKey.SetValue("DisableCDText", DisableCDText ? 1 : 0);

            regKey.SetValue("AutoRepeat", AutoRepeat ? 1 : 0);
            regKey.SetValue("OnStart", OnStart);
            regKey.SetValue("IntroStart", IntroStart);
            regKey.SetValue("IntroSeconds", IntroSeconds);
            regKey.SetValue("OnNewCD", OnNewCD);
            regKey.SetValue("OnExit", OnExit);
            regKey.SetValue("LapView", LapView);

            
            regKey.SetValue("AutoNewOpen", AutoNewOpen ? 1 : 0);
            regKey.SetValue("AutoBackup", AutoBackup ? 1 : 0);
            regKey.SetValue("ShowOnlyUsedCodes", ShowOnlyUsedCodes ? 1 : 0);
            regKey.SetValue("AutoDateToday", AutoDateToday ? 1 : 0);
            regKey.SetValue("AutoCompleteArtist", AutoCompleteArtist ? 1 : 0);
            regKey.SetValue("AutoCompleteCDTitle", AutoCompleteCDTitle ? 1 : 0);
            regKey.SetValue("AutoCompleteTrackname", AutoCompleteTrackname ? 1 : 0);
            regKey.SetValue("AutoSaveCDs", AutoSaveCDs ? 1 : 0);
            regKey.SetValue("AutoBackupDirectory", AutoBackupDirectory);
            regKey.SetValue("ArtistDisplay", ArtistDisplay);



            regKey.SetValue("AutoCheckNewVersion", AutoCheckNewVersion ? 1 : 0);
            regKey.SetValue("AutoCheckAnnouncement", AutoCheckAnnouncement ? 1 : 0);
            regKey.SetValue("AutoShowCover", AutoShowCover ? 1 : 0);
            regKey.SetValue("EqualizerQuality", EqualizerQuality);
            regKey.SetValue("ShowSplashScreen", ShowSplashScreen ? 1 : 0);


            regKey.SetValue("SymbolInTaskbar", SymbolInTaskbar ? 1 : 0);
            regKey.SetValue("ShowSymbol", ShowSymbol);

            regKey.SetValue("DontSearchDataCDsInCDArchive", DontSearchDataCDsInCDArchive ? 1 : 0);
            regKey.SetValue("AutoInsertQueue", AutoInsertQueue ? 1 : 0);


            regKey.SetValue("CrossFadeDefaultSeconds", CrossFadeDefaultSeconds);
            regKey.SetValue("CrossFadeActive", CrossFadeActive ? 1 : 0);
            regKey.SetValue("AutoVolumeAdjustMax", AutoVolumeAdjustMax);
            regKey.SetValue("StopTimeBeforeEnd", StopTimeBeforeEnd);

            regKey.SetValue("VirtualCDOutputDevice", VirtualCDOutputDevice);
            regKey.SetValue("VirtualCDBufferSize", VirtualCDBufferSize);


            regKey.SetValue("LoanedToRecentList", LoanedToRecentList.ToArray());
            regKey.SetValue("LoanedDefaultTimeSpan", loanedDefaultTimeSpan);
            regKey.SetValue("PersonGroupListType", PersonGroupListType);
            SetFontValue(regKey, "PrintDetailsFont", PrintDetailsFont);

            regKey.SetValue("PrintHeaderShowFilter", PrintHeaderFilter ? 1 : 0);
            regKey.SetValue("PrintHeaderShowSort", PrintHeaderSort ? 1 : 0);
            regKey.SetValue("PrintHeaderShowDate", PrintHeaderDate ? 1 : 0);

            regKey.SetValue("PrintCDCover", PrintCDCover ? 1 : 0);
            regKey.SetValue("PrintTextUnderCDCover", PrintTextUnderCDCover ? 1 : 0);
            regKey.SetValue("PrintCDCoverSize", PrintCDCoverSize);
            regKey.SetValue("PrintCDCoverAlign", PrintCDCoverAlign);

            regKey.SetValue("DefaultCDCoverPath", DefaultCDCoverPath);

            regKey.SetValue("AdjustSpelling", AdjustSpelling);

            regKey.SetValue("NoDuplicateArchiveNumbers", NoDuplicateArchiveNumbers ? 1 : 0);
            regKey.SetValue("ArchiveNumberNumeric", SortArchiveNumberNumeric ? 1 : 0);

            regKey.SetValue("SamplerUseText", SamplerUseFixedArtist ? 1 : 0);
            regKey.SetValue("SamplerText", SamplerFixedArtistText);

            regKey.SetValue("LastCatalogListType", LastCatalogListType);
            regKey.SetValue("OutputDevice", OutputDevice);
            regKey.SetValue("OutputDevicePreListen", OutputDevicePreListen);
            regKey.SetValue("UseID3Version", UseID3Version);

            regKey.SetValue("ManageSoundFilesMinSize", ManageSoundFilesMinSize);
            regKey.SetValue("ManageSoundFilesLastFolder", ManageSoundFilesLastFolder);

            regKey.SetValue("ManageSoundFilesCreateAlbumInfo", ManageSoundFilesCreateAlbumInfo ? 1 : 0);
            regKey.SetValue("ManageSoundFilesCreateOneCollection", ManageSoundFilesCreateOneCollection ? 1 : 0);
            regKey.SetValue("ManageSoundFilesCreateOneCollectionTitle1", ManageSoundFilesCreateOneCollectionTitle1);
            regKey.SetValue("ManageSoundFilesCreateOneCollectionTitle2", ManageSoundFilesCreateOneCollectionTitle2);

            regKey.SetValue("DetectTrackInfoFromFilename", DetectTrackInfoFromFilename ? 1 : 0);
            regKey.SetValue("DetectTrackInfoDelimiter", DetectTrackInfoDelimiter);
            regKey.SetValue("DeleteLeadingNumbers", DeleteLeadingNumbers ? 1 : 0);

            regKey.SetValue("PreListenVirgin", PreListenVirgin ? 1 : 0);

            regKey.SetValue("CatalogTreeViewWidth", CatalogTreeViewWidth);

            regKey.SetValue("PartyModusEnablePassword", PartyModusEnablePassword ? 1 : 0);
            regKey.SetValue("PartyModusPassword", PartyModusPassword);

            regKey.SetValue("LastAnnouncement", LastAnnouncementDate);

            regKey.SetValue("AutoSearchCover", AutoSearchCover ? 1 : 0);
            regKey.SetValue("AutoSearchLyrics", AutoSearchLyrics ? 1 : 0);

            regKey.SetValue("CurrentColorStyle", (int)CurrentColorStyle);

            regKey.SetValue("InternetConnection", InternetConnection);
            regKey.SetValue("ProxyType", ProxyType);
            regKey.SetValue("ProxyServerName", ProxyServerName);
            regKey.SetValue("HttpTimeout", HttpTimeoutInMS);

            regKey.SetValue("MyHitbaseLastUserName", MyHitbaseLastUserName);
            regKey.SetValue("MyHitbaseLastPassword", MyHitbaseLastPassword);
            regKey.SetValue("MyHitbaseDontSendImages", MyHitbaseDontSendImages ? 1 : 0);

            regKey.SetValue("ShowParticipantPictures", ShowParticipantPictures ? 1 : 0);

            // Record medium values
            regKey.SetValue("RecordFormatFile", RecordFormatFile);
            regKey.SetValue("RecordMediumLastDriveLetter", RecordMediumLastDriveLetter);        
            regKey.SetValue("RecordSelectedPath", RecordSelectedPath);
            //regKey.SetValue("RecordMethod", RecordMethod);
            regKey.SetValue("RecordAutoEjectFinish", RecordAutoEject ? 1 : 0);
            if (RecordFilenameFormat != null)
                regKey.SetValue("RecordFilenameFormat", RecordFilenameFormat);
            //regKey.SetValue("RecordDirectSpeed", RecordDirectSpeed);
            //regKey.SetValue("RecordAnalogFormat", RecordAnalogFormat);
            regKey.SetValue("RecordCopyRegion", RecordCopyRegion);
            regKey.SetValue("RecordSelectedTracks", RecordSelectedTracks);
            //regKey.SetValue("RecordStartPositionPerTrack", RecordStartPositionPerTrack);
            //regKey.SetValue("RecordLengthPerTrack", RecordLengthPerTrack);
            //regKey.SetValue("RecordStartPosition", RecordStartPosition);
            //regKey.SetValue("RecordLengthPosition", RecordLengthPosition);
            regKey.SetValue("RecordFormat", RecordFormat);
            regKey.SetValue("RecordFileNamesType", RecordFileNamesType);
            regKey.SetValue("RecordWriteID3Tags", RecordWriteID3Tags);
            regKey.SetValue("RecordFileNamesPrefix", RecordFileNamesPrefix);

            regKey.SetValue("RecordFileNameCharBackslash", RecordFileNameCharBackslash);
            regKey.SetValue("RecordFileNameCharSlash", RecordFileNameCharSlash);
            regKey.SetValue("RecordFileNameCharDoppelpunkt", RecordFileNameCharDoppelpunkt);
            regKey.SetValue("RecordFileNameCharStern", RecordFileNameCharStern);
            regKey.SetValue("RecordFileNameCharFragezeichen", RecordFileNameCharFragezeichen);
            regKey.SetValue("RecordFileNameCharAnfuehrung", RecordFileNameCharAnfuehrung);
            regKey.SetValue("RecordFileNameCharKleiner", RecordFileNameCharKleiner);
            regKey.SetValue("RecordFileNameCharGroesser", RecordFileNameCharGroesser);
            regKey.SetValue("RecordFileNameCharPipe", RecordFileNameCharPipe);

            regKey.SetValue("RecordFileNameCharUserOrg1", RecordFileNameCharUserOrg1);
            regKey.SetValue("RecordFileNameCharUserOrg2", RecordFileNameCharUserOrg2);
            regKey.SetValue("RecordFileNameCharUserOrg3", RecordFileNameCharUserOrg3);
            regKey.SetValue("RecordFileNameCharUserOrg4", RecordFileNameCharUserOrg4);
            regKey.SetValue("RecordFileNameCharUserOrg5", RecordFileNameCharUserOrg5);
            regKey.SetValue("RecordFileNameCharUserOrg6", RecordFileNameCharUserOrg6);

            regKey.SetValue("RecordFileNameCharUserNew1", RecordFileNameCharUserNew1);
            regKey.SetValue("RecordFileNameCharUserNew2", RecordFileNameCharUserNew2);
            regKey.SetValue("RecordFileNameCharUserNew3", RecordFileNameCharUserNew3);
            regKey.SetValue("RecordFileNameCharUserNew4", RecordFileNameCharUserNew4);
            regKey.SetValue("RecordFileNameCharUserNew5", RecordFileNameCharUserNew5);
            regKey.SetValue("RecordFileNameCharUserNew6", RecordFileNameCharUserNew6);

            regKey.SetValue("RecordLameExeParameter", RecordLameExeParameter);
            regKey.SetValue("RecordOggExeParameter", RecordOggExeParameter);
            regKey.SetValue("RecordFlacExeParameter", RecordFlacExeParameter); 
            regKey.SetValue("RecordUser1ExeParameter", RecordUser1ExeParameter);
            regKey.SetValue("RecordUser2ExeParameter", RecordUser2ExeParameter);

            regKey.SetValue("RecordLastSelectedFormat", RecordLastSelectedFormat);
            regKey.SetValue("RecordLastQuickSelectedFormat", RecordLastQuickSelectedFormat);
            regKey.SetValue("RecordLastQuickSelectedMP3Quality", RecordLastQuickSelectedMP3Quality);
            regKey.SetValue("RecordLastQuickSelectedWMAQuality", RecordLastQuickSelectedWMAQuality);
            regKey.SetValue("RecordLastQuickSelectedOGGQuality", RecordLastQuickSelectedOGGQuality);
            regKey.SetValue("RecordLastQuickSelectedFLACQuality", RecordLastQuickSelectedFLACQuality);
            regKey.SetValue("RecordAutoCDCopy", RecordAutoCDCopy ? 1 : 0);
            regKey.SetValue("RecordAutoCreateM3U", RecordAutoCreateM3U ? 1 : 0);
            regKey.SetValue("RecordSaveCDCover", RecordSaveCDCover ? 1 : 0);

            regKey.SetValue("RecordMP3CustomCBR", RecordMP3CustomCBR ? 1 : 0);
            regKey.SetValue("RecordMP3CustomCopyright", RecordMP3CustomCopyright ? 1 : 0);
            regKey.SetValue("RecordMP3CustomCRC", RecordMP3CustomCRC ? 1 : 0);
            regKey.SetValue("RecordMP3CustomPrivate", RecordMP3CustomPrivate ? 1 : 0);
            regKey.SetValue("RecordMP3CustomOriginal", RecordMP3CustomOriginal ? 1 : 0);
            regKey.SetValue("RecordMP3CustomChannels", RecordMP3CustomChannels);
                        
            regKey.SetValue("RecordMP3CustomCBRBitRates", RecordMP3CustomCBRBitRates);

            regKey.SetValue("RecordMP3CustomVBR", RecordMP3CustomVBR ? 1 : 0);
            regKey.SetValue("RecordMP3CustomVBRMethod", RecordMP3CustomVBRMethod);
            regKey.SetValue("RecordMP3CustomVBRMin", RecordMP3CustomVBRMin);
            regKey.SetValue("RecordMP3CustomVBRMax", RecordMP3CustomVBRMax);
            regKey.SetValue("RecordMP3CustomVBRAverage", RecordMP3CustomVBRAverage);
            regKey.SetValue("RecordMP3CustomVBRQuality", RecordMP3CustomVBRQuality);

            regKey.SetValue("RecordSpeed", RecordSpeed);

            regKey.SetValue("BurnSelectFilesColumn1", BurnSelectFilesColumn1);
            regKey.SetValue("BurnSelectFilesColumn2", BurnSelectFilesColumn2);
            regKey.SetValue("BurnSelectFilesColumn3", BurnSelectFilesColumn3);
            regKey.SetValue("BurnSelectFilesColumn4", BurnSelectFilesColumn4);
            
            regKey.SetValue("BurnFilesColumn1", BurnFilesColumn1);
            regKey.SetValue("BurnFilesColumn2", BurnFilesColumn2);
            regKey.SetValue("BurnFilesColumn3", BurnFilesColumn3);
            regKey.SetValue("BurnFilesColumn4", BurnFilesColumn4);

            regKey.SetValue("BurnFilesAudioColumn1", BurnFilesAudioColumn1);
            regKey.SetValue("BurnFilesAudioColumn2", BurnFilesAudioColumn2);
            regKey.SetValue("BurnFilesAudioColumn3", BurnFilesAudioColumn3);
            regKey.SetValue("BurnFilesAudioColumn4", BurnFilesAudioColumn4);

            regKey.SetValue("BurnDialogSizeX", BurnDialogSizeX);
            regKey.SetValue("BurnDialogSizeY", BurnDialogSizeY);

            regKey.SetValue("BurnGridDirsSize", BurnGridDirsSize);

            regKey.SetValue("BurnSelectedBurner", BurnSelectedBurner);
            regKey.SetValue("BurnTypeMedium", BurnTypeMedium);
            regKey.SetValue("BurnVerifyType", BurnVerifyType);
            regKey.SetValue("BurnEjectMedia", BurnEjectMedia ? 1 : 0);
            regKey.SetValue("BurnGapTracks", BurnGapTracks ? 1 : 0);
            regKey.SetValue("BurnDialogTop", BurnDialogTop);
            regKey.SetValue("BurnDialogLeft", BurnDialogLeft);
            regKey.SetValue("BurnEndPlaySound", BurnEndPlaySound ? 1 : 0);
            regKey.SetValue("BurnEndSoundFile", string.IsNullOrEmpty(BurnEndSoundFile) ? "" :  BurnEndSoundFile);

            regKey.SetValue("SelectGridDirsSize", SelectGridDirsSize);

            regKey.SetValue("NormalizeActive", NormalizeActive ? 1 : 0);
            regKey.SetValue("NormalizePercent", NormalizePercent);
            regKey.SetValue("NormalizePercentMin", NormalizePercentMin);
            regKey.SetValue("NormalizePercentMax", NormalizePercentMax);

            regKey.SetValue("RecentFileList", RecentFileList.ToArray());

            regKey.SetValue("ShowNavigationBar", ShowNavigationBar ? 1 : 0);
            regKey.SetValue("ShowPlaylist", ShowPlaylist ? 1 : 0);
            regKey.SetValue("ShowPlayer", ShowPlayer ? 1 : 0);

            regKey.SetValue("MyMusicViewType", MyMusicViewType);
            regKey.SetValue("AlbumViewType", AlbumViewType);
            regKey.SetValue("DirectoryViewType", DirectoryViewType);

            regKey.SetValue("ScrobbleActive", ScrobbleActive ? 1 : 0);

            regKey.SetValue("FreeArchiveNumberSearchWithGaps", FreeArchiveNumberSearchWithGaps ? 1 : 0);

            regKey.SetValue("ShowFrequencyBand", ShowFrequencyBand ? 1 : 0);
            regKey.SetValue("ShowLyrics", ShowLyrics ? 1 : 0);

            regKey.SetValue("PartyModeAllowPlayNext", PartyModeAllowPlayNext ? 1 : 0);
            regKey.SetValue("PartyModeAllowPlayLast", PartyModeAllowPlayLast ? 1 : 0);

            regKey.SetValue("PlaylistWidth", PlaylistWidth);
            regKey.SetValue("MainTreeWidth", MainTreeWidth);

            regKey.SetValue("PartyModePlaylistPinned", PartyModePlaylistPinned ? 1 : 0);
            regKey.SetValue("PartyModeWishlistPinned", PartyModeWishlistPinned ? 1 : 0);

            regKey.SetValue("RibbonIsMinimized", RibbonIsMinimized ? 1 : 0);
            regKey.SetValue("ShowQuickAccessToolBarOnTop", ShowQuickAccessToolBarOnTop ? 1 : 0);
            regKey.SetValue("RibbonQuickAccessToolBar", RibbonQuickAccessToolBar);

            regKey.SetValue("UseMCI", UseMCI ? 1 : 0);

            regKey.SetValue("DontShowFirstSteps", DontShowFirstSteps ? 1 : 0);

            regKey.SetValue("DisableScreenSaver", DisableScreenSaver ? 1 : 0);

            regKey.SetValue("AutoSortGenres", AutoSortGenres ? 1 : 0);
            regKey.SetValue("AutoSortMediums", AutoSortMediums ? 1 : 0);

            regKey.SetValue("TimeFormat", (int)TimeFormat);

            regKey.Close();

            WriteCDArchiveConfig();
        }

        /// <summary>
        /// Speichert die Position und Größe der angegebenen Form in die Registry.
        /// </summary>
        /// <param name="form"></param>
        public static void SaveWindowPlacement(Form form, string regSubKey)
        {
            RegistryKey regKey = Registry.CurrentUser.CreateSubKey(HitbaseRegistryKey + "\\" + regSubKey);

            if (form.WindowState != FormWindowState.Minimized)
            {
                regKey.SetValue("Top", form.Top);
                regKey.SetValue("Left", form.Left);
                regKey.SetValue("Width", form.Width);
                regKey.SetValue("Height", form.Height);
                regKey.SetValue("State", (int)form.WindowState);
            }

            regKey.Close();
        }

        /// <summary>
        /// Stellt die Position und Größe der angegebenen Form wieder her.
        /// </summary>
        /// <param name="form"></param>
        public static void RestoreWindowPlacement(Form form, string regSubKey)
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(HitbaseRegistryKey + "\\" + regSubKey);

            if (regKey == null)
                return;

            form.WindowState = (FormWindowState)regKey.GetValue("State", form.WindowState);
            if (form.WindowState != FormWindowState.Maximized)
            {
                form.Top = (int)regKey.GetValue("Top", form.Top);
                form.Left = (int)regKey.GetValue("Left", form.Left);
                form.Width = (int)regKey.GetValue("Width", form.Width);
                form.Height = (int)regKey.GetValue("Height", form.Height);
            }
            form.StartPosition = FormStartPosition.Manual;

            regKey.Close();
        }

        /// <summary>
        /// Speichert die Position und Größe des angegebenen WPF Windows in die Registry.
        /// </summary>
        /// <param name="form"></param>
        public static void SaveWindowPlacement(System.Windows.Window window, string regSubKey)
        {
            RegistryKey regKey = Registry.CurrentUser.CreateSubKey(HitbaseRegistryKey + "\\" + regSubKey);

            regKey.SetValue("Top", (int)window.Top);
            regKey.SetValue("Left", (int)window.Left);
            regKey.SetValue("Width", (int)window.Width);
            regKey.SetValue("Height", (int)window.Height);
            regKey.SetValue("State", (int)window.WindowState);
        }

        /// <summary>
        /// Stellt die Position und Größe des angegebenen WPF Windows wieder her.
        /// </summary>
        /// <param name="form"></param>
        public static void RestoreWindowPlacement(System.Windows.Window window, string regSubKey)
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(HitbaseRegistryKey + "\\" + regSubKey);

            if (regKey == null)
                return;

            window.Top = (int)regKey.GetValue("Top", (int)window.Top);
            window.Left = (int)regKey.GetValue("Left", (int)window.Left);
            window.Width = (int)regKey.GetValue("Width", (int)window.Width);
            window.Height = (int)regKey.GetValue("Height", (int)window.Height);

            System.Windows.WindowState windowState = (System.Windows.WindowState)regKey.GetValue("State", System.Windows.WindowState.Normal);
            if (windowState != System.Windows.WindowState.Minimized)
                window.WindowState = windowState;

            regKey.Close();
        }

        public static void SetValue(string name, object value)
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(HitbaseRegistryKey, true);
            regKey.SetValue(name, value);
            regKey.Close();
        }

        public static object GetValue(string name, object defaultValue)
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(HitbaseRegistryKey);

            object retValue = regKey.GetValue(name, defaultValue);

            regKey.Close();

            return retValue;
        }
    }
    
    public class CDArchiveConfig
    {
        public CDArchiveConfig() 
        {
        }
	
	    public CDArchiveConfig(CDArchiveType nType, String sArchiveName, bool active, bool autoSearch, bool upload, bool autoCreateSampler, String sSamplerTrennzeichen)
	    {
    		Type = nType;
		    ArchiveName = sArchiveName;
		    Active = active;
		    AutoSearch = autoSearch;
		    Upload = upload;
		    AutoCreateSampler = autoCreateSampler;
		    SamplerTrennzeichen = sSamplerTrennzeichen;
	    }
	
        public string GetArchiveDisplayName()
        {
	        string str = "";

	        switch (Type)
	        {
	        case CDArchiveType.File:
		        str = StringTable.CDArchiveHitbaseFile;
		        break;
	        case CDArchiveType.BIG3:
		        str = StringTable.CDArchiveHitbaseInternet;
		        break;
	        case CDArchiveType.CDDBsockets:
		        str = StringTable.CDArchiveCDDB + " (Sockets)";
		        break;
	        case CDArchiveType.CDDBhttp:
		        str = StringTable.CDArchiveCDDB + " (Http)";
		        break;
	        case CDArchiveType.CDArchiveLocal:
		        str = StringTable.CDArchiveLocal;
		        break;
	        case CDArchiveType.CDArchiveLocalCDDB:
		        str = StringTable.CDArchiveLocalCDDB;
		        break;
	        }

	        return str;
        }

	    public CDArchiveType Type { get; set; }        // Type des angegebenen Archives
        public String ArchiveName { get; set; }           // Dateiname oder www-Server
        public bool Active { get; set; }                  // Aktiviert oder nicht
        public bool AutoSearch { get; set; }              // Automatisch in diesem Archive suchen
        public bool Upload { get; set; }                  // In dieses Archiv uploaden
        public bool AutoCreateSampler { get; set; }	   // Automatisch CD-Sampler erzeugen
        public String SamplerTrennzeichen { get; set; }   // Trennzeichen für automatische CD-Sampler-Erzeugung
    }

	public enum CDArchiveType
    { 
        File, 
        BIG3, 
        CDDBsockets, 
        CDDBhttp, 
        CDArchiveLocal, 
        CDArchiveLocalCDDB 
    }

    public enum TimeFormat
    {
        HHMMSS,
        MMSS
    }

    [Flags]
    public enum CDArchiveFields
    {
        None = 0,
        Category = 1,
        Medium = 2,
        Comment = 4,
        BPM = 8,
        TrackComment = 16,
        Lyrics = 32
    }

    public enum ColorStyle
    {
        Default,
        Black,
        Silver
    }
}
