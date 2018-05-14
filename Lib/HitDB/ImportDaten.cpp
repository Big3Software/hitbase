/*
 * import 4.0 files into Hitline
 * import XMCD files into Hitline
 */

#include "stdafx.h"
#include <io.h>
#include "HitDB.h"
#include "..\..\app\hitbase\resource.h"

int ConvertUser = 0;

int CALLBACK ImportOpt(HWND hDlg, UINT message, WPARAM uParam, LPARAM lParam);

long get_frames_xmcd (char *line);
long convert_frames2ms (long frames);
long get_disc_length (char *line);
void make_track_length (CCD* pCD);
void remove_special_chars (CString *line);
long get_length_of_cd_data (char *buf); // Lesen TAR-Header - Länge der Daten

//#define MAX_CATEGORIES      50

//#define FILE_VERSION       400                  /* Version der Daten-Datei */
#define FILE_PROG_VERSION  300        /* Ab dieser Version Programme laden */
#define FILE_INDEX_VERSION 400       /* Ab dieser Version Inzizes... laden */

#define IMPORT_HITLINE 9501
#define IMPORT_XMCD    9502
#define XMCD_HEADER_SIZE 512

/*static sfint keys[] = { 
            2,  20, tSTRING|tDUPSOK,
           22,  18, tBINARY|tDUPSOK,
          145, 202, tUDEFINE|tDUPSOK,
           42, 101, tUDEFINE|tDUPSOK,               // Name des CD-Sets
           EOK };*/

#define BUFFER_SIZE 30000

#pragma pack (1)

struct tagDATEN
       {
       short version;               /* 000 Version */
       long ges_laenge;             /* 002 Länge der CD */
       short anzahl_lieder;         /* 006 Anzahl der Lieder auf der CD */
       long laenge_lied[3];         /* 008 Laenge der einzelnen Lieder (ms) */
       char artist[51];             /* 020 Gruppe oder Sänger/in */
       char cdname[51];             /* 071 Name der CD */
       char recname[10];            /* 122 Dateiname der Record-Dateien */
                         /*     z.b. a\b023456 */
       char category[21];           /* 132 Kategorie (z.B. Klassik, Pop, ...) */
       char date[9];                /* 153 Kaufdatum (JJJJMMTT) */
       char mark[6];                /* 162 Kennzeichen */
       };

#pragma pack ()

#pragma pack (2)

struct mRec {
    long rp;                 /* record pointer                       */
    unsigned short rLen;       /* length of data record                */
};

struct lli {
    long rp;              /* record number of locked record               */
    unsigned short procId;          /* process that owns this record                */
};

typedef struct {
    /*
     *  this top structure contains all the values that
     *  must be passed between the various processes.
     */
    struct vlH {
        long version;             /* software version number              */
        long vnumRecs;            /* number of records in mapper          */
        long delPtr;              /* first deleted mapper entry           */
        long mpDCount;            /* number of deleted mapper entries     */
        long lastByte;            /* last byte in use in the store        */
        long stDCount;            /* number of deleted bytes in the store */
        short okFlag;               /* file in use, corruption detection    */
        long sync;                /* synchronization value                */
        unsigned short minRecl;             /* smallest record length allowed       */
        unsigned short maxRecl;             /* largest record allowed               */
        unsigned short gran;                /* granularity (page size?)             */
        long mapOffset;           /* base of mapper entries               */
        unsigned short stash;               /* stash size                           */
        long dupNum;              /* ascending duplicate counter          */
        /*
         *  multi user stuff
         */
        unsigned short stashSerial;         /* serialization counter for the stash  */
        short activeProc;           /* # of processes accessing this vfile  */
        unsigned short exclusive;           /* process ID of exclusive owner        */
        short lockCount;            /* locked record count                  */
        struct lli lockList[30]; /* list of locked record pointers  */
    } v;

    /*
     *  this stuff is local to any particular process
     */
    char *check;                /* used only for runtime fd validation  */
    short locked;               /* locked by us? (boolean)              */
    short mp;                /* mapper file file descriptor          */
    short st;                /* store file file descriptor           */
    unsigned short mode;                /* file mode values                     */
} VF;
#pragma pack ()


/*
 * src = pfad/dateiname der zu konvertierenden datei
 * des = pfad/dateiname der neu zu erstellenden Datei
 */
