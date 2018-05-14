// CCDAudioItem.h: interface for the CCDAudioItem class.
//
//////////////////////////////////////////////////////////////////////

#pragma once

#include "SoundEngineItem.h"

class SOUNDENGINE_INTERFACE CCDAudioItem : public CSoundEngineItem
{
public:
	CCDAudioItem();
	virtual ~CCDAudioItem();

	virtual BOOL IsPlayCompleted();
	virtual BOOL Open();
	virtual BOOL Close();
	virtual BOOL Play(DWORD dwMSStart, DWORD dwMSEnd);
	virtual BOOL Pause(BOOL bPause = TRUE);
	virtual BOOL Stop();
	virtual BOOL Seek(DWORD dwPosition);
	virtual int GetTotalTime();
	virtual DWORD GetPlayPositionMS();
	virtual BOOL IsPlaying();

	virtual BOOL SaveToFile() { return TRUE; }

	BOOL SetSpeed(double dblSpeed);
	BOOL SetVolume(double dblVolume);
};
