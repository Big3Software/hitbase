// WMAEngine.h: interface for the CWMAEngine class.
//
//////////////////////////////////////////////////////////////////////

#pragma once

class CWMAPlay;
struct IWMReader;

#include "SoundEngineItem.h"

class SOUNDENGINE_INTERFACE CWMAEngine : public CSoundEngineItem
{
public:
	CWMAEngine();
	virtual ~CWMAEngine();

public:
	static void GetAvailableFormatList(CStringArray& saFormat);
	static HRESULT GetCodecNames(CStringArray &saFormat);
	BOOL IsPlayCompleted();
	BOOL IsPlaying();
	static CWMAEngine* CreateFromFile(const CString& sFilename);
	BOOL Open();
	BOOL Close();
	BOOL Play(DWORD dwMSStart, DWORD dwMSEnd);
	BOOL Pause(BOOL bPause = TRUE);
	BOOL Stop();
	BOOL Seek(DWORD dwPosition);
	DWORD GetPlayPositionMS();

	BOOL SetSpeed(double dblSpeed);
	BOOL SetVolume(double dblVolume);

	BOOL SaveToFile();

protected:
	CWMAPlay* m_pWMAPlay;

	static int PlayThread(LPVOID dwData);

	BOOL m_bPlayCompleted;
	HANDLE m_hStartEvent;

	DWORD m_dwMSStart;
	DWORD m_dwMSEnd;
};
