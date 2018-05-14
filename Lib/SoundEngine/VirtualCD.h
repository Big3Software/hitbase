// VirtualCD.h: interface for the CVirtualCD class.
//
//////////////////////////////////////////////////////////////////////

#pragma once

class CHitbaseView;

#include "soundengine.h"

class SOUNDENGINE_INTERFACE CVirtualCD : public CSoundEngine
{
public:
	CVirtualCD();
	virtual ~CVirtualCD();

public:
    long Info(char *buf, int maxlen);

public:
	virtual void SetNewTrackTime(CSoundEngineItem* pSoundEngineItem, DWORD dwNewTotalTime);
	void SetVirtualDirectory(const CString& sVirtualDirectory);

	static BOOL IsSupportedFileType(const CString &sFilename);

	virtual BOOL Open(const CString& sDeviceName);
	virtual BOOL IsOpened();
	virtual BOOL Close();
	virtual BOOL ReadInformation();
	virtual BOOL CanAddFiles();
	virtual int GetDeviceNumber();
	virtual BOOL IsVirtual();
	virtual BOOL IsPureDataCD();
	virtual BOOL IsMediaPresent();
	virtual CString GetIdentity();
	virtual BOOL CanEjectTray();
	virtual BOOL OpenTray(BOOL bWait = FALSE);
	virtual BOOL CloseTray(BOOL bWait = FALSE);
	virtual BOOL IsCDExtra();
	virtual BOOL IsCDText();
	virtual CCDExtra* GetCDExtra();
	virtual CCDText* GetCDText();

//	BOOL SetSpeed(double dblSpeed);
	double GetSpeed();

	BOOL SetVolume(double dblSpeed);
	double GetVolume();

protected:
	CString GetMediaIdentity();
	CString GetUPC();
};

struct tagTimerThreadData
{
	CHitbaseView* pView;
	HDC hdc;
};

UINT TimerThreadProc(LPVOID pParam);
