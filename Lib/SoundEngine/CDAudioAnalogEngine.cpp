// CDAudioEngine.cpp: implementation of the CCDAudioAnalogEngine class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "SoundEngine.h"
#include "CDAudioAnalogEngine.h"
#include "CDAudioItem.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

// Eine Applikation kann jedes Device nur einmal benutzen!
#define MAX_DEVICES 50

class CDeviceList {
public:
   CString m_sDeviceName;
   MCIDEVICEID m_hHandle;
   int m_iAccessCount;
};

static CDeviceList DeviceList[MAX_DEVICES];

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CCDAudioAnalogEngine::CCDAudioAnalogEngine()
{
	m_iDeviceNumber = -1;
	m_bPureDataCD = FALSE;
	m_hCDAudio = NULL;
}

CCDAudioAnalogEngine::~CCDAudioAnalogEngine()
{
}

// Der Dateiname muß hier folgendes Format haben.
// DEVICE=<devicenumber>;TRACK=<tracknumber>
// Die Variable devicenumber und tracknumber sind relativ zu 0
BOOL CCDAudioAnalogEngine::Open(const CString& sFilename)
{
	MCI_OPEN_PARMS mop;
	DWORD dwErrorCode;
	int i, iNumber = -1;
	CString sAlias;
	int iDeviceNumber = -1;
	CString sDeviceName;

	if (!sFilename.Left(7).CompareNoCase(L"DEVICE="))
		iDeviceNumber = _wtoi(sFilename.Mid(7));
	
	for (i=2;i<26;i++)
	{
		sDeviceName.Format(L"%c:", 'A'+i);
		UINT type = GetDriveType(sDeviceName);

		if (type == DRIVE_CDROM)
		{
			iNumber++;
			if (iDeviceNumber == iNumber)     // Device gefunden
				break;
		}
	}
	
	if (iDeviceNumber != iNumber)  // Entweder falsches Device, oder kein CDROM gefunden
		return FALSE;
	
	m_iDeviceNumber = iDeviceNumber;
	
	ASSERT(m_iDeviceNumber >= 0);
	
	memset(&mop, 0 ,sizeof(mop));
	mop.lpstrDeviceType = (LPWSTR)MAKELONG(MCI_DEVTYPE_CD_AUDIO, 0);
	mop.lpstrElementName = sDeviceName;
	sAlias.Format(L"CD%d", m_iDeviceNumber);
	mop.lpstrAlias = sAlias;
	
	// Ist Device schon geöffnet
	for (i=0;i<MAX_DEVICES && !DeviceList[i].m_sDeviceName.IsEmpty() && DeviceList[i].m_sDeviceName != sAlias;i++);
	
	if (DeviceList[i].m_sDeviceName == sAlias)  // Schon vorhanden!
	{
		m_hCDAudio = DeviceList[i].m_hHandle;
		DeviceList[i].m_iAccessCount++;
	}
	else
	{
		if ((dwErrorCode = OwnSendCommand(MCI_OPEN, MCI_OPEN_SHAREABLE|MCI_OPEN_ELEMENT|MCI_OPEN_TYPE|MCI_OPEN_TYPE_ID, (DWORD)&mop)))
		{
			m_hCDAudio = 0;
			
			if (dwErrorCode == MCIERR_INVALID_DEVICE_NAME)
				return FALSE;
			
			if (dwErrorCode == MCIERR_CANNOT_LOAD_DRIVER)   /* Perhaps no audio-CD in the drive */
				return FALSE;
			
	#if _DEBUG   
			wchar_t szBuf[200];
			
			mciGetErrorString(dwErrorCode, szBuf, 200);

			CString str;
			str.Format(L"Open: %s", szBuf);

			AfxMessageBox(str, MB_OK|MB_ICONSTOP);
	#endif   
			return FALSE;
		}
	
		m_hCDAudio = mop.wDeviceID;
		DeviceList[i].m_sDeviceName = sAlias;
		DeviceList[i].m_hHandle = m_hCDAudio;
		DeviceList[i].m_iAccessCount = 1;
	
		Set(MCI_SET_TIME_FORMAT, MCI_FORMAT_MILLISECONDS);         // Alles andere macht keinen Sinn
	}
	
	return TRUE;
}

