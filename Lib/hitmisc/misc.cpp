// hitmisc.cpp : implementation file
//

#include "stdafx.h"
#include "hitmisc.h"
#include "HitbaseWinAppBase.h"
#include "shlwapi.h"
#include <io.h>
#include "../../app/hitbase/resource.h"
#include ".\misc.h"

#ifdef _DEBUG
#undef THIS_FILE
static char BASED_CODE THIS_FILE[] = __FILE__;
#endif

using namespace System;
using namespace Big3::Hitbase::Configuration;

int CMisc::m_iXPVisualStyle = -1;

void WarningBox(CString format, ...)
{
   wchar_t szBuffer[512];              // Jus 05-Aug-96: War nur 256!
   char *pArguments;
   CString aufb;
   HWND hwnd;

//   hwnd = GetActiveWindow();
//   if (!hwnd)
      hwnd = AfxGetMainWnd()->GetSafeHwnd();

   aufb.LoadString(TEXT_WARNING);

   pArguments = (char *)&format+sizeof(format);
   vswprintf(szBuffer, format, pArguments);
   MessageBox(hwnd, szBuffer, aufb, MB_OK|MB_ICONINFORMATION);
}

void ErrorBox(CString format, ...)
{
   wchar_t szBuffer[512];              // Jus 05-Aug-96: War nur 256!
   char *pArguments;
   CString aufb;
   HWND hwnd;

//   hwnd = GetActiveWindow();
//   if (!hwnd)
      hwnd = AfxGetMainWnd()->GetSafeHwnd();

   aufb.LoadString(TEXT_ERROR);

   pArguments = (char *)&format+sizeof(format);
   vswprintf(szBuffer, format, pArguments);
   MessageBox(hwnd, szBuffer, aufb, MB_OK|MB_ICONSTOP);
}

void WarningResBox(UINT resid, ...)
{
   wchar_t szBuffer[512];              // Jus 05-Aug-96: War nur 256!
   char *pArguments;
   CString aufb, format;
   HWND hwnd;

//   hwnd = GetActiveWindow();
//   if (!hwnd)
      hwnd = AfxGetMainWnd()->GetSafeHwnd();

   aufb.LoadString(TEXT_WARNING);
   format.LoadString(resid);

   pArguments = (char *)&resid+sizeof(resid);
   vswprintf(szBuffer, format, pArguments);
   MessageBox(hwnd, szBuffer, aufb, MB_OK|MB_ICONSTOP);
}

/*void ErrorResBox(UINT resid, ...)
{
   CString aufb, format;
   HWND hwnd;

   hwnd = AfxGetMainWnd()->GetSafeHwnd();

   aufb.LoadString(TEXT_ERROR);
   format.LoadString(resid);

   pArguments = (char *)&resid+sizeof(resid);
   vsprintf(szBuffer, format, pArguments);
   MessageBox(hwnd, szBuffer, aufb, MB_OK|MB_ICONSTOP);
}*/

void ErrorResBox(CWnd* pWnd, UINT resid, ...)
{
   wchar_t szBuffer[512];              // Jus 05-Aug-96: War nur 256!
   char *pArguments;
   CString aufb, format;

   if (!pWnd)
      pWnd = AfxGetMainWnd();

   aufb.LoadString(TEXT_ERROR);
   format.LoadString(resid);

   pArguments = (char *)&resid+sizeof(resid);
   vswprintf(szBuffer, format, pArguments);
   pWnd->MessageBox(szBuffer, aufb, MB_OK|MB_ICONSTOP);
}

int MessageResBox(UINT resid, UINT nType, ...)
{
   wchar_t szBuffer[512];              // Jus 05-Aug-96: War nur 256!
   char *pArguments;
   CString format;

   format.LoadString(resid);

   pArguments = (char *)&nType+sizeof(nType);
   vswprintf(szBuffer, format, pArguments);
   return AfxMessageBox(szBuffer, nType);
}

int CMisc::MessageResBox(CWnd* pWnd, UINT resid, UINT nType, ...)
{
   wchar_t szBuffer[512];              // Jus 05-Aug-96: War nur 256!
   char *pArguments;
   CString format;

   format.LoadString(resid);

   pArguments = (char *)&nType+sizeof(nType);
   vswprintf(szBuffer, format, pArguments);
   return pWnd->MessageBox(szBuffer, get_string(AFX_IDS_APP_TITLE), nType);
}

void DrawBitmap(CDC *pdc, CBitmap *pBitmap, int xStart, int yStart, CDC* pDCSelect /* = NULL */)
{
	BITMAP bm;
	CDC hdcMem, *pDrawDC;
	CPoint ptOrg(0, 0);
	CBitmap* oldBitmap;
	
	if (pDCSelect)
	{
		hdcMem.CreateCompatibleDC(pDCSelect);
		pDrawDC = &hdcMem;
	}
	else
	{
		hdcMem.CreateCompatibleDC(pdc);
		pDrawDC = &hdcMem;
	}

	oldBitmap = pDrawDC->SelectObject(pBitmap);                               
	pDrawDC->SetMapMode(pdc->GetMapMode());
	
	pBitmap->GetObject(sizeof(BITMAP), (LPSTR)&bm);
	
	CSize bmSize(bm.bmWidth, bm.bmHeight);
	
	CPoint ptSize(bmSize);
	pdc->DPtoLP(&ptSize);
	
	pDrawDC->DPtoLP(&ptOrg);
	
	pdc->BitBlt(xStart, yStart, ptSize.x, ptSize.y, pDrawDC, ptOrg.x, ptOrg.y, SRCCOPY);
	
	pDrawDC->SelectObject(oldBitmap);
}

void DrawStretchBitmap(CDC *pdc, CBitmap *pBitmap, CRect rect)
{
   BITMAP bm;
   CDC hdcMem;
   CPoint ptOrg(0, 0);
   CBitmap* oldBitmap;

   hdcMem.CreateCompatibleDC(pdc);
   oldBitmap = hdcMem.SelectObject(pBitmap);                               
   hdcMem.SetMapMode(pdc->GetMapMode());

   pBitmap->GetObject(sizeof(BITMAP), (LPSTR)&bm);

   CSize bmSize(bm.bmWidth, bm.bmHeight);

   CPoint ptSize(bmSize);
   pdc->DPtoLP(&ptSize);

   hdcMem.DPtoLP(&ptOrg);

   pdc->SetStretchBltMode(COLORONCOLOR);
   pdc->StretchBlt(rect.left, rect.top, rect.Width(), rect.Height(), &hdcMem, ptOrg.x, ptOrg.y, ptSize.x, ptSize.y, SRCCOPY);

   pdc->SelectObject(oldBitmap);
}

// Wandelt den angegebenen COLORREF in Device-Farben um.

void ConvertRGBToDevice(COLORREF rgb, int& blue,  int maxblue, 
                                      int& green, int maxgreen,
                                      int& red,   int maxred)
{
   red   = GetRValue(rgb)*maxblue/255;
   green = GetGValue(rgb)*maxgreen/255;
   blue  = GetBValue(rgb)*maxblue/255;
}

void Draw3DSunken(CDC *pdc, int x, int y, int cx, int cy)
{
   CPen whitepen, blackpen, *oldpen;

   blackpen.CreatePen(PS_SOLID, 1, RGB(128, 128, 128));
   whitepen.CreatePen(PS_SOLID, 1, RGB(255, 255, 255));
   oldpen = (CPen *)pdc->SelectObject(&blackpen);
   pdc->MoveTo(x, y+cy);
   pdc->LineTo(x, y);
   pdc->LineTo(x+cx, y);
   pdc->SelectObject(&whitepen);
   pdc->LineTo(x+cx, y+cy);
   pdc->LineTo(x, y+cy);
   pdc->SelectObject(oldpen);
   blackpen.DeleteObject();
   whitepen.DeleteObject();
}

void Draw3DSunken(CDC *pdc, CRect rect)
{
   Draw3DSunken(pdc, rect.left, rect.top, rect.right-rect.left, rect.bottom-rect.top);
}

/*
 * Wandelt einen long integer (Anzahl der Millisekunden) in
 * das Zeitformat (MM:SS.TTT) um.
 */

CString long2mstime(long ltime)
{
	CString str;
	
	if (ltime < 0L)
		ltime = 0L;
	
	str.Format(L"%02ld:%02ld.%03ld", ltime/60000L, ltime%60000L/1000L, ltime%1000L);
	
	return str;
}

/*
 * Wandelt die angegebene Zeit (MM:SS) in einen long integer
 * (Anzahl der Millisekunden) um.
 * Returns -1, wenn Format nicht stimmt
 *         ansonsten Anzahl der Millisekunden.
 */
 
long time2long(const CString& stime)
{
	long ltime=0L;
	CString aufb;
	CString tmpbuf;
	
//	if (strlen(stime) > 5)
//		return -1L;
	
	aufb = stime;
	tmpbuf = stime;
	
	if (aufb.Find(':') >= 0)     /* Irgendwas in der Form MM:SS oder M:SS */
	{
		ltime += _wtol(aufb.Left(aufb.Find(':')))*60000;
		ltime += _wtol(aufb.Mid(aufb.Find(':')+1))*1000;
	}
	else
		ltime = _wtol(tmpbuf)*1000L;
	
	return ltime;
}