int CDataBase::convert_xmcd (const CString& src, CProgressCtrl& ctlProgress, CStatic& stcStatus)
{
	FILE *fh;
	wchar_t data_file[500];
	char line[512];
	int ret;
	int title_num, exttitle_num, max_title_num;
	int count = 0;
//	char title[10], exttitle[10];
	int line_pos;
    long filelen;
	int track_nr;
	long track_length;
	CString szTitelInterpret;
	CString titel_interpret;
	CString szKategorie;
	int trenn_pos;
	char buf[1000];
	int first_remark;
	CStringArray DiscIDs;
	long fpos;
	CString str, str2;
	CString str1;
	long cd_length, cd_start_pos, cd_end_pos;
	double fFaktor;
	unsigned long max_range;
	CString mu;

	int insert_count = 0;

	m_StopSearchProcess = FALSE;

	wcscpy (data_file, src);

	// nun... - fopen und fgets iss besser...
	fh = _wfopen(src, L"rb");

	if (fh == NULL)
	{
		return (-1);
	}

	filelen = _filelength(_fileno(fh));
	fFaktor = (double)0xffff / (double)(filelen - 1);

	max_range = (unsigned long)((double)filelen * fFaktor);
	if (max_range > 65535L)
		max_range = 65535L;

//	CDataBaseProgressDlg DataBaseProgressDlg(this);

	str.LoadString(IDS_IMPORTXMCD);
	
//	DataBaseProgressDlg.Create(IDD_HITDB_DATABASE_PROGRESS);
//	DataBaseProgressDlg.SetWindowText(get_string(IDS_IMPORTDATA));

	ctlProgress.SetRange(1, (unsigned short)max_range);
//!!!!!!!!!!!!!!!	SetProgressDlg (&DataBaseProgressDlg);
	m_bBackground = TRUE;

	while (1)
	{
		// Aus Performance-Gründen nur jedes 10. mal
		if (!(insert_count % 10))
		{
			CheckAbortProc ();

			if (m_StopSearchProcess == TRUE)
				break;
		}

		titel_interpret.Empty();
		max_title_num = 0;
		cd_start_pos = 0;
		cd_end_pos = 0;

		fpos = ftell(fh);
		max_range = (unsigned long)((double)fpos * fFaktor);
		if (max_range > 65535L)
			max_range = 65535L;
		ctlProgress.SetPos((unsigned short)max_range);
      	str1.Format(str, fpos, filelen);
		stcStatus.SetWindowText(str1);
//      	DataBaseProgressDlg.UpdateData(FALSE);

		count++;
		// Now read 512 bytes header
		ret = fread (buf, 1, 512, fh);

        if (ret != 512 || feof (fh) != 0)
			break;

		cd_length = get_length_of_cd_data (buf);

		szKategorie = strtok (buf, "/");
		mu = szKategorie.Left(1);
		mu.MakeUpper();
		
		if (szKategorie.GetLength() > 0)
		{
			if (mu[0] > 32)
				szKategorie.SetAt (0, mu[0]);
		}

		if (szKategorie.GetLength() > 0)
		{
			if (GetIDFromCategory (szKategorie) == 0)
			{
				AddCategory(szKategorie);
			}
		}

		CCD CD(this);
		CD.GetCategory()->m_sCategory = szKategorie;

		CD.m_wNumberOfTracks = 100;    // erst mal auf den Maximumwert setzen. Tatsächliche Anzahl kommt später.

		first_remark = FALSE;

		cd_start_pos = ftell(fh);

		while (1) // Schleife pro CD
		{
			// mal sehen ob hier auch echte Daten kommen...?
			// muß mit '#' anfangen
			ret = fread (buf, 1, 1, fh);
			if (ret == 1)
				ungetc (buf[0], fh);

			if (buf[0] == 0)
				break;

			if (first_remark == FALSE && buf[0] != '#')
			{
				break;
			}

			first_remark = TRUE;

			// scheint OK - Nun her damit
			if (fgets (line, 512, fh) == NULL)
				break;

			if (line[0] == '\0')
				break;

			 // skip remark lines exept for track frame offsets
			if (strncmp (line, "# Track frame offsets:", 22) == 0)
			{
				// Nun Track-Längen einlesen
				track_nr = 0;
				while (1)
				{
					if (fgets (line, 512, fh) == NULL)
						break;

					if ((track_length = get_frames_xmcd (line)) > 0)
					{
						//track_length = convert_frames2ms (track_length);
						CD.GetTrack(track_nr)->m_dwLength = track_length;
					}
					else
						break;

					track_nr++;
				}

				continue;
			}

			if (strncmp (line, "# Disc length:", 14) == 0)
			{
				CD.m_dwTotalLength = get_disc_length (&line[14]);
				continue;
			}

			if (line[strlen(line) - 1] == '\n')
				line[strlen(line) - 1] = '\0';

			// Get Disc-ID from xmcd
			if (strncmp (line, "DISCID=", 6) == 0)
			{
				GetDiscIDXmcd (DiscIDs, &line[7]);

				continue;
			}

			// now read cd-title and author - split later
			if (strncmp (line, "DTITLE=", 7) == 0)
			{
				if (titel_interpret.GetLength() == 0)
 					titel_interpret = &line[7];
				else
					titel_interpret += &line[7];

				continue;
			}

			if (strncmp (line, "TTITLE", 6) == 0)
			{
				if (line[strlen(line) - 1] == '\n')
					line[strlen(line) - 1] = '\0';

				title_num = atoi(&line[6]);

				// Bei der XMCD Qualität weiß man ja nie...
				if (title_num > max_title_num)
					max_title_num = title_num;

				if (title_num < 0)
				{
					// Track zu hoch - überspringen des Tracks
					continue;
				}

				line_pos = 8;
				if (title_num > 9)
					line_pos++;

				if (title_num > 99)
					line_pos++;

				CD.GetTrack(title_num)->m_wTrack = title_num + 1;

				if (CD.GetTrack(title_num)->m_sTitle.GetLength() == 0)
				    CD.GetTrack(title_num)->m_sTitle = &line[line_pos];
				else
					CD.GetTrack(title_num)->m_sTitle += &line[line_pos];
				
				if (CD.GetTrack(title_num)->m_sTitle.GetLength() > 99)
                    CD.GetTrack(title_num)->m_sTitle = CD.GetTrack(title_num)->m_sTitle.Left(99);

				continue;
			}
			if (strncmp (line, "EXTD", 4) == 0)
			{
				if (line[strlen(line) - 1] == '\n')
					line[strlen(line) - 1] = '\0';

				if (CD.m_sComment.GetLength() == 0)
				    CD.m_sComment = &line[5];
				else
					CD.m_sComment += &line[5];
				
				if (CD.m_sComment.GetLength() > 255)
                    CD.m_sComment = CD.m_sComment.Left(255);
				remove_special_chars (&CD.m_sComment);

				continue;
			}
			if (strncmp (line, "EXTT", 4) == 0)
			{
				if (line[strlen(line) - 1] == '\n')
					line[strlen(line) - 1] = '\0';

				exttitle_num = atoi(&line[4]);
				
				if (exttitle_num < 0)
				{
					// Track zu hoch - überspringen des Tracks
					continue;
				}

				line_pos = 6;
				if (exttitle_num > 9)
					line_pos++;
				if (exttitle_num > 99)
					line_pos++;

				if (CD.GetTrack(exttitle_num)->m_sComment.GetLength() == 0)
				    CD.GetTrack(exttitle_num)->m_sComment = &line[line_pos];
				else
					CD.GetTrack(exttitle_num)->m_sComment += &line[line_pos];
				
				if (CD.GetTrack(exttitle_num)->m_sComment.GetLength() > 255)
                    CD.GetTrack(exttitle_num)->m_sComment = CD.GetTrack(exttitle_num)->m_sComment.Left(255);

				remove_special_chars (&CD.GetTrack(exttitle_num)->m_sComment);

				continue;
			}
			// Lesen und wegschmeissen
			if (strncmp (line, "PLAYORDER", 9) == 0)
			{
				// und tschüss
				break;
			}
		}  // while (1) pro CD

		if (first_remark == FALSE)
			continue;

		// max_title_num war 0 basierend
		CD.m_wNumberOfTracks = max_title_num + 1;

		// Zuerst mal Titel der CD un Interpret aus DTITEL ermitteln
		trenn_pos = titel_interpret.Find(L"/");

		if (trenn_pos > 0)
		{
			CD.m_sTitle = titel_interpret.Mid(trenn_pos + 1);
			CD.GetArtist()->m_sArtist = titel_interpret.Left(trenn_pos);
		}
		else
		{
			CD.m_sTitle = titel_interpret;
			CD.GetArtist()->m_sArtist = titel_interpret;
		}

		// BASIC war ja nicht schlecht... :-)
		CD.m_sTitle.TrimLeft();
		CD.m_sTitle.TrimRight();
		CD.GetArtist()->m_sArtist.TrimLeft();
		CD.GetArtist()->m_sArtist.TrimRight();

		if (CD.GetArtist()->m_sArtist.GetLength () > 99)
			CD.GetArtist()->m_sArtist = CD.GetArtist()->m_sArtist.Left(99);
		if (CD.m_sTitle.GetLength () > 99)
			CD.m_sTitle = CD.m_sTitle.Left(99);

		if (CD.GetArtist()->m_sArtist.GetLength() == 0)
			CD.GetArtist()->m_sArtist = CD.m_sTitle;

		make_track_length (&CD);
		CD.m_dwTotalLength = CD.m_dwTotalLength * 1000L;
		
		if (DiscIDs.GetSize() > 0)
			CD.GetIdentity()->m_sIdentity = DiscIDs[0];

		// Nun ab in die HDB
		BOOL Added=FALSE;

		if (CD.Add () != TRUE)
		{
/*#ifdef _DEBUG
			{
				FILE *fht;
				char buferr[1000];
				fht = fopen("c:\\xmcd.err", "a");

				sprintf (buferr, "DiscID: '%s' Title: '%s' Artist: '%s' Count: %d\n", cdinfo.m_szIdentity, cdinfo.m_szCDTitle, cdinfo.m_szCDArtist, insert_count);
				fputs (buferr, fht);
				fclose (fht);
			}
#endif*/
		}
		else
		{
			for (int i=1;i<DiscIDs.GetSize();i++)
				WriteIdentity(DiscIDs[i], CD.m_dwID);
		}

		if (Added == FALSE)
		{
#ifdef _DEBUG
/*			{
				FILE *fht;
				char buferr[1000];
				fht = fopen("c:\\xmcdupdate.err", "a");

				sprintf (buferr, "DiscID: '%s' Title: '%s' Artist: '%s' Count: %d\n", cdinfo.m_szIdentity, cdinfo.m_szCDTitle, cdinfo.m_szCDArtist, insert_count);
				fputs (buferr, fht);
				fclose (fht);
			}*/
#endif
		}

		insert_count++;

//        str2.Format("CD# %d", insert_count);
//        SetStatusText(str2);

		cd_end_pos = ftell(fh);

		if (cd_length != cd_end_pos - cd_start_pos)
		{
			AfxMessageBox (L"Fatal error XMCD Import - Illegal TAR format!");
			break;
		}
		
		// Nun noch ein paar Bytes am Ende überlesen...
		if (cd_end_pos % 512 != 0)
		{
			cd_end_pos = 512 - (cd_end_pos % 512);

			fread (buf, 1, cd_end_pos, fh);
		}

		if (feof (fh) != 0)
			break;
	} // while (1) Kompletter Katalog

//	SetStatusText("");
	fclose (fh);
	
//	DataBaseProgressDlg.DestroyWindow();

	return (0);
}

