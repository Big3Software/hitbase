// CDRipper.h: Interface vor Class CCDRipper.
//
//////////////////////////////////////////////////////////////////////

#pragma once

#include <akrip32.h>

/*
 * WAV file header format
 */
typedef struct
{
  BYTE  riff[4];            /* must be "RIFF"                */
  DWORD len;                /* #bytes + 44 - 8               */
  BYTE  cWavFmt[8];         /* must be "WAVEfmt"             */
  DWORD dwHdrLen;
  WORD  wFormat;
  WORD  wNumChannels;
  DWORD dwSampleRate;
  DWORD dwBytesPerSec;
  WORD  wBlockAlign;
  WORD  wBitsPerSample;
  BYTE  cData[4];            /* must be "data"               */
  DWORD dwDataLen;           /* #bytes                       */
} PACKED AKRIP_WAVHDR, *AKRIP_PWAVHDR, *AKRIP_LPWAVHDR;

// events fired by ReadTrack()
#define CDRIPPER_READ_START    1
#define CDRIPPER_READ_END      2
#define CDRIPPER_READ_DATA     3
#define CDRIPPER_READ_CANCEL   4

typedef int (CALLBACK* CDRIPPERCALLBACKPROC)(int iState, BYTE* pData, int iDataLen, LPARAM lParam);

// The CCDRipper class
class CCDRipper  
{
public:
	CCDRipper();
	virtual ~CCDRipper();

	virtual bool Open(int iDeviceNumber);
	virtual bool Close();

	virtual bool IsOpened();
	virtual bool IsMediaPresent();

	virtual bool ReadInformation();

	virtual DWORD ReadTrack(int iTrack, int iStartMS, int iEndMS, CDRIPPERCALLBACKPROC pCallbackProc, LPARAM lParam);
	virtual DWORD ReadTrackEx(int iTrack, int iStartMS, int iEndMS, CDRIPPERCALLBACKPROC pCallbackProc, LPARAM lParam);
	virtual int GetNumberOfTracks();
	virtual long GetTrackLength(int iTrack);
	virtual void GetLastErrorCodes(int& iErrorCode, int& iASPIErrorCode);
	virtual long GetTrackLengthFrames(int iTrack);
	virtual DWORD GetCDRomList(CStringArray& saCDList);
	virtual bool GetDeviceInfo(int iDevice, CString& sDevice, int& iHA, int& iTgt, int& iLun);
	virtual int GetDeviceCount();
	virtual DWORD QueryDeviceParam(int iParam, bool bBoolean = true);

	virtual void SetJitterMode(int iMode, int iJitterReadTracks, int iJitterCheck, int iJitterOverlap);
	virtual void Stop(BOOL bWait = TRUE);

private:
	static UINT ReadTrackControllingThread(LPVOID pParam);
	void WriteWavHeader(CFile& file, DWORD len);
	void MSB2DWORD(DWORD *d, BYTE *b);
	LPTRACKBUF NewTrackBuf(DWORD numFrames);

	CDRIPPERCALLBACKPROC m_pCallbackProc;
	LPARAM m_ReadTrackLParam;

	HCDROM m_hCDRom;
	TOC m_TOC;

	BOOL m_bCancelRipping;
	int m_iJitterMode;
	int m_iJitterReadTracks;
	int m_iJitterCheck;
	int m_iJitterOverlap;

	int m_iReadStartMS;
	int m_iReadLengthMS;
	int m_iReadTrack;
	CWinThread* m_pRipThread;
};