/*
 * Wandelt die angegebene Zeit (MM:SS.TTT) in einen long integer
 * (Anzahl der Millisekunden) um.
 * Returns -1, wenn Format nicht stimmt
 *         ansonsten Anzahl der Millisekunden.
 */
 
long mstime2long(const CString& stime)
{
	long ltime=0L;
	wchar_t aufb[20], tmpbuf[20];
	
	if (wcslen(stime) > 9)
		return -1L;
	
	wcscpy(aufb, stime);
	wcscpy(tmpbuf, stime);
	
	if (wcschr(aufb, ':'))     /* Irgendwas in der Form MM:SS oder M:SS */
	{
		*wcschr(aufb, ':') = 0;
		ltime += _wtol(aufb)*60000L;
		wcscpy(aufb, wcschr(tmpbuf, ':')+1);
		ltime += _wtol(aufb)*1000L;
		if (wcschr(aufb, '.'))   // Noch Millisekunden
			ltime += _wtol(wcschr(aufb, '.')+1);
	}
	else
	{
		ltime = _wtol(tmpbuf)*1000L;
		if (wcschr(aufb, '.'))   // Noch Millisekunden
			ltime += _wtol(wcschr(aufb, '.')+1);
	}
	
	return ltime;
}

/*
 *  Do shell-style pattern matching for ?, \, [], and * characters.
 *  Might not be robust in face of malformed patterns; e.g., "foo[a-"
 *  could cause a segmentation violation... or GPF
 *
 *  usage: wildmat (text, pattern);
 */

int Star(char *s, char *p)
{
    while (wildmat(s, p) == FALSE)
        if (*s == '\0' || *++s == '\0')
            return(FALSE);
    return(TRUE);
}


int wildmat(char *s, char *p)
{
   int last;
   int matched;
   int reverse;
   char patt[51], *pattern;

   if (!*p)
      return TRUE;

   strcpy(patt, p);

   pattern = patt;

       for ( ; *pattern; s++, pattern++)
           switch (*pattern) {
               case '\\':
                   /* Literal match with following character; fall through. */
                   pattern++;
               default:
                   if (*s != *pattern)
                       return(FALSE);
                   continue;
               case '?':
                   /* Match anything. */
                   if (*s == '\0')
                       return(FALSE);
                   continue;
               case '*':
                   /* Trailing star matches everything. */
                   return(*++pattern ? Star(s, pattern) : TRUE);
               case '[':
                   /* [!....] means inverse character class. */
                   if (reverse = pattern[1] == '!')
                       pattern++;
                   for (last = 0400, matched = FALSE; *++pattern && *pattern != ']'; last = *pattern)
                       /* This next line requires a good C compiler. */
                       if (*pattern == '-' ? *s <= *++pattern && *s >= (char)last : *s == *pattern)
                           matched = TRUE;
                   if (matched == reverse)
                       return(FALSE);
                   continue;
           }
       return(*s == '\0');
}

int check_serialnumber()
{
	return FALSE;
}

// Liefert das aktuelle Datum in der Form TT.MM.JJJJ (oder MM/TT/JJJJ) zurück.
CString GetRealDate()
{
	CTime now = CTime::GetCurrentTime();
	CString str;
	
#if GERMAN
	str.Format(L"%02d.%02d.%04d", now.GetDay(), now.GetMonth(), now.GetYear());
#endif
#if ENGLISH
	str.Format(L"%02d/%02d/%04d", now.GetMonth(), now.GetDay(), now.GetYear());
#endif
	
	return str;
}

/*
 * Schneidet den übergebenen String nach cx Pixeln ab. Wenn der String länger
 * als cx pixel ist, werden am Schluß noch drei Punkte eingefügt.
 */

void CMisc::CutLength(CDC *pDC, CString& str, int cx)
{
	CSize size, size1;
	int cut=0;
	
	size1 = pDC->GetTextExtent("...");
	
	if (cx < size1.cx)       // Text ist kleiner als Drei Punkte! geht nicht!
	{
		str = "";
		return;
	}
	
	while (!str.IsEmpty())
	{
		size = pDC->GetTextExtent(str);
		
		if (!cut && size.cx <= cx-2 || size.cx <= cx-size1.cx-2)
            break;
		
		cut = 1;
		str = str.Left(str.GetLength()-1);
	}
	
	if (cut)      
		str += "...";
}

/*
 * Liefert eine Zufallszahl im Bereich [min;max] zurück.
 */
             
int GetRandom(int min, int max)
{
   return ((rand() % (int)(((max)+1) - (min))) + (min));
}

/*
 * Liefert einen Pointer auf der reinen Dateinamen der angegebenen Datei zurück.
 */

char *GetFileNameFromPath(char *filename)
{
	char *p;
	
	if ((p = strrchr(filename, '\\')))
		return p+1;
	else
		return filename;
}

CString GetFileNameFromPath(const CString& filename)
{
	int nPos;
	
	if ((nPos = filename.ReverseFind('\\')))
		return filename.Mid(nPos+1);
	else
		return filename;
}

/*
 * Liefert den Pfad des angegebenen Dateinamens zurück (mit '\\' am Schluß).
 */

CString GetPathFromFileName(const CString& filename)
{
   CString path;
   int p;

   path = filename;

   if ((p = path.ReverseFind('\\')))
      path = path.Left(p+1);

   return path;
}

CString CMisc::CombinePathWithFileName(const CString& path, const CString& filename)
{
	if (path.Right(1) == "\\")
		return path + filename;
	else
		return path + "\\" + filename;
}

/*
 * Schneidet die Extension von der Datei ab.
 */

/*void CutFileNameExtension(char *filename)
{
	char *p, *p1;
	
	if ((p = strrchr(filename, '.')))
	{
		p1 = strrchr(filename, '\\');          // Punkt kann theoretisch auch for \\ stehen
		if (!p1 || p > p1)                     // Wenn keine Extension angegeben wurde, würde
			*p = 0;                            // dann das Verzeichnis mit dem . gekürzt!
	}
}*/

void CMisc::CutFileNameExtension(CString &filename)
{
	int pointPos = filename.ReverseFind('.');
	if (pointPos >= 0)
	{
		int slashPos = filename.ReverseFind('\\');
		if (slashPos < 0 || pointPos > slashPos)
			filename = filename.Left(pointPos);
	}
}

/*
 * Liefert die Extension der angegebenen Datei zurück (mit Punkt).
 */

CString CMisc::GetFileNameExtension(const CString& sFilename)
{
	int pointPos = sFilename.ReverseFind('.');
	if (pointPos >= 0)
	{
		int slashPos = sFilename.ReverseFind('\\');     // Punkt kann theoretisch auch for \\ stehen
		if (slashPos < 0 || pointPos > slashPos)       // Wenn keine Extension angegeben wurde, würde
			return sFilename.Mid(pointPos);			   // dann das Verzeichnis mit dem . gekürzt!
	}

	return "";
}

/*
 * Liefert den angegebenen String aus den Resource direkt als CString-Object zurück.
 */

CString get_string(int id)
{
   CString rcstr;

   rcstr.LoadString(id);

   return rcstr;
}

/*
 * Spielt einen Sound aus der Event-Liste.
 */

void PlaySoundEvent(int Event)
{
   if (((CHitbaseWinAppBase*)AfxGetApp())->m_config.SoundEvent[Event][0])
   {
      // Beim Verlassen des Programms muß die Wave-Datei syncron abgespielt
      // werden, da der Sound sonst direkt wieder beendet wird.
      if (Event == EVENT_END)     
         PlaySound(((CHitbaseWinAppBase*)AfxGetApp())->m_config.SoundEvent[Event], NULL, SND_SYNC|SND_FILENAME|SND_NODEFAULT|SND_NOSTOP);
      else
         PlaySound(((CHitbaseWinAppBase*)AfxGetApp())->m_config.SoundEvent[Event], NULL, SND_ASYNC|SND_FILENAME|SND_NODEFAULT|SND_NOSTOP);
   }
}

/*
 * Returns formatted number with points in a string. 
 */
 
CString GetFormattedNumber(DWORD nr)
{
   CString str;
   
   if (nr < 1000L)
   {
      str.Format(L"%ld", nr);
      return str;
   }
   
   if (nr < 1000000L)
   {
      str.Format(L"%ld.%03ld", nr/1000L, nr % 1000L);
      return str;
   }
   
   if (nr < 1000000000L)
   {
      str.Format(L"%ld.%03ld.%03ld", nr/1000000L, nr/1000L%1000L, nr % 1000L);
      return str;
   }
   
   str.Format(L"%ld.%03ld.%03ld.%03ld", nr/1000000000L, nr/1000000L%1000L, nr/1000L%1000L, nr % 1000L);
   
   return str;
}

/*
 * Setzt die Felder einer Button-Liste.
 */