/*
 * Microsoft CD-Spieler ini-Datei
 * src = pfad/dateiname der zu konvertierenden datei
 * des = pfad/dateiname der neu zu erstellenden Datei
 */
int CDataBase::convert_mscd (const CString& src, CProgressCtrl& ctlProgress, CStatic& stcStatus)
{
	FILE *fh;
	char *p;
	char aufb[1000];
	char line[1024];
	int ch;
	int count = 0;
	char cd_title[100][100];
	char cd_key[20];
	int order[100];
	int numplay;
	double fFaktor;
	long filelen, fpos;
	CString str;
	CString str1;
	unsigned long max_range;

	m_StopSearchProcess = FALSE;

	// Zuerst mal schauen ob CDPLAYER Standard oder Deluxe
	CString sFileName;

	sFileName = src;
	if (sFileName.GetLength() < 5)
	{
		return (-1);
	}
	if (sFileName.Right(4) == ".mdb")
	{
		// Deluxe player Access Datenbank
		return (convert_mscddeluxe (sFileName, ctlProgress, stcStatus));
	}
	// nun... - fopen und fgets iss besser... :-) nee! einfacher
	fh = _wfopen(src, L"r");

	if (fh == NULL)
	{
		return (-1);
	}

	filelen = _filelength(_fileno(fh));
	fFaktor = (double)0xffff / (double)(filelen - 1);

	max_range = (unsigned long)((double)filelen * fFaktor);
	if (max_range > 65535L)
		max_range = 65535L;

	CDataBaseProgressDlg DataBaseProgressDlg;

	str.LoadString(IDS_IMPORTXMCD);
	
//	DataBaseProgressDlg.Create(IDD_HITDB_DATABASE_PROGRESS);
//	DataBaseProgressDlg.SetWindowText(get_string(IDS_IMPORTDATA));

	ctlProgress.SetRange(1, (unsigned short)max_range);
//!!!!!!!!!!!!!!!	SetProgressDlg (&DataBaseProgressDlg);

	memset (line, 0, 1024);
	// los geht's - read data from favorite :-) ms-cdplayer
	while (1)
	{
		CCD CD(this);

		CheckAbortProc ();

		if (m_StopSearchProcess == TRUE)
			break;

		fpos = ftell(fh);
		max_range = (unsigned long)((double)fpos * fFaktor);
		if (max_range > 65535)
			max_range = 65535;

		ctlProgress.SetPos((unsigned short)max_range);
		str1.Format(str, fpos, filelen);
//		DataBaseProgressDlg.UpdateData(FALSE);
		stcStatus.SetWindowText(str1);

		// check for new entry
		if (line[0] == '[')
		{
		    // got it! read new entry
			numplay = 0;

			strcpy (aufb, &line[1]);
			aufb[strlen(aufb) - 1] = '\0';
			// MS speichert den Kram Hexadezimal. Jetzt noch umwandeln
			sprintf (cd_key, "%lu", strtoul(aufb, (char **)0, 16));
			
			CD.GetIdentity()->m_sIdentity = cd_key;

	        memset (cd_title, 0, 10000);
			memset (order, 0, 100 * sizeof(int));

			while (fgets (line, 512, fh) != NULL)
			{
				// wenn die zeile zu lang ist, weg mit dem rest...
				if (line[strlen(line) - 1] != 0x0a)
				{
					while (1)
					{
						ch = fgetc(fh);
						if (ch == EOF || ch == 0x0a || ch == 0x0d)
							break;
					}
					if (ch == 0x0d)
						fgetc (fh);
				}

		        // schrott weg...
				if (strlen (line) > 0)
				{
					if (line[strlen(line) - 1] == 0x0a)
						line[strlen(line) - 1] = '\0';
				}

				if (strlen (line) == 0 || line[0] == '[')
					break;

			    if (_strnicmp (line, "EntryType", 9) == 0)
			    {
  				    // weg damit :-(
					continue;
			    }
				
			    if (_strnicmp (line, "artist", 6) == 0)
			    {
					if (strlen (line) > 7)
						CD.GetArtist()->m_sArtist = &line[7];
			    }

			    if (_strnicmp (line, "title", 5) == 0)
			    {
					if (strlen (line) > 6)
						CD.m_sTitle = &line[6];
			    }

			    if (_strnicmp (line, "numtracks", 9) == 0)
			    {
					if (strlen (line) > 10)
					{
				        strcpy (aufb, &line[10]);
						CD.m_wNumberOfTracks = atoi(aufb);
					}
			    }

			    // Titel-Namen 0-x
				if (line[0] >= 48 && line[0] <=57)
			    {
					if (strlen(line) > 2)
					{
						if (line[1] == '=' || line[2] == '=')
						{
							count = atoi (line);
							p = strchr (line, '=');
							if (p != NULL)
							{
								p++;
								if (*p != '\0')
								{
									CD.GetTrack(count)->m_sTitle = p;
								}
							}
						}
					}
			    }

/*			    if (strnicmp (line, "order", 5) == 0)
			    {
					if (strlen (line) > 6)
					{
				        strcpy (aufb, &line[6]);

						count = 0;
						p = strtok (aufb, " ");
						while (p != NULL)
						{
							cdinfo.m_Program[count] = atoi(p);
							p = strtok(NULL, " ");
							count++;
							if (count > MAX_PROGRAMS)
								break;
						}
					}
			    }

			    if (strnicmp (line, "numplay", 7) == 0)
			    {
					if (strlen (line) > 8)
					{
				        strcpy (aufb, &line[8]);
						numplay = atoi(aufb);
						if (numplay > 100)
							numplay = 100;
					}
			    }
				*/
			}

			// Nun ab in die HDB
			if (CD.Add () != TRUE)
			{
				AfxMessageBox (L"Fehler Add");
			}

			if (line[0] == '[')
				continue;
		}

		// read next line
		if (fgets (line, 512, fh) == NULL)
  			break;

        // wenn die zeile zu lang ist, weg mit dem rest...
		if (line[strlen(line) - 1] != 0x0a)
		{
			while (1)
			{
				ch = fgetc(fh);
				if (ch == EOF || ch == 0x0a || ch == 0x0d)
					break;
			}
			if (ch == 0x0d)
				fgetc (fh);
		}

		// schrott weg...
		if (strlen (line) > 0)
		{
			if (line[strlen(line) - 1] == 0x0a)
  				line[strlen(line) - 1] = '\0';
		}
	}

//	DataBaseProgressDlg.DestroyWindow();
	fclose (fh);

	return (0);
}

