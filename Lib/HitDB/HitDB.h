// HitDB.h : Defines the initialization routines for the DLL.
//

#pragma once

#ifdef _HITDB_DLL
#define HITDB_INTERFACE __declspec ( dllexport )
#else
#define HITDB_INTERFACE __declspec ( dllimport )
#endif

#define KEY_CDNAME          1
#define KEY_CDTITLE         2
#define KEY_CDSET           3
#define KEY_TOTALLENGTH     4
#define KEY_NUMBEROFTRACKS  5
#define KEY_DATE            6
#define KEY_CATEGORY        7
#define KEY_COMMENT         8
#define KEY_CDUSER1         9
#define KEY_CDUSER2         10

#define DBF_MOVEFIRST       -1     // Ersten Datensatz im Recordset lesen!
#define DBF_ALL             0      // Alle Informationen aller Tabellen lesen
#define DBF_CUSTOMSQL       1      // Benutzerdefinierter SQL-Befehl 
#define DBF_CD              2      // Nur notwendige Spalten aus CD-Tabelle lesen
#define DBF_CD_ALL          3      // Alle Spalten aus CD-Tabelle lesen
#define DBF_TRACK           4      // (Fast) nur Lied-Tabelle lesen
#define DBF_LOANCD			5      // Nur die ausgeliehenen CDs ausdrucken

// Die einzelnen Datums-Formate
#define DATE_TTMMJJJJ 0
#define DATE_MMJJJJ   1
#define DATE_JJJJ     2
#define DATE_NONE     3

// Die möglichen Datenbankabfragen
#define QUERY_UNKNOWN -1         // Unbekannt. Kein Query ausgeführt
#define QUERY_ARTIST   1         // Query auf Artisten-Tabelle ausgeführt
#define QUERY_CD       2         // Query auf CD-Tabelle ausgeführt
#define QUERY_TRACK    3         // Query auf Track-Tabelle ausgeführt

// Möglichen Sort-Gruppen Flags
#define SGF_ARTIST     1         // Sort-Felder für Artisten-Tabelle zurückliefern.
#define SGF_CD         2         // Sort-Felder für CD-Tabelle zurückliefern.
#define SGF_TRACK      4         // Sort-Felder für Lied-Tabelle zurückliefern.
#define SGF_ALL        SGF_ARTIST|SGF_CD|SGF_TRACK  // Sort-Felder für alle Tabellen zurückliefern.

// Anzahl der benutzerdefinierten Felder
#define MAX_USER_FIELDS 5

#define MAX_CODES                26
#define MAX_CATEGORIES           200
#define MAX_MEDIUM               100

///////////////////////////////////////////////////////////////////////////////

#include "resource.h"
#include "database.h"
#include "Artist.h"
#include "CD.h"
#include "CDQuery.h"
#include "CDSet.h"
#include "Track.h"
#include "Identity.h"
#include "Category.h"
#include "Medium.h"
#include "Program.h"
#include "Index.h"
#include "Master.h"
#include "Queue.h"
#include "VerlieheneCDs.h"
#include "Role.h"
#include "Participant.h"
#include "CDQuery.h"

#include "Selection.h"
#include "DataBaseProgressDlg.h"

