// CDAudioEngine.h: interface for the CCDAudio class.
//
//////////////////////////////////////////////////////////////////////

#pragma once

class CCDExtra;
class CCDText;

#include "SoundEngine.h"

class SOUNDENGINE_INTERFACE CCDAudio : public CSoundEngine
{
public:
	CCDAudio();
	virtual ~CCDAudio();

	static CCDAudio* CreateObject();

	CCDExtra* GetCDExtra();
	CCDText* GetCDText();
	BOOL IsCDExtra();
	BOOL IsCDText();

	BOOL CheckForCDExtra();
	BOOL ReadCDText();

	virtual BOOL CanAddFiles();
	virtual CString GetDeviceName();
	virtual int GetDeviceNumber();
	virtual BOOL IsVirtual();
	virtual BOOL IsPureDataCD();
	virtual BOOL IsMediaPresent();
	virtual BOOL CanEjectTray();
	virtual BOOL OpenTray(BOOL bWait = FALSE);
	virtual BOOL CloseTray(BOOL bWait = FALSE);
	virtual int ReadInformation();
	virtual CString GetMediaIdentity();
	virtual CString GetUPC();
	virtual BOOL StopAll();

	static int GetNumberOfDrives();

protected:
	CString GetDriveFromDeviceID(int id);

	BOOL m_bCDExtra;
	BOOL m_bCDText;
	CCDExtra* m_pCDExtra;
	CCDText* m_pCDText;

	CString m_sMediaIdentity;
	CString m_sUPC;

	int m_iDeviceNumber;

	BOOL m_bPureDataCD;
};