// Ermiteln der frames aus Zeile der
long get_frames_xmcd (char *line)
{
	char frames[50];
	int pos, dest_pos;;

	if (strlen (line) < 3)            // 21-May-97 : geändert
//	if (strlen (line) < 3 || *(line + 1) != 9)
		return (-1);

	memset (frames, 0, 50);

	pos = 0;
	while (!isdigit (*(line + pos)) && *(line + pos))
		pos++;
	dest_pos = 0;
	if (!line[pos])
		return -1;

	while (isdigit (*(line + pos)))
	{
		frames[dest_pos] = *(line + pos);
		dest_pos++;
		pos++;
	}

	return (atol (frames));
}

// konvertierung von frames in ms - 75 frames pro Sekunde
long convert_frames2ms (long frames)
{
	return (MulDiv(frames, 1000, 75));
}

// Ermiteln der CD-Länge aus Kommentar Zeile
long get_disc_length (char *line)
{
	char disc_length[50];
	int pos, dest_pos;;

	if (strlen (line) < 2)
		return (-1);

	memset (disc_length, 0, 50);
	pos = 1;
	dest_pos = 0;

	while (*(line + pos) != '\n' && *(line + pos) != 0)
	{
		if (isdigit (*(line + pos)))
		{
			disc_length[dest_pos] = *(line + pos);
			dest_pos++;
		}
		pos++;
	}

	return (atol (disc_length));
}