void FillButtonStruct(TBBUTTON *tb, int iBitmap, int idCommand, BOOL bSeparator)
{
   memset(tb, 0, sizeof(TBBUTTON));

   tb->iBitmap = iBitmap;
   tb->idCommand = idCommand;
   if (bSeparator)
   {
      tb->fsState = TBSTATE_ENABLED|TBSTATE_WRAP;
      tb->fsStyle = TBSTYLE_SEP;
   }
   else
   {
      tb->fsState = TBSTATE_ENABLED;
      tb->fsStyle = TBSTYLE_BUTTON;
   }
}

/*
 * Füll den angegebenen Static-Control mit dem benutzerdefinierten
 * Feld. Wenn es nicht definiert ist, so wird der Control disabled.
 * WndEdit kann auch NULL sein (dann wird dies auch nicht disabled!).
 */

void SetUserFieldControlText(CStatic *WndStatic, CEdit *WndEdit, const char *text)
{
   CString str;

   if (text[0])
   {
      str.Format(L"%s:", text);
      WndStatic->SetWindowText(str);
   }
   else
   {
      str.LoadString(TEXT_UNDEFINED);
      WndStatic->SetWindowText(str);
      WndStatic->EnableWindow(FALSE);
      if (WndEdit)
         WndEdit->EnableWindow(FALSE);
   }
}

void FillCDSamplerComboBox(CComboBox* pComboBox)
{
   pComboBox->AddString(get_string(TEXT_NO));
   pComboBox->AddString(get_string(TEXT_YES));
}


/*
 * Führt einen deutschen String-Vergleich aus.
 * Die Umlaute (Ä, Ö, Ü, ß) werden in AE, OE, UE, SS umgesetzt.
 */

int German_stricmp(const CString& left, const CString& right)
{
   CString GLeft, GRight;

   for (int j=0;j<left.GetLength();j++)
   {
      switch (left[j])
      {
      case 'ä':
      case 'Ä':
         GLeft += "AE";
         break;
      case 'ö':
      case 'Ö':
         GLeft += "OE";
         break;
      case 'ü':
      case 'Ü':
         GLeft += "UE";
         break;
      case 'ß':
         GLeft += "SS";
         break;
      default:
         GLeft += left[j];
      }
   }

   for (int j=0;j<right.GetLength();j++)
   {
      switch (right[j])
      {
      case 'ä':
      case 'Ä':
         GRight += "AE";
         break;
      case 'ö':
      case 'Ö':
         GRight += "OE";
         break;
      case 'ü':
      case 'Ü':
         GRight += "UE";
         break;
      case 'ß':
         GRight += "SS";
         break;
      default:
         GRight += right[j];
      }
   }
   return _wcsicmp(GLeft, GRight);
}

// Liefert die logische Device-ID des angegebenen CDROM-Laufwerks
// zurück. Wird für CDAudio.Open benötigt.
// drive: Laufwerksbuchstabe z.B. 'G'

int GetDeviceIDFromDrive(int drive)
{
   int i, DeviceID=-1;
   UINT type;
   CString devicename;

   for (i=2;i<26;i++)
   {
      devicename.Format(L"%c:\\", 'A'+i);
      type = GetDriveType(devicename);
      if (type == DRIVE_CDROM)
      {
         DeviceID++;

         if ('A'+i == toupper(drive))
            return DeviceID;
      }
   }

   return -1;
}

// Liefert die logische Device-ID des angegebenen CDROM-Laufwerks
// zurück. Wird für CDAudio.Open benötigt.
// drive: Laufwerksbuchstabe z.B. 'G'

CString GetDriveFromDeviceID(int id)
{
   int i, DeviceID=-1;
   UINT type;
   CString devicename;

   for (i=2;i<26;i++)
   {
      devicename.Format(L"%c:\\", 'A'+i);
      type = GetDriveType(devicename);
      if (type == DRIVE_CDROM)
      {
         DeviceID++;

         if (DeviceID == id)
            return devicename.Left(2);
      }
   }

   ASSERT(FALSE);

   return "";
}

// Der Shareware-String wird "leicht" verschlüsselt, damit er nicht direkt
// in den Binaries gefunden wird.
CString GetSharewareString(void)
{
#if BETAVERSION
/*!!!!!!!!!!SHAREWARE*/	CString str = L"SG?OAR;K=";      // ASCII -0, -1, -2, -3, -4, usw..
//!!!!!!!!!!BETA    	CString str = L"BDR>\x1c\x2e";   // ASCII -0, -1, -2, -3, -4, usw..
/*!!!!!!!!!!RC 1	CString str = L"RB\x1e\x2e";*/         // ASCII -0, -1, -2, -3, -4, usw..
//!!!!!!!!!!ALPHA 1   	CString str = L"AKNE=\x1b+";     // ASCII -0, -1, -2, -3, -4, usw..
#else
#if LICENSE_BHV
/*!!!!!!!!!!DEMO-VERSION*/	CString str = L"DDKL)Q?KK@EC";      // ASCII -0, -1, -2, -3, -4, usw..
#else
/*!!!!!!!!!!SHAREWARE*/	CString str = L"SG?OAR;K=";     // ASCII -0, -1, -2, -3, -4, usw..
//!!!!!!!!!!BETA 1   	CString str = L"BDR>\x1c\x2b";     // ASCII -0, -1, -2, -3, -4, usw..
//!!!!!!!!!!RC 2	CString str = L"RB\x1e\x2f";     // ASCII -0, -1, -2, -3, -4, usw..
//!!!!!!!!!!ALPHA 1   	CString str = L"AKNE=\x1b+";     // ASCII -0, -1, -2, -3, -4, usw..
#endif
#endif

	int i;
	
	for (i=0;i<str.GetLength();i++)
		str.SetAt(i, str[i]+i);
	
	return str;
}

void DebugMessage(CString fmt, ...)
{
   if (((CHitbaseWinAppBase*)AfxGetApp())->m_iDebug)
   {
      wchar_t szBuffer[256];
      char *pArguments;

      pArguments = (char *)&fmt+sizeof(const char *);
      vswprintf(szBuffer, fmt, pArguments);
      AfxMessageBox(szBuffer, MB_OK|MB_ICONEXCLAMATION);
   }
}

void NoMemoryMessage( void )
{
   ErrorResBox(NULL, TEXT_NOMEM);
}

HBITMAP LoadResourceBitmap(LPCTSTR lpString, HPALETTE * lphPalette)
{
    HRSRC  hRsrc;
    HGLOBAL hGlobal;
    HBITMAP hBitmapFinal = NULL;
    LPBITMAPINFOHEADER  lpbi;
    HDC hdc;
    int iNumColors;
	
    if (hRsrc = FindResource(AfxGetInstanceHandle(), lpString, RT_BITMAP))
	{
		hGlobal = LoadResource(AfxGetInstanceHandle(), hRsrc);
		lpbi = (LPBITMAPINFOHEADER)LockResource(hGlobal);
		
		hdc = GetDC(NULL);
		*lphPalette =  CreateDIBPalette ((LPBITMAPINFO)lpbi, &iNumColors);
		if (*lphPalette)
		{
			SelectPalette(hdc,*lphPalette,FALSE);
			RealizePalette(hdc);
		}
		
		hBitmapFinal = CreateDIBitmap(hdc,
			(LPBITMAPINFOHEADER)lpbi,
			(LONG)CBM_INIT,
			(LPSTR)lpbi + lpbi->biSize + iNumColors *
			sizeof(RGBQUAD),
			(LPBITMAPINFO)lpbi,
			DIB_RGB_COLORS );
		
		ReleaseDC(NULL,hdc);
		UnlockResource(hGlobal);
		FreeResource(hGlobal);
	}
	
    return (hBitmapFinal);
}
 
HPALETTE CreateDIBPalette (LPBITMAPINFO lpbmi, LPINT lpiNumColors)
{
   LPBITMAPINFOHEADER  lpbi;
   LPLOGPALETTE     lpPal;
   HANDLE           hLogPal;
   HPALETTE         hPal = NULL;
   int              i;
 
   lpbi = (LPBITMAPINFOHEADER)lpbmi;
   if (lpbi->biBitCount <= 8)
       *lpiNumColors = (1 << lpbi->biBitCount);
   else
       *lpiNumColors = 0;  // No palette needed for 24 BPP DIB
 
   if (*lpiNumColors)
      {
      hLogPal = GlobalAlloc (GHND, sizeof (LOGPALETTE) +
                             sizeof (PALETTEENTRY) * (*lpiNumColors));
      lpPal = (LPLOGPALETTE) GlobalLock (hLogPal);
      lpPal->palVersion    = 0x300;
      lpPal->palNumEntries = *lpiNumColors;
 
      for (i = 0;  i < *lpiNumColors;  i++)
         {
         lpPal->palPalEntry[i].peRed   = lpbmi->bmiColors[i].rgbRed;
         lpPal->palPalEntry[i].peGreen = lpbmi->bmiColors[i].rgbGreen;
         lpPal->palPalEntry[i].peBlue  = lpbmi->bmiColors[i].rgbBlue;
         lpPal->palPalEntry[i].peFlags = 0;
         }
      hPal = CreatePalette (lpPal);
      GlobalUnlock (hLogPal);
      GlobalFree   (hLogPal);
   }
   return hPal;
}

