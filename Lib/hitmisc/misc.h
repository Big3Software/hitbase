// hitmisc.h : header file
//

#pragma once

class CColorBitmap;
class CSoundEngine;
class CHttpConnection;
class CInternetSession;

class HITMISC_INTERFACE CMisc
{
public:
	CMisc();
	~CMisc();

public:
	static BOOL SetWindowTransparenzColor(CWnd *pWnd, COLORREF bkColor, int iTransparenz);
	static BOOL SetWindowTransparenz(CWnd* pWnd, int iTransparenz);
	static BOOL IsReadOnly(const CString& sFilename);
	static CString BrowseForDirectory(CWnd* pParentWnd, const CString& sTitle, const CString& sInitialDir);
	static CString GetPureFileName(const CString& sFilename);
	static CString GetCurrencySymbol();
	static CString GetFileNameExtension(const CString& sFilename);
	static CString CombinePathWithFileName(const CString& path, const CString& filename);
	static BOOL ComparePathNames(const CString& sPath1, const CString& sPath2);
	static CString GetDate();
	static BOOL ReadFileFromURL(const CString& sServer, const CString& sFilename, CString &sContent, BOOL bDisplayErrorMessage = TRUE, bool bBackgroundProcessing = false);
	static BOOL ReadFileFromConnection(CHttpConnection * pHttpConnection, const CString &sFilename, CString &sContent, BOOL bDisplayErrorMessage = TRUE, bool bBackgroundProcessing = false);
	static BOOL OpenHTTPConnection(const CString& sServer, CInternetSession** pInternetSession, CHttpConnection** pHTTPConnection, BOOL bDisplayErrorMessage = TRUE);
	static void OpenURL(CWnd* pWnd, const CString& sURL);
	static CString ConvertToHTTP(const CString& sStr);
	static CString GetSQLDateString(const COleDateTime& date);
	static CString GetLocalDate(const COleDateTime& date);
	static CString GetTempFilename();
	static void FilterFilenameChars(CString& sFilename);
	static void Comma2Point(CString& sValue);
	static void Point2Comma(CString& sValue);
	static BOOL CreateDirectory(const CString& sDirectory);
	static CString Long2Time(long ltime);
	static void FillComboBoxWithCDDrives(CComboBox *cb, BOOL bIncludeVirtualDrives = FALSE);
	static void GetCDDriveList(CStringArray& saDrives, BOOL bIncludeVirtualDrives = FALSE);
	static CString GetCDCoverFilename(const CString& filename);

	/*static CString RegQueryString(HKEY hKey, const CString& szSubKey, const CString &sDefault);
	static long RegQueryInt(HKEY hKey, const CString& szSubKey, int idefault);
	static void RegWriteString(HKEY hKey, const CString& sSubKey, const CString &sValue);
	static void RegWriteInt(HKEY hKey, const CString& sSubKey, int iValue);
*/
	static int MessageResBox(CWnd* pWnd, UINT resid, UINT nType, ...);

	static CString GetTime(long ltime);

//	static CString GetFileVersion(const CString& sFilename);
	static CString GetLongTimeFormat(long lTime);

	static CRect GetVirtualScreenRect();

	static HBITMAP ScaleBitmapInt(HBITMAP hBmp, WORD wNewWidth, WORD wNewHeight);

	static void CutLength(CDC *pDC, CString& str, int cx);
	static bool IsXPVisualStyle();

	static BOOL DoBackgroundProcessing();

	static void SqlPrepare(CString &str);
	static CString GetSqlString(const CString &str);

	static CString FormatCurrencyValue(long lValue);

	static void RemoveAllExceptAlphaNum(CString& sString);

	static int m_iXPVisualStyle;

	static CString GetPersonalHitbaseFolder(void);

	static void SplitFilenameArtistTitle(const CString& sFilename, const CString& sDelimiter, CString& sArtist, CString& sTitle);

	static HRESULT ResolveShortcut(/*in*/ LPCTSTR lpszShortcutPath, /*out*/ CString& sFilePath);

	static void CutFileNameExtension(CString &filename);

	static void DrawGradient(CDC* pDC, CRect& Rect, COLORREF StartColor, COLORREF EndColor, BOOL bHorizontal);

protected:
	static int CALLBACK BrowseCallbackProc(HWND hwnd, UINT uMsg, LPARAM lp, LPARAM pData);

	static int *CreateCoeffInt(int nLen, int nNewLen, BOOL bShrink);
	static void ShrinkDataInt(BYTE *pInBuff, WORD wWidth, WORD wHeight,BYTE *pOutBuff, WORD wNewWidth, WORD wNewHeight);
	static void EnlargeDataInt(BYTE *pInBuff, WORD wWidth, WORD wHeight,BYTE *pOutBuff, WORD wNewWidth, WORD wNewHeight);
	static BITMAPINFO* PrepareRGBBitmapInfo(WORD wWidth, WORD wHeight);
};

