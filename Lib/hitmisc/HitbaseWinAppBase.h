#pragma once

///////////////////////////////////////////////////////////////////////////
// Diese #defines sind für die ganze Applikation (also auch alle DLLs)

#define REGISTRY_KEY L"SOFTWARE\\Big 3\\Hitbase 2012"

#define HIT_VERSION L"1.0"
#define ApplicationName L"Hitbase 2012"

// Beta-spezifische Defines
#if ENGLISH
#define BETAVERSION TRUE        // Bei Release auf FALSE setzen!
#define BETAVERSION_EXPIRES_YEAR  2003
#define BETAVERSION_EXPIRES_MONTH 6
#define BETAVERSION_EXPIRES_DAY   30
#else
#define BETAVERSION FALSE        // Bei Release auf FALSE setzen!
#define BETAVERSION_EXPIRES_YEAR  2009
#define BETAVERSION_EXPIRES_MONTH 12
#define BETAVERSION_EXPIRES_DAY   31
#endif

#define LICENSED_VERSION  FALSE   // z.b. für BHV
#define LICENSE_BHV		  FALSE   // BHV

///////////////////////////////////////////////////////////////////////////
// Lautstärke-Einstellungen
#define MASTER_VOLUME 10           // Muß auf jeden Fall größer sein, als die AUXCAPS_...
#define VOLUME_WAVE   11           // Muß auf jeden Fall größer sein, als die AUXCAPS_...

///////////////////////////////////////////////////////////////////////////
// Datenbank-Felder
#define FIELD_TOTALLENGTH           1
#define FIELD_NUMBEROFTRACKS        2
#define FIELD_CDSET                 3
#define FIELD_CDSAMPLER             4
#define FIELD_CDNAME                5
#define FIELD_CDTITLE               6
#define FIELD_CATEGORY              7
#define FIELD_DATE                  8
#define FIELD_CODES                 9
#define FIELD_CDCOMMENT             10
#define FIELD_ARCHIVNUMMER          11
#define FIELD_MEDIUM                12
#define FIELD_YEAR_RECORDED         13     // JUS 17.01.2002
#define FIELD_COPYRIGHT             14     // JUS 17.01.2002
#define FIELD_CDCOVER_FILENAME      15     // JUS 02.09.2002
#define FIELD_CDCOVERBACK_FILENAME  16     // JUS 29.08.2003
#define FIELD_CDCOVERLABEL_FILENAME 17     // JUS 29.08.2003
#define FIELD_ORIGINAL_CD           18     // JUS 29.08.2003
#define FIELD_LABEL                 19     // JUS 29.08.2003
#define FIELD_UPC                   20     // JUS 29.08.2003
#define FIELD_URL                   21     // JUS 29.08.2003
#define FIELD_RATING                22     // JUS 29.08.2003
#define FIELD_PRICE                 23     // JUS 29.08.2003
#define FIELD_LANGUAGE              24     // JUS 14.12.2005
#define FIELD_LOCATION              25     // JUS 14.12.2005

// Die User-Felder MÜSSEN in einem Block zusammen bleiben!
#define FIELD_CDUSER1          50
#define FIELD_CDUSER2          51
#define FIELD_CDUSER3          52
#define FIELD_CDUSER4          53
#define FIELD_CDUSER5          54

#define FIELD_LAST_CD_FIELD    99     // Obere Schranke für CD-Felder

#define FIELD_TRACK_ARTIST     100
#define FIELD_TRACK_TITLE      101
#define FIELD_TRACK_LENGTH     102
#define FIELD_TRACK_NUMBER     103
#define FIELD_TRACK_BPM        104
#define FIELD_TRACK_CODES      105
#define FIELD_TRACK_COMMENT    106
//#define FIELD_TRACK_CDTITLE    107
//#define FIELD_TRACK_ARCHIVNR   108
#define FIELD_TRACK_LYRICS     109
#define FIELD_TRACK_SOUNDFILE  110
//#define FIELD_TRACK_FILENAME   111       // Für Virtuelle CD..... 09.08.2003: Das gleiche wie FIELD_TRACK_SOUNDFILE
#define FIELD_TRACK_YEAR_RECORDED 112    // JUS 17.01.2002
#define FIELD_TRACK_RATING        113    // JUS 29.08.2002
#define FIELD_TRACK_CHECKSUM      114    // JUS 29.08.2002
#define FIELD_TRACK_CATEGORY      115    // JUS 20.12.2005
#define FIELD_TRACK_LANGUAGE      116    // JUS 20.12.2005

// Die User-Felder MÜSSEN in einem Block zusammen bleiben!
#define FIELD_TRACK_USER1      150
#define FIELD_TRACK_USER2      151
#define FIELD_TRACK_USER3      152
#define FIELD_TRACK_USER4      153
#define FIELD_TRACK_USER5      154