//Here is an example of how you might use the above function to load a bitmap
//from a resource and display it using a logical palette:
 
void DrawPaletteBitmapResource(HDC hdc, LPCTSTR lpRes, short xStart, short yStart)
{
   HBITMAP hBitmap, hOldBitmap;
   HPALETTE hPalette, oldPal;
   HDC hMemDC;
   BITMAP bm;
 
   hBitmap = LoadResourceBitmap(lpRes, &hPalette);
   GetObject(hBitmap, sizeof(BITMAP), (LPSTR)&bm);
   hMemDC = CreateCompatibleDC(hdc);
   oldPal = SelectPalette(hdc, hPalette, FALSE);
   RealizePalette(hdc);
   SelectPalette(hMemDC, hPalette, FALSE);
   RealizePalette(hMemDC);
   hOldBitmap = (HBITMAP)SelectObject(hMemDC, hBitmap);
   BitBlt(hdc, xStart, yStart, bm.bmWidth, bm.bmHeight, hMemDC, 0, 0, SRCCOPY);
   DeleteObject(SelectObject(hMemDC, hOldBitmap));
   SelectObject(hdc, oldPal);
   DeleteDC(hMemDC);
   DeleteObject(hPalette);
}
 
//Here is an example of how you might use the above function to load a bitmap
//from a resource and display it using a logical palette:
 
void DrawPaletteBitmap(CDC* pDC, CBitmap* pBitmap, CPalette* pPalette, int x, int y)
{
   CBitmap* pOldBitmap;
   CPalette* pOldPalette;
   CDC MemDC;
   BITMAP bm;
 
   pBitmap->GetObject(sizeof(BITMAP), (LPSTR)&bm);
   MemDC.CreateCompatibleDC(pDC);
   pOldPalette = pDC->SelectPalette(pPalette, FALSE);
   pDC->RealizePalette();
   MemDC.SelectPalette(pPalette, FALSE);
   MemDC.RealizePalette();
   pOldBitmap = MemDC.SelectObject(pBitmap);
   pDC->BitBlt(x, y, bm.bmWidth, bm.bmHeight, &MemDC, 0, 0, SRCCOPY);
   MemDC.SelectObject(pOldBitmap);
   pDC->SelectObject(pOldPalette);
}
 
// Wandelt alle Strings so um, dass sie ohne Probleme bei SQL-Abfragen
// benukorrekt in die Datenbank
// eingetragen werden können. (alle " werden in "" umgewandelt)
void CMisc::SqlPrepare(CString &str)
{
   CString SqlStr = "";
   int i;

   for (i=0;i < str.GetLength();i++)
   {
      if (str[i] == '\"')
         SqlStr += "\"\"";
      else
         SqlStr += str[i];
   }
   
   str = SqlStr;
}

// Wandelt alle Strings so um, dass sie ohne Probleme bei SQL-Abfragen
// benukorrekt in die Datenbank
// eingetragen werden können. (alle " werden in "" umgewandelt)
CString CMisc::GetSqlString(const CString &str)
{
	CString sSQL = str;
	SqlPrepare(sSQL);

	return sSQL;
}

/*
 * Nur für globale Umsetzungen! Für Datenbank-Datum CDatabase::DateShort2Long
 * benutzen.
 * Erzeugt immer langes Format (TT.MM.JJJJ oder DD/MM/YYYY)
 */

CString DateShort2Long(CString sDate)
{
   CString datestr;

   if (sDate.IsEmpty())
      return (CString)"";

#if ENGLISH
   datestr = sDate.Mid(4,2)+"/"+sDate.Mid(6,2)+"/"+sDate.Mid(0,4);
#else
   datestr = sDate.Mid(6,2)+"."+sDate.Mid(4,2)+"."+sDate.Mid(0,4);
#endif

   return datestr;
}

// Wandelt die angegebenen Sekunden in einen Klartext um. Maximal
// werden Tage angezeigt.
// Z.B.: 5 Tage 3 Stunden 1 Minute 20 Sekunden

#define ANZ_SEK_TAG       86400    // jus 19-Okt-97: Hier stand vorher 43200!! Dieter, wie viele Stunden hat der Tag?
#define ANZ_SEK_STUNDEN   3600
#define ANZ_SEK_MINUTEN   60

CString GetTextFromSeconds(int nSeconds)
{
   CString ret, tmp;
   int iAnzTage=0;
   int iAnzStunden=0;
   int iAnzMinuten=0;
   int iAnzSekunden=0;

   // Tage ausrechnen

   if (nSeconds >= ANZ_SEK_TAG)
      iAnzTage = nSeconds / ANZ_SEK_TAG;
   nSeconds = nSeconds - (iAnzTage * ANZ_SEK_TAG);

   // Stunden ausrechnen
   if (nSeconds >= ANZ_SEK_STUNDEN)
      iAnzStunden = nSeconds / ANZ_SEK_STUNDEN;
   nSeconds = nSeconds - (iAnzStunden * ANZ_SEK_STUNDEN);

   // Minuten ausrechnen
   if (nSeconds >= ANZ_SEK_MINUTEN)
      iAnzMinuten = nSeconds / ANZ_SEK_MINUTEN;
   nSeconds = nSeconds - (iAnzMinuten * ANZ_SEK_MINUTEN);

   // Sekunden nicht ausrechnen sondern (nur) eintragen

   iAnzSekunden = nSeconds;

   ret = "";

   // Tage
   if (iAnzTage > 0)
      if (iAnzTage == 1)
         ret = L"1 " + get_string(TEXT_DAY) + L" ";
      else
      {
         tmp.Format(L"%d ", iAnzTage);
         ret = tmp + get_string(TEXT_DAYS) + L" ";
      }

   // Stunden
   if (iAnzStunden > 0)
      if (iAnzStunden == 1)
         ret += L"1 " + get_string(TEXT_HOUR) + L" ";
      else
      {
         tmp.Format(L"%d ", iAnzStunden);
         ret += tmp + get_string(TEXT_HOURS) + L" ";
      }

   // Minuten
   if (iAnzMinuten > 0)
      if (iAnzMinuten == 1)
         ret += L"1 " + get_string(TEXT_MINUTE) + L" ";
      else
      {
         tmp.Format(L"%d ", iAnzMinuten);
         ret += tmp + get_string(TEXT_MINUTES) + L" ";
      }

   // Sekunden
   if (iAnzSekunden > 0)
      if (iAnzSekunden == 1)
         ret += L"1 " + get_string(TEXT_SECOND);
      else
      {
         tmp.Format(L"%d ", iAnzSekunden);
         ret += tmp + get_string(TEXT_SECONDS);
      }

   return ret;
}

// CMisc-Klasse

// Zunächt die statischen Variablen initialisieren

CMisc::CMisc()
{
}

CMisc::~CMisc()
{
}

// Legt das Verzeichnis an. Legt auch direkt mehrere Verzeichnisse an, wenn
// dies erforderlich ist.
BOOL CMisc::CreateDirectory(const CString &sDirectory)
{
	CString sDirPart;

	for (int i=0;i<sDirectory.GetLength();i++)
	{
		if (sDirectory[i] == '\\' && i > 2)
		{
			if (_waccess(sDirPart, 0))
			{
				if (!::CreateDirectory(sDirPart, NULL))
					return FALSE;
			}
		}

		sDirPart += sDirectory[i];
	}

	// Jetzt noch den letzten Teil des Verzeichnissen anlegen
	if (sDirectory.Right(1) != L"\\" && !::CreateDirectory(sDirectory, NULL))
		return FALSE;

	return TRUE;
}

void CMisc::Point2Comma(CString & sValue)
{
	for (int i=0;i<sValue.GetLength();i++)
		if (sValue[i] == '.')
			sValue.SetAt(i, ',');
}

void CMisc::Comma2Point(CString & sValue)
{
	for (int i=0;i<sValue.GetLength();i++)
		if (sValue[i] == ',')
			sValue.SetAt(i, '.');
}

// JUS: 09.08.1998
// Filtert aus dem angegebenen String alle Zeichen raus, die nicht
// als Dateiname zugelassen sind.
void CMisc::FilterFilenameChars(CString & sFilename)
{
	const CString sInvalid = "\\/:*?\"<>|";
	CString sCleanFilename;

	int i=0;
	while (i < sFilename.GetLength())
	{
		if (sInvalid.Find(sFilename[i]) < 0)
			sCleanFilename += sFilename[i];

		i++;
	}

	sFilename = sCleanFilename;
}

CString CMisc::GetTempFilename()
{
	wchar_t sTempPath[_MAX_PATH];
	wchar_t sTempFilename[_MAX_PATH];

	GetTempPath(_MAX_PATH, sTempPath);

	GetTempFileName(sTempPath, L"hit", 0, sTempFilename);

	return sTempFilename;
}

// Liefert das Datum in lokalisierter Form 
// (z.B. "03.11.1998" für Deutsch oder "11/03/1998" für Amerikanisch)
CString CMisc::GetLocalDate(const COleDateTime &date)
{
	return date.Format(VAR_DATEVALUEONLY);
}

