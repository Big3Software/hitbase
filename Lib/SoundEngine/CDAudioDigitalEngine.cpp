// CDAudioDigitalEngine.cpp: implementation of the CCDAudioDigitalEngine class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "SoundEngineIntern.h"
#include "CDAudioDigitalEngine.h"
#include "CDAudioItem.h"
#include "../hitmisc/HitbaseWinAppBase.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CCDAudioDigitalEngine::CCDAudioDigitalEngine()
{
	m_pSoundStream = NULL;
	m_pSoundChannel = NULL;
	m_bPauseActive = false;
	m_iCurrentTrack = 0;
	m_bPlaying = false;
}

CCDAudioDigitalEngine::~CCDAudioDigitalEngine()
{
}

// Der Dateiname muﬂ hier folgendes Format haben.
// DEVICE=<devicenumber>;TRACK=<tracknumber>
// Die Variable devicenumber und tracknumber sind relativ zu 0
BOOL CCDAudioDigitalEngine::Open(const CString& sDeviceName)
{
	if (!GlobalInit())
		return false;

	int i, iNumber = -1;
	CString sAlias;
	int iDeviceNumber = -1;
	CString sDrive;

	if (!sDeviceName.Left(7).CompareNoCase(L"DEVICE="))
		iDeviceNumber = _wtoi(sDeviceName.Mid(7));
	
	for (i=2;i<26;i++)
	{
		sDrive.Format(L"%c:", 'A'+i);
		UINT type = GetDriveType(sDrive);

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

	m_sDeviceName = GetDriveFromDeviceID(m_iDeviceNumber);

	return TRUE;
}

BOOL CCDAudioDigitalEngine::Close()
{
	m_pSoundStream->release();

	m_pSoundStream = NULL;

	m_bPlaying = false;
	m_bPauseActive = false;

	return TRUE;
}

BOOL CCDAudioDigitalEngine::Play(DWORD dwMSStart, DWORD dwMSEnd, BOOL bOverlap, CWnd* pWndCallback)
{
	USES_CONVERSION;

	if (m_bPauseActive)
	{
		Pause();
		return TRUE;
	}

	if (dwMSStart == 0)
		dwMSStart = GetTrackStartPositionMS(0);

	if (IsPlaying())
	{
		Seek(dwMSStart);
		return TRUE;
	}

	if (!m_pSoundStream)
	{
		// JUS 06.04.2003
		CSoundEngine::m_pFMODSystem->setStreamBufferSize(64*1024, FMOD_TIMEUNIT_RAWBYTES);

	    FMOD_RESULT result = CSoundEngine::m_pFMODSystem->createStream(W2A(m_sDeviceName), FMOD_OPENONLY, 0, &m_pSoundStream);

		if (result != FMOD_OK)
		{
			AfxMessageBox(CString(FMOD_ErrorString(result)));

			return false;
		}

		// ==========================================================================================
		// SET AN END OF STREAM CALLBACK AND RIFF SYNCH POINTS CALLBACK
		// ==========================================================================================
		//!!!!!!!!!!!!!FSOUND_Stream_SetEndCallback(m_stream, EndCallback, (void*)this);
		//!!!!!!!!!!!!!FSOUND_Stream_SetSyncCallback(m_stream, EndCallback, (void*)this);
	}

	//!!!!!!!!!!!CSoundEngine::m_pFSoundDSPUnit = FSOUND_Stream_CreateDSP(m_stream, &FSoundDSPCallback, 0, (void*)this);

	m_iCurrentTrack = GetTrackFromPosition(dwMSStart);

	FMOD_RESULT res;

/*	if (res != FMOD_OK)
	{
		AfxMessageBox(CString(FMOD_ErrorString(res)));
		return false;
	}*/

	if ((res = CSoundEngine::m_pFMODSystem->playSound(FMOD_CHANNEL_FREE, m_pSoundStream, true, &m_pSoundChannel)) != FMOD_OK)
	{
		AfxMessageBox(CString(FMOD_ErrorString(res)));
		return false;
	}

	Seek(dwMSStart);
	//m_pSoundChannel->setPosition(dwMSStart, FMOD_TIMEUNIT_MS);

	m_pSoundChannel->setPaused(false);

	m_pSoundChannel->setCallback(&EndCallback);

	m_bPlaying = true;

	return TRUE;
}

BOOL CCDAudioDigitalEngine::PlayRange(long lFrom, long lTo, HWND hwndNotify)
{
	return TRUE;
}

BOOL CCDAudioDigitalEngine::Pause(BOOL bPause /* = TRUE */)
{
	if (!m_bPauseActive)
	{
		m_pSoundChannel->setPaused(TRUE);
		m_bPauseActive = true;
	}
	else
	{
		m_pSoundChannel->setPaused(FALSE);
		m_bPauseActive = false;
	}

	return TRUE;
}

BOOL CCDAudioDigitalEngine::Stop()
{
	TRACE0("CCDAudioDigitalEngine::Stop\n");

	m_pSoundChannel->stop();

	Close();

	m_bPlaying = false;
	m_bPauseActive = false;

	return TRUE;
}

BOOL CCDAudioDigitalEngine::Seek(DWORD dwPosition)
{ 
	// FMOD scheint bei setPosition immer bei 0 zu beginnen, auch wenn die Start-Position von Track 0 z.B. 2000 ist.
	m_pSoundChannel->setPosition(dwPosition - GetStartPositionMS(), FMOD_TIMEUNIT_MS);

	return TRUE;
}

BOOL CCDAudioDigitalEngine::IsPlaying()
{
	if (m_pSoundChannel != NULL)
	{
		bool isPlaying;
		m_pSoundChannel->isPlaying(&isPlaying);

		return isPlaying ? TRUE : FALSE;
	}

	return FALSE;
}

BOOL CCDAudioDigitalEngine::IsOpened()
{
   if (m_pSoundStream)
      return TRUE;
   else
      return FALSE;
}

// Ermittelt, ob eine CD im Laufwerk liegt.
BOOL CCDAudioDigitalEngine::IsMediaPresent()
{
	USES_CONVERSION;
	FMOD::Sound* tempSound;
	BOOL bOpened = IsOpened();

	TRACE0("CCDAudioDigitalEngine::IsMediaPresent\n");

	if (!bOpened)
	{
	    FMOD_RESULT result = CSoundEngine::m_pFMODSystem->createStream(W2A(m_sDeviceName), FMOD_OPENONLY|FMOD_IGNORETAGS, 0, &tempSound);

		if (result != FMOD_OK)
		{
			tempSound->release();
			return FALSE;
		}

		m_pSoundStream = tempSound;
	}
	else
	{
		FMOD_OPENSTATE openState;
		m_pSoundStream->getOpenState(&openState, NULL, NULL);

		if (openState != FMOD_OPENSTATE_READY)
			return FALSE;
	}

/*	int iNumberOfTracks = 0;
	tempSound->getNumSubSounds(&iNumberOfTracks);

	if (!bOpened)
		tempSound->release();

	if (iNumberOfTracks < 1)
	{
		return FALSE;			// Wohl keine CD im Laufwerk
	}*/

	return TRUE;

//   if (m_stream)
//      return TRUE;
//   else
//      return FALSE;
}

DWORD CCDAudioDigitalEngine::GetPlayPositionMS()
{
	if (!IsPlaying())
		return 0;

	if (!IsOpened())
		return 0;

	CSoundEngine::m_pFMODSystem->update();

	UINT pos;
	m_pSoundChannel->getPosition(&pos, FMOD_TIMEUNIT_MS);

	pos += GetStartPositionMS();

//	return m_Tracks[m_iCurrentTrack]->m_dwStartTime + pos;
	return pos;
}

BOOL CCDAudioDigitalEngine::IsPlayCompleted()
{
	return !m_bPlaying;
}

BOOL CCDAudioDigitalEngine::OpenTray(BOOL bWait)
{
	USES_CONVERSION;

//!!!!!!!!!!!!!!!!!!!!!!!	if (!m_sDeviceName.IsEmpty())
//		return FSOUND_CD_Eject(W2A(m_sDeviceName)[0]);
//	else
		return FALSE;
}

BOOL CCDAudioDigitalEngine::CloseTray(BOOL bWait)
{
	USES_CONVERSION;
//!!!!!!!!!!!!!!!!!!!!!!!!	if (!m_sDeviceName.IsEmpty())
//		return FSOUND_CD_Eject(W2A(m_sDeviceName)[0]);
//	else
		return FALSE;
}

CString CCDAudioDigitalEngine::GetIdentity()
{
	return L"";
}

BOOL CCDAudioDigitalEngine::CanEjectTray()
{
	return TRUE;							// Kˆnnen wir immer
}

BOOL CCDAudioDigitalEngine::ReadInformation()
{
	USES_CONVERSION;

	FMOD::Sound* pTempCDSound;

	FMOD_RESULT result = CSoundEngine::m_pFMODSystem->createStream(W2A(m_sDeviceName), FMOD_OPENONLY, 0, &pTempCDSound);

	if (result != FMOD_OK)
	{
		AfxMessageBox(CString(FMOD_ErrorString(result)));

		return FALSE;
	}

	int iNumberOfTracks = 0;
	pTempCDSound->getNumSubSounds(&iNumberOfTracks);

	if (iNumberOfTracks < 1)
	{
		pTempCDSound->release();

		return FALSE;			// Wohl keine CD im Laufwerk
	}

	m_bPureDataCD = FALSE;

	m_dwTotalTime = 0;

	FMOD_CDTOC* cdToc = NULL;
    for (;;)
    {
        FMOD_TAG tag;

        if (pTempCDSound->getTag(0, -1, &tag) != FMOD_OK)
        {
            break;
        }
        if (tag.datatype == FMOD_TAGDATATYPE_CDTOC)
        {
			cdToc = (FMOD_CDTOC*)tag.data;
        }
    }

	if (cdToc != NULL)
	{
		int i;
		for (i=0;i<iNumberOfTracks;i++)
		{
			FMOD::Sound* pTrackSound;
			FMOD_RESULT res = pTempCDSound->getSubSound(i, &pTrackSound);
			UINT dwTrackLength = 0;
			pTrackSound->getLength(&dwTrackLength, FMOD_TIMEUNIT_MS);

			CCDAudioItem* pNewItem = new CCDAudioItem;
			pNewItem->m_dwStartTime = (((cdToc->min[i] * 60 + cdToc->sec[i]) * 75 + cdToc->frame[i]) * 1000 + 74) / 75;
			pNewItem->m_dwLength = dwTrackLength;

			m_Tracks.SetAtGrow(i, pNewItem);

			pTrackSound->release();
		}

		// Die Frames werden hier ignoriert
		int t = ((cdToc->min[i] * 60 + cdToc->sec[i]) * 75) - ((cdToc->min[0] * 60 + cdToc->sec[0]) * 75);
		m_dwTotalTime = (t * 1000 + 74) / 75;
	}

	// INFO ID einlesen

	wchar_t szVolumeName[_MAX_PATH];
	wchar_t szFileSystemNameBuffer[_MAX_PATH];
	DWORD dwSerialNumber, dwMaximumComponentLength, dwFileSystemFlags;
	CString sDrive = GetDriveFromDeviceID(m_iDeviceNumber);
	sDrive += "\\";

	GetVolumeInformation(sDrive, szVolumeName, sizeof(szVolumeName), &dwSerialNumber, &dwMaximumComponentLength, &dwFileSystemFlags, szFileSystemNameBuffer, sizeof(szFileSystemNameBuffer));

	m_sMediaIdentity.Format(L"%d", dwSerialNumber);

	pTempCDSound->release();

//	return CCDAudio::ReadInformation();
	//!!!!!!!!!! TODO!! Wenn ich hier ReadInformation aufrufe, klappen danach die FMOD-Routinen nicht mehr (CD-Text)
	return TRUE;
}

BOOL CCDAudioDigitalEngine::IsPureDataCD()
{
	return FALSE;
}

FMOD_RESULT F_CALLBACK CCDAudioDigitalEngine::EndCallback(FMOD_CHANNEL *channel, FMOD_CHANNEL_CALLBACKTYPE type, void* commanddata1, void* commanddata2)
{
	//CCDAudioDigitalEngine* pThePlayer = (CCDAudioDigitalEngine*)command;

//	pThePlayer->m_bPlaying = false;

//	if (pThePlayer->m_iCurrentTrack < pThePlayer->GetNumberOfTracks()-1)
//		pThePlayer->Play(pThePlayer->m_Tracks[pThePlayer->m_iCurrentTrack+1]->m_dwStartTime, 0, 0);

	return FMOD_OK;
}

BOOL CCDAudioDigitalEngine::SetSpeed(double dblSpeed)
{
	FMOD::ChannelGroup* channelGroup = NULL;
	m_pSoundChannel->getChannelGroup(&channelGroup);
	channelGroup->setPitch(dblSpeed);
	return TRUE;
}