// read XMCD disc ID
void CDataBase::GetDiscIDXmcd (CStringArray& DiscIDs, const CString& pXmcdDiscID)
{
	wchar_t *p, copyXmcdID[1000];

	DiscIDs.RemoveAll();

	wcscpy(copyXmcdID, pXmcdDiscID);

	p = wcstok(copyXmcdID, L", ");
	while (p)
	{
		DiscIDs.Add((CString)L"XMCD" + p);

		p = wcstok(NULL, L", ");
	}
}

// Konvertieren der start-frames zu Lied-Längen
void make_track_length (CCD* pCD)
{
	int count;

	if (pCD->m_wNumberOfTracks < 1)
		return;

	// Aus frame-Startpositionen Liedlängen ermitteln - convert_frames2ms (long frames)
	for (count = 0; count < pCD->m_wNumberOfTracks - 1; count++)
	{
		pCD->GetTrack(count)->m_dwLength = convert_frames2ms (pCD->GetTrack(count + 1)->m_dwLength - pCD->GetTrack(count)->m_dwLength);
	}
	// Nun letzten Track auf korrekten Wert setzen
	pCD->GetTrack(pCD->m_wNumberOfTracks - 1)->m_dwLength = convert_frames2ms ((pCD->m_dwTotalLength * 75L) - pCD->GetTrack(pCD->m_wNumberOfTracks - 1)->m_dwLength);

	return;
}