BOOL CCDAudioAnalogEngine::IsOpened()
{
   if (m_hCDAudio)
      return TRUE;
   else
      return FALSE;
}

BOOL CCDAudioAnalogEngine::Close()
{
	if (!m_hCDAudio)
		return TRUE;

	MCI_GENERIC_PARMS mgp;
	DWORD dwErrorCode;
	
	memset(&mgp, 0, sizeof(mgp));
	
	// Handle aus Device-Tabelle löschen
	int i;
	for (i=0;i<MAX_DEVICES && !DeviceList[i].m_sDeviceName.IsEmpty() && DeviceList[i].m_hHandle != m_hCDAudio;i++);
	ASSERT(DeviceList[i].m_hHandle == m_hCDAudio);
	DeviceList[i].m_iAccessCount--;
	
	if (!DeviceList[i].m_iAccessCount)
	{
		if ((dwErrorCode = OwnSendCommand(MCI_CLOSE, 0, (DWORD)&mgp)))
		{
#if _DEBUG   
			wchar_t szBuf[200];
			
			mciGetErrorString(dwErrorCode, szBuf, 200);
			
			CString str;
			str.Format(L"Close: %s", szBuf);
			
			AfxMessageBox(str, MB_OK|MB_ICONSTOP);
#endif   
			return FALSE;
		}

		if (DeviceList[i].m_hHandle == m_hCDAudio)
		{
			for (int j=i;j<MAX_DEVICES-1;j++)
			{
				DeviceList[j].m_sDeviceName = DeviceList[j+1].m_sDeviceName;
				DeviceList[j].m_hHandle = DeviceList[j+1].m_hHandle;
				DeviceList[j].m_iAccessCount = DeviceList[j+1].m_iAccessCount;
			}
		}
	}
	
	m_hCDAudio = 0;
	return TRUE;
}

BOOL CCDAudioAnalogEngine::Play(DWORD dwMSStart, DWORD dwMSEnd, BOOL bOverlap, CWnd* pWndCallback)
{
	if (pWndCallback != NULL)
		return PlayRange(dwMSStart, dwMSEnd, pWndCallback->m_hWnd);
	else
		return PlayRange(dwMSStart, dwMSEnd, NULL);
}

BOOL CCDAudioAnalogEngine::PlayRange(long lFrom, long lTo, HWND hwndNotify)
{
	long mciret;
	long sort, flags=0L;
	MCI_PLAY_PARMS mpp;
	
	if (lTo < lFrom && lTo != lFrom && lTo)
	{
		sort = lFrom;            /* Second value lower than first! Ring-Tausch */
		lFrom = lTo;
		lTo = sort;
	}
	
	if (lFrom > 0L)
		flags |= MCI_FROM;
	
	if (lTo > 0L)
		flags |= MCI_TO;
	
	if (hwndNotify)
		flags |= MCI_NOTIFY;
	
	memset(&mpp, 0, sizeof(mpp));
	mpp.dwCallback = MAKELONG(hwndNotify, 0); ;
	mpp.dwFrom = lFrom;
	mpp.dwTo = lTo;
	
	mciret = OwnSendCommand(MCI_PLAY, flags, (DWORD)&mpp);
	
#if _DEBUG   
	if (mciret)
	{
		wchar_t szBuf[120];
		mciGetErrorString(mciret, szBuf, sizeof(szBuf));
		CString str;
		str.Format(L"Play: %s", szBuf);

		AfxMessageBox(str, MB_OK|MB_ICONSTOP);
	}
#endif   
	
	return TRUE;
}