// Liefert das Datum in SQL-Form ("#YYYY/MM/DD#").
CString CMisc::GetSQLDateString(const COleDateTime &date)
{
	CString str;

	str.Format(L"#%04d/%02d/%02d#", date.GetYear(), date.GetMonth(), date.GetDay());

	return str;
}

/*
 * Wandelt einen long integer (Anzahl der Millisekunden) in
 * das Zeitformat (MM:SS) um.
 */
 
CString CMisc::Long2Time(long ltime)
{
	CString str;
	
	if (ltime < 0L)
		ltime = 0L;
	
	str.Format(L"%02ld:%02ld", ltime/60000L, ltime%60000L/1000L);
	
	return str;
}

// Liste mit allen vorhandenen CDROM-Laufwerken füllen
void CMisc::GetCDDriveList(CStringArray& saDrives, BOOL bIncludeVirtualDrives /* = FALSE */)
{
	int i, drive = 0;
	CString devicename;
	UINT type;

	saDrives.RemoveAll();

	for (i=2;i<26;i++)
	{
		 devicename.Format(L"%c:\\", 'A'+i);
		 type = GetDriveType(devicename);
		 if (type == DRIVE_CDROM)
		 {
			 drive ++;
			 devicename.Format(L"%s %d (%c:)", get_string(TEXT_DRIVE), drive, 'A'+i);
			 saDrives.Add(devicename);
		 }
	}

	if (bIncludeVirtualDrives)
	{
		saDrives.Add(get_string(TEXT_VIRTUAL_CD));
	}
}

void CMisc::FillComboBoxWithCDDrives(CComboBox *cb, BOOL bIncludeVirtualDrives /* = FALSE */)
{
	CStringArray saDrives;

	GetCDDriveList(saDrives, bIncludeVirtualDrives);

	//  Combo box mit allen vorhandenen CDROM-Laufwerken füllen
	for (int i=0;i<saDrives.GetSize();i++)
		 cb->AddString(saDrives[i]);
}

// Wandelt alle Zeichen, die nicht direkt in einem HTTP-String übergeben werden
// können, um, in %ASCII-WERT.
CString CMisc::ConvertToHTTP(const CString &sStr)
{
	CString sHTTP = sStr;
	
	sHTTP.Replace(L"%", L"%25");
	sHTTP.Replace(L" ", L"%20");
	sHTTP.Replace(L"=", L"%3D");
	sHTTP.Replace(L"&", L"%26");
	sHTTP.Replace(L"+", L"%2B");
	sHTTP.Replace(L"#", L"%23");

	return sHTTP;
}

void CMisc::OpenURL(CWnd* pWnd, const CString &sURL)
{
	ShellExecute(*pWnd, L"open", (CString)L"http://" + sURL, NULL, L"", 0);
}

// Öffnet die HTTP Connection zum angegebenen Server
BOOL CMisc::OpenHTTPConnection(const CString& sServer, CInternetSession** pInternetSession, CHttpConnection** pHttpConnection, BOOL bDisplayErrorMessage /* = TRUE */)
{
	switch (Settings::Current->ProxyType)
	{
	case 0:
		*pInternetSession = new CInternetSession;
		break;
	case 1:
		*pInternetSession = new CInternetSession(NULL, 1, INTERNET_OPEN_TYPE_PROXY, (CString)Settings::Current->ProxyServerName);
		break;
	default:
		ASSERT(FALSE);
	}
	
	if (!(*pInternetSession))
		return FALSE;

	try
	{
	    *pHttpConnection = (*pInternetSession)->GetHttpConnection(sServer);
	}
	catch (CInternetException* e)
	{
		if (bDisplayErrorMessage)
		{
			wchar_t szCause[255];
			e->GetErrorMessage(szCause, 255);
			AfxMessageBox(szCause);
		}

		e->Delete();
		(*pInternetSession)->Close();
		delete *pInternetSession;

		return FALSE;
	}

	return TRUE;
}

// Die angegebene Datei auf einem Web-Server lesen. Inhalt steht danach im String.
BOOL CMisc::ReadFileFromURL(const CString& sServer, const CString &sFilename, CString &sContent, BOOL bDisplayErrorMessage /* = TRUE */, bool bBackgroundProcessing /* = false */)
{
	CInternetSession * pInternetSession;

	switch (Settings::Current->ProxyType)
	{
	case 0:
		pInternetSession = new CInternetSession;
		break;
	case 1:
		pInternetSession = new CInternetSession(NULL, 1, INTERNET_OPEN_TYPE_PROXY, (CString)Settings::Current->ProxyServerName);
		break;
	default:
		ASSERT(FALSE);
	}
	
	ASSERT(pInternetSession);

	CHttpConnection * pHttpConnection;
	try
	{
	    pHttpConnection = pInternetSession->GetHttpConnection(sServer);
	}
	catch (CInternetException* e)
	{
		if (bDisplayErrorMessage)
		{
			wchar_t szCause[255];
			e->GetErrorMessage(szCause, 255);
			AfxMessageBox(szCause);
		}

		e->Delete();
		pInternetSession->Close();
		delete pInternetSession;

		return FALSE;
	}

	BOOL bRet = ReadFileFromConnection(pHttpConnection, sFilename, sContent, bDisplayErrorMessage, bBackgroundProcessing);

	pHttpConnection->Close();
	delete pHttpConnection;

	pInternetSession->Close();
	delete pInternetSession;

	return bRet;
}

BOOL CMisc::ReadFileFromConnection(CHttpConnection * pHttpConnection, const CString &sFilename, CString &sContent, BOOL bDisplayErrorMessage /* = TRUE */, bool bBackgroundProcessing /* = false */)
{
	USES_CONVERSION;
	const TCHAR szHeaders[] =
		_T("Accept: text/*\r\nUser-Agent: Hitbase 2010\r\n");

	DWORD dwHttpRequestFlags =
		INTERNET_FLAG_RELOAD | INTERNET_FLAG_NO_AUTO_REDIRECT;

	CHttpFile * pFile = pHttpConnection->OpenRequest(CHttpConnection::HTTP_VERB_GET,
		sFilename, NULL, 1, NULL, NULL, dwHttpRequestFlags);
	pFile->AddRequestHeaders(szHeaders);
	
	try
	{
		pFile->SendRequest();
	}
	catch (CInternetException *e)
	{
		if (bDisplayErrorMessage)
		{
			wchar_t szCause[255];
			e->GetErrorMessage(szCause, 255);
			AfxMessageBox(szCause);
		}

		e->Delete();
		pFile->Close();
		delete pFile;

		return FALSE;
	}

	char str[10000];
	while (pFile->ReadString((LPTSTR)str, 10000))
	{
//		sContent += CString(A2W(str)) + "\r\n";
		sContent += CString(A2W(str));

		if (bBackgroundProcessing)
		{
			if (!DoBackgroundProcessing())
				break;
		}
	}

	pFile->Close();
	delete pFile;

	return TRUE;
}

// Liefert das aktuelle Tagesdatum in der Form (JJJJMMTT) zurück.
CString CMisc::GetDate()
{
	COleDateTime now = COleDateTime::GetCurrentTime();
	CString str;

	str.Format(L"%04d%02d%02d", now.GetYear(), now.GetMonth(), now.GetDay());

	return str;
}

// Vergleicht zwei Pfade miteinander. Liefert TRUE zurück, wenn sie gleich sind.
BOOL CMisc::ComparePathNames(const CString &sPath1, const CString &sPath2)
{
	CString sDir1 = sPath1;
	CString sDir2 = sPath2;

	if (sDir1.Right(1) != L"\\")
		sDir1 += L"\\";
	if (sDir2.Right(1) != L"\\")
		sDir2 += L"\\";

	return !sDir1.CompareNoCase(sDir2);
}

// Liefert das Währungssymbol zurück (z.B. "DM")
CString CMisc::GetCurrencySymbol()
{
	wchar_t szCur[10];

	GetLocaleInfo(LOCALE_USER_DEFAULT, LOCALE_SCURRENCY, szCur, sizeof(szCur));

	return szCur;
}

CString CMisc::FormatCurrencyValue(long lValue)
{
	CString str;
	CString sEuro;

	sEuro.Format(L"%d", lValue/100);
	if (sEuro.GetLength() > 3)
		sEuro.Insert(sEuro.GetLength()-3, L".");
	if (sEuro.GetLength() > 7)
		sEuro.Insert(sEuro.GetLength()-7, L".");

	str.Format(L"%s,%02d %s", sEuro, lValue % 100, GetCurrencySymbol());

	return str;
}