// Entfernen von Sonderzeichen 
void remove_special_chars (CString* line)
{
	int pos;

	while ((pos = line->Find(L"\\n")) != -1)
	{
		//CString str;
		*line = line->Left(pos) + "; " + line->Mid(pos + 2);
		//*line = str;
	}
	while ((pos = line->Find(L"\\r")) != -1)
	{
		*line = line->Left(pos) + "; " + line->Mid(pos + 2);
	}
	while ((pos = line->Find(L"\\t")) != -1)
	{
		*line = line->Left(pos) + " " + line->Mid(pos + 2);
	}
	while ((pos = line->Find(L";;")) != -1)
	{
		*line = line->Left(pos) + ";" + line->Mid(pos + 2);
	}
}

// Aus TAR-Header Länge der nächsten Datei ermitteln
long get_length_of_cd_data (char *buf)
{
	char buf1[20];
	char *end;

	memset (buf1, 0, 20);

	// An Pos 0x7C-86 steht in octal die Länge
	for (int sc = 0x7c; sc <= 0x86; sc ++)
	{
		buf1[sc - 0x7c] = *(buf + sc);
	}

	return (strtoul(buf1, &end, 8));
}

int CALLBACK ImportOpt(HWND hDlg, UINT message, WPARAM uParam, LPARAM lParam)
{
        switch (message) {
                case WM_INITDIALOG:
					{
                        CheckRadioButton (hDlg, IDC_HITDB_KONV1, IDC_HITDB_KONV3, IDC_HITDB_KONV1);
                        return (TRUE);
					}

	            case WM_COMMAND:
                    if (LOWORD(uParam) == IDOK || LOWORD(uParam) == IDCANCEL)
					{
					    ConvertUser = 0;

                        if (IsDlgButtonChecked (hDlg, IDC_HITDB_KONV2))
						{
							ConvertUser = 1;
						}
                        if (IsDlgButtonChecked (hDlg, IDC_HITDB_KONV3))
						{
							ConvertUser = 2;
						}

					    EndDialog(hDlg, TRUE);
                        return (TRUE);
                    }

                    break;
        }
        return (FALSE); // Didn't process the message

        lParam; // This will prevent 'unused formal parameter' warnings
}

