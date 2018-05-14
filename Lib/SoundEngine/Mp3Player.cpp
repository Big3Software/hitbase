// Mp3Player.cpp : Implementierungsdatei
//

#include "stdafx.h"
#include "SoundEngineIntern.h"
#include "Mp3Engine.h"
#include "Mp3Player.h"
#include "SoundEngine.h"
#include "../hitmisc/fourier.h"

// CMp3Player

CMp3Player::CMp3Player()
{
	m_bPlayCompleted = true;
	m_pSoundStream = NULL;
	m_dwExactDuration = -1;
	m_pSoundChannel = NULL;
	m_bPauseActive = false;
	m_pMp3Engine = NULL;
}

CMp3Player::~CMp3Player()
{
}

// CMp3Player-Meldungshandler

bool CMp3Player::Open(const CString& sFilename)
{
	USES_CONVERSION;
	if (!CSoundEngine::GlobalInit())
		return false;

	// JUS 06.04.2003
	//FSOUND_Stream_SetBufferSize(1000);

	FMOD_RESULT res = CSoundEngine::m_pFMODSystem->createStream(W2A(sFilename), FMOD_OPENONLY, 0, &m_pSoundStream);
	if (res != FMOD_OK)
	{
		AfxMessageBox(CString(FMOD_ErrorString(res)));

		return false;
	}

	res = m_pSoundStream->getLength((UINT*)&m_dwExactDuration, FMOD_TIMEUNIT_MS);
	if (m_pMp3Engine)
	{ 
		// Jetzt die virtuelle CD benachrichtigen, dass wir die exakte Lied-Länge haben!
		m_pMp3Engine->m_dwLength = m_dwExactDuration;

		FMOD_SOUND_TYPE soundType;
		FMOD_SOUND_FORMAT soundFormat;
		int channels;
		int bits;
		m_pSoundStream->getFormat(&soundType, &soundFormat, &channels, &bits);
		if (channels == 1)
			m_pMp3Engine->m_iChannels = 1;
		else
			m_pMp3Engine->m_iChannels = 2;

		if (soundFormat & FMOD_SOUND_FORMAT_PCM8)
			m_pMp3Engine->m_iBitsPerSample = 1;
		else
			m_pMp3Engine->m_iBitsPerSample = 0;

		if (m_pMp3Engine->m_pSoundEngine)
			m_pMp3Engine->m_pSoundEngine->RecalcTrackTimes();
	}

	// ==========================================================================================
	// SET AN END OF STREAM CALLBACK AND RIFF SYNCH POINTS CALLBACK
	// ==========================================================================================
	
	//!!!!!!!!!!!!!!!!!CSoundEngine::m_pFSoundDSPUnit = m_pFMODSystem->createDSP(m_stream, &CSoundEngine::FSoundDSPCallback, 0, (void*)this);

	return true;
}

// ==========================================================================================
// PLAY STREAM
// ==========================================================================================
bool CMp3Player::Play(int startPosition)
{
	if (m_bPauseActive)
	{
		Pause(false);
		return true;
	}

	FMOD_RESULT res;
	if ((res = CSoundEngine::m_pFMODSystem->playSound(FMOD_CHANNEL_FREE, m_pSoundStream, true, &m_pSoundChannel)) != FMOD_OK)
	{
		AfxMessageBox(CString(FMOD_ErrorString(res)));
		return false;
	}

	res = m_pSoundChannel->setCallback(EndCallback);

	m_pSoundChannel->setUserData(this);

	res = m_pSoundChannel->setPosition(startPosition, FMOD_TIMEUNIT_MS);

	m_pSoundChannel->setPaused(false);

	m_bPlayCompleted = false;

	return true;
}

bool CMp3Player::Close()
{
	if (m_pSoundChannel != NULL)
	{
		m_pSoundChannel->stop();
		m_pSoundChannel = NULL;
	}

	if (m_pSoundStream != NULL)
	{
		m_pSoundStream->release();
		m_pSoundStream = NULL;
	}

	return true;
}

bool CMp3Player::Stop()
{
	m_pSoundChannel->stop();
	m_pSoundChannel = NULL;

	m_pSoundStream->release();
	m_pSoundStream = NULL;

	m_bPauseActive = false;
	m_bPlayCompleted = true;

	return true;
}

bool CMp3Player::Pause(bool bPause)
{
	m_pSoundChannel->setPaused(bPause);
	m_bPauseActive = bPause;

	return true;
}

bool CMp3Player::Seek(DWORD dwPosition)
{
	FMOD_RESULT res = m_pSoundChannel->setPosition(dwPosition, FMOD_TIMEUNIT_MS);

	return true;
}

DWORD CMp3Player::GetCurrentPosition()
{
	if (!m_pSoundStream)
		return 0;

	CSoundEngine::m_pFMODSystem->update();

	UINT pos;
	m_pSoundChannel->getPosition(&pos, FMOD_TIMEUNIT_MS);
	return pos;
}

bool CMp3Player::IsPlaying()
{
	return !m_bPlayCompleted;
}

FMOD_RESULT F_CALLBACK CMp3Player::EndCallback(FMOD_CHANNEL *channel, FMOD_CHANNEL_CALLBACKTYPE type, void* commanddata1, void* commanddata2)
{
	if (type == FMOD_CHANNEL_CALLBACKTYPE_END)
	{
		FMOD::Channel* pFMODChannel = (FMOD::Channel*)channel;

		CMp3Player* pThePlayer = NULL;;
		pFMODChannel->getUserData((void**)&pThePlayer);

		pThePlayer->m_bPlayCompleted = true;
	}

	return FMOD_OK;
}

BOOL CMp3Player::SetSpeed(double dblSpeed)
{
	m_pSoundChannel->setFrequency((float)dblSpeed);
	//FSOUND_SetFrequency(m_iChannel, (int)(44100.0*dblSpeed));
	return TRUE;
}

BOOL CMp3Player::SetVolume(double dblVolume)
{
	return TRUE;
}

BOOL CMp3Player::SetAutoVolumeAdjust(BOOL bAutoVolumeAdjust)
{
//!!!!!!!!!!!!!!!!!!	FSOUND_DSP_SetActive(CSoundEngine::m_pFSoundDSPUnit, bAutoVolumeAdjust);

	return TRUE;
}