/*CString CMisc::RegQueryString(HKEY hKey, const CString& szSubKey, const CString &sDefault)
{
   wchar_t buff[200];
   DWORD ret;

   wcscpy(buff, sDefault);
   ret = sizeof(buff);
   RegQueryValueEx(hKey, szSubKey, 0, NULL, (unsigned char *)buff, &ret);

   return buff;
}

long CMisc::RegQueryInt(HKEY hKey, const CString& szSubKey, int idefault)
{
   DWORD ret;
   long value;

   value = idefault;
   ret = sizeof(int);
   RegQueryValueEx(hKey, szSubKey, 0, NULL, (unsigned char *)&value, &ret);

   return value;
}

void CMisc::RegWriteString(HKEY hKey, const CString& sSubKey, const CString &sValue)
{
   RegSetValueEx(hKey, sSubKey, 0, REG_SZ, (BYTE *)(LPCTSTR)sValue, sValue.GetLength()+1);
}

void CMisc::RegWriteInt(HKEY hKey, const CString& sSubKey, int iValue)
{
   RegSetValueEx(hKey, sSubKey, 0, REG_DWORD, (BYTE *)&iValue, sizeof(int));
}*/

CString CMisc::GetTime(long ltime)
{
	CString str;
	
	if (ltime < 0L)
		ltime = 0L;
	
	str.Format(L"%02ld:%02ld", ltime/60000L, ltime%60000L/1000L);
	
	return str;
}

CString CMisc::GetPureFileName(const CString& sFilename) 
{
	CString str = sFilename;

	str = GetFileNameFromPath(str);
	CutFileNameExtension(str);

	return str;
}

CString CMisc::BrowseForDirectory(CWnd *pParentWnd, const CString &sTitle, const CString &sInitialDir)
{
	BROWSEINFO bi;
	LPITEMIDLIST ItemIDList;
	CString sDirectory;
	wchar_t szInitialDir[_MAX_PATH];

	wcscpy(szInitialDir, sInitialDir);

	::ZeroMemory(&bi, sizeof(bi));
	bi.hwndOwner = *pParentWnd;
	bi.lpszTitle = sTitle;
	bi.lpfn = BrowseCallbackProc;
	bi.lParam = (LPARAM)&szInitialDir;
	bi.ulFlags = BIF_RETURNONLYFSDIRS|0x40;         // !!!!!!! HACK: BIF_NEWDIALOGSTYLE = 0x40
	
	if ((ItemIDList = SHBrowseForFolder(&bi)))
	{
		SHGetPathFromIDList(ItemIDList, sDirectory.GetBuffer(MAX_PATH));
		sDirectory.ReleaseBuffer();
	}
	else
		return L"";

	return sDirectory;
}

int CALLBACK CMisc::BrowseCallbackProc(HWND hwnd, UINT uMsg, LPARAM lp, LPARAM pData) 
{
	TCHAR szDir[MAX_PATH];
	
	switch(uMsg) 
	{
	case BFFM_INITIALIZED: 
		{
			SendMessage(hwnd, BFFM_SETSELECTION, TRUE, (LPARAM)pData);

			break;
		}

	case BFFM_SELCHANGED: 
		{
			// Set the status window to the currently selected path.
			if (SHGetPathFromIDList((LPITEMIDLIST) lp ,szDir)) 
				SendMessage(hwnd, BFFM_SETSTATUSTEXT, 0, (LPARAM)szDir);
		
			break;
		}

	default:
		break;
	}

	return 0;
}

BOOL CMisc::IsReadOnly(const CString &sFilename)
{
	CFile file;

	if (!file.Open(sFilename, CFile::modeWrite|CFile::shareDenyNone))
		return TRUE;

	file.Close();

	return FALSE;
}

BOOL CMisc::SetWindowTransparenz(CWnd *pWnd, int iTransparenz)
{
	BOOL bRet = FALSE;

	typedef long (CALLBACK* pSetLayeredWindowAttributes)(HWND hwnd, COLORREF crey, BYTE bAlpha, long dwFlags);
	
	HINSTANCE hDLL;               // Handle to DLL
	pSetLayeredWindowAttributes SetLayeredWindowAttributes;    // Function pointer
	
	hDLL = LoadLibrary(L"user32.dll");
	if (hDLL != NULL)
	{
		SetLayeredWindowAttributes = (pSetLayeredWindowAttributes)GetProcAddress(hDLL, "SetLayeredWindowAttributes");
		if (SetLayeredWindowAttributes)
		{
			// Set WS_EX_LAYERED on this window 
			SetWindowLong(*pWnd, GWL_EXSTYLE, GetWindowLong(*pWnd, GWL_EXSTYLE) | 0x80000);
			
			SetLayeredWindowAttributes(*pWnd, 0, 255 * iTransparenz / 100, 0x2);
			bRet = TRUE;
		}
	}
	
	FreeLibrary(hDLL);
	
	return bRet;
}

BOOL CMisc::SetWindowTransparenzColor(CWnd *pWnd, COLORREF bkColor, int iTransparenz)
{
	BOOL bRet = FALSE;

	typedef long (CALLBACK* pSetLayeredWindowAttributes)(HWND hwnd, COLORREF crey, BYTE bAlpha, long dwFlags);
	
	HINSTANCE hDLL;               // Handle to DLL
	pSetLayeredWindowAttributes SetLayeredWindowAttributes;    // Function pointer
	
	hDLL = LoadLibrary(L"user32.dll");
	if (hDLL != NULL)
	{
		SetLayeredWindowAttributes = (pSetLayeredWindowAttributes)GetProcAddress(hDLL, "SetLayeredWindowAttributes");
		if (SetLayeredWindowAttributes)
		{
			// Set WS_EX_LAYERED on this window 
			if (!(GetWindowLong(*pWnd, GWL_EXSTYLE) | 0x80000))
				SetWindowLong(*pWnd, GWL_EXSTYLE, GetWindowLong(*pWnd, GWL_EXSTYLE) | 0x80000);
			
			SetLayeredWindowAttributes(*pWnd, bkColor, 255 * iTransparenz / 100, 0x3);
			bRet = TRUE;
		}
	}
	
	FreeLibrary(hDLL);
	
	return bRet;
}

// Die Versionsnummer eines Moduls ermitteln.
/*CString CMisc::GetFileVersion(const CString& sFilename)
{
	CString sFileVersion;
	char *pFileVersion;
	DWORD dummy;

	int iVersionLen = GetFileVersionInfoSize((LPTSTR)(LPCTSTR)sFilename, &dummy);
	
	wchar_t* pVersion = new wchar_t[iVersionLen];
	GetFileVersionInfo((LPTSTR)(LPCTSTR)sFilename, 0L, iVersionLen, pVersion);

	UINT uiLen = 0;
	VerQueryValue(pVersion, _TEXT("\\StringFileInfo\\040704b0\\FileVersion"), (void **)&pFileVersion, &uiLen);
	
	sFileVersion = pFileVersion;

	delete pVersion;

	return sFileVersion;
}*/

// Wandelt die angegebene Millisekundenzahl in ein Format (HH:MM:SS) um.
CString CMisc::GetLongTimeFormat(long lTime)
{
	CString str;

	if (lTime < 0L)
		lTime = 0L;

	if (lTime/3600000L >= 100L)
		str.Format(L">%02d:%02d:%02d", (lTime/3600000L)%100L, ((lTime%3600000)/60000L)%100L, lTime%60000L/1000L);
	else
		str.Format(L"%02d:%02d:%02d", (lTime/3600000L)%100L, ((lTime%3600000)/60000L)%100L, lTime%60000L/1000L);

	return str;
}


// Liefert das Rect des virtuellen Screens zurück (für multiple monitor configurations)
CRect CMisc::GetVirtualScreenRect()
{
	int iLeft = GetSystemMetrics(SM_XVIRTUALSCREEN);
	int iTop = GetSystemMetrics(SM_YVIRTUALSCREEN);
	int iWidth = GetSystemMetrics(SM_CXVIRTUALSCREEN);
	int iHeight = GetSystemMetrics(SM_CYVIRTUALSCREEN);

	return CRect(CPoint(iLeft, iTop), CSize(iWidth, iHeight));
}

bool CMisc::IsXPVisualStyle()
{
	if (m_iXPVisualStyle == -1)
	{
		m_iXPVisualStyle = 0;
		HINSTANCE hinstDll = LoadLibrary(_T("COMCTL32.DLL"));

		if (hinstDll)
		{
			DLLGETVERSIONPROC pDllGetVersion =
				reinterpret_cast<DLLGETVERSIONPROC>(GetProcAddress(hinstDll,
				"DllGetVersion"));

			// Because some DLLs might not implement this function, you must test for
			// it explicitly. Depending on the particular DLL, the lack of a
			// DllGetVersion function can be a useful indicator of the version.
			if (pDllGetVersion)
			{
				DLLVERSIONINFO dvi;
				ZeroMemory(&dvi, sizeof(DLLVERSIONINFO));
				dvi.cbSize = sizeof(DLLVERSIONINFO);

				if (SUCCEEDED((*pDllGetVersion)(&dvi)) && dvi.dwMajorVersion >= 6)
					m_iXPVisualStyle = 1;
			}

			FreeLibrary(hinstDll);
		}
	}

	if (m_iXPVisualStyle == 0)
		return false;
	else
		return true;
}

