#pragma once

#include "fmod.hpp"
#include "fmod_errors.h"	// optional

// CMp3Player

class CMp3Engine;

class CMp3Player
{
public:
	CMp3Player();           // Dynamische Erstellung verwendet geschützten Konstruktor
	virtual ~CMp3Player();

public:
	virtual bool Open(const CString& sFilename);
	virtual bool Play(int startPosition);
	virtual bool Stop();
	virtual bool Pause(bool bPause);
	virtual bool Seek(DWORD dwPosition);
	virtual bool Close();
	virtual bool IsPlaying();

	virtual DWORD GetCurrentPosition();

	BOOL SetSpeed(double dblSpeed);
	BOOL SetVolume(double dblVolume);
	BOOL SetAutoVolumeAdjust(BOOL bAutoVolumeAdjust);

	DWORD m_dwExactDuration;
	bool m_bPlayCompleted;

	CMp3Engine* m_pMp3Engine;

protected:
	FMOD::Sound* m_pSoundStream;
	FMOD::Channel* m_pSoundChannel;
	bool m_bPauseActive;

private:
	static FMOD_RESULT F_CALLBACK EndCallback(FMOD_CHANNEL *channel, FMOD_CHANNEL_CALLBACKTYPE type, void* commanddata1, void* commanddata2);
};