BOOL CCDAudioAnalogEngine::Stop()
{
	MCI_GENERIC_PARMS mgp;
	DWORD dwErrorCode;
	
	memset(&mgp, 0, sizeof(mgp));
	
	if ((dwErrorCode = OwnSendCommand(MCI_STOP, 0L, (DWORD)&mgp)))
	{
#if _DEBUG   
		wchar_t aufb[200];
		
		mciGetErrorString(dwErrorCode, aufb, 200);
		CString str;
		str.Format(L"Stop: %s", aufb);

		TRACE(str);
#endif   
		return FALSE;
	}
	else
	{
		if (((CHitbaseWinAppBase*)AfxGetApp())->m_config.m_bResetCDRomOnStop)
		{
			Close();
			OpenDevice(m_iDeviceNumber);
		}
		return TRUE;
	}
}

BOOL CCDAudioAnalogEngine::Pause(BOOL bPause /* = TRUE */)
{
	if (!bPause)
		return PlayRange(0, 0, NULL);

	MCI_GENERIC_PARMS mgp;
	DWORD dwErrorCode;
	
	memset(&mgp, 0, sizeof(mgp));
	
	if ((dwErrorCode = OwnSendCommand(MCI_PAUSE, 0L, (DWORD)&mgp)))
	{
#if _DEBUG   
		wchar_t aufb[200];
		
		mciGetErrorString(dwErrorCode, aufb, 200);
		CString str;
		str.Format(L"Pause: %s", aufb);

		TRACE(str);
#endif   
		return FALSE;
	}
	else
		return TRUE;
}

BOOL CCDAudioAnalogEngine::Seek(DWORD dwPosition)
{ 
	return FALSE;
}

DWORD CCDAudioAnalogEngine::GetPlayPositionMS()
{
    return StatusItem(MCI_STATUS_POSITION);
}

DWORD CCDAudioAnalogEngine::OwnSendCommand(UINT wMessage, DWORD dwParam1, DWORD dwParam2)
{
	DWORD dwRet;
	
	if (m_hCDAudio <= 0 && wMessage != MCI_OPEN)
		return FALSE;        // Entweder kein CD-Rom-Laufwerk gefunden oder CD nicht im Laufwerk
	
	dwRet = mciSendCommand(m_hCDAudio, wMessage, dwParam1, dwParam2);
	
	return dwRet;
}

/*
 * Sets any specific data in the driver.
 */
 
DWORD CCDAudioAnalogEngine::Set(DWORD prop, DWORD value)
{
	MCI_SET_PARMS msp;
	DWORD dwErrorCode;
	
	memset(&msp, 0, sizeof(msp));
	msp.dwTimeFormat = value;
	
	if ((dwErrorCode = OwnSendCommand(MCI_SET, prop, (DWORD)&msp)))
	{
#if _DEBUG   
		wchar_t szBuf[200];
		
		mciGetErrorString(dwErrorCode, szBuf, 200);

		CString str;
		str.Format(L"Set: %s", szBuf);

		TRACE(str);
#endif   
		return -(long)dwErrorCode;
	}
	else
		return 0L;
}


BOOL CCDAudioAnalogEngine::IsPlaying()
{
	if (StatusItem(MCI_STATUS_MODE) == MCI_MODE_PLAY)
		return TRUE;
	else
		return FALSE;
}

BOOL CCDAudioAnalogEngine::IsPlayCompleted()
{
	return IsPlaying();       // Bei Audio-CDs das selbe!
}

/*
 * Returns status information from the current CD.
 * Negative values indicate errors.
 */
 
long CCDAudioAnalogEngine::Status(DWORD info, DWORD item, DWORD track)
{
	MCI_STATUS_PARMS msp;
	DWORD ErrorCode;
	
	memset(&msp, 0, sizeof(msp));
	msp.dwItem = item;
	msp.dwTrack = track;
	
	if ((ErrorCode = OwnSendCommand(MCI_STATUS, info, (DWORD)&msp)))
	{
#if _DEBUG   
		wchar_t aufb[200];
		
		mciGetErrorString(ErrorCode, aufb, 200);
		CString str;
		str.Format(L"Status: %s", aufb);
		TRACE(str);
#endif   
		return -(long)ErrorCode;
	}
	else
		return msp.dwReturn;
}

/*
 * Returns track information from the current CD.
 * Negative values indicate errors.
 */
 