// Liefert den kompletten Pfad zurück, für den Speicherort von CD-Covern
CString CMisc::GetCDCoverFilename(const CString& filename)
{
	CString directory;

	if (String::IsNullOrEmpty(Big3::Hitbase::Configuration::Settings::Current->DefaultCDCoverPath))
	{
		directory = CMisc::GetPersonalHitbaseFolder() + "\\Images";
	}
	else
	{
		directory = Big3::Hitbase::Configuration::Settings::Current->DefaultCDCoverPath;
	}

	// Verzeichnis anlegen, muss immer da sein.
	CMisc::CreateDirectory(directory);

	CString coverFilename = CMisc::CombinePathWithFileName(directory, filename);

	return coverFilename;
}

BOOL CMisc::DoBackgroundProcessing()
{
	MSG msg;
	while ( ::PeekMessage( &msg, NULL, 0, 0, PM_NOREMOVE ) ) 
	{ 
		if ( !AfxGetApp()->PumpMessage( ) ) 
		{ 
			::PostQuitMessage(0); 
			return FALSE; 
		} 
	} 
	// let MFC do its idle processing
	LONG lIdle = 0;
	while ( AfxGetApp()->OnIdle(lIdle++ ) )	;  

	return TRUE;
}

void CMisc::RemoveAllExceptAlphaNum(CString& sString)
{
	for (int i=0;i<sString.GetLength();)
	{
		if (!isalnum((unsigned char)sString[i]) && (unsigned char)sString[i] < 127)
			sString.Delete(i);
		else
			i++;
	}
}

CString CMisc::GetPersonalHitbaseFolder(void)
{
	CString sMyDocs;
	SHGetSpecialFolderPath(*AfxGetMainWnd(), sMyDocs.GetBuffer(_MAX_PATH), CSIDL_PERSONAL, FALSE);
	sMyDocs.ReleaseBuffer();

	CString sPath = CombinePathWithFileName(sMyDocs, "Hitbase");

	CreateDirectory(sPath);

	return sPath;
}

void CMisc::DrawGradient(CDC* pDC, CRect& Rect, COLORREF StartColor, COLORREF EndColor, BOOL bHorizontal)
{
	TRIVERTEX vert[2];
	GRADIENT_RECT gRect;

	vert [0].y = Rect.top;
	vert [0].x = Rect.left;

	vert [0].Red    = COLOR16(COLOR16(GetRValue(StartColor))<<8);
	vert [0].Green  = COLOR16(COLOR16(GetGValue(StartColor))<<8);
	vert [0].Blue   = COLOR16(COLOR16(GetBValue(StartColor))<<8);
	vert [0].Alpha  = 0x0000;

	vert [1].y = Rect.bottom;
	vert [1].x = Rect.right;

	vert [1].Red    = COLOR16(COLOR16(GetRValue(EndColor))<<8);
	vert [1].Green  = COLOR16(COLOR16(GetGValue(EndColor))<<8);
	vert [1].Blue   = COLOR16(COLOR16(GetBValue(EndColor))<<8);
	vert [1].Alpha  = 0x0000;

	gRect.UpperLeft  = 0;
	gRect.LowerRight = 1;

	if(bHorizontal)
	{
		pDC->GradientFill(vert,2,&gRect,1,GRADIENT_FILL_RECT_H);
	}
	else
	{
		pDC->GradientFill(vert,2,&gRect,1,GRADIENT_FILL_RECT_V);
	}
}

void CMisc::SplitFilenameArtistTitle(const CString& sFilename, const CString& sDelimiter, CString& sArtist, CString& sTitle)
{
	CString sPureFilename = CMisc::GetPureFileName(sFilename);

	int iPos;

	sArtist = "";
	sTitle = "";

	// Irgendwelche Unix Gurus haben scheinbar was gegen Leerzeichen
	// Das ändern wir jetzt erst mal wieder
	sPureFilename.Replace(L"_", L" ");

	// Die Ermittlung der Titels und des Artist machen wir am besten mal
	// rückwärts...
	int iPos1 = sPureFilename.ReverseFind(sDelimiter[0]);

	sTitle = sPureFilename.Mid(iPos1+1);

	if (iPos1 >= 0)
		sArtist = sPureFilename.Left(iPos1);

	int iPos2 = sArtist.ReverseFind(sDelimiter[0]);

	if (iPos2 > 0)
		sArtist = sArtist.Mid (iPos2 + 1);

	// Zahlen am Anfang eines Titels oders Artist werden
	// gelöscht... Das stimmt zwar nicht immer aber besser als
	// die Zahlen drin lassen
	while (sTitle.FindOneOf(L"0123456789 -") == 0)
	{
		sTitle = sTitle.Mid(1);
	}
	while (sArtist.FindOneOf(L"0123456789 -") == 0)
	{
		sArtist = sArtist.Mid(1);
	}

	// Wenn beim Artist noch ein Komma drin ist den Namen drehen
	if ((iPos = sArtist.Find(',')) > 0)
	{
		CString s1, s2;
		s1 = sArtist.Left(iPos);
		s1.Trim();
		s2 = sArtist.Mid(iPos + 1);
		s2.Trim();
		sArtist = s2 + " " + s1;
	}

	// So jetzt checken wir mal ob da irgendwelche Kommentare
	// noch dranhängen z.B.: Afrika(192 kbit)
	// d.h. alle Sachen beginnend mit '(', '{' und '[' fliegen raus!
	if ((iPos = sTitle.Find('(')) > 0)
		sTitle = sTitle.Mid(0, iPos);
	if ((iPos = sTitle.Find('[')) > 0)
		sTitle = sTitle.Mid(0, iPos);
	if ((iPos = sTitle.Find('{')) > 0)
		sTitle = sTitle.Mid(0, iPos);

	// Zum Schluß mal ein paar Leerzeichen weg...
    sArtist.Trim();
	sTitle.Trim();
}

/*********************************************************************
* Function......: ResolveShortcut
* Parameters....: lpszShortcutPath - string that specifies a path 
                                     and file name of a shortcut
*          lpszFilePath - string that will contain a file name
* Returns.......: S_OK on success, error code on failure
* Description...: Resolves a Shell link object (shortcut)
*********************************************************************/
HRESULT CMisc::ResolveShortcut(/*in*/ LPCTSTR lpszShortcutPath,
                        /*out*/ CString& sFilePath)
{
    HRESULT hRes = E_FAIL;
    CComPtr<IShellLink> ipShellLink;
        // buffer that receives the null-terminated string 
        // for the drive and path
    TCHAR szPath[MAX_PATH];     
        // buffer that receives the null-terminated 
        // string for the description
    TCHAR szDesc[MAX_PATH]; 
        // structure that receives the information about the shortcut
    WIN32_FIND_DATA wfd;    
    WCHAR wszTemp[MAX_PATH];

    // Get a pointer to the IShellLink interface
    hRes = CoCreateInstance(CLSID_ShellLink,
                            NULL, 
                            CLSCTX_INPROC_SERVER,
                            IID_IShellLink,
                            (void**)&ipShellLink); 

    if (SUCCEEDED(hRes)) 
    { 
        // Get a pointer to the IPersistFile interface
        CComQIPtr<IPersistFile> ipPersistFile(ipShellLink);

        // IPersistFile is using LPCOLESTR, 
                // so make sure that the string is Unicode
#if !defined _UNICODE
        MultiByteToWideChar(CP_ACP, 0, lpszShortcutPath,
                                       -1, wszTemp, MAX_PATH);
#else
        wcsncpy(wszTemp, lpszShortcutPath, MAX_PATH);
#endif

        // Open the shortcut file and initialize it from its contents
        hRes = ipPersistFile->Load(wszTemp, STGM_READ); 
        if (SUCCEEDED(hRes)) 
        {
            // Try to find the target of a shortcut, 
                        // even if it has been moved or renamed
            hRes = ipShellLink->Resolve(NULL, SLR_UPDATE); 
            if (SUCCEEDED(hRes)) 
            {
                // Get the path to the shortcut target
                hRes = ipShellLink->GetPath(szPath, 
                                     MAX_PATH, &wfd, SLGP_RAWPATH); 
                if (FAILED(hRes))
                    return hRes;

                // Get the description of the target
                hRes = ipShellLink->GetDescription(szDesc,
                                             MAX_PATH); 
                if (FAILED(hRes))
                    return hRes;

                sFilePath = szPath; 
            } 
        } 
    } 

    return hRes;
}

////////////////////////////////////////////////////////////////////////////
// Main resize function

