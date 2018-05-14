// WavePlay.cpp
//
// CWavePlay class implementation
//
////////////////////////////////////////////////////////////

#include <stdafx.h>
#include "WavePlay.h"

// Constructor
CWavePlay::CWavePlay (void)
{
    TRACE0 ("CWavePlay::CWavePlay\n\r");

	m_mciHandle = 0;
}


// Destructor
CWavePlay::~CWavePlay (void)
{
    TRACE0 ("CWavePlay::~CWavePlay\n\r");

	// Schließen, wenn Device noch offen!!
	Close();
}

// Open
BOOL CWavePlay::Open(const CString& sFilename)
{
    TRACE0("CWavePlay::Open\n\r");

	MCI_WAVE_OPEN_PARMS mop;
	memset(&mop, 0, sizeof(mop));
	mop.lpstrDeviceType = L"waveaudio";
	mop.lpstrElementName = sFilename;
	mop.dwBufferSeconds = 5;            // vielleicht später einstellbar machen!!??

	if (mciSendCommand(0, MCI_OPEN, MCI_OPEN_ELEMENT|MCI_OPEN_TYPE|MCI_WAVE_OPEN_BUFFER , (DWORD)&mop))
		return FALSE;

	m_mciHandle = mop.wDeviceID;

	// Jetzt noch das Zeitformat auf Millisekunden stellen!
	MCI_SET_PARMS msp;
	memset(&msp, 0, sizeof(msp));
	msp.dwTimeFormat = MCI_FORMAT_MILLISECONDS;
	if (mciSendCommand(m_mciHandle, MCI_SET, MCI_SET_TIME_FORMAT, (DWORD)&msp))
		return FALSE;

	return TRUE;
}

// Close
BOOL CWavePlay::Close()
{
    TRACE0("CWavePlay::Close\n\r");

	if (!m_mciHandle)
		return TRUE;

	MCI_GENERIC_PARMS mgp;
	memset(&mgp, 0, sizeof(mgp));
	if (mciSendCommand(m_mciHandle, MCI_CLOSE, MCI_WAIT, (DWORD)&mgp))
		return FALSE;

	m_mciHandle = 0;

	return TRUE;
}

// Play
BOOL CWavePlay::Play (DWORD dwMSStart, DWORD dwMSEnd)
{
    TRACE0("CWavePlay::Play\n\r");

	MCI_PLAY_PARMS mpp;
	memset(&mpp, 0, sizeof(mpp));
	mpp.dwFrom = dwMSStart;
	mpp.dwTo = dwMSEnd;

	DWORD dwFlags=0;
	if (dwMSStart > 0L)
		dwFlags |= MCI_FROM;
	
	if (dwMSEnd > 0L)
		dwFlags |= MCI_TO;
	
	if (mciSendCommand(m_mciHandle, MCI_PLAY, dwFlags, (DWORD)&mpp))
		return FALSE;

	return TRUE;
}

// Stop
BOOL CWavePlay::Stop (void)
{
    TRACE0("CWavePlay::Stop\n\r");

	MCI_GENERIC_PARMS mgp;
	memset(&mgp, 0, sizeof(mgp));

	if (mciSendCommand(m_mciHandle, MCI_STOP, 0, (DWORD)&mgp))
		return FALSE;

	Close();

	return TRUE;
}

// Pause
void CWavePlay::Pause (BOOL bPause)
{
    TRACE0("CWavePlay::Pause\n\r");

	if (!bPause)
	{
		Play(0, 0);
		return;
	}

	MCI_GENERIC_PARMS mgp;
	
	memset(&mgp, 0, sizeof(mgp));

	if (mciSendCommand(m_mciHandle, MCI_PAUSE, 0, (DWORD)&mgp))
		return;

	m_bPause = bPause;
}

// GetDuration
DWORD CWavePlay::GetDuration (void)
{
    TRACE0("CWavePlay::GetDuration\n\r");

	MCI_STATUS_PARMS msp;
	memset(&msp, 0, sizeof(msp));
	msp.dwItem = MCI_STATUS_LENGTH;

	if (mciSendCommand(m_mciHandle, MCI_STATUS, MCI_STATUS_ITEM, (DWORD)&msp))
		return FALSE;

	return msp.dwReturn;
}

// IsPlaying
BOOL CWavePlay::IsPlaying(void)
{
    TRACE0("CWavePlay::IsPlaying\n\r");

	MCI_STATUS_PARMS msp;
	memset(&msp, 0, sizeof(msp));
	msp.dwItem = MCI_STATUS_MODE;

	if (mciSendCommand(m_mciHandle, MCI_STATUS, MCI_STATUS_ITEM, (DWORD)&msp))
		return FALSE;

	if (msp.dwReturn == MCI_MODE_PLAY)
		return TRUE;
	else
		return FALSE;
}

// GetPlayPosition
DWORD CWavePlay::GetPlayPosition(void)
{
    TRACE0("CWavePlay::GetPlayPosition\n\r");

	MCI_STATUS_PARMS msp;
	memset(&msp, 0, sizeof(msp));
	msp.dwItem = MCI_STATUS_POSITION;

	if (mciSendCommand(m_mciHandle, MCI_STATUS, MCI_STATUS_ITEM, (DWORD)&msp))
		return 0;

	return msp.dwReturn;
}

 