long CCDAudioAnalogEngine::StatusItem(DWORD item)
{
	MCI_STATUS_PARMS msp;
	DWORD ErrorCode;
	
	memset(&msp, 0, sizeof(msp));
	msp.dwItem = item;
	
	if ((ErrorCode = OwnSendCommand(MCI_STATUS, MCI_STATUS_ITEM, (DWORD)&msp)))
	{
#if _DEBUG   
		wchar_t aufb[200];
		
		mciGetErrorString(ErrorCode, aufb, 200);
		CString str;
		str.Format(L"StatusItem: %s", aufb);
		TRACE(str);
#endif   
		return -(long)ErrorCode;
	}
	else
		return msp.dwReturn;
}

/*
 * Returns specified capability of the cdaudio-device
 */

long CCDAudioAnalogEngine::GetDevCaps(DWORD dwFlags, DWORD dwItem)
{
	MCI_GETDEVCAPS_PARMS mgp;
	DWORD ErrorCode;
	
	memset(&mgp, 0, sizeof(mgp));
	mgp.dwItem = dwItem;
	
	if ((ErrorCode = OwnSendCommand(MCI_GETDEVCAPS, dwFlags, (DWORD)&mgp)))
	{
#if _DEBUG   
		wchar_t aufb[200];
		
		mciGetErrorString(ErrorCode, aufb, 200);
		CString str;
		str.Format(L"GetDevCaps: %s", aufb);
		TRACE(str);
#endif   
		return -(long)ErrorCode;
	}
	else
		return mgp.dwReturn;
}

/*
 * Liefert Informationen (z.B. den UPC-Code) zurück.
 */
 
long CCDAudioAnalogEngine::Info(CString& sBuf)
{
	MCI_INFO_PARMS mip;
	DWORD ErrorCode;
	wchar_t buf[200];
	
	memset(&mip, 0, sizeof(mip));
	mip.lpstrReturn = buf;
	mip.dwRetSize = sizeof(buf);;
	
	if ((ErrorCode = OwnSendCommand(MCI_INFO, MCI_INFO_MEDIA_IDENTITY, (DWORD)&mip)))
	{
#if _DEBUG   
		wchar_t aufb[200];
		
		mciGetErrorString(ErrorCode, aufb, 200);
		CString str;
		str.Format(L"Info: %s", aufb);
		TRACE(str);
#endif   
		return -(long)ErrorCode;
	}
	else
	{
		sBuf = buf;

		return 0L;
	}
}

CString CCDAudioAnalogEngine::GetUPCCode()
{
	MCI_INFO_PARMS mip;
	DWORD ErrorCode;
	wchar_t buf[100];
	
	memset(&mip, 0, sizeof(mip));
	mip.lpstrReturn = buf;
	mip.dwRetSize = sizeof(buf);
	
	if ((ErrorCode = OwnSendCommand(MCI_INFO, MCI_INFO_MEDIA_UPC, (DWORD)&mip)))
	{
#if _DEBUG   
		wchar_t aufb[200];
		
		mciGetErrorString(ErrorCode, aufb, 200);
		CString str;
		str.Format(L"GetUPC: %s", aufb);
		TRACE(str);
#endif   
		return L"";
	}
	else
		return buf;
}

BOOL CCDAudioAnalogEngine::OpenTray(BOOL bWait)
{
	DWORD dwRet;

	if (bWait)
		dwRet = Set(MCI_SET_DOOR_OPEN|MCI_WAIT, 0);
	else
		dwRet = Set(MCI_SET_DOOR_OPEN, 0);

	return (dwRet >= 0);
}

BOOL CCDAudioAnalogEngine::CloseTray(BOOL bWait)
{
	DWORD dwRet;

	if (bWait)
		dwRet = Set(MCI_SET_DOOR_CLOSED|MCI_WAIT, 0);
	else
		dwRet = Set(MCI_SET_DOOR_CLOSED, 0);

	return (dwRet >= 0);
}