#define FIELD_LAST_TRACK_FIELD 199     // Obere Schranke für Track-Felder

#define FIELD_ARTIST_NAME             200
#define FIELD_ARTIST_CD_SORTKEY       201
#define FIELD_ARTIST_CD_GROUPTYPE     202
#define FIELD_ARTIST_CD_SEX           203
#define FIELD_ARTIST_CD_COMMENT       204
#define FIELD_ARTIST_CD_URL           205
#define FIELD_ARTIST_CD_COUNTRY       206
#define FIELD_ARTIST_CD_BIRTHDAY      207
#define FIELD_ARTIST_CD_DAYOFDEATH    208
#define FIELD_ARTIST_CD_IMAGEFILENAME 209

#define FIELD_LAST_ARTIST_CD_FIELD    249     // Obere Schranke für Artist-Felder

#define FIELD_ARTIST_TRACK_NAME          250
#define FIELD_ARTIST_TRACK_SORTKEY       251
#define FIELD_ARTIST_TRACK_GROUPTYPE     252
#define FIELD_ARTIST_TRACK_SEX           253
#define FIELD_ARTIST_TRACK_COMMENT       254
#define FIELD_ARTIST_TRACK_URL           255
#define FIELD_ARTIST_TRACK_COUNTRY       256
#define FIELD_ARTIST_TRACK_BIRTHDAY      257
#define FIELD_ARTIST_TRACK_DAYOFDEATH    258
#define FIELD_ARTIST_TRACK_IMAGEFILENAME 259

#define FIELD_LAST_ARTIST_TRACK_FIELD 299     // Obere Schranke für Artist-Felder

#define FIELD_LAST_ARTIST_FIELD       299     // Obere Schranke für Artist-Felder

#define FIELD_COMPOSER_CD_NAME          300
#define FIELD_COMPOSER_CD_SORTKEY       301
#define FIELD_COMPOSER_CD_GROUPTYPE     302
#define FIELD_COMPOSER_CD_SEX           303
#define FIELD_COMPOSER_CD_COMMENT       304
#define FIELD_COMPOSER_CD_URL           305
#define FIELD_COMPOSER_CD_COUNTRY       306
#define FIELD_COMPOSER_CD_BIRTHDAY      307
#define FIELD_COMPOSER_CD_DAYOFDEATH    308
#define FIELD_COMPOSER_CD_IMAGEFILENAME 309

#define FIELD_LAST_COMPOSER_CD_FIELD    349     // Obere Schranke für CD-Composer-Felder

#define FIELD_COMPOSER_TRACK_NAME          350
#define FIELD_COMPOSER_TRACK_SORTKEY       351
#define FIELD_COMPOSER_TRACK_GROUPTYPE     352
#define FIELD_COMPOSER_TRACK_SEX           353
#define FIELD_COMPOSER_TRACK_COMMENT       354
#define FIELD_COMPOSER_TRACK_URL           355
#define FIELD_COMPOSER_TRACK_COUNTRY       356
#define FIELD_COMPOSER_TRACK_BIRTHDAY      357
#define FIELD_COMPOSER_TRACK_DAYOFDEATH    358
#define FIELD_COMPOSER_TRACK_IMAGEFILENAME 359

#define FIELD_LAST_COMPOSER_TRACK_FIELD    399     // Obere Schranke für Track-Composer-Felder


// Die einzelnen Feld-Formate (User-defined Field Formats)
#define UDFF_TEXT     0
#define UDFF_NUMBER   1
#define UDFF_BOOL     2
#define UDFF_CURRENCY 3
#define UDFF_DATE     4

#include "hitmisc.h"
#include "config.h"
#include "afxwinappex.h"

// CHitbaseWinAppBase

class CPlugInManager;

class HITMISC_INTERFACE CHitbaseWinAppBase : public CWinAppEx
{
	DECLARE_DYNCREATE(CHitbaseWinAppBase)

protected:
	CHitbaseWinAppBase();           // protected constructor used by dynamic creation
	virtual ~CHitbaseWinAppBase();

public:
	virtual BOOL InitInstance();
	virtual int ExitInstance();

	CString m_sExecutablePath;      // In diesem Pfad befindet sich die Hitbase.exe
	int m_iCommandLineCDDevice;		// CD-Laufwerk über die Kommandozeile angegeben

	CConfig m_config;				// Komplette Konfiguration von Hitbase

	CPlugInManager* m_pPlugInManager;

	int m_iDebug;					// Debug-Modus

protected:
	DECLARE_MESSAGE_MAP()
};


