// SoundEngine1.h: interface for the CSoundEngineItem class.
//
//////////////////////////////////////////////////////////////////////

#pragma once

#include "SoundEngineItemData.h"

class CSoundEngine;

class SOUNDENGINE_INTERFACE CSoundEngineItem : public CSoundEngineItemData
{
public:
	CSoundEngineItem(CSoundEngine* pSoundEngine = NULL);
	virtual ~CSoundEngineItem();
	static CSoundEngineItem* CreateFromFile(const CString& sFilename);

public:
	virtual BOOL IsPlayCompleted() = 0;
	virtual BOOL Open() = 0;
	virtual BOOL Close() = 0;
	virtual BOOL Play(DWORD dwMSStart, DWORD dwMSEnd) = 0;
	virtual BOOL Pause(BOOL bPause = TRUE) = 0;
	virtual BOOL Stop() = 0;
	virtual BOOL Seek(DWORD dwPosition) = 0;
	virtual int GetTotalTime() { return m_dwLength; }
	virtual DWORD GetPlayPositionMS() = 0;
	virtual BOOL IsPlaying() = 0;

	virtual BOOL SetSpeed(double dblSpeed) = 0;
	virtual BOOL SetVolume(double dblVolume) = 0;

	virtual BOOL SaveToFile() = 0;			// z.B. ID3-Tags bei MP3-Dateien

	CSoundEngine* m_pSoundEngine;

	double m_dblSpeed;
	double m_dblVolume;

	BOOL m_bAutoVolumeAdjust;
	double m_dblAutoVolumeAdjustValue;

	virtual void SetAutoVolumeAdjust(BOOL bAutoVolumeAdjust) { m_bAutoVolumeAdjust = bAutoVolumeAdjust; }
	virtual BOOL GetAutoVolumeAdjust() { return m_bAutoVolumeAdjust; }
	virtual void SetAutoVolumeAdjustValue(double dblAutoVolumeAdjustValue);
	virtual double GetAutoVolumeAdjustValue() { return m_dblAutoVolumeAdjustValue; }
};