void HITMISC_INTERFACE WarningBox(const char *format, ...);
void HITMISC_INTERFACE ErrorBox(CString format, ...);
void HITMISC_INTERFACE WarningResBox(UINT resid, ...);
void HITMISC_INTERFACE ErrorResBox(CWnd* pWnd, UINT resid, ...);
int HITMISC_INTERFACE MessageResBox(UINT resid, UINT nType, ...);
void HITMISC_INTERFACE DebugMessage(CString fmt, ...);
void HITMISC_INTERFACE NoMemoryMessage( void );

int HITMISC_INTERFACE GetGraphicsFormat(CString FileName);
void HITMISC_INTERFACE DrawStretchBitmap(CDC *pdc, CBitmap *pBitmap, CRect rect);
void HITMISC_INTERFACE DrawBitmap(CDC *pdc, CBitmap *pBitmap, int xStart, int yStart, CDC* pDCSelect = NULL);
void HITMISC_INTERFACE Draw3DSunken(CDC *pdc, int x, int y, int cy, int cx);
void HITMISC_INTERFACE Draw3DSunken(CDC *pdc, CRect rect);
void HITMISC_INTERFACE ConvertRGBToDevice(COLORREF rgb, int& blue,  int maxblue, 
                                      int& green, int maxgreen,
                                      int& red,   int maxred);

CString HITMISC_INTERFACE long2mstime(long ltime);
long HITMISC_INTERFACE time2long(const CString& stime);
long HITMISC_INTERFACE mstime2long(const CString& stime);

int HITMISC_INTERFACE Star(char *s, char *p);
int HITMISC_INTERFACE wildmat(char *s, char *p);

int HITMISC_INTERFACE check_serialnumber(void);
CString HITMISC_INTERFACE GetDate(void);
CString HITMISC_INTERFACE GetRealDate(void);

int HITMISC_INTERFACE GetRandom(int min, int max);

char HITMISC_INTERFACE * GetFileNameFromPath(char *filename);
CString HITMISC_INTERFACE GetFileNameFromPath(const CString& filename);
char HITMISC_INTERFACE * GetPathFromFileName(char *filename);
CString HITMISC_INTERFACE GetPathFromFileName(const CString& filename);
//void HITMISC_INTERFACE CutFileNameExtension(char *filename);
CString HITMISC_INTERFACE get_string(int id);
CString HITMISC_INTERFACE GetFormattedNumber(DWORD nr);

void HITMISC_INTERFACE PlaySoundEvent(int Event);

#define EVENT_PLAY       0
#define EVENT_PAUSE      1
#define EVENT_STOP       2
#define EVENT_OPENDOOR   3
#define EVENT_CLOSEDOOR  4
#define EVENT_NEWCD      5
#define EVENT_TRACKP     6
#define EVENT_TRACKM     7
#define EVENT_FF         8
#define EVENT_REW        9
#define EVENT_START      10
#define EVENT_END        11

void HITMISC_INTERFACE FillButtonStruct(TBBUTTON *tb, int iBitmap, int idCommand, BOOL bSeparator = FALSE);
void HITMISC_INTERFACE SetUserFieldControlText(CStatic *WndStatic, CEdit *WndEdit, const char *text);
int HITMISC_INTERFACE German_stricmp(const CString& left, const CString& right);
int HITMISC_INTERFACE GetDeviceIDFromDrive(int drive);
CString HITMISC_INTERFACE GetDriveFromDeviceID(int id);
void HITMISC_INTERFACE FillCDSamplerComboBox(CComboBox* pComboBox);

CString HITMISC_INTERFACE DateShort2Long(CString sDate);

CString HITMISC_INTERFACE GetSharewareString(void);

HBITMAP HITMISC_INTERFACE LoadResourceBitmap(LPCTSTR lpString, HPALETTE* lphPalette);
HPALETTE HITMISC_INTERFACE CreateDIBPalette (LPBITMAPINFO lpbmi, LPINT lpiNumColors);
void HITMISC_INTERFACE DrawPaletteBitmapResource(HDC hdc, LPCTSTR lpRes, short xStart, short yStart);
void HITMISC_INTERFACE DrawPaletteBitmap(CDC* pDC, CBitmap* pBitmap, CPalette* pPalette, int x, int y);

CString HITMISC_INTERFACE GetTextFromSeconds(int nSeconds);

/////////////////////////////////////////////////////////////////////////////