HBITMAP CMisc::ScaleBitmapInt(HBITMAP hBmp, WORD wNewWidth, WORD wNewHeight)
{
	BITMAP bmp;
	::GetObject(hBmp, sizeof(BITMAP), &bmp);

	// check for valid size
	if((bmp.bmWidth > wNewWidth 
		&& bmp.bmHeight < wNewHeight) 
		|| (bmp.bmWidth < wNewWidth 
		&& bmp.bmHeight > wNewHeight))
		return NULL;

	HDC hDC = ::GetDC(NULL);
	BITMAPINFO *pbi = PrepareRGBBitmapInfo((WORD)bmp.bmWidth, 
		(WORD)bmp.bmHeight);
	BYTE *pData = new BYTE[pbi->bmiHeader.biSizeImage];

	::GetDIBits(hDC, hBmp, 0, bmp.bmHeight, pData, pbi, DIB_RGB_COLORS);

	delete pbi;
	pbi = PrepareRGBBitmapInfo(wNewWidth, wNewHeight);
	BYTE *pData2 = new BYTE[pbi->bmiHeader.biSizeImage];

	if(bmp.bmWidth >= wNewWidth 
		&& bmp.bmHeight >= wNewHeight)
		ShrinkDataInt(pData, 
		(WORD)bmp.bmWidth, 
		(WORD)bmp.bmHeight,
		pData2, 
		wNewWidth, 
		wNewHeight);
	else
		EnlargeDataInt(pData, 
		(WORD)bmp.bmWidth, 
		(WORD)bmp.bmHeight,
		pData2, 
		wNewWidth, 
		wNewHeight);

	delete pData;

	HBITMAP hResBmp = ::CreateCompatibleBitmap(hDC, 
		wNewWidth, 
		wNewHeight);

	::SetDIBits(hDC, 
		hResBmp, 
		0, 
		wNewHeight, 
		pData2, 
		pbi, 
		DIB_RGB_COLORS);

	::ReleaseDC(NULL, hDC);

	delete pbi;
	delete pData2;

	return hResBmp;
}

///////////////////////////////////////////////////////////

BITMAPINFO* CMisc::PrepareRGBBitmapInfo(WORD wWidth, WORD wHeight)
{
	BITMAPINFO *pRes = new BITMAPINFO;
	::ZeroMemory(pRes, sizeof(BITMAPINFO));
	pRes->bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
	pRes->bmiHeader.biWidth = wWidth;
	pRes->bmiHeader.biHeight = wHeight;
	pRes->bmiHeader.biPlanes = 1;
	pRes->bmiHeader.biBitCount = 24;

	pRes->bmiHeader.biSizeImage = 
		((3 * wWidth + 3) & ~3) * wHeight;

	return pRes;
}

///////////////////////////////////////////////////////////

int *CMisc::CreateCoeffInt(int nLen, int nNewLen, BOOL bShrink)
{
	int nSum = 0, nSum2;
	int *pRes = new int[2 * nLen];
	int *pCoeff = pRes;
	int nNorm = (bShrink) 
		? (nNewLen << 12) / nLen : 0x1000;
	int	nDenom = (bShrink)? nLen : nNewLen;

	::ZeroMemory(pRes, 2 * nLen * sizeof(int));
	for(int i = 0; i < nLen; i++, pCoeff += 2)
	{
		nSum2 = nSum + nNewLen;
		if(nSum2 > nLen)
		{
			*pCoeff = ((nLen - nSum) << 12) / nDenom;
			pCoeff[1] = ((nSum2 - nLen) << 12) / nDenom;
			nSum2 -= nLen;
		}
		else
		{
			*pCoeff = nNorm;
			if(nSum2 == nLen)
			{
				pCoeff[1] = -1;
				nSum2 = 0;
			}
		}
		nSum = nSum2;
	}

	return pRes;
}

///////////////////////////////////////////////////////////

void CMisc::ShrinkDataInt(BYTE *pInBuff, WORD wWidth, WORD wHeight,BYTE *pOutBuff, WORD wNewWidth, WORD wNewHeight)
{
	BYTE  *pLine = pInBuff, *pPix;
	BYTE  *pOutLine = pOutBuff;
	DWORD dwInLn = (3 * wWidth + 3) & ~3;
	DWORD dwOutLn = (3 * wNewWidth + 3) & ~3;
	int   x, y, i, ii;
	BOOL  bCrossRow, bCrossCol;
	int   *pRowCoeff = CreateCoeffInt(wWidth, 
		wNewWidth, 
		TRUE);
	int   *pColCoeff = CreateCoeffInt(wHeight, 
		wNewHeight, 
		TRUE);
	int   *pXCoeff, *pYCoeff = pColCoeff;
	DWORD dwBuffLn = 3 * wNewWidth * sizeof(DWORD);
	DWORD *pdwBuff = new DWORD[6 * wNewWidth];
	DWORD *pdwCurrLn = pdwBuff, 
		*pdwCurrPix, 
		*pdwNextLn = pdwBuff + 3 * wNewWidth;
	DWORD dwTmp, *pdwNextPix;

	::ZeroMemory(pdwBuff, 2 * dwBuffLn);

	y = 0;
	while(y < wNewHeight)
	{
		pPix = pLine;
		pLine += dwInLn;

		pdwCurrPix = pdwCurrLn;
		pdwNextPix = pdwNextLn;

		x = 0;
		pXCoeff = pRowCoeff;
		bCrossRow = pYCoeff[1] > 0;
		while(x < wNewWidth)
		{
			dwTmp = *pXCoeff * *pYCoeff;
			for(i = 0; i < 3; i++)
				pdwCurrPix[i] += dwTmp * pPix[i];
			bCrossCol = pXCoeff[1] > 0;
			if(bCrossCol)
			{
				dwTmp = pXCoeff[1] * *pYCoeff;
				for(i = 0, ii = 3; i < 3; i++, ii++)
					pdwCurrPix[ii] += dwTmp * pPix[i];
			}
			if(bCrossRow)
			{
				dwTmp = *pXCoeff * pYCoeff[1];
				for(i = 0; i < 3; i++)
					pdwNextPix[i] += dwTmp * pPix[i];
				if(bCrossCol)
				{
					dwTmp = pXCoeff[1] * pYCoeff[1];
					for(i = 0, ii = 3; i < 3; i++, ii++)
						pdwNextPix[ii] += dwTmp * pPix[i];
				}
			}
			if(pXCoeff[1])
			{
				x++;
				pdwCurrPix += 3;
				pdwNextPix += 3;
			}
			pXCoeff += 2;
			pPix += 3;
		}
		if(pYCoeff[1])
		{
			// set result line
			pdwCurrPix = pdwCurrLn;
			pPix = pOutLine;
			for(i = 3 * wNewWidth; i > 0; i--, pdwCurrPix++, pPix++)
				*pPix = ((LPBYTE)pdwCurrPix)[3];

			// prepare line buffers
			pdwCurrPix = pdwNextLn;
			pdwNextLn = pdwCurrLn;
			pdwCurrLn = pdwCurrPix;
			::ZeroMemory(pdwNextLn, dwBuffLn);

			y++;
			pOutLine += dwOutLn;
		}
		pYCoeff += 2;
	}

	delete [] pRowCoeff;
	delete [] pColCoeff;
	delete [] pdwBuff;
} 

///////////////////////////////////////////////////////////

void CMisc::EnlargeDataInt(BYTE *pInBuff, WORD wWidth, WORD wHeight,BYTE *pOutBuff, WORD wNewWidth, WORD wNewHeight)
{
	BYTE  *pLine = pInBuff, 
		*pPix = pLine, 
		*pPixOld, 
		*pUpPix, 
		*pUpPixOld;
	BYTE  *pOutLine = pOutBuff, *pOutPix;
	DWORD dwInLn = (3 * wWidth + 3) & ~3;
	DWORD dwOutLn = (3 * wNewWidth + 3) & ~3;
	int   x, y, i;
	BOOL  bCrossRow, bCrossCol;
	int   *pRowCoeff = CreateCoeffInt(wNewWidth, 
		wWidth, 
		FALSE);
	int   *pColCoeff = CreateCoeffInt(wNewHeight, 
		wHeight, 
		FALSE);
	int   *pXCoeff, *pYCoeff = pColCoeff;
	DWORD dwTmp, dwPtTmp[3];

	y = 0;
	while(y < wHeight)
	{
		bCrossRow = pYCoeff[1] > 0;
		x = 0;
		pXCoeff = pRowCoeff;
		pOutPix = pOutLine;
		pOutLine += dwOutLn;
		pUpPix = pLine;
		if(pYCoeff[1])
		{
			y++;
			pLine += dwInLn;
			pPix = pLine;
		}

		while(x < wWidth)
		{
			bCrossCol = pXCoeff[1] > 0;
			pUpPixOld = pUpPix;
			pPixOld = pPix;
			if(pXCoeff[1])
			{
				x++;
				pUpPix += 3;
				pPix += 3;
			}

			dwTmp = *pXCoeff * *pYCoeff;

			for(i = 0; i < 3; i++)
				dwPtTmp[i] = dwTmp * pUpPixOld[i];

			if(bCrossCol)
			{
				dwTmp = pXCoeff[1] * *pYCoeff;
				for(i = 0; i < 3; i++)
					dwPtTmp[i] += dwTmp * pUpPix[i];
			}

			if(bCrossRow)
			{
				dwTmp = *pXCoeff * pYCoeff[1];
				for(i = 0; i < 3; i++)
					dwPtTmp[i] += dwTmp * pPixOld[i];
				if(bCrossCol)
				{
					dwTmp = pXCoeff[1] * pYCoeff[1];
					for(i = 0; i < 3; i++)
						dwPtTmp[i] += dwTmp * pPix[i];
				}
			}

			for(i = 0; i < 3; i++, pOutPix++)
				*pOutPix = ((LPBYTE)(dwPtTmp + i))[3];

			pXCoeff += 2;
		}
		pYCoeff += 2;
	}

	delete [] pRowCoeff;
	delete [] pColCoeff;
} 

// end src


