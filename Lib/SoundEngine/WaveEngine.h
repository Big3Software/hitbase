// WaveEngine.h: interface for the CWaveEngine class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_WAVEENGINE_H__B84906EE_F7EC_11D2_A695_0080AD740CD1__INCLUDED_)
#define AFX_WAVEENGINE_H__B84906EE_F7EC_11D2_A695_0080AD740CD1__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

class CWavePlay;
class CWavePlayServices;

#include "SoundEngineItem.h"

class CWaveEngine : public CSoundEngineItem
{
public:
	CWaveEngine();
	virtual ~CWaveEngine();

	// Interface
public:
	BOOL IsPlayCompleted();
	BOOL IsPlaying();
	static CWaveEngine* CreateFromFile(const CString& sFilename);
	BOOL Open();
	BOOL Close();
	BOOL Play(DWORD dwMSStart, DWORD dwMSEnd);
	BOOL Pause(BOOL bPause = TRUE);
	BOOL Stop();
	BOOL Seek(DWORD dwPosition);
	int GetTotalTime();
	CString GetArtist();
	CString GetTitle();
	CString GetComment();
	DWORD GetPlayPositionMS();

	BOOL SetSpeed(double dblSpeed);
	BOOL SetVolume(double dblVolume);

	BOOL SaveToFile();

	int GetChannels();
	int GetBitRate();
	int GetSampleRate();

private:
	CWavePlay* m_pWavePlay;
	CWavePlayServices* m_pAudioServices;
};

#endif // !defined(AFX_WAVEENGINE_H__B84906EE_F7EC_11D2_A695_0080AD740CD1__INCLUDED_)
