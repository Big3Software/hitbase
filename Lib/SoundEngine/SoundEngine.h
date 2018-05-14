// SoundEngine1.h: interface for the CSoundEngine class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_SOUNDENGINE1_H__B84906F2_F7EC_11D2_A695_0080AD740CD1__INCLUDED_)
#define AFX_SOUNDENGINE1_H__B84906F2_F7EC_11D2_A695_0080AD740CD1__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

// File formats
#define SEFF_UNKNOWN 0
#define SEFF_MP3     1
#define SEFF_WMA     2
#define SEFF_WAVE    3

// Mögliche Ausgabe-Devices
#define POD_WAVEOUTPUT  1
#define POD_DIRECTSOUND 2

// Virtual CD Track Flags
const int VCTF_VORLAUFZEIT = 1;
const int VCTF_STOPTIME = 2;

// Hitbase virtuelle CD
const CString VIRTUALCDS_EXTENSION = L"hvc";   
const int VIRTUALCDS_VERSION = 1;

class CCDExtra;
class CCDText;

#include "SoundEngineIntern.h"
#include "SoundEngineItem.h"

#include "fmod.hpp"

class SOUNDENGINE_INTERFACE CSoundEngine  
{
public:
	CSoundEngine();
	virtual ~CSoundEngine();

	static bool m_bMp3PlayerInitialized;
	static bool GlobalInit();
	static bool GlobalExit();

	static void* F_API FSoundDSPCallback(void *originalbuffer, void *newbuffer, int length, void* param) ;
	static FMOD::DSP * m_pFSoundDSPUnit;
    static FMOD::System* m_pFMODSystem;

public:
	virtual BOOL IsPlayCompleted();
	virtual BOOL Play(DWORD dwMSStart = 0, DWORD dwMSEnd = 0, BOOL bOverlap = FALSE, CWnd* pWndCallback = NULL);
	virtual BOOL Pause(BOOL bPause = TRUE);
	virtual BOOL Stop();
	virtual BOOL StopAll();
	virtual BOOL Seek(DWORD dwPosition);
	virtual DWORD GetPlayPositionMS();
	virtual int SetOutputDevice(int iOutputDevice);
	virtual int GetNumberOfTracks();
    virtual DWORD GetStartPositionMS();
 	virtual int GetTrackFromPosition(DWORD pos);
    virtual DWORD GetTrackStartPositionMS(int iTrack);
    virtual DWORD GetTrackLengthMS(int iTrack);
    virtual DWORD GetTotalTimeMS(void);
	virtual int GetCurrentTrack();
	virtual BOOL SetSpeed(double dblSpeed);
	virtual BOOL Load(const CString& sFilename, CWnd* pWndParent, bool bSilent);
	virtual BOOL Save(const CString& sFilename);

	virtual CString GetDeviceName();
	virtual void SetDeviceName(const CString& sDeviceName);

	static BOOL WriteMP3Information(const CString& sFilename, const CSoundEngineItemData& MP3Info);

	virtual BOOL IsPlaying();

	CSoundEngineItem* GetTrack(int iTrack);
	CSoundEngineItem* GetCurrentTrackItem() { return m_pSoundEngineItem; }

	static int FillSoundDevicesInComboBox(CComboBox& theComboBox);

	BOOL SetTrackOption(int iTrack, DWORD dwFlag, DWORD dwValue);
	DWORD GetTrackOption(int iTrack, DWORD dwFlag);

	int AddFile(const CString &sFilename, int iInsertAt = -1);
	BOOL DeleteFile(int iIndex);
	BOOL DeleteAllFiles();
	void Sort(int iSortField, BOOL bAscending = TRUE);
	void Move(int iIndex, int iOffset);
	void Shuffle();

	virtual BOOL OpenDevice(int iDeviceNumber);

	virtual BOOL Open(const CString& sDeviceName);
	virtual BOOL IsOpened() = 0;
	virtual BOOL Close();
	virtual BOOL CanAddFiles() = 0;
	virtual int GetDeviceNumber() = 0;
	virtual BOOL IsVirtual() = 0;
	virtual BOOL IsPureDataCD() = 0;
	virtual BOOL IsMediaPresent() = 0;
	virtual BOOL ReadInformation() = 0;
	virtual BOOL OpenTray(BOOL bWait = FALSE) = 0;
	virtual BOOL CloseTray(BOOL bWait = FALSE) = 0;
	virtual BOOL CanEjectTray() = 0;
	virtual BOOL IsCDExtra() = 0;
	virtual BOOL IsCDText() = 0;
	virtual CCDExtra* GetCDExtra() = 0;
	virtual CCDText* GetCDText() = 0;
	virtual CString GetMediaIdentity() = 0;
	virtual CString GetUPC() = 0;

	int m_iOutputDevice;

	void RecalcTrackTimes();

protected:
	CArray <CSoundEngineItem*> m_Tracks;

	CSoundEngineItem* m_pSoundEngineItem;

	int m_nFileFormat;

	int m_dwTotalTime;
	int m_iCurrentTrack;
	BOOL m_bPlaying;

	CString m_sDeviceName;

public:
	BOOL ExportM3U(const CString& sM3UFilename);
	BOOL ImportM3U(const CString& sM3UFilename);
};

#endif // !defined(AFX_SOUNDENGINE1_H__B84906F2_F7EC_11D2_A695_0080AD740CD1__INCLUDED_)
