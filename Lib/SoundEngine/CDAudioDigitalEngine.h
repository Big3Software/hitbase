// CDAudioEngine.h: interface for the CCDAudioDigitalEngine class.
//
//////////////////////////////////////////////////////////////////////

#pragma once

#include "CDAudio.h"

class SOUNDENGINE_INTERFACE CCDAudioDigitalEngine : public CCDAudio
{
public:
	CCDAudioDigitalEngine();
	virtual ~CCDAudioDigitalEngine();

public:
	BOOL IsPureDataCD();
	BOOL CanEjectTray();
	CString GetIdentity();
	BOOL CloseTray(BOOL bWait);
	BOOL OpenTray(BOOL bWait);
	static BOOL CloseCDTray(int iDevice);
	static BOOL OpenCDTray(int iDevice);
	BOOL IsPlaying();
	BOOL IsPlayCompleted();
	BOOL IsOpened();
	BOOL IsMediaPresent();
	BOOL Open(const CString& sDeviceName);
	BOOL Close();
	BOOL Play(DWORD dwMSStart, DWORD dwMSEnd, BOOL bOverlap, CWnd* pWndCallback);
	BOOL PlayRange(long lFrom, long lTo, HWND hwndNotify);
	BOOL Pause(BOOL bPause = TRUE);
	BOOL Stop();
	BOOL Seek(DWORD dwPosition);
	DWORD GetPlayPositionMS();
	BOOL ReadInformation();
	BOOL SetSpeed(double dblSpeed);

protected:
	FMOD::Sound* m_pCDSound;
	FMOD::Sound* m_pSoundStream;
	FMOD::Channel* m_pSoundChannel;
	bool m_bPauseActive;

private:
	static FMOD_RESULT F_CALLBACK EndCallback(FMOD_CHANNEL *channel, FMOD_CHANNEL_CALLBACKTYPE type, void* commanddata1, void* commanddata2);
};