// konvertieren von Microsoft Deluxe CD-Player Daten
int CDataBase::convert_mscddeluxe (CString sFileName, CProgressCtrl& ctlProgress, CStatic& stcStatus)
{
//	CDataBaseProgressDlg DataBaseProgressDlg;
	CDaoDatabase db;
	unsigned long nRecordCount;
	int nCurrentRecordNumber;
	double dfMultiplikator;
	unsigned short nCurrentPos;
	COleVariant var;
	CString sBuf;
	CString sTrackLength;
	int nImportError = 0;
	int nImportOK = 0;
	CString str;

	try
	{
		// READONLY Open auf Deluxe CD
		db.Open(sFileName, TRUE, TRUE);
	}
	catch (CDaoException* e)
	{
		if (e->m_pErrorInfo)
			ErrorBox(e->m_pErrorInfo->m_strDescription);
		else
			ErrorBox(get_string(IDS_OPEN_FAILED), sFileName, e->m_nAfxDaoError);

		e->Delete();
		return (-1);
	}

	CDaoTableDef Table(&db);
	try
	{
		Table.Open(L"Titles");
	}
	catch(CDaoException* e)
	{
		// Iss wohl keine MS-Deluxe Datenbank... ???
		e->Delete();

		sBuf.LoadString (IDS_NODELUXEDATABASE);

		ErrorBox (sBuf, sFileName);
		return (-1);
	}

	Table.Close();

	CDaoRecordset RecordSet(&db);

	CString s = "SELECT * FROM Titles";
	RecordSet.Open(dbOpenDynaset, s, dbSeeChanges);

	RecordSet.MoveLast();
	RecordSet.MoveFirst();
	nRecordCount = RecordSet.GetRecordCount();
	
	if (nRecordCount < 1)
		return (-1);

	dfMultiplikator = (double)0xffff / (double)(nRecordCount);

	//nRecordCount = (unsigned long)((double)nRecordCount * dfMultiplikator);
	//if (nRecordCount > 65535L)
	//	nRecordCount = 65535L;

//	DataBaseProgressDlg.Create(IDD_HITDB_DATABASE_PROGRESS);
//	DataBaseProgressDlg.SetWindowText(get_string(IDS_IMPORTDATA));

	ctlProgress.SetRange(1, (unsigned short)((double)nRecordCount * dfMultiplikator));
//!!!!!!!!!!!!!!!	SetProgressDlg (&DataBaseProgressDlg);

	nCurrentRecordNumber = 1;
	while (!RecordSet.IsEOF())
	{
		CCD CD(this);

		CheckAbortProc ();

		if (m_StopSearchProcess == TRUE)
			break;

		// Schaun wir mal...
		var = RecordSet.GetFieldValue(L"TitleID");
		CD.GetIdentity()->m_sIdentity.Format (L"%d", var.lVal);

		var = RecordSet.GetFieldValue(L"Artist");
		CD.GetArtist()->m_sArtist = var.bstrVal;

		var = RecordSet.GetFieldValue(L"Title");
		CD.m_sTitle = var.bstrVal;

		var = RecordSet.GetFieldValue(L"NumTracks");
		CD.m_wNumberOfTracks = (short)var.lVal;

		// Hier stehen die Längen der Lieder und der CD
		var = RecordSet.GetFieldValue(L"TitleQuery");
		sTrackLength = var.bstrVal;

		// Ermitteln der CD Länge - Letzte Hex-Zahl lesen
		sBuf = sTrackLength.Mid(sTrackLength.ReverseFind('+'));
		// Frames in Millisekunden wandeln
		CD.m_dwTotalLength = convert_frames2ms ((unsigned long)wcstoul(sBuf, (wchar_t **)0, 16));

		CDaoRecordset TracksRecordSet(&db);
		
		// Jetzt Lieder Tabelle lesen
		s.Format (L"SELECT TitleID,TrackID,TrackName FROM Tracks WHERE TitleID = %s", CD.GetIdentity()->m_sIdentity);
		TracksRecordSet.Open(dbOpenDynaset, s, dbSeeChanges); //dbForwardOnly

		// Titel Werte füllen
		int nTrackNr;
		while (!TracksRecordSet.IsEOF())
		{
			var = TracksRecordSet.GetFieldValue (L"TrackID");
			nTrackNr = var.lVal;

			if (nTrackNr < 0 || nTrackNr > CD.m_wNumberOfTracks - 1)
			{
				// Fehler - zu viele Einträge
				break;
			}

			var = TracksRecordSet.GetFieldValue (L"TrackName");
			sBuf = var.bstrVal;

			CD.GetTrack(nTrackNr)->m_sTitle = sBuf;
			
			// Länge für Lied X aus String auslesen
			CD.GetTrack(nTrackNr)->m_dwLength = GetMSDeluxeTrackLength(sTrackLength, nTrackNr);
	
			TracksRecordSet.MoveNext();
		}

		TracksRecordSet.Close();

		nCurrentPos = (unsigned short)((double)nCurrentRecordNumber * dfMultiplikator);
		// Darf nicht größer werden aber man weiß ja nie...
		if (nCurrentPos > 65535)
			nCurrentPos = 65535;

		ctlProgress.SetPos((unsigned short)nCurrentPos);
		sBuf.LoadString (IDS_CONVERTTEXT);
		str.Format(sBuf, nCurrentRecordNumber, nRecordCount);
		stcStatus.SetWindowText(str);
//		DataBaseProgressDlg.UpdateData(FALSE);
		
		nCurrentRecordNumber++;

		// Ab in die Datenbank
		if (CD.Add () != TRUE)
		{
			nImportError++;
		}
		else
			nImportOK++;

		
		RecordSet.MoveNext();
	}
	
	RecordSet.Close();

//	DataBaseProgressDlg.DestroyWindow();
	
	// Close und tschüss
	db.Close();

	CString sInfo;

	sBuf.LoadString (IDS_IMPORTDELUXEINFO);
	sInfo.Format(sBuf, nImportOK, nImportError);
	AfxMessageBox (sInfo, MB_OK);

	return 0;
}

