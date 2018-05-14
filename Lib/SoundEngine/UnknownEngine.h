#pragma once
#include "soundengineitem.h"

class CUnknownEngine :
	public CSoundEngineItem
{
public:
	CUnknownEngine(void);
	~CUnknownEngine(void);

	BOOL IsPlaying();
	BOOL IsPlayCompleted();
	BOOL Open();
	BOOL Close();
	BOOL Play(DWORD dwMSStart, DWORD dwMSEnd);
	BOOL Pause(BOOL bPause = TRUE);
	BOOL Stop();
	BOOL Seek(DWORD dwPosition);
	DWORD GetPlayPositionMS();
};
