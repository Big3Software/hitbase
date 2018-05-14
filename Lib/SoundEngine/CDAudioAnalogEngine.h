// CDAudioEngine.h: interface for the CCDAudioAnalogEngine class.
//
//////////////////////////////////////////////////////////////////////

#pragma once

#include "CDAudio.h"

class CCDExtra;
class CCDText;

class SOUNDENGINE_INTERFACE CCDAudioAnalogEngine : public CCDAudio
{
public:
	CCDAudioAnalogEngine();
	virtual ~CCDAudioAnalogEngine();

public:
	int GetDeviceNumber();
	CString GetDeviceName();
	BOOL IsPureDataCD();
	BOOL CanEjectTray();
	CString GetIdentity();
	BOOL IsMediaPresent();
	BOOL IsOpened();
	BOOL CloseTray(BOOL bWait);
	BOOL OpenTray(BOOL bWait);
	static BOOL CloseCDTray(int iDevice);
	static BOOL OpenCDTray(int iDevice);
	BOOL IsPlaying();
	BOOL IsPlayCompleted();
	BOOL Open(const CString& sDeviceName);
	BOOL Close();
	BOOL Play(DWORD dwMSStart, DWORD dwMSEnd, BOOL bOverlap, CWnd* pWndCallback);
	BOOL PlayRange(long lFrom, long lTo, HWND hwndNotify);
	BOOL Pause(BOOL bPause = TRUE);
	BOOL Stop();
	BOOL Seek(DWORD dwPosition);
	DWORD GetPlayPositionMS();
	int ReadInformation();

protected:
    MCIDEVICEID m_hCDAudio;

private:
	DWORD Set(DWORD prop, DWORD value);
	DWORD OwnSendCommand(UINT wMessage, DWORD dwParam1, DWORD dwParam2);

	long Status(DWORD info, DWORD item, DWORD track);
	long StatusItem(DWORD item);
	long Info(CString& sBuf);
	long GetDevCaps(DWORD dwFlags, DWORD dwItem);
	CString GetUPCCode();
};