// Ermitteln der einzelnen Liedlängen aus dem Query String
long CDataBase::GetMSDeluxeTrackLength(CString& sTrackLength, int nTrackNr)
{
	// sTrackLength Beispiel: cd=4+CF+5582+9D37+EF26+1249B
	// Länge in ms für jeden Track ermitteln - 0 basierend
	CString sBuf, eBuf;
	unsigned long ret;
	int nPos;
	
	sBuf = sTrackLength;

	sBuf = sBuf.Mid(sBuf.Find('+') + 1);

	//dwStartMS = strtoul(sBuf.Left(sBuf.Find('+')), (char **)0, 16);

	// Bis zum gewünschten Track vorhangeln...
	nPos = 0;
	while (nPos < nTrackNr)
	{
		sBuf = sBuf.Mid(sBuf.Find('+') + 1);
		nPos++;
	}

	// Jetzt Laufzeiten ermitteln
	eBuf = sBuf.Mid(sBuf.Find('+') + 1);

	sBuf = sBuf.Left(sBuf.Find('+'));
	if (eBuf.Find('+') > 0)
		eBuf = eBuf.Left(eBuf.Find('+'));

	ret = (unsigned long)wcstoul(eBuf, (wchar_t **)0, 16) - (unsigned long)wcstoul(sBuf, (wchar_t **)0, 16);
	// Frames in Millisekunden umwandeln
	ret = convert_frames2ms (ret);
	return (ret);
}
