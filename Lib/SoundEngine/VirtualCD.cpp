// VirtualCD.cpp: implementation of the CVirtualCD class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "SoundEngine.h"
#include "VirtualCD.h"
#include <io.h>

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CVirtualCD::CVirtualCD()
{
	m_iCurrentTrack = 0;
	m_bPlaying = FALSE;
}

CVirtualCD::~CVirtualCD()
{
}

BOOL CVirtualCD::Open(const CString& sDeviceName)
{
	m_sDeviceName = sDeviceName;

	return TRUE;
}

BOOL CVirtualCD::IsOpened()
{
	return TRUE;
}

BOOL CVirtualCD::Close()
{
	return TRUE;
}

// Diese Funktion ist notwendig, weil ich die genaue Zeit von MP3s erst beim Abspielen
// ermitteln kann!
void CVirtualCD::SetNewTrackTime(CSoundEngineItem* pSoundEngineItem, DWORD dwNewTotalTime)
{
	if (pSoundEngineItem->m_dwLength != dwNewTotalTime)
	{
		pSoundEngineItem->m_dwLength = dwNewTotalTime;

		RecalcTrackTimes();
	}
}

// Berechnet eine "Pseudo"-Media-Identity aus der virtuellen CD. Ist für MP3-CDs nötig, die
// in der Datenbank gespeichert werden sollen.
// Die Identity ist wie folgt aufgebaut: "Maaabbbbccddee"
// aaa = Anzahl Lieder MOD 1000
// bbbb = Gesamtlänge der Lieder in Millisekunden MOD 10000
// cc,dd,ee = Jeweils Länge von Lied 1, 2, 3 in Sekunden MOD 100 ("00", wenn Lied nicht vorhanden)
CString CVirtualCD::GetMediaIdentity()
{
	int iTrackCount = m_Tracks.GetSize();
	int iTotalLength = 0;

	for (int i=0;i<m_Tracks.GetSize();i++)
	{
		iTotalLength += m_Tracks[i]->m_dwLength;
	}

	int iTrack1Length=0;
	int iTrack2Length=0;
	int iTrack3Length=0;

	if (m_Tracks.GetSize() >= 1)
		iTrack1Length = m_Tracks[0]->m_dwLength/1000;
	if (m_Tracks.GetSize() >= 2)
		iTrack2Length = m_Tracks[1]->m_dwLength/1000;
	if (m_Tracks.GetSize() >= 3)
		iTrack3Length = m_Tracks[2]->m_dwLength/1000;

	CString sIdentity;
	sIdentity.Format(L"M%03d%04d%02d%02d%02d", iTrackCount % 1000, iTotalLength % 10000, iTrack1Length % 100, iTrack2Length % 100, iTrack3Length % 100);

	return sIdentity;
}

CString CVirtualCD::GetUPC()
{
	return L"";
}

BOOL CVirtualCD::CanAddFiles()
{
	return TRUE;
}

int CVirtualCD::GetDeviceNumber()
{
	if (m_sDeviceName.GetLength() > 0)
		return GetDeviceIDFromDrive(m_sDeviceName[0]);

	return -1;
}

BOOL CVirtualCD::IsVirtual()
{
	return TRUE;
}

BOOL CVirtualCD::IsMediaPresent()
{
	if (m_sDeviceName.IsEmpty())
		return TRUE;
	else
	{
		// Prüfen, ob auf das Verzeichnis zugegriffen werden kann.
		if (_waccess(m_sDeviceName, 0))
			return FALSE;
		else
			return TRUE;
	}
}

CString CVirtualCD::GetIdentity(void)
{
	return GetMediaIdentity();
}

BOOL CVirtualCD::ReadInformation()
{
	return TRUE;
}

BOOL CVirtualCD::CanEjectTray()
{
	return TRUE;
}

BOOL CVirtualCD::OpenTray(BOOL bWait)
{
	return TRUE;
}

BOOL CVirtualCD::CloseTray(BOOL bWait)
{
	return TRUE;
}

BOOL CVirtualCD::IsCDExtra()
{
	return FALSE;
}

BOOL CVirtualCD::IsCDText()
{
	return FALSE;
}

CCDExtra* CVirtualCD::GetCDExtra()
{
	return NULL;
}

CCDText* CVirtualCD::GetCDText()
{
	return NULL;
}

BOOL CVirtualCD::IsPureDataCD()
{
	return FALSE;
}

// Liefert TRUE zurück, wenn es sich um ein unterstütztes Sound-Format handelt.
// Dabei wird nur die Extension geprüft.
BOOL CVirtualCD::IsSupportedFileType(const CString &sFilename)
{
	CString sExt = CMisc::GetFileNameExtension(sFilename);
	sExt.MakeLower();

	if (sExt == ".mp3" || sExt == ".wav" || sExt == ".wma" || sExt == ".asf" || sExt == ".ogg" ||
		sExt == ".mp2" || sExt == ".mpg" || sExt == ".mpeg" || sExt == ".ogm" || sExt == ".mp4" || sExt == ".lnk" ||
		sExt == ".flac")
		return TRUE;

	return FALSE;
}