BOOL CCDAudioAnalogEngine::OpenCDTray(int iDevice)
{
	CCDAudioAnalogEngine thecdengine;
	thecdengine.OpenDevice(iDevice);
	thecdengine.OpenTray(TRUE);
	thecdengine.Close();

	return TRUE;
}

BOOL CCDAudioAnalogEngine::CloseCDTray(int iDevice)
{
	CCDAudioAnalogEngine thecdengine;
	thecdengine.OpenDevice(iDevice);
	thecdengine.CloseTray(TRUE);
	thecdengine.Close();

	return TRUE;
}

/*
 * Liest alle vorhandenen Informationen von der CD.
 * Liefert FALSE zurück, wenn mindestens ein Lied der CD kein AUDIO-Track ist.
 */

BOOL CCDAudioAnalogEngine::ReadInformation()
{
	CWaitCursor wait;

	m_Tracks.RemoveAll();

	int iNumberOfTracks = StatusItem(MCI_STATUS_NUMBER_OF_TRACKS);
	int iTotalTime = StatusItem(MCI_STATUS_LENGTH);
	
	if (iTotalTime < 0 || iNumberOfTracks < 0)
	{
		iNumberOfTracks = 0;
		iTotalTime = 0;
		return 0;
	}
	
	m_bPureDataCD = TRUE;

	m_dwTotalTime = 0;

	for (int i=0;i<iNumberOfTracks;i++)
	{
		CCDAudioItem* pNewItem = new CCDAudioItem;
		
		int startTime = Status(MCI_STATUS_ITEM|MCI_TRACK, MCI_STATUS_POSITION, i+1);
		
		pNewItem->m_dwStartTime = startTime;

		// Die Funktion MCI_STATUS_LENGTH liefert manchmal eine Millisekunde zuviel zurück
		// (vermutlich rundungsprobleme). Deshalb berechnen wir uns die Länge der Tracks
		// selbst (außer beim letzten Track, da geht es nicht anders).
		if (i == iNumberOfTracks-1)
		{
			pNewItem->m_dwLength = Status(MCI_STATUS_ITEM|MCI_TRACK, MCI_STATUS_LENGTH, i+1);
		}
		else
		{	
			int startTimeNext;
			startTimeNext = Status(MCI_STATUS_ITEM|MCI_TRACK, MCI_STATUS_POSITION, i+2);
			pNewItem->m_dwLength = startTimeNext - startTime;
		}

		m_dwTotalTime += pNewItem->m_dwLength;

		if (Status(MCI_STATUS_ITEM|MCI_TRACK, MCI_CDA_STATUS_TYPE_TRACK, i+1) != MCI_CDA_TRACK_OTHER)
			m_bPureDataCD = FALSE;
		m_Tracks.SetAtGrow(i, pNewItem);
	}


	Info(m_sMediaIdentity);

	m_sUPC = GetUPCCode();

	return CCDAudio::ReadInformation();
}

BOOL CCDAudioAnalogEngine::IsMediaPresent()
{
    return StatusItem(MCI_STATUS_MEDIA_PRESENT);
}

BOOL CCDAudioAnalogEngine::CanEjectTray()
{
	return (GetDevCaps(MCI_GETDEVCAPS_ITEM, MCI_GETDEVCAPS_CAN_EJECT) > 0);
}


BOOL CCDAudioAnalogEngine::IsPureDataCD()
{
	int iNumberOfTracks = StatusItem(MCI_STATUS_NUMBER_OF_TRACKS);
	
	BOOL bPureDataCD = TRUE;

	for (int i=0;i<iNumberOfTracks;i++)
	{
		if (Status(MCI_STATUS_ITEM|MCI_TRACK, MCI_CDA_STATUS_TYPE_TRACK, i+1) == MCI_CDA_TRACK_AUDIO)
			bPureDataCD = FALSE;
	}

	return bPureDataCD;
}

CString CCDAudioAnalogEngine::GetDeviceName()
{
	return GetDriveFromDeviceID(m_iDeviceNumber);
}

int CCDAudioAnalogEngine::GetDeviceNumber()
{
	return m_iDeviceNumber;
}

